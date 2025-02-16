using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/*
	File name: SweetsPower.cs
	Summary: A base class used by classes that manage the power gained by the green peg
	Creation Date: 27/01/2025
	Last Modified: 17/02/2025
*/
public abstract class GreenPegPower : MonoBehaviour
{ 
	public PlayerControls m_playerControls;
    public Text m_PowerChargesText;
	[HideInInspector] public int m_gainedPowerCharges = 0;
    [HideInInspector] public int m_powerCharges = 0;

    public void ModifyPowerCharges(int a_modifier)
    {
        // increase the power charges by the modifier
        m_powerCharges += a_modifier;
        // update the UI text
        m_PowerChargesText.text = m_powerCharges.ToString();
    }

	public void ResetPowerCharges()
	{
		// reset the power charges
		m_powerCharges = 0;
		m_PowerChargesText.text = m_powerCharges.ToString();
	}

	public virtual void Initialize()
	{

	}

    public virtual void Trigger(Vector3 a_greenPegPosition)
	{
		// if there are 0 power charges
		if (m_powerCharges == 0)
		{
			// have the power set up next turn
			m_playerControls.m_setUpPowerNextTurn = true;
		}
		// add the charges
		ModifyPowerCharges(m_gainedPowerCharges);
	}

	public virtual void SetUp()
	{

	}

	public virtual void OnShoot()
	{
		if (m_powerCharges > 0)
		{
			// reduce the power charges by 1
			ModifyPowerCharges(-1);
			// if there are now 0 charges
			if (m_powerCharges == 0)
			{
				// have the power resolve at the start of next turn
				m_playerControls.m_resolvePowerNextTurn = true;
			}
		}
	}

	public virtual void ResolveTurn()
	{

	}

	public virtual void ResolvePower()
	{
		// if there are 0 power charges
		if (m_powerCharges == 0)
		{
			// resolve the power using the same logic used had the power been reloaded
			Reload();
		}
	}

	public virtual void Reload()
	{

	}

	public virtual void Update()
	{

	}
}
