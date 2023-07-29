using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        MoveCamera();
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
}
