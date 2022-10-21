using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unlock : MonoBehaviour
{



    [SerializeField] private Material baseM;
    [SerializeField] private Material activeM;
    [SerializeField] private GameObject activator;
    //Activator a;


    // Start is called before the first frame update
    void Start()
    {
        this.GetComponent<MeshRenderer>().material = baseM;
        //a = activator.GetComponent<Activator>();
        //a.locked = true;



    }



    void OnTriggerEnter(Collider other)
    {
        if (this.transform.gameObject.tag == "Lock" && other.GetComponent<Collider>().tag == "Key")
        {


            //a.locked = false;
            //a.ReevaluateActivation();
            Destroy(other.gameObject);
            Destroy(gameObject);
        }

    }


}
