using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/*
    File name: PlayerControls.cs
    Summary: Manages the player's ability to shoot the ball and speed up time, as well as to make use of the different powers
    Creation Date: 01/10/2023
    Last Modified: 03/03/2025
*/
public class PlayerControls : MonoBehaviour
{
    public RectTransform m_playAreaBounds;

    [HideInInspector] public bool m_setUpPowerNextTurn = false;
    [HideInInspector] public bool m_resolvePowerNextTurn = false;
    [HideInInspector] public bool m_ballInPlay = false;
    // TEMP
    public GreenPegPower m_power;

    [Header("Other Scripts")]
    public UIManager m_UIManager;
    public PegManager m_pegManager;
    public CameraZoom m_cameraZoom;
    public LauncherRotation m_LauncherLookControls;
    public BallTrajectory m_ballTrajectory;

    [Header("UI")]
    public Canvas m_canvas;
    public Text m_ballCountText;
    public float m_freeBallTextDuration = 2.0f;
    float m_freeBallTextTimer = 0.0f;

    [Header("Ball")]
    public GameObject m_ballPrefab;
    public float m_ballLaunchSpeed;
    public byte m_startingBallCount = 10;
    public float m_ballKillFloor = -7.0f;
    [HideInInspector] public GameObject m_ball = null;
    [HideInInspector] public int m_ballCount = 0;

    [Header("Buckets")]
    public MoveToPoints m_bucket;
    public GameObject m_victoryBuckets;

    [Header("Time Scale")]
    public float m_spedUpTimeScale = 5.0f;
    public float m_nearVictoryTimeScale = 0.25f;
    [HideInInspector] public float m_defaultTimeScale = 1.0f;
    [HideInInspector] float m_defaultDeltaTime = 0.02f;

    [Header("Audio")]
    public AudioSource m_freeBallAudioSource;
    public AudioClip[] m_freeBallSounds;

    public void SetGreenPegPower(int a_characterID)
    {
        // TEMP
        //switch (a_characterID)
        //{
        //    case 0:
        //        m_greenPegPower = SweetsPower;
        //        break;
        //    case 1:
        //        m_greenPegPower = JackPower;
        //        break;
        //    case 2:
        //        m_greenPegPower = MatejaPower;
        //        break;
        //    case 3:
        //        m_greenPegPower = PhoebePower;
        //        break;
        //    case 4:
        //        m_greenPegPower = DanielPower;
        //        break;
        //    case 5:
        //        m_greenPegPower = KevinPower;
        //        break;
        //    case 6:
        //        m_greenPegPower = EthenPower;
        //        break;
        //    case 7:
        //        m_greenPegPower = LokiPower;
        //        break;
        //    case 8:
        //        m_greenPegPower = BenPower;
        //        break;
        //}
    }

	public void RemoveBall()
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

    public void SetUpTurn()
    {
        // enable the ball trajectory line
        m_ballTrajectory.ShowLine(true);

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

    GameObject Shoot()
    {
        // switch the time scale back to default
        ModifyTimeScale(m_defaultTimeScale);

        // create a copy of the ball prefab
        GameObject Ball = Instantiate(m_ballPrefab) as GameObject;
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
        // update the ball count text
        m_ballCountText.text = m_ballCount.ToString();
        // return the ball gameobject
        return Ball;
    }

    public void ResolveTurn()
    {
        // if the ball is in play
        if (m_ball != null)
        {
            // destroy it
            Destroy(m_ball);
        }

        // store that the ball is not in play
        m_ballInPlay = false;

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
            // play the free ball sound that corresponds to the amount of free balls earned this round 
            m_freeBallAudioSource.PlayOneShot(m_freeBallSounds[m_pegManager.m_freeBallsAwarded]);
        }

        // if the free ball text should shown
        if (a_showFreeBallText)
        {
            // update the ball count text with a message denoting that a free ball has been given
            m_ballCountText.text = "Free Ball!";
            // start a timer to determine how long the ballCountText should display the "Free Ball!" text
            m_freeBallTextTimer = m_freeBallTextDuration;
        }
        // otherwise, if the Free Ball Text is not currently being shown
        else if (m_freeBallTextTimer <= 0.0f)
        {
            // update the ball count text with the amount of balls available
            m_ballCountText.text = m_ballCount.ToString();
        }
    }

    public void ModifyTimeScale(float a_newTimeScale)
    {
        // set the time scale to the new value
        Time.timeScale = a_newTimeScale;
        // ensure fixedUpdate is called with the same frequency regardless of time scale so physics remains smooth
        Time.fixedDeltaTime = m_defaultDeltaTime * Time.timeScale;
    }

    public void Reload()
    {
        // reset the ball count
        m_ballCount = m_startingBallCount;
        m_ballCountText.text = m_ballCount.ToString();

        // reload the power
        m_power.Reload();
		
		// destroy the ball if one exists
		if (m_ball != null)
		{
		    Destroy(m_ball);
		}
		
		// reset the time scale
		ModifyTimeScale(m_defaultTimeScale);

        // set up the turn
        SetUpTurn();
    }

    void Start()
    {
        // initialise default time variables
        m_defaultTimeScale = Time.timeScale;
        m_defaultDeltaTime = Time.fixedDeltaTime;

        // initialise the ball count
        m_ballCount = m_startingBallCount;

        // TEMP
        // if the green peg power has not been set
        //if (m_greenPegPower == null)
        //{
        //    // set it to the default power for the stage
        //    SetGreenPegPower(m_pegManager.m_stages[GlobalSettings.m_currentStageID].m_defaultPowerID);
        //}

        // TEMP
        m_power.Initialize();
        
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
        //transform.parent.parent.GetComponentInParent<SpriteRenderer>().color = (CursorWithinPlayArea()) ? m_pegManager.m_greenPegColor : m_pegManager.m_orangePegColor;

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
            ModifyTimeScale(m_defaultTimeScale);
        }
        // TEMP

        // if the Free Ball Text Timer is active
        if (m_freeBallTextTimer > 0.0f)
        {
            // reduce the timer
            m_freeBallTextTimer -= Time.unscaledDeltaTime;

            // if the timer has ran out
            if (m_freeBallTextTimer <= 0.0f)
            {
                // set it to 0
                m_freeBallTextTimer = 0.0f;
                // replace the ball Count Text with the ball count
                m_ballCountText.text = m_ballCount.ToString();
            }
        }

        // if the ball is in play
        if (m_ballInPlay)
        {
            // if the ball exists, trigger the power's ball removal check function. If it does not override the default ball removal check and the ball has fallen low enough
            if (m_ball != null && !m_power.BallRemovalCheck(m_ball) && m_ball.transform.position.y <= m_ballKillFloor)
            {
                // remove the ball from play
                RemoveBall();
            }
            
        }
        // if the ball does not exist, allow the player to shoot a ball
        else
        {
            // if the Shoot / Use Power input has been detected
            if (Input.GetButtonDown("Shoot / Use Power"))
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
