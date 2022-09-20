using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetachScript : MonoBehaviour
{
    [Header("Keybindings")]
    public KeyCode detachKey;
    public KeyCode attachKey;

    bool detached;
    public Transform limbParent;
    public Transform body;
    // Start is called before the first frame update
    private void Awake()
    {

    }
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(detachKey) && detached == false)
        {
            gameObject.AddComponent<Rigidbody>();
            gameObject.transform.parent = null;
            detached = true;
        }
        else if (Input.GetKeyDown(attachKey) && detached == true)
        {
            Destroy(GetComponent<Rigidbody>());
            gameObject.transform.parent = limbParent;
            gameObject.transform.localPosition = Vector3.zero;
            gameObject.transform.localEulerAngles = Vector3.zero;
            gameObject.transform.localScale = Vector3.one;
            detached = false;

        }
    }
}
