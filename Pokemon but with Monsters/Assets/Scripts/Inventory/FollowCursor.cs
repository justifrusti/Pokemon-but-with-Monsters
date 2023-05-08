using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FollowCursor : MonoBehaviour
{
    public Image hand;

    void Update()
    {
        Vector2 mouse = new Vector2(Input.mousePosition.x, Input.mousePosition.y);

        hand.transform.position = mouse;
    }
}
