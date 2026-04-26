using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
	File name: MatejaPower.cs
	Summary: Manages the magic power gained from the green peg when playing as Mateja
	Creation Date: 27/01/2025
	Last Modified: 27/04/2026
*/
public class MatejaPower : MagicPower
{
	public GameObject m_matejaPrefab;
	Mateja m_mateja = null;

	void CreateMateja()
	{
        // if Mateja does not currently exist
        if (m_mateja == null)
        {
            // create the mateja game object and add it to the player projectile container
            m_mateja = Instantiate(m_matejaPrefab, m_gameManager.m_playerProjectilesContainer).GetComponent<Mateja>();
            // give it game manager, player controls, the bucket and the victory buckets
            m_mateja.m_gameManager = m_gameManager;
            m_mateja.m_playerControls = m_playerControls;
            m_mateja.m_bucket = m_playerControls.m_pegManager.m_bucket;
            m_mateja.m_victoryBuckets = m_playerControls.m_pegManager.m_victoryBuckets;
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
        // Create Mateja
        CreateMateja();
	}

	public override void SetUp()
	{
        // create Mateja
        CreateMateja();

        // store that the power is ready for the game to be in the shooting state
        m_powerState = GameManager.GameState.Shooting;
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
            m_mateja.JiuJitsuBall(a_ball);
            // have the player controls remove the ball
            m_gameManager.RemoveProjectile(a_ball);
        }

        // return that this function should override the default ball removal check
        return true;
    }

    public override void Reload()
	{
        // destroy the Mateja game object
        Destroy(m_mateja.gameObject);

        // store that the power is ready for the game to be in the pre shot state
        m_powerState = GameManager.GameState.PreShot;
    }

    public override bool IsReady(GameManager.GameState a_goalState)
    {
        // if the goal state is Shooting
        if (a_goalState == GameManager.GameState.Shooting)
        {
            // return true if the power's state is Shooting and the Mateja object is in its Ready state
            return (m_powerState == a_goalState && m_mateja.m_matejaState == Mateja.MatejaState.Ready);
        }
        // otherwise, if the goal state is another game state
        else
        {
            // return whether the power is ready for the game to be at the goal state
            return (m_powerState == a_goalState);
        }
    }
}
