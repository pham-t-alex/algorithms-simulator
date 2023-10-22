using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

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
    private bool paused;
    private bool stepping;
    private bool guiding;

    private List<HeapElement> elements = new List<HeapElement>();

    private List<Comparison> comparisons = new List<Comparison>();

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
    [SerializeField] private GameObject heapSortButton;
    [SerializeField] private GameObject pause;
    [SerializeField] private GameObject step;
    [SerializeField] private GameObject pauseText;
    [SerializeField] private GameObject guide;
    [SerializeField] private List<GameObject> guidePages;
    [SerializeField] private GameObject forwardButton;
    [SerializeField] private GameObject backwardButton;

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
        foreach (GameObject g in guidePages)
        {
            g.SetActive(false);
        }

        guide.SetActive(false);
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
            SelectElement();
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
            ScreenCamera.transform.Translate(Vector3.left * 5f * Mathf.Sqrt(ScreenCamera.orthographicSize) * Time.deltaTime);
        }
        if (Input.GetKey(KeyCode.S))
        {
            ScreenCamera.transform.Translate(Vector3.down * 5f * Mathf.Sqrt(ScreenCamera.orthographicSize) * Time.deltaTime);
        }
        if (Input.GetKey(KeyCode.W))
        {
            ScreenCamera.transform.Translate(Vector3.up * 5f * Mathf.Sqrt(ScreenCamera.orthographicSize) * Time.deltaTime);
        }
        if (Input.GetKey(KeyCode.D))
        {
            ScreenCamera.transform.Translate(Vector3.right * 5f * Mathf.Sqrt(ScreenCamera.orthographicSize) * Time.deltaTime);
        }
        if (Input.GetKey(KeyCode.Z))
        {
            ScreenCamera.orthographicSize /= (1 + 1f * Time.deltaTime);
        }
        if (Input.GetKey(KeyCode.X))
        {
            ScreenCamera.orthographicSize *= (1 + 1f * Time.deltaTime);
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

    public void Pause()
    {
        if (paused)
        {
            paused = false;
            step.GetComponent<UnityEngine.UI.Button>().interactable = false;
        }
        else
        {
            paused = true;
        }
    }

    public void Step()
    {
        stepping = true;
    }

    public void SetMax()
    {
        usingMin = false;
        maxCheck.SetActive(true);
        minCheck.SetActive(false);
        heapSortButton.GetComponent<UnityEngine.UI.Button>().interactable = true;
    }

    public void SetMin()
    {
        usingMin = true;
        maxCheck.SetActive(false);
        minCheck.SetActive(true);
        heapSortButton.GetComponent<UnityEngine.UI.Button>().interactable = false;
    }

    public void SelectElement()
    {
        if (Input.GetMouseButtonDown(0))
        {
            foreach (HeapElement element in elements)
            {
                if (element.MouseTouching)
                {
                    sourceElement = element;
                    RunAlgorithm();
                    return;
                }
            }
        }
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
        if (guiding)
        {
            foreach (GameObject g in guidePages)
            {
                g.SetActive(false);
            }
            guide.SetActive(false);
            defaultUI.SetActive(true);
            heapAsArray.SetActive(true);
            guiding = false;
        } else if (selectingPhase)
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

    public void OpenGuide()
    {
        guide.SetActive(true);
        guidePages[0].SetActive(true);
        forwardButton.GetComponent<Button>().interactable = true;
        backwardButton.GetComponent<Button>().interactable = false;
        defaultUI.SetActive(false);
        heapAsArray.SetActive(false);
        guiding = true;
    }

    public void GuideForward()
    {
        for (int i = 0; i < guidePages.Count - 1; i++)
        {
            if (guidePages[i].activeSelf)
            {
                guidePages[i + 1].SetActive(true);
                guidePages[i].SetActive(false);
                if (i + 1 == guidePages.Count - 1)
                {
                    forwardButton.GetComponent<Button>().interactable = false;
                }
                backwardButton.GetComponent<Button>().interactable = true;
            }
        }
    }

    public void GuideBackward()
    {
        for (int i = 1; i < guidePages.Count; i++)
        {
            if (guidePages[i].activeSelf)
            {
                guidePages[i - 1].SetActive(true);
                guidePages[i].SetActive(false);
                if (i - 1 == 0)
                {
                    backwardButton.GetComponent<Button>().interactable = false;
                }
                forwardButton.GetComponent<Button>().interactable = true;
            }
        }
    }


    public void ChangeAnimationSpeed(float f)
    {
        animationSpeed = Mathf.Pow(2, f);
    }

    public void RunAlgorithm()
    {
        Unselect();
        comparisons.Clear();
        running = true;
        selectingPhase = false;
        backButton.SetActive(false);
        sourceText.SetActive(false);
        heapAsArray.SetActive(true);
        running = true;
        foreach (HeapElement e in elements)
        {
            e.CreateInfoText();
            e.SetColor(Color.white);
            if (e.HeapElementConnector != null)
            {
                e.HeapElementConnector.SetColor(Color.white);
            }
        }
        runtimeUI.SetActive(true);
        paused = false;
        stepping = false;
        pauseText.SetActive(false);
        pause.GetComponent<UnityEngine.UI.Button>().interactable = true;
        step.GetComponent<UnityEngine.UI.Button>().interactable = false;
        SetLogText("");

        switch (currentAlg)
        {
            case "HeapRemove":
                StartCoroutine(HeapRemove());
                break;
            case "Heapify":
                StartCoroutine(Heapify());
                break;
            case "BuildHeap":
                StartCoroutine(BuildHeap());
                break;
            case "HeapSort":
                StartCoroutine(HeapSort());
                break;
        }
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
                if (e.HeapElementConnector != null)
                {
                    e.HeapElementConnector.SetColor(Color.white);
                }
            }
            paused = false;
            stepping = false;
            pauseText.SetActive(false);
            pause.GetComponent<UnityEngine.UI.Button>().interactable = true;
            step.GetComponent<UnityEngine.UI.Button>().interactable = false;
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
        comparisons.Clear();
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

    public IEnumerator WaitUntilPlaying()
    {
        if (paused && !stepping)
        {
            step.GetComponent<UnityEngine.UI.Button>().interactable = true;
            pauseText.SetActive(true);
        }
        yield return new WaitUntil(() => stepping | !paused);
        stepping = false;
        pauseText.SetActive(false);
        step.GetComponent<UnityEngine.UI.Button>().interactable = false;
    }

    public void SwapValues(HeapElement one, HeapElement two)
    {
        float temp = one.ElementValue;
        one.ChangeValue(two.ElementValue);
        two.ChangeValue(temp);
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
        yield return WaitUntilPlaying();

        int currIndex = elements.Count - 1;
        while (currIndex > 0)
        {
            HeapElement child = elements[currIndex];
            HeapElement parent = elements[(currIndex - 1) / 2];
            highlights.Add(child);
            highlights.Add(parent);
            UpdateHeapAsArray(highlights);
            SetLogText("Comparing: " + child.ElementValue + " v. " + parent.ElementValue);
            comparisons.Add(new Comparison(child.ElementValue, parent.ElementValue));
            SelectOne(child);
            SelectTwo(parent);

            yield return new WaitForSeconds(2 / animationSpeed);

            if ((usingMin && child.ElementValue < parent.ElementValue) || (!usingMin && child.ElementValue > parent.ElementValue))
            {
                SwapValues(parent, child);

                UpdateHeapAsArray(highlights);
                SetLogText("Swapped: " + parent.ElementValue + ", " + child.ElementValue);

                yield return new WaitForSeconds(2 / animationSpeed);
                yield return WaitUntilPlaying();
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

    public IEnumerator HeapRemove()
    {
        if (elements.Count == 0)
        {
            EndAlgorithm();
            yield break;
        }
        yield return new WaitForSeconds(3 / animationSpeed);
        yield return WaitUntilPlaying();

        List<HeapElement> highlights = new List<HeapElement>();
        if (elements.Count > 1)
        {
            HeapElement lastElement = elements[elements.Count - 1];
            HeapElement top = elements[0];
            highlights.Add(lastElement);
            highlights.Add(top);
            UpdateHeapAsArray(highlights);
            SelectOne(top);
            SelectTwo(lastElement);

            SetLogText("Swapping " + top.ElementValue + " and " + lastElement.ElementValue);

            yield return new WaitForSeconds(2 / animationSpeed);

            SwapValues(top, lastElement);
            SetLogText("Swapped: " + lastElement.ElementValue + ", " + top.ElementValue);

            UpdateHeapAsArray(highlights);
        }
        else
        {
            SelectTwo(elements[0]);
            highlights.Add(elements[0]);
            UpdateHeapAsArray(highlights);
        }
        highlights.Clear();

        yield return new WaitForSeconds(3 / animationSpeed);
        yield return WaitUntilPlaying();

        UnselectOne();
        HeapElement last = elements[elements.Count - 1];
        SetLogText("Deleted " + last.ElementValue);

        HeapElement parent = null;
        if (elements.Count > 1)
        {
            parent = elements[(elements.Count - 2) / 2];
        }
        elements.RemoveAt(elements.Count - 1);

        if (last.HeapElementConnector != null && elements.Count > 0)
        {
            parent.RemoveIncomingConnector(last.HeapElementConnector);
        }
        last.Destroy();
        UpdateHeapAsArray(null);
        yield return new WaitForSeconds(2 / animationSpeed);
        yield return WaitUntilPlaying();

        yield return HeapifyHelper(0, elements.Count);

        Unselect();
        SetLogText("");
        UpdateHeapAsArray(null);
        yield return End();
    }

    public IEnumerator Heapify()
    {
        int index = 0;
        while (index < elements.Count && elements[index] != sourceElement)
        {
            index++;
        }
        if (index == elements.Count)
        {
            yield return End();
            yield break;
        }
        yield return new WaitForSeconds(3 / animationSpeed);
        yield return WaitUntilPlaying();
        yield return HeapifyHelper(index, elements.Count);

        Unselect();
        SetLogText("");
        UpdateHeapAsArray(null);
        yield return End();
    }

    public IEnumerator BuildHeap()
    {
        yield return new WaitForSeconds(3 / animationSpeed);
        yield return WaitUntilPlaying();
        yield return BuildHeapHelper();

        Unselect();
        SetLogText("");
        UpdateHeapAsArray(null);
        yield return End();
    }

    public IEnumerator HeapSort()
    {
        yield return new WaitForSeconds(3 / animationSpeed);
        yield return WaitUntilPlaying();
        yield return BuildHeapHelper();

        string buildHeapComparisons = ComparisonsToString();
        comparisons.Clear();

        int arrayCount = elements.Count;
        List<HeapElement> highlights = new List<HeapElement>();
        while (arrayCount > 0)
        {
            Unselect();
            SelectOne(elements[0]);
            highlights.Add(elements[0]);
            UpdateHeapAsArray(highlights);
            highlights.Clear();
            SetLogText("Moving " + elements[0].ElementValue + " to end");
            yield return new WaitForSeconds(2 / animationSpeed);

            if (arrayCount > 1)
            {
                HeapElement lastElement = elements[arrayCount - 1];
                HeapElement top = elements[0];
                highlights.Add(lastElement);
                highlights.Add(top);
                UpdateHeapAsArray(highlights);
                SelectOne(top);
                SelectTwo(lastElement);

                SetLogText("Swapping " + top.ElementValue + " and " + lastElement.ElementValue);

                yield return new WaitForSeconds(2 / animationSpeed);

                SwapValues(top, lastElement);
                SetLogText("Swapped: " + lastElement.ElementValue + ", " + top.ElementValue);
                UpdateHeapAsArray(highlights);
            }
            else
            {
                SelectTwo(elements[0]);
                highlights.Add(elements[0]);
                UpdateHeapAsArray(highlights);
            }
            highlights.Clear();

            yield return new WaitForSeconds(3 / animationSpeed);
            yield return WaitUntilPlaying();

            UnselectOne();
            HeapElement last = elements[arrayCount - 1];
            SetLogText("Sorted " + last.ElementValue);
            highlights.Add(last);

            UpdateHeapAsArray(highlights);
            last.SetColor(new Color(0.6f, 0.6f, 0.6f));
            highlights.Clear();
            yield return new WaitForSeconds(2 / animationSpeed);
            yield return WaitUntilPlaying();

            arrayCount--;

            yield return HeapifyHelper(0, arrayCount);
            Unselect();
            UpdateHeapAsArray(null);
            yield return new WaitForSeconds(1 / animationSpeed);
            yield return WaitUntilPlaying();
        }

        Unselect();
        SetLogText("<size=15>BuildHeap Comparisons: " + buildHeapComparisons + " | Other Comparisons: " + ComparisonsToString() + "</size>");
        UpdateHeapAsArray(null);
        yield return End();
    }

    public IEnumerator HeapifyHelper(int index, int arrayLength)
    {
        int currIndex = index;
        List<HeapElement> highlights = new List<HeapElement>();

        while (currIndex < arrayLength / 2)
        {
            HeapElement one = elements[currIndex];
            HeapElement two = elements[currIndex * 2 + 1];
            highlights.Add(one);
            highlights.Add(two);
            SetLogText("Comparing: " + one.ElementValue + " v. " + two.ElementValue);
            comparisons.Add(new Comparison(one.ElementValue, two.ElementValue));
            UpdateHeapAsArray(highlights);
            SelectOne(one);
            SelectTwo(two);

            yield return new WaitForSeconds(2 / animationSpeed);

            if ((usingMin && one.ElementValue > two.ElementValue) || (!usingMin && one.ElementValue < two.ElementValue))
            {
                if (currIndex * 2 + 2 < arrayLength)
                {
                    highlights.Remove(one);
                    highlights.Remove(two);
                    one = elements[currIndex * 2 + 1];
                    two = elements[currIndex * 2 + 2];
                    highlights.Add(one);
                    highlights.Add(two);
                    SetLogText("Comparing: " + one.ElementValue + " v. " + two.ElementValue);
                    comparisons.Add(new Comparison(one.ElementValue, two.ElementValue));
                    UpdateHeapAsArray(highlights);
                    SelectOne(one);
                    SelectTwo(two);

                    yield return new WaitForSeconds(2 / animationSpeed);

                    highlights.Clear();
                    if ((usingMin && one.ElementValue > two.ElementValue) || (!usingMin && one.ElementValue < two.ElementValue))
                    {
                        one = elements[currIndex];
                        currIndex = currIndex * 2 + 2;
                    }
                    else
                    {
                        two = one;
                        one = elements[currIndex];
                        currIndex = currIndex * 2 + 1;
                    }
                    highlights.Add(one);
                    highlights.Add(two);

                    SwapValues(one, two);
                    UpdateHeapAsArray(highlights);
                    SelectOne(one);
                    SelectTwo(two);

                    SetLogText("Swapped: " + one.ElementValue + ", " + two.ElementValue);

                    yield return new WaitForSeconds(2 / animationSpeed);
                    yield return WaitUntilPlaying();


                }
                else
                {
                    SwapValues(one, two);
                    UpdateHeapAsArray(highlights);

                    SetLogText("Swapped: " + one.ElementValue + ", " + two.ElementValue);

                    yield return new WaitForSeconds(2 / animationSpeed);
                    yield return WaitUntilPlaying();

                    currIndex = currIndex * 2 + 1;
                }
            }
            else
            {
                if (currIndex * 2 + 2 < arrayLength)
                {
                    highlights.Remove(two);
                    two = elements[currIndex * 2 + 2];
                    highlights.Add(two);
                    SetLogText("Comparing: " + one.ElementValue + " v. " + two.ElementValue);
                    comparisons.Add(new Comparison(one.ElementValue, two.ElementValue));
                    UpdateHeapAsArray(highlights);
                    SelectTwo(two);

                    yield return new WaitForSeconds(2 / animationSpeed);

                    if ((usingMin && one.ElementValue > two.ElementValue) || (!usingMin && one.ElementValue < two.ElementValue))
                    {
                        SwapValues(one, two);
                        UpdateHeapAsArray(highlights);

                        SetLogText("Swapped: " + one.ElementValue + ", " + two.ElementValue);

                        yield return new WaitForSeconds(2 / animationSpeed);
                        yield return WaitUntilPlaying();

                        currIndex = currIndex * 2 + 2;
                    }
                    else
                    {
                        break;
                    }
                }
                else
                {
                    break;
                }
            }
            highlights.Clear();
        }
    }

    public IEnumerator BuildHeapHelper()
    {
        if (elements.Count < 2)
        {
            yield break;
        }

        int currIndex = (elements.Count - 2) / 2;
        while (currIndex >= 0)
        {
            Unselect();
            SelectOne(elements[currIndex]);
            SetLogText("MaxHeapify on " + elements[currIndex].ElementValue);
            yield return new WaitForSeconds(2 / animationSpeed);
            yield return WaitUntilPlaying();

            yield return HeapifyHelper(currIndex, elements.Count);

            currIndex = currIndex - 1;
        }

        yield return null;
    }

    public string ComparisonsToString()
    {
        string s = "";
        if (comparisons.Count > 0)
        {
            s += comparisons[0].ToString();
            for (int i = 1; i < comparisons.Count; i++)
            {
                s += ", " + comparisons[i].ToString();
            }
        }
        return s;
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
