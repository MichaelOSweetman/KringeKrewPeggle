using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/*
	File name: GreenPegPower.cs
	Summary: A base class used by classes that manage the power gained by the green peg
	Creation Date: 27/01/2025
	Last Modified: 04/08/2025
*/
public abstract class GreenPegPower : MonoBehaviour
{ 
	public int m_gainedPowerCharges = 0;
	[HideInInspector] public PlayerControls m_playerControls;
    [HideInInspector] public Text m_powerChargesText;
    [HideInInspector] public int m_powerCharges = 0;

    public void ModifyPowerCharges(int a_modifier)
    {
        // increase the power charges by the modifier
        m_powerCharges += a_modifier;
        // update the UI text
        m_powerChargesText.text = m_powerCharges.ToString();
    }

	public void ResetPowerCharges()
	{
		// reset the power charges
		m_powerCharges = 0;
		m_powerChargesText.text = m_powerCharges.ToString();
	}

	public virtual void Initialize()
	{

	}

    public virtual void Trigger(Vector3 a_greenPegPosition)
	{
		// if there are 0 power charges and the power was not about to be resolved (and therefore already active)
		if (m_powerCharges == 0 && !m_playerControls.m_resolvePowerNextTurn)
		{
			// have the power set up next turn
			m_playerControls.m_setUpPowerNextTurn = true;
        }

		// add the charges
		ModifyPowerCharges(m_gainedPowerCharges);

        // ensure the power doesn't resolve at the end of this turn
        m_playerControls.m_resolvePowerNextTurn = false;
    }

	public virtual void SetUp()
	{

	}

	public virtual bool OnShoot()
	{
        // return that this function should not override the default shoot function
        return false;
    }

	public virtual bool BallRemovalCheck(GameObject a_ball)
	{
		// return that this function should not override the default ball removal check
		return false;
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
