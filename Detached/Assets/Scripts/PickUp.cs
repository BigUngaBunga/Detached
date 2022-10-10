using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUp : MonoBehaviour
{
    public Transform dest;

    private Transform dropDest;
    bool holding = false;

    public Camera camera;

    private Transform objectHit;
    private string nameObject;
    private bool hitObject;
    private GameObject heldItem;

    //TODO dela upp i mindre metoder
    private void Update()
    {
        Cursor.visible = true;


        RaycastHit hit;
        Ray ray = camera.ScreenPointToRay(Input.mousePosition);

        hitObject = false;
        if (Physics.Raycast(ray, out hit))
        {
            hitObject = true;
            objectHit = hit.transform;
            nameObject = hit.transform.gameObject.name;
        }


        if (hitObject)
        {
            if (objectHit.transform.gameObject.tag == "Box" || objectHit.transform.gameObject.tag == "Battery" || objectHit.transform.gameObject.tag == "Key")
            {
                if (Input.GetKeyDown("e") && !holding)
                {
                    heldItem = GameObject.Find(objectHit.transform.gameObject.name);
                    heldItem.transform.parent = dest.transform;
                    heldItem.GetComponent<Rigidbody>().useGravity = false;
                }
            }


        }


        if (holding)
        {
            heldItem.transform.position = dest.position;
            heldItem.GetComponent<Rigidbody>().velocity = new Vector3(0, 0, 0);
            heldItem.transform.eulerAngles = new Vector3(0, 0, 0);
            heldItem.GetComponent<Rigidbody>().angularVelocity = new Vector3(0, 0, 0);


        }
        if (Input.GetKeyDown("e"))
        {

            if (!holding && hitObject)
            {
                if (objectHit.transform.gameObject.tag == "Box" || objectHit.transform.gameObject.tag == "Battery" || objectHit.transform.gameObject.tag == "Key")
                {
                    heldItem = GameObject.Find(objectHit.transform.gameObject.name);
                    heldItem.transform.parent = dest.transform;
                    heldItem.GetComponent<Rigidbody>().useGravity = false;
                    holding = true;
                }
            }
            else if (holding)
            {

                if (TooFarGone(objectHit) == false)
                {
                    if (heldItem.transform.gameObject.tag == "Battery")
                {
                    if (objectHit.transform.gameObject.tag == "BatteryBox")
                    {
                        dropDest = GameObject.Find(objectHit.transform.gameObject.name + "/SlotDestination").transform;

                    }
                    else
                    {
                        dropDest = dest.transform;
                    }
                }
                else if (heldItem.transform.gameObject.tag == "Key")
                {
                    if (objectHit.transform.gameObject.tag == "Lock")
                    {
                        dropDest = GameObject.Find(objectHit.transform.gameObject.name + "/KeyDestination").transform;
                    }
                    else
                    {
                        dropDest = dest.transform;
                    }
                }
            }

            if (heldItem.transform.gameObject.tag == "Box")
                {
                    dropDest = dest.transform;

                }

                heldItem.transform.position = dropDest.position;
                heldItem.transform.parent = null;
                heldItem.GetComponent<Rigidbody>().useGravity = true;
                holding = false;
                heldItem = null;
            }
        }

    }

    bool TooFarGone(Transform oh)
    {
        int size = 10;
        return Mathf.Abs(transform.position.x - oh.position.x) > size
            || Mathf.Abs(transform.position.y - oh.position.y) > size
            || Mathf.Abs(transform.position.z - oh.position.z) > size;
    }
}
