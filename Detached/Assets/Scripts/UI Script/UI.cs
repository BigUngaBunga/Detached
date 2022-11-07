using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI : MonoBehaviour
{
    // Start is called before the first frame update

    public GameObject pauseUI, gameUI;

    void Start()
    {
        DontDestroyOnLoad(this.gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            pauseUI.SetActive(true);
            //gameUI.SetActive(false);
        }

        if (Input.GetKeyDown("9"))
        {
            pauseUI.SetActive(false);
        }
        if (Input.GetKeyDown("8"))
        {
            gameUI.SetActive(true);
        }
    }
}
