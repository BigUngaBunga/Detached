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

    private QuickRigMover[] rigMovers;

    private bool HasRightArm => detach.RightArm.activeSelf;
    private bool HasLeftArm => detach.LeftArm.activeSelf;
    private bool HasRightLeg => detach.RightLeg.activeSelf;
    private bool HasLeftLeg => detach.LeftLeg.activeSelf;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        detach = GetComponent<Detach>();
        networkAnimator = GetComponent<NetworkAnimator>();
        rigMovers = GetComponentsInChildren<QuickRigMover>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!isLocalPlayer || animator == null) return;
        // Animation
        EvaluateCrawl();


        if (animator.GetBool("isCarrying"))
        {
            if (CheckKeys(KeyCode.W, KeyCode.A, KeyCode.S, KeyCode.D))
            {
                animator.SetTrigger("CarryWalk");
                networkAnimator.SetTrigger("CarryWalk");
            }
            else if (Input.GetKeyDown(KeyCode.F))
            {
                animator.SetBool("isCarrying", false);
            }
            else
            {
                animator.SetTrigger("CarryIdle");
                networkAnimator.SetTrigger("CarryIdle");
            }
        }

        


        if (CheckKeys(KeyCode.W, KeyCode.A, KeyCode.S, KeyCode.D) && HasLeftLeg && HasRightLeg)
        {
            animator.SetTrigger("Walking");
            networkAnimator.SetTrigger("Walking");
        }
        else if (Input.GetKeyDown(KeyCode.Space))
        {
            animator.SetTrigger("Jump");
            networkAnimator.SetTrigger("Jump");
        }
        else if (Input.GetKeyDown(KeyCode.E))
        {
            if (animator.GetBool("isCarrying") )
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

    private void EvaluateCrawl()
    {
        if (!HasLeftLeg && !HasRightLeg)
        {
            SetCrawl(true);
            if (CheckKeys(KeyCode.W, KeyCode.A, KeyCode.S, KeyCode.D))
            {
                animator.SetTrigger("Crawling");
                networkAnimator.SetTrigger("Crawling");
            }
            //else
            //{
            //    animator.SetTrigger("Crawling Idle");
            //    networkAnimator.SetTrigger("Crawling Idle");
            //}
        }
        else if (!HasLeftLeg)
        {
            SetCrawl(false);
            if (CheckKeys(KeyCode.W, KeyCode.A, KeyCode.S, KeyCode.D))
            {
                animator.SetTrigger("Right Jump");
                networkAnimator.SetTrigger("Right Jump");

            }
        }
        else if (!HasRightLeg)
        {
            SetCrawl(false);
            if (CheckKeys(KeyCode.W, KeyCode.A, KeyCode.S, KeyCode.D))
            {
                animator.SetTrigger("Left Jump");
                networkAnimator.SetTrigger("Left Jump");
            }
        }
    }

    private bool CheckKeys(params KeyCode[] keyCodes)
    {
        for (int i = 0; i < keyCodes.Length; i++)
        {
            if (Input.GetKey(keyCodes[i]))
                return true;
        }
        return false;
    }

    private void SetCrawl(bool isCrawling)
    {
        for (int i = 0; i < rigMovers.Length; i++)
            rigMovers[i].SetPosition(isCrawling);
        animator.SetBool("No Leg", isCrawling);
    }
}
