using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterTileMoveAI : MonoBehaviour
{
    private GridMap gridMap;
    public Vector2Int curGridPos;

    // 移动速度 (world units / second)
    public float moveSpeed = 2f;

    void Start()
    {
        // 从 GameManager 或 TileManager 获取引用TODO
        //gridMap = GameManager.Instance.TileTypeManager.gridMap;

        // 初始化角色所在格子，如果不在逻辑格子上
        curGridPos = GridToCell(transform.position);
    }

    // 外部调用：随机移动到一个可走格子
    public void MoveToRandomCell()
    {
        Vector2Int target = gridMap.GetRandomWalkablePos();
        StartCoroutine(MoveToCell(target));
    }

    // 核心移动协程
    private IEnumerator MoveToCell(Vector2Int targetCell)
    {
        Vector3 start = transform.position;
        Vector3 end = CellToWorldCenter(targetCell);

        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime * moveSpeed;
            transform.position = Vector3.Lerp(start, end, t);
            yield return null;
        }

        // 更新逻辑格子
        curGridPos = targetCell;
    }


    // 世界坐标 → 格子坐标
    private Vector2Int GridToCell(Vector3 worldPos)
    {
        int x = Mathf.FloorToInt(worldPos.x);
        int y = Mathf.FloorToInt(worldPos.z); // z 是 forward
        return new Vector2Int(x, y);
    }

    // 格子中心 → 世界坐标
    private Vector3 CellToWorldCenter(Vector2Int cellPos)
    {
        // 根据格子大小计算中心点
        float halfSize = 0.5f; // 假设格子 1x1
        return new Vector3(cellPos.x + halfSize, transform.position.y, cellPos.y + halfSize);
    }


}
