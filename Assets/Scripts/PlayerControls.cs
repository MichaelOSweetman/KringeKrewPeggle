using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/*
    File name: PlayerControls.cs
    Summary: Manages the player's ability to shoot the ball and speed up time, as well as to make use of the different powers
    Creation Date: 01/10/2023
    Last Modified: 31/12/2023
*/
public class PlayerControls : MonoBehaviour
{
    public enum GameState
    {
        TurnSetUp,
        Shoot,
        BallInPlay,
        ResolveTurn,
        LevelOver,
        Paused
    }

    public enum PowerFunctionMode
    {
        Trigger,
        SetUp,
        OnShoot,
        Resolve
    }


    [HideInInspector] public GameState m_currentGameState = GameState.Shoot;

    [Header("Other Scripts")]
    public PegManager m_pegManager;
    public CameraZoom m_cameraZoom;

    [Header("UI")]
    public Text m_ballCountText;

    [Header("Ball")]
    public GameObject m_ballPrefab;
    public float m_ballLaunchSpeed;
    public byte m_startingBallCount = 10;
    public float m_ballKillFloor = -7.0f;
    GameObject m_ball = null;
    int m_ballCount = 0;

    [Header("Green Pegs")]
    public Text m_PowerChargesText;
    public delegate void GreenPegPower(PowerFunctionMode a_powerFunctionMode, Vector3 a_greenPegPosition);
    public GreenPegPower m_greenPegPower;
    int m_powerCharges = 0;
    bool m_setUpPowerNextTurn = false;
    bool m_resolvePowerNextTurn = false;

    [Header("Mat Power")]
    public GameObject m_matejaPrefab;
    GameObject m_mateja = null;

    [Header("Daniel Power")]
    public GameObject m_wasp;
    public float m_searchRadius = 5.0f;

    [Header("Kevin Power")]
    public int m_kevinPowerChargesGained = 2;
    public GameObject m_scopeOverlay;
    public float m_forceToBall = 2000.0f;
    public float m_scopedTimeScale = 0.3f;

    [Header("Sweets Power")]
    public int m_sweetsPowerChargesGained = 3;
    public GameObject m_launcher;
    public GameObject m_topPlayAreaCollider;
    public GameObject m_gameOverlay;
    public GameObject m_hillsideGameOverlay;

    [Header("Buckets")]
    public GameObject m_bucket;
    public GameObject m_victoryBuckets;

    [Header("Time Scale")]
    public float m_spedUpTimeScale = 5.0f;
    [HideInInspector] public float m_timeScale = 1.0f;

    void BenPower(PowerFunctionMode a_powerFunctionMode, Vector3 a_greenPegPosition)
    {
        // have the power set up next turn
        m_setUpPowerNextTurn = true;
        // TEMP
        print("BenPower() called");
    }

    void DanielPower(PowerFunctionMode a_powerFunctionMode, Vector3 a_greenPegPosition)
    {
        // if the green peg has been triggered
        if (a_powerFunctionMode == PowerFunctionMode.Trigger)
        {
            // get an array of all colliders around the green peg
            Collider2D[] CollidersInRange = Physics2D.OverlapCircleAll(new Vector2(a_greenPegPosition.x, a_greenPegPosition.y), m_searchRadius);

            // loop for each collider
            for (int i = 0; i < CollidersInRange.Length; ++i)
            {
                Peg peg = CollidersInRange[i].GetComponent<Peg>();
                // if the collider has the peg component and hasn't been hit
                if (peg && !peg.m_hit)
                {
                    // create a wasp
                    GameObject wasp = Instantiate(m_wasp) as GameObject;
                    // position it on the green peg
                    wasp.transform.position = a_greenPegPosition;
                    // give the wasp the peg as its target
                    wasp.GetComponent<Wasp>().m_targetPeg = peg;
                    // give the wasp this gameobject's PlayerControls script so it can access the current time scale
                    wasp.GetComponent<Wasp>().m_playerControls = this;
                }
            }
        }

        // TEMP
        print("DanielPower() called");
    }

    void EthenPower(PowerFunctionMode a_powerFunctionMode, Vector3 a_greenPegPosition)
    {
        // TEMP
        print("EthenPower() called");
    }

    void JackPower(PowerFunctionMode a_powerFunctionMode, Vector3 a_greenPegPosition)
    {
        // TEMP
        print("JackPower() called");
    }

    void KevinPower(PowerFunctionMode a_powerFunctionMode, Vector3 a_greenPegPosition)
    {
        // if the green peg has been triggered
        if (a_powerFunctionMode == PowerFunctionMode.Trigger)
        {
            // add the charges
            ModifyPowerCharges(m_kevinPowerChargesGained);
        }

        // TEMP
        print("KevinPower() called");
    }

    void LokiPower(PowerFunctionMode a_powerFunctionMode, Vector3 a_greenPegPosition)
    {
        // TEMP
        print("LokiPower() called");
    }

    void MatPower(PowerFunctionMode a_powerFunctionMode, Vector3 a_greenPegPosition)
    {
        // if the green peg has been triggered or if the green peg power is to be set up
        if (a_powerFunctionMode == PowerFunctionMode.Trigger || a_powerFunctionMode == PowerFunctionMode.SetUp)
        {
            // if Mateja does not currently exist
            if (m_mateja == null)
            {
                // create the mateja game object
                m_mateja = Instantiate(m_matejaPrefab) as GameObject;
                // give it this instance of player controls so it can access the time scale
                Mateja matejaScript = m_mateja.GetComponent<Mateja>();
                matejaScript.m_playerControls = this;
                // give it the bucket and victory bucket game objects
                matejaScript.m_bucket = m_bucket;
                matejaScript.m_victoryBuckets = m_victoryBuckets;
            }
            // if there is already a Mateja
            else
            {
                // have another Mateja be created next turn
                m_setUpPowerNextTurn = true;
            }
        }

        // TEMP
        print("MatPower() called");
    }

    void ToggleHillside()
    {
        // flip the bucket around the x axis
        m_bucket.transform.position = new Vector3(m_bucket.transform.position.x, -m_bucket.transform.position.y);
        m_bucket.transform.rotation = Quaternion.Euler(m_bucket.transform.rotation.eulerAngles.x, m_bucket.transform.rotation.eulerAngles.y, m_bucket.transform.rotation.eulerAngles.z + 180.0f);

        // flip the victory buckets around the x axis
        m_victoryBuckets.transform.position = new Vector3(m_victoryBuckets.transform.position.x, -m_victoryBuckets.transform.position.y);
        m_victoryBuckets.transform.rotation = Quaternion.Euler(m_victoryBuckets.transform.rotation.eulerAngles.x, m_victoryBuckets.transform.rotation.eulerAngles.y, m_victoryBuckets.transform.rotation.eulerAngles.z + 180.0f);

        // flip the launcher around the x axis
        m_launcher.transform.position = new Vector3(m_launcher.transform.position.x, -m_launcher.transform.position.y);
        m_launcher.transform.rotation = Quaternion.Euler(m_launcher.transform.rotation.eulerAngles.x, m_launcher.transform.rotation.eulerAngles.y, m_launcher.transform.rotation.eulerAngles.z + 180.0f);

        // flip the Top Play Area Collider around the x axis
        m_topPlayAreaCollider.transform.position = new Vector3(m_topPlayAreaCollider.transform.position.x, -m_topPlayAreaCollider.transform.position.y);
        m_topPlayAreaCollider.transform.rotation = Quaternion.Euler(m_topPlayAreaCollider.transform.rotation.eulerAngles.x, m_topPlayAreaCollider.transform.rotation.eulerAngles.y, m_topPlayAreaCollider.transform.rotation.eulerAngles.z + 180.0f);

        // swap the active state of the game overlays
        m_gameOverlay.SetActive(!m_gameOverlay.activeSelf);
        m_hillsideGameOverlay.SetActive(!m_hillsideGameOverlay.activeSelf);

        // invert gravity
        Physics2D.gravity *= -1;
    }

    void SweetsPower(PowerFunctionMode a_powerFunctionMode, Vector3 a_greenPegPosition)
    {
        // if the green peg has been triggered
        if (a_powerFunctionMode == PowerFunctionMode.Trigger)
        {
            // if there are 0 power charges
            if (m_powerCharges == 0)
            {
                // have the power set up next turn
                m_setUpPowerNextTurn = true;
            }
            // add the charges
            ModifyPowerCharges(m_sweetsPowerChargesGained);
        }
        // otherwise, if the player has just shot a ball
        else if (a_powerFunctionMode == PowerFunctionMode.OnShoot)
        {
            // reduce the power charges by 1
            ModifyPowerCharges(-1);
            // if there are now 0 charges
            if (m_powerCharges == 0)
            {
                // have the power resolve at the start of next turn
                m_resolvePowerNextTurn = true;
            }
        }
        // otherwise, if the green peg power is to be set up or resolved
        else
        {
            ToggleHillside();
        }

        // TEMP
        print("SweetsPower() called");
    }

    void PhoebePower(PowerFunctionMode a_powerFunctionMode, Vector3 a_greenPegPosition)
    {
        // TEMP
        print("PhoebePower() called");
    }

    void ModifyPowerCharges(int a_modifier)
    {
        // increase the power charges by the modifier
        m_powerCharges += a_modifier;
        // update the UI text
        m_PowerChargesText.text = m_powerCharges.ToString();
    }

    GameObject Shoot()
    {
        // create a copy of the ball prefab
        GameObject Ball = Instantiate(m_ballPrefab) as GameObject;
        // set its position to be the same as this game object
        Ball.transform.position = transform.position;
        // apply the launch speed force to the ball, in the direction this gameobject is facing
        Ball.GetComponent<Rigidbody2D>().AddForce(transform.up * m_ballLaunchSpeed, ForceMode2D.Impulse);
        // give the ball the peg manager
        Ball.GetComponent<Ball>().m_pegManager = m_pegManager;
        // reduce the ball count by one as a ball has been expended
        --m_ballCount;
        // update the ball count text
        m_ballCountText.text = m_ballCount.ToString();
        // set the game state to Ball In Play
        m_currentGameState = GameState.BallInPlay;
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

        // set the game state to Resolve Turn
        m_currentGameState = GameState.ResolveTurn;
        // tell the peg manager to clear all the hit pegs. If there were no pegs to clear give the player a 50% chance to get back a free ball
        if (!m_pegManager.ClearHitPegs() && Random.Range(0,2) == 1)
        {
            FreeBall();
        }
        // tell the camera to return to default in case it had moved while the ball was in play
        m_cameraZoom.ReturnToDefault();
        // tell the peg manager to resolve the turn
        m_pegManager.ResolveTurn();
    }

    public void FreeBall()
    {
        // increase the ball count by 1 as a ball has been gained
        ++m_ballCount;
        // update the ball count text
        m_ballCountText.text = m_ballCount.ToString();
    }

    void Start()
    {
        // initialise the ball count
        m_ballCount = m_startingBallCount;

        // TEMP
        m_greenPegPower = MatPower;
    }

    void Update()
    {
        // TEMP
        if (Input.GetKeyDown(KeyCode.P))
        {
            m_mateja.GetComponent<Mateja>().JiuJitsuBall(m_ball);
        }

        // if the green peg power is Kevin's and the show sniper scope button has been released
        if (m_greenPegPower == KevinPower && Input.GetButtonUp("Show Sniper Scope"))
        {
            // tell the camera to return to its default state
            m_cameraZoom.ReturnToDefault();
            // hide the scope overlay
            m_scopeOverlay.SetActive(false);
            // reset the time scale
            m_timeScale = 1.0f;
        }

        // if the current game state is Turn Set Up
        if (m_currentGameState == GameState.TurnSetUp)
        {
            // if the power should be set up this turn
            if (m_setUpPowerNextTurn)
            {
                // set up the power for this turn
                m_greenPegPower(PowerFunctionMode.SetUp, Vector3.zero);
                // set the Set Up Power Next Turn flag to false as the power has now been set up
                m_setUpPowerNextTurn = false;
            }
            else if (m_resolvePowerNextTurn)
            {
                // resolve the power
                m_greenPegPower(PowerFunctionMode.Resolve, Vector3.zero);
                // set the Resolve Power Next Turn flag to false as the power has now been resolved
                m_resolvePowerNextTurn = false;
            }

            // TEMP
            m_currentGameState = GameState.Shoot;
        }
        // if the current game state is Shoot
        else if (m_currentGameState == GameState.Shoot)
        {
            // if the Shoot / Use Power input has been detected
            if (Input.GetButtonDown("Shoot / Use Power"))
            {
                // shoot a ball
                m_ball = Shoot();
                
                // if there are power charges
                if (m_powerCharges > 0)
                {
                    // have the power trigger its On Shoot effect
                    m_greenPegPower(PowerFunctionMode.OnShoot, Vector3.zero);
                }
            }

            // if the Speed Up Time input has been detected
            if (Input.GetButton("Speed Up Time"))
            {
                // change the timescale to the sped up timescale
                m_timeScale = m_spedUpTimeScale;
            }
            // otherwise, if the Speed Up Time button has been released
            else if (Input.GetButtonUp("Speed Up Time"))
            {
                // reset the timescale
                m_timeScale = 1.0f;
            }
        }
        // if the current gamestate is Ball In Play
        else if (m_currentGameState == GameState.BallInPlay)
        {
            // if there are power charges
            if (m_powerCharges > 0)
            {
                // if the green peg power is kevin's
                if (m_greenPegPower == KevinPower)
                {
                    // if the show sniper scope button has been pressed
                    if (Input.GetButtonDown("Show Sniper Scope"))
                    {
                        // tell the camera to zoom and track the cursor
                        m_cameraZoom.ZoomAndTrack();
                        // show the scope overlay
                        m_scopeOverlay.SetActive(true);
                        // set the time scale to the scoped time scale
                        m_timeScale = m_scopedTimeScale;
                    }

                    // if the shoot / use power button has been pressed and the camera is at max zoom
                    if (Input.GetButtonDown("Shoot / Use Power") && m_cameraZoom.m_atMaxZoom)
                    {
                        // reduce the power charges by 1
                        ModifyPowerCharges(-1);
                        // if there are now 0 charges
                        if (m_powerCharges == 0)
                        {
                            // have the power resolve at the start of next turn
                            m_resolvePowerNextTurn = true;
                        }

                        // if the camera is looking at a point on the ball
                        if (m_ball.GetComponent<Collider2D>().bounds.Contains(new Vector3(Camera.main.transform.position.x, Camera.main.transform.position.y, m_ball.transform.position.z)))
                        {
                            // shoot the ball in the direction opposite of where it got shot from, with a magnitude determined by m_forceToBall
                            m_ball.GetComponent<Rigidbody2D>().AddForce((m_ball.transform.position - Camera.main.transform.position).normalized * m_forceToBall, ForceMode2D.Impulse);
                        }
                    }
                }
            }

            // if the ball is in play and has fallen low enough (or high enough with the Sweets Power)
            if (m_ball != null && (m_ball.transform.position.y <= m_ballKillFloor || m_ball.transform.position.y >= -m_ballKillFloor))
            {
                // if there is a mateja in play
                if (m_mateja != null)
                {
                    // have mateja launch back up
                    m_mateja.GetComponent<Mateja>().JiuJitsuBall(m_ball);
                    // destroy the ball
                    Destroy(m_ball);
                }
                // if there is not a mateja in play
                else
                {
                    // resolve the turn
                    ResolveTurn();
                }
            }
        }
        // if the current game state is Level Over
        else if (m_currentGameState == GameState.LevelOver)
        {

        }
        // if the current game state is Paused
        else if (m_currentGameState == GameState.Paused)
        {

        }

        // TEMP
        //print(m_currentGameState.ToString());
    }
}
