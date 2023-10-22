using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class SortsController : MonoBehaviour
{
    private static SortsController sortsController;
    public static SortsController ListSortController
    {
        get
        {
            if (sortsController == null)
            {
                sortsController = FindObjectOfType<SortsController>();
            }
            return sortsController;
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

    private bool frozen = false;

    private bool selectingPhase = false;
    private bool running = false;
    private float animationSpeed = 1;
    private bool paused;
    private bool stepping;
    private bool guiding;

    private List<ListElement> elements = new List<ListElement>();
    private HashSet<ListElement> nonIntegerElements = new HashSet<ListElement>();
    private List<Comparison> comparisons = new List<Comparison>();

    [SerializeField] private GameObject defaultUI;
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
    [SerializeField] private GameObject parameterText;
    [SerializeField] private GameObject pause;
    [SerializeField] private GameObject step;
    [SerializeField] private GameObject pauseText;
    [SerializeField] private GameObject guide;
    [SerializeField] private List<GameObject> guidePages;
    [SerializeField] private GameObject forwardButton;
    [SerializeField] private GameObject backwardButton;

    [SerializeField] private List<GameObject> nonFloatAlgs = new List<GameObject>();

    private ListElement sourceElement;
    private string currentAlg;

    private float baseX;
    private float baseY;

    private int parameter;

    // Start is called before the first frame update
    void Start()
    {
        algMenu.SetActive(false);
        sourceText.SetActive(false);
        runtimeUI.SetActive(false);
        endButton.SetActive(false);
        addUI.SetActive(false);
        Unselect();
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

    public void SelectElement()
    {
        if (Input.GetMouseButtonDown(0))
        {
            for (int i = 0; i < elements.Count; i++)
            {
                if (elements[i].MouseTouching)
                {
                    Remove(i);
                    return;
                }
            }
        }
    }

    public void AddNonIntegerElement(ListElement element)
    {
        nonIntegerElements.Add(element);
    }

    public void RemoveNonIntegerElement(ListElement element)
    {
        nonIntegerElements.Remove(element);
    }

    public void OpenAlgMenu()
    {
        algMenu.SetActive(true);
        defaultUI.SetActive(false);
        foreach (GameObject button in nonFloatAlgs)
        {
            button.GetComponent<UnityEngine.UI.Button>().interactable = (nonIntegerElements.Count == 0);
        }
    }

    public void SelectAlgorithm(string s)
    {
        sourceElement = null;
        currentAlg = s;
        defaultUI.SetActive(false);
        algMenu.SetActive(false);
        switch (currentAlg)
        {
            case "Randomize":
                Randomize();
                defaultUI.SetActive(true);
                break;
            case "Add":
                parameterText.GetComponent<TMP_Text>().text = "Insert an element:";
                insertInput.GetComponent<TMP_InputField>().text = "";
                addUI.SetActive(true);
                break;
            case "QuickSelect":
                parameterText.GetComponent<TMP_Text>().text = "Kth smallest element:";
                insertInput.GetComponent<TMP_InputField>().text = "";
                addUI.SetActive(true);
                break;
            case "Bucket":
                if (elements.Count > 1)
                {
                    parameterText.GetComponent<TMP_Text>().text = "Bucket count:";
                    insertInput.GetComponent<TMP_InputField>().text = "";
                    addUI.SetActive(true);
                }
                else
                {
                    currentAlg = null;
                    defaultUI.SetActive(true);
                    SetLogText("");
                    comparisons.Clear();
                }
                break;
            case "Remove":
                selectingPhase = true;
                sourceText.SetActive(true);
                break;
            case "TMerge":
            case "BMerge":
            case "QuickSort":
            case "Counting":
            case "Radix":
            case "Insertion":
                if (elements.Count > 1)
                {
                    RunAlgorithm();
                }
                else
                {
                    currentAlg = null;
                    defaultUI.SetActive(true);
                    SetLogText("");
                    comparisons.Clear();
                }
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
            guiding = false;
        }
        else if (selectingPhase)
        {
            if (currentAlg == "Remove")
            {
                defaultUI.SetActive(true);
            }
            else
            {
                algMenu.SetActive(true);
            }

            sourceElement = null;
            currentAlg = null;
            selectingPhase = false;
            sourceText.SetActive(false);
        }
        else if (addUI.activeSelf)
        {
            if (currentAlg == "Add")
            {
                sourceElement = null;
                currentAlg = null;
                addUI.SetActive(false);
                defaultUI.SetActive(true);
            }
            else
            {
                sourceElement = null;
                currentAlg = null;
                addUI.SetActive(false);
                algMenu.SetActive(true);
            }
        }
        else if (algMenu.activeSelf)
        {
            algMenu.SetActive(false);
            defaultUI.SetActive(true);
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
        running = true;
        (ListElement, ListElement) minMax = ElementMinMax();
        foreach (ListElement e in elements)
        {
            e.CreateInfoText();
            float relative = (minMax.Item2.ElementValue - e.ElementValue) / (minMax.Item2.ElementValue - minMax.Item1.ElementValue);
            e.SetColor(new Color(1 - 0.3f * relative, 1 - 0.3f * relative, 1 - 0.3f * relative));
            e.SetMovement(Vector3.zero);
        }
        paused = false;
        stepping = false;
        pause.GetComponent<UnityEngine.UI.Button>().interactable = true;
        step.GetComponent<UnityEngine.UI.Button>().interactable = false;
        runtimeUI.SetActive(true);
        pauseText.SetActive(false);
        SetLogText("");

        switch (currentAlg)
        {
            case "TMerge":
                StartCoroutine(TMerge());
                break;
            case "BMerge":
                StartCoroutine(BMerge());
                break;
            case "QuickSort":
                StartCoroutine(QuickSort());
                break;
            case "Counting":
                StartCoroutine(Counting());
                break;
            case "Radix":
                StartCoroutine(Radix());
                break;
            case "Bucket":
                StartCoroutine(Bucket());
                break;
            case "QuickSelect":
                StartCoroutine(QuickSelect());
                break;
            case "Insertion":
                StartCoroutine(Insertion());
                break;
        }
    }

    public void PassValue(string s)
    {
        switch (currentAlg)
        {
            case "Add":
                Add(s);
                break;
            case "Bucket":
                BucketParam(s);
                break;
            case "QuickSelect":
                SelectParam(s);
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
        ListElement e = null;
        if (elements.Count == 0)
        {
            Vector3 position = ScreenCamera.transform.position;
            baseX = position.x;
            baseY = position.y;
            position.z = 0;
            e = Instantiate(Resources.Load<GameObject>("Prefabs/ListElement"), position, Quaternion.identity, GameObject.Find("Elements").transform).GetComponent<ListElement>();
            elements.Add(e);
            e.SetValue(newWeight);
        }
        else
        {
            Vector3 position = elements[elements.Count - 1].transform.position + new Vector3(2, 0);
            e = Instantiate(Resources.Load<GameObject>("Prefabs/ListElement"), position, Quaternion.identity, GameObject.Find("Elements").transform).GetComponent<ListElement>();
            elements.Add(e);
            e.SetValue(newWeight);
        }
        if (e.ElementValue != (int)e.ElementValue)
        {
            AddNonIntegerElement(e);
        }
        Unfreeze();
        selectingPhase = false;
        currentAlg = null;
        defaultUI.SetActive(true);
    }

    public void Randomize()
    {
        foreach (ListElement e in elements)
        {
            e.Destroy();
        }
        elements = new List<ListElement>();
        Vector3 position = ScreenCamera.transform.position - new Vector3(9, 0);
        baseX = position.x;
        baseY = position.y;
        position.z = 0;
        for (int i = 0; i < 10; i++)
        {
            ListElement e = Instantiate(Resources.Load<GameObject>("Prefabs/ListElement"), position, Quaternion.identity, GameObject.Find("Elements").transform).GetComponent<ListElement>();
            elements.Add(e);
            e.SetValue(Random.Range(0, 100));
            position += new Vector3(2, 0);
        }
        if (ScreenCamera.orthographicSize < 6)
        {
            ScreenCamera.orthographicSize = 6;
        }
    }

    public void BucketParam(string s)
    {
        int value;
        bool success = int.TryParse(s, out value);
        if (!success || value < 2)
        {
            insertInput.GetComponent<TMP_InputField>().text = "";
            return;
        }
        addUI.SetActive(false);
        parameter = value;
        Unfreeze();
        RunAlgorithm();
    }

    public void SelectParam(string s)
    {
        int value;
        bool success = int.TryParse(s, out value);
        if (!success || value < 1 || value > elements.Count)
        {
            insertInput.GetComponent<TMP_InputField>().text = "";
            return;
        }
        addUI.SetActive(false);
        parameter = value;
        Unfreeze();
        RunAlgorithm();
    }

    public void Remove(int i)
    {
        selectingPhase = false;
        sourceText.SetActive(false);
        defaultUI.SetActive(true);

        RemoveNonIntegerElement(elements[i]);
        elements[i].Destroy();
        elements.RemoveAt(i);
        while (i < elements.Count)
        {
            elements[i].transform.position -= new Vector3(2, 0);
            elements[i].UpdatePosition();
            i++;
        }
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

        foreach (ListElement e in elements)
        {
            e.DestroyInfoText();
            e.SetColor(Color.white);
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

    public IEnumerator TMerge()
    {
        SetLogText("");
        yield return new WaitForSeconds(3 / animationSpeed);

        GameObject[] dividers = new GameObject[elements.Count - 1];
        GameObject leftDivider = Instantiate(Resources.Load<GameObject>("Prefabs/Divider"), new Vector3(baseX - 1, baseY), Quaternion.identity, GameObject.Find("Other").transform);
        GameObject rightDivider = Instantiate(Resources.Load<GameObject>("Prefabs/Divider"), new Vector3(baseX - 1 + (2 * elements.Count), baseY), Quaternion.identity, GameObject.Find("Other").transform);
        Stack<(int, int, bool)> stack = new Stack<(int, int, bool)>();
        stack.Push((0, elements.Count - 1, false));
        yield return WaitUntilPlaying();
        while (stack.Count > 0)
        {
            (int, int, bool) tuple = stack.Pop();
            if (tuple.Item3)
            {
                int leftIndex = tuple.Item1;
                int mid = (tuple.Item1 + tuple.Item2) / 2;
                int leftBound = mid + 1;
                int rightIndex = leftBound;
                int rightBound = tuple.Item2 + 1;
                SetLogText("Merging " + ListToString(elements, leftIndex, leftBound - 1) + " and " + ListToString(elements, rightIndex, rightBound - 1));
                yield return new WaitForSeconds(2.5f / animationSpeed);
                yield return WaitUntilPlaying();
                ListElement[] tempElements = new ListElement[rightBound - tuple.Item1];
                int tempElementIndex = 0;
                int offset = tuple.Item1;
                while (leftIndex < leftBound && rightIndex < rightBound)
                {
                    SetLogText("Comparing: " + elements[leftIndex].ElementValue + " v. " + elements[rightIndex].ElementValue);
                    comparisons.Add(new Comparison(elements[leftIndex].ElementValue, elements[rightIndex].ElementValue));
                    SelectOne(elements[leftIndex]);
                    SelectTwo(elements[rightIndex]);
                    yield return new WaitForSeconds(1 / animationSpeed);
                    if (elements[leftIndex].ElementValue <= elements[rightIndex].ElementValue)
                    {
                        yield return DragDown(leftIndex, tempElementIndex + offset, true);
                        tempElements[tempElementIndex] = elements[leftIndex];
                        leftIndex++;
                    }
                    else
                    {
                        yield return DragDown(rightIndex, tempElementIndex + offset, false);
                        tempElements[tempElementIndex] = elements[rightIndex];
                        rightIndex++;
                    }
                    yield return new WaitForSeconds(1 / animationSpeed);
                    tempElementIndex++;
                    Unselect();
                    yield return WaitUntilPlaying();
                }
                while (leftIndex < leftBound)
                {
                    SetLogText("");
                    SelectOne(elements[leftIndex]);
                    yield return new WaitForSeconds(0.5f / animationSpeed);
                    yield return DragDown(leftIndex, tempElementIndex + offset, true);
                    tempElements[tempElementIndex] = elements[leftIndex];
                    tempElementIndex++;
                    leftIndex++;
                    yield return new WaitForSeconds(1 / animationSpeed);
                    UnselectOne();
                }
                while (rightIndex < rightBound)
                {
                    SetLogText("");
                    SelectOne(elements[rightIndex]);
                    yield return new WaitForSeconds(0.5f / animationSpeed);
                    yield return DragDown(rightIndex, tempElementIndex + offset, true);
                    tempElements[tempElementIndex] = elements[rightIndex];
                    tempElementIndex++;
                    rightIndex++;
                    yield return new WaitForSeconds(1 / animationSpeed);
                    UnselectOne();
                }
                SetLogText("Finish Merge");
                for (int k = 0; k < tempElements.Length; k++)
                {
                    elements[k + offset] = tempElements[k];
                }
                yield return DragUp(tuple.Item1, tuple.Item1 + tempElements.Length - 1);
                Destroy(dividers[mid]);
                dividers[mid] = null;
                yield return new WaitForSeconds(1 / animationSpeed);
                yield return WaitUntilPlaying();
            }
            else
            {
                if (tuple.Item1 >= tuple.Item2)
                {
                    continue;
                }
                SetLogText("Dividing " + ListToString(elements, tuple.Item1, tuple.Item2));
                stack.Push((tuple.Item1, tuple.Item2, true));
                int mid = (tuple.Item1 + tuple.Item2) / 2;
                if (mid < elements.Count - 1)
                {
                    dividers[mid] = Instantiate(Resources.Load<GameObject>("Prefabs/Divider"), new Vector3(baseX + 1 + (2 * mid), baseY), Quaternion.identity, GameObject.Find("Other").transform);
                }
                stack.Push((mid + 1, tuple.Item2, false));
                stack.Push((tuple.Item1, mid, false));
                yield return new WaitForSeconds(1.5f / animationSpeed);
                yield return WaitUntilPlaying();
            }
        }
        Destroy(leftDivider);
        Destroy(rightDivider);
        Unselect();
        SetLogText("<size=15>Comparisons: " + ListToString(comparisons, 0, comparisons.Count - 1) + "</size>");
        yield return End();
    }

    public IEnumerator BMerge()
    {
        SetLogText("");
        yield return new WaitForSeconds(3 / animationSpeed);

        int iterations = Mathf.CeilToInt(Mathf.Log(elements.Count, 2));
        yield return WaitUntilPlaying();
        for (int i = 0; i < iterations; i++)
        {
            for (int j = 0; j < elements.Count; j += (int) Mathf.Pow(2, i + 1))
            {
                int leftIndex = j;
                int leftBound = j + (int)Mathf.Pow(2, i);
                if (leftBound >= elements.Count)
                {
                    continue;
                }
                int rightIndex = leftBound;
                int rightBound = j + (int)Mathf.Pow(2, i + 1);
                if (rightBound > elements.Count)
                {
                    rightBound = elements.Count;
                }
                GameObject leftDivider = Instantiate(Resources.Load<GameObject>("Prefabs/Divider"), new Vector3(baseX - 1 + (2 * leftIndex), baseY), Quaternion.identity, GameObject.Find("Other").transform);
                GameObject centerDivider = Instantiate(Resources.Load<GameObject>("Prefabs/Divider"), new Vector3(baseX - 1 + (2 * rightIndex), baseY), Quaternion.identity, GameObject.Find("Other").transform);
                GameObject rightDivider = Instantiate(Resources.Load<GameObject>("Prefabs/Divider"), new Vector3(baseX - 1 + (2 * rightBound), baseY), Quaternion.identity, GameObject.Find("Other").transform);
                SetLogText("Merging " + ListToString(elements, leftIndex, leftBound - 1) + " and " + ListToString(elements, rightIndex, rightBound - 1));
                yield return new WaitForSeconds(2.5f / animationSpeed);
                yield return WaitUntilPlaying();
                ListElement[] tempElements = new ListElement[rightBound - j];
                int tempElementIndex = 0;
                int offset = j;
                while (leftIndex < leftBound && rightIndex < rightBound)
                {
                    SetLogText("Comparing: " + elements[leftIndex].ElementValue + " v. " + elements[rightIndex].ElementValue);
                    comparisons.Add(new Comparison(elements[leftIndex].ElementValue, elements[rightIndex].ElementValue));
                    SelectOne(elements[leftIndex]);
                    SelectTwo(elements[rightIndex]);
                    yield return new WaitForSeconds(1 / animationSpeed);
                    if (elements[leftIndex].ElementValue <= elements[rightIndex].ElementValue)
                    {
                        yield return DragDown(leftIndex, tempElementIndex + offset, true);
                        tempElements[tempElementIndex] = elements[leftIndex];
                        leftIndex++;
                    }
                    else
                    {
                        yield return DragDown(rightIndex, tempElementIndex + offset, false);
                        tempElements[tempElementIndex] = elements[rightIndex];
                        rightIndex++;
                    }
                    yield return new WaitForSeconds(1 / animationSpeed);
                    tempElementIndex++;
                    Unselect();
                    yield return WaitUntilPlaying();
                }
                while (leftIndex < leftBound)
                {
                    SetLogText("");
                    SelectOne(elements[leftIndex]);
                    yield return new WaitForSeconds(0.5f / animationSpeed);
                    yield return DragDown(leftIndex, tempElementIndex + offset, true);
                    tempElements[tempElementIndex] = elements[leftIndex];
                    tempElementIndex++;
                    leftIndex++;
                    yield return new WaitForSeconds(1 / animationSpeed);
                    UnselectOne();
                }
                while (rightIndex < rightBound)
                {
                    SetLogText("");
                    SelectOne(elements[rightIndex]);
                    yield return new WaitForSeconds(0.5f / animationSpeed);
                    yield return DragDown(rightIndex, tempElementIndex + offset, true);
                    tempElements[tempElementIndex] = elements[rightIndex];
                    tempElementIndex++;
                    rightIndex++;
                    yield return new WaitForSeconds(1 / animationSpeed);
                    UnselectOne();
                }
                SetLogText("Finish Merge");
                for (int k = 0; k < tempElements.Length; k++)
                {
                    elements[k + offset] = tempElements[k];
                }
                yield return DragUp(j, j + tempElements.Length - 1);
                Destroy(leftDivider);
                Destroy(centerDivider);
                Destroy(rightDivider);
                yield return new WaitForSeconds(1 / animationSpeed);
                yield return WaitUntilPlaying();
            }
        }
        Unselect();
        SetLogText("<size=15>Comparisons: " + ListToString(comparisons, 0, comparisons.Count - 1) + "</size>");
        yield return End();
    }

    public IEnumerator QuickSort()
    {
        (ListElement, ListElement) minMax = ElementMinMax();
        float min = minMax.Item1.ElementValue;
        float max = minMax.Item2.ElementValue;
        SetLogText("");
        yield return new WaitForSeconds(3 / animationSpeed);
        yield return WaitUntilPlaying();

        Stack<(int, int)> stack = new Stack<(int, int)>();
        stack.Push((0, elements.Count - 1));
        while (stack.Count > 0)
        {
            (int, int) tuple = stack.Pop();
            if (tuple.Item1 >= tuple.Item2)
            {
                if (tuple.Item1 == tuple.Item2)
                {
                    elements[tuple.Item1].SetColor(new Color(0.5f, 0.5f, 0.5f));
                    yield return new WaitForSeconds(1f / animationSpeed);
                }
                continue;
            }
            List<ListElement> leftSide = new List<ListElement>();
            List<ListElement> rightSide = new List<ListElement>();
            ListElement pivot = elements[tuple.Item2];
            pivot.SetColor(new Color(1f, 0.8f, 0.8f));
            SetLogText("Partitioning: " + ListToString(elements, tuple.Item1, tuple.Item2 - 1) + " | Pivot: " + pivot.ToString());
            SelectOne(pivot);
            yield return new WaitForSeconds(2 / animationSpeed);
            yield return WaitUntilPlaying();
            for (int i = tuple.Item1; i < tuple.Item2; i++)
            {
                SetLogText("Comparing: " + elements[i].ToString() + " v. " + pivot.ToString());
                SelectTwo(elements[i]);
                comparisons.Add(new Comparison(elements[i].ElementValue, pivot.ElementValue));
                if (elements[i].ElementValue <= pivot.ElementValue)
                {
                    leftSide.Add(elements[i]);
                    elements[i].SetColor(new Color(0.9f, 0.9f, 1f));
                }
                else
                {
                    rightSide.Add(elements[i]);
                    elements[i].SetColor(new Color(0.9f, 1f, 0.9f));
                }
                yield return new WaitForSeconds(1f / animationSpeed);
                yield return WaitUntilPlaying();
            }
            SetLogText("Partitioning: " + ListToString(elements, tuple.Item1, tuple.Item2 - 1) + " | Pivot: " + pivot.ToString());
            UnselectTwo();
            int index = tuple.Item1;
            foreach (ListElement e in leftSide)
            {
                elements[index] = e;
                index++;
            }
            elements[index] = pivot;
            stack.Push((index + 1, tuple.Item2));
            stack.Push((tuple.Item1, index - 1));
            index++;
            foreach (ListElement e in rightSide)
            {
                elements[index] = e;
                index++;
            }
            yield return SetNewPositions(pivot, tuple.Item1, tuple.Item2);
            yield return new WaitForSeconds(1 / animationSpeed);
            pivot.SetColor(new Color(0.5f, 0.5f, 0.5f));
            foreach (ListElement e in leftSide)
            {
                float relative = (max - e.ElementValue) / (max - min);
                e.SetColor(new Color(1 - 0.3f * relative, 1 - 0.3f * relative, 1 - 0.3f * relative));
            }
            foreach (ListElement e in rightSide)
            {
                float relative = (max - e.ElementValue) / (max - min);
                e.SetColor(new Color(1 - 0.3f * relative, 1 - 0.3f * relative, 1 - 0.3f * relative));
            }
            UnselectOne();
            SetLogText("");
            yield return new WaitForSeconds(1 / animationSpeed);
            yield return WaitUntilPlaying();
        }
        Unselect();
        SetLogText("<size=15>Comparisons: " + ListToString(comparisons, 0, comparisons.Count - 1) + "</size>");
        yield return End();
    }

    public IEnumerator Counting()
    {
        (ListElement, ListElement) minMax = ElementMinMax();
        List<RuntimeArrayElement> runtimeArray = new List<RuntimeArrayElement>();
        RuntimeArrayElement prevArrayElement = null;
        Vector3 position = new Vector3(baseX, baseY + 3f);
        for (int i = (int) minMax.Item1.ElementValue; i <= (int) minMax.Item2.ElementValue; i++)
        {
            RuntimeArrayElement element = Instantiate(Resources.Load<GameObject>("Prefabs/RuntimeArrayElement").GetComponent<RuntimeArrayElement>(), position, Quaternion.identity, GameObject.Find("Other").transform);
            runtimeArray.Add(element);
            element.CreateInfoText();
            element.AddVariable("i", i);
            element.AddVariable("val", 0);
            string text = $"{i}\n<size=8>{0}</size>";
            element.UpdateInfoText(text, false);
            position += new Vector3(2, 0, 0);
        }

        List<GameObject> indices = new List<GameObject>();
        for (int i = 0; i < elements.Count; i++)
        {
            GameObject indexText = Instantiate(Resources.Load<GameObject>("Prefabs/InfoText"), new Vector3(IndexToXPos(i), baseY - 3.6f), Quaternion.identity, GameObject.Find("Other").transform);
            indexText.GetComponent<TMP_Text>().text = "<color=#ffffffff>" + i.ToString() + "</color>";
            indices.Add(indexText);
        }

        yield return new WaitForSeconds(3 / animationSpeed);
        yield return WaitUntilPlaying();

        for (int i = 0; i < elements.Count; i++)
        {
            SelectOne(elements[i]);
            int index = (int) elements[i].ElementValue - (int) runtimeArray[0].GetVariable("i");
            RuntimeArrayElement element = runtimeArray[index];
            element.SetVariable("val", (int) element.GetVariable("val") + 1);
            string text;
            if (prevArrayElement != null)
            {
                text = $"{prevArrayElement.GetVariable("i")}\n<size=8>{prevArrayElement.GetVariable("val")}</size>";
                prevArrayElement.UpdateInfoText(text, false);
            }
            prevArrayElement = element;
            text = $"{element.GetVariable("i")}\n<size=8>{element.GetVariable("val")}</size>";
            element.UpdateInfoText(text, true);
            SetLogText($"Count {(int) elements[i].ElementValue}");
            yield return new WaitForSeconds(1 / animationSpeed);
            yield return WaitUntilPlaying();
        }
        if (prevArrayElement != null)
        {
            string text = $"{prevArrayElement.GetVariable("i")}\n<size=8>{prevArrayElement.GetVariable("val")}</size>";
            prevArrayElement.UpdateInfoText(text, false);
        }
        prevArrayElement = null;
        SetLogText("");
        Unselect();
        yield return new WaitForSeconds(1.5f / animationSpeed);

        for (int i = 1; i < runtimeArray.Count; i++)
        {
            RuntimeArrayElement element = runtimeArray[i];
            SelectOneArrayElement(element);
            element.SetVariable("val", (int)element.GetVariable("val") + (int)runtimeArray[i - 1].GetVariable("val"));
            string text;
            if (prevArrayElement != null)
            {
                text = $"{prevArrayElement.GetVariable("i")}\n<size=8>{prevArrayElement.GetVariable("val")}</size>";
                prevArrayElement.UpdateInfoText(text, false);
            }
            prevArrayElement = element;
            text = $"{element.GetVariable("i")}\n<size=8>{element.GetVariable("val")}</size>";
            element.UpdateInfoText(text, true);
            SetLogText($"C[{i}] = C[{i}] + C[{i-1}]");
            yield return new WaitForSeconds(1 / animationSpeed);
            yield return WaitUntilPlaying();
        }
        if (prevArrayElement != null)
        {
            string text = $"{prevArrayElement.GetVariable("i")}\n<size=8>{prevArrayElement.GetVariable("val")}</size>";
            prevArrayElement.UpdateInfoText(text, false);
        }
        SetLogText("");
        prevArrayElement = null;
        Unselect();
        yield return new WaitForSeconds(1.5f / animationSpeed);

        ListElement[] newList = new ListElement[elements.Count];
        for (int i = elements.Count - 1; i >= 0; i--)
        {
            SelectOne(elements[i]);
            int index = (int)elements[i].ElementValue - (int)runtimeArray[0].GetVariable("i");
            RuntimeArrayElement element = runtimeArray[index];
            element.SetVariable("val", (int)element.GetVariable("val") - 1);
            string text;
            if (prevArrayElement != null)
            {
                text = $"{prevArrayElement.GetVariable("i")}\n<size=8>{prevArrayElement.GetVariable("val")}</size>";
                prevArrayElement.UpdateInfoText(text, false);
            }
            prevArrayElement = element;
            text = $"{element.GetVariable("i")}\n<size=8>{element.GetVariable("val")}</size>";
            element.UpdateInfoText(text, true);
            SetLogText($"C[{element.GetVariable("i")}]--");
            yield return new WaitForSeconds(1 / animationSpeed);
            SetLogText($"Place {(int)elements[i].ElementValue}");
            yield return DragDown(i, (int)element.GetVariable("val"), true);
            newList[(int)element.GetVariable("val")] = elements[i];
            yield return new WaitForSeconds(1 / animationSpeed);
            Unselect();
            yield return new WaitForSeconds(1 / animationSpeed);
            yield return WaitUntilPlaying();
        }
        SetLogText("");
        if (prevArrayElement != null)
        {
            string text = $"{prevArrayElement.GetVariable("i")}\n<size=8>{prevArrayElement.GetVariable("val")}</size>";
            prevArrayElement.UpdateInfoText(text, false);
        }
        prevArrayElement = null;
        for (int i = 0; i < newList.Length; i++)
        {
            elements[i] = newList[i];
        }
        yield return DragUp(0, elements.Count - 1);

        yield return End();
        for (int i = 0; i < indices.Count; i++)
        {
            Destroy(indices[i]);
            indices[i] = null;
        }

        for (int i = 0; i < runtimeArray.Count; i++)
        {
            runtimeArray[i].Destroy();
            runtimeArray[i] = null;
        }
    }

    public IEnumerator Radix()
    {
        (ListElement, ListElement) minMax = ElementMinMax();
        int modifier = 0;
        if (minMax.Item1.ElementValue < 0)
        {
            SetLogText("Elements made nonnegative");
            modifier = -1 * ((int) minMax.Item1.ElementValue);
            foreach (ListElement element in elements)
            {
                element.SetValue((int)element.ElementValue + modifier);
            }
        }

        List<RuntimeArrayElement> runtimeArray = new List<RuntimeArrayElement>();
        RuntimeArrayElement prevArrayElement = null;
        Vector3 position = new Vector3(baseX, baseY + 3f);
        for (int i = 0; i < 10; i++)
        {
            RuntimeArrayElement element = Instantiate(Resources.Load<GameObject>("Prefabs/RuntimeArrayElement").GetComponent<RuntimeArrayElement>(), position, Quaternion.identity, GameObject.Find("Other").transform);
            runtimeArray.Add(element);
            element.CreateInfoText();
            element.AddVariable("i", i);
            element.AddVariable("val", 0);
            string text = $"{i}\n<size=8>{0}</size>";
            element.UpdateInfoText(text, false);
            position += new Vector3(2, 0, 0);
        }

        List<GameObject> indices = new List<GameObject>();
        for (int i = 0; i < elements.Count; i++)
        {
            GameObject indexText = Instantiate(Resources.Load<GameObject>("Prefabs/InfoText"), new Vector3(IndexToXPos(i), baseY - 3.6f), Quaternion.identity, GameObject.Find("Other").transform);
            indexText.GetComponent<TMP_Text>().text = "<color=#ffffffff>" + i.ToString() + "</color>";
            indices.Add(indexText);
        }

        yield return new WaitForSeconds(3 / animationSpeed);
        yield return WaitUntilPlaying();

        int iterations = 0;
        if (minMax.Item2.ElementValue > 0)
        {
            iterations = Mathf.FloorToInt(Mathf.Log10(minMax.Item2.ElementValue));
        }

        for (int digit = 1; digit <= (int)Mathf.Pow(10, iterations); digit *= 10)
        {
            SetLogText($"Counting sort using {digit}'s digit");
            for (int i = 0; i < runtimeArray.Count; i++)
            {
                runtimeArray[i].SetVariable("val", 0);
                string text = $"{i}\n<size=8>{0}</size>";
                runtimeArray[i].UpdateInfoText(text, false);
            }
            yield return new WaitForSeconds(2 / animationSpeed);
            yield return WaitUntilPlaying();
            //count
            for (int i = 0; i < elements.Count; i++)
            {
                SelectOne(elements[i]);
                int index = (((int)elements[i].ElementValue / digit) % 10) - (int)runtimeArray[0].GetVariable("i");
                RuntimeArrayElement element = runtimeArray[index];
                element.SetVariable("val", (int)element.GetVariable("val") + 1);
                string text;
                if (prevArrayElement != null)
                {
                    text = $"{prevArrayElement.GetVariable("i")}\n<size=8>{prevArrayElement.GetVariable("val")}</size>";
                    prevArrayElement.UpdateInfoText(text, false);
                }
                prevArrayElement = element;
                text = $"{element.GetVariable("i")}\n<size=8>{element.GetVariable("val")}</size>";
                element.UpdateInfoText(text, true);
                SetLogText($"Count {(int)elements[i].ElementValue} ({element.GetVariable("i")})");
                yield return new WaitForSeconds(1 / animationSpeed);
            }
            if (prevArrayElement != null)
            {
                string text = $"{prevArrayElement.GetVariable("i")}\n<size=8>{prevArrayElement.GetVariable("val")}</size>";
                prevArrayElement.UpdateInfoText(text, false);
            }
            prevArrayElement = null;
            SetLogText($"Counting sort using {digit}'s digit");
            Unselect();
            yield return new WaitForSeconds(1.5f / animationSpeed);
            yield return WaitUntilPlaying();

            //update count array
            for (int i = 1; i < runtimeArray.Count; i++)
            {
                RuntimeArrayElement element = runtimeArray[i];
                SelectOneArrayElement(element);
                element.SetVariable("val", (int)element.GetVariable("val") + (int)runtimeArray[i - 1].GetVariable("val"));
                string text;
                if (prevArrayElement != null)
                {
                    text = $"{prevArrayElement.GetVariable("i")}\n<size=8>{prevArrayElement.GetVariable("val")}</size>";
                    prevArrayElement.UpdateInfoText(text, false);
                }
                prevArrayElement = element;
                text = $"{element.GetVariable("i")}\n<size=8>{element.GetVariable("val")}</size>";
                element.UpdateInfoText(text, true);
                SetLogText($"C[{i}] = C[{i}] + C[{i - 1}]");
                yield return new WaitForSeconds(1 / animationSpeed);
            }
            if (prevArrayElement != null)
            {
                string text = $"{prevArrayElement.GetVariable("i")}\n<size=8>{prevArrayElement.GetVariable("val")}</size>";
                prevArrayElement.UpdateInfoText(text, false);
            }
            prevArrayElement = null;
            SetLogText($"Counting sort using {digit}'s digit");
            Unselect();
            yield return new WaitForSeconds(1.5f / animationSpeed);
            yield return WaitUntilPlaying();

            //sort
            ListElement[] newList = new ListElement[elements.Count];
            for (int i = elements.Count - 1; i >= 0; i--)
            {
                SelectOne(elements[i]);
                int index = (((int)elements[i].ElementValue / digit) % 10) - (int)runtimeArray[0].GetVariable("i");
                RuntimeArrayElement element = runtimeArray[index];
                element.SetVariable("val", (int)element.GetVariable("val") - 1);
                string text;
                if (prevArrayElement != null)
                {
                    text = $"{prevArrayElement.GetVariable("i")}\n<size=8>{prevArrayElement.GetVariable("val")}</size>";
                    prevArrayElement.UpdateInfoText(text, false);
                }
                prevArrayElement = element;
                text = $"{element.GetVariable("i")}\n<size=8>{element.GetVariable("val")}</size>";
                element.UpdateInfoText(text, true);
                SetLogText($"C[{element.GetVariable("i")}]--");
                yield return new WaitForSeconds(1f / animationSpeed);
                SetLogText($"Place {(int)elements[i].ElementValue}");
                yield return DragDown(i, (int)element.GetVariable("val"), true);
                newList[(int)element.GetVariable("val")] = elements[i];
                yield return new WaitForSeconds(1f / animationSpeed);
                Unselect();
                yield return new WaitForSeconds(1f / animationSpeed);
            }
            if (prevArrayElement != null)
            {
                string text = $"{prevArrayElement.GetVariable("i")}\n<size=8>{prevArrayElement.GetVariable("val")}</size>";
                prevArrayElement.UpdateInfoText(text, false);
            }
            SetLogText($"Counting sort using {digit}'s digit");
            prevArrayElement = null;
            for (int i = 0; i < newList.Length; i++)
            {
                elements[i] = newList[i];
            }
            yield return DragUp(0, elements.Count - 1);
            yield return new WaitForSeconds(2f / animationSpeed);
            yield return WaitUntilPlaying();
        }

        if (modifier > 0)
        {
            SetLogText("Elements returned to normal");
            foreach (ListElement element in elements)
            {
                element.SetValue((int)element.ElementValue - modifier);
            }
        }

        yield return End();
        for (int i = 0; i < indices.Count; i++)
        {
            Destroy(indices[i]);
            indices[i] = null;
        }

        for (int i = 0; i < runtimeArray.Count; i++)
        {
            runtimeArray[i].Destroy();
            runtimeArray[i] = null;
        }
    }

    public IEnumerator Bucket()
    {
        (ListElement, ListElement) minMax = ElementMinMax();
        float range = minMax.Item2.ElementValue - minMax.Item1.ElementValue;
        List<RuntimeArrayElement> buckets = new List<RuntimeArrayElement>();
        List<List<ListElement>> elementsInBuckets = new List<List<ListElement>>();
        SetLogText("");

        Vector3 position = new Vector3(baseX, baseY - 3f);
        for (int i = 0; i < parameter; i++)
        {
            RuntimeArrayElement element = Instantiate(Resources.Load<GameObject>("Prefabs/RuntimeArrayElement").GetComponent<RuntimeArrayElement>(), position, Quaternion.identity, GameObject.Find("Other").transform);
            buckets.Add(element);
            element.CreateInfoText();
            element.AddVariable("min", minMax.Item1.ElementValue + (((float)i) / parameter) * range);
            element.AddVariable("max", minMax.Item1.ElementValue + (((float)(i + 1)) / parameter) * range);
            string text = $"{((int)(((float) element.GetVariable("min")) * 100)) / 100f} - {((int)(((float)element.GetVariable("max")) * 100)) / 100f}";
            element.UpdateInfoText(text, false);
            position += new Vector3(2.5f, 0, 0);

            elementsInBuckets.Add(new List<ListElement>());
        }

        yield return new WaitForSeconds(3f / animationSpeed);
        yield return WaitUntilPlaying();

        foreach (ListElement element in elements)
        {
            SelectOne(element);
            SetLogText(element.ElementValue.ToString());
            yield return new WaitForSeconds(1f / animationSpeed);
            int bucket = (int) (((element.ElementValue - minMax.Item1.ElementValue) / range) * parameter);
            if (bucket >= parameter)
            {
                bucket = parameter - 1;
            }
            elementsInBuckets[bucket].Add(element);
            SetLogText(element.ElementValue.ToString() + " -> Bucket " + ((float)buckets[bucket].GetVariable("min")).ToString() + " - " + ((float)buckets[bucket].GetVariable("max")).ToString());
            yield return Move(new Vector3(buckets[bucket].transform.position.x, buckets[bucket].transform.position.y - (2 * elementsInBuckets[bucket].Count)), element, true);
            yield return new WaitForSeconds(1f / animationSpeed);
            yield return WaitUntilPlaying();
        }
        SetLogText("");
        Unselect();

        yield return new WaitForSeconds(2f / animationSpeed);

        SetLogText("Insertion sorting buckets");
        foreach (List<ListElement> bucketList in elementsInBuckets)
        {
            for (int i = 1; i < bucketList.Count; i++)
            {
                int j = i;
                while (j > 0 && bucketList[j - 1].ElementValue > bucketList[j].ElementValue)
                {
                    ListElement temp = bucketList[j - 1];
                    bucketList[j - 1] = bucketList[j];
                    bucketList[j] = temp;
                    SelectOne(bucketList[j - 1]);
                    SelectTwo(bucketList[j]);
                    yield return new WaitForSeconds(1f / animationSpeed);
                    yield return Swap(bucketList[j - 1], bucketList[j]);
                    j--;
                    yield return new WaitForSeconds(1f / animationSpeed);
                    yield return WaitUntilPlaying();
                }
            }
        }
        Unselect();

        yield return new WaitForSeconds(2f / animationSpeed);

        float x = baseX;
        int index = 0;
        SetLogText("Merge buckets");
        foreach (List<ListElement> bucketList in elementsInBuckets)
        {
            if (bucketList.Count > 0)
            {
                for (int i = 0; i < bucketList.Count; i++)
                {
                    elements[index] = bucketList[i];
                    index++;
                }
                yield return LineUp(bucketList, x);
                x += 2f * bucketList.Count;
                yield return new WaitForSeconds(2f / animationSpeed);
                yield return WaitUntilPlaying();
            }
        }

        yield return End();

        for (int i = 0; i < buckets.Count; i++)
        {
            buckets[i].Destroy();
            buckets[i] = null;
        }
    }

    public IEnumerator QuickSelect()
    {
        SetLogText("");
        (ListElement, ListElement) minMax = ElementMinMax();
        float min = minMax.Item1.ElementValue;
        float max = minMax.Item2.ElementValue;
        yield return new WaitForSeconds(3 / animationSpeed);
        yield return WaitUntilPlaying();

        Stack<(int, int)> stack = new Stack<(int, int)>();
        stack.Push((0, elements.Count - 1));
        while (stack.Count > 0)
        {
            (int, int) tuple = stack.Pop();
            if (tuple.Item1 >= tuple.Item2)
            {
                if (tuple.Item1 == tuple.Item2)
                {
                    elements[tuple.Item1].SetColor(new Color(0.5f, 0.5f, 0.5f));
                    yield return new WaitForSeconds(1f / animationSpeed);
                    if (tuple.Item1 == parameter - 1)
                    {
                        SelectOne(elements[tuple.Item1]);
                        SetLogText($"{Ordinal(parameter)} smallest element: {elements[tuple.Item1].ElementValue}");
                        break;
                    }
                }
                continue;
            }
            List<ListElement> leftSide = new List<ListElement>();
            List<ListElement> rightSide = new List<ListElement>();
            ListElement pivot = elements[tuple.Item2];
            pivot.SetColor(new Color(1f, 0.8f, 0.8f));
            SetLogText("Partitioning: " + ListToString(elements, tuple.Item1, tuple.Item2 - 1) + " | Pivot: " + pivot.ToString());
            SelectOne(pivot);
            yield return new WaitForSeconds(2 / animationSpeed);
            yield return WaitUntilPlaying();
            for (int i = tuple.Item1; i < tuple.Item2; i++)
            {
                SetLogText("Comparing: " + elements[i].ToString() + " v. " + pivot.ToString());
                SelectTwo(elements[i]);
                comparisons.Add(new Comparison(elements[i].ElementValue, pivot.ElementValue));
                if (elements[i].ElementValue <= pivot.ElementValue)
                {
                    leftSide.Add(elements[i]);
                    elements[i].SetColor(new Color(0.9f, 0.9f, 1f));
                }
                else
                {
                    rightSide.Add(elements[i]);
                    elements[i].SetColor(new Color(0.9f, 1f, 0.9f));
                }
                yield return new WaitForSeconds(1f / animationSpeed);
                yield return WaitUntilPlaying();
            }
            SetLogText("Partitioning: " + ListToString(elements, tuple.Item1, tuple.Item2 - 1) + " | Pivot: " + pivot.ToString());
            UnselectTwo();
            int index = tuple.Item1;
            foreach (ListElement e in leftSide)
            {
                elements[index] = e;
                index++;
            }
            elements[index] = pivot;
            bool done = false;
            if (index == parameter - 1)
            {
                done = true;
            }
            else if (index > parameter - 1)
            {
                stack.Push((tuple.Item1, index - 1));
            }
            else
            {
                stack.Push((index + 1, tuple.Item2));
            }
            index++;
            foreach (ListElement e in rightSide)
            {
                elements[index] = e;
                index++;
            }
            yield return SetNewPositions(pivot, tuple.Item1, tuple.Item2);
            yield return new WaitForSeconds(1 / animationSpeed);
            pivot.SetColor(new Color(0.5f, 0.5f, 0.5f));
            foreach (ListElement e in leftSide)
            {
                float relative = (max - e.ElementValue) / (max - min);
                e.SetColor(new Color(1 - 0.3f * relative, 1 - 0.3f * relative, 1 - 0.3f * relative));
            }
            foreach (ListElement e in rightSide)
            {
                float relative = (max - e.ElementValue) / (max - min);
                e.SetColor(new Color(1 - 0.3f * relative, 1 - 0.3f * relative, 1 - 0.3f * relative));
            }
            if (done)
            {
                SelectOne(elements[parameter - 1]);
                SetLogText($"{Ordinal(parameter)} smallest element: {elements[parameter-1].ElementValue}");
                break;
            }
            UnselectOne();
            SetLogText("");
            yield return new WaitForSeconds(1 / animationSpeed);
            yield return WaitUntilPlaying();
        }
        yield return End();
        Unselect();
    }

    public IEnumerator Insertion()
    {
        SetLogText("");
        yield return new WaitForSeconds(3 / animationSpeed);
        yield return WaitUntilPlaying();

        for (int i = 1; i < elements.Count; i++)
        {
            int j = i;
            SetLogText("Comparing: " + elements[j - 1].ElementValue + " v. " + elements[j].ElementValue);
            comparisons.Add(new Comparison(elements[j - 1].ElementValue, elements[j].ElementValue));
            UnselectOne();
            SelectTwo(elements[j]);
            yield return new WaitForSeconds(2f / animationSpeed);
            while (j > 0 && elements[j - 1].ElementValue > elements[j].ElementValue)
            {
                ListElement temp = elements[j - 1];
                elements[j - 1] = elements[j];
                elements[j] = temp;
                
                SetLogText("Swapping " + elements[j - 1].ElementValue + " and " + elements[j].ElementValue);
                yield return Swap(elements[j], elements[j-1]);
                j--;
                yield return new WaitForSeconds(2f / animationSpeed);
                yield return WaitUntilPlaying();
                if (j > 0)
                {
                    UnselectOne();
                    SelectTwo(elements[j]);
                    SetLogText("Comparing: " + elements[j - 1].ElementValue + " v. " + elements[j].ElementValue);
                    comparisons.Add(new Comparison(elements[j - 1].ElementValue, elements[j].ElementValue));
                    yield return new WaitForSeconds(2f / animationSpeed);
                }
            }
            yield return WaitUntilPlaying();
        }
        Unselect();
        SetLogText("<size=15>Comparisons: " + ListToString(comparisons, 0, comparisons.Count - 1) + "</size>");
        yield return End();
    }

    public IEnumerator DragDown(int index, int newIndex, bool one)
    {
        yield return Move(new Vector3(IndexToXPos(newIndex), elements[index].transform.position.y - 2.3f), elements[index], one);
    }
    public IEnumerator Move(Vector3 position, ListElement element, bool one)
    {
        element.SetMovement((position - element.transform.position) / 10f);
        for (int i = 0; i < 10; i++)
        {
            element.Move();
            if (one)
            {
                SelectOne(element);
            }
            else
            {
                SelectTwo(element);
            }
            yield return new WaitForSeconds(0.02f / animationSpeed);
        }
        element.transform.position = position;
        element.UpdatePosition();
        if (one)
        {
            SelectOne(element);
        }
        else
        {
            SelectTwo(element);
        }
    }

    public IEnumerator DragUp(int startIndex, int endIndex)
    {
        for (int i = startIndex; i <= endIndex; i++)
        {
            elements[i].SetMovement(new Vector3(0, 0.23f));
        }
        for (int j = 0; j < 10; j++)
        {
            for (int i = startIndex; i <= endIndex; i++)
            {
                elements[i].Move();
            }
            yield return new WaitForSeconds(0.02f / animationSpeed);
        }
        for (int i = startIndex; i <= endIndex; i++)
        {
            elements[i].transform.position = new Vector3(IndexToXPos(i), baseY);
            elements[i].UpdatePosition();
        }
    }

    public IEnumerator SetNewPositions(ListElement pivot, int startIndex, int endIndex)
    {
        for (int i = startIndex; i <= endIndex; i++)
        {
            if (IndexToXPos(i) == elements[i].transform.position.x)
            {
                elements[i].SetMovement(Vector3.zero);
            }
            else if (IndexToXPos(i) < elements[i].transform.position.x)
            {
                elements[i].SetMovement(new Vector3((IndexToXPos(i) - elements[i].transform.position.x) / 2f, -1f) / 10f);
            }
            else
            {
                elements[i].SetMovement(new Vector3((IndexToXPos(i) - elements[i].transform.position.x) / 2f, 1f) / 10f);
            }
        }
        for (int j = 0; j < 10; j++)
        {
            for (int i = startIndex; i <= endIndex; i++)
            {
                elements[i].Move();
                if (elements[i] == pivot)
                {
                    SelectOne(pivot);
                }
            }
            yield return new WaitForSeconds(0.02f / animationSpeed);
        }
        yield return new WaitForSeconds(0.1f / animationSpeed);
        for (int i = startIndex; i <= endIndex; i++)
        {
            if (IndexToXPos(i) == elements[i].transform.position.x)
            {
                elements[i].SetMovement(Vector3.zero);
            }
            else if (IndexToXPos(i) < elements[i].transform.position.x)
            {
                elements[i].SetMovement(new Vector3(IndexToXPos(i) - elements[i].transform.position.x, 1f) / 10f);
            }
            else
            {
                elements[i].SetMovement(new Vector3(IndexToXPos(i) - elements[i].transform.position.x, -1f) / 10f);
            }
        }
        for (int j = 0; j < 10; j++)
        {
            for (int i = startIndex; i <= endIndex; i++)
            {
                elements[i].Move();
                if (elements[i] == pivot)
                {
                    SelectOne(pivot);
                }
            }
            yield return new WaitForSeconds(0.02f / animationSpeed);
        }
        for (int i = startIndex; i <= endIndex; i++)
        {
            elements[i].transform.position = new Vector3(IndexToXPos(i), baseY);
            elements[i].UpdatePosition();
            if (elements[i] == pivot)
            {
                SelectOne(pivot);
            }
        }
    }

    public IEnumerator Swap(ListElement e1, ListElement e2)
    {
        Vector3 p1 = e1.transform.position;
        Vector3 p2 = e2.transform.position;
        e1.SetMovement((p2 - p1) / 10f);
        e2.SetMovement((p1 - p2) / 10f);
        for (int i = 0; i < 10; i++)
        {
            e1.Move();
            e2.Move();
            SelectOne(e1);
            SelectTwo(e2);
            yield return new WaitForSeconds(0.02f / animationSpeed);
        }
        e1.transform.position = p2;
        e1.UpdatePosition();
        e2.transform.position = p1;
        e2.UpdatePosition();
        SelectOne(e1);
        SelectTwo(e2);
    }

    public IEnumerator LineUp(List<ListElement> list, float startX)
    {
        Vector3[] positions = new Vector3[list.Count];
        for (int i = 0; i < list.Count; i++)
        {
            Vector3 pos = new Vector3(startX + (2 * i), baseY);
            positions[i] = pos;
            list[i].SetMovement((pos - list[i].transform.position) / 10f);
        }
        
        for (int i = 0; i < 10; i++)
        {
            foreach (ListElement element in list)
            {
                element.Move();
            }
            yield return new WaitForSeconds(0.02f / animationSpeed);
        }
        for (int i = 0; i < list.Count; i++)
        {
            list[i].transform.position = positions[i];
        }
    }

    private string Ordinal(int num)
    {
        if (num == 1)
        {
            return "1st";
        }
        else if (num == 2)
        {
            return "2nd";
        }
        else if (num == 3)
        {
            return "3rd";
        }
        else
        {
            return num + "th";
        }
    }

    private float IndexToXPos(int index)
    {
        return baseX + (2 * index);
    }

    public (ListElement, ListElement) ElementMinMax()
    {
        (ListElement, ListElement) tuple = (elements[0], elements[0]);
        for (int i = 1; i < elements.Count; i++)
        {
            if (elements[i].ElementValue > tuple.Item2.ElementValue)
            {
                tuple.Item2 = elements[i];
            }
            if (elements[i].ElementValue < tuple.Item1.ElementValue)
            {
                tuple.Item1 = elements[i];
            }
        }
        return tuple;
    }

    public void SelectOne(ListElement e)
    {
        elementSelected.transform.position = e.transform.position;
        elementSelected.GetComponent<SpriteRenderer>().enabled = true;
    }

    public void SelectTwo(ListElement e)
    {
        elementSelected2.transform.position = e.transform.position;
        elementSelected2.GetComponent<SpriteRenderer>().enabled = true;
    }

    public void SelectOneArrayElement(RuntimeArrayElement e)
    {
        elementSelected.transform.position = e.transform.position;
        elementSelected.GetComponent<SpriteRenderer>().enabled = true;
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

    public string ListToString<T>(List<T> list, int startIndex, int endIndex)
    {
        string s = "";
        if (endIndex >= startIndex)
        {
            s += list[startIndex].ToString();
            for (int i = startIndex + 1; i <= endIndex; i++)
            {
                s += ", " + list[i].ToString();
            }
        }
        return s;
    }
}
