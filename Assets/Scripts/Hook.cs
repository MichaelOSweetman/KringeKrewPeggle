using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
    File name: Hook.cs
    Summary: Manages collision detection of the hook used in Loki Power
    Creation Date: 18/03/2024
    Last Modified: 17/02/2025
*/
public class Hook : MonoBehaviour
{
    public LokiPower m_lokiPower;

    private void OnTriggerEnter2D(Collider2D a_collision)
    {
        // if the hook has collided with a peg
        if (a_collision.GetComponent<Peg>())
        {
            // connect the hook to the peg
            m_lokiPower.ConnectHook(a_collision.gameObject);
        }
    }
}
