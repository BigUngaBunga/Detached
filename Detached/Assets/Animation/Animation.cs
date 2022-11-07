using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Animation : MonoBehaviour
{
    public Animator animator;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        // Animation
        if (animator != null)
        {
            if (Input.GetKeyDown(KeyCode.W))
            {
                animator.SetTrigger("Walking");
            }
            else if (Input.GetKeyDown(KeyCode.S))
            {
                animator.SetTrigger("Walking");
            }
            else if (Input.GetKeyDown(KeyCode.A))
            {
                animator.SetTrigger("Walking");
            }
            else if (Input.GetKeyDown(KeyCode.D))
            {
                animator.SetTrigger("Walking");
            }
            else if (Input.GetKeyDown(KeyCode.Space))
            {
                animator.SetTrigger("Jump");
            }
        }

        if (Input.GetKeyDown("1"))
        {
            gameObject.GetComponent<Renderer>().material.color = Color.red;
        }
        else if (Input.GetKeyDown("2"))
        {
            gameObject.GetComponent<Renderer>().material.color = Color.blue;
        }
        else if (Input.GetKeyDown("3"))
        {
            gameObject.GetComponent<Renderer>().material.color = Color.yellow;
        }
    }
}
