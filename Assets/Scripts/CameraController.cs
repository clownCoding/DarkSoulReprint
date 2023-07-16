using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CameraController : MonoBehaviour
{
    public IUserInput pi;

    public float horizontalSpeed = 100f;
    public float verticalSpeed = 80f;
    public Image lockDot;
    public bool lockState;

    private GameObject playerHandle;
    private GameObject cameraHandle;
    private GameObject mainCamera;
    private GameObject model;
    private LockTarget lockTarget;

    private float angle;
    private Vector3 cameraDampVelocity;

    private class LockTarget{
        public GameObject obj;
        public float halfHeight;
       
        public LockTarget(GameObject obj, float halfHeight) {
            this.obj = obj;
            this.halfHeight = halfHeight; 
        } 
    };

    void Awake()
    {
        lockDot.enabled = false;
        cameraHandle = transform.parent.gameObject;    
        playerHandle = transform.parent.parent.gameObject;
        angle = 20f;
        mainCamera = Camera.main.gameObject;
        model = playerHandle.GetComponent<PlayerController>().model;

        Cursor.lockState = CursorLockMode.Locked;
    }

    private void FixedUpdate()
    {
        if(lockTarget == null)
        {
            lockDot.enabled = false;
            lockState = false;
            Vector3 modelAngle = model.transform.eulerAngles;

            playerHandle.transform.Rotate(Vector3.up, pi.Jright * horizontalSpeed * Time.fixedDeltaTime);

            angle -= pi.Jup * -verticalSpeed * Time.fixedDeltaTime;
            angle = Mathf.Clamp(angle, -40f, 30f);
            cameraHandle.transform.localEulerAngles = new Vector3(angle, 0f, 0f);

            model.transform.eulerAngles = modelAngle;

        }
        else
        {
            lockDot.enabled = true;
            lockState = true;
            Vector3 lockForward = lockTarget.obj.transform.position - model.transform.position;
            lockForward.y = 0;
            playerHandle.transform.forward = lockForward;
            cameraHandle.transform.LookAt(lockTarget.obj.transform.position - new Vector3(0f, lockTarget.halfHeight / 2, 0f));
        }

        mainCamera.transform.position = Vector3.SmoothDamp(mainCamera.transform.position, transform.position,
            ref cameraDampVelocity, 0.1f);
        //mainCamera.transform.eulerAngles = transform.eulerAngles;
        mainCamera.transform.LookAt(cameraHandle.transform);    
    }

    private void Update()
    {
        if(lockTarget != null)
        {
            lockDot.transform.position = Camera.main.WorldToScreenPoint(lockTarget.obj.transform.position);
            if(Vector3.Distance(model.transform.position, lockTarget.obj.transform.position) > 10f)
            {
                lockTarget = null;
                lockState = false;
                lockDot.enabled = false;
            }
        }
    }

    public void ToggleLock()
    {
        Vector3 viewPos = model.transform.position + new Vector3(0f, 1f, 0f);
        Vector3 boxCenter = viewPos + model.transform.forward * 5.0f;
        Collider[] cols = Physics.OverlapBox(boxCenter, new Vector3(0.5f, 0.5f, 5.0f), model.transform.rotation, LayerMask.GetMask("Enemy"));
        if(cols.Length == 0 ) {
            lockTarget = null;
        }
        foreach(Collider col in cols)
        {
            if(lockTarget != null && lockTarget.obj == col.gameObject)
            {
                lockTarget = null;
                break;
            }
            lockTarget = new LockTarget(col.gameObject, col.bounds.extents.y);
            break;
        }
    }

}
