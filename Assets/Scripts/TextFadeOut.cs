using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/*
    File name: TextFadeOut.cs
    Summary: Sets a text object to reduce its opacity to 0 over time after a set duration and then destroy itself
    Creation Date: 06/11/2023
    Last Modified: 04/05/2025
*/

public class TextFadeOut : MonoBehaviour
{
    public float m_durationBeforeFade = 3.0f;
    public float m_lowerOpacitySpeed = 10.0f;
    Text m_text;
    Color m_textColor;

    void Awake()
    {
        // get the text component
        m_text = GetComponent<Text>();
        // get the colour of the text
        m_textColor = m_text.color;
    }

    // Update is called once per frame
    void Update()
    {
        // if there is still time before the text should begin to fade
        if (m_durationBeforeFade > 0.0f)
        {
            // reduce the timer
            m_durationBeforeFade -= Time.unscaledDeltaTime;
        }
        // if the fading process has begun
        else
        {
            // reduce the opacity of the text
            m_textColor.a -= m_lowerOpacitySpeed * Time.unscaledDeltaTime;
            m_text.color = m_textColor;

            // if the text is now invisible
            if (m_text.color.a <= 0.0f)
            {
                // destroy this gameobject
                Destroy(gameObject);
            }
        }
    }
}
