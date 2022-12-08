
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PingSystem : MonoBehaviour
{
    [Header("GameObject")]

    [SerializeField] GameObject pingPoint;
    GameObject resetGO;
    int timer = 0;
    private Transform objectHit;
    private string nameObject;
    private bool hitObject;
    RaycastHit hit;
    Ray ray;
    [SerializeField] Camera camera;
    Vector2 center = new Vector2(Screen.width / 2, Screen.height / 2);

    // Start is called before the first frame update
    void Start()
    {
        camera = GameObject.Find("Camera").GetComponent<Camera>();
        resetGO = pingPoint;
        pingPoint.active = false;
    }


    void PlaySoundOnTrigger()
    {
        SFXManager.PlayOneShot(SFXManager.PingSound, SFXManager.SFXVolume, pingPoint.transform.position);
    }
    // Update is called once per frame
    void Update()
    {
        timer++;
        if (timer >= 400)
        {
            ResetPing();
            timer = 0;
        }
        if (Input.GetMouseButtonDown(0) && !Input.GetMouseButtonDown(1)) //Replace jump with apropriate key
        {
            timer = 0;
            Ping();
        }

        ray = camera.ScreenPointToRay(center);

        hitObject = false;
        if (Physics.Raycast(ray, out hit))
        {
            hitObject = true;
            objectHit = hit.transform;
            nameObject = hit.transform.gameObject.name;
        }

    }

    void ResetPing()
    {
        pingPoint = resetGO;
        pingPoint.transform.position = this.transform.position;
        pingPoint.transform.parent = this.transform;
        pingPoint.active = false;
    }

    void Ping()
    {
        ResetPing();
        pingPoint.active = true;
        pingPoint.transform.position = new Vector3(objectHit.transform.position.x, objectHit.transform.position.y + 3f, objectHit.transform.position.z);
        //pingPoint.transform.rotation = Quaternion.identity;
        pingPoint.transform.parent = null;

        PlaySoundOnTrigger();
    }
}
