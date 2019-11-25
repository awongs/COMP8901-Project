using System.Collections.Generic;

public class PriorityQueue<T>
{
    public delegate bool Tiebreaker(T first, T second);

    private class Node
    {
        public T item;
        public int priority;

        public Node(T item, int priority)
        {
            this.item = item;
            this.priority = priority;
        }
    }

    // List of items in the queue.
    private readonly List<Node> m_items;

    // Lambda function for breaking ties when priority is equal.
    private readonly Tiebreaker m_tiebreaker;

    public int Count {
        get {
            return m_items.Count;
        }
    }

    public PriorityQueue(Tiebreaker tiebreaker)
    {
        m_items = new List<Node>();
        m_tiebreaker = tiebreaker;
    }

    public void Enqueue(T item, int priority)
    {
        int index;
        for (index = 0; index < m_items.Count; index++)
        {
            if (m_items[index].priority == priority)
            {
                if (m_tiebreaker(item, m_items[index].item))
                {
                    break;
                }
                else
                {
                    continue;
                }
            }
            
            if (m_items[index].priority > priority)
            {
                break;
            }
        }

        // Create a node and insert it to the list.
        Node itemNode = new Node(item, priority);
        m_items.Insert(index, itemNode);
    }

    public T Dequeue()
    {
        if (m_items.Count == 0)
        {
            return default;
        }

        T item = m_items[0].item;
        m_items.RemoveAt(0);
        return item;
    }
}
