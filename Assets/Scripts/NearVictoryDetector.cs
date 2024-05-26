using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
    File name: NearVictoryDetector.cs
    Summary: Prompts the camera to zoom in towards the ball when it is in the detector's space
    Creation Date: 04/12/2023
    Last Modified: 27/05/2023
*/
public class NearVictoryDetector : MonoBehaviour
{
    [HideInInspector] public PlayerControls m_playerControls;
    [HideInInspector] public CameraZoom m_cameraZoom;
    List<GameObject> m_ballsInRange;

    // Start is called before the first frame update
    void Start()
    {
        // get the Camera Zoom script from Player Controls
        m_cameraZoom = m_playerControls.m_cameraZoom;

        // create a list to store all balls that are in range of the detector
        m_ballsInRange = new List<GameObject>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D a_collision)
    {
        // if a ball has triggered the detector
        if (a_collision.gameObject.CompareTag("Ball"))
        {
            // if there are currently no balls in range of the detector
            if (m_ballsInRange.Count == 0)
            {
                // tell the camera to zoom and track this ball
                m_cameraZoom.ZoomAndTrack(a_collision.gameObject);
                // slow down the time scale
                m_playerControls.ModifyTimeScale(m_playerControls.m_nearVictoryTimeScale);
            }
            
            // add the ball to the list of balls in range
            m_ballsInRange.Add(a_collision.gameObject);
        }
    }

    private void OnTriggerExit2D(Collider2D a_collision)
    {
        // try to remove the object from the balls in range list. If it was in the list
        if (m_ballsInRange.Remove(a_collision.gameObject))
        {
            // if there is still at least 1 ball in range
            if (m_ballsInRange.Count > 0)
            {
                // tell the camera to zoom and track the first listed ball instead
                m_cameraZoom.ZoomAndTrack(m_ballsInRange[0]);
            }
            // if there are no other balls in range
            else
            {
                // tell the camera to return to its default zoom and position
                m_cameraZoom.ReturnToDefault();
                // reset the time scale
                m_playerControls.ModifyTimeScale(m_playerControls.m_defaultTimeScale);
            }
        }
    }
}
