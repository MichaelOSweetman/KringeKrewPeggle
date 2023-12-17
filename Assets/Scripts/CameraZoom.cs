using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
    File name: CameraZoom.cs
    Summary: Allows the camera to zoom in and track a target or return to its default state
    Creation Date: 04/12/2023
    Last Modified: 18/12/2023
*/
public class CameraZoom : MonoBehaviour
{
    [Header("Zoom")]
    public float m_maxZoom = 2.0f;
    public float m_zoomSpeed = 20.0f;
    [HideInInspector] public bool m_atMaxZoom = false;

    [Header("Return To Default Position")]
    public float m_returnMoveSpeed = 5.0f;
    public float m_maxValidSquaredDistanceFromDefaultPosition = 0.05f;

    [Header("Camera Bounds")]
    public float m_minHorizontalCameraBounds = -5.0f;
    public float m_maxHorizontalCameraBounds = 5.0f;
    public float m_minVerticalCameraBounds = -4.0f;
    public float m_maxVerticalCameraBounds = 4.0f;

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
        
        // if there is not a target provided
        if (a_target == null)
        {
            // set the target to be the current mouse position
            m_cameraPosition = m_camera.ScreenToWorldPoint(Input.mousePosition);
            // hide the cursor
            Cursor.visible = false;
        }
        // if there is a target provided
        else
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
        // if the camera should be tracking a target
        if (m_tracking)
        {
            // if the camera isn't fully zoomed in
            if (m_camera.orthographicSize > m_maxZoom)
            {
                // increase the camera zoom and clamp it to valid bounds
                m_camera.orthographicSize = Mathf.Clamp(m_camera.orthographicSize - m_zoomSpeed * Time.deltaTime, m_maxZoom, m_defaultZoom);
            }
            // if the camera is fully zoomed in
            else
            {
                // store that the camera is at max zoom
                m_atMaxZoom = true;
            }

            // move the camera's x and y coordinates to the target if there is one, or move it based on the mouse movements this frame if there isn't
            m_cameraPosition.x = Mathf.Clamp((m_target == null) ? m_cameraPosition.x + Input.GetAxis("Mouse X") : m_target.transform.position.x, m_minHorizontalCameraBounds, m_maxHorizontalCameraBounds);
            m_cameraPosition.y = Mathf.Clamp((m_target == null) ? m_cameraPosition.y + Input.GetAxis("Mouse Y") : m_target.transform.position.y, m_minVerticalCameraBounds, m_maxVerticalCameraBounds);

            // move the camera to its designated position
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

            // store that the camera is not at max zoom
            m_atMaxZoom = false;

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
