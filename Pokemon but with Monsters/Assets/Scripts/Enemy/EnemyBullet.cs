using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class EnemyBullet : MonoBehaviour
{
    private EnemyBehaviour masterBehaviour;
    private Rigidbody rb;

    public void AssignMasterBehaviour(EnemyBehaviour behaviour)
    {
        masterBehaviour = behaviour;

        rb = GetComponent<Rigidbody>();
    }

    private void OnTriggerEnter(Collider other)
    {
        rb.velocity = Vector3.zero;
        
        if(other.gameObject.CompareTag("Player"))
        {
            other.gameObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
            other.gameObject.GetComponent<PlayerController>().TakeDamage(masterBehaviour.damage);
        }

        Destroy(this.gameObject);
    }
}
