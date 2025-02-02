using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
	File name: JackPower.cs
	Summary: Manages the power gained from the green peg when playing as Jack
	Creation Date: 27/01/2025
	Last Modified: 03/02/2025
*/
public class JackPower : GreenPegPower
{
	public new int m_gainedPowerCharges = 4;
    int m_communistPegScore = 0;
    int m_defaultBluePegScore = 0;
    int m_defaultOrangePegScore = 0;
    int m_defaultPurplePegScore = 0;
    int m_defaultGreenPegScore = 0;

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

	public override void Reload()
	{
        // return the score gained from pegs to their default bases
        m_playerControls.m_pegManager.m_baseBluePegScore = m_defaultBluePegScore;
        m_playerControls.m_pegManager.m_baseOrangePegScore = m_defaultOrangePegScore;
        m_playerControls.m_pegManager.m_basePurplePegScore = m_defaultPurplePegScore;
        m_playerControls.m_pegManager.m_baseGreenPegScore = m_defaultGreenPegScore;
    }
}
