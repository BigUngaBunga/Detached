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

    bool dragging;

    //public Transform throwPoint;

    Vector3 originPos;

    private Vector3 mousePressDownPos;
    private Vector3 mouseReleasePos;
    Vector3 dir;
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

        ThrowButton();
        if (dragging && !detachScript[select].detached)
        {
            if (detachScript[select].partName == "Arm" && DetachScript.numOfArms < 2)
                return;
            TrajectoryCal();
        }
    }

    private void TrajectoryCal()
    {
        Vector3 forceInit = Input.mousePosition - mousePressDownPos + cam.transform.forward * throwForce + transform.up * throwUpwardForce; //idek what im doing anymore
        Vector3 forceV = new Vector3(forceInit.x, forceInit.y, z: forceInit.y);
        dir = (Input.mousePosition - mousePressDownPos).normalized;
        //if (readyToThrow)
        //{
        /*      if (!limbList[select].GetComponent<Rigidbody>())
                  limbList[select].AddComponent<Rigidbody>();*/
        //DrawTrajectory.instance.UpdateTrajectory(forceV, limbList[select].transform.position, dir.y); //throwing point = body?
        //}
    }

    private void ThrowButton()
    {
        if (Input.GetMouseButtonDown(1))
        {
            mousePressDownPos = Input.mousePosition;
            readyToThrow = true;
            dragging = true;

            if (selectArm == 2 || detachScript[select].detached)
                return;
            originPos = limbList[select].transform.position;
            limbList[select].transform.position = throwPoint.position;

        }
        else if (Input.GetMouseButtonUp(1))
        {
            readyToThrow = false;
            dragging = false;

            DrawTrajectory.instance.HideLine();

            if (selectArm == 2 || detachScript[select].detached)
                return;

            limbList[select].transform.position = originPos;
        }
        if (Input.GetMouseButtonUp(0) && readyToThrow && !detachScript[select].detached && DetachScript.numOfArms >= 1)
        {
            if (detachScript[select].partName == "Arm" && DetachScript.numOfArms < 2)
                return;

            if (!limbList[select].GetComponent<Rigidbody>())
                limbList[select].AddComponent<Rigidbody>();

            if (detachScript[select].partName == "Arm")
                DetachScript.numOfArms--;
            if (detachScript[select].partName == "Leg")
                DetachScript.numOfLegs--;

            detachScript[select].detached = true;
            DrawTrajectory.instance.HideLine();
            mouseReleasePos = Input.mousePosition;

            //ending point - starting point + cam movement
            dir = (Input.mousePosition - mousePressDownPos).normalized;
            Throw(force: (mouseReleasePos - mousePressDownPos) * dir.y + cam.transform.forward * throwForce + transform.up * throwUpwardForce);
        }
    }

    void Throw(Vector3 force)
    {

        readyToThrow = false;

        // GameObject throwObject = Instantiate(objectToThrow, throwPoint.position, cam.rotation);

        objectRb = limbList[select].GetComponent<Rigidbody>();
        Vector3 forceToAdd = new Vector3(force.x, force.y, z: force.y);

        objectRb.AddForce(forceToAdd);

        Invoke(nameof(ResetThrow), throwCD);
    }


    void ResetThrow()
    {
        readyToThrow = true;
    }
}
