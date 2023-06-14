using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Profiling;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(NavMeshAgent))]
public class EnemyBehaviour : MonoBehaviour
{
    public enum EnemyState
    {
        Roaming,
        Following,
        Aggro,
        Captured,
        Forklift
    };

    public enum EnemyType
    {
        None,
        Death
    }

    public enum Enemy
    {
        FleshBlob,
        WheepingAngel
    }

    public EnemyType type;
    public EnemyState behaviourState;
    public Enemy behaviour;
    [Space]
    public Transform target;
    public float pathRefreshRate = 1.0f;
    [Space]
    public int maxHP;
    public float invisFrameTime;
    [Space]
    public float speed = 5;
    [Tooltip("Keep this a minimum of 2X of the speed.")]public float acceleration = 10;
    public float obstacleAvoidanceRadius = .5f;
    public float dstThreshold;
    [Tooltip("The desired distance the Enemy keeps from the player when following")]public float stoppingDst;
    [Space]
    public float aggroRange;
    public float followRange;
    [Tooltip("The time between the updates of the list of aggro points")]public float aggroUpdateSpeed;
    [Space]
    public bool isTamable;
    [Space]
    public NavMeshAgent agent;

    [HideInInspector] public List<Transform> roamingPoints;
    [HideInInspector] public List<Transform> aggroPoints;
    [HideInInspector] public bool navMeshBuild;
    [HideInInspector] public bool inView = false;

    private EnemyState previousBehaviourState;
    private int currentHP;
    private float aggroTimer;
    private bool invisFramesActive = false;
    [HideInInspector]public bool canInvokeInvisReset = true;
    private Transform player;
    private List<Vector3> path;
    private Grids grid;
    private float pathRefreshTimer = 0f;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
        //grid = GameObject.FindGameObjectWithTag("Pathfinder").GetComponent<Grids>();

        InitializeUnit();
    }

    private void Update()
    {
        if(navMeshBuild)
        {
            CheckEnemySpecificTrait();

            if(behaviourState != previousBehaviourState)
            {
                previousBehaviourState = behaviourState;

                CheckBehaviourSettings();

                target = null;
            }

            if(behaviourState != EnemyState.Forklift && behaviourState != EnemyState.Captured)
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

            MoveEnemy();
        }
    }

    void CheckEnemySpecificTrait()
    {
        switch(behaviour)
        {
            case Enemy.WheepingAngel:
                if(inView)
                {
                    agent.speed = 0;
                }else
                {
                    agent.speed = speed;
                }
                break;
        }
    }

    void MoveEnemy()
    {
        switch (behaviourState)
        {
            case EnemyState.Following:
                if (target != player)
                {
                    target = player;
                }

                if(target != null)
                {
                    float dstToPlayer = Vector3.Distance(target.position, transform.position);

                    if(dstToPlayer <= dstThreshold)
                    {
                        agent.speed = 0;
                        agent.velocity = Vector3.zero;

                        /*pathRefreshTimer += Time.deltaTime;

                        if (pathRefreshTimer > pathRefreshRate)
                        {
                            pathRefreshTimer = 0;
                            FindPath();
                        }*/
                    }
                    else
                    {
                        if(behaviour != Enemy.WheepingAngel)
                        {
                            agent.speed = speed;
                        }
                    }
                }
                break;

            case EnemyState.Captured:
                if (target != player)
                {
                    target = player;
                }

                if (target != null)
                {
                    float dstToPlayer = Vector3.Distance(target.position, transform.position);

                    if (dstToPlayer <= agent.stoppingDistance)
                    {
                        agent.speed = 0;
                        agent.velocity = Vector3.zero;
                    }
                    else
                    {
                        agent.speed = speed;
                    }
                }
                break;

            case EnemyState.Aggro:
                if(aggroPoints.Count == 0)
                {
                    return;
                }

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
                if (roamingPoints.Count == 0)
                {
                    return;
                }

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
                if(target != player)
                {
                    target = player;
                }

                if (target != null)
                {
                    float dstToPlayer = Vector3.Distance(target.position, transform.position);

                    if (dstToPlayer <= dstThreshold)
                    {
                        agent.speed = 0;
                        agent.velocity = Vector3.zero;

                        AudioManager.instance.PlayClip("ForkliftCertified");
                    }
                    else
                    {
                        agent.speed = speed;
                    }
                }
                break;
        }

        if (target != null && !agent.pathPending && agent.remainingDistance < .5f)
        {
            Vector3 posV3 = new Vector3(target.position.x, transform.position.y, target.position.z);

            agent.SetDestination(posV3);
        }
    }

    EnemyState CheckAggroState()
    {
        float dst = Vector3.Distance(GameObject.FindGameObjectWithTag("Player").transform.position, transform.position);

        return (dst < aggroRange && dst > followRange) ? EnemyState.Aggro : EnemyState.Roaming;
    }

    EnemyState CheckFollowState()
    {
        float dst = Vector3.Distance(GameObject.FindGameObjectWithTag("Player").transform.position, transform.position);

        if(behaviourState == EnemyState.Aggro)
        {
            return dst < followRange ? EnemyState.Following : EnemyState.Aggro;
        }

        return dst < followRange ? EnemyState.Following : EnemyState.Roaming;
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

    void CheckBehaviourSettings()
    {
        switch(behaviourState)
        {
            case EnemyState.Following:
                agent.stoppingDistance = 0;
                break;

            case EnemyState.Roaming:
                agent.stoppingDistance = 0;
                break;

            case EnemyState.Aggro:
                agent.stoppingDistance = 0;
                break;

            case EnemyState.Captured:
                agent.stoppingDistance = stoppingDst;
                break;
        }
    }

    public void CheckCapture(ArtifactBehaviour.ArtifactElement element, ArtifactBehaviour behaviour)
    {
        if(isTamable)
        {
            switch (element)
            {
                case ArtifactBehaviour.ArtifactElement.None:
                    if (type == EnemyType.None)
                    {
                        behaviourState = EnemyState.Captured;
                        behaviour.hasCapture = true;
                        behaviour.capturedEnemy = this;
                    }
                    break;

                case ArtifactBehaviour.ArtifactElement.Death:
                    if (type == EnemyType.Death)
                    {
                        behaviourState = EnemyState.Captured;
                        behaviour.hasCapture = true;
                        behaviour.capturedEnemy = this;
                    }
                    break;
            }
        }
    }

    public void TakeDamage(int damage)
    {
        if (!invisFramesActive)
        {
            invisFramesActive = true;

            CheckHealth(damage);

            if (canInvokeInvisReset)
            {
                canInvokeInvisReset = false;

                Invoke("ResetInvisFrames", invisFrameTime);
            }
        }
    }

    void CheckHealth(int damage)
    {
        if ((currentHP -= damage) <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        Destroy(gameObject);
    }

    void ResetInvisFrames()
    {
        if (invisFramesActive)
        {
            invisFramesActive = false;
            canInvokeInvisReset = true;
        }
    }

    /*private void FindPath()
    {
        Profiler.BeginSample($"Finding path");

        switch(behaviourState)
        {
            case EnemyState.Captured:
                path = grid.FindPath(transform.position, player.transform.position);
                transform.transform.LookAt(player.transform);
                break;

            case EnemyState.Following:
                path = grid.FindPath(transform.position, player.transform.position);
                transform.transform.LookAt(player.transform);
                break;
        }    

        Profiler.EndSample();

        *//*if (enemyType == EnemyTypes.flyEnemy)
        {
            Profiler.BeginSample("Lifting path up in the air for flying enemies");
            float averageHeight = 0;
            foreach (Vector3 waypoint in path)
            {
                averageHeight += waypoint.y;
            }

            averageHeight /= path.Count;

            for (int i = 0; i < path.Count; i++)
            {
                if (path[i].y > averageHeight)
                {
                    averageHeight = path[i].y + (flyEnemyFlightHeight / 3);
                }
            }

            for (int i = 0; i < path.Count; i++)
            {
                //path[i] = new Vector3(path[i].x, averageHeight + flyEnemyFlightHeight, path[i].z);
                Vector3 newWaypoint;
                newWaypoint.x = path[i].x;
                newWaypoint.y = averageHeight + flyEnemyFlightHeight;
                newWaypoint.z = path[i].z;
                path[i] = newWaypoint;
            }

            Profiler.EndSample();
        }*//*
    }*/

    void InitializeUnit()
    {
        GameObject[] roamingObjects = GameObject.FindGameObjectsWithTag("RoamingPoint");

        foreach (GameObject rp in roamingObjects)
        {
            roamingPoints.Add(rp.transform);
        }

        if(behaviourState != EnemyState.Forklift)
        {
            behaviourState = EnemyState.Roaming;
        }

        previousBehaviourState = behaviourState;

        currentHP = maxHP;

        agent.speed = speed;
        agent.radius = obstacleAvoidanceRadius;
        agent.acceleration = acceleration;
        agent.autoBraking = false;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, aggroRange);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, followRange);
    }
}
