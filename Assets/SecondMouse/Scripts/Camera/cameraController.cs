using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cameraController : MonoBehaviour
{
    [Header("Point of Interest Settings")]
    public int currentPOI = -1; //the index of -1 is saved for master tagret and mount for the start

    [Header("Camera Mount Settings")]
    public Transform masterCameraMount;
    public Transform currentCameraMount;
    public float cameraMoveSpeed = 1f;

    [Header("Look At Settings")]
    public Transform masterLookAtTarget;
    private Transform currentLookAtTarget;
    public float cameraTurnSpeed = 0.1f;

    [Header("POI Container Settings")]
    public pointOfInterest[] POIs;



    private void Start()
    {
        if (currentPOI == -1)
        {
            currentLookAtTarget = masterLookAtTarget;// camera looks at master target until it is overridden by another point of interest;
            currentCameraMount = masterCameraMount;// camera is mounted to master position until it is overridden by another point of interest;
        }
        else
        {
            UpdateCurrentPOI(currentPOI);
        }
    }

    public void UpdateCurrentPOI(int _index)
    {
        if (_index < POIs.Length)
        {
            if (POIs[_index].overrideCameraTarget)
            {
                currentLookAtTarget = POIs[_index].target;
            }
            if (POIs[_index].overrideCameraMount)
            {
                currentCameraMount = POIs[_index].mount;
            }

            if (POIs[_index].timedInterest)
            {
                Invoke("ResetTarget", POIs[_index].lookAtTimer);
            }
            if (POIs[_index].timedMount)
            {
                Invoke("ResetMount", POIs[_index].mountTimer);
            }
        }
    }
    public bool NextPOI()
    {
        if (currentPOI+1<POIs.Length)
        {
            currentPOI++;
            UpdateCurrentPOI(currentPOI);
            return true;
        }
        else
        {
            return false;
        }
    }
    public bool PreviousPOI()
    {
        if (currentPOI - 1>=0)
        {
            currentPOI--;
            UpdateCurrentPOI(currentPOI);
            return true;
        }
        else
        {
            return false;
        }
    }
    void LateUpdate()
    {
        UpdatePosition();
        UpdateRotation();
    }
    public void ResetMount()
    { 
        currentLookAtTarget = masterLookAtTarget; 
    }
    public void ResetTarget()
    {
        currentLookAtTarget = masterLookAtTarget;
    }
    private void UpdatePosition()
    {
        transform.position = Vector3.Lerp(transform.position, currentCameraMount.position, Time.deltaTime * cameraMoveSpeed);
    }
    private void UpdateRotation()
    {
        transform.rotation = Quaternion.Lerp(transform.rotation, UpdateLookAtDirection(), Time.time * cameraTurnSpeed);
    }
    private Quaternion UpdateLookAtDirection()
    {
        return Quaternion.Euler((currentLookAtTarget.position - transform.position).normalized);
    }
}
[System.Serializable]
public class pointOfInterest
{
    public string name;
    public bool overrideCameraTarget;//if enabled, camera target will lerp to override target
    public Transform target;
    public bool timedInterest; //if enabled, camera look at target is reset to master after lookAtTimer reaches 0
    public float lookAtTimer;
    public bool overrideCameraMount;//if enabled, camera position will lerp to override position
    public Transform mount;
    public bool timedMount;//if enabled, camera mount resets to master after mountTimer reaches 0
    public float mountTimer;
}
