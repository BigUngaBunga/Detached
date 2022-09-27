using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUp : MonoBehaviour
{
    // Start is called before the first frame update
    public Transform dest;
    public Material material1;
    public Material material2;
    bool holding = false;
    public GameObject Object;

    

    void OnMouseOver()
    {
        if (Input.GetMouseButtonDown(0))
        {
            holding = true;
        }

        

    }

    public void OnMouseUp()
    {
            this.transform.parent = null;
            GetComponent<Rigidbody>().useGravity = true;
        holding = false;
          
    }

    


    private void Update()
    {
        Cursor.visible = true;

        if (holding)
        {
            GetComponent<Rigidbody>().useGravity = false;
            this.transform.position = dest.position;
            this.transform.parent = GameObject.Find("PickUpDestination").transform;
        }


    }
}
