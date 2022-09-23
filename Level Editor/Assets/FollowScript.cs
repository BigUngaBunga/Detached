using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowScript : MonoBehaviour
{

    void Update()
    {
        transform.position = GameObject.FindGameObjectWithTag("LevelEditorManager").GetComponent<LevelEditorManager>().GetMousePosition3D();
        transform.position.Set(transform.position.x, 3, transform.position.z);
    }
}
