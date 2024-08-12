using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

/*
    File name: SaveFile.cs
    Summary: manages the storage and reading of the player's save file
    Creation Date: 22/07/2024
    Last Modified: 05/08/2024
*/
public class SaveFile : MonoBehaviour
{
	public int m_levelSetCount = 0;
	public int m_levelsPerSet = 0;
	public string m_saveFilePath = "";
	string m_fullSavePath = Application.dataPath;
	[HideInInspector] public int[][] m_highScores;
	[HideInInspector] public int m_lastCompletedLevel = 0;
	StreamWriter m_streamWriter;
	StreamReader m_streamReader;
	
	void UpdateSaveFile()
	{
		// create the save file, or open and clear it if it already exists
		m_streamWriter = File.CreateText(m_fullSavePath);
		
		string m_line = "";
		
		// loop for each level set 
		for (int i = 0; i < m_levelSetCount - 1; ++i)
		{
			// reset the line string
			m_line = "";
			
			// loop for each level
			for (int j = 0; j < m_levelSetCount - 1; ++j)
			{
				// add the high score to the line
				m_line += m_highScores[i][j].ToString() + ",";
			}
			
			// store the high scores of the level set in the save file
			m_streamWriter.WriteLine(m_line);
		}

		// close the file
		m_streamWriter.Dispose();

		// TEMP
		print("File accessed and written to");
	}
	
	void ReadSaveFile()
	{
		// if the save file exists
		if (File.Exists(m_fullSavePath))
		{
			// open the file for reading
			m_streamReader = File.OpenText(m_fullSavePath);
			// create a variable to store the data from each line
			string line = "";
			
			// loop for each level set
			for (int i = 0; i < m_levelSetCount - 1; ++i)
			{
				// read the next line
				line = m_streamReader.ReadLine();
				
				// if the line is empty, the reader has gathered all data required
				if (lineValue == "")
				{
					// exit the loop
					break;
				}
				// otherwise, if the line is not empty
				else
				{
					int levelNumber = 0;
					string highScore = "";
					// loop through the line
					for (int j = 0; j < line.Length - 1; ++j)
					{
						if (line[j] == ",")
						{
							// add the high score to the high scores array
							m_highScores[i][levelNumber] = int.Parse(highScore);
							// move to the next level
							++levelNumber;
							// reset the highscore variable for the next level
							highScore = "";
						}
						else
						{
							// add the character to the current high score value
							highScore += line[j];
						}
					}
				}
			}
			
			// close the file
			m_streamReader.Dispose();

			// TEMP
			print("File found and read");
		}
		// otherwise, if the save file does not exist
		else
		{
			// create the save file
			UpdateSaveFile();
			// TEMP
			print("File not found, creating file");
		}
	}
	
    // Start is called before the first frame update
    void Start()
    {
		// determine the save file location using the application's save location and the specified location for the save file
		m_fullSavePath += "\" + m_saveFilePath;
		// initialise the high scores array to have an element for each level in the game
        m_highScores = new int[m_levelSetCount][m_levelsPerSet];
		// get the high score info from the save file, if the data exists
		ReadSaveFile();
    }

    // Update is called once per frame
    void Update()
    {
		// TEMP
		if (Input.GetKeyDown(KeyCode.K))
		{
			m_highScores[0] = 6;
			UpdateSaveFile();
		}
    }
}
