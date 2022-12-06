using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PingSystem : MonoBehaviour
{
    [Header("GameObject")]

    [SerializeField] GameObject pingPoint;
    int timer = 0;
    private Transform objectHit;
    private string nameObject;
    private bool hitObject;
    // Start is called before the first frame update
    void Start()
    {
        pingPoint.active = false;
    }

    // Update is called once per frame
    void Update()
    {
        timer++;
        if(timer >= 400)
        {
            pingPoint.active = false;
            timer = 0;
        }
        if (Input.GetMouseButtonDown(0)) //Replace jump with apropriate key
        {
            timer = 0;
            Ping();
        }

        RaycastHit hit;
        Ray ray = GetComponent<Camera>().ScreenPointToRay(Input.mousePosition);

        hitObject = false;
        if (Physics.Raycast(ray, out hit))
        {
            hitObject = true;
            objectHit = hit.transform;
            nameObject = hit.transform.gameObject.name;
        }

    }

    void Ping()
    {
        pingPoint.active = true;
        pingPoint.transform.position = objectHit.transform.position;

    }
}
