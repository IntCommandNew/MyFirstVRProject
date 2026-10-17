using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Animator))]
public class HandAnimator : MonoBehaviour
{
    [SerializeField]
    private InputActionProperty gripAction;

    [SerializeField]
    private InputActionProperty triggerAction;

    [SerializeField]
    private string gripParameter = "Grip";
    [SerializeField]
    private string triggerParameter = "Trigger";

    [SerializeField, Min(0.1f)]
    private float followSpeed = 20f;

    private Animator animator;
    private int gripId;
    private int triggerId;
    private float grip;
    private float trigger;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        gripId = Animator.StringToHash(gripParameter);
        triggerId = Animator.StringToHash(triggerParameter);
    }

    private void OnEnable()
    {
        gripAction.action?.Enable();
        triggerAction.action?.Enable();
    }

    private void Update()
    {
        float targetGrip = gripAction.action?.ReadValue<float>() ?? 0f;
        float targetTrigger = triggerAction.action?.ReadValue<float>() ?? 0f;

        float t = 1f - Mathf.Exp(-followSpeed * Time.deltaTime);
        grip = Mathf.Lerp(grip, targetGrip, t);
        trigger = Mathf.Lerp(trigger, targetTrigger, t);

        animator.SetFloat(gripId, grip);
        animator.SetFloat(triggerId, trigger);
    }
}