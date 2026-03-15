using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
    File name: BallTrajectory.cs
    Summary: Uses a line renderer to show the player the predicted path of the ball if it were fired this frame
    Creation Date: 02/12/2024
    Last Modified: 18/08/2025
*/
public class BallTrajectory : MonoBehaviour
{
    public int m_linePointCount = 10;
    public float m_maxLineLength = 4.0f;
    public Transform m_ballSpawnPoint;
    public Rigidbody2D m_ballRigidbody;
    LineRenderer m_lineRenderer;
    Vector2 m_startVelocity = Vector2.zero;
    float m_timeStep = 0.0f;
    Vector2 m_launchDirection = Vector2.zero;

    public void CreateTrajectoryLine(float a_ballLaunchSpeed)
    {
        // set the point count of the line to its default
        m_lineRenderer.positionCount = m_linePointCount;

        // set the first point of the line to the spawn point of the ball
        m_lineRenderer.SetPosition(0, m_ballSpawnPoint.position);

        // determine the direction the launcher is facing in 2D space
        m_launchDirection.x = m_ballSpawnPoint.up.x;
        m_launchDirection.y = m_ballSpawnPoint.up.y;

        // determine the start velocity using the speed the ball is launched, the direction launched and the mass of the ball
        m_startVelocity = a_ballLaunchSpeed * m_launchDirection / m_ballRigidbody.mass;

        // loop for each point to be placed along the line, starting at the second point
        for (int i = 1; i < m_linePointCount; ++i)
        {
            // use the following formula to determine the position of the next point
            // Displacement = InitialVelocity * Time + 0.5 * Acceleration * Time * Time
            // S = ut + ½at²
            m_lineRenderer.SetPosition(i, (Vector2)m_ballSpawnPoint.position + m_startVelocity * (i * m_timeStep) + 0.5f * Physics2D.gravity * (i * m_timeStep) * (i * m_timeStep));

            // create a circlecast, with the width of the ball's diameter, between this point and the previous point
            RaycastHit2D hit = Physics2D.CircleCast(m_lineRenderer.GetPosition(i - 1), m_ballRigidbody.transform.localScale.x * 0.5f, (m_lineRenderer.GetPosition(i) - m_lineRenderer.GetPosition(i - 1)).normalized, (m_lineRenderer.GetPosition(i) - m_lineRenderer.GetPosition(i - 1)).magnitude);
            // if the raycast hit a collider that is not a trigger
            if (hit && !hit.collider.isTrigger)
            {
                // set the position of this point to the position the ball would be when it hits the collider
                m_lineRenderer.SetPosition(i, hit.centroid);
                // adjust the point count of the line renderer now that this is the final point
                m_lineRenderer.positionCount = i + 1;
                // exit the loop
                return;
            }
        }

    }

    public void ShowLine(bool a_showLine)
    {
        // enable/disable the line renderer
        m_lineRenderer.enabled = a_showLine;
    }

    void Awake()
    {
        // get the line renderer component from this gameobject
        m_lineRenderer = GetComponent<LineRenderer>();

        // determine the amount of time the ball will travel along it's path before the next point is created, given the amount of points and the max length of the line
        m_timeStep = m_maxLineLength / m_linePointCount / 10.0f;
    }
}
