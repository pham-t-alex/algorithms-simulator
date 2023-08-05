using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GraphVertex : MonoBehaviour
{
    private string vertexName;
    private Dictionary<string, object> variables;
    private VertexText nameInput;
    [SerializeField] private bool mouseTouching;
    public bool MouseTouching
    {
        get
        {
            return mouseTouching;
        }
    }
    private List<Edge> incomingEdges = new List<Edge>();
    private List<Edge> outgoingEdges = new List<Edge>();
    // Start is called before the first frame update
    void Start()
    {
        nameInput = Instantiate(Resources.Load<GameObject>("Prefabs/VertexText"), transform.position, Quaternion.identity, GameObject.Find("UIWorld").transform).GetComponent<VertexText>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnMouseEnter()
    {
        mouseTouching = true;
    }

    private void OnMouseExit()
    {
        mouseTouching = false;
    }

    public void UpdatePosition()
    {
        nameInput.transform.position = transform.position;
        foreach (Edge e in incomingEdges)
        {
            e.UpdatePosition();
        }
        foreach (Edge e in outgoingEdges)
        {
            e.UpdatePosition();
        }
    }

    public void AddIncomingEdge(Edge e)
    {
        incomingEdges.Add(e);
    }

    public void RemoveIncomingEdge(Edge e)
    {
        incomingEdges.Remove(e);
    }

    public void AddOutgoingEdge(Edge e)
    {
        outgoingEdges.Add(e);
    }

    public void RemoveOutgoingEdge(Edge e)
    {
        outgoingEdges.Remove(e);
    }

    public void SetColor(Color color)
    {
        GetComponent<SpriteRenderer>().color = color;
    }
}
