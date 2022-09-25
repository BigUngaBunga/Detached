using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Throwing : MonoBehaviour
{

    [Header("References")]
    public Transform cam;
    public Transform throwPoint;

    public List<GameObject> limbList;
    List<DetachScript> detachScript;
    Rigidbody objectRb;

    public int select;

    public KeyCode throwKey = KeyCode.Mouse0;
    public KeyCode cycleNext = KeyCode.Alpha2;
    public KeyCode cyclePrev = KeyCode.Alpha1;
    public float throwForce;
    public float throwUpwardForce;

    public float throwCD;
    bool readyToThrow;
    bool next;
    bool prev;


    void Start()
    {
        next = true;
        prev = true;
        //limbList = new List<GameObject>();
        detachScript = new List<DetachScript>();
        // objectRb = objectToThrow.GetComponent<Rigidbody>();
        for (int i = 0; i < limbList.Count; i++)
        {
            detachScript.Add(limbList[i].GetComponent<DetachScript>());
        }

        readyToThrow = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(cycleNext) &&!next)
        {
            select++;
            if (select >= limbList.Count)
                select = 0;
        }
        else if (Input.GetKey(cyclePrev) &&!prev)
        {
            select--;
            if (select < 0)
                select = limbList.Count - 1;
        }
        next = Input.GetKey(cycleNext);
        prev = Input.GetKey(cyclePrev);

        if (Input.GetKey(throwKey) && readyToThrow && detachScript[select].detached)
        {
            Throw();
        }
    }

    void Throw()
    {

        readyToThrow = false;

        // GameObject throwObject = Instantiate(objectToThrow, throwPoint.position, cam.rotation);

        objectRb = limbList[select].GetComponent<Rigidbody>();
        Vector3 forceToAdd = cam.transform.forward * throwForce + transform.up * throwUpwardForce;

        objectRb.AddForce(forceToAdd, ForceMode.Impulse);

        Invoke(nameof(ResetThrow), throwCD);
    }

    void ResetThrow()
    {
        readyToThrow = true;
    }
}
