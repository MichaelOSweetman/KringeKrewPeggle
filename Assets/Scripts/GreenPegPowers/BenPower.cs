using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
	File name: BenPower.cs
	Summary: Manages the power gained from the green peg when playing as Ben
	Creation Date: 27/01/2025
	Last Modified: 03/03/2025
*/
public class BenPower : GreenPegPower
{
    public GameObject m_IsaacPrefab;
    public Vector3 m_isaacSpawnPosition;

    public override void Trigger(Vector3 a_greenPegPosition)
	{
        // add the charges
        ModifyPowerCharges(m_gainedPowerCharges);
    }

	public override bool OnShoot()
	{
        if (m_powerCharges == 0)
        {
            // return that this function should not override the default shoot function
            return false;
        }

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

        // create a copy of the Isaac prefab
        GameObject Isaac = Instantiate(m_IsaacPrefab);
        // set its position to be the Isaac Spawn Position
        Isaac.transform.position = m_isaacSpawnPosition;
        // give Isaac the player controls component
        Isaac.GetComponent<Isaac>().m_playerControls = m_playerControls;

        // give player controls Isaac in place of the ball
        m_playerControls.m_ball = Isaac;

        // reduce the ball count by one as a the Isaac counts as a ball for the purposes of the ball count
        --m_playerControls.m_ballCount;
        // update the ball count text
        m_playerControls.m_ballCountText.text = m_playerControls.m_ballCount.ToString();

        // return that this function should override the default shoot function
        return true;
    }

	public override void Reload()
	{
        // set the bucket to be active
        m_playerControls.m_bucket.gameObject.SetActive(true);

        // reset the power charges
        ResetPowerCharges();
    }
}
