using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
    File name: Hook.cs
    Summary: Manages collision detection of the hook used in Loki Power
    Creation Date: 18/03/2024
    Last Modified: 18/03/2024
*/
public class Hook : MonoBehaviour
{
    public PlayerControls m_playerControls;

    private void OnTriggerEnter2D(Collider2D a_collision)
    {
        // if the hook has collided with a peg
        if (a_collision.GetComponent<Peg>())
        {
            // connect the hook to the peg
            m_playerControls.ConnectHook(a_collision.gameObject);
        }
    }
}
