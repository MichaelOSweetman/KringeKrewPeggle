using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/*
    File name: Dialogue.cs
    Summary: Procedurely fills a text box with a message, moving to the next message when prompted by the player
    Creation Date: 10/06/2024
    Last Modified: 17/06/2024
*/
public class Dialogue : MonoBehaviour
{
	Text m_dialogueTextBox;
	public List<string> m_messages;
	public float m_generationSpeed = 0.2f;
	float m_timer;
	int m_messageIndex = 0;
	int m_characterIndex = 0;
	
    // Start is called before the first frame update
    void Start()
    {
		// get the text component on this textbox
		m_dialogueTextBox = GetComponent<Text>();
    }

    // Update is called once per frame
    void Update()
    {
		// if there is a message to display and it isn't already finished
		if (m_messages[m_messageIndex].Length > 0 && m_dialogueTextBox.text.Length < m_messages[m_messageIndex].Length)
		{
			// update the timer
			m_timer += Time.fixedDeltaTime;
			
			// if enough time has passed
			if (m_timer >= m_generationSpeed)
			{
				// reset the timer
				m_timer -= m_generationSpeed;
				// add the next character to the text box
				m_dialogueTextBox.text += m_messages[m_messageIndex][m_characterIndex];
				++m_characterIndex;
			}
		}
		
		// if the player presses the Shoot / Use Power button
		if (Input.GetButtonDown("Shoot / Use Power"))
		{
			// if the message isn't finished
			if (m_dialogueTextBox.text.Length < m_messages[m_messageIndex].Length)
			{
				// instantly complete the message
				m_dialogueTextBox.text = m_messages[m_messageIndex];
			}
			// otherwise, if there are no more messages to display
			else if (m_messageIndex == m_messages.Count - 1)
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
				++m_messageIndex;
			}
		}
    }
}