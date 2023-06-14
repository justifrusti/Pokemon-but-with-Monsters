using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public SaveData saveData;

    public PlayerController pc;

    private void Update()
    {
        if(pc == null)
        {
            GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        }
    }
}
