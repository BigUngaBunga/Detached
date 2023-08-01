using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.SceneManagement;
using TMPro;
using Mirror;
public class UI : MonoBehaviour
{
    public GameObject pauseUI;
    public GameObject optionUIMenu;
    public GameObject controlUIMenu;
    public GameObject mainMenu;
    public GameObject levelSelectMenu;
    public GameObject promptObj;
    private TextMeshProUGUI promptText;
    public static bool gameIsPaused;

    private void Awake()
    {
        promptText = promptObj.GetComponent<TextMeshProUGUI>();
    }
    void Start()
    {
        gameIsPaused = false;
        DontDestroyOnLoad(gameObject);
        //cam = FindObjectOfType<CinemachineFreeLook>();

    }
    public void ResumeGame()
    {
        //Time.timeScale = 1;
        gameIsPaused = false;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        optionUIMenu.SetActive(false);
        controlUIMenu.SetActive(false);
    }
    public void FreezeGame()
    {
        gameIsPaused = true;
        //Time.timeScale = 0;
    }

    public void RestartGame()
    {
        Time.timeScale = 1;
        gameIsPaused = false;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        FallOutOfWorld script = GameObject.Find("FallOutOfWorldTrigger").GetComponent<FallOutOfWorld>();
        script.ChangeScene();

    }

    [System.Obsolete]
    public void LoadMainMenu()
    {
        Application.LoadLevel("SteamLobby");
    }

    public void IngameChangeLevel(int i)
    {
        //if (isServer)
        
           Debug.Log("Change Level to " + i);
        
        
            Debug.Log("Start prompt");
        
    }

    public void prompt(string prompt)
    {
       
    }
    void Update()
    {

        if (Input.GetKeyDown(KeyCode.Escape) && !gameIsPaused && !mainMenu && 2 <= SceneManager.GetActiveScene().buildIndex)
        {
            pauseUI.SetActive(true);
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            FreezeGame();
        }

        else if (Input.GetKeyDown(KeyCode.Escape) && !mainMenu && gameIsPaused && 2 <= SceneManager.GetActiveScene().buildIndex)
        {
            if (levelSelectMenu)
            {
                DisableLevelListScreen();
            }
            else
            {
                pauseUI.SetActive(false);
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
                ResumeGame();
            }
        }
        //if (Input.GetKeyDown("9"))
        //{
        //    pauseUI.SetActive(false);
        //    Cursor.lockState = CursorLockMode.Locked;
        //    Cursor.visible = false;
        //}
    }

    public void EnableLevelListScreen()
    {
        levelSelectMenu.SetActive(true);
        pauseUI.SetActive(false);
    }

    public void DisableLevelListScreen()
    {
        levelSelectMenu.SetActive(false);
        pauseUI.SetActive(true);
    }
    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        Debug.Log("QUIT!");

#else
        Application.Quit();
#endif

    }
}
