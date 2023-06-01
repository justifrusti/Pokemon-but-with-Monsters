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

    public ArtifactType artifactType;
    public ArtifactElement element;

    public float useRange = 5f;

    public bool hasCapture;
    public EnemyBehaviour capturedEnemy;

    private Transform player;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    private void Update()
    {
        if(Input.GetButtonDown("LMB") && artifactType == ArtifactType.Capture && !hasCapture)
        {
            foreach(Collider c in Physics.OverlapSphere(transform.position, useRange))
            {
                EnemyBehaviour b = c.gameObject.GetComponent<EnemyBehaviour>();

                if (b != null)
                {
                    b.CheckCapture(element, this);
                }
            }
        }

        if(Input.GetButtonDown("RMB") && hasCapture)
        {
            capturedEnemy.behaviourState = EnemyBehaviour.EnemyState.Roaming;
            capturedEnemy = null;
            hasCapture = false;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position, useRange);
    }
}
