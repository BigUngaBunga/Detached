using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowScript : MonoBehaviour
{

    void Update()
    {
        LevelEditorManager editor = GameObject.FindGameObjectWithTag("LevelEditorManager").GetComponent<LevelEditorManager>();
        Vector3 worldPosition = Vector3.zero;
        worldPosition = editor.GetMousePosition3D();
        Debug.Log("X:" + worldPosition.x + " Y:" + worldPosition.y + " Z:" + worldPosition.z);
        transform.position = new Vector3(worldPosition.x, worldPosition.y + editor.ItemButtons[editor.CurrentButtonPressed].height, worldPosition.z);
    }
}
