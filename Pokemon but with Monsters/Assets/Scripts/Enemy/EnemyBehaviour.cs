using Pathfinding;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(AIDestinationSetter))]
[RequireComponent(typeof(AIPath))]
[RequireComponent(typeof(Seeker))]
public class EnemyBehaviour : MonoBehaviour
{
    public enum EnemyState
    {
        Roaming,
        Following,
        Aggro,
        Captured
    };

    public enum EnemyType
    {
        None,
        Death,
        Fire
    }

    public enum Enemy
    {
        FleshBlob,
        WheepingAngel,
        Hellspore
    }

    public EnemyType type;
    public EnemyState behaviourState;
    public Enemy behaviour;
    [Space]
    public Transform target;
    public float pathRefreshRate = 1.0f;
    [Space]
    public int damage;
    public int maxHP;
    public float invisFrameTime;
    [Space]
    public float speed = 5;
    [Tooltip("The distance required to trigger a new point or action")]public float dstThreshold;
    [Space]
    public float aggroRange;
    public float followRange;
    [Tooltip("The time between the updates of the list of aggro points")]public float aggroUpdateSpeed;
    [Space]
    public bool isTamable;

    [Header("Ranged Components")]
    public float shootRange;
    public float shootDelay;
    public float shootForce;
    [Space]
    public GameObject shootBullet;
    [Space]
    public Transform bulletPoint;
    [Tooltip("The point the bulletpoint rotates around for 360 degrees shooting")]public Transform bulletRotPoint;
    public GameObject deathParticle;

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
    private float pathRefreshTimer = 0f;
    private AIDestinationSetter pathSetter;
    private bool canShoot = true;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;

        InitializeUnit();
    }

    private void Update()
    {
        pathSetter.target = target;

        if(transform.position.y < -20)
        {
            transform.position = new Vector3(transform.position.x, 20, transform.position.z);
        }

        if(navMeshBuild)
        {
            CheckEnemySpecificTrait();

            if(behaviourState != previousBehaviourState)
            {
                previousBehaviourState = behaviourState;

                target = null;
            }

            if(behaviourState != EnemyState.Captured)
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

            PlayerInShootRange();
        }
    }

    void CheckEnemySpecificTrait()
    {
        switch(behaviour)
        {
            case Enemy.WheepingAngel:
                if(inView)
                {
                    //agent.speed = 0;
                }else
                {
                    //agent.speed = speed;
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
                break;

            case EnemyState.Captured:
                if (target != player)
                {
                    target = player;
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

    public void PlayerInShootRange()
    {
        if(behaviourState != EnemyState.Captured)
        {
            float dstToPlayer = Vector3.Distance(player.position, transform.position);

            if (dstToPlayer <= shootRange && canShoot)
            {
                canShoot = false;

                StartCoroutine(Shoot());
            }
        }else
        {
            float dstToEnemy = Vector3.Distance(player.position, transform.position);

            if (dstToEnemy <= shootRange && canShoot)
            {
                canShoot = false;

                StartCoroutine(Shoot());
            }
        }
    }

    IEnumerator Shoot()
    {
        bulletRotPoint.LookAt(player.position);

        GameObject spawnedBullet = Instantiate(shootBullet, bulletPoint.position, Quaternion.identity);

        spawnedBullet.GetComponent<Rigidbody>().AddForce(bulletPoint.forward * shootForce, ForceMode.Impulse);
        spawnedBullet.GetComponent<EnemyBullet>().AssignMasterBehaviour(this);

        yield return new WaitForSeconds(shootDelay);

        canShoot = true;
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
        if(behaviour == Enemy.Hellspore)
        {
            Instantiate(deathParticle, transform.position, Quaternion.identity);

            Invoke("DestroyInTime", .1f);
        }else
        {
            Destroy(gameObject);
        }
    }

    void DestroyInTime()
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

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.CompareTag("Player"))
        {
            collision.gameObject.GetComponent<PlayerController>().TakeDamage(damage);
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

        if(behaviour == Enemy.Hellspore && !canShoot)
        {
            canShoot = true;
        }

        previousBehaviourState = behaviourState;

        currentHP = maxHP;

        if(pathSetter == null)
        {
            pathSetter = GetComponent<AIDestinationSetter>();
        }

        GetComponent<AIPath>().maxSpeed = speed;
        GetComponent<AIPath>().slowdownDistance = dstThreshold;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, aggroRange);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, followRange);
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, shootRange);
    }
}
