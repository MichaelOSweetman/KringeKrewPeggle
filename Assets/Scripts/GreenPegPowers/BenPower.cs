using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
	File name: BenPower.cs
	Summary: Manages the magic power gained from the green peg when playing as Ben
	Creation Date: 27/01/2025
	Last Modified: 20/04/2026
*/
public class BenPower : MagicPower
{
    public GameObject m_IsaacPrefab;
    public Vector3 m_isaacSpawnPosition;
    GameObject m_bucket;

    public override void Initialize()
    {
        // get access to the peg manager through player controls and use it to access and store the bucket
        m_bucket = m_playerControls.m_pegManager.m_bucket;
    }

    public override void Trigger(Vector3 a_greenPegPosition)
	{
        // add the charges
        ModifyPowerCharges(m_gainedPowerCharges);
        // ensure the power doesn't resolve at the end of this turn
        m_resolveNextTurn = false;
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
        m_bucket.gameObject.SetActive(false);
        // if there are now 0 charges
        if (m_powerCharges == 0)
        {
            // have the power resolve at the start of next turn
            m_resolveNextTurn = true;
        }

        // create a copy of the Isaac prefab
        GameObject Isaac = Instantiate(m_IsaacPrefab, m_gameManager.m_playerProjectilesContainer);
        // set its position to be the Isaac Spawn Position
        Isaac.transform.position = m_isaacSpawnPosition;
        // give Isaac the game manager component
        Isaac.GetComponent<Isaac>().m_gameManager = m_gameManager;

        // give player controls Isaac in place of the ball
        m_playerControls.m_ball = Isaac;

        // tell the game manager that the shot has occurred
        m_gameManager.OnShoot();

        // return that this function should override the default shoot function
        return true;
    }

	public override void Reload()
	{
        // set the bucket to be active
        m_bucket.gameObject.SetActive(true);

        // reset the power charges
        ResetPowerCharges();
    }
}
