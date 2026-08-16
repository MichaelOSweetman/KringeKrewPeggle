using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
    File name: PlayerControls.cs
    Summary: Manages the player's ability to shoot the ball and speed up time, as well as to make use of the different powers
    Creation Date: 01/10/2023
    Last Modified: 17/08/2026
*/
public class PlayerControls : MonoBehaviour
{
    public GameManager m_gameManager;
    public PegManager m_pegManager;
    public PlayAreaBounds m_playAreaBounds;
    public GameObject m_ballPrefab;
    public float m_ballLaunchSpeed;
    public float m_ballKillFloor = -7.0f;
    public float m_spedUpTimeScale = 5.0f;
    [HideInInspector] public GameObject m_ball = null;
    
    GameObject Shoot()
    {
        // create a copy of the ball prefab and put it in the player projectiles container
        GameObject Ball = Instantiate(m_ballPrefab, m_gameManager.m_playerProjectilesContainer);
		// set its position to be the same as this game object
		Ball.transform.position = transform.position;
		// apply the launch speed force to the ball, in the direction this gameobject is facing
		Ball.GetComponent<Rigidbody2D>().AddForce(transform.up * m_ballLaunchSpeed, ForceMode2D.Impulse);
		// give the ball the peg manager
		Ball.GetComponent<Ball>().m_pegManager = m_pegManager;

        // tell the game manager that the ball has been shot
        m_gameManager.OnShoot();

        // return the ball gameobject
        return Ball;
    }

    void Update()
    {
        // if the Toggle Menu button is pressed
        if (Input.GetButtonDown("Toggle Menu"))
        {
            // have the game manager toggle attempt to pause the game
            m_gameManager.TogglePause();
        }

        // if the game is not paused
        if (Time.timeScale > 0.0f)
        {
            // if the game state is Mid Shot
            if (m_gameManager.m_gameState == GameManager.GameState.MidShot)
            {
                // if the ball exists, trigger the power's ball removal check function. If it does not override the default ball removal check and the ball has fallen low enough
                if (m_ball != null && !m_gameManager.m_magicPower.BallRemovalCheck(m_ball) && m_ball.transform.position.y <= m_ballKillFloor)
                {
                    // have the game manager remove the ball from play
                    m_gameManager.RemoveProjectile(m_ball);
                }

            }
            // otherwise, if the game state is Shooting or Post Shot
            else if (m_gameManager.m_gameState == GameManager.GameState.Shooting || m_gameManager.m_gameState == GameManager.GameState.PostShot)
            {
                // if the Speed Up Time input is active
                if (Input.GetButtonDown("Speed Up Time"))
                {
                    // change the timescale to the sped up timescale
                    m_gameManager.ModifyTimeScale(m_spedUpTimeScale);
                }
                // if the Speed Up Time input is released
                else if (Input.GetButtonUp("Speed Up Time"))
                {
                    // reset the timescale
                    m_gameManager.ModifyTimeScale();
                }

                // if the game state is shooting
                if (m_gameManager.m_gameState == GameManager.GameState.Shooting)
                {
                    // if the Shoot / Use Power input has been detected and the cursor is within the play area
                    if (Input.GetButtonDown("Shoot / Use Power") && m_playAreaBounds.CursorWithinPlayArea())
                    {
                        // trigger the power's on shoot function. If it should not override the default shoot function
                        if (!m_gameManager.m_magicPower.OnShoot())
                        {
                            // shoot a ball
                            m_ball = Shoot();
                        }
                    }
                }
            }
        }
        // otherwise, if the game is paused and the Speed Up Time input is released
        else if (Input.GetButtonUp("Speed Up Time"))
        {
            // have the game manager return the time scale to the default time scale upon unpausing, rather than to the sped up time it would have had stored
            m_gameManager.ResetUnpausedTimeScale();
        }
    }
}
