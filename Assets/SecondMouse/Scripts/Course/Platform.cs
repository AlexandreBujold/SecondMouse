using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Platform : MonoBehaviour
{
    public UnityEvent OnLanded;

    [Space]
    public bool fallOnLand;
    public bool destroyOnLand;

    // Start is called before the first frame update
    void Start()
    {
        OnLanded.AddListener(Landed);
        OnLanded.Invoke();
    }

    public void RandomizeScale()
    {

    }

    public void Fall()
    {

    }

    public void Landed()
    {

    }
}
