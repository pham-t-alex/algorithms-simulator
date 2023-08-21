using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Connector<T> : MonoBehaviour where T : MonoBehaviour
{
    protected T source;
    public T Source
    {
        get
        {
            return source;
        }
    }

    protected T destination;
    public T Destination
    {
        get
        {
            return destination;
        }
    }

    public bool SelfConnect
    {
        get
        {
            return source == destination;
        }
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void SetElements(T source, T destination)
    {
        this.source = source;
        this.destination = destination;
        UpdatePosition();
    }

    public abstract void UpdatePosition();

    public void SetColor(Color color)
    {
        GetComponent<SpriteRenderer>().color = color;
    }
}
