using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuButtons : MonoBehaviour
{
    public GameObject controlls;
    public GameObject settings;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void OnClickStartGame()
    {
        //load Level One// SceneManager.LoadScene(1);

    }
    public void OnClickControlls()
    {
        if (controlls.activeSelf || settings.activeSelf)
        {
            controlls.SetActive(false);
        }
        else
        {
            controlls.SetActive(true);
        }
        
    }
    public void OnClickSettings()
    {
        if (settings.activeSelf || controlls.activeSelf)
        {
            settings.SetActive(false);
        }
        else
        {
            settings.SetActive(true);
        }

    }

    public void OnClickQuit()
    {
        Application.Quit();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
