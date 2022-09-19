using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ItemController : MonoBehaviour
{
    public int ID;
    public int height;
    public TextMeshProUGUI heightText;
    public bool Clicked = false;
    private LevelEditorManager editor;


    // Start is called before the first frame update
    void Start()
    {
        heightText.text = height.ToString();
        editor = GameObject.FindGameObjectWithTag("LevelEditorManager").GetComponent<LevelEditorManager>();
    }

    public void ButtonClicked()
    {
        Clicked = true;
        editor.CurrentButtonPressed = ID;
    }
}
