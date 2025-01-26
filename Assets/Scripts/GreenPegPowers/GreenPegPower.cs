using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
	File name: SweetsPower.cs
	Summary: A base class used by classes that manage the power gained by the green peg
	Creation Date: 27/01/2025
	Last Modified: 27/01/2025
*/
public abstract class GreenPegPower : MonoBehaviour
{ 
	public PlayerControls m_playerControls;

	public virtual void Trigger(Vector3 a_greenPegPosition)
	{
		// if there are 0 power charges
		if (m_playerControls.m_powerCharges == 0)
		{
			// have the power set up next turn
			m_playerControls.m_setUpPowerNextTurn = true;
		}
		// add the charges
		m_playerControls.ModifyPowerCharges();
	}

	public abstract void SetUp();

	public virtual void OnShoot()
	{
		// reduce the power charges by 1
		m_playerControls.ModifyPowerCharges(-1);
		// if there are now 0 charges
		if (m_playerControls.m_powerCharges == 0)
		{
			// have the power resolve at the start of next turn
			m_playerControls.m_resolvePowerNextTurn = true;
		}
	}

	public abstract void Resolve();

	public abstract void Reload();
}
