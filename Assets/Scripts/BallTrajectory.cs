using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
    File name: BallTrajectory.cs
    Summary: Uses a line renderer to show the player the predicted path of the ball if it were fired this frame
    Creation Date: 02/12/2024
    Last Modified: 02/12/2024
*/
public class BallTrajectory : MonoBehaviour
{
    public int m_linePointCount = 10;
    public float m_maxLineLength = 10.0f;
    public Transform m_ballSpawnPoint;
    public Rigidbody2D m_ballRigidbody;
    LineRenderer m_lineRenderer;
    Vector2 m_startVelocity = Vector2.zero;
    float m_distanceBetweenPoints = 0.0f;
    Vector2 m_launchDirection = Vector2.zero;

    public void CreateTrajectoryLine(float a_ballLaunchSpeed)
    {
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
            m_lineRenderer.SetPosition(i, (Vector2)m_ballSpawnPoint.position + m_startVelocity * (i * m_distanceBetweenPoints) + 0.5f * Physics2D.gravity * (i * m_distanceBetweenPoints) * (i * m_distanceBetweenPoints));
        }
    }

    public void ShowLine(bool a_showLine)
    {
        // enable/disable the line renderer
        m_lineRenderer.enabled = a_showLine;
    }

    // Start is called before the first frame update
    void Start()
    {
        // get the line renderer component from this gameobject
        m_lineRenderer = GetComponent<LineRenderer>();

        // initialise the line renderer with the amount of positions along the line
        m_lineRenderer.positionCount = m_linePointCount;

        // determine the distance between each point along the line given the amount of points and the max length of the line
        m_distanceBetweenPoints = m_maxLineLength / m_linePointCount;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
