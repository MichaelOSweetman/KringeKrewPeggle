using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
        File name: SashaPower.cs
        Summary: Manages the power gained from the green peg when playing as Sasha
        Creation Date: 01/06/2025
        Last Modified: 09/06/2025
*/
public class SashaPower : GreenPegPower
{
    Transform m_pegContainer;
    Vector3 m_displacement = Vector3.zero;
    public float m_TEMP = 0.25F;
    float m_timer = 0.0f;
    public float m_beatDelay = 0.5f;
    public float m_gracePeriod = 0.1f;
    Vector3 m_containerDefaultPosition = Vector3.zero;
    public float m_moveDistance = 1.0f;

    // TEMP
    SpriteRenderer m_TEMPbeatShowcase;

    public override void Initialize()
    {
        // get from the pegmanager gameobject's transform
        m_pegContainer = m_playerControls.m_pegManager.transform;
        // store its current position as the default position
        m_pegContainer.transform.position = m_containerDefaultPosition;

        // TEMP
        m_TEMPbeatShowcase = m_playerControls.m_UIManager.m_launcherRotation.GetComponent<SpriteRenderer>();
    }

    public override void Trigger(Vector3 a_greenPegPosition)
    {
        // add the charges
        ModifyPowerCharges(m_gainedPowerCharges);

        // play music
    }

    public override bool OnShoot()
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
        // ensure the peg container is at the default position
        m_pegContainer.transform.position = m_containerDefaultPosition;
    }

    public override void Update()
    {
        // if the ball is in play
        if (m_playerControls.m_ballInPlay && m_powerCharges > 0)
        {
            // TEMP
            m_TEMPbeatShowcase.color = Color.magenta;

            m_timer += Time.fixedDeltaTime;

            // TEMP
            print(m_timer);

            // if the down beat has just occured in the music
            if (m_timer >= m_beatDelay - 0.5f * m_gracePeriod && m_timer <= m_beatDelay + 0.5f * m_gracePeriod)
            {
                // TEMP
                m_TEMPbeatShowcase.color = Color.yellow;

                // if the pegs are at their base position
                if ((m_pegContainer.position - m_containerDefaultPosition).sqrMagnitude < m_TEMP)
                {
                    // if the pegs should move up
                    if (Input.GetAxis("Shoot Isaac's Tears Vertical") > 0)
                    {
                        m_displacement = Vector3.up * m_moveDistance;
                    }
                    // otherwise, if the pegs should move down
                    else if (Input.GetAxis("Shoot Isaac's Tears Vertical") < 0)
                    {
                        m_displacement = Vector3.down * m_moveDistance;
                    }
                    // otherwise, if the pegs should move left
                    else if (Input.GetAxis("Shoot Isaac's Tears Horizontal") < 0)
                    {
                        m_displacement = Vector3.left * m_moveDistance;
                    }
                    // otherwise, if the pegs should move right
                    else if (Input.GetAxis("Shoot Isaac's Tears Horizontal") > 0)
                    {
                        m_displacement = Vector3.right * m_moveDistance;
                    }

                    // TEMP
                    if (m_displacement.sqrMagnitude > m_TEMP)
                    { 
                        // apply the displacement
                        m_pegContainer.position += m_displacement;
                        // reset the timer for the next beat
                        m_timer -= m_beatDelay;
                    }
                }
            }
            // otherwise, if the timer has surpassed the beat
            else if (m_timer > m_beatDelay + m_gracePeriod * 0.5f)
            {
                // reset the timer for the next beat
                m_timer -= m_beatDelay;
            }
        }
    }
}