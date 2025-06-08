using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
        File name: SashaPower.cs
        Summary: Manages the power gained from the green peg when playing as Sasha
        Creation Date: 01/06/2025
        Last Modified: 02/06/2025
*/
public class SashaPower : GreenPegPower
{
    Rigidbody2D m_pegContainer;
    Vector3 m_displacement = Vector3.zero;
    public float m_TEMP = 0.25F;
    float m_timer = 0.0f;
    public float m_beatDelay = 0.5f;
    public float m_gracePeriod = 0.1f;

    public override void Initialize()
    {
        // get the rigidbody from the pegmanager's gameobject
        m_pegContainer = m_playerControls.m_pegManager.GetComponent<Rigidbody2D>();
    }

    public override void Trigger(Vector3 a_greenPegPosition)
    {
        // add the charges
        ModifyPowerCharges(m_gainedPowerCharges);

        // play music
    }

    public override void OnShoot()
    {
        // play music

        // return that this function should not override the default shoot function
        return false;
    }

    public override void ResolveTurn()
    {
        // if there are power charges
        if (m_powerCharges > 0)
        {
            // remove one
            ModifyPowerCharges(-1);
        }
    }

    public override void Reload()
    {
        // reset the power charges
        ResetPowerCharges();
    }

    public override void Update()
    {
        // if the ball is in play
        if (m_playerControls.m_ballInPlay && m_powerCharges > 0)
        {
            m_timer += Time.fixedDeltaTime;
            // if the down beat has just occured in the music
            if (m_timer >= m_beatDelay - 0.5f * m_gracePeriod && m_timer <= m_beatDelay + 0.5f * m_gracePeriod)
            {
                // if the pegs are at their base position
                if (m_displacement.SqrMagnitude < m_TEMP)
                {
                    // if the pegs should move up
                    if (Input.GetAxis("Shoot Isaac's Tears Vertical") > 0)
                    {

                    }
                    // otherwise, if the pegs should move up
                    else if (Input.GetAxis("Shoot Isaac's Tears Vertical") < 0)
                    {

                    }
                    // otherwise, if the pegs should move left
                    else if (Input.GetAxis("Shoot Isaac's Tears Horizontal") < 0)
                    {

                    }
                    // otherwise, if the pegs should move right
                    else if (Input.GetAxis("Shoot Isaac's Tears Horizontal") > 0)
                    {

                    }
                }
                else
                {
                    // TEMP - use displacement vector to return to centre
                }
                // reset the timer for the next beat
                m_timer -= m_beatDelay
            }
            // otherwise, if the timer has surpassed the beat
            else if (m_timer > m_beatDelay + m_gracePeriod * 0.5f)
            {
                // reset the timer for the next beat
                m_timer -= m_beatDelay
            }
        }
    }