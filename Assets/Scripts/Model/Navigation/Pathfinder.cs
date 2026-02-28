using System.Collections.Generic;
using UnityEngine;

public sealed class Pathfinder
{
    private readonly GridMap map;

    private static readonly Vector2Int[] Neighbors =
    {
        new( 1, 0),
        new(-1, 0),
        new( 0, 1),
        new( 0,-1),
    };

    public Pathfinder(GridMap map)
    {
        this.map = map;
    }

    public List<Vector2Int> FindPath(
        Vector2Int start,
        Vector2Int goal,
        int maxExpand = 10_000)
    {
        var nodes = new List<Node>(256);
        var nodeIndex = new Dictionary<Vector2Int, int>(256);
        var open = new MinHeap();

        nodes.Add(new Node
        {
            pos = start,
            g = 0,
            f = Heuristic(start, goal),
            parent = -1
        });

        nodeIndex[start] = 0;
        open.Push(0, nodes[0].f);

        int expanded = 0;

        while (open.Count > 0)
        {
            int currentIndex = open.Pop();
            var current = nodes[currentIndex];

            if (current.pos == goal)
                return Reconstruct(nodes, currentIndex);

            if (++expanded > maxExpand)
                break;

            foreach (var d in Neighbors)
            {
                var nextPos = current.pos + d;
                if (!map.CanWalk(nextPos)) continue;

                int g = current.g + 1;

                if (nodeIndex.TryGetValue(nextPos, out int nextIndex))
                {
                    if (g >= nodes[nextIndex].g) continue;

                    nodes[nextIndex] = new Node
                    {
                        pos = nextPos,
                        g = g,
                        f = g + Heuristic(nextPos, goal),
                        parent = currentIndex
                    };

                    open.DecreaseKey(nextIndex, nodes[nextIndex].f);
                }
                else
                {
                    int index = nodes.Count;
                    nodes.Add(new Node
                    {
                        pos = nextPos,
                        g = g,
                        f = g + Heuristic(nextPos, goal),
                        parent = currentIndex
                    });

                    nodeIndex[nextPos] = index;
                    open.Push(index, nodes[index].f);
                }
            }
        }

        return null;
    }

    private static int Heuristic(Vector2Int a, Vector2Int b)
        => Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y);

    private static List<Vector2Int> Reconstruct(List<Node> nodes, int index)
    {
        var path = new List<Vector2Int>();
        while (index != -1)
        {
            path.Add(nodes[index].pos);
            index = nodes[index].parent;
        }
        path.Reverse();
        return path;
    }
}

struct Node
{
    public Vector2Int pos;
    public int g;
    public int f;
    public int parent; // index
}
