using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
	File name: MatejaPower.cs
	Summary: Manages the power gained from the green peg when playing as Mateja
	Creation Date: 27/01/2025
	Last Modified: 17/02/2025
*/
public class MatejaPower : GreenPegPower
{
	public GameObject m_matejaPrefab;
	GameObject m_mateja = null;

	void CreateMateja()
	{
        // if Mateja does not currently exist
        if (m_mateja == null)
        {
            // create the mateja game object
            m_mateja = Instantiate(m_matejaPrefab) as GameObject;
            Mateja matejaScript = m_mateja.GetComponent<Mateja>();
            // give it player controls and the bucket game objects
            matejaScript.m_playerControls = m_playerControls;
            matejaScript.m_bucket = m_playerControls.m_bucket.gameObject;
            matejaScript.m_victoryBuckets = m_playerControls.m_victoryBuckets;
        }
        // if there is already a Mateja
        else
        {
            // have another Mateja be created next turn
            m_playerControls.m_setUpPowerNextTurn = true;
        }
    }

    public override void Trigger(Vector3 a_greenPegPosition)
	{
        CreateMateja();
	}

	public override void SetUp()
	{
        CreateMateja();
	}

	public override void Reload()
	{
        // destroy Mateja
        Destroy(m_mateja);
    }
}
