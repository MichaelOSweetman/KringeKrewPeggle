using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
    File name: BallInBucket.cs
    Summary: Grants the player a free ball upon colliding with it
    Creation Date: 09/10/2023
    Last Modified: 30/11/2025
*/
public class BallInBucket : MonoBehaviour
{
    public PlayerControls m_playerControls;

    private void OnTriggerEnter2D(Collider2D a_collision)
    {
        // if the object that entered this trigger is the ball
        if (a_collision.CompareTag("Ball"))
        {
            // grant the player a free ball
            m_playerControls.FreeBall();
            // have the player controls remove the ball
            m_playerControls.RemoveProjectile(a_collision.gameObject);
        }
    }
}
