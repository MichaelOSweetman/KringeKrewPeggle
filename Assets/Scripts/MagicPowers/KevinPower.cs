using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
	File name: KevinPower.cs
	Summary: Manages the magic power gained from the green peg when playing as Kevin
	Creation Date: 27/01/2025
	Last Modified: 27/04/2026
*/
public class KevinPower : MagicPower
{
    public GameObject m_scopeOverlayPrefab;
    public CameraZoom m_cameraZoom;
    GameObject m_scopeOverlay;
    public float m_forceToBall = 2000.0f;
    public float m_scopedTimeScale = 0.3f;

    void HideSniperScope(bool a_instant = false)
    {
        // tell the camera to return to its default state
        m_cameraZoom.ReturnToDefault(a_instant);
        // hide the scope overlay
        m_scopeOverlay.SetActive(false);
        // reset the time scale
        m_playerControls.ModifyTimeScale();
    }

    public override void Initialize()
    {
        // have the UI Manager create the scope overlay
        m_UIManager.CreatePowerUIAsset(m_scopeOverlayPrefab);

        // give the sniper ball indicator access to player controls so it can access the ball
        m_scopeOverlay.transform.GetChild(0).GetComponent<RotateToBall>().m_playerControls = m_playerControls;

        // get access to the camera zoom component from the game manager
        m_cameraZoom = m_gameManager.m_cameraZoom;

        // store that the power is ready for the game to be in the pre shot state
        m_powerState = GameManager.GameState.PreShot;
    }

    public override void Trigger(Vector3 a_greenPegPosition)
	{
        // add the charges
        ModifyPowerCharges(m_gainedPowerCharges);
    }

    public override void ResolveTurn()
    {
        // hide the sniper scope and return to the default camera position and zoom immediately
        HideSniperScope(true);

        // store that the power is ready for the game to be in the pre shot state
        m_powerState = GameManager.GameState.PreShot;
    }

    public override void Reload()
    {
        // tell the camera to return to its default state instantly
        m_cameraZoom.ReturnToDefault(true);

        // destroy the scope overlay
        Destroy(m_scopeOverlay);

        // reset the power charges
        ResetPowerCharges();

        // store that the power is ready for the game to be in the pre shot state
        m_powerState = GameManager.GameState.PreShot;
    }

    public override void Update()
    {
        // if the show sniper scope button has been released
        if (Input.GetButtonUp("Show Sniper Scope"))
        {
            // hide the sniper scope
            HideSniperScope();
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
                    // hide the sniper scope
                    HideSniperScope();
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
