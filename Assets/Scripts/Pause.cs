using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class Pause : MonoBehaviour
{
    public GameObject pauseMenu;
    // Start is called before the first frame update
    void Start()
    {

    }

    public void OnClickMenu()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene("Menu");
    }

    public void OnClickQuit()
    {
        Application.Quit();
    }


    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("escape"))
        {
            Debug.Log("esc pressed");
            if (pauseMenu.activeSelf == false)
            {
                pauseMenu.SetActive(true);
                Time.timeScale = 0;
            }
            else
            {
                pauseMenu.SetActive(false);
                Time.timeScale = 1;
            }
        }
    }
}
