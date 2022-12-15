using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasketBall : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject goalDetecter;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.name == "Test") {
            Debug.Log("Play sound");
            goalDetecter.SetActive(false);
        }
    }
}
