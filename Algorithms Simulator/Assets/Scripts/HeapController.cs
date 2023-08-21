using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class HeapController : MonoBehaviour
{
    private static HeapController heapController;
    public static HeapController HeapMainController
    {
        get
        {
            if (heapController == null)
            {
                heapController = FindObjectOfType<HeapController>();
            }
            return heapController;
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
    private bool usingMin = false;
    public bool UsingMin
    {
        get
        {
            return usingMin;
        }
    }

    private bool selectingPhase = false;
    private bool running = false;
    private GameObject selectedObject;
    private float selectedTime = float.MinValue;
    private Vector3 prevMousePosition;
    private float animationSpeed = 1;

    private List<HeapElement> elements = new List<HeapElement>();

    [SerializeField] private GameObject heapAsArray;
    [SerializeField] private GameObject heapAsArrayText;
    [SerializeField] private GameObject defaultUI;
    [SerializeField] private GameObject maxCheck;
    [SerializeField] private GameObject minCheck;
    [SerializeField] private GameObject algMenu;
    [SerializeField] private GameObject backButton;
    [SerializeField] private GameObject sourceText;
    [SerializeField] private GameObject runtimeUI;
    [SerializeField] private GameObject algLog;
    [SerializeField] private GameObject animSpeed;
    [SerializeField] private GameObject endButton;
    [SerializeField] private GameObject addUI;
    [SerializeField] private GameObject elementSelected;
    [SerializeField] private GameObject elementSelected2;
    [SerializeField] private GameObject insertInput;

    private HeapElement sourceElement;
    private string currentAlg;

    // Start is called before the first frame update
    void Start()
    {
        minCheck.SetActive(false);
        algMenu.SetActive(false);
        sourceText.SetActive(false);
        runtimeUI.SetActive(false);
        endButton.SetActive(false);
        addUI.SetActive(false);
        Unselect();
        UpdateHeapAsArray(null);
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
            
        }
        else if (!running && !algMenu.activeSelf && !addUI.activeSelf)
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

    public void SetMax()
    {
        usingMin = false;
        maxCheck.SetActive(true);
        minCheck.SetActive(false);
    }

    public void SetMin()
    {
        usingMin = true;
        maxCheck.SetActive(false);
        minCheck.SetActive(true);
    }

    public void NotRunning()
    {
        if (selectedObject != null)
        {
            if (selectedTime > 0)
            {
                selectedObject.transform.position += MousePosition - prevMousePosition;
                selectedObject.GetComponent<HeapElement>().UpdatePosition();
            }
            else if (selectedTime > float.MinValue)
            {
                selectedTime = float.MinValue;
                selectedObject.GetComponent<HeapElement>().SetColor(Color.white);
                selectedObject = null;
            }
        }
        if (Input.GetMouseButtonDown(0))
        {
            if (selectedObject == null)
            {
                foreach (HeapElement element in elements)
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

    public void OpenAlgMenu()
    {
        algMenu.SetActive(true);
        defaultUI.SetActive(false);
        heapAsArray.SetActive(false);
    }

    public void UpdateHeapAsArray(List<HeapElement> highlights)
    {
        string s = "Array:";
        for (int i = 0; i < elements.Count; i++)
        {
            if (highlights != null && highlights.Contains(elements[i]))
            {
                s += " <color=#e3d1ffff>" + elements[i].ElementValue + "</color>";
            }
            else
            {
                s += " " + elements[i].ElementValue;
            }
        }
        heapAsArrayText.GetComponent<TMP_Text>().text = s;
    }

    public void SelectAlgorithm(string s)
    {
        sourceElement = null;
        currentAlg = s;
        algMenu.SetActive(false);
        switch (currentAlg)
        {
            case "Append":
            case "Insert":
                insertInput.GetComponent<TMP_InputField>().text = "";
                addUI.SetActive(true);
                break;
            case "Heapify":
                selectingPhase = true;
                sourceText.SetActive(true);
                break;
            case "HeapRemove":
            case "BuildHeap":
            case "HeapSort":
                RunAlgorithm();
                break;
        }
    }

    public void Back()
    {
        if (selectingPhase)
        {
            sourceElement = null;
            currentAlg = null;
            selectingPhase = false;
            sourceText.SetActive(false);
            algMenu.SetActive(true);
        }
        else if (addUI.activeSelf)
        {
            sourceElement = null;
            currentAlg = null;
            addUI.SetActive(false);
            algMenu.SetActive(true);
        }
        else if (algMenu.activeSelf)
        {
            algMenu.SetActive(false);
            defaultUI.SetActive(true);
            heapAsArray.SetActive(true);
        }
        else
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene("Menu");
        }
    }

    public void ChangeAnimationSpeed(float f)
    {
        animationSpeed = f;
    }

    public void RunAlgorithm()
    {

    }

    public void Add(string s)
    {
        float newWeight;
        bool success = float.TryParse(s, out newWeight);
        if (!success)
        {
            insertInput.GetComponent<TMP_InputField>().text = "";
            return;
        }
        addUI.SetActive(false);
        if (elements.Count == 0)
        {
            Vector3 position = ScreenCamera.transform.position;
            position.z = 0;
            HeapElement h = Instantiate(Resources.Load<GameObject>("Prefabs/HeapElement"), position, Quaternion.identity, GameObject.Find("Elements").transform).GetComponent<HeapElement>();
            elements.Add(h);
            h.SetValue(newWeight);
        }
        else
        {
            Vector3 position;
            if (elements.Count % 2 == 1)
            {
                position = elements[(elements.Count - 1) / 2].transform.position + new Vector3(-2, -2);
            }
            else
            {
                position = elements[(elements.Count - 1) / 2].transform.position + new Vector3(2, -2);
            }
            HeapElement h = Instantiate(Resources.Load<GameObject>("Prefabs/HeapElement"), position, Quaternion.identity, GameObject.Find("Elements").transform).GetComponent<HeapElement>();
            elements.Add(h);
            h.SetValue(newWeight);
            h.CreateConnector(elements[(elements.Count - 2) / 2]);
        }

        UpdateHeapAsArray(null);
        Unfreeze();
        selectingPhase = false;

        if (currentAlg == "Insert")
        {
            backButton.SetActive(false);
            heapAsArray.SetActive(true);
            running = true;
            foreach (HeapElement e in elements)
            {
                e.CreateInfoText();
                e.SetColor(Color.white);
            }
            runtimeUI.SetActive(true);
            StartCoroutine(InsertBubbleUp());
        }
        else
        {
            currentAlg = null;
            defaultUI.SetActive(true);
            heapAsArray.SetActive(true);
        }
    }

    public void RemoveFromEnd()
    {
        if (elements.Count == 0)
        {
            currentAlg = null;
            selectingPhase = false;
            algMenu.SetActive(false);
            defaultUI.SetActive(true);
            heapAsArray.SetActive(true);
            return;
        }
        HeapElement e = elements[elements.Count - 1];
        HeapElement parent = elements[(elements.Count - 2) / 2];
        elements.RemoveAt(elements.Count - 1);

        if (e.HeapElementConnector != null)
        {
            parent.RemoveIncomingConnector(e.HeapElementConnector);
        }
        e.Destroy();

        UpdateHeapAsArray(null);

        currentAlg = null;
        selectingPhase = false;
        algMenu.SetActive(false);
        defaultUI.SetActive(true);
        heapAsArray.SetActive(true);
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

        foreach (HeapElement e in elements)
        {
            e.DestroyInfoText();
            e.SetColor(Color.white);
            if (e.HeapElementConnector != null)
            {
                e.HeapElementConnector.SetColor(Color.white);
            }
        }

        backButton.SetActive(true);
        currentAlg = null;
        defaultUI.SetActive(true);
        sourceElement = null;
    }

    public IEnumerator InsertBubbleUp()
    {
        List<HeapElement> highlights = new List<HeapElement>();
        highlights.Add(elements[elements.Count - 1]);
        UpdateHeapAsArray(highlights);
        highlights.Clear();
        SelectOne(elements[elements.Count - 1]);
        SetLogText("");
        
        yield return new WaitForSeconds(3 / animationSpeed);

        int currIndex = elements.Count - 1;
        while (currIndex > 0)
        {
            HeapElement child = elements[currIndex];
            HeapElement parent = elements[(currIndex - 1) / 2];
            highlights.Add(child);
            highlights.Add(parent);
            UpdateHeapAsArray(highlights);
            SetLogText("Comparing: " + child.ElementValue + " v. " + parent.ElementValue);
            SelectOne(child);
            SelectTwo(parent);

            yield return new WaitForSeconds(2 / animationSpeed);

            if ((usingMin && child.ElementValue < parent.ElementValue) || (!usingMin && child.ElementValue > parent.ElementValue))
            {
                float parentValue = parent.ElementValue;
                parent.ChangeValue(child.ElementValue);
                child.ChangeValue(parentValue);

                UpdateHeapAsArray(highlights);
                SetLogText("Swapped: " + child.ElementValue + ", " + parent.ElementValue);

                yield return new WaitForSeconds(2 / animationSpeed);
            }
            else
            {
                break;
            }
            highlights.Clear();

            currIndex = (currIndex - 1) / 2;
        }

        Unselect();
        SetLogText("");
        UpdateHeapAsArray(null);
        yield return End();
    }


    public void SelectOne(HeapElement e)
    {
        elementSelected.transform.position = e.transform.position;
        elementSelected.GetComponent<SpriteRenderer>().enabled = true;
    }

    public void SelectTwo(HeapElement e)
    {
        elementSelected2.transform.position = e.transform.position;
        elementSelected2.GetComponent<SpriteRenderer>().enabled = true;
    }

    public void Unselect()
    {
        UnselectOne();
        UnselectTwo();
    }

    public void UnselectOne()
    {
        elementSelected.GetComponent<SpriteRenderer>().enabled = false;
    }

    public void UnselectTwo()
    {
        elementSelected2.GetComponent<SpriteRenderer>().enabled = false;
    }

    public void SetLogText(string s)
    {
        algLog.GetComponent<TMP_Text>().text = s;
    }
}
