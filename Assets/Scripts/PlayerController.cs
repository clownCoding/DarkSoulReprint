using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerController : MonoBehaviour
{
    public GameObject model;
    public float walkSpeed = 2.8f;
    public float runMultiplier = 2.7f;
    public float jumpVelocity = 5.0f;
    public float rollVelocity = 3.0f;
    public float jabVelocity = 10.0f;

    [Space(10)]
    [Header("===== Friction Setting =====")]
    public PhysicMaterial frictionOne;
    public PhysicMaterial frictionZero;

    public IUserInput input;
    private Animator animator;
    private Rigidbody rigid;
    private Vector3 planarVec;
    private Vector3 thrustVec;
    private bool lockPlanar;
    private bool trackDirection;
    private bool canAttack;
    private CapsuleCollider col;
    private float targetWeight;
    private Vector3 deltaPos;
    private CameraController camcon;

    void Awake()
    {
        IUserInput[] inputs = GetComponents<IUserInput>();
        foreach(var input in inputs)
        {
            if(input.enabled == true)
            {
                this.input = input; 
            }
        }
        //input = GetComponent<PlayerInput>();
        animator = model.GetComponent<Animator>();
        rigid = GetComponent<Rigidbody>();
        col = GetComponent<CapsuleCollider>();
        camcon = GetComponentInChildren<CameraController>();
    }


    void Update()
    { 
        if(camcon.lockState == false)
        {
            animator.SetFloat("forward", input.Dmag * Mathf.Lerp(animator.GetFloat("forward"), ((input.run) ? 1.0f : 0.5f), 0.5f));
            animator.SetFloat("right", 0);
        }
        else
        {
            Vector3 localDirection = transform.InverseTransformVector(input.Direction);
            animator.SetFloat("forward", localDirection.z * ((input.run) ? 1.0f : 0.5f));
            animator.SetFloat("right", localDirection.x * ((input.run) ? 1.0f : 0.5f));
        }
        animator.SetBool("defense", input.defense);

        if (input.lockon)
        {
            camcon.ToggleLock();
        }

        if(input.roll || rigid.velocity.magnitude > 7f)
        {
            animator.SetTrigger("roll");
            canAttack = false;
        }

        if (input.jump)
        {
            animator.SetTrigger("jump");
            canAttack = false;
        }

        if (input.attack && CheackState("Ground") && canAttack)
        {
            animator.SetTrigger("attack");
        }

        if(camcon.lockState == false)
        {
            if (input.Dmag > 0.1f)
            {
                model.transform.forward = Vector3.Slerp(model.transform.forward, input.Direction, 0.3f);
            }

            if (!lockPlanar)
            {
                planarVec = input.Dmag * model.transform.forward * walkSpeed * ((input.run) ? runMultiplier : 1.0f);
            }
        }
        else
        {
            if(trackDirection == true)
            {
                model.transform.forward = planarVec;
            }
            else
            {
                model.transform.forward = transform.forward;
            }

            if (!lockPlanar)
            {
                planarVec = input.Direction * walkSpeed * ((input.run) ? runMultiplier : 1.0f);
            }
        }


    }

    private void FixedUpdate()
    {
        rigid.position += deltaPos;
        rigid.velocity = new Vector3(planarVec.x, rigid.velocity.y, planarVec.z) + thrustVec;
        thrustVec = Vector3.zero;
        deltaPos = Vector3.zero;
    }

    private bool CheackState(string stateName, string layerName = "Base Layer")
    {
        return animator.GetCurrentAnimatorStateInfo(animator.GetLayerIndex(layerName)).IsName(stateName);
    }
    

    /// 
    /// Message processing block
    /// 

    private void IsGround()
    {
        animator.SetBool("isGround", true);
    }

    private void IsNotGround()
    {
        animator.SetBool("isGround", false);
    }

    private void OnGroundEnter()
    {
        lockPlanar = false;
        input.inputEnabled = true;
        canAttack = true;

        col.material = frictionOne;
        trackDirection = false;
    }

    private void OnGroundExit()
    {
        col.material = frictionZero;
    }

    private void OnRollEnter()
    {
        lockPlanar = true;
        input.inputEnabled = false;
        thrustVec.y = rollVelocity;
        trackDirection = true;
    }

    private void OnJabEnter()
    {
        lockPlanar = true;
        input.inputEnabled = false;
    }

    private void OnJabStay()
    {
        thrustVec = animator.GetFloat("jabVelocity") * model.transform.forward;
    }

    private void OnJumpEnter()
    {
        lockPlanar = true;
        input.inputEnabled = false;
        thrustVec.y = jumpVelocity;
        trackDirection = true;
    }

    private void OnAttack1hAEnter()
    {
        targetWeight = 1.0f;
        //animator.SetLayerWeight(layerIndex, 1.0f);
        input.inputEnabled = false;
    }

    private void OnAttack1hAUpdate()
    {
        thrustVec = animator.GetFloat("attack1hAVelocity") * model.transform.forward;

        int layerIndex = animator.GetLayerIndex("attack");
        float currentWeight = animator.GetLayerWeight(layerIndex);
        currentWeight = Mathf.Lerp(currentWeight, targetWeight, 0.4f);
        animator.SetLayerWeight(layerIndex, currentWeight);
    }

    private void OnAttackIdleEnter()
    {
        //animator.SetLayerWeight(animator.GetLayerIndex("attack"), 0.0f);
        targetWeight = 0f;
        input.inputEnabled = true;
    }

    private void OnAttackIdleUpdate()
    {
        int layerIndex = animator.GetLayerIndex("attack");
        float currentWeight = animator.GetLayerWeight(layerIndex);
        currentWeight = Mathf.Lerp(currentWeight, targetWeight, 0.4f);
        animator.SetLayerWeight(layerIndex, currentWeight);
    }

    private void OnUpdateRM(object _deltaPos)
    {
        if (CheackState("attack1hC", "attack"))
        {
            deltaPos += (Vector3)_deltaPos;
        }
    }
}
