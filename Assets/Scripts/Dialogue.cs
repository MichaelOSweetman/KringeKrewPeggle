using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/*
    File name: Dialogue.cs
    Summary: Procedurely fills a text box with a message, moving to the next message when prompted by the player
    Creation Date: 10/06/2024
    Last Modified: 19/08/2024
*/
public class Dialogue : MonoBehaviour
{
	public struct Character
	{
		SpriteRenderer m_portrait;
		Sprite[] m_expressions;
	}

	public struct Message
	{
		Character m_character;
		string m_message;
		int m_expressionID;
	}

	Text m_dialogueTextBox;
	public PegManager m_pegManager;
	public List<List<Message>> m_dialogueSets;
	[HideInInspector] public int m_dialogueIndex = 0;
	public float m_generationSpeed = 0.2f;
	float m_characterDelay = 0.0f;
	public bool m_generateEachMessageOverSameDuration = false;
	public bool m_loadNextLevelOnComplete = true;
	float m_timer;
	int m_messageIndex = 0;
	int m_characterIndex = 0;
	

    // Start is called before the first frame update
    void Start()
    {
		// get the text component on this textbox
		m_dialogueTextBox = GetComponent<Text>();

		// determine the delay between characters appearing depending on if m_generationSpeed should represent the time for the full message to appear or each character
		m_characterDelay = (m_generateEachMessageOverSameDuration) ? m_dialogueSets[m_dialogueIndex][m_messageIndex].m_message.Length / m_generationSpeed : m_generatationSpeed;
    }

    // Update is called once per frame
    void Update()
    {
		// if there is a message to display and it isn't already finished
		if (m_dialogueSets[m_dialogueIndex][m_messageIndex].m_message.Length > 0 && m_dialogueTextBox.text.Length < m_dialogueSets[m_dialogueIndex][m_messageIndex].m_message.Length)
		{
			// update the timer
			m_timer += Time.fixedDeltaTime;
			
			// if enough time has passed
			if (m_timer >= m_characterDelay)
			{
				// reset the timer
				m_timer -= m_generationSpeed;
				// add the next character to the text box
				m_dialogueTextBox.text += m_dialogueSets[m_dialogueIndex][m_messageIndex].m_message[m_characterIndex];
				++m_characterIndex;
			}
		}
		
		// if the player presses the Shoot / Use Power button
		if (Input.GetButtonDown("Shoot / Use Power"))
		{
			// if the message isn't finished
			if (m_dialogueTextBox.text.Length < m_dialogueSets[m_dialogueIndex][m_messageIndex].m_message.Length)
			{
				// instantly complete the message
				m_dialogueTextBox.text = m_dialogueSets[m_dialogueIndex][m_messageIndex].m_message;
			}
			// otherwise, if there are no more messages to display
			else if (m_messageIndex == m_dialogueSets[m_dialogueIndex].Count - 1)
			{
				// if the next level should be loaded now that the dialogue has completed
				if (m_loadNextLevelOnComplete)
				{
					// have the peg manager load the next level
					m_pegManager.LoadNextLevel();
				}

				// make this textbox inactive
				gameObject.SetActive(false);
			}
			// otherwise, if there are more messages to display and the current message is finished
			else
			{
				// set the current speaking character's expression to the default
				m_dialogueSets[m_dialogueIndex][m_messageIndex].m_character.m_portrait.sprite = m_dialogueSets[m_dialogueIndex][m_messageIndex].m_character.m_expressions[0];

				// clear the text box
				m_dialogueTextBox.text = "";
				// reset the character index
				m_characterIndex = 0;
				// get the next message
				++m_messageIndex;

				// determine the delay between characters appearing depending on if m_generationSpeed should represent the time for the full message to appear or each character
				m_characterDelay = (m_generateEachMessageOverSameDuration) ? m_dialogueSets[m_dialogueIndex][m_messageIndex].m_message.Length / m_generationSpeed : m_generatationSpeed;

				// set the expression of the character speaking to the corresponding expression
				m_dialogueSets[m_dialogueIndex][m_messageIndex].m_character.m_portrait.sprite = m_dialogueSets[m_dialogueIndex][m_messageIndex].m_character.m_expressions[m_dialogueSets[m_dialogueIndex][m_messageIndex].m_expressionID];
			}
		}
    }
}