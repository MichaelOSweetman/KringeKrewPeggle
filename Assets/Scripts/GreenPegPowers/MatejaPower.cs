using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
	File name: MatejaPower.cs
	Summary: Manages the magic power gained from the green peg when playing as Mateja
	Creation Date: 27/01/2025
	Last Modified: 30/03/2026
*/
public class MatejaPower : MagicPower
{
	public GameObject m_matejaPrefab;
	GameObject m_mateja = null;

	void CreateMateja()
	{
        // if Mateja does not currently exist
        if (m_mateja == null)
        {
            // create the mateja game object and add it to the player projectile container
            m_mateja = Instantiate(m_matejaPrefab, m_playerControls.m_playerProjectilesContainer);
            Mateja matejaScript = m_mateja.GetComponent<Mateja>();
            // give it game manager, player controls, the bucket and the victory buckets
            matejaScript.m_gameManager = m_gameManager;
            matejaScript.m_playerControls = m_playerControls;
            matejaScript.m_bucket = m_playerControls.m_pegManager.m_bucket;
            matejaScript.m_victoryBuckets = m_playerControls.m_pegManager.m_victoryBuckets;
        }
        // if there is already a Mateja
        else
        {
            // have another Mateja be created next turn
            m_setUpNextTurn = true;
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

    public override bool BallRemovalCheck(GameObject a_ball)
    {
        // if Mateja does not currently exist
        if (m_mateja == null)
        {
            // return that this function should not override the default ball removal check
            return false;
        }

        // if the ball is in play and has fallen low enough
        if (a_ball.transform.position.y <= m_playerControls.m_ballKillFloor)
        {
            // have mateja launch back up
            m_mateja.GetComponent<Mateja>().JiuJitsuBall(a_ball);
            // have the player controls remove the ball
            m_playerControls.RemoveProjectile(a_ball);
        }

        // return that this function should override the default ball removal check
        return true;
    }

    public override void Reload()
	{
        // destroy Mateja
        Destroy(m_mateja);
    }
}
