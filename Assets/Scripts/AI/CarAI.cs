using System;
using System.Collections;
using System.Collections.Generic;
using SimpleCity.AI;
using UnityEngine;
using UnityEngine.Events;

public class CarAI : MonoBehaviour
{
    [SerializeField]
    private List<Vector3> path = null;
    [SerializeField]
    private float arriveDistance = .3f, lastPointArriveDistance = .1f;
    [SerializeField]
    private float turningAngleOffset = 5;
    [SerializeField]
    private Vector3 currentTargetPosition;

    [SerializeField]
    private GameObject raycastStartingPoint = null;
    [SerializeField]
    private float collisionRaycastLength = 0.1f;

    internal bool IsThisLastPathIndex()
    {
        return index >= path.Count-1;
    }

    private int index = 0;

    private bool stop;
    private bool collisionStop = false;

    public bool Stop
    {
        get { return stop || collisionStop; }
        set { stop = value; }
    }

    private bool charging = false;
    private Vector3 lastPosition;
    private float idleTime = 0f;
    private const float idleThreshold = 0.1f;
    private const float idleDuration = 5f;

    public event Action<GameObject> OnCarDestroyed;

    private void OnDestroy()
    {
        OnCarDestroyed?.Invoke(gameObject);
    }

    [field: SerializeField]
    public UnityEvent<Vector2> OnDrive { get; set; }

    public bool isElectric = false;
    public bool needsCharge = false;
    public StructureModel charger = null;
    public void SetEVParams(bool isElectric, bool needsCharge, StructureModel charger)
    {
        this.isElectric = isElectric;
        this.needsCharge = needsCharge;
        this.charger = charger;
    }

    public static Dictionary<StructureModel, int> chargerCarCount = new Dictionary<StructureModel, int>();

    public event Action<GameObject> OnChargingComplete;

    public Vector3 CurrentPosition => path[Math.Min(index, path.Count - 1)];

    private void Start()
    {
        lastPosition = transform.position;
        if(path == null || path.Count == 0)
        {
            Stop = true;
        }
        else
        {
            currentTargetPosition = path[index];
        }
    }

    public void SetPath(List<Vector3> path)
    {
        if(path.Count == 0)
        {
            Destroy(gameObject);
            return;
        }
        this.path = path;
        index = 0;
        currentTargetPosition = this.path[index];

        Vector3 relativepoint = transform.InverseTransformPoint(this.path[index + 1]);

        float angle = Mathf.Atan2(relativepoint.x, relativepoint.z) * Mathf.Rad2Deg;

        transform.rotation = Quaternion.Euler(0, angle, 0);
        Stop = false;
    }

    private void Update()
    {
        CheckIfArrived();
        Drive();
        CheckForCollisions();
        CheckIfIdle();
    }

    private void CheckForCollisions()
    {
        if(Physics.Raycast(raycastStartingPoint.transform.position, transform.forward,collisionRaycastLength, 1 << gameObject.layer))
        {
            collisionStop = true;
        }
        else
        {
            collisionStop = false;
        }
    }

    private void Drive()
    {
        if (Stop)
        {
            OnDrive?.Invoke(Vector2.zero);
        }
        else
        {
            Vector3 relativepoint = transform.InverseTransformPoint(currentTargetPosition);
            float angle = Mathf.Atan2(relativepoint.x, relativepoint.z) * Mathf.Rad2Deg;
            var rotateCar = 0;
            if(angle > turningAngleOffset)
            {
                rotateCar = 1;
            }else if(angle < -turningAngleOffset)
            {
                rotateCar = -1;
            }
            OnDrive?.Invoke(new Vector2(rotateCar, 1));
        }
    }

    private void CheckIfArrived()
    {
        if(Stop == false)
        {
            var distanceToCheck = arriveDistance;
            if(index == path.Count - 1)
            {
                distanceToCheck = lastPointArriveDistance;
            }
            if(Vector3.Distance(currentTargetPosition,transform.position) < distanceToCheck)
            {
                SetNextTargetIndex();
            }
        }
    }

    private void SetNextTargetIndex()
    {
        index++;
        if(index >= path.Count)
        {
            if (isElectric && needsCharge)
            {
                StartCharging();
            }
            else
            {
                AiDirector.completedJourneys++;
                if (isElectric)
                {
                    AiDirector.evJourneys++;
                }
                else
                {
                    AiDirector.normalJourneys++;
                }
                Stop = true;
                Destroy(gameObject);
            }
        }
        else
        {
            currentTargetPosition = path[index];
        }
    }

    private void CheckIfIdle()
    {
        float speed = (transform.position - lastPosition).magnitude / Time.deltaTime;
        lastPosition = transform.position;

        if (speed < idleThreshold)
        {
            idleTime += Time.deltaTime;
        }
        else
        {
            idleTime = 0f;
        }

        if (idleTime >= idleDuration && !charging)
        {
            Debug.Log("Car is idle");
            Stop = true;
            if (isElectric){
                AiDirector.evJourneys--;
            } else {
                AiDirector.normalJourneys--;
            }
            AiDirector.completedJourneys--;
            Destroy(gameObject);
        }
    }

    private void StartCharging()
    {
        Stop = true;
        charging = true;
        transform.position = new Vector3(transform.position.x, -10, transform.position.z);
        float chargingTime = UnityEngine.Random.Range(2f, 5f);

        if (!chargerCarCount.ContainsKey(charger))
        {
            chargerCarCount[charger] = 0;
        }
        chargerCarCount[charger]++;

        GameObject[] cubes = GameObject.FindGameObjectsWithTag("Cube");
        foreach (GameObject cube in cubes)
        {
            if (cube.transform.IsChildOf(charger.transform))
            {
                Debug.Log("Charging the car");
                MeshRenderer cubeRenderer = cube.GetComponent<MeshRenderer>();
                if (cubeRenderer != null)
                {
                    Debug.Log("Changing the color of the cube");
                    cubeRenderer.material.color = Color.magenta;
                }
                break;
            }
        }

        StartCoroutine(ChargingCoroutine(chargingTime));
    }

    private IEnumerator ChargingCoroutine(float chargingTime)
    {
        yield return new WaitForSeconds(chargingTime);
        charging = false;
        OnChargingComplete?.Invoke(gameObject);
    }
}
