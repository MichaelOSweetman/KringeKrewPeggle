using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/*
	File name: SweetsPower.cs
	Summary: Manages the power gained from the green peg when playing as Sweets
	Creation Date: 27/01/2025
	Last Modified: 24/05/2025
*/
public class SweetsPower : GreenPegPower
{
	MoveToPoints m_bucket;
	GameObject m_victoryBuckets;
	GameObject m_topWall;
	LauncherRotation m_launcherRotation;
	RawImage m_gameOverlay;
	Texture m_defaultOverlay;
	public Texture m_hillsideOverlay;

	void ToggleHillside()
	{
		// flip the bucket around the x axis
		m_bucket.transform.position = new Vector3(m_bucket.transform.position.x, -m_bucket.transform.position.y);
		m_bucket.transform.rotation = Quaternion.Euler(m_bucket.transform.rotation.eulerAngles.x, m_bucket.transform.rotation.eulerAngles.y, m_bucket.transform.rotation.eulerAngles.z + 180.0f);

		// invert the y axis values of the positions of the bucket's MoveToPoints script
		m_bucket.m_firstPosition.y *= -1.0f;
		m_bucket.m_secondPosition.y *= -1.0f;
		m_bucket.m_targetPosition.y *= -1.0f;

		// flip the victory buckets around the x axis
		m_victoryBuckets.transform.position = new Vector3(m_victoryBuckets.transform.position.x, -m_victoryBuckets.transform.position.y);
		m_victoryBuckets.transform.rotation = Quaternion.Euler(m_victoryBuckets.transform.rotation.eulerAngles.x, m_victoryBuckets.transform.rotation.eulerAngles.y, m_victoryBuckets.transform.rotation.eulerAngles.z + 180.0f);

        // flip the Top Wall around the x axis
        m_topWall.transform.position = new Vector3(m_topWall.transform.position.x, -m_topWall.transform.position.y);
        m_topWall.transform.rotation = Quaternion.Euler(m_topWall.transform.rotation.eulerAngles.x, m_topWall.transform.rotation.eulerAngles.y, m_topWall.transform.rotation.eulerAngles.z + 180.0f);
		
		// flip the launcher around the x axis
		m_launcherRotation.transform.parent.rotation = Quaternion.Euler(m_launcherRotation.transform.parent.rotation.eulerAngles.x, m_launcherRotation.transform.parent.rotation.eulerAngles.y, m_launcherRotation.transform.parent.rotation.eulerAngles.z + 180.0f);

		// invert the rotation center of the launcher rotation component
		// TEMP - should be 180 or 0
		m_launcherRotation.m_validRotationCentre = 0.0f;

		// swap the texture of the game overlay
		m_gameOverlay.texture = (m_gameOverlay.texture == m_defaultOverlay) ? m_hillsideOverlay : m_defaultOverlay;

		// invert gravity
		Physics2D.gravity *= -1;
	}

	public override void Initialize()
	{
		// get access to the peg manager through player controls and use it to access the bucket's MoveToPoints component and the victory buckets
        m_bucket = m_playerControls.m_pegManager.m_bucket.GetComponent<MoveToPoints>();
		m_victoryBuckets = m_playerControls.m_pegManager.m_victoryBuckets;

        // get access to the ui manager through player controls and use it to access and store the launcher's LauncherRotation component
        m_launcherRotation = m_playerControls.m_UIManager.m_launcherRotation;

		// get access to the ui manager through player controls and use it to access and store the game overlay
		m_gameOverlay = m_playerControls.m_UIManager.m_gameOverlay;

		// store the game overlay's current texture as the default overlay
		m_defaultOverlay = m_gameOverlay.texture;

		// get the launcher's parent's parent to access the top wall
		m_topWall = m_launcherRotation.transform.parent.parent.gameObject;
    }

	public override void SetUp()
	{
		// flip the bucket, launcher and gravity
		ToggleHillside();
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

    public override bool BallRemovalCheck(GameObject a_ball)
    {
		// if the ball is in play and has fallen low enough (or high enough if Hillside is active)
		if (a_ball.transform.position.y <= m_playerControls.m_ballKillFloor || a_ball.transform.position.y >= -m_playerControls.m_ballKillFloor)
		{
            // have player controls remove the ball from play
            m_playerControls.RemoveBall();
        }

		// return that this function should override the default ball removal check
		return true;
    }

    public override void Reload()
	{
		// if the power has flipped gravity to be positive
		if (Physics2D.gravity.y > 0)
		{
			// flip the bucket, launcher and gravity back to default positions
			ToggleHillside();
		}

		// reset the power charges
		ResetPowerCharges();
	}
}
