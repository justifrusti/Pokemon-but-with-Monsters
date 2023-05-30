using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBehaviour : MonoBehaviour
{
    public enum EnemyState
    {
        Roaming,
        Following,
        Aggro,
        Captured
    };

    public EnemyState behaviourState;

    public Transform target;

    public float speed = 5;

    private void Start()
    {

    }

    private void Update()
    {
        if (Time.timeSinceLevelLoad > 3.0f)
        {
            UpdateState();
        }
    }

    void UpdateState()
    {
        switch (behaviourState)
        {
            case EnemyState.Following:
                target = GameObject.FindGameObjectWithTag("Player").transform;
                break;
        }
    }
}
