using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;
using UnityEngine.UI;

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
    private List<Edge> edges = new List<Edge>();

    private bool frozen = false;
    private bool runningAlgorithm = false;
    private bool selectingPhase = false;
    [SerializeField] private GameObject selectedObject;
    private GraphVertex edgeSourceSelected;
    private float selectedTime = float.MinValue;
    private Vector3 prevMousePosition;

    private GraphVertex sourceVertex;

    [SerializeField] private GameObject weightedCheckmark;
    [SerializeField] private GameObject createGraphUI;
    [SerializeField] private GameObject algsMenu;
    [SerializeField] private List<GameObject> algButtons;
    [SerializeField] private GameObject backButton;
    [SerializeField] private GameObject sourceVertexText;
    [SerializeField] private GameObject runtimeUI;
    [SerializeField] private GameObject algLog;
    [SerializeField] private GameObject animSpeed;
    [SerializeField] private GameObject vertexSelected;
    [SerializeField] private GameObject dfsKey;
    [SerializeField] private GameObject endButton;

    private bool weighted = true;
    public bool Weighted
    {
        get
        {
            return weighted;
        }
    }

    public Vector3 MousePosition
    {
        get
        {
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePosition.z = 0;
            return mousePosition;
        }
    }

    private string currentAlgorithm;
    private readonly string[] algsNeedingStart = { "BFS", "DFS", "BellmanFord", "Dijkstra", "DAGSSSP" };
    private float animationSpeed = 1;

    // Start is called before the first frame update
    void Start()
    {
        NoVertexSelected();
        algsMenu.SetActive(false);
        sourceVertexText.SetActive(false);
        runtimeUI.SetActive(false);
        dfsKey.SetActive(false);
        endButton.SetActive(false);
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
        else if (selectingPhase)
        {
            SelectVertex();
        }
        else
        {
            if (!algsMenu.activeSelf)
            {
                NotRunningAlgorithms();
            }
            
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
        edges.Add(Instantiate(Resources.Load<GameObject>("Prefabs/DirectedEdge"), position, Quaternion.identity, GameObject.Find("Edges").transform).GetComponent<Edge>());
    }

    public void Freeze()
    {
        frozen = true;
    }

    public void Unfreeze()
    {
        frozen = false;
    }

    public void SelectVertex()
    {
        if (Input.GetMouseButtonDown(0))
        {
            foreach (GraphVertex vertex in vertices)
            {
                if (vertex.MouseTouching)
                {
                    sourceVertex = vertex;
                    RunAlgorithm();
                    return;
                }
            }
        }
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
                    selectedObject.GetComponent<GraphVertex>().SetColor(Color.white);
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
                        vertex.SetColor(new Color(0.8f, 0.8f, 0.8f));
                        selectedTime = 0.05f;
                        return;
                    }
                }
                foreach (Edge edge in edges)
                {
                    if (edge.MouseTouching)
                    {
                        selectedObject = edge.gameObject;
                        edge.SetColor(new Color(0.8f, 0.8f, 0.8f));
                        return;
                    }
                }
            }
            else if (selectedObject.tag == "Edge")
            {
                bool foundNothing = true;
                foreach (GraphVertex vertex in vertices)
                {
                    if (vertex.MouseTouching)
                    {
                        foundNothing = false;
                        if (edgeSourceSelected != null)
                        {
                            selectedObject.GetComponent<Edge>().SetVertices(edgeSourceSelected, vertex);
                            selectedObject.GetComponent<Edge>().SetColor(Color.white);
                            selectedObject = null;
                            edgeSourceSelected.SetColor(Color.white);
                            edgeSourceSelected = null;
                            return;
                        }
                        else
                        {
                            edgeSourceSelected = vertex;
                            edgeSourceSelected.SetColor(new Color(0.8f, 0.8f, 0.8f));
                            return;
                        }
                    }
                }
                if (foundNothing)
                {
                    selectedObject.GetComponent<Edge>().SetColor(Color.white);
                    selectedObject = null;
                    if (edgeSourceSelected != null)
                    {
                        edgeSourceSelected.SetColor(Color.white);
                    }
                    edgeSourceSelected = null;
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

    public void Back()
    {
        if (selectingPhase)
        {
            algsMenu.SetActive(true);
            currentAlgorithm = null;
            selectingPhase = false;
            sourceVertexText.SetActive(false);
        }
        else if (algsMenu.activeSelf)
        {
            algsMenu.SetActive(false);
            createGraphUI.SetActive(true);
        }
        else
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene("Menu");
        }
    }

    public void ToggleWeighted()
    {
        if (weighted)
        {
            weighted = false;
            weightedCheckmark.SetActive(false);
            foreach (Edge e in edges)
            {
                e.HideWeight();
            }
        }
        else
        {
            weighted = true;
            weightedCheckmark.SetActive(true);
            foreach (Edge e in edges)
            {
                e.ShowWeight();
            }
        }
    }

    public void OpenAlgMenu()
    {
        algsMenu.SetActive(true);
        createGraphUI.SetActive(false);

        foreach (GameObject button in algButtons)
        {
            if (button.tag == "WeightedButton")
            {
                button.GetComponent<UnityEngine.UI.Button>().interactable = weighted;
            }
        }
    }

    public void SelectAlgorithm(string s)
    {
        sourceVertex = null;
        currentAlgorithm = s;
        algsMenu.SetActive(false);
        bool requiresStart = false;
        foreach (string alg in algsNeedingStart)
        {
            if (currentAlgorithm == alg)
            {
                requiresStart = true;
                break;
            }
        }
        if (requiresStart)
        {
            selectingPhase = true;
            sourceVertexText.SetActive(true);
        }
        else
        {
            RunAlgorithm();
        }
    }

    public void RunAlgorithm()
    {
        runningAlgorithm = true;
        selectingPhase = false;
        backButton.SetActive(false);
        sourceVertexText.SetActive(false);

        runtimeUI.SetActive(true);
        SetLogText("");

        vertices.Sort(new VertexComparer());

        for (int i = 0; i < edges.Count; i++)
        {
            if (edges[i].Source == null && edges[i].Destination == null)
            {
                Edge e = edges[i];
                edges.RemoveAt(i);
                e.Destroy();
                i--;
            }
        }

        edges.Sort(new EdgeComparer());
        foreach (GraphVertex v in vertices)
        {
            v.CreateInfoText();
            v.SortEdges();
            v.SetColor(Color.white);
        }
        
        foreach (Edge e in edges)
        {
            e.CreateInfoText();
            e.SetColor(Color.white);
        }
        UnreverseEdges();

        InitializeAlgorithm();

        foreach (GraphVertex v in vertices)
        {
            v.UpdateInfoText();
        }

        switch (currentAlgorithm)
        {
            case "BFS":
                StartCoroutine(BFS());
                break;
            case "DFS":
                StartCoroutine(DFS());
                break;
            case "Kahn":
                StartCoroutine(Kahn());
                break;
            case "DFSTopSort":
                StartCoroutine(DFSTopSort());
                break;
            case "Kosaraju":
                StartCoroutine(Kosaraju());
                break;
            case "BellmanFord":
                StartCoroutine(BellmanFord());
                break;
            case "Dijkstra":
                StartCoroutine(Dijkstra());
                break;
            case "DAGSSSP":
                StartCoroutine(DAGSSSP());
                break;
        }
    }

    public void InitializeAlgorithm()
    {
        switch (currentAlgorithm)
        {
            case "BFS":
                foreach (GraphVertex v in vertices)
                {
                    v.AddVariable("disc.", false);
                    v.AddVariable("d", float.PositiveInfinity);
                    v.AddVariable("pi", null);
                }
                sourceVertex.SetVariable("disc.", true);
                sourceVertex.SetVariable("d", 0);
                sourceVertex.SetColor(new Color(0.8f, 0.8f, 0.8f));
                break;
            case "DFS":
                foreach (GraphVertex v in vertices)
                {
                    v.AddVariable("disc.", -1);
                    v.AddVariable("fin.", -1);
                    v.AddVariable("pi", null);
                }
                break;
            case "Kahn":
                foreach (GraphVertex v in vertices)
                {
                    v.AddVariable("in.", v.IncomingEdges.Count);
                }
                break;
            case "DFSTopSort":
                foreach (GraphVertex v in vertices)
                {
                    v.AddVariable("disc.", -1);
                    v.AddVariable("fin.", -1);
                }
                break;
            case "Kosaraju":
                foreach (GraphVertex v in vertices)
                {
                    v.AddVariable("comp.", null);
                    v.AddVariable("disc.", -1);
                    v.AddVariable("fin.", -1);
                }
                break;
            case "BellmanFord":
            case "Dijkstra":
                foreach (GraphVertex v in vertices)
                {
                    v.AddVariable("d", float.PositiveInfinity);
                    v.AddVariable("pi", null);
                }
                sourceVertex.SetVariable("d", 0f);
                break;
            case "DAGSSSP":
                foreach (GraphVertex v in vertices)
                {
                    v.AddVariable("disc.", -1);
                    v.AddVariable("fin.", -1);
                }
                break;
        }
    }

    public void ChangeAnimationSpeed(float f)
    {
        animationSpeed = f;
    }

    public void SetLogText(string s)
    {
        algLog.GetComponent<TMP_Text>().text = s;
    }

    public void AddToLogText(string s)
    {
        algLog.GetComponent<TMP_Text>().text += s;
    }

    public IEnumerator BFS()
    {
        List<GraphVertex> newAddedElements = new List<GraphVertex>();
        List<GraphVertex> vertexQueue = new List<GraphVertex>();
        List<GraphVertex> previousVertices = new List<GraphVertex>();
        newAddedElements.Add(sourceVertex);
        vertexQueue.Add(sourceVertex);
        SetLogText(ListToString(vertexQueue, newAddedElements, "Queue"));
        newAddedElements.Clear();
        yield return new WaitForSeconds(3 / animationSpeed);
        while (vertexQueue.Count > 0)
        {
            GraphVertex currVertex = vertexQueue[0];
            SetVertexSelected(currVertex);
            vertexQueue.RemoveAt(0);
            SetLogText(ListToString(vertexQueue, null, "Queue"));
            yield return new WaitForSeconds(2 / animationSpeed);
            foreach (Edge e in currVertex.OutgoingEdges)
            {
                e.SetColor(new Color(0.6f, 0.6f, 0.6f));
                GraphVertex destination = e.Destination;
                if (!((bool) destination.GetVariable("disc.")))
                {
                    destination.SetVariable("disc.", true);
                    destination.SetVariable("d", ((int) currVertex.GetVariable("d")) + 1);
                    destination.SetVariable("pi", currVertex);
                    destination.SetColor(new Color(0.8f, 0.8f, 0.8f));
                    destination.UpdateInfoText();
                    previousVertices.Add(destination);
                    vertexQueue.Add(destination);
                    newAddedElements.Add(destination);
                    SetLogText(ListToString(vertexQueue, newAddedElements, "Queue"));
                    yield return new WaitForSeconds(1 / animationSpeed);
                }
            }
            newAddedElements.Clear();
            previousVertices.Clear();
            currVertex.SetColor(new Color(0.6f, 0.6f, 0.6f));
            NoVertexSelected();
            SetLogText(ListToString(vertexQueue, null, "Queue"));
            yield return new WaitForSeconds(3 / animationSpeed);

            foreach (GraphVertex vertex in previousVertices)
            {
                vertex.UpdateInfoText();
            }
        }
        SetLogText(ListToString(vertexQueue, null, "Queue"));
        yield return End();
    }

    public string ListToString<T>(List<T> list, List<T> newAddedElements, string listName)
    {
        string s = $"{listName}: ";
        if (list.Count > 0)
        {
            if (newAddedElements != null && newAddedElements.Contains(list[0]))
            {
                s += "<color=#e3d1ffff>" + list[0].ToString() + "</color>";
            }
            else
            {
                s += list[0].ToString();
            }
            for (int i = 1; i < list.Count; i++)
            {
                if (newAddedElements != null && newAddedElements.Contains(list[i]))
                {
                    s += ", <color=#e3d1ffff>" + list[i].ToString() + "</color>";
                }
                else
                {
                    s += ", " + list[i].ToString();
                }
            }
        }
        else
        {
            s += "[empty]";
        }
        return s;
    }

    public IEnumerator DFS()
    {
        dfsKey.SetActive(true);
        SetLogText("Parentheses:");
        int time = 1;
        Stack<object> stack = new Stack<object>();
        stack.Push(sourceVertex);
        GraphVertex previousVertex = null;
        yield return new WaitForSeconds(3 / animationSpeed);
        while (stack.Count > 0)
        {
            object obj = stack.Pop();
            if (obj is GraphVertex)
            {
                GraphVertex v = (GraphVertex) obj;
                if ((int) v.GetVariable("disc.") < 0)
                {
                    SetVertexSelected(v);
                    v.SetVariable("disc.", time);
                    time++;
                    AddToLogText($" <size=10>{v.VertexName}</size>(");
                    v.SetColor(new Color(0.8f, 0.8f, 0.8f));
                    v.UpdateInfoText();
                    if (previousVertex != null)
                    {
                        previousVertex.UpdateInfoText();
                    }
                    previousVertex = v;
                    yield return new WaitForSeconds(2 / animationSpeed);
                    stack.Push(v);
                    for (int i = v.OutgoingEdges.Count - 1; i >= 0; i--)
                    {
                        stack.Push(v.OutgoingEdges[i]);
                    }
                }
                else if ((int) v.GetVariable("fin.") < 0)
                {
                    SetVertexSelected(v);
                    v.SetVariable("fin.", time);
                    time++;
                    AddToLogText($" )<size=10>{v.VertexName}</size>");
                    v.SetColor(new Color(0.6f, 0.6f, 0.6f));
                    v.UpdateInfoText();
                    if (previousVertex != null && previousVertex != v)
                    {
                        previousVertex.UpdateInfoText();
                    }
                    previousVertex = v;
                    yield return new WaitForSeconds(2 / animationSpeed);
                }
            }
            else
            {
                Edge e = (Edge) obj;
                GraphVertex v = e.Destination;

                if ((int) v.GetVariable("disc.") < 0)
                {
                    v.SetVariable("pi", e.Source);
                    stack.Push(v);
                    e.SetColor(new Color(0.6f, 0.6f, 0.6f));
                }
                else if ((int) v.GetVariable("fin.") < 0)
                {
                    e.SetColor(new Color(0.8f, 0.35f, 0));
                }
                else if ((int) v.GetVariable("disc.") > (int) e.Source.GetVariable("disc."))
                {
                    e.SetColor(new Color(0, 0.75f, 0.8f));
                }
                else
                {
                    e.SetColor(new Color(0, 0.8f, 0.4f));
                }
            }
        }
        NoVertexSelected();
        if (previousVertex != null)
        {
            previousVertex.UpdateInfoText();
        }
        yield return End();
    }

    public IEnumerator Kahn()
    {
        List<GraphVertex> topSorted = new List<GraphVertex>();
        List<GraphVertex> newAdditions = new List<GraphVertex>();
        List<GraphVertex> previousVertices = new List<GraphVertex>();
        foreach (GraphVertex v in vertices)
        {
            if ((int) v.GetVariable("in.") == 0)
            {
                topSorted.Add(v);
                newAdditions.Add(v);
            }
        }
        SetLogText(ListToString(topSorted, newAdditions, "List"));
        newAdditions.Clear();
        yield return new WaitForSeconds(3 / animationSpeed);
        int index = 0;
        while (index < topSorted.Count)
        {
            GraphVertex v = topSorted[index];
            SetVertexSelected(v);
            foreach (Edge e in v.OutgoingEdges)
            {
                GraphVertex destination = e.Destination;
                destination.SetVariable("in.", (int) destination.GetVariable("in.") - 1);
                destination.UpdateInfoText();
                if ((int) destination.GetVariable("in.") == 0)
                {
                    topSorted.Add(destination);
                    newAdditions.Add(destination);
                    previousVertices.Add(destination);
                    SetLogText(ListToString(topSorted, newAdditions, "List"));
                }
                yield return new WaitForSeconds(1 / animationSpeed);
            }
            newAdditions.Clear();
            NoVertexSelected();
            SetLogText(ListToString(topSorted, null, "List"));
            index++;
            yield return new WaitForSeconds(3 / animationSpeed);

            foreach (GraphVertex vertex in previousVertices)
            {
                vertex.UpdateInfoText();
            }
        }
        yield return End();
    }

    public IEnumerator DFSTopSort()
    {
        int time = 1;
        Stack<GraphVertex> stack = new Stack<GraphVertex>();
        List<GraphVertex> topSort = new List<GraphVertex>();
        List<GraphVertex> newAddition = new List<GraphVertex>();
        GraphVertex previousVertex = null;
        SetLogText(ListToString(topSort, null, "List"));
        yield return new WaitForSeconds(3 / animationSpeed);
        foreach (GraphVertex vertex in vertices)
        {
            stack.Push(vertex);

            while (stack.Count > 0)
            {
                GraphVertex v = stack.Pop();
                if ((int) v.GetVariable("disc.") < 0)
                {
                    SetVertexSelected(v);
                    v.SetVariable("disc.", time);
                    time++;
                    v.SetColor(new Color(0.8f, 0.8f, 0.8f));
                    v.UpdateInfoText();
                    if (previousVertex != null)
                    {
                        previousVertex.UpdateInfoText();
                    }
                    previousVertex = v;
                    yield return new WaitForSeconds(2 / animationSpeed);
                    stack.Push(v);
                    for (int i = v.OutgoingEdges.Count - 1; i >= 0; i--)
                    {
                        GraphVertex destination = v.OutgoingEdges[i].Destination;
                        if ((int) destination.GetVariable("disc.") < 0)
                        {
                            stack.Push(destination);
                        }
                    }
                }
                else if ((int)v.GetVariable("fin.") < 0)
                {
                    SetVertexSelected(v);
                    v.SetVariable("fin.", time);
                    topSort.Insert(0, v);
                    newAddition.Add(v);
                    SetLogText(ListToString(topSort, newAddition, "List"));
                    newAddition.Clear();
                    time++;
                    v.SetColor(new Color(0.6f, 0.6f, 0.6f));
                    v.UpdateInfoText();
                    if (previousVertex != null && previousVertex != v)
                    {
                        previousVertex.UpdateInfoText();
                    }
                    previousVertex = v;
                    yield return new WaitForSeconds(2 / animationSpeed);
                }
            }
            SetLogText(ListToString(topSort, null, "List"));
            NoVertexSelected();
            if (previousVertex != null)
            {
                previousVertex.UpdateInfoText();
            }
            previousVertex = null;
            yield return new WaitForSeconds(1 / animationSpeed);
        }

        yield return End();
    }

    public IEnumerator Kosaraju()
    {
        List<GraphVertex> topSort = new List<GraphVertex>();
        yield return DFSTopSortSubroutine(topSort);

        yield return new WaitForSeconds(1 / animationSpeed);

        Edge previousEdge = null;
        foreach (Edge e in edges)
        {
            if (!e.SelfEdge)
            {
                e.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Sprites/directededgeReversed");
            }
            e.SetColor(new Color(0.4f, 0, 1));
            if (previousEdge != null)
            {
                previousEdge.SetColor(Color.white);
            }
            previousEdge = e;
            yield return new WaitForSeconds(1 / animationSpeed);
        }
        if (previousEdge != null)
        {
            previousEdge.SetColor(Color.white);
        }

        yield return new WaitForSeconds(1 / animationSpeed);

        Stack<GraphVertex> stack = new Stack<GraphVertex>();
        foreach (GraphVertex v in vertices)
        {
            v.SetVariable("disc.", -1);
            v.SetVariable("fin.", -1);
            v.SetColor(Color.white);
            v.UpdateInfoText();
        }
        yield return new WaitForSeconds(2 / animationSpeed);
        foreach (GraphVertex v in vertices)
        {
            v.UpdateInfoText();
        }
        GraphVertex previousVertex = null;
        int time = 1;

        List<GraphVertex> currentVertex = new List<GraphVertex>();
        string currentComponent = "";
        string components = "";

        foreach (GraphVertex vertex in topSort)
        {
            stack.Push(vertex);
            currentVertex.Add(vertex);
            SetLogText(ListToString(topSort, currentVertex, "L"));
            AddToLogText(" | Comps: " + components);

            while (stack.Count > 0)
            {
                GraphVertex v = stack.Pop();
                if ((int)v.GetVariable("disc.") < 0)
                {
                    SetVertexSelected(v);
                    v.SetVariable("disc.", time);
                    v.SetVariable("comp.", vertex);
                    time++;
                    v.SetColor(new Color(0.8f, 0.8f, 0.8f));
                    v.UpdateInfoText();
                    if (previousVertex != null)
                    {
                        previousVertex.UpdateInfoText();
                    }
                    previousVertex = v;
                    if (currentComponent == "")
                    {
                        if (components == "")
                        {
                            AddToLogText("<color=#e3d1ffff>" + v.VertexName + "</color>");
                        }
                        else
                        {
                            AddToLogText(", <color=#e3d1ffff>" + v.VertexName + "</color>");
                        }
                    }
                    else
                    {
                        AddToLogText("<color=#e3d1ffff>" + v.VertexName + "</color>");
                    }
                    currentComponent += v.VertexName;
                    yield return new WaitForSeconds(2 / animationSpeed);
                    stack.Push(v);
                    for (int i = v.IncomingEdges.Count - 1; i >= 0; i--)
                    {
                        GraphVertex destination = v.IncomingEdges[i].Source;
                        if ((int)destination.GetVariable("disc.") < 0)
                        {
                            stack.Push(destination);
                        }
                    }
                }
                else if ((int)v.GetVariable("fin.") < 0)
                {
                    SetVertexSelected(v);
                    v.SetVariable("fin.", time);
                    time++;
                    v.SetColor(new Color(0.6f, 0.6f, 0.6f));
                    v.UpdateInfoText();
                    if (previousVertex != null && previousVertex != v)
                    {
                        previousVertex.UpdateInfoText();
                    }
                    previousVertex = v;
                    yield return new WaitForSeconds(2 / animationSpeed);
                }
            }
            if (currentComponent != "")
            {
                if (components == "")
                {
                    components += currentComponent;
                }
                else
                {
                    components += ", " + currentComponent;
                }
            }
            currentComponent = "";
            NoVertexSelected();
            if (previousVertex != null)
            {
                previousVertex.UpdateInfoText();
            }
            previousVertex = null;
            yield return new WaitForSeconds(1 / animationSpeed);
            currentVertex.Clear();
        }

        SetLogText(ListToString(topSort, null, "L"));
        AddToLogText(" | Comps: " + components);
        UnreverseEdges();
        yield return End();
    }

    public IEnumerator DFSTopSortSubroutine(List<GraphVertex> topSort)
    {
        int time = 1;
        Stack<GraphVertex> stack = new Stack<GraphVertex>();
        List<GraphVertex> newAddition = new List<GraphVertex>();
        GraphVertex previousVertex = null;
        SetLogText(ListToString(topSort, null, "List"));
        yield return new WaitForSeconds(3 / animationSpeed);
        foreach (GraphVertex vertex in vertices)
        {
            stack.Push(vertex);

            while (stack.Count > 0)
            {
                GraphVertex v = stack.Pop();
                if ((int)v.GetVariable("disc.") < 0)
                {
                    SetVertexSelected(v);
                    v.SetVariable("disc.", time);
                    time++;
                    v.SetColor(new Color(0.8f, 0.8f, 0.8f));
                    v.UpdateInfoText();
                    if (previousVertex != null)
                    {
                        previousVertex.UpdateInfoText();
                    }
                    previousVertex = v;
                    yield return new WaitForSeconds(2 / animationSpeed);
                    stack.Push(v);
                    for (int i = v.OutgoingEdges.Count - 1; i >= 0; i--)
                    {
                        GraphVertex destination = v.OutgoingEdges[i].Destination;
                        if ((int)destination.GetVariable("disc.") < 0)
                        {
                            stack.Push(destination);
                        }
                    }
                }
                else if ((int)v.GetVariable("fin.") < 0)
                {
                    SetVertexSelected(v);
                    v.SetVariable("fin.", time);
                    topSort.Insert(0, v);
                    newAddition.Add(v);
                    SetLogText(ListToString(topSort, newAddition, "List"));
                    newAddition.Clear();
                    time++;
                    v.SetColor(new Color(0.6f, 0.6f, 0.6f));
                    v.UpdateInfoText();
                    if (previousVertex != null && previousVertex != v)
                    {
                        previousVertex.UpdateInfoText();
                    }
                    previousVertex = v;
                    yield return new WaitForSeconds(2 / animationSpeed);
                }
            }
            SetLogText(ListToString(topSort, null, "List"));
            NoVertexSelected();
            if (previousVertex != null)
            {
                previousVertex.UpdateInfoText();
            }
            previousVertex = null;
            yield return new WaitForSeconds(1 / animationSpeed);
        }
    }

    public IEnumerator BellmanFord()
    {
        GraphVertex previousVertex = null;
        Edge previousEdge = null;
        List<Edge> currentEdge = new List<Edge>();
        SetLogText("0/" + (vertices.Count - 1) + " | " + ListToString(edges, null, "Edges"));
        yield return new WaitForSeconds(3 / animationSpeed);
        for (int i = 1; i < vertices.Count; i++)
        {
            foreach (Edge e in edges)
            {
                currentEdge.Add(e);
                SetLogText(i + "/" + (vertices.Count - 1) + " | " + ListToString(edges, currentEdge, "Edges"));
                e.SetColor(new Color(0.4f, 0, 1));
                if (previousEdge != null)
                {
                    previousEdge.SetColor(Color.white);
                }
                previousEdge = e;
                RelaxEdge(e);
                if (previousVertex != null)
                {
                    previousVertex.UpdateInfoText();
                }
                previousVertex = e.Destination;
                yield return new WaitForSeconds(2 / animationSpeed);
                currentEdge.Clear();
            }
            SetLogText(i + "/" + (vertices.Count - 1) + " | " + ListToString(edges, null, "Edges"));
            if (previousEdge != null)
            {
                previousEdge.SetColor(Color.white);
            }
            if (previousVertex != null)
            {
                previousVertex.UpdateInfoText();
            }
            yield return new WaitForSeconds(2 / animationSpeed);
        }
        yield return End();
    }

    public IEnumerator Dijkstra()
    {
        PriorityQueue<GraphVertex> priorityQueue = new PriorityQueue<GraphVertex>();
        List<GraphVertex> highlights = new List<GraphVertex>();
        GraphVertex previousVertex = null;
        Edge previousEdge = null;
        foreach (GraphVertex v in vertices)
        {
            priorityQueue.AddElement((float) v.GetVariable("d"), v);
        }
        SetLogText(PriorityQueueToString(priorityQueue, null, "P.Queue"));
        yield return new WaitForSeconds(3 / animationSpeed);

        while (priorityQueue.Count > 0)
        {
            GraphVertex v = priorityQueue.Remove();
            SetVertexSelected(v);
            SetLogText(PriorityQueueToString(priorityQueue, null, "P.Queue"));
            yield return new WaitForSeconds(1 / animationSpeed);
            foreach (Edge e in v.OutgoingEdges)
            {
                RelaxEdge(e);
                e.SetColor(new Color(0.4f, 0, 1));
                if (previousEdge != null)
                {
                    previousEdge.SetColor(Color.white);
                }
                previousEdge = e;
                GraphVertex destination = e.Destination;
                if (previousVertex != null)
                {
                    previousVertex.UpdateInfoText();
                }
                previousVertex = destination;
                bool change = priorityQueue.DecreaseKey((float)destination.GetVariable("d"), destination);
                if (change)
                {
                    highlights.Add(destination);
                }
                SetLogText(PriorityQueueToString(priorityQueue, highlights, "P.Queue"));
                highlights.Clear();
                yield return new WaitForSeconds(2 / animationSpeed);
            }
            NoVertexSelected();
            SetLogText(PriorityQueueToString(priorityQueue, null, "P.Queue"));
            if (previousEdge != null)
            {
                previousEdge.SetColor(Color.white);
            }
            previousEdge = null;
            if (previousVertex != null)
            {
                previousVertex.UpdateInfoText();
            }
            previousVertex = null;
            yield return new WaitForSeconds(1 / animationSpeed);
        }

        yield return End();
    }

    public string PriorityQueueToString<T>(PriorityQueue<T> priorityQueue, List<T> highlights, string queueName)
    {
        string s = $"{queueName}: ";
        if (priorityQueue.Count > 0)
        {
            List<T> elements = priorityQueue.Elements();
            List<float> keys = priorityQueue.Keys();
            string keyAsString = keys[0].ToString();
            if (float.IsPositiveInfinity(keys[0]))
            {
                keyAsString = "Inf.";
            }
            if (highlights != null && highlights.Contains(elements[0]))
            {
                s += "<color=#e3d1ffff>" + elements[0].ToString() + "<size=10>" + keyAsString + "</size></color>";
            }
            else
            {
                s += elements[0].ToString() + "<size=10>" + keyAsString + "</size>";
            }
            for (int i = 1; i < priorityQueue.Count; i++)
            {
                keyAsString = keys[i].ToString();
                if (float.IsPositiveInfinity(keys[i]))
                {
                    keyAsString = "Inf.";
                }
                if (highlights != null && highlights.Contains(elements[i]))
                {
                    s += ", <color=#e3d1ffff>" + elements[i].ToString() + "<size=10>" + keyAsString + "</size></color>";
                }
                else
                {
                    s += ", " + elements[i].ToString() + "<size=10>" + keyAsString + "</size>";
                }
            }
        }
        else
        {
            s += "[empty]";
        }
        return s;
    }
    
    public IEnumerator DAGSSSP()
    {
        List<GraphVertex> topSort = new List<GraphVertex>();
        yield return DFSTopSortSubroutine(topSort);

        yield return new WaitForSeconds(1 / animationSpeed);

        foreach (GraphVertex v in vertices)
        {
            v.RemoveVariable("disc.");
            v.RemoveVariable("fin.");
            v.AddVariable("d", float.PositiveInfinity);
            v.AddVariable("pi", null);
            v.SetColor(Color.white);
            v.UpdateInfoText();
        }
        sourceVertex.SetVariable("d", 0f);
        sourceVertex.UpdateInfoText();
        SetLogText(ListToString(topSort, null, "L"));

        yield return new WaitForSeconds(1 / animationSpeed);

        List<GraphVertex> highlights = new List<GraphVertex>();
        GraphVertex previousVertex = null;
        Edge previousEdge = null;

        foreach (GraphVertex v in topSort)
        {
            SetVertexSelected(v);
            highlights.Add(v);
            SetLogText(ListToString(topSort, highlights, "L"));
            highlights.Clear();
            yield return new WaitForSeconds(1 / animationSpeed);
            foreach (Edge e in v.OutgoingEdges)
            {
                RelaxEdge(e);
                e.SetColor(new Color(0.4f, 0, 1));
                if (previousEdge != null)
                {
                    previousEdge.SetColor(Color.white);
                }
                previousEdge = e;
                GraphVertex destination = e.Destination;
                if (previousVertex != null)
                {
                    previousVertex.UpdateInfoText();
                }
                previousVertex = destination;
                yield return new WaitForSeconds(2 / animationSpeed);
            }
            NoVertexSelected();
            SetLogText(ListToString(topSort, null, "L"));
            if (previousEdge != null)
            {
                previousEdge.SetColor(Color.white);
            }
            previousEdge = null;
            if (previousVertex != null)
            {
                previousVertex.UpdateInfoText();
            }
            previousVertex = null;
            yield return new WaitForSeconds(1 / animationSpeed);
        }

        yield return End();
    }

    public IEnumerator End()
    {
        yield return new WaitForSeconds(3 / animationSpeed);
        endButton.SetActive(true);
    }

    public void EndAlgorithm()
    {
        endButton.SetActive(false);
        runningAlgorithm = false;
        runtimeUI.SetActive(false);
        SetLogText("");

        foreach (GraphVertex v in vertices)
        {
            v.ClearVariables();
            v.DestroyInfoText();
            v.SetColor(Color.white);
        }
        foreach (Edge e in edges)
        {
            e.DestroyInfoText();
            e.SetColor(Color.white);
        }
        backButton.SetActive(true);
        currentAlgorithm = null;
        createGraphUI.SetActive(true);
        sourceVertex = null;
    }

    public void RelaxEdge(Edge e)
    {
        GraphVertex source = e.Source;
        GraphVertex destination = e.Destination;
        if ((float) destination.GetVariable("d") > ((float) source.GetVariable("d") + e.Weight))
        {
            destination.SetVariable("d", (float) source.GetVariable("d") + e.Weight);
            destination.SetVariable("pi", source);
            destination.UpdateInfoText();
        }
    }

    public void SetVertexSelected(GraphVertex v)
    {
        vertexSelected.transform.position = v.transform.position;
        vertexSelected.GetComponent<SpriteRenderer>().enabled = true;
    }

    public void UnreverseEdges()
    {
        foreach (Edge e in edges)
        {
            if (!e.SelfEdge)
            {
                e.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Sprites/directededge");
            }
        }
    }

    public void NoVertexSelected()
    {
        vertexSelected.GetComponent<SpriteRenderer>().enabled = false;
    }
}
