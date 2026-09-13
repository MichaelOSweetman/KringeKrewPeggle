using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/*
	File name: SweetsPower.cs
	Summary: Manages the magic power gained from the green peg when playing as Sweets
	Creation Date: 27/01/2025
	Last Modified: 14/09/2026
*/
public class SweetsPower : MagicPower
{
	public Texture m_hillsideOverlay;
	Rigidbody2D m_ball;
	MoveToPoints m_bucket;
	GameObject m_victoryBuckets;
	GameObject m_topWall;
	LauncherRotation m_launcherRotation;
	RawImage m_gameOverlay;
	Texture m_defaultOverlay;

	void ToggleHillside()
	{
		// flip the bucket around the z axis
		m_bucket.transform.position = new Vector3(m_bucket.transform.position.x, -m_bucket.transform.position.y);
		m_bucket.transform.rotation = Quaternion.Euler(m_bucket.transform.rotation.eulerAngles.x, m_bucket.transform.rotation.eulerAngles.y, m_bucket.transform.rotation.eulerAngles.z + 180.0f);

		// invert the y axis values of the positions of the bucket's MoveToPoints script
		m_bucket.m_firstPosition.y *= -1.0f;
		m_bucket.m_secondPosition.y *= -1.0f;
		m_bucket.m_targetPosition.y *= -1.0f;

		// flip the victory buckets around the z axis
		m_victoryBuckets.transform.position = new Vector3(m_victoryBuckets.transform.position.x, -m_victoryBuckets.transform.position.y);
		m_victoryBuckets.transform.rotation = Quaternion.Euler(m_victoryBuckets.transform.rotation.eulerAngles.x, m_victoryBuckets.transform.rotation.eulerAngles.y, m_victoryBuckets.transform.rotation.eulerAngles.z + 180.0f);

        // flip the Top Wall around the z axis
        m_topWall.transform.position = new Vector3(m_topWall.transform.position.x, -m_topWall.transform.position.y);
        m_topWall.transform.rotation = Quaternion.Euler(m_topWall.transform.rotation.eulerAngles.x, m_topWall.transform.rotation.eulerAngles.y, m_topWall.transform.rotation.eulerAngles.z + 180.0f);
		
		// flip the launcher around the z axis
		m_launcherRotation.transform.parent.rotation = Quaternion.Euler(m_launcherRotation.transform.parent.rotation.eulerAngles.x, m_launcherRotation.transform.parent.rotation.eulerAngles.y, m_launcherRotation.transform.parent.rotation.eulerAngles.z + 180.0f);

		// invert the rotation center of the launcher rotation component, keeping it within the range of 0° and 360°
		m_launcherRotation.m_validRotationCentre = (m_launcherRotation.m_validRotationCentre < 180.0f) ? m_launcherRotation.m_validRotationCentre + 180.0f : m_launcherRotation.m_validRotationCentre - 180.0f;

		// swap the texture of the game overlay
		m_gameOverlay.texture = (m_gameOverlay.texture == m_defaultOverlay) ? m_hillsideOverlay : m_defaultOverlay;

		// invert the effect of gravity on the ball
		m_ball.gravityScale *= -1.0f;
	}

	public override void Initialize()
	{
		// get access to the rigidbody component of the ball via the game manager and player controls 
		m_ball = m_gameManager.m_playerControls.m_ballPrefab.GetComponent<Rigidbody2D>();

		// get the bucket's MoveToPoints component and the victory buckets via the peg manager
        m_bucket = m_pegManager.m_bucket.GetComponent<MoveToPoints>();
		m_victoryBuckets = m_pegManager.m_victoryBuckets;

        // get access to the LauncherRotation via the game manager
        m_launcherRotation = m_gameManager.m_launcherRotation;

		// get access to the ui manager through the game manager and use it to access and store the game overlay
		m_gameOverlay = m_gameManager.m_UIManager.m_gameOverlay;

		// store the game overlay's current texture as the default overlay
		m_defaultOverlay = m_gameOverlay.texture;

		// get the launcher's parent's parent to access the top wall
		m_topWall = m_launcherRotation.transform.parent.parent.gameObject;

		// store that the power is ready for the game to be in the pre shot state
		m_powerState = GameManager.GameState.PreShot;
	}

	public override void SetUp()
	{
		// if the power should be set up this turn
		if (m_setUpNextTurn)
		{
			// flip the bucket, launcher and gravity
			ToggleHillside();

			// disable the set up power flag
			m_setUpNextTurn = false;
		}

		// store that the power is ready for the game to be in the shooting state
		m_powerState = GameManager.GameState.Shooting;
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
                // have the power resolve at the end of this turn
                m_resolvePowerThisTurn = true;
            }
        }

        // return that this function should not override the default shoot function
        return false;
    }

    public override bool BallRemovalCheck(Ball a_ball)
    {
		// if the ball is in play and has fallen low enough (or high enough if Hillside is active)
		if (a_ball.transform.position.y <= a_ball.m_ballKillFloor || a_ball.transform.position.y >= -a_ball.m_ballKillFloor)
		{
            // have the game manager remove the ball from play
            m_gameManager.RemoveProjectile(a_ball.gameObject);
        }

		// return that this function should override the default ball removal check
		return true;
    }

    public override void Reload()
	{
		// if the power has flipped the effect of gravity on the ball
		if (m_ball.gravityScale < 0.0f)
		{
			// flip the bucket, launcher and gravity back to default positions
			ToggleHillside();
		}

		// reset the power charges
		ResetPowerCharges();

		// store that the power is ready for the game to be in the pre shot state
		m_powerState = GameManager.GameState.PreShot;
	}
}
