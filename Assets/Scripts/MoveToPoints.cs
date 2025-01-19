using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
    File name: MoveToPoints.cs
    Summary: Automatically and repeatedly moves an object between 2 points
    Creation Date: 22/01/2024
    Last Modified: 20/01/2025
*/
public class MoveToPoints : MonoBehaviour
{
    public float m_maxSpeed = 10.0f;
    public float m_minSpeed = 0.01f;
    public Vector3 m_secondPosition = Vector3.zero;
    public float m_maxValidSquaredDistanceFromTarget = 0.05f;
    public float m_minSquareDistanceFromPointForMaxSpeed = 1.5f;
    float m_speed;
    [HideInInspector] public Vector3 m_firstPosition = Vector3.zero;
    [HideInInspector] public Vector3 m_targetPosition = Vector3.zero;

    // Start is called before the first frame update
    void Start()
    {
        // store the object's current position as the first position
        m_firstPosition = transform.position;
        // set the first target position to be the second position
        m_targetPosition = m_secondPosition;
    }

    // Update is called once per frame
    void Update()
    {

        // if the object has reached the target position
        if ((transform.position - m_targetPosition).sqrMagnitude <= m_maxValidSquaredDistanceFromTarget)
        {
            // set the object's position to be exactly the target position
            transform.position = m_targetPosition;

            // swap the target position
            m_targetPosition = (m_targetPosition == m_firstPosition) ? m_secondPosition : m_firstPosition;
        }
        // otherwise, if the object is far enough from the first and second point
        else if ((transform.position - m_firstPosition).sqrMagnitude > m_minSquareDistanceFromPointForMaxSpeed && (transform.position - m_secondPosition).sqrMagnitude > m_minSquareDistanceFromPointForMaxSpeed)
        {
            // set the object's speed to the max
            m_speed = m_maxSpeed;
        }
        // if the object is not within the max speed bounds
        else
        {
            // lerp the object's speed between the min and max speed based on its position between the closest point and the point from which the object should be at max speed
            m_speed = Mathf.Lerp(m_minSpeed, m_maxSpeed, (transform.position - ((transform.position - m_firstPosition).sqrMagnitude <= (transform.position - m_secondPosition).sqrMagnitude ? m_firstPosition : m_secondPosition)).sqrMagnitude / m_minSquareDistanceFromPointForMaxSpeed);
        }

        // move the object towards the target position using its speed per second, modified by the player controls time scale and the direction
        transform.position = Vector3.MoveTowards(transform.position, m_targetPosition, m_speed * Time.deltaTime);
    }
}