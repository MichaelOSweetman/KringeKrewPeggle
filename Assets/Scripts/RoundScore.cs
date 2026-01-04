using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/*
    File name: RoundScore.cs
    Summary: Manages the text display of the score gained in a round
    Creation Date: 29/12/2025
    Last Modified: 05/01/2026
*/
public class RoundScore : MonoBehaviour
{
    public Transform m_levelScoreText;
    public float m_openingDelay = 1.0f;
    public float m_increaseDuration = 0.4f;
    public float m_displayDuration = 1.0f;
    public float m_moveSpeed = 15.0f;
    public float m_sizeLossRate = 0.05f;
    public float m_maxValidSquaredDistanceFromTarget = 0.05f;
    Text m_text;
    Vector3 m_defaultPosition = Vector3.zero;
    Vector3 m_defaultScale = Vector3.zero;
    float m_timer = 0.0f;
    int m_scoreUnit = 0;
    int m_increaseFrequency = 0;
    int m_score = 0;
    float m_lap = 0.0f;
    int m_increaseCount = 0;

    public void Activate(int a_scoreUnit, int a_increaseFrequency)
    {
        // show the text
        m_text.enabled = true;
        // reset the timer
        m_timer = 0.0f;
        // move to the default position
        transform.position = m_defaultPosition;
        // ensure the text is at the default scale
        transform.localScale = m_defaultScale;

        // reset the increase count
        m_increaseCount = 0;
        // reset the score
        m_score = 0;

        // store the amount that the score value will increase each time it is increased
        m_scoreUnit = a_scoreUnit;
        // store the amount of times the displayed score will increase
        m_increaseFrequency = a_increaseFrequency;

        // determine the amount of time between each score increase update in order for fulfill the full m_increaseDuration duration
        m_lap = m_increaseDuration / m_increaseFrequency;

        // set the starting text display
        m_text.text = m_scoreUnit.ToString("#,#") + " X PEGS";
    }

    private void Awake()
    {
        // get this game object's text component
        m_text = GetComponent<Text>();
        // store this game object's current position as the default position
        m_defaultPosition = transform.position;
        // store this game object's current scale as the default scale
        m_defaultScale = transform.localScale;
    }

    void Start()
    {
        m_text.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        // if the text is enabled
        if (m_text.enabled)
        {
            // increase the timer
            m_timer += Time.unscaledDeltaTime;

            // if the score has not yet increased enough times to reach the total round score
            if (m_increaseCount < m_increaseFrequency)
            {
                // if the timer has gone on long enough for the score to update
                if (m_timer > m_lap + m_openingDelay)
                {
                    // increase the round score by a score unit
                    m_score += m_scoreUnit;
                    // update the count for the amount of times the round score has increased
                    ++m_increaseCount;
                    // reduce the timer by the lap delay, leaving the opening delay elapsed
                    m_timer -= m_lap;
                }

                // if the score has increased at least once
                if (m_increaseCount > 0)
                {
                    // display the score information
                    m_text.text = m_scoreUnit.ToString("#,#") + " X " + m_increaseCount.ToString() + " PEGS" + "\n" + m_score.ToString("#,#");
                }

            }
            // if the total round score has been reached and the score has been displayed for enough time
            else if (m_timer > m_displayDuration + m_openingDelay)
            {
                // display the score
                m_text.text = m_score.ToString("#,#");
                // move the round score text towards the level score text
                transform.position = Vector3.MoveTowards(transform.position, m_levelScoreText.position, m_moveSpeed);
                // reduce the scale of this gameobject
                transform.localScale = Vector3.MoveTowards(transform.localScale, Vector3.zero, m_sizeLossRate);

                // if the round score text is close enough to the level score text
                if ((transform.position - m_levelScoreText.position).sqrMagnitude <= m_maxValidSquaredDistanceFromTarget)
                {
                    // disable the text
                    m_text.enabled = false;
                }
            }
        }

    }
}
