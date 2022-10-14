using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Drag : MonoBehaviour
{
    public Transform dest;

    private Transform dropDest;
    bool draging = false;

    public Camera camera;

    private Transform objectHit;
    private string nameObject;
    private bool hitObject;
    private GameObject heldItem;


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
            if (objectHit.transform.gameObject.tag == "BigBox")
            {
                if (Input.GetKeyDown("e") && !draging && TooFarGone(objectHit) == false)
                {

                    heldItem = GameObject.Find(objectHit.transform.gameObject.name);
                    dest.position = heldItem.transform.position;

                }
            }
        }


        if (draging)
        {

            heldItem.transform.position = new Vector3(dest.position.x, heldItem.transform.position.y, dest.position.z);
            heldItem.GetComponent<Rigidbody>().velocity = new Vector3(0, 0, 0);
            heldItem.transform.eulerAngles = new Vector3(0, 0, 0);
            heldItem.GetComponent<Rigidbody>().angularVelocity = new Vector3(0, 0, 0);


        }
        if (Input.GetKeyDown("e"))
        {

            if (!draging && hitObject)
            {
                if (objectHit.transform.gameObject.tag == "BigBox" && TooFarGone(objectHit) == false)
                {
                    heldItem = GameObject.Find(objectHit.transform.gameObject.name);
                    draging = true;
                }
            }
            else if (draging)
            {
                draging = false;
                heldItem = null;
                dest.position = new Vector3(this.transform.position.x, this.transform.position.y + 1, this.transform.position.z + 1.5f);
            }
        }

    }

    bool TooFarGone(Transform oh)
    {
        int size = 5;

        if (Mathf.Abs(transform.position.x - oh.position.x) > size || Mathf.Abs(transform.position.y - oh.position.y) > size || Mathf.Abs(transform.position.z - oh.position.z) > size)
        {
            return true;
        }
        return false;
    }
}
