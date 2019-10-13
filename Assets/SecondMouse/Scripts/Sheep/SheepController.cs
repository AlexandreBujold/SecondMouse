using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class SheepController : MonoBehaviour
{

    Rigidbody m_rb;
    [Header("Movement Options")]
    public float constantSpeed = 5f;
    public float maxBounceVelocity = 10f;
    public float verticalForceOnBounce = 20f;
    [Range(0, 180)]
    public float mouseHorizontalSensitivity = 180;
    [Range(0, 180)]
    public float mouseVerticalSensitivity = 180;
    //public float slowDownRate = 1f;
    [Space]
    [Header("Calculated Velocity")]
    public Vector3 currentVelocity;

    private Vector2 _initialDisplacement;

    [Space]
    [Header("References")]
    //Camera Movement & Player Rotation
    public Transform m_cameraPivot;
    public Transform m_cam;
    private float heading = 0;
    private float originalBounceVelocity;

    //Speed up Factors


    // Start is called before the first frame update
    void Start()
    {
        if (m_rb == null)
        {
            m_rb = GetComponentInChildren<Rigidbody>();
        }
        originalBounceVelocity = maxBounceVelocity;

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        //Rotate Sheep and therefore the Camera
        heading += Input.GetAxis("Mouse X") * Time.deltaTime * mouseHorizontalSensitivity;
        Quaternion newRotation = Quaternion.Euler(0, heading, 0);
        newRotation = new Quaternion(transform.rotation.x, newRotation.y, transform.rotation.z, newRotation.w);
        transform.rotation = newRotation;

        //Camera X Rotation
        float cameraXRotation = Input.GetAxis("Mouse Y") * Time.deltaTime * mouseVerticalSensitivity;
        m_cameraPivot.Rotate(cameraXRotation, 0, 0, Space.Self);

        //Clamp X Rotation
        float xRotation = m_cameraPivot.eulerAngles.x;
        if (xRotation > 45 && xRotation <= 90) //If gone past 45 degrees (without touching -)
        {
            xRotation = 45;
        }
        else if (xRotation < 300 && xRotation > 270)
        {
            xRotation = 300;
        }
        //Apply clamp
        m_cameraPivot.localRotation = Quaternion.Euler(xRotation, 0, 0);

        //Rotate Sheep to face same direction as Camera
        //transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, m_cameraPivot.transform.eulerAngles.y, transform.rotation.eulerAngles.x);
        Vector3 camF = m_cam.forward;
        Vector3 camR = m_cam.right;
        camF.y = 0;
        camR.y = 0;
        camF = camF.normalized;
        camR = camR.normalized;
        //Player Movement
        Vector2 input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        input = Vector2.ClampMagnitude(input, 1);

        //? Mustafa code for slowing down
        // if (input.magnitude > .1f)
        // {
        //     //Debug.Log("Input:ON: " + _initialDisplacement);
        //     _initialDisplacement = input;
        //     transform.position += (camF * input.y + camR * input.x) * Time.deltaTime * constantSpeed;
        //     //m_rb.velocity += (camF * input.y + camR * input.x) * Time.deltaTime * speed.x;
        // }
        // else
        // {
        //     
        //     //Debug.Log("Input:OFF: " + _initialDisplacement);
        //     //Vector2 _slowDown = Vector2.Lerp(new Vector2(_initialDisplacement.x, _initialDisplacement.y), Vector2.zero, Time.deltaTime * slowDownRate);
        //     //_initialDisplacement = _slowDown;
        //     //transform.position += (camF * _slowDown.y + camR * _slowDown.x) * Time.deltaTime * constantSpeed;
        // }

        //Calculates velocity change to go forwards
        Vector3 displacement = (camF * constantSpeed) * Time.deltaTime;
        //transform.position += displacement;
        displacement = new Vector3(displacement.x, 0, displacement.z);
        m_rb.AddForceAtPosition(displacement, m_rb.position, ForceMode.VelocityChange);

        ClampBounce();
        currentVelocity = m_rb.velocity;
    }

    public void AddBounceForce(float amount)
    {
        if (m_rb != null)
        {
            amount = Mathf.Clamp(amount, -maxBounceVelocity, maxBounceVelocity);
            Vector3 force = new Vector3(0, amount, 0);
            m_rb.AddForceAtPosition(force, m_rb.position, ForceMode.VelocityChange);
        }
    }

    public void ClampBounce()
    {
        if (m_rb != null)
        {
            m_rb.velocity = new Vector3(m_rb.velocity.x, Mathf.Clamp(m_rb.velocity.y, -maxBounceVelocity, maxBounceVelocity), m_rb.velocity.z);
        }
    }

    public void SetTemporaryMaxBounceVelocity(float maxVelocity, float time)
    {
        maxBounceVelocity = maxVelocity;
        if (m_rb != null)
        {
            AddBounceForce(verticalForceOnBounce);
        }
        Invoke("ResetMaxBounceVelocity", time);
    }

    public void ResetMaxBounceVelocity()
    {
        maxBounceVelocity = originalBounceVelocity;
    }

    private void OnDrawGizmos()
    {
        Debug.DrawRay(transform.position, transform.forward * 10f, Color.red);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Platform"))
        {
            AddBounceForce(verticalForceOnBounce);
        }
    }

    /*

    IEnumerator RotateBy(float amount, float lerpTime) //Rotate smoothly by amount over lerpTime
	{
		float initialRotation = Mathf.FloorToInt(transform.eulerAngles.y);
		float targetRotation =  Mathf.FloorToInt(transform.eulerAngles.y + amount);
		for (float t = 0; t <= 1; t+= 1 / (lerpTime / Time.deltaTime))
		{
			transform.eulerAngles = new Vector3(0, Mathf.Lerp(initialRotation, targetRotation, t) , 0);
			yield return null;
		}
		transform.eulerAngles = new Vector3(0,targetRotation, 0);
        rotate = null;
	}
    
    
     */
}
