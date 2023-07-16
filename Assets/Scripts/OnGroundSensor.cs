using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements.Experimental;

public class OnGroundSensor : MonoBehaviour
{
    public CapsuleCollider capCol;
    public float offset = 0.1f;

    private Vector3 point1;
    private Vector3 point2;
    private float radius;

    void Awake()
    {
        radius = capCol.radius - 0.05f;
    }

 
    void FixedUpdate()
    {
        point1 = transform.position + transform.up * (radius - offset);
        point2 = transform.position + transform.up * (capCol.height - radius + offset);

        Collider[] colliders = Physics.OverlapCapsule(point1, point2, radius, LayerMask.GetMask("Ground"));

        if(colliders.Length > 0)
        {
            //foreach(Collider collider in colliders)
            //{
            //    print("Collision: " + collider.name);
            //}
            SendMessageUpwards("IsGround");
        }else
        {
            SendMessageUpwards("IsNotGround");
        }

    }
}
