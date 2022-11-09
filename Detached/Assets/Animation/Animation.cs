using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Animation : MonoBehaviour
{
    public Animator animator;
    bool isCarrying = false;
    bool moving = true;
    public Detach detach;
    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        detach = GetComponent<Detach>();
    }

    // Update is called once per frame
    void Update()
    {
        // Animation
        if (animator != null)
        {
            if(animator.GetBool("isCarrying") == true)
            {
                if (Input.GetKeyDown(KeyCode.W))
                {
                    animator.SetTrigger("CarryWalk");
                }
                else if (Input.GetKeyDown(KeyCode.S))
                {
                    animator.SetTrigger("CarryWalk");
                }
                else if (Input.GetKeyDown(KeyCode.A))
                {
                    animator.SetTrigger("CarryWalk");
                }
                else if (Input.GetKeyDown(KeyCode.D))
                {
                    animator.SetTrigger("CarryWalk");
                }
                else if (Input.GetKeyDown(KeyCode.F))
                {
                    if (animator.GetBool("isCarrying") == true)
                    {
                        animator.SetBool("isCarrying", false);
                    }
                }
                else
                {
                    animator.SetTrigger("CarryIdle");
                }
            }

            if (detach.LeftLeg.active == false && detach.RightLeg.active == false)
            {
                animator.SetBool("No Leg", true);
                if (Input.GetKeyDown(KeyCode.W) && detach.LeftArm.active == false && detach.RightArm.active == false)
                {
                    animator.SetTrigger("Crawling");
                }
                else if (Input.GetKeyDown(KeyCode.S))
                {
                    animator.SetTrigger("Crawling");
                }
                else if (Input.GetKeyDown(KeyCode.A))
                {
                    animator.SetTrigger("Crawling");
                }
                else if (Input.GetKeyDown(KeyCode.D))
                {
                    animator.SetTrigger("Crawling");
                }
            }
            else if (detach.LeftLeg.active == false)
            {
                if (Input.GetKeyDown(KeyCode.W))
                {
                    animator.SetTrigger("Right Jump");
                }
                else if (Input.GetKeyDown(KeyCode.S))
                {
                    animator.SetTrigger("Right Jump");
                }
                else if (Input.GetKeyDown(KeyCode.A))
                {
                    animator.SetTrigger("Right Jump");
                }
                else if (Input.GetKeyDown(KeyCode.D))
                {
                    animator.SetTrigger("Right Jump");
                }
            }
            else if(detach.RightLeg.active == false)
            {
                if (Input.GetKeyDown(KeyCode.W))
                {
                    animator.SetTrigger("Left Jump");
                }
                else if (Input.GetKeyDown(KeyCode.S))
                {
                    animator.SetTrigger("Left Jump");
                }
                else if (Input.GetKeyDown(KeyCode.A))
                {
                    animator.SetTrigger("Left Jump");
                }
                else if (Input.GetKeyDown(KeyCode.D))
                {
                    animator.SetTrigger("Left Jump");
                }
            }


            if (Input.GetKey(KeyCode.W) && detach.LeftLeg.active == true && detach.RightLeg.active == true)
            {
                animator.SetTrigger("Walking");
            }
            else if (Input.GetKeyDown(KeyCode.S) && detach.LeftLeg.active == true && detach.RightLeg.active == true)
            {
                animator.SetTrigger("Walking");
            }
            else if (Input.GetKeyDown(KeyCode.A) && detach.LeftLeg.active == true && detach.RightLeg.active == true)
            {
                animator.SetTrigger("Walking");
            }
            else if (Input.GetKeyDown(KeyCode.D) && detach.LeftLeg.active == true && detach.RightLeg.active == true)
            {
                animator.SetTrigger("Walking");
            }
            else if (Input.GetKeyDown(KeyCode.Space))
            {
                animator.SetTrigger("Jump");
            }
            else if (Input.GetKeyDown(KeyCode.E))
            {
                if(animator.GetBool("isCarrying") == true)
                {
                    animator.SetBool("isCarrying", false);
                }
                animator.SetTrigger("Carry");
                animator.SetBool("isCarrying", true);
            }
            else if (!Input.anyKey)
            {
                animator.SetTrigger("Idle");

            }

            if (Input.GetKeyDown(KeyCode.Z))
            {
                animator.SetTrigger("Facepalm");
            }
            else if (Input.GetKeyDown(KeyCode.X))
            {
                animator.SetTrigger("Wave");
            }
            else if (Input.GetKeyDown(KeyCode.C))
            {
                animator.SetTrigger("Mock");
            }
            else if (Input.GetKeyDown(KeyCode.V))
            {
                animator.SetTrigger("Laugh");
            }
        }

    }
}
