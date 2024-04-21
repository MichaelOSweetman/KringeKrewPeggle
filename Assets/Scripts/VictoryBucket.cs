using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
    File name: VictoryBucket.cs
    Summary: Adds to the player's score when the ball enters its space
    Creation Date: 06/11/2023
    Last Modified: 22/04/2024
*/

public class VictoryBucket : MonoBehaviour
{
    public UIManager m_UIManager;
    public PegManager m_pegManager;
    public int m_score = 0;

    private void OnTriggerEnter2D(Collider2D a_collision)
    {
        // if the object that entered this trigger is the ball
        if (a_collision.CompareTag("Ball"))
        {
            // add the score to the peg manager's total score
            m_pegManager.AddScore(m_score);
            // tell the UI manager that the level is over and that the player won
            m_UIManager.LevelOver(true);
        }
    }
}
