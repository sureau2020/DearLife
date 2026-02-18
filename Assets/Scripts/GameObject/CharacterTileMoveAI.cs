using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Tilemaps;

public class CharacterTileMoveAI : MonoBehaviour
{
    private RoomManager room;
    [SerializeField] private Tilemap floorMap;
    [SerializeField] private SortingGroup sortingGroup;
    [SerializeField] private CharacterBreatheComponent breatheComponent;
    public float moveSpeed = 2f;
    public Vector3 offset = new Vector3(-0.4f, 0, 0.3f); // 角色在格子中的偏移，调整为适合角色的值

    private List<Vector2Int> currentPath; // A* 传回来的逻辑坐标列表
    private int targetIndex = 0; // 当前目标点在路径中的索引 
    Vector3Int lastGridPos;


    void OnEnable()
    {
        RoomManager.OnRoomManagerInitialized += InitializeWalking;
    }

    void OnDisable()
    {
        RoomManager.OnRoomManagerInitialized -= InitializeWalking;
    }

    private void InitializeWalking (RoomManager roomManager){
        room = roomManager;
        Wander();
    }

    void Update()
    {
        Vector3Int currentGridPos = floorMap.WorldToCell(transform.position);
        if (currentGridPos != lastGridPos)
        {
            // 只有格子变了才更新 Order
            sortingGroup.sortingOrder = -currentGridPos.y;
            lastGridPos = currentGridPos;
        }
    }

    public void Wander()
    {
        var walkables = new List<Vector2Int>(room.GridMap.GetAllWalkableCells());
        if (walkables.Count == 0) return;

        Vector2Int start = CurrentCell();
        Vector2Int goal = walkables[UnityEngine.Random.Range(0, walkables.Count)];

        var path = room.FindPath(start, goal);
        if (path == null) { Debug.Log("no path"); return; }

        Debug.Log($"Path found from {start} to {goal}: {string.Join(" -> ", path)}");

        currentPath = path;
        targetIndex = 0;

        StopAllCoroutines();
        StartCoroutine(FollowPath());
    }


    private Vector2Int CurrentCell()
    {
        Vector3 worldPos = transform.position;
        Vector3Int gridPos = floorMap.WorldToCell(worldPos);
        Debug.Log($"Current world position: {worldPos}, grid position: {gridPos}");
        return new Vector2Int(gridPos.x,gridPos.y);
    }


    private IEnumerator FollowPath()
    {
        while (targetIndex < currentPath.Count)
        {
            // 关键：在这里直接把逻辑格点转为世界坐标
            Vector3Int cellPos = new Vector3Int(currentPath[targetIndex].x, currentPath[targetIndex].y, 0);
            Vector3 targetWorldPos = floorMap.GetCellCenterWorld(cellPos)+ offset;

            targetWorldPos.y = transform.position.y;

            // 平滑位移
            while (Vector3.Distance(transform.position, targetWorldPos) > 0.05f)
            {
                transform.position = Vector3.MoveTowards(
                    transform.position,
                    targetWorldPos,
                    moveSpeed * Time.deltaTime
                );
                UpdateSpriteDirection(targetWorldPos.x - transform.position.x);
                yield return null; // 等待下一帧
            }

            targetIndex++;
        }
        targetIndex = 0; // 重置索引
        Debug.Log("Reached destination"+CurrentCell());
    }

    void UpdateSpriteDirection(float x)
    {
        if (breatheComponent == null) return;

        if (x > 0.1f)  // 往右走
        {
            breatheComponent.SetFlipDirection(-1);  // 翻转
        }
        else if (x < -0.1f) // 往左走
        {
            breatheComponent.SetFlipDirection(1);   // 正常
        }
    }
}
