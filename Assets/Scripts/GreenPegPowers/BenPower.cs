using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
	File name: BenPower.cs
	Summary: Manages the power gained from the green peg when playing as Ben
	Creation Date: 27/01/2025
	Last Modified: 17/02/2025
*/
public class BenPower : GreenPegPower
{
    public new int m_gainedPowerCharges = 1;
    public GameObject m_IsaacPrefab;
    bool m_spawnIsaac = false;

    public override void Trigger(Vector3 a_greenPegPosition)
	{
        // add the charges
        ModifyPowerCharges(m_gainedPowerCharges);
        // spawn isaac instead of the ball next turn
        m_spawnIsaac = true;
    }

	public override void OnShoot()
	{
        if (m_powerCharges > 0)
        {
            // reduce the power charges by 1
            ModifyPowerCharges(-1);
            // disable the bucket
            m_playerControls.m_bucket.gameObject.SetActive(false);
            // if there are now 0 charges
            if (m_powerCharges == 0)
            {
                // have the power resolve at the start of next turn
                m_playerControls.m_resolvePowerNextTurn = true;
            }
        }
    }

	public override void Reload()
	{
        // ensure the ball is spawned instead of Isaac next shoot phase
        m_spawnIsaac = false;
        m_playerControls.m_bucket.gameObject.SetActive(true);

        // reset the power charges
        ResetPowerCharges();
    }
}
