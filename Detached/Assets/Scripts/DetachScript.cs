using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetachScript : MonoBehaviour
{
    [Header("Keybindings")]
    public KeyCode detachKey;
    public KeyCode attachKey;

    public bool detached;
    public Transform limbParent;
    public Transform detachedList;
    public Transform body;


    public string partName;

    public static int numOfArms;
    public static int numOfLegs;

    // Start is called before the first frame update
    private void Awake()
    {

    }
    void Start()
    {

        if (partName == "Arm")
            numOfArms++;  
        if (partName == "Leg")
            numOfLegs++;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(detachKey) && detached == false)
        {

            if (!gameObject.GetComponent<Rigidbody>())
                gameObject.AddComponent<Rigidbody>();

            if (partName == "Arm")
                numOfArms--;

            if (partName == "Leg")
                numOfLegs--;

            gameObject.transform.parent = detachedList;
            detached = true;
        }
        else if (Input.GetKeyDown(attachKey) && detached == true)
        {

            if (partName == "Leg")
            {
                body.transform.position += new Vector3(0, gameObject.GetComponent<Collider>().bounds.size.y, 0);
                numOfLegs++;
            }

            if (partName == "Arm")
                numOfArms++;

            Destroy(GetComponent<Rigidbody>());
            gameObject.transform.parent = limbParent;
            gameObject.transform.localPosition = Vector3.zero;
            gameObject.transform.localEulerAngles = Vector3.zero;
            gameObject.transform.localScale = Vector3.one;
            detached = false;


        }
    }
}
