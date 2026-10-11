using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

[RequireComponent(typeof(Rigidbody), typeof(XRGrabInteractable))]
public class DartFlight : MonoBehaviour
{
    [SerializeField]
    private Vector3 tipLocalDirection = Vector3.left;
    [SerializeField, Min(0f)]
    private float alignSpeed = 15f;
    [SerializeField, Min(0f)]
    private float minAlignSpeed = 1f;
    [SerializeField, Min(0f)]
    private float minStickSpeed = 2f;
    [SerializeField, Range(0f, 90f)]
    private float maxStickAngle = 70f;
    [SerializeField, Min(0f)]
    private float penetrationDepth = 0.015f;
    [SerializeField]
    private bool simulatorLaunch = true;
    [SerializeField, Min(0f)]
    private float simulatorLaunchSpeed = 7f;

    private Rigidbody body;
    private XRGrabInteractable grab;

    private float tipDistance;
    private bool inFlight;
    private bool releasePending;
    private Vector3 lastVelocity;
    private Quaternion lastRotation;
    private Collider dartCollider;
    private Collider stuckBoardCollider;
    private Vector3 startPosition;
    private Quaternion startRotation;
    private bool isStuck;

    private Vector3 TipDirection => transform.TransformDirection(tipLocalDirection).normalized;

    private void Awake()
    {
        body = GetComponent<Rigidbody>();
        grab = GetComponent<XRGrabInteractable>();
        dartCollider = GetComponentInChildren<Collider>();
        tipDistance = MeasureTipDistance();

        startPosition = transform.position;
        startRotation = transform.rotation;
    }

    public void ResetToStart()
    {
        if (grab.isSelected && grab.interactionManager != null)
            grab.interactionManager.CancelInteractableSelection((IXRSelectInteractable)grab);

        inFlight = false;
        releasePending = false;
        isStuck = false;

        if (stuckBoardCollider != null)
        {
            Physics.IgnoreCollision(dartCollider, stuckBoardCollider, false);
            stuckBoardCollider = null;
        }

        body.constraints = RigidbodyConstraints.None;
        body.linearVelocity = Vector3.zero;
        body.angularVelocity = Vector3.zero;

        body.position = startPosition;
        body.rotation = startRotation;
        transform.SetPositionAndRotation(startPosition, startRotation);
    }

    private void OnEnable()
    {
        grab.selectEntered.AddListener(OnGrabbed);
        grab.selectExited.AddListener(OnReleased);
    }

    private void OnDisable()
    {
        grab.selectEntered.RemoveListener(OnGrabbed);
        grab.selectExited.RemoveListener(OnReleased);
    }

    private void OnGrabbed(SelectEnterEventArgs args)
    {
        inFlight = false;
        releasePending = false;

        if (isStuck)
        {
            isStuck = false;
            body.constraints = RigidbodyConstraints.None;
        }
    }

    private void OnReleased(SelectExitEventArgs args)
    {
        releasePending = true;
    }

    private void FixedUpdate()
    {
        if (releasePending)
        {
            releasePending = false;
            inFlight = true;

            if (stuckBoardCollider != null)
            {
                Physics.IgnoreCollision(dartCollider, stuckBoardCollider, false);
                stuckBoardCollider = null;
            }

            if (simulatorLaunch && Application.isEditor)
            {
                body.linearVelocity = TipDirection * simulatorLaunchSpeed;
                body.angularVelocity = Vector3.zero;
            }
        }

        if (!inFlight || body.isKinematic)
            return;

        lastVelocity = body.linearVelocity;
        lastRotation = body.rotation;

        if (lastVelocity.magnitude < minAlignSpeed)
            return;

        Quaternion target = Quaternion.LookRotation(lastVelocity) * Quaternion.Inverse(Quaternion.LookRotation(tipLocalDirection));

        lastRotation = Quaternion.Slerp(body.rotation, target, alignSpeed * Time.fixedDeltaTime);
        body.MoveRotation(lastRotation);
        body.angularVelocity = Vector3.zero;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!inFlight)
            return;
        inFlight = false;

        DartBoard board = collision.collider.GetComponentInParent<DartBoard>();
        if (board == null)
            return;

        ContactPoint contact = collision.GetContact(0);
        if (lastVelocity.magnitude < minStickSpeed)
            return;
        if (Vector3.Angle(lastVelocity, -contact.normal) > maxStickAngle)
            return;

        StickInto(board, collision.collider, contact.point);
    }

    private void StickInto(DartBoard board, Collider boardCollider, Vector3 hitPoint)
    {
        isStuck = true;

        body.linearVelocity = Vector3.zero;
        body.angularVelocity = Vector3.zero;
        body.constraints = RigidbodyConstraints.FreezeAll;

        stuckBoardCollider = boardCollider;
        Physics.IgnoreCollision(dartCollider, stuckBoardCollider, true);

        Vector3 tipDirection = lastRotation * tipLocalDirection.normalized;
        Vector3 tipTarget = hitPoint + tipDirection * penetrationDepth;
        Vector3 tipOffset = lastRotation * Vector3.Scale(tipLocalDirection.normalized * tipDistance, transform.lossyScale);
        transform.SetPositionAndRotation(tipTarget - tipOffset, lastRotation);

        board.RegisterHit(this, hitPoint);
    }

    private float MeasureTipDistance()
    {
        MeshFilter meshFilter = GetComponentInChildren<MeshFilter>();
        if (meshFilter == null || meshFilter.sharedMesh == null)
            return 0f;

        Bounds bounds = meshFilter.sharedMesh.bounds;
        Vector3 direction = tipLocalDirection.normalized;
        float furthest = 0f;

        for (int i = 0; i < 8; i++)
        {
            Vector3 corner = bounds.center + Vector3.Scale(bounds.extents, new Vector3((i & 1) == 0 ? -1 : 1, (i & 2) == 0 ? -1 : 1, (i & 4) == 0 ? -1 : 1));
            Vector3 local = transform.InverseTransformPoint(meshFilter.transform.TransformPoint(corner));
            furthest = Mathf.Max(furthest, Vector3.Dot(local, direction));
        }

        return furthest;
    }
}
