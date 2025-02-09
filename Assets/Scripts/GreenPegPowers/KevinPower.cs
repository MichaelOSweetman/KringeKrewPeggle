using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
	File name: KevinPower.cs
	Summary: Manages the power gained from the green peg when playing as Kevin
	Creation Date: 27/01/2025
	Last Modified: 10/02/2025
*/
public class KevinPower : GreenPegPower
{
	public new int m_gainedPowerCharges = 2;
    public CameraZoom m_cameraZoom;
    public GameObject m_scopeOverlay;
    public float m_forceToBall = 2000.0f;
    public float m_scopedTimeScale = 0.3f;

    public override void Trigger(Vector3 a_greenPegPosition)
	{
        // add the charges
        ModifyPowerCharges(m_gainedPowerCharges);
    }

	public override void SetUp()
	{

	}

	public override void OnShoot()
	{

	}

	public override void ResolvePower()
	{

	}

	public override void Reload()
	{
        // tell the camera to return to its default state instantly
        m_playerControls.m_cameraZoom.ReturnToDefault(true);
        // hide the scope overlay
        m_scopeOverlay.SetActive(false);
    }

    public override void Update()
    {
		// if the current game state is 'Ball in Play' and there are power charges
		if (m_playerControls.m_currentGameState == PlayerControls.GameState.BallInPlay && m_powerCharges > 0)
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
