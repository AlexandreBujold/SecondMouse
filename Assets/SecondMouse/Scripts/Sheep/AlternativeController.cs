using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlternativeController : MonoBehaviour
{
    Rigidbody m_rb;
    public float topSpeed = 5f;
    public float jumpForce = 10f;
    public float slowDownFactor = 1f;
    public float turnSpeed = 3f;
    public Vector3 forceDirection;
    public Vector3 currentVelocity;

    [Space]
    //Camera Movement & Player Rotation
    public Transform m_cameraPivot;
    public Transform m_cam;
    private float heading = 0;

    [Space]
    public string groundTag;
    [Range(0, 2)]
    public float raycastRange;


    void Start()
    {
        if (m_rb == null)
        {
            m_rb = GetComponentInChildren<Rigidbody>();
        }
    }


    void Update()
    {
        RotateCameraAndSheep();
        forceDirection = ReadInput();
        ApplyPhysics(forceDirection);
        AdjustSpeed(topSpeed);
    }

    private void AdjustSpeed(float topSpeed)
    {
        m_rb.velocity = Vector3.ClampMagnitude(m_rb.velocity, topSpeed);
        currentVelocity = m_rb.velocity;
    }

    private void ApplyPhysics(Vector3 _forceDirection)
    {
        if (forceDirection != Vector3.zero)
        {
            m_rb.AddForce(_forceDirection * jumpForce, ForceMode.Impulse);
            Debug.Log("Force applied: " + _forceDirection);
        }
        else
        {
            m_rb.velocity = Vector3.Lerp(m_rb.velocity, Vector3.zero, slowDownFactor * Time.deltaTime);
        }
    }

    private Vector3 ReadInput()
    {
        Vector3 finalInput = Vector3.zero;
        //Player Movement
        if (CheckGround())
        {
            Vector3 input = new Vector3(Input.GetAxisRaw("Horizontal"), 1, Input.GetAxisRaw("Vertical"));
            Vector3 camF = m_cam.forward;
            Vector3 camR = m_cam.right;
            camF.y = 0;
            camR.y = 0;
            camF = camF.normalized;
            camR = camR.normalized;
            finalInput = (camF * input.z + camR * input.x + input).normalized;
            Debug.Log("Final Input: " + finalInput);
        }
        return finalInput;
    }

    private void RotateCameraAndSheep()
    {
        heading += Input.GetAxis("Mouse X") * Time.deltaTime * 180;
        m_cameraPivot.rotation = Quaternion.Euler(0, heading, 0);

        //Rotate Sheep to face same direction as Camera
        transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, m_cameraPivot.transform.eulerAngles.y, m_cameraPivot.rotation.eulerAngles.z);
    }
    private bool CheckGround()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, -transform.up, out hit, raycastRange) && hit.transform.CompareTag(groundTag))
        {
            Debug.DrawRay(transform.position, -transform.up * hit.distance, Color.green);
            Debug.Log("Hit " + groundTag);
            return true;
        }
        else
        {
            Debug.DrawRay(transform.position, -transform.up * hit.distance, Color.red);
            Debug.Log("Did not hit " + groundTag);
            forceDirection = Vector3.zero;
            return false;
        }

    }
}
