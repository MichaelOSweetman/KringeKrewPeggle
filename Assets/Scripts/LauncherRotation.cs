using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// TEMP
using UnityEngine.UI;

/*
    File name: LauncherRotation.cs
    Summary: Rotates the launcher to face the cursor or via micro adjustments from player input
    Creation Date: 02/10/2023
    Last Modified: 14/07/2025
*/
public class LauncherRotation : MonoBehaviour
{
    Vector3 m_mousePosition = Vector3.zero;
    public float m_rotationRange = 160.0f;
	public float m_floatAccuracy = 0.001f;
	public float m_scrollRotationModifier = 2.5f;
    [HideInInspector] public float m_validRotationCentre = 0.0f;
	Vector3 m_previousMousePosition = Vector3.zero;

	// TEMP
	public Text text;

	float FormatAngle(float a_angle)
	{
		// shift the angle so that 0° is parallel with the x axis rather than the y axis, so the angle value transitioning from 0° to 360° doesn't disrupt clamping calculations
		a_angle += 90.0f;

		// return the angle within the range of 0°-360°
		return (a_angle > 360.0f) ? a_angle - 360.0f : a_angle;
	}

	void ClampRotation()
	{
		//transform.rotation = Quaternion.Euler(transform.localEulerAngles.x, transform.localEulerAngles.y, Mathf.Clamp(FormatAngle(transform.localEulerAngles.z), FormatAngle(m_validRotationCentre - m_rotationRange * 0.5f), FormatAngle(m_validRotationCentre + m_rotationRange * 0.5f)));

		// TEMP
		float formattedRotation = FormatAngle(transform.localEulerAngles.z);
		float formattedRotationCentre = FormatAngle(m_validRotationCentre);

		float formattedClampedRotation = Mathf.Clamp(formattedRotation, formattedRotationCentre - (m_rotationRange * 0.5f), formattedRotationCentre + (m_rotationRange * 0.5f));
		float clampedRotation = formattedClampedRotation - 90.0f;
		if (clampedRotation < 0.0f)
		{
			clampedRotation += 360.0f;
		}

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

		// TEMP
		text.text = FormatAngle(transform.localEulerAngles.z).ToString();

	}
}