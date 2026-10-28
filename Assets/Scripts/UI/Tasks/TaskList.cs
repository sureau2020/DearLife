using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TaskList : MonoBehaviour
{
    [Header("Prefab / Pool")]
    [SerializeField] private GameObject taskPrefab; // 用于在池不足时实例化
    [Tooltip("可在 Inspector 放入已有的 8 个 GameObject 作为初始池")]
    [SerializeField] private List<GameObject> pooledItems = new List<GameObject>();

    [Header("Scroll Settings")]
    [SerializeField] private ScrollRect scrollRect; // 必须在 Inspector 绑定
    [SerializeField] private RectTransform content; // 可留空，将自动使用 scrollRect.content
    [SerializeField] private int desiredPoolSize = 8; // 池大小（您提到 8 个）

    private List<MissionData> currentTasks = new List<MissionData>();
    private float itemHeight = 0f;
    private int totalCount = 0;
    private int topIndex = 0; // content 首个可见项索引（用于池重绑）
    private bool initialized = false;

    private void Awake()
    {
        if (scrollRect == null)
            scrollRect = GetComponentInChildren<ScrollRect>(true);

        if (content == null && scrollRect != null)
            content = scrollRect.content;

        InitializePool();
    }

    private void OnEnable()
    {
        if (TaskManager.Instance != null)
        {
            TaskManager.Instance.OnTaskListUpdated += UpdateTaskList;
        }
        if (scrollRect != null)
        {
            scrollRect.onValueChanged.AddListener(OnScrollChanged);
        }
    }

    private void OnDisable()
    {
        if (TaskManager.Instance != null)
        {
            TaskManager.Instance.OnTaskListUpdated -= UpdateTaskList;
        }
        if (scrollRect != null)
        {
            scrollRect.onValueChanged.RemoveListener(OnScrollChanged);
        }
    }

    private void InitializePool()
    {
        if (content == null)
        {
            Debug.LogError("TaskList: Content 未绑定，请在 Inspector 指定 ScrollRect/Content。");
            return;
        }

        // 保证 pooledItems 至少 desiredPoolSize 个，若 Inspector 已放入现成的 8 个则不会重复创建
        for (int i = pooledItems.Count; i < desiredPoolSize; i++)
        {
            if (taskPrefab == null)
            {
                Debug.LogError("TaskList: taskPrefab 未设置，无法补足对象池。");
                break;
            }
            var go = Instantiate(taskPrefab, content);
            go.name = $"PooledTask_{i}";
            pooledItems.Add(go);
        }

        // 父节点与Rect配置（统一项的锚点 / pivot 便于布局控制）
        foreach (var go in pooledItems)
        {
            if (go == null) continue;
            var rt = go.GetComponent<RectTransform>();
            if (rt == null) continue;
            rt.SetParent(content, false);
            // 设置为顶端布局，宽度跟 content 一致，高度由 prefab 决定
            rt.anchorMin = new Vector2(0, 1);
            rt.anchorMax = new Vector2(1, 1);
            rt.pivot = new Vector2(0.5f, 1f);
        }

        // 读取 itemHeight（使用第一个有效池元素）
        foreach (var go in pooledItems)
        {
            if (go == null) continue;
            var rt = go.GetComponent<RectTransform>();
            if (rt == null) continue;
            itemHeight = rt.rect.height;
            // 如果 prefab 使用布局元素，优先使用 LayoutElement.preferredHeight
            var layout = go.GetComponent<UnityEngine.UI.LayoutElement>();
            if (layout != null && layout.preferredHeight > 0)
                itemHeight = layout.preferredHeight;
            break;
        }

        if (itemHeight <= 0f)
        {
            // 兜底高度，避免除以 0
            itemHeight = 100f;
        }

        // 隐藏所有池项，等待 UpdateTaskList 分配位置与数据
        foreach (var go in pooledItems)
        {
            if (go != null) go.SetActive(false);
        }

        initialized = true;
    }

    // TaskManager 会调用此方法传入任务列表
    public void UpdateTaskList(List<MissionData> tasks)
    {
        if (!initialized) InitializePool();

        currentTasks = tasks ?? new List<MissionData>();
        totalCount = currentTasks.Count;

        // 设置 content 的高度以允许滚动（高度 = 项数 * 单项高度）
        var size = content.sizeDelta;
        size.y = totalCount * itemHeight;
        content.sizeDelta = size;

        // 将滚动位置复位到顶部（可根据需求保留当前位置）
        content.anchoredPosition = Vector2.zero;
        topIndex = 0;

        // 强制刷新显示池内容
        RefreshVisibleItems(force: true);
    }

    private void OnScrollChanged(Vector2 _)
    {
        RefreshVisibleItems(force: false);
    }

    private void RefreshVisibleItems(bool force)
    {
        if (!initialized) return;

        // 计算当前 content 滚动偏移以得到首可见项索引
        // content.anchoredPosition.y 为从顶部的偏移（向下滚动时为正）
        int newTopIndex = Mathf.FloorToInt(content.anchoredPosition.y / itemHeight);
        newTopIndex = Mathf.Clamp(newTopIndex, 0, Mathf.Max(0, totalCount - 1));

        // 把 topIndex 限制为使得后续填充不会越界
        int maxTop = Mathf.Max(0, totalCount - pooledItems.Count);
        newTopIndex = Mathf.Clamp(newTopIndex, 0, maxTop);

        if (!force && newTopIndex == topIndex) return;

        topIndex = newTopIndex;

        // 为池中每个 slot 绑定对应的数据索引并放到正确位置
        for (int i = 0; i < pooledItems.Count; i++)
        {
            int dataIndex = topIndex + i;
            var go = pooledItems[i];
            if (go == null) continue;

            if (dataIndex < 0 || dataIndex >= totalCount)
            {
                // 超出数据范围，隐藏并清除
                var taskCompHide = go.GetComponent<Task>();
                if (taskCompHide != null) taskCompHide.ClearTaskData();
                go.SetActive(false);
                continue;
            }

            // 显示 & 设置位置
            go.SetActive(true);
            var rt = go.GetComponent<RectTransform>();
            if (rt != null)
            {
                // anchor/pivot 已在初始化设置为顶端模式
                rt.anchoredPosition = new Vector2(0, -dataIndex * itemHeight);
                // 宽度保持 content 宽度（sizeDelta.x）
                rt.sizeDelta = new Vector2(rt.sizeDelta.x, itemHeight);
            }

            // 绑定数据（复用）
            var taskComp = go.GetComponent<Task>();
            if (taskComp != null)
            {
                // 用 GameManager 的 setting 初始化（与之前行为一致）
                taskComp.Initialize(currentTasks[dataIndex], GameManager.Instance.StateManager.Settings);
            }
        }
    }
}
