using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/*
        File name: SashaPower.cs
        Summary: Manages the power gained from the green peg when playing as Sasha
        Creation Date: 01/06/2025
        Last Modified: 16/06/2025
*/
public class SashaPower : GreenPegPower
{
    public GameObject m_UIArrowPrefab;
    RawImage m_UIArrow;
    public float m_beatDelay = 0.5f;
    public float m_gracePeriod = 0.25f;
    public float m_moveDistance = 1.0f;
    public float m_moveSpeed = 6.0f;
    Transform m_pegContainer;
    Vector3 m_containerDefaultPosition = Vector3.zero;
    Vector3 m_direction = Vector3.zero;
    bool m_atDefaultPosition = true;
    float m_timer = 0.0f;

    public override void Initialize()
    {
        // create the ui arrow and set its parent to be the parent of the power charges text so they are on the canvas
        m_UIArrow = Instantiate(m_UIArrowPrefab, m_powerChargesText.rectTransform.parent).GetComponent<RawImage>();

        // get from the pegmanager gameobject's transform
        m_pegContainer = m_playerControls.m_pegManager.transform;
        // store its current position as the default position
        m_pegContainer.transform.position = m_containerDefaultPosition;
    }

    public override void Trigger(Vector3 a_greenPegPosition)
    {
        // add the charges
        ModifyPowerCharges(m_gainedPowerCharges);

        // play music
    }

    public override bool OnShoot()
    {
        // if there are power charges
        if (m_powerCharges > 0)
        {
            // remove one
            ModifyPowerCharges(-1);
        }

        // play music

        // return that this function should not override the default shoot function
        return false;
    }

    public override void ResolveTurn()
    {
        // ensure the pegs are at their default position
        m_pegContainer.position = m_containerDefaultPosition;
        m_atDefaultPosition = true;

        // temp
        m_UIArrow.color = Color.white;
    }

    public override void Reload()
    {
        // reset the power charges
        ResetPowerCharges();
        // ensure the peg container is at the default position
        m_pegContainer.transform.position = m_containerDefaultPosition;
        m_atDefaultPosition = true;

        // temp
        m_UIArrow.color = Color.white;
    }

    public override void Update()
    {
        // if the ball is in play and there are power charges
        if (true)//m_playerControls.m_ballInPlay && m_powerCharges > 0)
        {
            // increase the timer
            m_timer += Time.unscaledDeltaTime;

            // determine the direction the pegs should move
            if (Input.GetAxis("Use Power Vertical") > 0)
            {
                m_direction = Vector3.up;
            }
            else if (Input.GetAxis("Use Power Vertical") < 0)
            {
                m_direction = Vector3.down;
            }
            else if (Input.GetAxis("Use Power Horizontal") < 0)
            {
                m_direction = Vector3.left;
            }
            else if (Input.GetAxis("Use Power Horizontal") > 0)
            {
                m_direction = Vector3.right;
            }
            else
            {
                m_direction = Vector3.zero;
            }

            // if the pegs aren't at the default position and the 'up beat' has just occured in the song
            if (!m_atDefaultPosition && m_timer > m_beatDelay * 0.5f)
            {
                m_pegContainer.position = m_containerDefaultPosition; // lerp back
                m_atDefaultPosition = true;

                // temp
                m_UIArrow.color = Color.white;
            }
            // if the 'down beat' has just occured in the song
            else if (m_timer > m_beatDelay && m_timer < m_beatDelay + m_gracePeriod)
            {
                // temp
                m_UIArrow.color = Color.yellow;

                // lerp the pegs from the default position to a point in the direction and a distance away as specified by m_direction and m_moveDistance, at a speed determined by m_moveSpeed
                m_pegContainer.position = Vector3.Lerp(m_containerDefaultPosition, m_containerDefaultPosition + m_direction * m_moveDistance, (m_timer - m_beatDelay) * m_moveSpeed);

                // if the time for the lerp has fully elapsed
                if ((m_timer - m_beatDelay * m_moveSpeed) >= 1.0f)
                {
                    // set the the pegs to the end position
                    m_pegContainer.position += m_direction * m_moveDistance;
                    // store that the pegs have moved
                    m_atDefaultPosition = false;
                    // reset the timer
                    m_timer = 0.0f; // subtract not set
                }

            }
            // if the down beat was missed
            else if (m_timer > m_beatDelay + m_gracePeriod)
            {
                // reset the timer so it is in time for the next beat
                m_timer -= m_beatDelay;

                // temp
                m_UIArrow.color = Color.white;
            }
        }
    }
}
