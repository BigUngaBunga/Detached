using UnityEngine;
using UnityEditor;
public class MainMenu : MonoBehaviour
{
    public void PlayGame()
    {
        //SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
    void Start()
    {
        DontDestroyOnLoad(this.gameObject);

    }
    public void QuitGame()
    {
        Debug.Log("QUIT!");
        //EditorApplication.isPlaying = false; // causing build errors because we're not in editor when playing from an app standpoint
        Application.Quit();

    }
}
