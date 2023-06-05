using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectDisabler : MonoBehaviour
{
    public Transform player;

    public GameObject disableObj;

    public float disableRange;

    public bool isRemovable;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    // Update is called once per frame
    void Update()
    {
        float dst = Vector3.Distance(transform.position, player.position);

        if (disableObj == null)
        {
            Destroy(GetComponent<ObjectDisabler>());
        }else
        {
            if (dst > disableRange)
            {
                disableObj.SetActive(false);
            }
            else
            {
                disableObj.SetActive(true);
            }
        }
    }
}
