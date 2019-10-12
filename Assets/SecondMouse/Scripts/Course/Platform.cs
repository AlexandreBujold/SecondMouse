using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Rigidbody))]
public class Platform : MonoBehaviour
{
    public UnityEvent OnLanded;

    public bool landedOn;
    [Space]
    public bool fallOnLand;
    public bool destroyOnLand;

    private Rigidbody m_rb;

    // Start is called before the first frame update
    void Start()
    {
        OnLanded.AddListener(Landed);
        m_rb = GetComponent<Rigidbody>();
    }

    public void RandomizeScale(Vector3 minScale, Vector3 maxScale)
    {
        transform.localScale = new Vector3(Random.Range(minScale.x, maxScale.x), Random.Range(minScale.y, maxScale.y), Random.Range(minScale.z, maxScale.z));
    }

    public void Fall()
    {
        if (m_rb != null)
        {
            m_rb.isKinematic = true;
            Destroy(gameObject, 5f);
        }
    }

    public void Landed()
    {
        if (landedOn == false)
        {
            landedOn = true;
            if (fallOnLand)
            {
                Fall();
            }

            if (destroyOnLand)
            {
                Destroy(gameObject, 2f);
            }
        }
    }

    private void OnDestroy()
    {

    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            OnLanded.Invoke();
        }
    }

    private void OnTriggerEnter(Collider other)
    {

    }
}
