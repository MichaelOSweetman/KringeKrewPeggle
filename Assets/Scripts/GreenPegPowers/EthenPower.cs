using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
	File name: EthenPower.cs
	Summary: Manages the power gained from the green peg when playing as Ethen
	Creation Date: 27/01/2025
	Last Modified: 03/02/2025
*/
public class EthenPower : GreenPegPower
{
	public new int m_gainedPowerCharges = 2;
    public RectTransform m_drawingBounds;
    public GameObject m_lines;
    public GameObject m_linePrefab;
    public GameObject m_endDrawButton;
    public GameObject m_clearButton;
    public GameObject m_inkResourceBarBackground;
    public float m_maxInk = 500.0f;
    public float m_minValidSquareMouseMovement = 0.05f;
    [HideInInspector] public float m_ink = 0.0f;
    [HideInInspector] public bool m_drawing = false;
    bool m_lineBegun = false;
    LineRenderer m_currentLineRenderer;
    RectTransform m_inkResourceBar;
    float m_inkResourceBarMaxWidth = 0.0f;
    Vector3 m_previousMousePosition = Vector3.zero;

    public bool CursorWithinPlayArea()
    {
        // get the cursor position in screen space
        Vector3 cursorPosition = Input.mousePosition;
        //cursorPosition.x -= m_canvas.GetComponent<RectTransform>().rect.width * 0.5f;
        //cursorPosition.y += m_canvas.GetComponent<RectTransform>().rect.height * 0.5f;
        Vector3 minCorner = new Vector3(m_drawingBounds.rect.x, m_drawingBounds.rect.y, 1.0f);
        Vector3 maxCorner = new Vector3(m_drawingBounds.rect.xMax, m_drawingBounds.rect.yMax, 1.0f);

        //print(cursorPosition + "| " + (m_playAreaBounds.transform.position + minCorner) + "| " + (m_playAreaBounds.transform.localPosition + maxCorner));

        // return whether the cursor is within the play area bounds (converted to screen space)
        return
        (
            cursorPosition.x > m_drawingBounds.transform.position.x - (0.5f * m_drawingBounds.rect.width) &&
            cursorPosition.x < m_drawingBounds.transform.position.x + (0.5f * m_drawingBounds.rect.width) &&
            cursorPosition.y > m_drawingBounds.transform.position.y - (0.5f * m_drawingBounds.rect.height) &&
            cursorPosition.y < m_drawingBounds.transform.position.y + (0.5f * m_drawingBounds.rect.height)

        //cursorPosition.x > (m_playAreaBounds.transform.position + minCorner).x &&
        //cursorPosition.y > (m_playAreaBounds.transform.position + minCorner).y &&
        //cursorPosition.x < (m_playAreaBounds.transform.position + maxCorner).x &&
        //cursorPosition.y < (m_playAreaBounds.transform.position + maxCorner).y
        //cursorPosition.x < Camera.main.WorldToScreenPoint(m_playAreaBounds.transform.localPosition - m_playAreaBounds.rect.x) &&
        //cursorPosition.y > Camera.main.WorldToScreenPoint(m_playAreaBounds.transform.localPosition - m_playAreaBounds.rect.y)
        );
    }

    public void UpdateLine(Vector3 a_newLinePoint)
    {
        // ensure the z component of the new point is 0
        a_newLinePoint.z = 0;

        // add the new position to the current line
        ++m_currentLineRenderer.positionCount;
        m_currentLineRenderer.SetPosition(m_currentLineRenderer.positionCount - 1, a_newLinePoint);
    }

    public void UpdateInkResourceBar()
    {
        // modify the ink resource bar to be representative of the amount of ink remaining relative to the max ink
        m_inkResourceBar.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, Mathf.Clamp01(m_ink / m_maxInk) * m_inkResourceBarMaxWidth);
    }

    public void DestroyLines(bool a_onlyDestroyIfHit = false)
    {
        // loop for each line
        for (int i = m_lines.transform.childCount - 1; i >= 0; --i)
        {
            // if the line should be destroyed regardless or if the line has been hit and the only destroy if hit arguement is true
            if (!a_onlyDestroyIfHit || m_lines.transform.GetChild(i).GetComponent<Line>().m_hit)
            {
                // destroy the current line
                Destroy(m_lines.transform.GetChild(i).gameObject);
            }
        }
    }

	public override void SetUp()
	{
        // set the UI elements required for drawing to be active
        m_endDrawButton.SetActive(true);
        m_clearButton.SetActive(true);
        m_inkResourceBarBackground.SetActive(true);

        // reset the ink meter
        m_ink = m_maxInk;
        UpdateInkResourceBar();

        // disable the ball trajectory
        m_playerControls.m_ballTrajectory.ShowLine(false);

        // turn off the LookAtCursor component of the launcher
        m_playerControls.m_LauncherLookControls.enabled = false;

        // put the player in drawing mode
        m_drawing = true;

        // initialise the previous mouse position variable
        m_previousMousePosition = Input.mousePosition;
    }

	public override void OnShoot()
	{

	}

	public override void Resolve()
	{

	}

	public override void Reload()
	{
        // Destroy all lines
        DestroyLines();

        // turn off the drawing UI elements
        m_endDrawButton.SetActive(false);
        m_clearButton.SetActive(false);
        m_inkResourceBarBackground.SetActive(false);

        // take the player out of drawing mode
        m_drawing = false;

        // store that a line has not begun being drawn
        m_lineBegun = false;

        // ensure the LookAtCursor component of the launcher is on
        m_playerControls.m_LauncherLookControls.enabled = true;
    }
}
