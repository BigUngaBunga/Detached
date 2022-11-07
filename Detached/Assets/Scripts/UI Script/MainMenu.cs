using UnityEngine;
using UnityEditor;
public class MainMenu : MonoBehaviour
{
    public void PlayGame()
    {
        //SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void QuitGame()
    {
        Debug.Log("QUIT!");
        EditorApplication.isPlaying = false;
        Application.Quit();

    }
}
