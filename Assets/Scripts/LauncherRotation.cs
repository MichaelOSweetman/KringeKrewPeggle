using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
    File name: LauncherRotation.cs
    Summary: Rotates the launcher to face the cursor or via micro adjustments from player input
    Creation Date: 02/10/2023
    Last Modified: 04/08/2025
*/
public class LauncherRotation : MonoBehaviour
{
    Vector3 m_mousePosition = Vector3.zero;
    public float m_rotationRange = 160.0f;
	public float m_floatAccuracy = 0.001f;
	public float m_scrollRotationModifier = 2.5f;
    [HideInInspector] public float m_validRotationCentre = 0.0f;
	Vector3 m_previousMousePosition = Vector3.zero;

	void ClampRotation()
	{
		// shift the angle such that 0° is opposite the rotation centre rather than the y axis, so the angle value transitioning from 0° to 360° doesn't disrupt clamping calculations
		float clampedRotation = transform.localEulerAngles.z - m_validRotationCentre - 180.0f;
		// restrict the angle to be within 0° and 360°
		clampedRotation = (clampedRotation < 0.0f) ? clampedRotation + 360.0f : clampedRotation;
        // clamp the angle to the valid rotation range
        clampedRotation = Mathf.Clamp(clampedRotation, 180.0f - (m_rotationRange * 0.5f), 180.0f + (m_rotationRange * 0.5f));
		// shift the angle back by the amount it was initially displaced
		clampedRotation += m_validRotationCentre + 180.0f;
        // restrict the angle to be within 0° and 360°
        clampedRotation = (clampedRotation > 360.0f) ? clampedRotation - 360.0f : clampedRotation;
        // apply the clamped rotation
        transform.rotation = Quaternion.Euler(transform.localEulerAngles.x, transform.localEulerAngles.y, clampedRotation);
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