using System.Collections.Generic;

public sealed class MinHeap
{
    private readonly List<HeapItem> heap = new();
    private readonly Dictionary<int, int> indexMap = new();
    // nodeIndex -> heapIndex

    public int Count => heap.Count;

    public void Push(int nodeIndex, int priority)
    {
        var item = new HeapItem
        {
            nodeIndex = nodeIndex,
            priority = priority
        };

        heap.Add(item);
        int i = heap.Count - 1;
        indexMap[nodeIndex] = i;

        SiftUp(i);
    }

    public int Pop()
    {
        int result = heap[0].nodeIndex;

        Swap(0, heap.Count - 1);
        heap.RemoveAt(heap.Count - 1);
        indexMap.Remove(result);

        if (heap.Count > 0)
            SiftDown(0);

        return result;
    }

    public void DecreaseKey(int nodeIndex, int newPriority)
    {
        if (!indexMap.TryGetValue(nodeIndex, out int i))
            return;

        if (heap[i].priority <= newPriority)
            return;

        heap[i] = new HeapItem
        {
            nodeIndex = nodeIndex,
            priority = newPriority
        };

        SiftUp(i);
    }

    // ===== Heap ops =====

    private void SiftUp(int i)
    {
        while (i > 0)
        {
            int parent = (i - 1) / 2;
            if (heap[parent].priority <= heap[i].priority)
                break;

            Swap(i, parent);
            i = parent;
        }
    }

    private void SiftDown(int i)
    {
        int count = heap.Count;

        while (true)
        {
            int left = i * 2 + 1;
            int right = left + 1;
            int smallest = i;

            if (left < count && heap[left].priority < heap[smallest].priority)
                smallest = left;

            if (right < count && heap[right].priority < heap[smallest].priority)
                smallest = right;

            if (smallest == i)
                break;

            Swap(i, smallest);
            i = smallest;
        }
    }

    private void Swap(int a, int b)
    {
        var tmp = heap[a];
        heap[a] = heap[b];
        heap[b] = tmp;

        indexMap[heap[a].nodeIndex] = a;
        indexMap[heap[b].nodeIndex] = b;
    }
}


struct HeapItem
{
    public int nodeIndex;
    public int priority;
}