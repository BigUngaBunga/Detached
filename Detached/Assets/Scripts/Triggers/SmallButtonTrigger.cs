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
        if (activatingObject.CompareTag("Player"))
            return HasEnoughArms(activatingObject, requiredArms);
        return IsLimbOfType(activatingObject, ItemManager.Limb_enum.Arm);
    }
    
    public void Interact(GameObject activatingObject)
    {
        if (!IsTriggered && CanInteract(activatingObject))
            StartCoroutine(PushButton());
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
        if (IsLimbOfType(collision.gameObject, ItemManager.Limb_enum.Arm) && !IsTriggered)
            StartCoroutine(PushButton());
    }
}
