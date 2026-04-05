using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
	File name: MagicPower.cs
	Summary: A base class used by classes that manage the magic power gained by the green peg
	Creation Date: 27/01/2025
	Last Modified: 06/04/2026
*/
public abstract class MagicPower : MonoBehaviour
{ 
	public int m_gainedPowerCharges = 0;
	[HideInInspector] public GameManager m_gameManager;
	[HideInInspector] public PlayerControls m_playerControls;
    [HideInInspector] public UIManager m_UIManager;
    [HideInInspector] public int m_powerCharges = 0;
	[HideInInspector] public bool m_setUpNextTurn = false;
	[HideInInspector] public bool m_resolveNextTurn = false;


    public void ModifyPowerCharges(int a_modifier)
    {
        // increase the power charges by the modifier
        m_powerCharges += a_modifier;
		// update the UI text
		m_UIManager.UpdatePowerChargeText(m_powerCharges);
    }

	public void ResetPowerCharges()
	{
		// reset the power charges
		m_powerCharges = 0;
        m_UIManager.UpdatePowerChargeText(m_powerCharges);
    }

	public virtual void Initialize()
	{

	}

    public virtual void Trigger(Vector3 a_greenPegPosition)
	{
		// if there are 0 power charges and the power was not about to be resolved (and therefore already active)
		if (m_powerCharges == 0 && !m_resolveNextTurn)
		{
			// have the power set up next turn
			m_setUpNextTurn = true;
        }

		// add the charges
		ModifyPowerCharges(m_gainedPowerCharges);

        // ensure the power doesn't resolve at the end of this turn
        m_resolveNextTurn = false;
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
