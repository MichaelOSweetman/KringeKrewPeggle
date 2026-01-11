using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;

/*
	File name: LokiPower.cs
	Summary: Manages the power gained from the green peg when playing as Loki
	Creation Date: 27/01/2025
	Last Modified: 12/01/2026
*/
public class LokiPower : GreenPegPower
{

    public GameObject m_cordPrefab;
    public GameObject m_hookPrefab;
    public float m_maxCordLength = 5.0f;
    public float m_hookLaunchSpeed = 30.0f;
    public float m_pullSpeed = 7.5f;
    LineRenderer m_cord;
    GameObject m_hook;
    GameObject m_connectionPoint;
    bool m_connectedToPeg = false;

    public void ConnectHook(GameObject a_connectionPeg)
    {
        // set the connection point of the cord to be the peg
        m_connectionPoint = a_connectionPeg;
        // store that the end of the cord has been connected to a peg
        m_connectedToPeg = true;
        // make the hook inactive as the peg has replaced it
        m_hook.SetActive(false);
        // expend a power charge
        ModifyPowerCharges(-1);
    }

    public override void Initialize()
    {
        // create the hook and cord. Give the hook access to this component
        m_hook = Instantiate(m_hookPrefab);
        m_hook.GetComponent<Hook>().m_lokiPower = this;
        GameObject cord = Instantiate(m_cordPrefab);
        m_cord = cord.GetComponent<LineRenderer>();
    }

    public override void Trigger(Vector3 a_greenPegPosition)
	{
        // add the charges
        ModifyPowerCharges(m_gainedPowerCharges);
    }

    public override void ResolveTurn()
    {
        // hide the cord if it is visible
        m_cord.gameObject.SetActive(false);

        // disable the hook
        m_hook.SetActive(false);
    }

    public override void ResolvePower()
    {
        
    }

    public override void Reload()
	{
        // clear the connection point
        m_connectionPoint = null;

        // destroy the cord and hook
        Destroy(m_cord.gameObject);
        Destroy(m_hook);

        // store that the ball is not connected to a peg
        m_connectedToPeg = false;

        // reset the power charges
        ResetPowerCharges();
    }

    public override void Update()
    {
        // if the ball is in play
        if (m_playerControls.m_ballInPlay)
        {
            // if the shoot / use power button is currently pressed
            if (Input.GetButton("Shoot / Use Power"))
            {
                // if there are power charges and this is the first frame the shoot / use power button has been pressed
                if (m_powerCharges > 0 && Input.GetButtonDown("Shoot / Use Power"))
                {
                    // initialise the hook
                    m_hook.SetActive(true);
                    m_hook.transform.position = m_playerControls.m_ball.transform.position;

                    // have the end of the cord be the hook
                    m_connectionPoint = m_hook;

                    // shoot the hook towards the cursor
                    m_hook.GetComponent<Rigidbody2D>().AddForce((Camera.main.ScreenToWorldPoint(Input.mousePosition) - m_playerControls.m_ball.transform.position).normalized * m_hookLaunchSpeed, ForceMode2D.Impulse);

                    // have the cord be active
                    m_cord.gameObject.SetActive(true);

                    // store that the ball is not currently connected to a peg
                    m_connectedToPeg = false;
                }

                if (m_connectedToPeg)
                {
                    // pull the ball towards the peg it is connected to
                    m_playerControls.m_ball.GetComponent<Rigidbody2D>().AddForce((m_connectionPoint.transform.position - m_playerControls.m_ball.transform.position).normalized * m_pullSpeed, ForceMode2D.Force);
                }


                // if the cord is currently active
                if (m_cord.gameObject.activeSelf)
                {
                    // if the connection point is null or inactive, or if the cord has gone beyond its max length
                    if (m_connectionPoint == null || !m_connectionPoint.activeSelf || (m_playerControls.m_ball.transform.position - m_hook.transform.position).sqrMagnitude >= m_maxCordLength * m_maxCordLength)
                    {
                        // make the cord and hook inactive
                        m_cord.gameObject.SetActive(false);
                        m_hook.SetActive(false);

                        // store that the ball is not connected to a peg
                        m_connectedToPeg = false;
                    }
                    // if the cord is still not too long
                    else
                    {
                        // draw a line between the ball and the connection point
                        m_cord.SetPosition(0, m_playerControls.m_ball.transform.position);
                        m_cord.SetPosition(1, m_connectionPoint.transform.position);
                    }
                }
            }
            else
            {
                // disable the cord
                m_cord.gameObject.SetActive(false);
                // disable the hook
                m_hook.SetActive(false);
                // store that the ball is not connected to a peg
                m_connectedToPeg = false;
            }
        }
    }
}
