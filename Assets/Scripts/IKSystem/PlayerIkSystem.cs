using System.Collections;
using UnityEngine;

public class PlayerIkSystem : MonoBehaviour
{
    private Animator animator;
    public Transform lookTarget;
    [Range(0f, 1f)] public float lookWeight = 1f;

    private Vector3 originalLookTargetPosition;  // Store original position
    public float crouchOffsetY = -0.5f;  // Offset for look target when crouching (adjust as needed)

    private void Start()
    {
        animator = GetComponent<Animator>();
        if (lookTarget != null)
        {
            originalLookTargetPosition = lookTarget.localPosition;  // Use local position for relative adjustment
        }
    }

    private void OnAnimatorIK(int layerIndex)
    {
        if (lookTarget != null)
        {
            ApplyLookIK(lookTarget);
        }
        else
        {
            ResetLookIK();
        }
    }

    private void ApplyLookIK(Transform target)
    {
        animator.SetLookAtWeight(lookWeight);
        animator.SetLookAtPosition(target.position);
    }

    private void ResetLookIK()
    {
        animator.SetLookAtWeight(0f);
    }

    public void AdjustLookTargetForCrouch(bool isCrouching)
    {
        if (lookTarget != null)
        {
            Vector3 adjustedPosition = originalLookTargetPosition;
            adjustedPosition.y += isCrouching ? crouchOffsetY : 0f;  // Apply vertical offset for crouching
            lookTarget.localPosition = adjustedPosition;
        }
    }
}
