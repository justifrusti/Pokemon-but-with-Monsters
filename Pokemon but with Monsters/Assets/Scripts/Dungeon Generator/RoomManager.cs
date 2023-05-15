using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class RoomManager : MonoBehaviour
{
    public DungeonGenerator generator;

    [Header("Room Buildup")]
    public GameObject[] walls; 
    public GameObject[] doorways;
    
    public void UpdateRoom(bool[] status)
    {
        for (int i = 0; i < status.Length; i++)
        {
            doorways[i].SetActive(status[i]);
            walls[i].SetActive(!status[i]);
        }
    }

    private void Start()
    {
        generator = GameObject.FindGameObjectWithTag("DungeonGenerator").GetComponent<DungeonGenerator>();
    }
}
