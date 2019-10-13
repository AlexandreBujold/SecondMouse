using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class PlatformSpawner : MonoBehaviour
{
    [Header("Generation Settings")]
    public float startDelay = 20f;
    public bool spawnAnotherSpawnerAtEndOfCourse = false;
    public bool ResetValues = false;
    [Range(0, 5f)]
    public float tickRate = 1f;
    public int platformCount = 20;
    [Tooltip("The angle range that the new heading's angle can be.")]
    public Vector2 newHeadingAngle = new Vector2(90f, 45f);
    [Tooltip("A weight of 1 will make it so that the course heading matches the newest heading, a weight of 0 will keep the course heading as is. A weight in between will add that % to the course heading.")]
    public Vector3 newHeadingWeight;
    public bool randomizeHeading = false;
    [Space]
    public Vector3 minimumDistanceBetweenPlatforms = Vector3.one;
    public Vector3 maximumDistanceBetweenPlatforms = Vector3.one * 10f;
    [Space]
    [Header("Randomness Settings")]
    public float randomMaxValue = 5f;
    [Range(0, 1)]
    public float randomXChance = 0.05f;
    [Range(0, 1)]
    public float randomYChance = 0.05f;


    [Header("Course Info")]
    [Space(20)]
    public Vector3 courseHeading = Vector3.forward;
    private Vector3 originalCourseHeading;

    public bool TestGeneration = false;

    [Header("Lists & References")]
    [Space]
    public GameObject platformSpawnerPrefab;
    public List<Vector3> platformPositions;
    public List<Transform> platforms;
    [Space]
    public List<GameObject> platformPrefabs;

    private Coroutine CalculatePositionsCoroutine;

    Vector3 startPos;

    bool positionsCreated = false;
    bool platformsCreated = false;
    bool courseCreated = false;

    // Start is called before the first frame update
    void Start()
    {
        startPos = transform.position;


        if (randomizeHeading)
        {
            courseHeading = new Vector3(Random.Range(-1f, 1f), 1, 1);
        }

        originalCourseHeading = courseHeading;

        if (courseCreated == false && TestGeneration == false)
        {
            StartCoroutine(CreateCourse(true, true));
        }
        if (this.tickRate <= 0)
        {
            this.tickRate = 0;
        }

        if (this.startDelay == 0)
        {
            this.startDelay = 0.1f;
        }

        if (ResetValues)
        {
            startDelay = 20f;
            tickRate = 0.1f;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (transform.position.y <= 10)
        {
            transform.position = startPos;
        }

        if (TestGeneration)
        {
            if (CalculatePositionsCoroutine == null)
            {
                RemoveAnyExistingCourse();
                CalculatePositionsCoroutine = StartCoroutine(CalculatePositions(platformCount, 0));
            }
        }

    }

    public IEnumerator CreateCourse()
    {
        return CreateCourse(false, true);
    }

    public IEnumerator CreateCourse(bool randomizePlatforms, bool removeExisting)
    {
        yield return new WaitForSeconds(startDelay);
        if (removeExisting)
        {
            RemoveAnyExistingCourse();
        }
        for (; ; )
        {
            if (positionsCreated == false)
            {
                if (CalculatePositionsCoroutine == null)
                {
                    CalculatePositionsCoroutine = StartCoroutine(CalculatePositions(platformCount, 0));
                }
            }
            else
            {
                if (platformsCreated == false)
                {
                    Vector3 lastPos = Vector3.zero;
                    foreach (Vector3 pos in platformPositions)
                    {
                        CreatePlatform(pos, randomizePlatforms);
                        lastPos = pos;
                        yield return new WaitForSeconds(tickRate);
                    }

                    platformsCreated = true;
                    if (spawnAnotherSpawnerAtEndOfCourse)
                    {
                        SpawnSpawner(lastPos);
                    }
                }
            }

            if (positionsCreated == true && platformsCreated == true)
            {
                courseCreated = true;
            }
            yield return new WaitForSeconds(tickRate);
        }
    }

    public GameObject CreatePlatform(Vector3 position, bool randomizePlatforms)
    {
        if (platformPrefabs != null)
        {
            if (platformPrefabs.Count > 0)
            {
                GameObject newPlatform;
                if (randomizePlatforms)
                {
                    newPlatform = Instantiate(platformPrefabs[Random.Range(0, platformPrefabs.Count)], position, Quaternion.identity, transform);
                    //Randomize via the Platform script
                }
                else
                {
                    newPlatform = Instantiate(platformPrefabs[0], position, Quaternion.identity, transform);
                }
                return newPlatform;
            }
        }
        return null;
    }

    public void RemoveAnyExistingCourse()
    {
        platformPositions.Clear();
        foreach (Transform platformTransform in platforms)
        {
            Destroy(transform);
        }
        SetHeading(originalCourseHeading);
        courseCreated = false;
        platformsCreated = false;
        positionsCreated = false;
    }

    public void UpdateHeading(Vector3 newestPlatformDirection)
    {
        Vector3 newHeadingInfluence = new Vector3(newestPlatformDirection.x * newHeadingWeight.x, newestPlatformDirection.y * newHeadingWeight.y, newestPlatformDirection.z * newHeadingWeight.z);

        //Randomness
        Vector3 randomInfluence = Vector3.zero;
        if (Random.value <= randomXChance)
        {
            randomInfluence = new Vector3(Random.Range(0, randomMaxValue), 0, 0);
        }

        if (Random.value <= randomYChance)
        {
            randomInfluence = new Vector3(randomInfluence.x, Random.Range(0, randomMaxValue), 0);
        }

        newHeadingInfluence.z = 0;
        newHeadingInfluence += randomInfluence;
        courseHeading += newHeadingInfluence;
    }

    public void SetHeading(Vector3 newHeading)
    {
        courseHeading = newHeading;
    }

    public GameObject SpawnSpawner(Vector3 pos)
    {
        if (platformSpawnerPrefab != null)
        {
            Quaternion randomRotation = Random.rotation;
            randomRotation = new Quaternion(0, randomRotation.y, 0, randomRotation.w);
            return Instantiate(platformSpawnerPrefab, pos, randomRotation, transform.parent);
        }
        return null;
    }

    public IEnumerator CalculatePositions(float amount, int startIndex)
    {
        if (platformPositions.Count >= startIndex && startIndex != 0) //Start index out of bounds, go to last item in collection
        {
            startIndex = platformPositions.Count - 1;
        }

        //Iterate until a position count of amount has been created
        for (int i = 0; i < amount; i++)
        {
            Vector3 currentHeading = courseHeading;

            //Check for trending downwards
            if (i >= amount - 1 && platformPositions[platformPositions.Count - 1].y < platformPositions[0].y) //If the last position's y is lower than the first position's, recalculate by resetting i
            {
                //Re run this loop - reset i to 0
                i = 0;
                platformPositions.RemoveRange(startIndex, platformPositions.Count);
                currentHeading = originalCourseHeading;
            }

            //Get the proper index so we know its all in order
            int currentIndex = startIndex + i;

            //Calculate the randomized rotation that the heading vector will be rotated by
            Vector2 halfRotation = new Vector2(newHeadingAngle.x / 2f, newHeadingAngle.y / 2f);
            float randomXRotation = Random.Range(-halfRotation.x, halfRotation.x);
            float randomYRotation = Random.Range(-halfRotation.y, halfRotation.y);

            //Create Ray
            Ray newHeading;
            //Setting ray's origin depending on if there are previous platforms or not
            if (currentIndex == 0)
            {
                newHeading = new Ray(transform.position, currentHeading * maximumDistanceBetweenPlatforms.z);
            }
            else
            {
                newHeading = new Ray(platformPositions[currentIndex - 1], currentHeading * maximumDistanceBetweenPlatforms.z);
            }

            //Rotate Ray
            newHeading.direction = Quaternion.Euler(randomYRotation, randomXRotation, 0) * newHeading.direction;

            //Get the platform position along the ray between a random 
            Vector3 newPosition = newHeading.GetPoint(Random.Range(minimumDistanceBetweenPlatforms.magnitude, maximumDistanceBetweenPlatforms.magnitude));
            platformPositions.Add(newPosition);
            UpdateHeading(newHeading.direction);

            if (tickRate <= 0)
            {
                yield return new WaitForFixedUpdate();
            }
            else
            {
                yield return new WaitForSeconds(tickRate);
            }
        }
        positionsCreated = true;
        CalculatePositionsCoroutine = null;
    }

    private void OnDrawGizmosSelected()
    {
        float halfFOV_X = newHeadingAngle.x / 2.0f;
        float halfFOX_Y = newHeadingAngle.y / 2f;

        Quaternion leftRayRotation = Quaternion.AngleAxis(-halfFOV_X, Vector3.up);
        Quaternion rightRayRotation = Quaternion.AngleAxis(halfFOV_X, Vector3.up);
        Quaternion upRayRotation = Quaternion.AngleAxis(halfFOX_Y, Vector3.right);
        Quaternion downRayRotation = Quaternion.AngleAxis(-halfFOX_Y, Vector3.right);

        Vector3 leftRayDirection = leftRayRotation * courseHeading;
        Vector3 rightRayDirection = rightRayRotation * courseHeading;
        Vector3 upRayDirection = upRayRotation * courseHeading;
        Vector3 downRayDirection = downRayRotation * courseHeading;

        Gizmos.color = Color.red;

        Ray leftRay;
        Ray rightRay;
        Ray upRay;
        Ray downRay;


        if (platformPositions.Count == 0)
        {
            Debug.DrawLine(transform.position, transform.position + minimumDistanceBetweenPlatforms, Color.yellow);

            leftRay = new Ray(transform.position + minimumDistanceBetweenPlatforms, leftRayDirection * maximumDistanceBetweenPlatforms.z);
            rightRay = new Ray(transform.position + minimumDistanceBetweenPlatforms, rightRayDirection * maximumDistanceBetweenPlatforms.z);

            upRay = new Ray(transform.position + minimumDistanceBetweenPlatforms, upRayDirection * maximumDistanceBetweenPlatforms.z);
            downRay = new Ray(transform.position + minimumDistanceBetweenPlatforms, downRayDirection * maximumDistanceBetweenPlatforms.z);
        }
        else
        {
            leftRay = new Ray(platformPositions[platformPositions.Count - 1] + minimumDistanceBetweenPlatforms, leftRayDirection * maximumDistanceBetweenPlatforms.z);
            rightRay = new Ray(platformPositions[platformPositions.Count - 1] + minimumDistanceBetweenPlatforms, rightRayDirection * maximumDistanceBetweenPlatforms.z);

            upRay = new Ray(platformPositions[platformPositions.Count - 1] + minimumDistanceBetweenPlatforms, upRayDirection * maximumDistanceBetweenPlatforms.z);
            downRay = new Ray(platformPositions[platformPositions.Count - 1] + minimumDistanceBetweenPlatforms, downRayDirection * maximumDistanceBetweenPlatforms.z);

            Debug.DrawLine(platformPositions[platformPositions.Count - 1], leftRay.origin, Color.yellow);
        }


        Gizmos.color = Color.red;
        Gizmos.DrawRay(leftRay.origin, leftRay.direction * maximumDistanceBetweenPlatforms.z);
        Gizmos.DrawRay(rightRay.origin, rightRay.direction * maximumDistanceBetweenPlatforms.z);
        Gizmos.color = Color.green;
        Gizmos.DrawRay(upRay.origin, upRay.direction * maximumDistanceBetweenPlatforms.z);
        Gizmos.DrawRay(downRay.origin, downRay.direction * maximumDistanceBetweenPlatforms.z);

        List<Vector3> rayEnds = new List<Vector3>();
        rayEnds.Add(leftRay.GetPoint(maximumDistanceBetweenPlatforms.z));
        rayEnds.Add(downRay.GetPoint(maximumDistanceBetweenPlatforms.z));
        rayEnds.Add(rightRay.GetPoint(maximumDistanceBetweenPlatforms.z));
        rayEnds.Add(upRay.GetPoint(maximumDistanceBetweenPlatforms.z));

        for (int i = 0; i < rayEnds.Count; i++)
        {
            if (i == 0)
            {
                Debug.DrawLine(rayEnds[rayEnds.Count - 1], rayEnds[i], Color.white);
            }
            else
            {
                Debug.DrawLine(rayEnds[i - 1], rayEnds[i], Color.white);
            }
        }
        DrawPositions();
    }

    public void DrawPositions()
    {
        if (platformPositions != null)
        {
            if (platformPositions.Count != 0)
            {
                for (int i = 0; i < platformPositions.Count; i++)
                {
                    Gizmos.color = Color.red;
                    Gizmos.DrawSphere(platformPositions[i], 1f);
                    if (i != 0)
                    {
                        Debug.DrawLine(platformPositions[i - 1], platformPositions[i], Color.magenta);
                    }
                }
            }
        }
    }
}
