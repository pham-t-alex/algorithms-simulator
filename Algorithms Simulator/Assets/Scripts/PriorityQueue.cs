using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PriorityQueue<T>
{
    private List<QueueElement> priorityQueue = new List<QueueElement>();
    class QueueElement
    {
        private float key;
        private T element;
    }
}
