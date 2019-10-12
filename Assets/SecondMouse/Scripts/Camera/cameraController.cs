using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cameraController : MonoBehaviour
{
    public Transform cameraMountPoint;
    public Transform lookAtTarget;
    public float cameraTurnSpeed = 0.1f;
    void LateUpdate()
    {
        
        transform.rotation = Quaternion.Lerp(transform.rotation, transform.LookAt(lookAt), Time.time * cameraTurnSpeed);
    }
}
