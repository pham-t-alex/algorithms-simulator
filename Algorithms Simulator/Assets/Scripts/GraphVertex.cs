using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GraphVertex : MonoBehaviour
{
    private string vertexName = "?";
    public string VertexName
    {
        get
        {
            return vertexName;
        }
    }
    private Dictionary<string, object> variables = new Dictionary<string, object>();
    private HashSet<string> updatedVariables = new HashSet<string>();
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
    public List<Edge> IncomingEdges
    {
        get
        {
            return incomingEdges;
        }
    }
    private List<Edge> outgoingEdges = new List<Edge>();
    public List<Edge> OutgoingEdges
    {
        get
        {
            return outgoingEdges;
        }
    }

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
        MarkVarUpdated(varName);
    }

    public object GetVariable(string varName)
    {
        return variables[varName];
    }

    public void UpdateInfoText()
    {
        string s = vertexName;
        foreach (KeyValuePair<string, object> var in variables)
        {
            s += $"\n{var.Key} = ";
            if (updatedVariables.Contains(var.Key))
            {
                s += "<color=#6a00ffff>";
            }
            if (var.Value == null)
            {
                s += "<i>null</i>";
            }
            else if (var.Value is bool)
            {
                if ((bool) var.Value == true)
                {
                    s += "<color=green>T</color>";
                }
                else
                {
                    s += "<color=red>F</color>";
                }
            }
            else if (var.Value is GraphVertex)
            {
                s += "<i>" + ((GraphVertex) var.Value).vertexName + "</i>";
            }
            else
            {
                s += var.Value.ToString();
            }
            if (updatedVariables.Contains(var.Key))
            {
                s += "</color>";
            }
        }
        vertexInfoText.GetComponent<TMP_Text>().text = s;
        updatedVariables.Clear();
    }

    public void MarkVarUpdated(string var)
    {
        updatedVariables.Add(var);
    }

    public void SortOutgoingEdges()
    {
        OutgoingEdgeComparer comparer = new OutgoingEdgeComparer();
        outgoingEdges.Sort(comparer);
    }
}
