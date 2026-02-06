using UnityEngine;

[CreateAssetMenu(fileName = "New Decor", menuName = "Furniture/Decor Data", order = 2)]
public class DecorScriptableObject : ScriptableObject
{
    [Header("基础信息")]
    public string decorId;
    public string decorName;
    public DecorType decorType;
    
    [Header("渲染设置")]
    [Tooltip("装饰的Sprite图片")]
    public Sprite decorSprite;
    
    [Tooltip("相对于格子的渲染偏移")]
    public Vector2 renderOffset = Vector2.zero;
    
    [Tooltip("排序层级")]
    public int sortingOrder = 100;
    
    // 转换为运行时数据
    public DecorData ToDecorData()
    {
        return new DecorData
        {
            id = decorId,
            name = decorName,
            type = decorType,
            renderOffset = renderOffset,
            prefabPath = "", // 不再使用prefab
            spritePath = "", // 不再使用路径
            sortingOrder = sortingOrder,
            sprite = decorSprite // 添加Sprite引用
        };
    }
    
    private void OnValidate()
    {
        // 确保ID不为空
        if (string.IsNullOrWhiteSpace(decorId))
        {
            decorId = name.ToLower().Replace(" ", "_");
        }
    }
}