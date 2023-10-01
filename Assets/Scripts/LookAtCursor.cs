using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
    File name: LookAtCursor.cs
    Summary: Rotates the game object to face the cursor
    Creation Date: 02/10/2023
    Last Modified: 02/10/2023
*/
public class LookAtCursor : MonoBehaviour
{
    Vector3 m_mousePosition = Vector3.zero;

    void Start()
    {
        
    }

    void Update()
    {
        // get the mouse position in world space
        m_mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        // remove the z component of the mouse position
        m_mousePosition.z = 0.0f;

        // rotate the game object to face the mouse
        transform.up = m_mousePosition - transform.position;
    }
}
