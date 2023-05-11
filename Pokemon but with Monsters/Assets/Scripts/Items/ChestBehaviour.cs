using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ChestBehaviour : MonoBehaviour
{
    public Transform player;
    public Transform chestTop;

    public float range = 2f;

    private void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    private void Start()
    {
        
    }

    private void Update()
    {
        if (Vector3.Distance(transform.position, player.position) <= range)
        {
            Quaternion targetRotation = Quaternion.Euler(-110f, chestTop.rotation.eulerAngles.y, chestTop.rotation.eulerAngles.z);

            chestTop.rotation = Quaternion.Lerp(chestTop.rotation, targetRotation, 2.5f * Time.deltaTime);

            if(Mathf.Approximately(chestTop.rotation.eulerAngles.x, 270f))
            {
                range = 0;
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(new Vector3(transform.position.x, transform.position.y + .25f, transform.position.z), range);
    }
}
