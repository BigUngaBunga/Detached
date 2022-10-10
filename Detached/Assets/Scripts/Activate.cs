using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Activate : MonoBehaviour
{


    bool active;
    public Material baseM;
    public Material activeM;

    // Start is called before the first frame update
    void Start()
    {
        active = false;
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
    }

    void OnTriggerEnter(Collider other)
    {
        if (this.transform.gameObject.tag == "Lock" && other.GetComponent<Collider>().tag == "Key")
        {
            active = true;
        }
        else if (this.transform.gameObject.tag == "BatteryBox" && other.GetComponent<Collider>().tag == "Battery")
        {
            active = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (this.transform.gameObject.tag == "Lock" && other.GetComponent<Collider>().tag == "Key")
        {
            active = false;
        }
        else if (this.transform.gameObject.tag == "BatteryBox" && other.GetComponent<Collider>().tag == "Battery")
        {
            active = false;
        }
    }
}
