using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class DragShoot : MonoBehaviour
{
    // Start is called before the first frame update

    private Vector3 mousePressDownPos;
    private Vector3 mouseReleasePos;

    private Rigidbody rb;
    private bool isShoot;

    private float forceMulti = 3;
    bool dragging;

    void Start()
    {
        rb = gameObject.GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        LeftMouse();
        RightClickDrag();
    }

    void RightClickDrag()
    {
        if (dragging)
        {
            TrajectoryCal();
        }
    }

    private void TrajectoryCal()
    {
        Vector3 forceInit = Input.mousePosition - mousePressDownPos;
        Vector3 forceV = new Vector3(forceInit.x, forceInit.y, forceInit.y) * forceMulti;

       // if (!isShoot)
            //DrawTrajectory.instance.UpdateTrajectory(forceV, transform.position);
    }

    private void LeftMouse()
    {
        if (Input.GetMouseButtonDown(1))
        {

            mousePressDownPos = Input.mousePosition;
            dragging = true;

        }
        else if (Input.GetMouseButtonUp(0))
        {
            DrawTrajectory.instance.HideLine();
            mouseReleasePos = Input.mousePosition;

            Shoot(force: mouseReleasePos - mousePressDownPos);
        }

        if (Input.GetMouseButtonUp(1))
        {
            dragging = false;
            DrawTrajectory.instance.HideLine();
        }


    }

    //private void OnMouseDown()
    //{
    //    mousePressDownPos = Input.mousePosition;

    //}

    //private void OnMouseUp()
    //{
    //    DrawTrajectory.instance.HideLine();
    //    mouseReleasePos = Input.mousePosition;

    //    Shoot(force: mouseReleasePos - mousePressDownPos);
    //}


    void Shoot(Vector3 force)
    {
        if (isShoot)
            return;

        rb.AddForce(new Vector3(force.x, force.y, z: force.y) * forceMulti);
        isShoot = true;
    }
}
