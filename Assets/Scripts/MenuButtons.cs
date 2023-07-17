using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

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
        SceneManager.LoadScene("Level 1");

    }
    public void OnClickControlls()
    {
        if (controlls.activeSelf)
        {
            controlls.SetActive(false);
        }
        else
        {
            controlls.SetActive(true);
            settings.SetActive(false);
        }
        
    }
    public void OnClickSettings()
    {
        if (settings.activeSelf)
        {
            settings.SetActive(false);
        }
        else
        {
            settings.SetActive(true);
            controlls.SetActive(false);
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
