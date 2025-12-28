using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/*
    File name: RoundScore.cs
    Summary: Manages the text display of the score gained in a round
    Creation Date: 29/12/2025
    Last Modified: 29/12/2025
*/
public class RoundScore : MonoBehaviour
{
    public Transform m_levelScoreText;
    public float m_increaseDuration;
    Text m_text;
    Vector3 m_defaultPosition = Vector3.zero;
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
        // move to the default position
        transform.position = m_defaultPosition;

        // store the amount that the score value will increase each time it is increased
        m_scoreUnit = a_scoreUnit;
        // store the amount of times the displayed score will increase
        m_increaseFrequency = a_increaseFrequency;

        // determine the amount of time between each score increase update in order for fulfill the full m_increaseDuration duration
        m_lap = m_increaseDuration / m_increaseFrequency;
    }

    private void Awake()
    {
        // get this game object's text component
        m_text = GetComponent<Text>();
        // store this game object's current position as the default position
        m_defaultPosition = transform.position;
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

            // if the timer has gone off long enough for the score to update
            if (m_timer > m_lap)
            {
                m_score += m_scoreUnit;
                ++m_increaseCount;
                m_timer -= m_lap;

                // if the score has increased enough times
                if (m_increaseCount == m_increaseFrequency)
                {
                    // start moving to totalscore process
                }
            }

            // display the score information
            m_text.text = m_scoreUnit + " x " + m_increaseFrequency + "\n" + m_score;
        }


    }

    /*
     * appears at center of screen
     * increases by same increment over set duration
     * lingers
     * moves and shrinks towards totalscore text
     * disappears
     */
}
