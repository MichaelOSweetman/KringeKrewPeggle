using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/*
    File name: PlayerControls.cs
    Summary: Manages the player's ability to shoot the ball and speed up time, as well as to make use of the different powers
    Creation Date: 01/10/2023
    Last Modified: 30/10/2023
*/
public class PlayerControls : MonoBehaviour
{
    [Header("Peg Manager")]
    public PegManager m_pegManager;

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
    public delegate void GreenPegPower(Vector3 a_greenPegPosition);
    public GreenPegPower m_triggerPower;
    int m_powerCharges = 0;

    [Header("Daniel Power")]
    public float m_searchRadius = 5.0f;
    public int m_maxPegs = 8;

    [Header("Sweets Power")]
    public int m_sweetsPowerChargesGained = 3;

    [Header("Time Scale")]
    public float m_spedUpTimeScale = 5.0f;
    [HideInInspector] public float m_timeScale;

    void BenPower(Vector3 a_greenPegPosition)
    {
        // TEMP
        print("BenPower() called");
    }

    void DanielPower(Vector3 a_greenPegPosition)
    {
        // TEMP
        print("DanielPower() called");
    }

    void EthenPower(Vector3 a_greenPegPosition)
    {
        // TEMP
        print("EthenPower() called");
    }

    void JackPower(Vector3 a_greenPegPosition)
    {
        // TEMP
        print("JackPower() called");
    }

    void KevinPower(Vector3 a_greenPegPosition)
    {
        // TEMP
        print("KevinPower() called");
    }

    void LokiPower(Vector3 a_greenPegPosition)
    {
        // TEMP
        print("LokiPower() called");
    }

    void MatPower(Vector3 a_greenPegPosition)
    {
        // TEMP
        print("MatPower() called");
    }

    void SweetsPower(Vector3 a_greenPegPosition)
    {
        // TEMP
        print("SweetsPower() called");
        // add the charges
        ModifyPowerCharges(m_sweetsPowerChargesGained);
    }

    void PhoebePower(Vector3 a_greenPegPosition)
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
        // return the ball gameobject
        return Ball;
    }

    public void DestroyBall()
    {
        // destroy the ball
        Destroy(m_ball);
        // tell the peg manager to clear all the hit pegs. If there were no pegs to clear give the player a 50% chance to get back a free ball
        if (!m_pegManager.ClearHitPegs() && Random.Range(0,2) == 1)
        {
            FreeBall();
        }
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
        m_triggerPower = SweetsPower;
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
                DestroyBall();

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
