using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
    File name: RotateToBall.cs
    Summary: Rotates an object to face either the ball or optionally the cursor when there is no ball active
    Creation Date: 10/06/2024
    Last Modified: 15/07/2024
*/
public class RotateToBall : MonoBehaviour
{
	public PlayerControls m_playerControls;
	public bool m_targetCursorWithoutBall = false;
	Vector3 m_transformToTarget = Vector3.zero;
	Vector3 m_mousePosition = Vector3.zero;
	Vector3 m_eulerRotation = Vector3.zero;
	
    // Start is called before the first frame update
    void Start()
    {
        
    }

	void RotateTowardsTarget(Vector3 a_targetPosition)
	{
		// get the vector from the transform to the target
		m_transformToTarget = a_targetPosition - transform.position;
		// use the vector to determine the rotation to apply to this transform
		m_eulerRotation.z = Mathf.Atan2(m_transformToTarget.y, m_transformToTarget.x) * Mathf.Rad2Deg;
		// apply the rotation
		transform.rotation = Quaternion.Euler(m_eulerRotation);
	}

    // Update is called once per frame
    void Update()
    {
		// if the ball is present
        if (m_playerControls.m_ball != null)
		{
			// rotate this transform to face the ball
			RotateTowardsTarget(m_playerControls.m_ball.transform.position);
		}
		// otherwise, if the cursor should be the target
		else if (m_targetCursorWithoutBall)
		{
			// get the mouse position in world space
			m_mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
			// remove the z component of the mouse position
			m_mousePosition.z = 0.0f;
			
			// rotate this transform to face the cursor
			RotateTowardsTarget(m_mousePosition);
		}
    }
}
