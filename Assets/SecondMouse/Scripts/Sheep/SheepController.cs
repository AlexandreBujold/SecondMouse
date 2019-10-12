using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class SheepController : MonoBehaviour
{

    Rigidbody m_rb;
    public float topSpeed = 5f;
    public float maxBounceVelocity = 10f;
    public Vector3 currentVelocity;

    [Space]
    //Camera Movement & Player Rotation
    public Transform m_cameraPivot;
    public Transform m_cam;
    private float heading = 0;

    // Start is called before the first frame update
    void Start()
    {
        if (m_rb == null)
        {
            m_rb = GetComponentInChildren<Rigidbody>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        //Rotate Camera
        heading += Input.GetAxis("Mouse X") * Time.deltaTime * 180;
        m_cameraPivot.rotation = Quaternion.Euler(0, heading, 0);

        //Rotate Sheep to face same direction as Camera
        transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, m_cameraPivot.transform.eulerAngles.y, m_cameraPivot.rotation.eulerAngles.z);

        //Player Movement
        Vector2 input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        input = Vector2.ClampMagnitude(input, topSpeed);

        Vector3 camF = m_cam.forward;
        Vector3 camR = m_cam.right;

        camF.y = 0;
        camR.y = 0;
        camF = camF.normalized;
        camR = camR.normalized;

        //m_rb.velocity = Vector3.Lerp(camF * input.y + camR * input.x, m_rb.velocity, topSpeed);

        transform.position += (camF * input.y + camR * input.x) * Time.deltaTime * topSpeed;
        //m_rb.velocity += (camF * input.y + camR * input.x) * Time.deltaTime * speed.x;

        AdjustBounce();
        currentVelocity = m_rb.velocity;
    }

    public void AdjustBounce()
    {
        if (m_rb != null)
        {
            Vector3.ClampMagnitude(m_rb.velocity, maxBounceVelocity);
        }
    }

    // public void Decelerate(ref Vector3 cameraForward, ref Vector3 cameraRight, ref Vector2 input)
    // {
    //     if (input.x == 0 || input.y == 0)
    //     {
    //         if (m_rb.velocity.x >= -0.01f && m_rb.velocity.x <= 0.01f)
    //         {
    //             m_rb.velocity = new Vector3(0, m_rb.velocity.y, m_rb.velocity.z);
    //         }
    //         else
    //         {
    //             m_rb.velocity += (-cameraForward - cameraRight) * Time.deltaTime * speed.x;
    //         }
    //     }

    //     // if (input.y == 0)
    //     // {
    //     //     if (m_rb.velocity.z >= -0.01f && m_rb.velocity.z <= 0.01f)
    //     //     {
    //     //         m_rb.velocity = new Vector3(m_rb.velocity.x, m_rb.velocity.y, 0);
    //     //     }
    //     //     else
    //     //     {
    //     //         m_rb.velocity += (-cameraForward * input.y - cameraRight * input.x) * Time.deltaTime * speed.x;
    //     //     }
    //     // }
    // }


    private void OnDrawGizmos()
    {
        Debug.DrawRay(transform.position, transform.up * 10f, Color.red);
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
