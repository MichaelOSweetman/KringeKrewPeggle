using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
    File name: Bucket.cs
    Summary: Automatically and repeatedly moves left and right
    Creation Date: 02/10/2023
    Last Modified: 02/10/2023
*/
public class Bucket : MonoBehaviour
{
    public PlayerControls m_playerControls;
    public float m_horizontalBounds = 6.4f;
    public float m_speed = 10.0f;
    Vector3 m_clampVector = Vector3.zero;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        // move the bucket along the x axis using its speed per second, modified by the player controls time scale
        transform.position += Vector3.right * m_speed * Time.deltaTime * m_playerControls.m_timeScale;
        
        // if the bucket has exceeded its bounds
        if (transform.position.x <= -m_horizontalBounds || transform.position.x >= m_horizontalBounds)
        {
            // invert the speed so the bucket moves in the opposite direction
            m_speed *= -1.0f;

            // clamp the bucket's position to be within the horizontal bounds
            m_clampVector = transform.position;
            m_clampVector.x = Mathf.Clamp(m_clampVector.x, -m_horizontalBounds, m_horizontalBounds);
            transform.position = m_clampVector;
        }
    }
}
