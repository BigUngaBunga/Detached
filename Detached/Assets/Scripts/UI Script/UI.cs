using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI : MonoBehaviour
{
    // Start is called before the first frame update

    public GameObject pauseUI;
    public GameObject mainMenu;
    public bool gameIsPaused;
    void Start()
    {
        DontDestroyOnLoad(this.gameObject);
    }
    public void ResumeGame()
    {
        Time.timeScale = 1;
        gameIsPaused = false;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
    public void FreezeGame()
    {
        gameIsPaused = true;
        Time.timeScale = 0;
    }
    // Update is called once per frame
    void Update()
    {

        if (Input.GetKeyDown(KeyCode.Escape) && !gameIsPaused)
        {
            pauseUI.SetActive(true);
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            FreezeGame();
        }

        if (Input.GetKeyDown(KeyCode.Escape) && !gameIsPaused)
        {
            pauseUI.SetActive(false);
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            ResumeGame();
        }

        //if (Input.GetKeyDown("9"))
        //{
        //    pauseUI.SetActive(false);
        //    Cursor.lockState = CursorLockMode.Locked;
        //    Cursor.visible = false;
        //}
    }
}
