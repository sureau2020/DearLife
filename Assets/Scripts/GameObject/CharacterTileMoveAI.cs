using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Tilemaps;

public class CharacterTileMoveAI : MonoBehaviour
{
    [Header("移动设置")]
    public float moveSpeed = 2f;
    public Vector3 offset = new Vector3(-0.4f, 0, 0.3f); // 角色在格子中的偏移
    
    [Header("等待设置")]
    public float minWait = 2f;        // 等待时间范围
    public float maxWait = 5f;
    
    [Header("组件引用")]
    [SerializeField] private Tilemap floorMap;
    [SerializeField] private SortingGroup sortingGroup;
    [SerializeField] private CharacterBreatheComponent breatheComponent;

    // 寻路相关
    private RoomManager room;
    private List<Vector2Int> currentPath;
    private int targetIndex = 0;
    private Coroutine currentMovementCoroutine;
    private Coroutine currentWaitCoroutine;
    
    // 状态管理
    private CharacterData character;
    private bool isAlive = true;
    private bool isWaiting = false;
    private bool isFocused = false;
    private bool isMoving = false;


    
    // 位置跟踪
    private Vector3Int lastGridPos;

    void OnEnable()
    {
        RoomManager.OnRoomManagerInitialized += InitializeWalking;
    }

    void OnDisable()
    {
        RoomManager.OnRoomManagerInitialized -= InitializeWalking;
        if (character != null)
        {
            character.OnHealthChanged -= OnCharacterStateChangedHandler;
        }
    }

    private void InitializeWalking(RoomManager roomManager)
    {
        room = roomManager;
        character = GameManager.Instance.StateManager.Character;
        character.OnHealthChanged += OnCharacterStateChangedHandler;
        
        // 开始随机游荡
        StartWandering();
    }

    void Update()
    {
        if (!isAlive) return;

        // 更新排序层级
        UpdateSortingOrder();
        
        // 如果没有在移动且没有被focus，检查是否需要开始下一次移动
        if (!isFocused && !isMoving && !isWaiting)
        {
            StartWandering();
        }
    }

    private void UpdateSortingOrder()
    {
        Vector3Int currentGridPos = floorMap.WorldToCell(transform.position);
        if (currentGridPos != lastGridPos)
        {
            sortingGroup.sortingOrder = -currentGridPos.y;
            lastGridPos = currentGridPos;
        }
    }

    private void OnCharacterStateChangedHandler()
    {
        if (character.HealthState == HealthState.Dead && isAlive)
        {
            isAlive = false;
            StopAllMovement();
            
            // 停止呼吸动画
            if (breatheComponent != null)
            {
                breatheComponent.SetBreathing(false);
            }
            
            Debug.Log("Character died, stopping all movement");
        }
        else if (character.HealthState != HealthState.Dead && !isAlive)
        {
            isAlive = true;
            
            // 恢复呼吸动画
            if (breatheComponent != null)
            {
                breatheComponent.SetBreathing(true);
            }
            
            // 如果没有被focus，恢复游荡
            if (!isFocused)
            {
                StartWandering();
            }
            
            Debug.Log("Character revived, resuming movement");
        }
    }

    /// <summary>
    /// 开始随机游荡
    /// </summary>
    public void StartWandering()
    {
        if (!isAlive || isFocused || room == null) return;
        isMoving = true;

        // 获取所有可行走的格子
        var walkableCells = new List<Vector2Int>(room.GridMap.GetAllWalkableCells());
        if (walkableCells.Count == 0) 
        {
            isMoving = false;
            StartWaiting();
            return;
        }

        // 选择随机目标
        Vector2Int start = CurrentCell();
        Vector2Int goal = walkableCells[UnityEngine.Random.Range(0, walkableCells.Count)];

        // 寻找路径
        var path = room.FindPath(start, goal);
        
        if (path != null && path.Count > 1) 
        {
            currentPath = path;
            targetIndex = 0;
            
            Debug.Log($"Starting journey from {start} to {goal}, path length: {path.Count}");
            StartMovement();
        }
        else
        {
            isMoving = false;
            StartWaiting(); // 如果找不到路径，等待后重试
        }
    }

    /// <summary>
    /// 开始移动
    /// </summary>
    private void StartMovement()
    {
        if (currentMovementCoroutine != null)
        {
            StopCoroutine(currentMovementCoroutine);
        }
        
        currentMovementCoroutine = StartCoroutine(FollowPath());
    }

    /// <summary>
    /// 开始等待
    /// </summary>
    private void StartWaiting()
    {
        if (currentWaitCoroutine != null)
        {
            StopCoroutine(currentWaitCoroutine);
        }
        
        currentWaitCoroutine = StartCoroutine(WaitAndContinue());
    }

    /// <summary>
    /// 停止所有移动相关协程
    /// </summary>
    private void StopAllMovement()
    {
        if (currentMovementCoroutine != null)
        {
            StopCoroutine(currentMovementCoroutine);
            currentMovementCoroutine = null;
        }
        
        if (currentWaitCoroutine != null)
        {
            StopCoroutine(currentWaitCoroutine);
            currentWaitCoroutine = null;
        }
        
        isMoving = false;
        isWaiting = false;
    }

    /// <summary>
    /// 跟随路径移动
    /// </summary>
    private IEnumerator FollowPath()
    {
        isMoving = true;
        
        while (targetIndex < currentPath.Count && !isFocused && isAlive)
        {
            // 获取目标世界坐标
            Vector3Int cellPos = new Vector3Int(currentPath[targetIndex].x, currentPath[targetIndex].y, 0);
            Vector3 targetWorldPos = floorMap.GetCellCenterWorld(cellPos) + offset;
            //targetWorldPos.y = transform.position.y; // 保持Y轴不变

            // 移动到目标点
            while (Vector3.Distance(transform.position, targetWorldPos) > 0.05f && !isFocused && isAlive)
            {
                Vector3 direction = (targetWorldPos - transform.position).normalized;
                transform.position = Vector3.MoveTowards(
                    transform.position,
                    targetWorldPos,
                    moveSpeed * Time.deltaTime
                );
                
                // 更新精灵朝向
                if (!isFocused)
                {
                    UpdateSpriteDirection(direction.x);
                }
                
                yield return null;
            }

            targetIndex++;
        }

        isMoving = false;
        
        // 如果成功完成路径且角色仍活着且没有被focus
        if (!isFocused && isAlive)
        {
            Debug.Log($"Reached destination: {CurrentCell()}");
            StartWaiting(); // 到达目的地后等待
        }
    }

    /// <summary>
    /// 等待并继续下一次移动
    /// </summary>
    private IEnumerator WaitAndContinue()
    {
        isWaiting = true;
        float waitTime = Random.Range(minWait, maxWait);
        
        Debug.Log($"Waiting for {waitTime:F1} seconds at {CurrentCell()}");
        
        float elapsed = 0f;
        while (elapsed < waitTime && !isFocused && isAlive)
        {
            elapsed += Time.deltaTime;
            yield return null;
        }
        
        isWaiting = false;
        
        // 等待结束后，如果角色仍活着且没有被focus，开始新的游荡
        if (!isFocused && isAlive)
        {
            StartWandering();
        }
    }

    /// <summary>
    /// 获取当前格子坐标
    /// </summary>
    private Vector2Int CurrentCell()
    {
        Vector3 worldPos = transform.position;
        Vector3Int gridPos = floorMap.WorldToCell(worldPos);
        return new Vector2Int(gridPos.x, gridPos.y);
    }

    /// <summary>
    /// 更新精灵朝向
    /// </summary>
    private void UpdateSpriteDirection(float velocityX)
    {
        if (breatheComponent == null) return;

        if (velocityX > 0.1f)  // 往右走
        {
            breatheComponent.SetFlipDirection(-1);  // 翻转
        }
        else if (velocityX < -0.1f) // 往左走
        {
            breatheComponent.SetFlipDirection(1);   // 正常
        }
    }

    /// <summary>
    /// 设置焦点状态
    /// </summary>
    /// <param name="focus">是否聚焦</param>
    public void SetFocus(bool focus)
    {
        bool wasMoving = isMoving;
        bool wasWaiting = isWaiting;
        
        isFocused = focus;

        if (focus)
        {
            // 被focus时停止所有移动
            StopAllMovement();
            Debug.Log("Character focused, stopping movement");
        }
        else
        {
            // 取消focus时恢复移动（如果角色活着）
            if (isAlive)
            {
                Debug.Log("Character unfocused, resuming movement");
                // 立即开始新的游荡，而不是继续之前的路径
                StartWandering();
            }
        }
    }

    /// <summary>
    /// 强制移动到指定位置（如果有效）
    /// </summary>
    /// <param name="targetPos">目标格子坐标</param>
    public bool MoveToPosition(Vector3 targetPos)
    {
        if (!isAlive || isFocused || room == null) return false;

        Vector2Int start = CurrentCell();
        Vector3Int end = floorMap.WorldToCell(targetPos);
        Vector2Int targetCell = new Vector2Int(end.x, end.y);
        var path = room.FindPath(start, targetCell);
        
        if (path != null && path.Count > 1)
        {
            currentPath = path;
            targetIndex = 0;
            
            StopAllMovement(); // 停止当前的移动
            StartMovement(); // 开始新的移动
            
            return true;
        }
        else
        {
            return false; 
        }
    }

    /// <summary>
    /// 获取当前状态信息（用于调试）
    /// </summary>
    public string GetStatusInfo()
    {
        return $"Alive: {isAlive}, Focused: {isFocused}, Moving: {isMoving}, Waiting: {isWaiting}, Position: {CurrentCell()}";
    }
}
