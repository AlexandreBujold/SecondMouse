using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class SheepPowers : MonoBehaviour
{
    [Header("Powers")]
    public bool cloudPower;
    public bool superJump;
    public bool doubleScore;

    [Space]

    [Header("Power Settings")]
    public float superJumpTime;
    public float doubleScoreTime;
    public float yOffsetBelow = 2f;

    [Space]
    [Header("Timer")]
    public float jumpTimeRemaining = 0;
    public float scoreTimeRemaining = 0;

    [Space]
    public GameObject pickupEffect;
    public PlatformSpawner platformSpawner;
    public SheepController sheepController;


    [Space(20)]
    public UnityEvent PowerPickup = new UnityEvent();

    // Start is called before the first frame update
    void Start()
    {
        if (platformSpawner == null)
        {
            platformSpawner = GameObject.FindGameObjectWithTag("Spawner").GetComponent<PlatformSpawner>();
        }

        if (sheepController == null)
        {
            sheepController = GetComponent<SheepController>();
        }
        jumpTimeRemaining = superJumpTime;
        scoreTimeRemaining = doubleScoreTime;
    }

    // Update is called once per frame
    void Update()
    {
        if (cloudPower == true)
        {
            if (Input.GetAxis("Jump") != 0)
            {
                if (platformSpawner != null)
                {
                    platformSpawner.CreatePlatform(transform.position - new Vector3(0, yOffsetBelow, 0), true);
                    DeactivateCloudPower();
                }
            }
        }

        //Timers really used for UI, not logic
        if (superJump == true)
        {
            jumpTimeRemaining -= Time.deltaTime;
        }

        if (doubleScore == true)
        {
            scoreTimeRemaining -= Time.deltaTime;
        }

        UpdateUI();
    }

    private void UpdateUI() //!
    {

    }

    public void CloudPower()
    {
        if (cloudPower == false)
        {
            cloudPower = true;
            PowerPickup.Invoke();
        }
    }

    private void DeactivateCloudPower()
    {
        cloudPower = false;
    }

    public void JumpPower()
    {
        if (superJump == false)
        {
            superJump = true;
            if (sheepController != null)
            {
                sheepController.SetTemporaryMaxBounceVelocity(sheepController.maxBounceVelocity * 1.5f, superJumpTime);
                Invoke("DeactiveJumpPower", superJumpTime);
            }
            PowerPickup.Invoke();
        }
    }

    private void DeactiveJumpPower()
    {
        sheepController.ResetMaxBounceVelocity();
        superJump = false;
        jumpTimeRemaining = superJumpTime;
    }

    public void ScorePower()
    {
        if (doubleScore == false)
        {
            doubleScore = true;
            PowerPickup.Invoke();
            ScoreManager.instance.MultiplierActive(2f, doubleScoreTime);
        }
    }

    private void DeactiveScorePower()
    {
        doubleScore = false;
        scoreTimeRemaining = doubleScoreTime;
    }

    private void OnTriggerEnter(Collider other)
    {
        switch (other.tag)
        {
            case "CloudPower":
                CloudPower();
                DestroyPickup(other);
                break;
            case "JumpPower":
                JumpPower();
                DestroyPickup(other);
                break;
            case "ScorePower":
                ScorePower();
                DestroyPickup(other);
                break;
        }
    }

    public void DestroyPickup(Collider other)
    {
        if (pickupEffect != null)
        {
            Instantiate(pickupEffect, other.gameObject.transform.position, Quaternion.identity);
        }
        Destroy(other.gameObject);
    }
}
