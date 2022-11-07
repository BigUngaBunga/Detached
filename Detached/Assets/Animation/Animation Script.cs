using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationScript : MonoBehaviour
{
    public Animator animator;
    public GameObject[] models;
    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        // Animation
        if(animator != null)
        {
            if(Input.GetKeyDown(KeyCode.W))
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
            models[0].SetActive(true);
        }
    }
}
