using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveToTrigger : MonoBehaviour
{
    public string triggerName;
    public bool moved = false;

    public void Update()
    {
        if (triggerName == null)
        {
            return;
        }
        if (!moved)
            Move();
    }
    public void Move()
    {
        GameObject obj = GameObject.Find(triggerName);
        if (obj == null) { return; }
        float x = obj.transform.position.x;
        float y = obj.transform.position.y;
        float z = obj.transform.position.z;

        Debug.Log(" x, " + x + " y, " + y + " z, " + z);
        Debug.Log("name of trigger = " + triggerName);
        transform.position.Set(x, y + 1, z);
        moved = true;
    }
}
