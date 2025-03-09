using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
	File name: EthenPower.cs
	Summary: Manages the power gained from the green peg when playing as Ethen
	Creation Date: 27/01/2025
	Last Modified: 10/03/2025
*/
public class EthenPower : GreenPegPower
{
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

    public void EndDrawButtonPressed()
    {
        // turn off the drawing UI elements
        m_endDrawButton.SetActive(false);
        m_clearButton.SetActive(false);
        m_inkResourceBarBackground.SetActive(false);

        // take the player out of drawing mode
        m_drawing = false;

        // enable player controls
        m_playerControls.enabled = true;

        // turn on the LookAtCursor component of the launcher
        m_playerControls.m_LauncherLookControls.enabled = true;

        // turn on the ball trajectory
        m_playerControls.m_ballTrajectory.ShowLine(true);

        // reduce the amount of power charges
        ModifyPowerCharges(-1);

        // if there are still power charges
        if (m_powerCharges > 0)
        {
            // have the drawing power get set up next turn
            m_playerControls.m_setUpPowerNextTurn = true;
        }
    }

    public void ClearDrawingButtonPressed()
    {
        // reset the amount of ink
        m_ink = m_maxInk;
        UpdateInkResourceBar();

        // destroy all lines
        DestroyLines();

    }

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

    public void DestroyLines()
    {
        // loop for each line
        for (int i = m_lines.transform.childCount - 1; i >= 0; --i)
        {
            // destroy the current line
            Destroy(m_lines.transform.GetChild(i).gameObject);
        }
    }

    public override void Initialize()
    {
        // get the child of the ink resource bar background as the ink resource bar
        m_inkResourceBar = m_inkResourceBarBackground.transform.GetChild(0).GetComponent<RectTransform>();

        // get the width of the ink resource bar background
        m_inkResourceBarMaxWidth = m_inkResourceBarBackground.GetComponent<RectTransform>().sizeDelta.x;
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

        // disable player controls
        m_playerControls.enabled = false;

        // put the player in drawing mode
        m_drawing = true;

        // initialise the previous mouse position variable
        m_previousMousePosition = Input.mousePosition;
    }

    public override void ResolveTurn()
    {
        // Destroy any active lines
        DestroyLines();
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

        // ensure that player controls is enabled
        m_playerControls.enabled = true;

        // ensure the LookAtCursor component of the launcher is on
        m_playerControls.m_LauncherLookControls.enabled = true;

        // turn on the ball trajectory
        m_playerControls.m_ballTrajectory.ShowLine(true);

        // reset the power charges
        ResetPowerCharges();
    }

    public override void Update()
    {
        // if drawing mode is on
        if (m_drawing)
        {
            // if there is ink remaining, the Shoot / Use Power input is currently pressed and the cursor is within the play area bounds
            if (m_ink > 0.0f && Input.GetButton("Shoot / Use Power") && CursorWithinPlayArea())
            {
                // if this was the first frame that the Shoot / Use Power input was pressed
                if (Input.GetButtonDown("Shoot / Use Power"))
                {
                    // store that the player has started drawing a line
                    m_lineBegun = true;
                }
                // otherwise, if the mouse has moved enough since last frame
                else if ((Input.mousePosition - m_previousMousePosition).sqrMagnitude >= m_minValidSquareMouseMovement)
                {
                    // if the player started drawing a line previously
                    if (m_lineBegun == true)
                    {
                        // create a new line object and make it a child of the lines game object
                        GameObject line = Instantiate(m_linePrefab, m_lines.transform) as GameObject;

                        // give the line the peg manager
                        line.GetComponent<Line>().m_pegManager = m_playerControls.m_pegManager;

                        // store the line renderer for the current line
                        m_currentLineRenderer = line.GetComponent<LineRenderer>();

                        // Update the line with the previous point
                        UpdateLine(Camera.main.ScreenToWorldPoint(m_previousMousePosition));

                        // Update the line with the new point
                        UpdateLine(Camera.main.ScreenToWorldPoint(Input.mousePosition));

                        // store that the line has no longer begun being drawn
                        m_lineBegun = false;
                    }

                    // reduce the amount of ink the player has based on the amount the mouse moved
                    m_ink -= (Input.mousePosition - m_previousMousePosition).magnitude;

                    // update the ink resource bar
                    UpdateInkResourceBar();

                    // Update the line with the new point
                    UpdateLine(Camera.main.ScreenToWorldPoint(Input.mousePosition));
                }
            }
            // otherwise, if a line has been drawn, the line doesn't currently have a collider and the Shoot / Use Power input was released
            else if (m_currentLineRenderer != null && m_currentLineRenderer.gameObject.GetComponent<EdgeCollider2D>() == null && m_currentLineRenderer.positionCount > 0 && Input.GetButtonUp("Shoot / Use Power"))
            {
                // add an edge collider to the line
                EdgeCollider2D collider = m_currentLineRenderer.gameObject.AddComponent<EdgeCollider2D>();

                // convert the line renderer points to vector2
                Vector2[] points = new Vector2[m_currentLineRenderer.positionCount];
                for (int i = 0; i < points.Length; ++i)
                {
                    points[i] = new Vector2(m_currentLineRenderer.GetPosition(i).x, m_currentLineRenderer.GetPosition(i).y);
                }

                // give the vector2 line points to the collider
                collider.points = points;
            }

            // store the mouse position for next frame
            m_previousMousePosition = Input.mousePosition;
        }
    }
}
