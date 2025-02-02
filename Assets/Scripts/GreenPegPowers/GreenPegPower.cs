using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/*
	File name: SweetsPower.cs
	Summary: A base class used by classes that manage the power gained by the green peg
	Creation Date: 27/01/2025
	Last Modified: 03/02/2025
*/
public abstract class GreenPegPower : MonoBehaviour
{ 
	public PlayerControls m_playerControls;
	public int m_gainedPowerCharges = 0;
    public Text m_PowerChargesText;
    int m_powerCharges = 0;

    public void ModifyPowerCharges(int a_modifier)
    {
        // increase the power charges by the modifier
        m_powerCharges += a_modifier;
        // update the UI text
        m_PowerChargesText.text = m_powerCharges.ToString();
    }

    public virtual void Trigger(Vector3 a_greenPegPosition)
	{
		// if there are 0 power charges
		if (m_playerControls.m_powerCharges == 0)
		{
			// have the power set up next turn
			m_playerControls.m_setUpPowerNextTurn = true;
		}
		// add the charges
		ModifyPowerCharges(m_gainedPowerCharges);
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

	public virtual void Resolve()
	{
		// if there are 0 power charges
		if (m_powerCharges == 0)
		{
			// resolve the power using the same logic used had the power been reloaded
			Reload();
		}
	}

	public abstract void Reload();
}
