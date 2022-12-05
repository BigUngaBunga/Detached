using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Animation : NetworkBehaviour
{
    public Animator animator;
    private NetworkAnimator networkAnimator;
    bool isCarrying = false;
    bool moving = true;
    public Detach detach;
    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        detach = GetComponent<Detach>();
        networkAnimator = GetComponent<NetworkAnimator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!isLocalPlayer) return;
        // Animation
        if (animator != null)
        {
            if(animator.GetBool("isCarrying") == true)
            {
                if (Input.GetKey(KeyCode.W))
                {
                    animator.SetTrigger("CarryWalk");
                    networkAnimator.SetTrigger("CarryWalk");
                }
                else if (Input.GetKey(KeyCode.S))
                {
                    animator.SetTrigger("CarryWalk");
                    networkAnimator.SetTrigger("CarryWalk");
                }
                else if (Input.GetKey(KeyCode.A))
                {
                    animator.SetTrigger("CarryWalk");
                    networkAnimator.SetTrigger("CarryWalk");
                }
                else if (Input.GetKey(KeyCode.D))
                {
                    animator.SetTrigger("CarryWalk");
                    networkAnimator.SetTrigger("CarryWalk");
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
                    networkAnimator.SetTrigger("CarryIdle");
                }
            }

            if (detach.LeftLeg.active == false && detach.RightLeg.active == false)
            {
                animator.SetBool("No Leg", true);
                if (Input.GetKey(KeyCode.W) && detach.LeftArm.active == false && detach.RightArm.active == false)
                {
                    animator.SetTrigger("Crawling");
                    networkAnimator.SetTrigger("Crawling");
                }
                else if (Input.GetKey(KeyCode.S))
                {
                    animator.SetTrigger("Crawling");
                    networkAnimator.SetTrigger("Crawling");
                }
                else if (Input.GetKey(KeyCode.A))
                {
                    animator.SetTrigger("Crawling");
                    networkAnimator.SetTrigger("Crawling");
                }
                else if (Input.GetKey(KeyCode.D))
                {
                    animator.SetTrigger("Crawling");
                    networkAnimator.SetTrigger("Crawling");
                }
            }
            else if (detach.LeftLeg.active == false)
            {
                if (Input.GetKey(KeyCode.W))
                {
                    animator.SetTrigger("Right Jump");
                    networkAnimator.SetTrigger("Right Jump");

                }
                else if (Input.GetKey(KeyCode.S))
                {
                    animator.SetTrigger("Right Jump");
                    networkAnimator.SetTrigger("Right Jump");
                }
                else if (Input.GetKey(KeyCode.A))
                {
                    animator.SetTrigger("Right Jump");
                    networkAnimator.SetTrigger("Right Jump");
                }
                else if (Input.GetKey(KeyCode.D))
                {
                    animator.SetTrigger("Right Jump");
                    networkAnimator.SetTrigger("Right Jump");
                }
            }
            else if(detach.RightLeg.active == false)
            {
                if (Input.GetKey(KeyCode.W))
                {
                    animator.SetTrigger("Left Jump");
                    networkAnimator.SetTrigger("Left Jump");
                }
                else if (Input.GetKey(KeyCode.S))
                {
                    animator.SetTrigger("Left Jump");
                    networkAnimator.SetTrigger("Left Jump");
                }
                else if (Input.GetKey(KeyCode.A))
                {
                    animator.SetTrigger("Left Jump");
                    networkAnimator.SetTrigger("Left Jump");
                }
                else if (Input.GetKey(KeyCode.D))
                {
                    animator.SetTrigger("Left Jump");
                    networkAnimator.SetTrigger("Left Jump");
                }
            }
            else
            {
                animator.SetBool("No Leg", false);
            }

            if (Input.GetKey(KeyCode.W) && detach.LeftLeg.active == true && detach.RightLeg.active == true)
            {
                animator.SetTrigger("Walking");
                networkAnimator.SetTrigger("Walking");
            }
            else if (Input.GetKey(KeyCode.S) && detach.LeftLeg.active == true && detach.RightLeg.active == true)
            {
                animator.SetTrigger("Walking");
                networkAnimator.SetTrigger("Walking");
            }
            else if (Input.GetKey(KeyCode.A) && detach.LeftLeg.active == true && detach.RightLeg.active == true)
            {
                animator.SetTrigger("Walking");
                networkAnimator.SetTrigger("Walking");
            }
            else if (Input.GetKey(KeyCode.D) && detach.LeftLeg.active == true && detach.RightLeg.active == true)
            {
                animator.SetTrigger("Walking");
                networkAnimator.SetTrigger("Walking");
            }
            else if (Input.GetKeyDown(KeyCode.Space))
            {
                animator.SetTrigger("Jump");
                networkAnimator.SetTrigger("Jump");
            }
            else if (Input.GetKeyDown(KeyCode.F))
            {
                if(animator.GetBool("isCarrying") == true)
                {
                    animator.SetBool("isCarrying", false);
                }
                animator.SetTrigger("Carry");
                networkAnimator.SetTrigger("Carry");
                animator.SetBool("isCarrying", true);               
            }
            else if (!Input.anyKey)
            {
                animator.SetTrigger("Idle");
                networkAnimator.SetTrigger("Idle");
            }

            if (Input.GetKeyDown(KeyCode.Z))
            {

                animator.SetTrigger("Facepalm");
                networkAnimator.SetTrigger("Facepalm");
            }
            else if (Input.GetKeyDown(KeyCode.X))
            {
                animator.SetTrigger("Wave");
                networkAnimator.SetTrigger("Wave");
            }
            else if (Input.GetKeyDown(KeyCode.C))
            {
                animator.SetTrigger("Mock");

                networkAnimator.SetTrigger("Mock");
            }
            else if (Input.GetKeyDown(KeyCode.V))
            {
                networkAnimator.SetTrigger("Laugh");

                animator.SetTrigger("Laugh");
            }
        }

    }
}
