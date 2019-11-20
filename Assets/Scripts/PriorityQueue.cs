using System.Collections.Generic;

public class PriorityQueue<T>
{
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

    private readonly List<Node> m_items;

    public PriorityQueue()
    {
        m_items = new List<Node>();
    }

    public void Enqueue(T item, int priority)
    {
        int index;
        for (index = 0; index < m_items.Count; index++)
        {
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
