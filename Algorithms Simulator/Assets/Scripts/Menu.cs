using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{
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
        
    }
    
    public void ChangeScene(string s)
    {
        SceneManager.LoadScene(s);
    }

}
