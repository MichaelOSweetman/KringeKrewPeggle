using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
    File name: Ball.cs
    Summary: Prevents the ball from getting stuck
    Creation Date: 30/10/2023
    Last Modified: 24/08/2026
*/
public class Ball : MonoBehaviour
{
    [HideInInspector] public GameManager m_gameManager;
    Rigidbody2D m_rigidbody;
    public float m_ballKillFloor = -7.0f;
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
        // trigger the power's ball removal check function. If it does not override the default ball removal check and the ball has fallen low enough
        if (!m_gameManager.m_magicPower.BallRemovalCheck(this) && transform.position.y <= m_ballKillFloor)
        {
            // have the game manager remove the ball from play
            m_gameManager.RemoveProjectile(gameObject);
        }

        // if the ball's velocity is too low
        if (m_rigidbody.velocity.sqrMagnitude < m_lowestAllowedVelocitySquared)
        {
            // increase the timer
            m_timer += Time.deltaTime;

            // if the max time that the ball can be low velocity has been reached
            if (m_timer >= m_maxLowVelocityDuration)
            {
                // have the Peg Manager clear the hit pegs
                m_gameManager.m_pegManager.ClearHitPegs();
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
