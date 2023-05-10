using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBehaviour : MonoBehaviour
{
    public enum EnemyState 
    { 
        Roaming, 
        Patrolling, 
        Following, 
        Aggro, 
        Captured 
    };

    public EnemyState behaviourState;
}
