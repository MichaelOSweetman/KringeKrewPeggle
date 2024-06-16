using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/*
    File name: Dialogue.cs
    Summary: Procedurely fills a text box with a message, moving to the next message when prompted by the player
    Creation Date: 10/06/2024
    Last Modified: 10/06/2024
*/
public class SniperBallIndicator : MonoBehaviour
{
	Text m_dialogueTextBox;
	public Queue<string> m_messages;
	public float m_generationSpeed = 0.2f;
	float m_timer;
	string m_currentMessage = "";
	int m_characterIndex = 0;
	
    // Start is called before the first frame update
    void Start()
    {
        m_currentMessage = m_messages.Dequeue();
    }

    // Update is called once per frame
    void Update()
    {
		// if there is a message to display and it isn't already finished
		if (!m_currentMessage.Empty && m_dialogueTextBox.text.Length < m_currentMessage.Length)
		{
			// update the timer
			m_timer += Time.fixedDeltaTime;
			
			// if enough time has passed
			if (m_timer >= m_generationSpeed)
			{
				// reset the timer
				m_timer -= m_generationSpeed;
				// add the next character to the text box
				m_dialogueTextBox.text += m_currentMessage[m_characterIndex];
				++m_characterIndex;
			}
		}
		
		// if the player presses the Shoot / Use Power button
		if (Input.GetButton("Shoot / Use Power"))
		{
			// if the message isn't finished
			if (m_dialogueTextBox.text.Length < m_currentMessage.Length)
			{
				// instantly complete the message
				m_dialogueTextBox.text = m_currentMessage;
			}
			// otherwise, if there are no more messages to display
			else if (m_messages.Count == 0)
			{
				// make this textbox inactive
				this.gameObject.SetActive(false);
			}
			// otherwise, if there are more messages to display and the current message is finished
			else
			{
				// clear the text box
				m_dialogueTextBox.text = "";
				// reset the character index
				m_characterIndex = 0;
				// get the next message
				m_currentMessage = m_messages.Dequeue();
			}
		}
    }
}