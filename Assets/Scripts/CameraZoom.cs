using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
    File name: CameraZoom.cs
    Summary: Allows the camera to zoom in and track a target or return to its default state
    Creation Date: 04/12/2023
    Last Modified: 04/12/2023
*/
public class CameraZoom : MonoBehaviour
{
    public float m_maxZoom = 2.0f;
    public float m_zoomSpeed = 2.0f;
    public float m_returnMoveSpeed = 5.0f;
    public float m_maxValidSquaredDistanceFromDefaultPosition = 0.05f;

    GameObject m_target;
    bool m_tracking = false;
    Camera m_camera;
    float m_defaultZoom;
    Vector3 m_defaultCameraPosition;
    Vector3 m_cameraPosition;

    public void ReturnToDefault()
    {
        // store that the camera should no longer be tracking
        m_tracking = false;
        // show the cursor if it had been hidden
        Cursor.visible = true;

    }

    public void ZoomAndTrack(GameObject a_target = null)
    {
        // store that the camera should be tracking
        m_tracking = true;

        // if there is a target provided, track it instead of the cursor
        if (a_target != null)
        {
            // store the target
            m_target = a_target;
        }
        
    }

    // Start is called before the first frame update
    void Start()
    {
        // get the camera component
        m_camera = GetComponent<Camera>();
        // store the starting size as the default zoom
        m_defaultZoom = m_camera.orthographicSize;
        // store the starting position as the default position
        m_defaultCameraPosition = transform.position;
        // initialise the camera position vector with the default position
        m_cameraPosition = m_defaultCameraPosition;
    }

    // Update is called once per frame
    void Update()
    {
        // TEMP
        //print((Input.mousePosition.x - Screen.width * 0.5f) + ", " + (Input.mousePosition.y - Screen.height * 0.5f));

        // if the camera should be tracking a target
        if (m_tracking)
        {
            // if the camera isn't fully zoomed in
            if (m_camera.orthographicSize > m_maxZoom)
            {
                // increase the camera zoom and clamp it to valid bounds
                m_camera.orthographicSize = Mathf.Clamp(m_camera.orthographicSize - m_zoomSpeed * Time.deltaTime, m_maxZoom, m_defaultZoom);
            }

            // set the camera's x and y position to the target's position, or the cursor if there is no target object
            m_cameraPosition.x = (m_target != null) ? m_target.transform.position.x : Input.mousePosition.x - Screen.width * 0.5f;
            m_cameraPosition.y = (m_target != null) ? m_target.transform.position.y : Input.mousePosition.y - Screen.height * 0.5f;
            transform.position = m_cameraPosition;

        }
        // if the camera shouldn't be tracking a target
        else
        {
            // if the camera isn't fully zoomed out
            if (m_camera.orthographicSize < m_defaultZoom)
            {
                // reduce the camera zoom and clamp it to valid bounds
                m_camera.orthographicSize = Mathf.Clamp(m_camera.orthographicSize + m_zoomSpeed * Time.deltaTime, m_maxZoom, m_defaultZoom);
            }

            // if the camera hasn't returned to its default position
            if ((transform.position - m_defaultCameraPosition).sqrMagnitude > m_maxValidSquaredDistanceFromDefaultPosition)
            {
                // move the camera towards its default position
                transform.position = Vector3.MoveTowards(transform.position, m_defaultCameraPosition, m_returnMoveSpeed * Time.deltaTime);

                // if the camera is now close enough to the target position
                if ((transform.position - m_defaultCameraPosition).sqrMagnitude <= m_maxValidSquaredDistanceFromDefaultPosition)
                {
                    // return the camera to the default position
                    transform.position = m_defaultCameraPosition;
                }
            }
        }
    }
}
