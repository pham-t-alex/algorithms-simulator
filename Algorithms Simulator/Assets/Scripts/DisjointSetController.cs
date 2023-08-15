using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class DisjointSetController : MonoBehaviour
{
    private static DisjointSetController disjointSetController;
    public static DisjointSetController SetController
    {
        get
        {
            if (disjointSetController == null)
            {
                disjointSetController = FindObjectOfType<DisjointSetController>();
            }
            return disjointSetController;
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

    public Vector3 MousePosition
    {
        get
        {
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePosition.z = 0;
            return mousePosition;
        }
    }

    private bool frozen = false;
    private bool usingRank = false;
    public bool UsingRank
    {
        get
        {
            return usingRank;
        }
    }
    private bool selectingPhase = false;
    private bool running = false;
    private GameObject selectedObject;
    private float selectedTime = float.MinValue;
    private Vector3 prevMousePosition;
    private float animationSpeed = 1;

    private List<DisjointSetElement> disjointSetElements = new List<DisjointSetElement>();
    [SerializeField] private GameObject algButtons;
    [SerializeField] private GameObject backButton;
    [SerializeField] private GameObject sourceText;
    [SerializeField] private GameObject runtimeUI;
    [SerializeField] private GameObject endButton;
    [SerializeField] private GameObject rankOrSize;
    [SerializeField] private GameObject vertexSelected;
    [SerializeField] private GameObject vertexSelected2;
    [SerializeField] private GameObject log;
    [SerializeField] private GameObject animSpeed;

    private string currentAlgorithm;
    
    private DisjointSetElement element1;
    private DisjointSetElement element2;

    private DisjointSetElement foundElement;

    // Start is called before the first frame update
    void Start()
    {
        sourceText.SetActive(false);
        runtimeUI.SetActive(false);
        endButton.SetActive(false);
        NotSelected();
    }

    // Update is called once per frame
    void Update()
    {
        if (!frozen)
        {
            MoveCamera();
        }
        if (selectingPhase)
        {
            SelectElements();
        }
        else if (!running)
        {
            NotRunning();
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

    public void Freeze()
    {
        frozen = true;
    }

    public void Unfreeze()
    {
        frozen = false;
    }

    public void CreateElement()
    {
        Vector3 position = ScreenCamera.transform.position;
        position.z = 0;
        disjointSetElements.Add(Instantiate(Resources.Load<GameObject>("Prefabs/DisjointSetElement"), position, Quaternion.identity, GameObject.Find("Elements").transform).GetComponent<DisjointSetElement>());
    }

    public void ToggleRankSize()
    {
        usingRank = !usingRank;
        foreach (DisjointSetElement e in disjointSetElements)
        {
            e.Reset();
        }
        if (usingRank)
        {
            rankOrSize.GetComponent<TMP_Text>().text = "current:\nRank";
        }
        else
        {
            rankOrSize.GetComponent<TMP_Text>().text = "current:\nSize";
        }
    }

    public void NotRunning()
    {
        if (selectedObject != null)
        {
            if (selectedTime > 0)
            {
                selectedObject.transform.position += MousePosition - prevMousePosition;
                selectedObject.GetComponent<DisjointSetElement>().UpdatePosition();
            }
            else if (selectedTime > float.MinValue)
            {
                selectedTime = float.MinValue;
                selectedObject.GetComponent<DisjointSetElement>().SetColor(Color.white);
                selectedObject = null;
            }
        }
        if (Input.GetMouseButtonDown(0))
        {
            if (selectedObject == null)
            {
                foreach (DisjointSetElement element in disjointSetElements)
                {
                    if (element.MouseTouching)
                    {
                        selectedObject = element.gameObject;
                        element.SetColor(new Color(0.8f, 0.8f, 0.8f));
                        selectedTime = 0.05f;
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

    public void SelectElements()
    {
        if (Input.GetMouseButtonDown(0))
        {
            foreach (DisjointSetElement element in disjointSetElements)
            {
                if (element.MouseTouching)
                {
                    if (element1 != null)
                    {
                        element2 = element;
                        RunAlgorithm();
                        return;
                    }
                    else
                    {
                        element1 = element;
                        if (currentAlgorithm != "Find")
                        {
                            element1.SetColor(new Color(0.8f, 0.8f, 0.8f));
                        }
                        else
                        {
                            RunAlgorithm(); 
                        }
                        return;
                    }
                }
            }
        }
    }

    public void Back()
    {
        if (selectingPhase)
        {
            algButtons.SetActive(true);
            currentAlgorithm = null;
            selectingPhase = false;
            sourceText.SetActive(false);
            if (element1 != null)
            {
                element1.SetColor(Color.white);
            }
            element1 = null;
            element2 = null;
        }
        else
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene("Menu");
        }
    }

    public void SelectAlgorithm(string s)
    {
        element1 = null;
        element2 = null;
        currentAlgorithm = s;
        algButtons.SetActive(false);
        selectingPhase = true;
        if (s == "Find")
        {
            sourceText.GetComponent<TMP_Text>().text = "Select an element:";
        }
        else
        {
            sourceText.GetComponent<TMP_Text>().text = "Select two elements:";
        }
        sourceText.SetActive(true);
    }

    public void RunAlgorithm()
    {
        if (currentAlgorithm == "UnionInstant")
        {
            foreach (DisjointSetElement e in disjointSetElements)
            {
                e.SetColor(Color.white);
            }
            UnionInstant();
            element1 = null;
            element2 = null;
            currentAlgorithm = null;
            selectingPhase = false;
            sourceText.SetActive(false);
            algButtons.SetActive(true);
            return;
        }
        running = true;
        selectingPhase = false;
        backButton.SetActive(false);
        sourceText.SetActive(false);

        runtimeUI.SetActive(true);
        SetLogText("");

        foreach (DisjointSetElement e in disjointSetElements)
        {
            e.CreateInfoText();
            e.SetColor(Color.white);
        }

        if (currentAlgorithm == "Find")
        {
            StartCoroutine(Find());
        }
        else
        {
            StartCoroutine(Union());
        }
    }

    public void ChangeAnimationSpeed(float f)
    {
        animationSpeed = f;
    }

    public void SetLogText(string s)
    {
        log.GetComponent<TMP_Text>().text = s;
    }

    public IEnumerator Find()
    {
        Stack<DisjointSetElement> elements = new Stack<DisjointSetElement>();
        SetLogText("Find(" + element1.ElementName + ")");
        DisjointSetElement currentElement = element1;
        SetSelectedOne(currentElement);
        DisjointSetArrow previousArrow = null;
        DisjointSetElement previousElement = null;

        yield return new WaitForSeconds(3 / animationSpeed);

        while (currentElement.Rep != currentElement)
        {
            elements.Push(currentElement);
            currentElement = currentElement.Rep;
            SetSelectedOne(currentElement);
            yield return new WaitForSeconds(2 / animationSpeed);
        }
        SetSelectedOne(currentElement);
        yield return new WaitForSeconds(1 / animationSpeed);
        DisjointSetElement rep = currentElement;

        while (elements.Count > 0)
        {
            currentElement = elements.Pop();
            SetSelectedOne(currentElement);
            currentElement.ChangeRep(rep);
            currentElement.Arrow.SetColor(new Color(0.4f, 0, 1));
            if (previousArrow != null)
            {
                previousArrow.SetColor(Color.white);
            }
            previousArrow = currentElement.Arrow;
            currentElement.UpdateInfoText();
            if (previousElement != null)
            {
                previousElement.UpdateInfoText();
            }
            previousElement = currentElement;
            yield return new WaitForSeconds(2 / animationSpeed);
        }

        if (previousArrow != null)
        {
            previousArrow.SetColor(Color.white);
        }
        currentElement.UpdateInfoText();
        if (previousElement != null)
        {
            previousElement.UpdateInfoText();
        }
        NotSelectedOne();
        SetLogText("Find(" + element1.ElementName + ") -> " + rep.ElementName);

        yield return End();
    }

    public IEnumerator Union()
    {
        SetLogText("Union(" + element1.ElementName + ", " + element2.ElementName + ")");
        yield return FindForUnion(element1, 1);
        DisjointSetElement rep1 = foundElement;
        SetSelectedOne(rep1);
        yield return FindForUnion(element2, 2);
        DisjointSetElement rep2 = foundElement;
        SetSelectedTwo(rep2);

        yield return new WaitForSeconds(1 / animationSpeed);
        DisjointSetArrow updatedArrow = null;

        if (rep1 != rep2)
        {
            if (usingRank)
            {
                if (rep1.Rank > rep2.Rank)
                {
                    rep2.ChangeRep(rep1);
                    updatedArrow = rep2.Arrow;
                }
                else
                {
                    rep1.ChangeRep(rep2);
                    if (rep1.Rank == rep2.Rank)
                    {
                        rep2.IncrementRank();
                    }
                    updatedArrow = rep1.Arrow;
                }
            }
            else
            {
                if (rep1.Size > rep2.Size)
                {
                    rep2.ChangeRep(rep1);
                    rep1.AddToSize(rep2.Size);
                    updatedArrow = rep2.Arrow;
                }
                else
                {
                    rep1.ChangeRep(rep2);
                    rep2.AddToSize(rep1.Size);
                    updatedArrow = rep1.Arrow;
                }
            }
            updatedArrow.SetColor(new Color(0.4f, 0, 1));
            rep1.UpdateInfoText();
            rep2.UpdateInfoText();
            SetLogText("Union(" + element1.ElementName + ", " + element2.ElementName + ") -> True");
        }
        else
        {
            SetLogText("Union(" + element1.ElementName + ", " + element2.ElementName + ") -> False");
        }
        yield return new WaitForSeconds(2 / animationSpeed);
        if (updatedArrow != null)
        {
            updatedArrow.SetColor(Color.white);
        }
        rep1.UpdateInfoText();
        rep2.UpdateInfoText();

        NotSelected();
        yield return End();
    }

    public IEnumerator FindForUnion(DisjointSetElement e, int side)
    {
        Stack<DisjointSetElement> elements = new Stack<DisjointSetElement>();
        DisjointSetElement currentElement;
        if (side == 1)
        {
            currentElement = element1;
        }
        else
        {
            currentElement = element2;
        }
        
        if (side == 1)
        {
            SetSelectedOne(currentElement);
        }
        else
        {
            SetSelectedTwo(currentElement);
        }
        DisjointSetArrow previousArrow = null;
        DisjointSetElement previousElement = null;

        yield return new WaitForSeconds(1 / animationSpeed);

        while (currentElement.Rep != currentElement)
        {
            elements.Push(currentElement);
            currentElement = currentElement.Rep;
            if (side == 1)
            {
                SetSelectedOne(currentElement);
            }
            else
            {
                SetSelectedTwo(currentElement);
            }
            yield return new WaitForSeconds(1 / animationSpeed);
        }
        if (side == 1)
        {
            SetSelectedOne(currentElement);
        }
        else
        {
            SetSelectedTwo(currentElement);
        }
        yield return new WaitForSeconds(1 / animationSpeed);
        DisjointSetElement rep = currentElement;
        foundElement = rep;

        while (elements.Count > 0)
        {
            currentElement = elements.Pop();
            if (side == 1)
            {
                SetSelectedOne(currentElement);
            }
            else
            {
                SetSelectedTwo(currentElement);
            }
            currentElement.ChangeRep(rep);
            currentElement.Arrow.SetColor(new Color(0.4f, 0, 1));
            if (previousArrow != null)
            {
                previousArrow.SetColor(Color.white);
            }
            previousArrow = currentElement.Arrow;
            currentElement.UpdateInfoText();
            if (previousElement != null)
            {
                previousElement.UpdateInfoText();
            }
            previousElement = currentElement;
            yield return new WaitForSeconds(1 / animationSpeed);
        }

        if (previousArrow != null)
        {
            previousArrow.SetColor(Color.white);
        }
        currentElement.UpdateInfoText();
        if (previousElement != null)
        {
            previousElement.UpdateInfoText();
        }

        yield return null;
    }

    public void UnionInstant()
    {
        DisjointSetElement rep1 = FindForUnionInstant(element1);
        DisjointSetElement rep2 = FindForUnionInstant(element2);

        if (rep1 != rep2)
        {
            if (usingRank)
            {
                if (rep1.Rank > rep2.Rank)
                {
                    rep2.ChangeRep(rep1);
                }
                else
                {
                    rep1.ChangeRep(rep2);
                    if (rep1.Rank == rep2.Rank)
                    {
                        rep2.IncrementRank();
                    }
                }
            }
            else
            {
                if (rep1.Size > rep2.Size)
                {
                    rep2.ChangeRep(rep1);
                    rep1.AddToSize(rep2.Size);
                }
                else
                {
                    rep1.ChangeRep(rep2);
                    rep2.AddToSize(rep1.Size);
                }
            }
        }
    }

    public DisjointSetElement FindForUnionInstant(DisjointSetElement e)
    {
        Stack<DisjointSetElement> elements = new Stack<DisjointSetElement>();
        DisjointSetElement currentElement = e;

        while (currentElement.Rep != currentElement)
        {
            elements.Push(currentElement);
            currentElement = currentElement.Rep;
        }

        DisjointSetElement rep = currentElement;

        while (elements.Count > 0)
        {
            currentElement = elements.Pop();
            currentElement.ChangeRep(rep);
        }

        return rep;
    }

    public IEnumerator End()
    {
        yield return new WaitForSeconds(3 / animationSpeed);
        endButton.SetActive(true);
    }

    public void EndAlgorithm()
    {
        endButton.SetActive(false);
        running = false;
        runtimeUI.SetActive(false);
        SetLogText("");

        foreach (DisjointSetElement e in disjointSetElements)
        {
            e.DestroyInfoText();
            e.SetColor(Color.white);
            e.Arrow.SetColor(Color.white);
        }

        backButton.SetActive(true);
        currentAlgorithm = null;
        algButtons.SetActive(true);
        element1 = null;
        element2 = null;
    }

    public void SetSelectedOne(DisjointSetElement e)
    {
        vertexSelected.transform.position = e.transform.position;
        vertexSelected.GetComponent<SpriteRenderer>().enabled = true;
    }

    public void SetSelectedTwo(DisjointSetElement e)
    {
        vertexSelected2.transform.position = e.transform.position;
        vertexSelected2.GetComponent<SpriteRenderer>().enabled = true;
    }

    public void NotSelectedOne()
    {
        vertexSelected.GetComponent<SpriteRenderer>().enabled = false;
    }
    public void NotSelectedTwo()
    {
        vertexSelected2.GetComponent<SpriteRenderer>().enabled = false;
    }

    public void NotSelected()
    {
        NotSelectedOne();
        NotSelectedTwo();
    }

}
