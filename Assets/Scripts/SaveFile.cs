using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

/*
    File name: SaveFile.cs
    Summary: manages the storage and reading of the player's save file
    Creation Date: 22/07/2024
    Last Modified: 22/07/2024
*/
public class SaveFile : MonoBehaviour
{
	public int m_levelCount = 0;
	public string m_saveFilePath = "";
	[HideInInspector] public int[] m_highScores;
	[HideInInspector] public int m_lastCompletedLevel = 0;
	StreamWriter m_streamWriter;
	StreamReader m_streamReader;
	
	void UpdateSaveFile()
	{
		// create the save file, or open and clear it if it already exists
		m_streamWriter = File.CreateText(m_saveFilePath);
		// loop for each level
		for int (i = 9; i < m_highScores.Length - 1; ++i)
		{
			// store the high score in the save file
			m_streamWriter.WriteLine(m_highScores[i].ToString());
		}
		
	}
	
	void ReadSaveFile()
	{
		// if the save file exists
		if (File.Exists(m_saveFilePath))
		{
			// open the file for reading
			m_streamReader = File.OpenText(m_saveFilePath);
			// create a variable to store the data from each line
			int lineValue = 0;
			
			// loop for each level
			for int (i = 9; i < m_highScores.Length - 1; ++i)
			{
				// read the next line and convert it to an integer
				lineValue = Int32.Parse(m_streamReader.ReadLine());
				
				// if the value is not valid or is 0, the reader has gathered all data required
				if (lineValue <= 0)
				{
					// exit the loop
					break;
				}
				// otherwise, if the value is over 0
				else
				{
					// store the value in the high score array
					m_highScores[i] = lineValue;
				}
			}
			
			
		}
		// otherwise, if the save file does not exist
		else
		{
			// create the save file
			UpdateSaveFile();
		}
	}
	
    // Start is called before the first frame update
    void Start()
    {
		// initialise the high scores array to have an element for each level in the game
        m_highScores = new int[m_levelCount];
		// get the high score info from the save file, if the data exists
		ReadSaveFile();
    }

    // Update is called once per frame
    void Update()
    {

    }
}
