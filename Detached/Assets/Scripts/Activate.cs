using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Activate : MonoBehaviour
{


    bool active;
    public Material baseM;
    public Material activeM;
    int triggerObjects;

    // Start is called before the first frame update
    void Start()
    {
        active = false;
        triggerObjects = 0;
    }

    // Update is called once per frame
    void Update()
    {


        if (active)
        {
            this.GetComponent<MeshRenderer>().material = activeM;
        }
        else if (!active)
        {
            this.GetComponent<MeshRenderer>().material = baseM;
        }


        if(triggerObjects > 0)
        {
            active = true;
        }
        else
        {
            active = false;
        }

    }

    void OnTriggerEnter(Collider other)
    {
        if (this.transform.gameObject.tag == "Lock" && other.GetComponent<Collider>().tag == "Key")
        {
            triggerObjects = 0;
            triggerObjects++;
        }
        else if (this.transform.gameObject.tag == "BatteryBox" && other.GetComponent<Collider>().tag == "Battery")
        {
            triggerObjects++;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (this.transform.gameObject.tag == "BatteryBox" && other.GetComponent<Collider>().tag == "Battery")
        {
            triggerObjects--;
        }
    }
}
