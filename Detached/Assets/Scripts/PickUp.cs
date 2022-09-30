using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUp : MonoBehaviour
{
    // Start is called before the first frame update
    public Transform dest;
    private Transform dropDest;
    public Material baseMaterial;
    public Material hoverMaterial;
    bool holding = false;

    private Transform objectHit;
    public Camera camera;
    private string nameObject;
    private bool hitObject;

   

    void OnMouseOver()
    {

        this.GetComponent<MeshRenderer>().material = hoverMaterial;
        if (Input.GetKeyDown("e"))
        {
            GetComponent<Rigidbody>().useGravity = false;
            this.transform.parent = GameObject.Find("PickUpDestination").transform;
            holding = true;
        }

        

    }

    void OnMouseExit()
    {
        this.GetComponent<MeshRenderer>().material = baseMaterial;
    }


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

        if (holding)
        {
            this.transform.position = dest.position;
            
            if (Input.GetKeyUp("e"))
            {

                if(this.transform.gameObject.name == "Battery")
                {
                    if (objectHit.transform.gameObject.tag == "BatteryBox")
                    {
                        dropDest = GameObject.Find(objectHit.transform.gameObject.name + "/SlotDestination").transform;
                        this.transform.parent = dropDest.transform;
                    }
                    else
                    {
                        this.transform.parent = null;
                        dropDest = this.transform;
                    }
                }
                else if (this.transform.gameObject.name == "Key")
                {
                    if (objectHit.transform.gameObject.tag == "Lock")
                    {
                        dropDest = GameObject.Find(objectHit.transform.gameObject.name + "/KeyDestination").transform;
                        this.transform.parent = dropDest.transform;
                    }
                    else
                    {
                        this.transform.parent = null;
                        dropDest = this.transform;
                    }
                }
                else if (this.transform.gameObject.tag == "Box")
                {
                    
                    this.transform.parent = null;
                    dropDest = this.transform;
                    
                }


                this.transform.position = dropDest.position;
                GetComponent<Rigidbody>().useGravity = true;
                

                holding = false;

            }

            GetComponent<Rigidbody>().velocity = new Vector3(0, 0, 0);
            this.transform.eulerAngles = new Vector3(90, 0, 0);
            GetComponent<Rigidbody>().angularVelocity = new Vector3(0, 0, 0);
        }

        





    }
}
