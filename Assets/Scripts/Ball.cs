using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
    File name: Ball.cs
    Summary: Prevents the ball from getting stuck
    Creation Date: 30/10/2023
    Last Modified: 04/05/2025
*/
public class Ball : MonoBehaviour
{
    [HideInInspector] public PegManager m_pegManager;
    [HideInInspector] public PlayerControls m_playerControls;
    Rigidbody2D m_rigidbody;
    public float m_lowestAllowedVelocitySquared = 0.01f;
    public float m_maxLowVelocityDuration = 1.0f;
    float m_timer = 0.0f;

    void Awake()
    {
        // get the rigidbody2D component
        m_rigidbody = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        // if the ball's velocity is too low
        if (m_rigidbody.velocity.sqrMagnitude < m_lowestAllowedVelocitySquared)
        {
            // increase the timer
            m_timer += Time.deltaTime;

            // if the max time that the ball can be low velocity has been reached
            if (m_timer >= m_maxLowVelocityDuration)
            {
                // have the Peg Manager clear the hit pegs
                m_pegManager.ClearHitPegs();
                // reset the timer
                m_timer = 0.0f;
            }
        }
        // if the ball's velocity is high enough
        else
        {
            // reset the timer
            m_timer = 0.0f;
        }
    }
}
