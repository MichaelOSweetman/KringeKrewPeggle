using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/*
    File name: Dialogue.cs
    Summary: Procedurely fills a text box with a message, moving to the next message when prompted by the player
    Creation Date: 10/06/2024
    Last Modified: 04/05/2025
*/
public class Dialogue : MonoBehaviour
{
	[System.Serializable] public struct Character
	{
		public string m_title;
		public Texture[] m_expressions;
	}

	[System.Serializable] public struct Message
	{
		public int m_characterID;
		public string m_message;
		public int m_expressionID;
	}

	[System.Serializable] public struct MessageSet
	{
		public Message[] m_messages;
	}


	Text m_dialogueTextBox;
	public UIManager m_uiManager;
	public RawImage m_characterPortrait;
	public Text m_speakerText;
	public float m_generationSpeed = 0.2f;
	public bool m_generateEachMessageOverSameDuration = false;
	public List<Character> m_characters;
	public List<MessageSet> m_dialogueSets;
	[HideInInspector] public int m_dialogueIndex = 0;
	float m_letterDelay = 0.0f;
	float m_timer;
	int m_messageIndex = 0;
	int m_characterIndex = 0;
	

    void Awake()
    {
		// get the text component on this textbox
		m_dialogueTextBox = GetComponent<Text>();

		// determine the delay between letters appearing depending on if m_generationSpeed should represent the time for the full message to appear or each character
		m_letterDelay = (m_generateEachMessageOverSameDuration) ? m_dialogueSets[m_dialogueIndex].m_messages[m_messageIndex].m_message.Length / m_generationSpeed : m_generationSpeed;
	}

    // Update is called once per frame
    void Update()
    {
		// if there is a message to display and it isn't already finished
		if (m_dialogueSets[m_dialogueIndex].m_messages[m_messageIndex].m_message.Length > 0 && m_dialogueTextBox.text.Length < m_dialogueSets[m_dialogueIndex].m_messages[m_messageIndex].m_message.Length)
		{
			// update the timer
			m_timer += Time.fixedDeltaTime;
			
			// if enough time has passed
			if (m_timer >= m_letterDelay)
			{
				// reset the timer
				m_timer -= m_letterDelay;
				// add the next character to the text box
				m_dialogueTextBox.text += m_dialogueSets[m_dialogueIndex].m_messages[m_messageIndex].m_message[m_characterIndex];
				++m_characterIndex;
			}
		}
		
		// if the player presses the Shoot / Use Power button
		if (Input.GetButtonDown("Shoot / Use Power"))
		{
			// if the message isn't finished
			if (m_dialogueTextBox.text.Length < m_dialogueSets[m_dialogueIndex].m_messages[m_messageIndex].m_message.Length)
			{
				// instantly complete the message
				m_dialogueTextBox.text = m_dialogueSets[m_dialogueIndex].m_messages[m_messageIndex].m_message;
			}
			// otherwise, if there are no more messages to display
			else if (m_messageIndex == m_dialogueSets[m_dialogueIndex].m_messages.Length - 1)
			{
				// have the ui manager close this dialogue screen
				m_uiManager.CloseDialogueScreen();
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

				// determine the delay between characters appearing depending on if m_generationSpeed should represent the time for the full message to appear or each character
				m_letterDelay = (m_generateEachMessageOverSameDuration) ? m_dialogueSets[m_dialogueIndex].m_messages[m_messageIndex].m_message.Length / m_generationSpeed : m_generationSpeed;

				// update the speaker text
				m_speakerText.text = m_characters[m_dialogueSets[m_dialogueIndex].m_messages[m_messageIndex].m_characterID].m_title;

				// set the expression of the character speaking to the corresponding expression
				m_characterPortrait.texture = m_characters[m_dialogueSets[m_dialogueIndex].m_messages[m_messageIndex].m_characterID].m_expressions[m_dialogueSets[m_dialogueIndex].m_messages[m_messageIndex].m_expressionID];
			}
		}
    }
}