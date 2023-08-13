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
    private bool selectingPhase = false;
    private bool running = false;
    private GameObject selectedObject;
    private float selectedTime = float.MinValue;
    private Vector3 prevMousePosition;
    private float animationSpeed;

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

    // Start is called before the first frame update
    void Start()
    {
        sourceText.SetActive(false);
        runtimeUI.SetActive(false);
        endButton.SetActive(false);
        vertexSelected.SetActive(false);
        vertexSelected2.SetActive(false);
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
                    if (element1 != null && element != element1)
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
        running = true;
        selectingPhase = false;
        backButton.SetActive(false);
        sourceText.SetActive(false);

        animationSpeed = 1;
        animSpeed.GetComponent<Slider>().value = 1;

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
        yield return null;
    }

    public IEnumerator Union()
    {
        yield return null;
    }
}
