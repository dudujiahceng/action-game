using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
using System.Net.Sockets;

public class ViewerController : MonoBehaviour {
    //Some parameters of camera
    private float freeDistance = 6f;
    private float maxElevation = 80;
    private float maxDepression = 80;
    private bool canControlDistance = true;
    private bool canControlDirection = true;
    //Rotation
    private float rotateSpeed = 80;
    private float targetRotateLerp = 0.3f;
    private float eulerX;

    private GameObject target;             //The target is which camera rotate around
    private GameObject cameraPivot;        //The cameraPivot is which camera look at
    private Transform carmerInitPosition;  //The initial position of camera  
    private Vector3 offset;               // Offset between cameraPivot and camera
    public GameObject enemyTarget;

    void InitCamera()
    {
        target = GameObject.FindGameObjectWithTag("Player");
        Transform[] childs = target.GetComponentsInChildren<Transform>();
        foreach(Transform child in childs)
        {
            if (child.name == "CameraPivot")
                cameraPivot = child.gameObject;
            if (child.name == "CameraInitPosition")
                carmerInitPosition = child;
        }
    }

    void FreeCamera()
    {
        offset = offset.normalized * freeDistance;
        transform.position = cameraPivot.transform.position + offset;
        //Control hero's rotate by press WASD       
        if(canControlDirection && !target.GetComponent<Player>().GetLiftStatus)
        {
            Quaternion targetCurrentRotation = target.transform.rotation;
            if(Input.GetKey(KeyCode.A))
            {
                //left_forward_A_W
                if(Input.GetKey(KeyCode.W))
                {
                    target.transform.rotation = Quaternion.Slerp(targetCurrentRotation, Quaternion.Euler(new Vector3(target.transform.localEulerAngles.x, transform.localEulerAngles.y - 45, target.transform.localEulerAngles.z)), targetRotateLerp);
                }
                //left_backward_A_S
                else if(Input.GetKey(KeyCode.S))
                {
                    target.transform.rotation = Quaternion.Slerp(targetCurrentRotation, Quaternion.Euler(new Vector3(target.transform.localEulerAngles.x, transform.localEulerAngles.y - 135, target.transform.localEulerAngles.z)), targetRotateLerp);
                }
                //left_A
                else if(!Input.GetKey(KeyCode.W)&&!Input.GetKey(KeyCode.S))
                {
                    target.transform.rotation = Quaternion.Slerp(targetCurrentRotation, Quaternion.Euler(new Vector3(target.transform.localEulerAngles.x, transform.localEulerAngles.y - 90, target.transform.localEulerAngles.z)), targetRotateLerp);
                }
            }
            else if(Input.GetKey(KeyCode.D))
            {
                //right_forward_D_W
                if (Input.GetKey(KeyCode.W))
                {
                    target.transform.rotation = Quaternion.Slerp(targetCurrentRotation, Quaternion.Euler(new Vector3(target.transform.localEulerAngles.x, transform.localEulerAngles.y + 45, target.transform.localEulerAngles.z)), targetRotateLerp);
                }
                //right_backward_D_S
                else if (Input.GetKey(KeyCode.S))
                {
                    target.transform.rotation = Quaternion.Slerp(targetCurrentRotation, Quaternion.Euler(new Vector3(target.transform.localEulerAngles.x, transform.localEulerAngles.y + 135, target.transform.localEulerAngles.z)), targetRotateLerp);
                }
                //right_D
                else if (!Input.GetKey(KeyCode.W) && !Input.GetKey(KeyCode.S))
                {
                    target.transform.rotation = Quaternion.Slerp(targetCurrentRotation, Quaternion.Euler(new Vector3(target.transform.localEulerAngles.x, transform.localEulerAngles.y + 90, target.transform.localEulerAngles.z)), targetRotateLerp);
                }
            }
            //forward_W
            else if(Input.GetKey(KeyCode.W))
            {
                target.transform.rotation = Quaternion.Slerp(targetCurrentRotation, Quaternion.Euler(new Vector3(target.transform.localEulerAngles.x, transform.localEulerAngles.y, target.transform.localEulerAngles.z)), targetRotateLerp);
            }
            //backward_S
            else if (Input.GetKey(KeyCode.S))
            {
                target.transform.rotation = Quaternion.Slerp(targetCurrentRotation, Quaternion.Euler(new Vector3(target.transform.localEulerAngles.x, transform.localEulerAngles.y - 180, target.transform.localEulerAngles.z)), targetRotateLerp);
            }
        }
        
        //Left or right rotate around target by press J and L
        if(Input.GetKey(KeyCode.J) || Input.GetKey(KeyCode.L))
        {
            if(Input.GetKey(KeyCode.J))
            {
                transform.RotateAround(cameraPivot.transform.position, cameraPivot.transform.up, -rotateSpeed * Time.deltaTime);
            }
            else
                transform.RotateAround(cameraPivot.transform.position, cameraPivot.transform.up, rotateSpeed * Time.deltaTime);
        }
        //Up or down rotate around target by press I and K
        eulerX = transform.localEulerAngles.x;
        if (Input.GetKey(KeyCode.I) || Input.GetKey(KeyCode.K))
        {
            //Limit rotation angles of up and down
            if(eulerX >= 30 && eulerX < 45)
            {
                if (Input.GetKey(KeyCode.I))
                {
                    transform.RotateAround(cameraPivot.transform.position, transform.right, -rotateSpeed * Time.deltaTime);
                }
            }
            else if(eulerX < 345 && eulerX > 330)
            {
                if (Input.GetKey(KeyCode.K))
                {
                    transform.RotateAround(cameraPivot.transform.position, transform.right, rotateSpeed * Time.deltaTime);
                }
            }
            else
            {
                if (Input.GetKey(KeyCode.I))
                {
                    transform.RotateAround(cameraPivot.transform.position, transform.right, -rotateSpeed * Time.deltaTime);
                }
                else
                {
                    transform.RotateAround(cameraPivot.transform.position, transform.right, rotateSpeed * Time.deltaTime);
                }
            }
        }
        //offset value needs update!!!
        //offset value needs update!!!
        //offset value needs update!!!
        offset = transform.position - cameraPivot.transform.position;
    }

    void LockTarget()
    {
        if (!enemyTarget)
            return;
        if(Input.GetKeyDown(KeyCode.F))
        {
            if(enemyTarget)
            {
                enemyTarget = null;
            }
            else
            {
                
            }
        }
    }

    void Awake()
    {
        InitCamera();
        transform.position  = carmerInitPosition.position;            //Put camera at its initial position
        offset              = transform.position - cameraPivot.transform.position;
        enemyTarget         = null;
    }

    void FixedUpdate()
    {
        FreeCamera();
    }
}
