using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
    File name: PlayerControls.cs
    Summary: Manages the player's ability to shoot the ball and speed up time, as well as to make use of the different powers
    Creation Date: 01/10/2023
    Last Modified: 02/10/2023
*/
public class PlayerControls : MonoBehaviour
{
    
    [Header("Ball")]
    public GameObject m_ballPrefab;
    public float m_ballLaunchSpeed;
    public byte m_startingBallCount = 10;
    public float m_ballKillFloor = -7.0f;
    GameObject m_ball = null;

    [Header("Time Scale")]
    [HideInInspector] public float m_timeScale;
    public float m_spedUpTimeScale = 5.0f;
    GameObject Shoot()
    {
        // create the a copy of the ball prefab
        GameObject Ball = Instantiate(m_ballPrefab) as GameObject;
        // set its position to be the same as this game object
        Ball.transform.position = transform.position;
        // apply the launch speed force to the ball, in the direction this gameobject is facing
        Ball.GetComponent<Rigidbody2D>().AddForce(transform.up * m_ballLaunchSpeed, ForceMode2D.Impulse);
        // return the ball gameobject
        return Ball;
    }

    void Start()
    {

    }

    void Update()
    {
        // reset the time scale
        m_timeScale = 1.0f;

        // if there is a ball in play
        if (m_ball)
        {
            // if the ball has fallen low enough
            if (m_ball.transform.position.y <= m_ballKillFloor)
            {
                // destroy it
                Destroy(m_ball);
            }
        }
        // if there is not a ball in play
        else
        {
            // if the Shoot / Use Power input has been detected
            if (Input.GetButtonDown("Shoot / Use Power"))
            {
                // shoot a ball
                m_ball = Shoot();
            }

            // if the Speed Up Time input has been detected
            if (Input.GetButton("Speed Up Time"))
            {
                // change the timescale to the sped up timescale
                m_timeScale = m_spedUpTimeScale;
            }
        }
    }
}
