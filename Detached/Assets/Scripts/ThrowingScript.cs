using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class ThrowingScript : NetworkBehaviour
{

    [Header("References")]
    public Transform cam;


    public List<GameObject> limbList;
    List<DetachScript> detachScript;
    Rigidbody objectRb;

    public int selectHead;
    public int selectArm;
    public int selectLeg;
    public int select;

    int startingArmNum = 0;
    int startingLegNum = 2;


    public KeyCode throwKey = KeyCode.Mouse0;
    public KeyCode selectHeadKey = KeyCode.Alpha1;
    public KeyCode cycleArm = KeyCode.Alpha2;
    public KeyCode cycleLeg = KeyCode.Alpha3;
    public float throwForce;
    public float throwUpwardForce;

    public float throwCD;
    bool readyToThrow;
    bool arm;
    bool leg;


    void Start()
    {
        arm = true;
        leg = true;
        selectHead = 0;
        selectArm = startingArmNum;
        selectLeg = startingLegNum;
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
        if (Input.GetKey(selectHeadKey))
        {
            select = selectHead;
            selectArm = startingArmNum;
            selectLeg = startingLegNum;
        }
        if (Input.GetKey(cycleArm) && !arm)
        {
            selectArm++;
            if (selectArm > 2)
                selectArm = 1;
            select = selectArm;
            selectLeg = startingLegNum;
        }
        else if (Input.GetKey(cycleLeg) && !leg)
        {
            selectLeg++;
            if (selectLeg > 4)
                selectLeg = 3;
            select = selectLeg;
            selectArm = startingArmNum;
        }
        arm = Input.GetKey(cycleArm);
        leg = Input.GetKey(cycleLeg);

        if (Input.GetKey(throwKey) && readyToThrow /* && !detachScript[select].detached  && DetachScript.numOfArms > 1*/)
        {

            if (!limbList[select].GetComponent<Rigidbody>())
                limbList[select].AddComponent<Rigidbody>();

            //if (detachScript[select].partName == "Arm")
                //DetachScript.numOfArms--;

            detachScript[select].detached = true;
            Throw();
        }
    }

    [Command]
    void CmdThrowLimb()
    {

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


