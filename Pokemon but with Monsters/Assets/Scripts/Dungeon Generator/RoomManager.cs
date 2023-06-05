using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class RoomManager : MonoBehaviour
{
    [Header("Room Buildup")]
    public GameObject[] walls; 
    public GameObject[] doorways;

    [Header("Prop Spawning")]
    public Prop[] spawnableProps;
    [Space]
    public Transform propPos;
    [Space]
    public int minProps;
    public int maxProps;
    [Space]
    public bool canSpawnProps;

    //privates
    private int spawnedProps;

    [System.Serializable]
    public class Prop
    {
        public GameObject prop;

        [Range(0.00f, 1.00f)] public float spawnChance;

        [Tooltip("The minimum room spawn position (-2.5 in this project)")]public Vector3 minPos;
        [Tooltip("The maximum room spawn position (2.5 in this project)")] public Vector3 maxPos;
    }

    private void Start()
    {
        if(canSpawnProps)
        {
            int props = Random.Range(minProps, maxProps + 1);

            while (spawnedProps < props)
            {
                int index = Random.Range(0, spawnableProps.Length);

                float posX = Random.Range(spawnableProps[index].minPos.x, spawnableProps[index].maxPos.x);
                float posZ = Random.Range(spawnableProps[index].minPos.z, spawnableProps[index].maxPos.z);

                Vector3 spawnPos = new Vector3(posX, 0, posZ);

                if (Random.value < spawnableProps[index].spawnChance)
                {
                    GameObject go = Instantiate(spawnableProps[index].prop, propPos.position + spawnPos, Quaternion.identity);
                }

                spawnedProps++;
            }
        }
    }

    public void UpdateRoom(bool[] status)
    {
        for (int i = 0; i < status.Length; i++)
        {
            doorways[i].SetActive(status[i]);
            walls[i].SetActive(!status[i]);
        }
    }
}
