using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Edge : MonoBehaviour
{
    private float weight;
    public float Weight
    {
        get
        {
            return weight;
        }
    }
    private bool mouseTouching;
    public bool MouseTouching
    {
        get
        {
            return mouseTouching;
        }
    }
    private EdgeText weightInput;
    private GameObject edgeInfoText;

    private GraphVertex source;
    public GraphVertex Source
    {
        get
        {
            return source;
        }
    }
    private GraphVertex destination;
    public GraphVertex Destination
    {
        get
        {
            return destination;
        }
    }

    public string EdgeName
    {
        get
        {
            return source.VertexName + "-" + destination.VertexName;
        }
    }

    public bool SelfEdge
    {
        get
        {
            return source == destination;
        }
    }

    [SerializeField] private bool directed;
    // Start is called before the first frame update
    void Start()
    {
        weightInput = Instantiate(Resources.Load<GameObject>("Prefabs/EdgeText"), transform.position, Quaternion.identity, GameObject.Find("UIWorld").transform).GetComponent<EdgeText>();
        if (!DirectedGraphController.DirGraphController.Weighted)
        {
            HideWeight();
        }
        weightInput.SetEdge(this);
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

    public void SetVertices(GraphVertex source, GraphVertex destination)
    {
        if (directed)
        {
            if (this.source != null)
            {
                this.source.RemoveOutgoingEdge(this);
            }
            if (this.destination != null)
            {
                this.destination.RemoveIncomingEdge(this);
            }
            source.AddOutgoingEdge(this);
            destination.AddIncomingEdge(this);
        }
        else
        {
            if (source == destination)
            {
                return;
            }
            if (this.source != null)
            {
                this.source.RemoveIncomingEdge(this);
                this.source.RemoveOutgoingEdge(this);
            }
            if (this.destination != null)
            {
                this.destination.RemoveIncomingEdge(this);
                this.destination.RemoveOutgoingEdge(this);
            }
            source.AddIncomingEdge(this);
            source.AddOutgoingEdge(this);
            destination.AddIncomingEdge(this);
            destination.AddOutgoingEdge(this);
        }
        this.source = source;
        this.destination = destination;
        UpdatePosition();
    }
    public void UpdatePosition()
    {
        if (source != destination)
        {
            if (directed)
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
                GetComponent<BoxCollider2D>().size = new Vector2(5, 0.2f);
                GetComponent<BoxCollider2D>().offset = Vector2.zero;
            }
        }
        else
        {
            if (directed)
            {
                transform.position = source.transform.position + new Vector3(1, 1, 0);
                transform.rotation = Quaternion.identity;
                transform.localScale = Vector3.one;
                GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Sprites/directedselfedge");
                GetComponent<BoxCollider2D>().size = new Vector2(1, 1);
                GetComponent<BoxCollider2D>().offset = new Vector2(-0.1f, -0.1f);
            }
        }
        weightInput.transform.position = transform.position;
    }

    public void SetColor(Color color)
    {
        GetComponent<SpriteRenderer>().color = color;
    }

    public void HideWeight()
    {
        weightInput.gameObject.SetActive(false);
    }

    public void ShowWeight()
    {
        weightInput.gameObject.SetActive(true);
    }

    public bool SetWeight(string weight)
    {
        float newWeight;

        bool success = float.TryParse(weight, out newWeight);
        if (!success)
        {
            this.weight = 0;
            return false;
        }
        this.weight = newWeight;
        return true;
    }

    public void CreateInfoText()
    {
        weightInput.GetComponent<TMP_InputField>().interactable = false;
    }

    public override string ToString()
    {
        return EdgeName;
    }
}
