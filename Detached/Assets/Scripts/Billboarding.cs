using TMPro;
using UnityEngine;

public class Billboarding : MonoBehaviour
{
    enum PromptStatus
    {
        Unread, Displayed, Read
    }

    [Header("Prompt Text")]
    [SerializeField] bool PreSpawn;
    [SerializeField] private string prompt;
    [SerializeField] private string promptMarker;
    [SerializeField] private float markerSizeFactor = 2f;
    [SerializeField] private GameObject floatingText;

    [Header("Text Movement")]
    [SerializeField] private float lerpSpeed;
    [SerializeField] private Vector3 amplitudes;
    [SerializeField] private Vector3 angularVelocities;
    private float initialTextSize;
    private Vector3 initialPosition;

    [Range(0f, 90f)]
    [SerializeField] private float shakeWidth;
    [SerializeField] private float shakeVelocity;

    private PromptStatus promptStatus;
    private new Camera camera;
    private TextMeshPro text;

    private void Awake()
    {
        CreateText();
        angularVelocities *= Random.Range(1f, 1.33f);
        SetTextStatus(PreSpawn ? PromptStatus.Displayed : PromptStatus.Unread);
    }

    void FixedUpdate()
    {
        if (text == null) return;
        if(camera == null)
            camera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();

        Vector3 targetPosition = initialPosition + Vector3.up * GetBobbingHeight();
        Quaternion targetRotation = Quaternion.LookRotation(camera.transform.forward);
        if (promptStatus == PromptStatus.Unread)
            targetRotation *= Quaternion.Euler(0, 0, Mathf.Sin(Time.timeSinceLevelLoad * shakeVelocity) * shakeWidth);
        LerpCameraTransform(targetPosition, targetRotation);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name == "ChedBody" || other.gameObject.name == "DetaBody")
            SetTextStatus(PromptStatus.Displayed);
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.name == "ChedBody" || other.gameObject.name == "DetaBody")
            SetTextStatus(PromptStatus.Read);
    }

    private void CreateText()
    {
        text = Instantiate(floatingText, transform.position, Quaternion.identity, transform).GetComponentInChildren<TextMeshPro>();
        text.SetText(prompt);
        int multiplier = (int)(2.5f + prompt.Length / 35f);
        text.gameObject.transform.position += Vector3.up * multiplier;
        initialTextSize = text.fontSize;
        initialPosition = text.gameObject.transform.position;
    }

    private void SetTextStatus(PromptStatus newStatus)
    {
        promptStatus = newStatus;
        switch (newStatus)
        {
            case PromptStatus.Unread:
                text.SetText(promptMarker);
                text.fontSize = initialTextSize * markerSizeFactor;
                text.color = Color.yellow;
                break;
            case PromptStatus.Displayed:
                text.SetText(prompt);
                text.fontSize = initialTextSize;
                text.color = Color.white;
                break;
            case PromptStatus.Read:
                text.SetText(promptMarker);
                text.fontSize = initialTextSize * markerSizeFactor;
                text.color = Color.green;
                break;
        }
    }

    private float GetBobbingHeight()
    {
        float amplitude, angularVelocity;
        switch (promptStatus)
        {
            case PromptStatus.Unread:
                amplitude = amplitudes.x; angularVelocity = angularVelocities.x;
                break;
            case PromptStatus.Displayed:
                amplitude = amplitudes.y; angularVelocity = angularVelocities.y;
                break;
            default:
                amplitude = amplitudes.z; angularVelocity = angularVelocities.z;
                break;
        }

        return Mathf.Sin(Time.timeSinceLevelLoad * Mathf.Deg2Rad * angularVelocity) * amplitude;
    }
    private void LerpCameraTransform(Vector3 targetPosition, Quaternion targetRotation)
    {
        text.transform.SetPositionAndRotation(
            Vector3.Lerp(text.transform.position, targetPosition, lerpSpeed), 
            Quaternion.Lerp(text.transform.rotation, targetRotation, lerpSpeed));
    }
}
