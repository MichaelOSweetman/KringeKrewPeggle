using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
	File name: KevinPower.cs
	Summary: Manages the power gained from the green peg when playing as Kevin
	Creation Date: 27/01/2025
	Last Modified: 03/02/2025
*/
public class KevinPower : GreenPegPower
{
	public new int m_gainedPowerCharges = 2;
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

	public override void Resolve()
	{

	}

	public override void Reload()
	{
        // tell the camera to return to its default state instantly
        m_playerControls.m_cameraZoom.ReturnToDefault(true);
        // hide the scope overlay
        m_scopeOverlay.SetActive(false);
    }
}
