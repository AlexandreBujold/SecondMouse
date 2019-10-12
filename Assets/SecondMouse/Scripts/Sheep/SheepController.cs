using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class SheepController : MonoBehaviour
{

    Rigidbody m_rb;
    public Vector3 speed = Vector3.one;
    public Vector3 currentVelocity;
    private Quaternion startRotation;

    // Start is called before the first frame update
    void Start()
    {
        if (m_rb == null)
        {
            m_rb = GetComponentInChildren<Rigidbody>();
        }

        startRotation = transform.rotation;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Vector2 input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        if (m_rb != null)
        {
            Vector3 velocity = new Vector3(input.x * speed.x, m_rb.velocity.y, input.y * speed.z);
            //Debug.LogWarning(velocity);
            //velocity = transform.TransformDirection(velocity);
            //Debug.LogError(velocity);
            m_rb.velocity = velocity;
            currentVelocity = velocity;
        }
        //transform.rotation = startRotation;
    }

    /*
    
    public NavMeshAgent myNavAgent;
    public Camera gameCamera;
    public Vector3 speed;

    private Coroutine rotate;

    // Start is called before the first frame update
    void Start()
    {
        if (myNavAgent == null)
        {
            myNavAgent = GetComponentInChildren<NavMeshAgent>();
        }

        if (gameCamera == null)
        {
            gameCamera = Camera.main; //!
        }

    }

    // Update is called once per frame
    void Update()
    {
        Vector2 input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        if (myNavAgent != null && gameCamera != null)
        {
            if (Mathf.Sign(input.y) == -1)
            {
                input.y = 0;
            }
            Vector3 velocity = new Vector3(input.x * speed.x, myNavAgent.velocity.y, input.y * speed.z);
            velocity = transform.TransformDirection(velocity);
            myNavAgent.velocity = velocity;
        }

        if (Input.GetKeyDown(KeyCode.S))
        {
            if (rotate == null)
            {
                rotate = StartCoroutine(RotateBy(180, 0.5f));
            }
        }
    }

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
