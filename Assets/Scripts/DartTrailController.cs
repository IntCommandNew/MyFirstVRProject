using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(XRGrabInteractable))]
public class DartTrailController : MonoBehaviour
{
    [SerializeField] private TrailRenderer dartTrail;
    [SerializeField, Min(0f)] private float stopSpeed = 0.15f;
    [SerializeField] private float minimumTrailDuration = 0.3f;

    private Rigidbody rb;
    private XRGrabInteractable grabInteractable;
    private bool hasBeenReleased;
    private float releaseTime;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        grabInteractable = GetComponent<XRGrabInteractable>();

        if (dartTrail == null)
            dartTrail = GetComponentInChildren<TrailRenderer>();

        if (dartTrail != null)
            dartTrail.emitting = false;
    }

    private void OnEnable()
    {
        grabInteractable.selectEntered.AddListener(OnGrabbed);
        grabInteractable.selectExited.AddListener(OnReleased);
    }

    private void OnDisable()
    {
        grabInteractable.selectEntered.RemoveListener(OnGrabbed);
        grabInteractable.selectExited.RemoveListener(OnReleased);
    }

    private void OnGrabbed(SelectEnterEventArgs args)
    {
        hasBeenReleased = false;

        if (dartTrail != null)
        {
            dartTrail.emitting = false;
            dartTrail.Clear();
        }
    }

    private void OnReleased(SelectExitEventArgs args)
    {
        hasBeenReleased = true;

        if (dartTrail == null)
        {
            return;
        }

        dartTrail.Clear();
        dartTrail.emitting = true;

        releaseTime = Time.time;
    }

    private void Update()
    {
        if (!hasBeenReleased || dartTrail == null)
            return;

        float speed = rb.linearVelocity.magnitude;
        float elapsed = Time.time - releaseTime;

        if (elapsed < minimumTrailDuration)
            return;

        if (speed <= stopSpeed)
        {
            dartTrail.emitting = false;
            hasBeenReleased = false;
        }
    }
}