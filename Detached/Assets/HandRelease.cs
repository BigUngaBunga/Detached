using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class HandRelease : MonoBehaviour
{
    // Start is called before the first frame update

    public float targetTime = 0.0f;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        targetTime += Time.deltaTime;

        if (targetTime > 0.9f)
        {
            timerEnded();
        }
    }
    void timerEnded()
    {
        //do your stuff here.
        this.gameObject.SetActive(false);
    }
}
