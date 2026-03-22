using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
    File name: PlayAreaBounds.cs
    Summary: Determines if the cursor is within this object's bounds
    Creation Date: 23/03/2026
    Last Modified: 23/03/2026
*/
public class PlayAreaBounds : MonoBehaviour
{
    Vector2 m_bottomLeft;
    Vector3 m_topRight;

    public bool CursorWithinPlayArea()
    {
        // get the coordinates of the current mouse position in world space
        Vector3 worldSpaceMousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        // return true if the following is true
        return
        (
        // the cursor is right of (or at) the bottom left point of the bounds
        worldSpaceMousePosition.x >= m_bottomLeft.x &&
        // the cursor is above (or at) the bottom left point of the bounds
        worldSpaceMousePosition.y >= m_bottomLeft.y &&
        // the cursor is left of (or at) the top right point of the bounds
        worldSpaceMousePosition.x <= m_topRight.x &&
        // the cursor is below (or at) the top right point of the bounds
        worldSpaceMousePosition.y <= m_topRight.y
        );
    }

    void Start()
    {
        // store the bottom left and top right coordinates of the play area bounds
        m_bottomLeft = new Vector2(transform.position.x - transform.lossyScale.x * 0.5f, transform.position.y - transform.lossyScale.y * 0.5f);
        m_topRight = new Vector2(transform.position.x + transform.lossyScale.x * 0.5f, transform.position.y + transform.lossyScale.y * 0.5f);
    }
}
