using Mirror;
using System.Collections;
using UnityEngine;

public class SmallButtonTrigger : Trigger, IInteractable
{
    [Header("Small button fields")]
    [SerializeField] GameObject smallButton;
    [SerializeField] float pushedHeight;
    [Min(0.1f)]
    [SerializeField] float triggeredSeconds;
    private int requiredArms = 1;
    private Vector3 heightDifference => new Vector3(0, pushedHeight, 0);


    public bool CanInteract(GameObject activatingObject)
    {
        if (useLimiter)
            return limiter.ContainsObject(activatingObject) && !IsTriggered && HasEnoughArms(activatingObject, requiredArms);
        return !IsTriggered && HasEnoughArms(activatingObject, requiredArms);

    }
    
    public void Interact(GameObject activatingObject)
    {
        if (!IsTriggered)
            StartCoroutine(PushButton());
    }

    protected override void PlaySoundOnTrigger()
    {
        OneShotVolume.PlayOneShot(AudioPaths.PushButtonSound, VolumeManager.GetSFXVolume(), transform.position);
    }

    protected override void PlaySoundOnStopTrigger()
    {
        OneShotVolume.PlayOneShot(AudioPaths.PushButtonSound, VolumeManager.GetSFXVolume(), transform.position);
    }



    public IEnumerator PushButton()
    {
        IsTriggered = true;
        smallButton.transform.position -= heightDifference;
        yield return new WaitForSeconds(triggeredSeconds);
        IsTriggered = false;
        smallButton.transform.position += heightDifference;
        yield return null;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (isServer && !collision.gameObject.CompareTag("Player") && CanInteract(collision.gameObject))
            StartCoroutine(PushButton());
    }
}
