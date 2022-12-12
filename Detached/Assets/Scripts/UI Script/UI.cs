using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI : MonoBehaviour
{
    // Start is called before the first frame update

    public GameObject pauseUI;
    public GameObject gameUI;
    void Start()
    {
        DontDestroyOnLoad(this.gameObject);
    }
    public void ResumeGame()
    {
        Time.timeScale = 1;
    }
    // Update is called once per frame
    void Update()
    {

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            pauseUI.SetActive(true);
            Cursor.lockState = CursorLockMode.None;
            Time.timeScale = 0;
        }

        if (Input.GetKeyDown("9"))
        {
            pauseUI.SetActive(false);
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }
}
