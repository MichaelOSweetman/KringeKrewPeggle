using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
    File name: PlayerControls.cs
    Summary: Manages the player's ability to shoot the ball and speed up time, as well as to make use of the different powers
    Creation Date: 01/10/2023
    Last Modified: 30/11/2025
*/
public class PlayerControls : MonoBehaviour
{
    [HideInInspector] public bool m_setUpPowerNextTurn = false;
    [HideInInspector] public bool m_resolvePowerNextTurn = false;
    [HideInInspector] public bool m_ballInPlay = false;
    [HideInInspector] public GreenPegPower m_power;

    [Header("Other Scripts")]
    public UIManager m_UIManager;
    public PegManager m_pegManager;
    public CameraZoom m_cameraZoom;
    public BallTrajectory m_ballTrajectory;

    [Header("Play Area Bounds")]
    public Transform m_playAreaBounds;
    Vector2 m_boundsBottomLeft;
    Vector2 m_boundsTopRight;

    [Header("Ball")]
    public GameObject m_ballPrefab;
    public Transform m_playerProjectilesContainer;
    public float m_ballLaunchSpeed;
    public byte m_startingBallCount = 10;
    public float m_ballKillFloor = -7.0f;
    [HideInInspector] public GameObject m_ball = null;
    [HideInInspector] public int m_ballCount = 0;

    [Header("Time Scale")]
    public float m_spedUpTimeScale = 5.0f;
    public float m_nearVictoryTimeScale = 0.25f;
    [HideInInspector] public float m_defaultTimeScale = 1.0f;
    [HideInInspector] float m_defaultDeltaTime = 0.02f;

    [Header("Audio")]
    public AudioClip[] m_freeBallSounds;

    public void RemoveProjectile(GameObject a_projectile)
    {
        // set the projectile to have no parent
        a_projectile.transform.parent = null;
        // destroy the projectile
        Destroy(a_projectile);
        // if the projectile count is now equal to 0
        if (m_playerProjectilesContainer.childCount == 0)
        {
            // if the ball count is over 0
            if (m_ballCount > 0)
            {
                // resolve the turn
                ResolveTurn();
            }
            // if the player has run out of balls
            else
            {
                // destroy the ball
                Destroy(m_ball);
                // tell the UI Manager that the level is over and the player failed
                m_UIManager.LevelOver(false);
            }
        }
    }

    public void SetUpTurn()
    {
        // enable the ball trajectory line
        m_ballTrajectory.ShowLine(true);

        // store that the ball is not in play
        m_ballInPlay = false;

        // if the power should be set up
        if (m_setUpPowerNextTurn)
        {
            // set up the power
            m_power.SetUp();
            // store that the power has been set up
            m_setUpPowerNextTurn = false;
        }

        // if the power should be resolved
        if (m_resolvePowerNextTurn)
        {
            // resolve the power
            m_power.ResolvePower();
            // store that the power has been resolved
            m_resolvePowerNextTurn = false;
        }
    }

    public bool CursorWithinPlayArea()
    {
        // get the coordinates of the current mouse position in world space
        Vector3 worldSpaceMousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        // return true if the following is true
        return
        (
        // the cursor is further right than the bottom left point of the bounds
        worldSpaceMousePosition.x >= m_boundsBottomLeft.x &&
        // the cursor is further up than the bottom left point of the bounds
        worldSpaceMousePosition.x <= m_boundsTopRight.x &&
        // the cursor is not further right than the top right point of the bounds
        worldSpaceMousePosition.y >= m_boundsBottomLeft.y &&
        // the cursor is not further up than the top right point of the bounds
        worldSpaceMousePosition.y <= m_boundsTopRight.y
        );
    }

    GameObject Shoot()
    {
        // switch the time scale back to default
        ModifyTimeScale();

        // create a copy of the ball prefab and put it in the player projectiles container
        GameObject Ball = Instantiate(m_ballPrefab, m_playerProjectilesContainer) as GameObject;
		// set its position to be the same as this game object
		Ball.transform.position = transform.position;
		// apply the launch speed force to the ball, in the direction this gameobject is facing
		Ball.GetComponent<Rigidbody2D>().AddForce(transform.up * m_ballLaunchSpeed, ForceMode2D.Impulse);
		// give the ball the peg manager
		Ball.GetComponent<Ball>().m_pegManager = m_pegManager;
		// give the ball this component
		Ball.GetComponent<Ball>().m_playerControls = this;

        // reduce the ball count by one as a ball has been expended
        --m_ballCount;
        // have the UI Manager update the ball count text
        m_UIManager.UpdateBallCountText();
        // return the ball gameobject
        return Ball;
    }

    public void ResolveTurn()
    {
        // tell the peg manager to clear all the hit pegs. If there were no pegs to clear give the player a 50% chance to get back a free ball
        if (!m_pegManager.ClearHitPegs() && Random.Range(0,2) == 1)
        {
            // give the player a free ball without playing the free ball sound
            FreeBall(false, false);
        }

        // tell the camera to return to default in case it had moved while the ball was in play
        m_cameraZoom.ReturnToDefault();
        // tell the peg manager to resolve the turn
        m_pegManager.ResolveTurn();
        // tell the power to resolve the turn
        m_power.ResolveTurn();

        // set up the next turn
        SetUpTurn();
    }

    public void FreeBall(bool a_playSound = true, bool a_showFreeBallText = false)
    {
        // increase the ball count by 1 as a ball has been gained
        ++m_ballCount;

        // if the free ball sound effect should be played
        if (a_playSound)
        {
            // play the free ball sound that corresponds to the amount of free balls earned this round, using the sound effect volume
            AudioSource.PlayClipAtPoint(m_freeBallSounds[m_pegManager.m_freeBallsAwarded], Vector3.zero, GlobalSettings.m_soundEffectVolume);
        }

        // if the free ball text should shown
        if (a_showFreeBallText)
        {
            // have the UI manager show the free ball text
            m_UIManager.DisplayFreeBallText();
        }
        // if the free ball text should not be shown
        else
        {
            // have the UI manager update the text with the new ball count value
            m_UIManager.UpdateBallCountText();
        }
    }

    public void ModifyTimeScale(float a_newTimeScale = -1.0f)
    {
        // set the time scale to the new value (use the default time scale if the value is below 0 or if no argument value has been provided)
        Time.timeScale = (a_newTimeScale < 0.0f) ? m_defaultTimeScale: a_newTimeScale;
        // ensure fixedUpdate is called with the same frequency regardless of time scale so physics remains smooth
        Time.fixedDeltaTime = m_defaultDeltaTime * Time.timeScale;
    }

    public void Reload()
    {
        // reset the ball count
        m_ballCount = m_startingBallCount;

        // reload the power if it exists
        if (m_power != null)
        {
            m_power.Reload();
        }
		
		// destroy the ball if one exists
		if (m_ball != null)
		{
		    Destroy(m_ball);
		}
		
		// reset the time scale
		ModifyTimeScale();

        // set up the turn
        SetUpTurn();
    }

    void Awake()
    {
        // initialise default time variables
        m_defaultTimeScale = Time.timeScale;
        m_defaultDeltaTime = Time.fixedDeltaTime;

        // initialise the ball count
        m_ballCount = m_startingBallCount;

        // store the bottom left and top right coordinates of the play area bounds
        m_boundsBottomLeft = new Vector2(m_playAreaBounds.position.x - m_playAreaBounds.lossyScale.x * 0.5f, m_playAreaBounds.position.y - m_playAreaBounds.lossyScale.y * 0.5f);
        m_boundsTopRight   = new Vector2(m_playAreaBounds.position.x + m_playAreaBounds.lossyScale.x * 0.5f, m_playAreaBounds.position.y + m_playAreaBounds.lossyScale.y * 0.5f);

        // TEMP
        // if the green peg power has not been set
        //if (m_greenPegPower == null)
        //{
        //    // set it to the default power for the stage
        //    SetGreenPegPower(m_pegManager.m_stages[GlobalSettings.m_currentStageID].m_defaultPowerID);
        //}

        // TEMP
        //m_power.Initialize();

    }

    private void FixedUpdate()
    {
        // if the ball is not currently in play and the ball trajectory line is currently active
        if (!m_ballInPlay && m_ballTrajectory.enabled)
        {
            // draw a line to show the expected trajectory of the ball, using the speed at which the ball will be launched from the launcher
            m_ballTrajectory.CreateTrajectoryLine(m_ballLaunchSpeed);
        }
    }

    void Update()
    {
        // TEMP
        if (Input.GetButton("Speed Up Time"))
        {
            // change the timescale to the sped up timescale
            ModifyTimeScale(m_spedUpTimeScale);
        }
        // otherwise, if the Speed Up Time button has been released
        else if (Input.GetButtonUp("Speed Up Time"))
        {
            // reset the timescale
            ModifyTimeScale();
        }
        // TEMP

        // if the ball is in play
        if (m_ballInPlay)
        {
            // if the ball exists, trigger the power's ball removal check function. If it does not override the default ball removal check and the ball has fallen low enough
            if (m_ball != null && !m_power.BallRemovalCheck(m_ball) && m_ball.transform.position.y <= m_ballKillFloor)
            {
                // remove the ball from play
                RemoveProjectile(m_ball);
            }
            
        }
        // if the ball does not exist, allow the player to shoot a ball
        else
        {
            // if the Shoot / Use Power input has been detected and the cursor is within the play area
            if (Input.GetButtonDown("Shoot / Use Power") && CursorWithinPlayArea())
            {
                // trigger the power's on shoot function. If it should not override the default shoot function
                if (!m_power.OnShoot())
                {
                    // shoot a ball
                    m_ball = Shoot();
                }

                // store that the ball is now in play
                m_ballInPlay = true;

                // disable the ball trajectory line
                m_ballTrajectory.ShowLine(false);
            }
        }
    }
}
