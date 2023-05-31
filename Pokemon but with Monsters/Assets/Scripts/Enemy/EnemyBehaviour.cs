using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(NavMeshAgent))]
public class EnemyBehaviour : MonoBehaviour
{
    //Aggro State grabs points closer to the player
    public enum EnemyState
    {
        Roaming,
        Following,
        Aggro,
        Captured,
        Forklift
    };

    public EnemyState behaviourState;
    [Space]
    public Transform target;
    [Space]
    public float speed = 5;
    public float acceleration = 5;
    public float obstacleAvoidanceRadius = .5f;
    public float dstThreshold;
    [Space]
    public float aggroRange;
    public float followRange;
    [Tooltip("The time between the updates of the list of aggro points")]public float aggroUpdateSpeed;
    [Space]
    public NavMeshAgent agent;

    [HideInInspector] public List<Transform> roamingPoints;
    [HideInInspector] public List<Transform> aggroPoints;
    [HideInInspector] public bool navMeshBuild;

    private float aggroTimer;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();

        InitializeUnit();
    }

    private void Update()
    {
        if(navMeshBuild)
        {
            if(behaviourState != EnemyState.Forklift)
            {
                behaviourState = CheckAggroState();
                behaviourState = CheckFollowState();
            }

            if(aggroTimer <= 0)
            {
                UpdateAggroList();

                aggroTimer = aggroUpdateSpeed;
            }else
            {
                aggroTimer -= Time.deltaTime;
            }

            UpdateState();
        }
    }

    void UpdateState()
    {
        switch (behaviourState)
        {
            case EnemyState.Following:
                if(target != GameObject.FindGameObjectWithTag("Player").transform)
                {
                    target = GameObject.FindGameObjectWithTag("Player").transform;
                }

                if(target != null)
                {
                    float dstToPlayer = Vector3.Distance(target.position, transform.position);

                    if (dstToPlayer <= dstThreshold)
                    {
                        agent.speed = 0;
                    }else
                    {
                        agent.speed = speed;
                    }
                }
                break;

            case EnemyState.Aggro:
                if (target == null)
                {
                    target = aggroPoints[Random.Range(0, aggroPoints.Count)];
                }

                if (target != null)
                {
                    float dst = Vector3.Distance(target.position, transform.position);

                    if (dst <= dstThreshold)
                    {
                        target = null;
                    }
                }
                break;

            case EnemyState.Roaming:
                if (target == null)
                {
                    target = roamingPoints[Random.Range(0, roamingPoints.Count)];
                }

                if (target != null)
                {
                    float dst = Vector3.Distance(target.position, transform.position);

                    if (dst <= dstThreshold)
                    {
                        target = null;
                    }
                }
                break;

            case EnemyState.Forklift:
                if (target != GameObject.FindGameObjectWithTag("Player").transform)
                {
                    target = GameObject.FindGameObjectWithTag("Player").transform;
                }

                if (target != null)
                {
                    float dstToPlayer = Vector3.Distance(target.position, transform.position);

                    if (dstToPlayer <= dstThreshold)
                    {
                        agent.speed = 0;
                    }
                    else
                    {
                        agent.speed = speed;
                    }
                }
                break;
        }

        if(target != null)
        {
            agent.SetDestination(target.position);
        }
    }

    EnemyState CheckAggroState()
    {
        float dst = Vector3.Distance(GameObject.FindGameObjectWithTag("Player").transform.position, transform.position);

        return (dst < aggroRange && dst > followRange) ? EnemyState.Aggro : behaviourState;
    }

    EnemyState CheckFollowState()
    {
        float dst = Vector3.Distance(GameObject.FindGameObjectWithTag("Player").transform.position, transform.position);

        return dst < followRange ? EnemyState.Following : behaviourState;
    }

    void UpdateAggroList()
    {
        aggroPoints.Clear();

        foreach(Collider ap in Physics.OverlapSphere(transform.position, aggroRange))
        {
            if(ap.gameObject.CompareTag("RoamingPoint"))
            {
                aggroPoints.Add(ap.transform);
            }
        }
    }

    void InitializeUnit()
    {
        GameObject[] roamingObjects = GameObject.FindGameObjectsWithTag("RoamingPoint");

        foreach (GameObject rp in roamingObjects)
        {
            roamingPoints.Add(rp.transform);
        }

        behaviourState = EnemyState.Roaming;

        agent.speed = speed;
        agent.radius = obstacleAvoidanceRadius;
        agent.acceleration = acceleration;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, aggroRange);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, followRange);
    }
}
