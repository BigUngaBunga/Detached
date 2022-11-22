using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadUI : MonoBehaviour
{
    public GameObject gameUI;
    // Start is called before the first frame update
    void Start()
    {
        DontDestroyOnLoad(gameUI);
     
    }


    // Update is called once per frame
    void Update()
    {
        gameUI.SetActive(true);
    }

}
