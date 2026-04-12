using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
    File name: BallInBucket.cs
    Summary: Grants the player a free ball upon colliding with it
    Creation Date: 09/10/2023
    Last Modified: 13/04/2026
*/
public class BallInBucket : MonoBehaviour
{
    public GameManager m_gameManager;

    private void OnTriggerEnter2D(Collider2D a_collision)
    {
        // if the object that entered this trigger is the ball
        if (a_collision.CompareTag("Ball"))
        {
            // grant the player a free ball, removing the chance to gain another from hitting 0 pegs
            m_gameManager.FreeBall(true, false, false);
            // have the game manager remove the ball
            m_gameManager.RemoveProjectile(a_collision.gameObject);
        }
    }
}
