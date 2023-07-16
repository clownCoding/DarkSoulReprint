using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeftArmAnimFixed : MonoBehaviour
{
    public Vector3 rotation;

    private Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    private void OnAnimatorIK()
    {
        if (animator.GetBool("defense") == false)
        {
            Transform armTransform = animator.GetBoneTransform(HumanBodyBones.LeftLowerArm);
            armTransform.localEulerAngles += rotation;
            animator.SetBoneLocalRotation(HumanBodyBones.LeftLowerArm, Quaternion.Euler(armTransform.localEulerAngles));
        }
    }
}
