using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
    File name: LauncherRotation.cs
    Summary: Rotates the launcher to face the cursor or via micro adjustments from player input
    Creation Date: 02/10/2023
    Last Modified: 24/05/2025
*/
public class LauncherRotation : MonoBehaviour
{
    Vector3 m_mousePosition = Vector3.zero;
    public float m_rotationRange = 160.0f;
	public float m_floatAccuracy = 0.001f;
	public float m_scrollRotationModifier = 2.5f;
    [HideInInspector] public float m_validRotationCentre = 0.0f;
	Vector3 m_previousMousePosition = Vector3.zero;

	float ReformatAngle(float a_angle)
	{
		if (a_angle > 180.0f)
		{
			return a_angle - 360.0f;
		}
		return a_angle;
	}

	void ClampRotation()
	{
		//transform.rotation = Quaternion.Euler(transform.localEulerAngles.x, transform.localEulerAngles.y, Mathf.Clamp(ReformatAngle(transform.localEulerAngles.z), ReformatAngle(m_validRotationCentre - m_rotationRange * 0.5f), ReformatAngle(m_validRotationCentre + m_rotationRange * 0.5f)));
	}

    void Start()
    {
        // set the center of the valid rotation range to the starting rotation
        m_validRotationCentre = transform.eulerAngles.z;
    }

    void Update()
    {
		// get the mouse position in world space
		m_mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
		// remove the z component of the mouse position
		m_mousePosition.z = 0.0f;
		
		// if the mouse has moved this frame
		if ((m_mousePosition - m_previousMousePosition).magnitude > m_floatAccuracy)
		{
			// rotate the game object to face the mouse
			transform.up = m_mousePosition - transform.position;

            // clamp the rotation to the valid range
            ClampRotation();
        }
		// otherwise, if the scroll wheel has moved
		else if (Input.mouseScrollDelta.y > m_floatAccuracy || Input.mouseScrollDelta.y < -m_floatAccuracy)
		{
			// apply the rotation
			transform.Rotate(Vector3.forward, Input.mouseScrollDelta.y * m_scrollRotationModifier);

			// clamp the rotation to the valid range
			ClampRotation();
		}
		
		// store this frame's mouse position for next frame
		m_previousMousePosition = m_mousePosition;
    }
}