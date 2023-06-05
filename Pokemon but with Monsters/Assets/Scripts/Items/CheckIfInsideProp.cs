using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckIfInsideProp : MonoBehaviour
{
    public GameObject collisionObj;

    float timeSinceSpawn;

    private void Update()
    {
        if(timeSinceSpawn <= 3.00f)
        {
            timeSinceSpawn += Time.deltaTime;
        }else
        {
            Destroy(GetComponent<CheckIfInsideProp>());
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collisionObj.GetComponent<Collision>().gameObject.CompareTag("Prop"))
        {
            Destroy(gameObject);
        }
    }
}
