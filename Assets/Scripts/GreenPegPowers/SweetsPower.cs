using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
	File name: SweetsPower.cs
	Summary: Manages the power gained from the green peg when playing as Sweets
	Creation Date: 27/01/2025
	Last Modified: 24/02/2025
*/
public class SweetsPower : GreenPegPower
{
	public MoveToPoints m_bucket;
	public GameObject m_victoryBuckets;
	public GameObject m_launcher;
	public GameObject m_topWall;
	public GameObject m_gameOverlay;
	public GameObject m_hillsideGameOverlay;
	public LauncherRotation m_LauncherLookControls;

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

		// flip the launcher around the x axis
		m_launcher.transform.position = new Vector3(m_launcher.transform.position.x, -m_launcher.transform.position.y);
		m_launcher.transform.rotation = Quaternion.Euler(m_launcher.transform.rotation.eulerAngles.x, m_launcher.transform.rotation.eulerAngles.y, m_launcher.transform.rotation.eulerAngles.z + 180.0f);

		// invert the rotation center of the launcher
		m_LauncherLookControls.m_validRotationCentre *= -1.0f;

		// flip the Top Wall around the x axis
		m_topWall.transform.position = new Vector3(m_topWall.transform.position.x, -m_topWall.transform.position.y);
		m_topWall.transform.rotation = Quaternion.Euler(m_topWall.transform.rotation.eulerAngles.x, m_topWall.transform.rotation.eulerAngles.y, m_topWall.transform.rotation.eulerAngles.z + 180.0f);

		// swap the active state of the game overlays
		m_gameOverlay.SetActive(!m_gameOverlay.activeSelf);
		m_hillsideGameOverlay.SetActive(!m_hillsideGameOverlay.activeSelf);

		// invert gravity
		Physics2D.gravity *= -1;
	}

	public override void SetUp()
	{
		// flip the bucket, launcher and gravity
		ToggleHillside();
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
