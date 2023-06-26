using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArtifactBehaviour : MonoBehaviour
{
    public enum ArtifactType
    {
        Capture,
        Weapon,
        Utility,
        Other
    }

    public enum ArtifactElement
    {
        None,
        Death
    }

    public enum ArtifactUtilityType
    {
        None,
        Hourglass,
        MonsterDetector
    }

    public enum AmuletType
    {
        None,
        Health,
        Stealth,
        Speed
    }

    public enum WeaponType
    {
        None,
        Boomerang
    }

    public ArtifactType artifactType;

    public string artifactName;

    [Header("Capture Artifact Settings")]
    public ArtifactElement element;
    [Space]
    public float useRange = 5f;
    [Space]
    public bool hasCapture;
    public EnemyBehaviour capturedEnemy;

    [Header("Utility Artifact Settings")]
    [Tooltip("The position the hourglass moves you to when used")]public Vector3 playerStartPos = new Vector3(0, 1, 0);
    [Space]
    public ArtifactUtilityType utilityType;
    [Space]
    [Tooltip("The time it takes to recheck the closest monster")]public float utilityCheckTime;
    [Space]
    public List<Transform> enemies;
    [Space]
    public Gradient detectorGradient;
    public Material matToChange;
    public Material matToChangeDetector;
    public Light detectorLight;

    private Transform player;
    private PlayerController controller;
    private Transform closestEnemyPos;

    private float currentUtilityCheckTime;

    [Header("Amulets")]
    public AmuletType amuletType;

    [Header("Weapon Artifact Settings")]
    public WeaponType weaponType;
    [Space]
    public int damage;
    [Space]
    public GameObject boomerangSpawnInstance;
    [Space]
    public float throwForce;

    [HideInInspector] public bool isEquiped = false;
    [HideInInspector] public Rigidbody rb;
    [HideInInspector] public bool canSpawnBoomerang = true;
    [HideInInspector] public bool isCopy = false;
    [HideInInspector] public bool startedDeleteSequence = false;

    private void Start()
    {
        Initialize();
    }

    private void Update()
    {
        if (isCopy && !startedDeleteSequence)
        {
            startedDeleteSequence = true;

            Invoke("DestroyArtifactAfterTime", 5.0f);
        }

        if (matToChange == null || matToChangeDetector == null)
        {
            if (artifactType == ArtifactType.Utility && utilityType == ArtifactUtilityType.MonsterDetector)
            {
                GameManager.instance.GetDetectorMat(matToChange, matToChangeDetector);
            }
        }

        if(controller.currentItem != null)
        {
            if (utilityType == ArtifactUtilityType.MonsterDetector)
            {
                if (currentUtilityCheckTime <= 0)
                {
                    currentUtilityCheckTime = utilityCheckTime;

                    closestEnemyPos = CheckClosestMonster(transform.position, enemies);
                }
                else
                {
                    currentUtilityCheckTime -= Time.deltaTime;
                }

                float dst = 0;

                if (closestEnemyPos != null)
                {
                    dst = Vector3.Distance(player.position, closestEnemyPos.position);
                }

                if(dst < closestEnemyPos.GetComponent<EnemyBehaviour>().aggroRange)
                {
                    float t = dst / closestEnemyPos.GetComponent<EnemyBehaviour>().aggroRange;

                    if(t < closestEnemyPos.GetComponent<EnemyBehaviour>().aggroRange)
                    {
                        Color emissionColor = detectorGradient.Evaluate(t);

                        matToChange.SetColor("_EmissionColor", emissionColor);
                        matToChange.EnableKeyword("_EMISSION");

                        matToChangeDetector.SetColor("_EmissionColor", emissionColor);
                        matToChangeDetector.EnableKeyword("_EMISSION");

                        detectorLight.color = emissionColor;
                    }
                }
            }

            if (Input.GetButtonDown("LMB") && controller.currentItem.itemName == artifactName)
            {
                if (artifactType == ArtifactType.Capture && !hasCapture)
                {
                    foreach (Collider c in Physics.OverlapSphere(transform.position, useRange))
                    {
                        EnemyBehaviour b = c.gameObject.GetComponent<EnemyBehaviour>();

                        if (b != null)
                        {
                            b.CheckCapture(element, this);
                        }
                    }
                }

                if(utilityType == ArtifactUtilityType.Hourglass)
                {
                    player.position = playerStartPos;
                }
            }

            if (Input.GetButtonDown("RMB") && controller.currentItem.itemName == artifactName)
            {
                if (artifactType == ArtifactType.Capture && hasCapture)
                {
                    capturedEnemy.behaviourState = EnemyBehaviour.EnemyState.Roaming;
                    capturedEnemy = null;
                    hasCapture = false;
                }
            }

            if(Input.GetButtonDown("LMB") && controller.currentItem.itemName == artifactName)
            {
                if(!isCopy)
                {
                    if (artifactType == ArtifactType.Weapon && weaponType == WeaponType.Boomerang)
                    {
                        GameObject boomerangInstance = Instantiate(boomerangSpawnInstance, player.GetComponent<PlayerController>().holdingHand.position, transform.rotation);
                        boomerangInstance.GetComponent<ArtifactBehaviour>().isCopy = true;
                        boomerangInstance.gameObject.AddComponent<Rigidbody>();

                        boomerangInstance.GetComponent<ArtifactBehaviour>().ThrowBoomerang();
                    }
                }
            }
        }
    }

    Transform CheckClosestMonster(Vector3 startPosition, List<Transform> enemiesList)
    {
        Transform closestMonster = null;
        float closestDstSqr = Mathf.Infinity;

        foreach (Transform potentionalTarget in enemiesList)
        {
            Vector3 dirToTarget = potentionalTarget.transform.position - startPosition;
            float dirSqrToTarget = dirToTarget.sqrMagnitude;

            if(dirSqrToTarget < closestDstSqr)
            {
                closestDstSqr = dirSqrToTarget;
                closestMonster = potentionalTarget;
            }
        }

        return closestMonster;
    }

    public void RegisterEnemies()
    {
        foreach (GameObject go in GameObject.FindGameObjectsWithTag("Enemy"))
        {
            enemies.Add(go.transform);
        }
    }

    public void ThrowBoomerang()
    {
        if(canSpawnBoomerang && isCopy)
        {
            canSpawnBoomerang = false;

            rb = GetComponent<Rigidbody>();

            rb.AddForce(-transform.forward * throwForce, ForceMode.Impulse);

            Invoke("ResetSpawnBool", 1.5f);
        }
    }

    void Initialize()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        controller = player.GetComponent<PlayerController>();
        rb = GetComponent<Rigidbody>();

        canSpawnBoomerang = true;

        if(artifactType == ArtifactType.Utility && utilityType == ArtifactUtilityType.MonsterDetector)
        {
            InitializedDetectorColor();

            GameManager.instance.RegisterDetectorMat(matToChange, matToChangeDetector);
        }

        if(Time.timeSinceLevelLoad < 3.0f)
        {
            Invoke("RegisterEnemies", 5.0f);
        }else
        {
            RegisterEnemies();
        }
    }

    void InitializedDetectorColor()
    {
        Color emissionColor = new Color(0, 255, 0);

        matToChange.SetColor("_EmissionColor", emissionColor);
        matToChange.EnableKeyword("_EMISSION");

        matToChangeDetector.SetColor("_EmissionColor", emissionColor);
        matToChangeDetector.EnableKeyword("_EMISSION");

        detectorLight.color = emissionColor;
    }

    public void ResetSpawnBool()
    {
        canSpawnBoomerang = true;
    }

    public void DestroyArtifactAfterTime()
    {
        DestroyObj();
    }

    public void DestroyObj()
    {
        Destroy(this.gameObject);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position, useRange);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (artifactType == ArtifactType.Weapon && weaponType == WeaponType.Boomerang)
        {
            if (isCopy)
            {
                if (other.gameObject.CompareTag("Enemy"))
                {
                    other.gameObject.GetComponent<EnemyBehaviour>().TakeDamage(damage);
                }
            }
        }
    }
}
