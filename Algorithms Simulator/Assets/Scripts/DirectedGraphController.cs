using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DirectedGraphController : MonoBehaviour
{
    private static DirectedGraphController directedGraphController;
    public static DirectedGraphController DirGraphController
    {
        get
        {
            if (directedGraphController == null)
            {
                directedGraphController = FindObjectOfType<DirectedGraphController>();
            }
            return directedGraphController;
        }
    }

    private Camera screenCamera;
    public Camera ScreenCamera
    {
        get
        {
            if (screenCamera == null)
            {
                screenCamera = Camera.main;
            }
            return screenCamera;
        }
    }

    private List<GraphVertex> vertices = new List<GraphVertex>();
    private List<DirectedEdge> edges = new List<DirectedEdge>();

    private bool frozen = false;
    private bool runningAlgorithm = false;
    [SerializeField] private GameObject selectedObject;
    private float selectedTime = float.MinValue;
    private Vector3 prevMousePosition;

    public Vector3 MousePosition
    {
        get
        {
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePosition.z = 0;
            return mousePosition;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!frozen)
        {
            MoveCamera();
        }
        if (runningAlgorithm)
        {

        }
        else
        {
            NotRunningAlgorithms();
        }
        prevMousePosition = MousePosition;
    }

    public void MoveCamera()
    {
        if (Input.GetKey(KeyCode.A))
        {
            ScreenCamera.transform.Translate(Vector3.left * 0.01f * Mathf.Sqrt(ScreenCamera.orthographicSize));
        }
        if (Input.GetKey(KeyCode.S))
        {
            ScreenCamera.transform.Translate(Vector3.down * 0.01f * Mathf.Sqrt(ScreenCamera.orthographicSize));
        }
        if (Input.GetKey(KeyCode.W))
        {
            ScreenCamera.transform.Translate(Vector3.up * 0.01f * Mathf.Sqrt(ScreenCamera.orthographicSize));
        }
        if (Input.GetKey(KeyCode.D))
        {
            ScreenCamera.transform.Translate(Vector3.right * 0.01f * Mathf.Sqrt(ScreenCamera.orthographicSize));
        }
        if (Input.GetKey(KeyCode.Z))
        {
            ScreenCamera.orthographicSize /= 1.002f;
        }
        if (Input.GetKey(KeyCode.X))
        {
            ScreenCamera.orthographicSize *= 1.002f;
        }
    }

    public void CreateVertex()
    {
        Vector3 position = ScreenCamera.transform.position;
        position.z = 0;
        vertices.Add(Instantiate(Resources.Load<GameObject>("Prefabs/Vertex"), position, Quaternion.identity, GameObject.Find("Vertices").transform).GetComponent<GraphVertex>());
    }

    public void CreateEdge()
    {
        Vector3 position = ScreenCamera.transform.position;
        position.z = 0;
        edges.Add(Instantiate(Resources.Load<GameObject>("Prefabs/DirectedEdge"), position, Quaternion.identity, GameObject.Find("Edges").transform).GetComponent<DirectedEdge>());
    }

    public void Freeze()
    {
        frozen = true;
    }

    public void Unfreeze()
    {
        frozen = false;
    }

    public void NotRunningAlgorithms()
    {
        if (selectedObject != null)
        {
            if (selectedObject.tag == "Vertex")
            {
                if (selectedTime > 0)
                {
                    selectedObject.transform.position += MousePosition - prevMousePosition;
                    selectedObject.GetComponent<GraphVertex>().UpdatePosition();
                }
                else if (selectedTime > float.MinValue)
                {
                    selectedTime = float.MinValue;
                    selectedObject = null;
                }
            }
        }
        if (Input.GetMouseButtonDown(0))
        {
            if (selectedObject == null)
            {
                foreach (GraphVertex vertex in vertices)
                {
                    if (vertex.MouseTouching)
                    {
                        selectedObject = vertex.gameObject;
                        selectedTime = 0.05f;
                        return;
                    }
                }
                foreach (Edge edge in edges)
                {
                    if (edge.MouseTouching)
                    {
                        selectedObject = edge.gameObject;
                        return;
                    }
                }
            }
            
        }
        if (!Input.GetMouseButton(0))
        {
            if (selectedTime > 0)
            {
                selectedTime -= Time.deltaTime;
            }
        }
    }
}
