using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
    File name: Bucket.cs
    Summary: Automatically and repeatedly moves left and right
    Creation Date: 02/10/2023
    Last Modified: 12/11/2023
*/
public class Bucket : MonoBehaviour
{
    public PlayerControls m_playerControls;
    public float m_maxSpeed = 10.0f;
    public float m_minSpeed = 0.01f;
    public float m_horizontalBounds = 6.4f;
    public float m_maxSpeedBounds = 5.0f;
    float m_direction = 1.0f;
    public float m_boundForgivenessThreshold = 0.5f;
    public float m_speed;
    Vector3 m_clampVector = Vector3.zero;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

        // if the bucket is beyond the horizontal bounds
        if (transform.position.x <= -m_horizontalBounds + m_boundForgivenessThreshold || transform.position.x >= m_horizontalBounds - m_boundForgivenessThreshold)
        {
            // invert the direction so the bucket moves in the opposite direction
            m_direction *= -1.0f;

            // clamp the bucket's position to be within the horizontal bounds
            m_clampVector = transform.position;
            m_clampVector.x = Mathf.Clamp(m_clampVector.x, -m_horizontalBounds, m_horizontalBounds);
            transform.position = m_clampVector;

        }
        // otherwise, if the bucket is within the max speed bounds
        else if (transform.position.x <= m_maxSpeedBounds && transform.position.x >= -m_maxSpeedBounds)
        {
            // set the bucket's speed to the max
            m_speed = m_maxSpeed;
        }
        // if the bucket is not within the max speed bounds
        else
        {
            // lerp the bucket's speed between the max and min speed based on its position between the max speed and horizontal bounds
            m_speed = Mathf.Lerp(m_maxSpeed, m_minSpeed, (Mathf.Abs(transform.position.x) - m_maxSpeedBounds)/(m_horizontalBounds-m_maxSpeedBounds));
        }



        // move the bucket along the x axis using its speed per second, modified by the player controls time scale and the direction
        transform.position += Vector3.right * m_speed * m_direction * Time.deltaTime * m_playerControls.m_timeScale;


    }
}
