using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
	File name: JackPower.cs
	Summary: Manages the power gained from the green peg when playing as Jack
	Creation Date: 27/01/2025
	Last Modified: 10/03/2025
*/
public class JackPower : GreenPegPower
{
    int m_communistPegScore = 0;
    int m_defaultBluePegScore = 0;
    int m_defaultOrangePegScore = 0;
    int m_defaultPurplePegScore = 0;
    int m_defaultGreenPegScore = 0;

    public override void Initialize()
    {
        // store the base peg scores of each type
        m_defaultBluePegScore = m_playerControls.m_pegManager.m_baseBluePegScore;
        m_defaultOrangePegScore = m_playerControls.m_pegManager.m_baseOrangePegScore;
        m_defaultPurplePegScore = m_playerControls.m_pegManager.m_basePurplePegScore;
        m_defaultGreenPegScore = m_playerControls.m_pegManager.m_baseGreenPegScore;
    }

    public override void SetUp()
	{
        // get the average of the scores of all the active pegs
        m_communistPegScore = m_playerControls.m_pegManager.GetAverageActivePegScore();

        // replace the score gained from these pegs with this average
        m_playerControls.m_pegManager.m_baseBluePegScore = m_communistPegScore;
        m_playerControls.m_pegManager.m_baseOrangePegScore = m_communistPegScore;
        m_playerControls.m_pegManager.m_basePurplePegScore = m_communistPegScore;
        m_playerControls.m_pegManager.m_baseGreenPegScore = m_communistPegScore;
    }

    public override bool OnShoot()
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

        // return that this function should not override the default ball removal check
        return false;
    }

    public override void Reload()
	{
        // return the score gained from pegs to their default bases
        m_playerControls.m_pegManager.m_baseBluePegScore = m_defaultBluePegScore;
        m_playerControls.m_pegManager.m_baseOrangePegScore = m_defaultOrangePegScore;
        m_playerControls.m_pegManager.m_basePurplePegScore = m_defaultPurplePegScore;
        m_playerControls.m_pegManager.m_baseGreenPegScore = m_defaultGreenPegScore;

        // reset the power charges
        ResetPowerCharges();
    }
}
