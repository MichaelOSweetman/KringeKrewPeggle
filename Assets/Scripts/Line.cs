using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
    File name: Line.cs
    Summary: manages hit response of lines generated by the Ethen Power
    Creation Date: 26/02/2024
    Last Modified: 04/05/2025
*/
public class Line : MonoBehaviour
{
    public PegManager m_pegManager;
    public Color m_hitLineColor;
    LineRenderer m_lineRenderer;

    void Awake()
    {
        // get the line renderer
        m_lineRenderer = GetComponent<LineRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter2D(Collision2D a_collision)
    {
        // if this line was hit by a ball
        if (a_collision.gameObject.CompareTag("Ball"))
        {
            // add the line to the hit peg queue in the peg manager
            m_pegManager.m_hitPegs.Enqueue(gameObject);

            // change the line's colour to its hit version
            m_lineRenderer.startColor = m_hitLineColor;
            m_lineRenderer.endColor = m_hitLineColor;
        }
    }
}
