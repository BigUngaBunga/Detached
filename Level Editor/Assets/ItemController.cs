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
        Vector3 worldPosition = editor.GetMousePosition3D();
        //Debug.Log("X:" + worldPosition.x + " Y:" + worldPosition.y + " Z:"+ worldPosition.z);

        Clicked = true;

        Instantiate(editor.ItemImage[ID], new Vector3(worldPosition.x, worldPosition.y, worldPosition.z), Quaternion.identity);
        editor.CurrentButtonPressed = ID;
    }
}
