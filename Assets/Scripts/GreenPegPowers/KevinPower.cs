using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
	File name: KevinPower.cs
	Summary: Manages the power gained from the green peg when playing as Kevin
	Creation Date: 27/01/2025
	Last Modified: 24/03/2025
*/
public class KevinPower : GreenPegPower
{
    public GameObject m_scopeOverlayPrefab;
    public CameraZoom m_cameraZoom;
    GameObject m_scopeOverlay;
    public float m_forceToBall = 2000.0f;
    public float m_scopedTimeScale = 0.3f;

    public override void Initialize()
    {
        // create the scope overlay and set its parent to be the parent of the power charges text so they are on the canvas
        m_scopeOverlay = Instantiate(m_scopeOverlayPrefab, m_powerChargesText.rectTransform.parent);
    }

    public override void Trigger(Vector3 a_greenPegPosition)
	{
        // add the charges
        ModifyPowerCharges(m_gainedPowerCharges);
    }

    public override void Reload()
	{
        // tell the camera to return to its default state instantly
        m_playerControls.m_cameraZoom.ReturnToDefault(true);
        // hide the scope overlay
        m_scopeOverlay.SetActive(false);

        // reset the power charges
        ResetPowerCharges();
    }

    public override void Update()
    {
        // if the show sniper scope button has been released
        if (Input.GetButtonUp("Show Sniper Scope"))
        {
            // tell the camera to return to its default state
            m_cameraZoom.ReturnToDefault();
            // hide the scope overlay
            m_scopeOverlay.SetActive(false);
            // reset the time scale
            m_playerControls.ModifyTimeScale();
        }

        // if the ball is in play and there are power charges
        if (m_playerControls.m_ballInPlay && m_powerCharges > 0)
		{
            // if the show sniper scope button has been pressed
            if (Input.GetButtonDown("Show Sniper Scope"))
            {
                // tell the camera to zoom and track the cursor
                m_cameraZoom.ZoomAndTrack();
                // show the scope overlay
                m_scopeOverlay.SetActive(true);
                // set the time scale to the scoped time scale
                m_playerControls.ModifyTimeScale(m_scopedTimeScale);
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
                    m_playerControls.m_resolvePowerNextTurn = true;
                }

                // if the camera is looking at the ball
                if (m_playerControls.m_ball.GetComponent<Collider2D>().bounds.Contains(new Vector3(Camera.main.transform.position.x, Camera.main.transform.position.y, m_playerControls.m_ball.transform.position.z)))
                {
                    // shoot the ball in the direction opposite of where it got hit by this power, with a magnitude determined by m_forceToBall
                    m_playerControls.m_ball.GetComponent<Rigidbody2D>().AddForce((m_playerControls.m_ball.transform.position - Camera.main.transform.position).normalized * m_forceToBall, ForceMode2D.Impulse);
                }
            }
        }
        
    }
}
