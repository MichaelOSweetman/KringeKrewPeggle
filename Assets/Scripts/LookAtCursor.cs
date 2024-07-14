using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
    File name: LookAtCursor.cs
    Summary: Rotates the game object to face the cursor
    Creation Date: 02/10/2023
    Last Modified: 15/07/2024
*/
public class LookAtCursor : MonoBehaviour
{
    Vector3 m_mousePosition = Vector3.zero;
    public float m_rotationRange = 160.0f;
    float m_validRotationCentre = 0.0f;

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

        // rotate the game object to face the mouse
        transform.up = m_mousePosition - transform.position;

        // clamp the rotation to the valid range
        transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y, Mathf.Clamp(transform.rotation.eulerAngles.z, m_validRotationCentre - m_rotationRange * 0.5f, m_validRotationCentre + m_rotationRange * 0.5f));
    }
}
