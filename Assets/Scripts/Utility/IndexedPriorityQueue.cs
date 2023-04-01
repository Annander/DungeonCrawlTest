using System;

public sealed class IndexedPriorityQueue<T>
    where T : IComparable<T>
{
    private T[] keys;
    private int[] heap;
    private int[] inverseHeap;

    private int size;

    public IndexedPriorityQueue(T[] keys, int maxSize)
    {
        this.keys = keys;

        heap = new int[maxSize + 1];
        inverseHeap = new int[maxSize + 1];
    }

    private void Swap(int a, int b)
    {
        int temp = heap[a];
        heap[a] = heap[b];
        heap[b] = temp;

        inverseHeap[heap[a]] = a;
        inverseHeap[heap[b]] = b;
    }

    private void ReorderUpwards(int node)
    {
        while((node > 1) && (keys[node /2].CompareTo(keys[heap[node]]) > 0))
        {
            Swap(node/2, node);
            node /= 2;
        }
    }

    private void ReorderDownwards(int node, int heapSize)
    {
        while(2 * node <= heapSize)
        {
            var child = 2 * node;

            if((child < heapSize) && (keys[heap[child]].CompareTo(keys[heap[child+1]]) > 0))
            {
                ++child;
            }

            if(keys[heap[node]].CompareTo(keys[heap[child+1]]) > 0)
            {
                Swap(child, node);
                node = child;
            }
            else
            {
                break;
            }
        }
    }

    public bool Empty
    {
        get { return size == 0; }
    }

    public void Insert(int index)
    {
        ++size;

        heap[size] = index;

        inverseHeap[index] = size;

        ReorderUpwards(size);
    }

    public int Pop()
    {
        Swap(1, size);
        ReorderDownwards(1, size - 1);
        return heap[size--];
    }

    public void ChangePriority(int index)
    {
        ReorderUpwards(inverseHeap[index]);
    }
}