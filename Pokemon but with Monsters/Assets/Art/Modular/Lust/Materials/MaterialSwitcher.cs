using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MaterialSwitcher : MonoBehaviour
{
    public Material[] materialToSwitch;

    private void Start()
    {
        gameObject.GetComponent<Renderer>().material = materialToSwitch[Random.Range(0, materialToSwitch.Length)];
    }
}
