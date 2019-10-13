using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Rigidbody))]
public class Platform : MonoBehaviour
{
    // Get Unity Event Reference
    [FMODUnity.EventRef]
    public string selectedSound;
    FMOD.Studio.EventInstance soundEvent;

    // Get Particle Effect Reference
    public GameObject particleEffect;

    public UnityEvent OnLanded = new UnityEvent();
    //public string landEffectPath;

    public bool landedOn;
    [Space]
    public bool fallOnLand;
    public bool destroyOnLand;

    private Rigidbody m_rb;

    // Start is called before the first frame update
    void Start()
    {
        
        soundEvent = FMODUnity.RuntimeManager.CreateInstance(selectedSound);

        OnLanded.AddListener(Landed);
        m_rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        //FMODUnity.RuntimeManager.AttachInstanceToGameObject(soundEvent,GetComponent<Transform>(),GetComponent<Rigidbody>());
    }

    public void RandomizeScale(Vector3 minScale, Vector3 maxScale)
    {
        transform.localScale = new Vector3(Random.Range(minScale.x, maxScale.x), Random.Range(minScale.y, maxScale.y), Random.Range(minScale.z, maxScale.z));
    }

    public void Fall()
    {
        if (m_rb != null)
        {
            m_rb.isKinematic = false;
            Destroy(gameObject, 5f);
        }
    }

    public void Landed()
    {

        soundEvent.start();
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
            //FMODUnity.RuntimeManager.PlayOneShot(landEffectPath, transform.position);

        }
    }

    private void OnDestroy()
    {

    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            Instantiate(particleEffect, transform.position, Quaternion.identity);
            OnLanded.Invoke();
        }
    }

    private void OnTriggerEnter(Collider other)
    {

    }
}
