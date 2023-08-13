using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisjointSetArrow : MonoBehaviour
{
    private DisjointSetElement source;
    public DisjointSetElement Source
    {
        get
        {
            return source;
        }
    }
    private DisjointSetElement destination;
    public DisjointSetElement Destination
    {
        get
        {
            return destination;
        }
    }
    public bool SelfArrow
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

    public void SetElements(DisjointSetElement source, DisjointSetElement destination)
    {
        this.source = source;
        this.destination = destination;
        UpdatePosition();
    }

    public void UpdatePosition()
    {
        if (source != destination)
        {
            transform.position = (source.transform.position + destination.transform.position) / 2f;
            Vector3 direction = (destination.transform.position - source.transform.position).normalized;
            float angle = Mathf.Asin(direction.y);
            if (direction.x < 0)
            {
                angle = Mathf.PI - angle;
            }
            transform.rotation = Quaternion.Euler(0, 0, angle * Mathf.Rad2Deg);
            float scale = (Vector3.Distance(source.transform.position, destination.transform.position) - 2) / 5f;
            transform.localScale = new Vector3(scale, 1, 1);
            GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Sprites/directededge");
        }
        else
        {
            transform.position = source.transform.position + new Vector3(1, 1, 0);
            transform.rotation = Quaternion.identity;
            transform.localScale = Vector3.one;
            GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Sprites/directedselfedge");
        }
    }

    public void SetColor(Color color)
    {
        GetComponent<SpriteRenderer>().color = color;
    }
}
