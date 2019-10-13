using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class SheepController : MonoBehaviour
{
    #region Variables

    [Header("Movement Options")]
    public float constantSpeed = 5f;
    public float maxBounceVelocity = 10f;
    public float verticalForceOnBounce = 20f;
    [Range(0, 180)]
    public float mouseHorizontalSensitivity = 180;
    [Range(0, 180)]
    public float mouseVerticalSensitivity = 180;
    [Space]
    [Header("Calculated Velocity")]
    public Vector3 currentVelocity;

    private Vector2 _initialDisplacement;

    //Speed up Factors
    [Header("Speed Up Settings")]
    public float speedUpTotal = 0;
    public float bounceUpTotal = 0;
    [Space]
    public int speedStacks = 0;
    public float stackDecayRate = 1f;

    public float speedIncreasePerStack = 3f;
    public float bounceIncreasePerStack = 4f;
    [Range(0, 10)]
    public int maxStacks = 5;
    private List<Coroutine> speedStacksActive = new List<Coroutine>();

    [Space]
    [Header("References")]
    //Camera Movement & Player Rotation
    public Transform m_cameraPivot;
    public Transform m_cam;
    [HideInInspector]
    Rigidbody m_rb;
    private float heading = 0;
    private float originalBounceVelocity;

    #endregion

    // Start is called before the first frame update
    void Start()
    {
        if (m_rb == null)
        {
            m_rb = GetComponentInChildren<Rigidbody>();
        }
        originalBounceVelocity = maxBounceVelocity;

        //* Cursor
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        // for (int i = 0; i < maxStacks; i++)
        // {
        //     Invoke("AddMovementStack", 1f + i);
        // }
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

        #region Mustafa Old Code
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
        #endregion
        //Calculates velocity change to go forwards
        Vector3 displacement = (camF * (constantSpeed + speedUpTotal)) * Time.deltaTime;
        //transform.position += displacement;
        displacement = new Vector3(displacement.x, 0, displacement.z);
        m_rb.AddForceAtPosition(displacement, m_rb.position, ForceMode.VelocityChange);

        if (bounceUpTotal != 0)
        {
            AddBounceForce(bounceUpTotal);
        }

        ClampBounce();
        currentVelocity = m_rb.velocity;
        speedStacks = speedStacksActive.Count;
    }

    #region Speed Up Logic

    public IEnumerator SpeedStack()
    {
        float speedIncrease = speedIncreasePerStack;
        float bounceIncrease = bounceIncreasePerStack;

        speedUpTotal += speedIncrease;
        bounceUpTotal += bounceIncrease;
        while (speedIncrease > 0 && bounceIncrease > 0)
        {
            float decay = stackDecayRate * Time.deltaTime;
            speedUpTotal = Mathf.Clamp(speedUpTotal - decay, 0, 10000);
            bounceUpTotal = Mathf.Clamp(speedUpTotal - decay, 0, 10000);
            yield return new WaitForFixedUpdate();
        }
        yield break;
    }

    public IEnumerator RemoveCoroutine(float time, Coroutine target)
    {
        yield return new WaitForSeconds(time);
        speedStacksActive.Remove(target);
    }

    public bool AddMovementStack()
    {
        if (speedStacksActive.Count < maxStacks)
        {
            Coroutine newStack = StartCoroutine(SpeedStack());
            speedStacksActive.Add(newStack);
            //Time average divided by number of times run in a second to get the real world time
            float coroutineRuntimeAverage = ((speedIncreasePerStack + bounceIncreasePerStack) / 2) / (1 / Time.deltaTime);
            StartCoroutine(RemoveCoroutine(coroutineRuntimeAverage, newStack));
            return true;
        }
        return false;
    }

    public bool RemoveStack(Coroutine stack)
    {
        return speedStacksActive.Remove(stack);
    }

    #endregion

    #region Bouncing

    public void AddBounceForce(float amount)
    {
        if (m_rb != null)
        {
            amount = Mathf.Clamp(amount, -maxBounceVelocity, maxBounceVelocity);
            Vector3 force = new Vector3(0, amount, 0);
            m_rb.AddForceAtPosition(force, m_rb.position, ForceMode.VelocityChange);
        }
    }

    public void AddBounceSpeed(float amount)
    {
        if (m_rb != null)
        {
            amount = Mathf.Clamp(amount, -maxBounceVelocity, maxBounceVelocity);
            Vector3 speed = new Vector3(0, Mathf.Sign(m_rb.velocity.y) * amount, 0);
            m_rb.AddForce(speed, ForceMode.Acceleration);
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

    #endregion

    #region Collisions & Gizmos

    private void OnDrawGizmos()
    {
        Debug.DrawRay(transform.position, transform.forward * 10f, Color.red);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Platform"))
        {
            AddBounceForce(maxBounceVelocity);
            AddMovementStack();
        }
    }
    #endregion
}
