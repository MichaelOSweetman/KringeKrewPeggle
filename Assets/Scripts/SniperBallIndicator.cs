using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
    File name: SniperBallIndicator.cs
    Summary: Rotates a graphic to tell the player where the ball is relative to the camera
    Creation Date: 10/06/2024
    Last Modified: 10/06/2024
*/
public class SniperBallIndicator : MonoBehaviour
{
	[HideInInspector] public PlayerControls m_playerControls;
	Vector3 m_cameraToBall = Vector3.zero;
	Vector3 m_indicatorRotation = Vector3.zero;
	
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
		// if the ball is present
        if (m_playerControls.m_ball != null)
		{
			// get the vector from the camera to the ball
			/* TEMP Make sure camera and ball have same z position */ 
			m_cameraToBall = m_playerControls.m_ball.transform.position - Camera.main.gameObject.transform.position;
			// use the vector to determine the rotation to apply to the Sniper's Ball Indicator
			m_indicatorRotation.z = Mathf.Atan2(m_cameraToBall.y, m_cameraToBall.x) * Mathf.Rad2Deg;
			// apply the rotation
			transform.rotation = Quarternion.Euler(m_indicatorRotation);
		}
    }
}
