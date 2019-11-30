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
    private readonly List<Node> m_nodes;

    // Lambda function for breaking ties when priority is equal.
    private readonly Tiebreaker m_tiebreaker;

    public int Count {
        get {
            return m_nodes.Count;
        }
    }

    public PriorityQueue(Tiebreaker tiebreaker)
    {
        m_nodes = new List<Node>();
        m_tiebreaker = tiebreaker;
    }

    public void Enqueue(T item, int priority)
    {
        int index;
        for (index = 0; index < m_nodes.Count; index++)
        {
            if (m_nodes[index].priority == priority)
            {
                if (m_tiebreaker(item, m_nodes[index].item))
                {
                    break;
                }
                else
                {
                    continue;
                }
            }
            
            if (m_nodes[index].priority > priority)
            {
                break;
            }
        }

        // Create a node and insert it to the list.
        Node itemNode = new Node(item, priority);
        m_nodes.Insert(index, itemNode);
    }

    public T Dequeue()
    {
        if (m_nodes.Count == 0)
        {
            return default;
        }

        T item = m_nodes[0].item;
        m_nodes.RemoveAt(0);
        return item;
    }

    public bool Remove(T item)
    {
        for (int i = 0; i < m_nodes.Count; i++)
        {
            if (m_nodes[i].item.Equals(item))
            {
                m_nodes.RemoveAt(i);
                return true;
            }
        }

        return false;
    }
}
