using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PriorityQueue<T>
{
    private List<QueueElement> priorityQueue = new List<QueueElement>();

    public int Count
    {
        get
        {
            return priorityQueue.Count;
        }
    }

    class QueueElement
    {
        public float key;
        public T element;

        public QueueElement(float key, T element)
        {
            this.key = key;
            this.element = element;
        }
    }

    public void AddElement(float key, T obj)
    {
        QueueElement element = new QueueElement(key, obj);
        int index = priorityQueue.Count;
        while (index > 0 && priorityQueue[index - 1].key > key)
        {
            index--;
        }
        priorityQueue.Insert(index, element);
    }

    public bool DecreaseKey(float newKey, T obj)
    {
        int index = 0;
        while (index < priorityQueue.Count && !priorityQueue[index].element.Equals(obj))
        {
            index++;
        }
        if (index == priorityQueue.Count)
        {
            return false;
        }
        if (priorityQueue[index].key == newKey)
        {
            return false;
        }
        priorityQueue.RemoveAt(index);
        while (index > 0 && priorityQueue[index - 1].key > newKey)
        {
            index--;
        }
        priorityQueue.Insert(index, new QueueElement(newKey, obj));
        return true;
    }

    public T Remove()
    {
        T element = default;
        if (priorityQueue.Count > 0)
        {
            element = priorityQueue[0].element;
            priorityQueue.RemoveAt(0);
        }
        return element;
    }

    public List<float> Keys()
    {
        List<float> keys = new List<float>();
        foreach (QueueElement e in priorityQueue)
        {
            keys.Add(e.key);
        }
        return keys;
    }

    public List<T> Elements()
    {
        List<T> elements = new List<T>();
        foreach (QueueElement e in priorityQueue)
        {
            elements.Add(e.element);
        }
        return elements;
    }
}
