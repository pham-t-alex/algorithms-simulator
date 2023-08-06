using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GraphVertex : MonoBehaviour
{
    private string vertexName = "?";
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

    private GameObject vertexInfoText;
    // Start is called before the first frame update
    void Start()
    {
        nameInput = Instantiate(Resources.Load<GameObject>("Prefabs/VertexText"), transform.position, Quaternion.identity, GameObject.Find("UIWorld").transform).GetComponent<VertexText>();
        nameInput.SetVertex(this);
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

    public void SetName(string name)
    {
        if (name == null || name == "")
        {
            vertexName = "?";
        }
        else
        {
            vertexName = name;
        }
        
    }

    public void CreateInfoText()
    {
        vertexInfoText = Instantiate(Resources.Load<GameObject>("Prefabs/InfoText"), transform.position, Quaternion.identity, GameObject.Find("Other").transform);
        vertexInfoText.GetComponent<TMP_Text>().text = vertexName;
        nameInput.gameObject.SetActive(false);
    }

    public void AddVariable(string varName, object initialVal)
    {
        variables.Add(varName, initialVal);
    }

    public void SetVariable(string varName, object value)
    {
        variables[varName] = value;
    }

    public object getVariable(string varName)
    {
        return variables[varName];
    }
}
