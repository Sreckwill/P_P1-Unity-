using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerIkSystem : MonoBehaviour
{
    private Animator animator;
    public Transform lookTarget;
    [Range(0f, 1f)] public float lookWeight = 1f;

    private void Start()
    {
        animator = GetComponent<Animator>();
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

    public void SetLookTarget(Transform target)
    {
        lookTarget = target;
    }

    public void ClearLookTarget()
    {
        lookTarget = null;
    }
}
