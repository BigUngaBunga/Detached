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
    private CrawlManager crawlManager;

    private bool HasRightArm => detach.RightArm.activeSelf;
    private bool HasLeftArm => detach.LeftArm.activeSelf;
    private bool HasRightLeg => detach.RightLeg.activeSelf;
    private bool HasLeftLeg => detach.LeftLeg.activeSelf;

    void Start()
    {
        animator = GetComponent<Animator>();
        detach = GetComponent<Detach>();
        networkAnimator = GetComponent<NetworkAnimator>();
        crawlManager = GetComponentInChildren<CrawlManager>();
    }

    void Update()
    {
        if (!isLocalPlayer || animator == null) return;
        // Animation

        IsCarrying();

        if (IsCrawling() || IsHopping()) { }

        if (!IsWalking() && Input.GetKeyDown(KeyCode.Space))
        {
            animator.SetTrigger("Jump");
            networkAnimator.SetTrigger("Jump");
        }
        else if (Input.GetKeyDown(KeyCode.F))
        {
            animator.SetBool("isCarrying", false);
            animator.SetTrigger("Carry");
            networkAnimator.SetTrigger("Carry");
            animator.SetBool("isCarrying", true);
        }
        else if (!Input.anyKey)
        {
            animator.SetTrigger("Idle");
            networkAnimator.SetTrigger("Idle");
        }

        Emote();
    }

    private bool IsCarrying()
    {
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
            return true;
        }
        return false;
    }

    private bool IsCrawling()
    {
        if (!HasLeftLeg && !HasRightLeg)
        {
            SetCrawl(true);
            if (CheckKeys(KeyCode.W, KeyCode.A, KeyCode.S, KeyCode.D) && (HasRightArm || HasLeftArm))
            {
                animator.SetTrigger("Crawling");
                networkAnimator.SetTrigger("Crawling");
            }
            return true;
        }
        SetCrawl(false);
        return false;
    }

    private bool IsHopping()
    {
        if (CheckKeys(KeyCode.W, KeyCode.A, KeyCode.S, KeyCode.D))
        {
            if (!HasLeftLeg && HasRightLeg)
            {
                animator.SetTrigger("Right Jump");
                networkAnimator.SetTrigger("Right Jump");
                return true;
            }
            else if (!HasRightLeg && HasLeftLeg)
            {
                animator.SetTrigger("Left Jump");
                networkAnimator.SetTrigger("Left Jump");
                return true;
            }
        }
        return false;
    }

    private bool IsWalking()
    {
        if (CheckKeys(KeyCode.W, KeyCode.S, KeyCode.A, KeyCode.D) && HasLeftLeg && HasRightLeg)
        {
            animator.SetTrigger("Walking");
            networkAnimator.SetTrigger("Walking");
            return true;
        }
        return false;
    }

    private void Emote()
    {
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

    private bool CheckKeys(params KeyCode[] keyCodes)
    {
        for (int i = 0; i < keyCodes.Length; i++)
        {
            if (Input.GetKey(keyCodes[i]))
                return true;
        }
        return false;
    }

    [Command]
    private void SetCrawl(bool isCrawling)
    {
        if (animator.GetBool("No Leg") != isCrawling)
            RPCSetCrawl(isCrawling);
    }
    [ClientRpc]
    private void RPCSetCrawl(bool isCrawling)
    {
        crawlManager.SetCrawl(isCrawling);
        animator.SetBool("No Leg", isCrawling);
    }
}
