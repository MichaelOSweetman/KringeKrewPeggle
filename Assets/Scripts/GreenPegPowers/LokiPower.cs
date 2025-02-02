using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
	File name: LokiPower.cs
	Summary: Manages the power gained from the green peg when playing as Loki
	Creation Date: 27/01/2025
	Last Modified: 03/02/2025
*/
public class LokiPower : GreenPegPower
{
	public new int m_gainedPowerCharges = 2;
    public LineRenderer m_lokiPowerCord;
    public float m_maxCordLength = 5.0f;
    public GameObject m_hook;
    public float m_hookLaunchSpeed = 30.0f;
    public float m_pullSpeed = 7.5f;
    GameObject m_connectionPoint;
    bool m_connectedToPeg = false;

    public void ConnectHook(GameObject a_connectionPeg)
    {
        // set the connection point of the cord to be the peg
        m_connectionPoint = a_connectionPeg;
        // store that the end of the cord has been connected to a peg
        m_connectedToPeg = true;
        // make the hook inactive as the peg has replaced it
        m_hook.SetActive(false);
        // expend a power charge
        ModifyPowerCharges(-1);
    }

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
        // clear the connection point
        m_connectionPoint = null;

        // make the cord and hook inactive
        m_lokiPowerCord.gameObject.SetActive(false);
        m_hook.gameObject.SetActive(false);

        // store that the ball is not connected to a peg
        m_connectedToPeg = false;
    }
}
