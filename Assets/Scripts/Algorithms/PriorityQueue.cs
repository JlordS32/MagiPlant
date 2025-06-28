using System.Collections.Generic;

public class PriorityQueue<T>
{
    private readonly SortedDictionary<int, Queue<T>> _dict = new();
    public int Count { get; private set; }

    public void Enqueue(T item, int priority)
    {
        if (!_dict.TryGetValue(priority, out var queue))
        {
            queue = new Queue<T>();
            _dict[priority] = queue;
        }
        queue.Enqueue(item);
        Count++;
    }

    public T Dequeue()
    {
        foreach (var kvp in _dict)
        {
            if (kvp.Value.Count > 0)
            {
                Count--;
                T item = kvp.Value.Dequeue();
                if (kvp.Value.Count == 0)
                    _dict.Remove(kvp.Key);
                return item;
            }
        }
        throw new System.InvalidOperationException("Queue is empty");
    }
}
