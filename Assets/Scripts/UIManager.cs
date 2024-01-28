using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
    File name: UIManager.cs
    Summary: Manages UI buttons and transitions
    Creation Date: 29/01/2024
    Last Modified: 29/01/2024
*/
public class UIManager : MonoBehaviour
{
    public PlayerControls m_playerControls;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void EndDrawButtonPressed()
    {
        // turn off the drawing UI elements
        m_playerControls.m_endDrawButton.SetActive(false);
        m_playerControls.m_clearButton.SetActive(false);
        m_playerControls.m_inkResourceBar.SetActive(false);

        // take the player out of drawing mode
        m_playerControls.m_drawing = false;
    }

    public void ClearDrawingButtonPressed()
    {
        // reset the amount of ink
        m_playerControls.m_ink = m_playerControls.m_maxSecondsOfInk;

        // clear the line
    }
}
