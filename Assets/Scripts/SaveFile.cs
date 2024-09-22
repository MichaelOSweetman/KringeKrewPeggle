using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

/*
    File name: SaveFile.cs
    Summary: manages the storage and reading of the player's save file
    Creation Date: 22/07/2024
    Last Modified: 23/09/2024
*/
public class SaveFile : MonoBehaviour
{
	public UIManager m_uiManager;
	public int m_levelSetCount = 0;
	public int m_levelsPerSet = 0;
	public string m_saveFilePath = "";
	string m_fullSavePath = "";
	[HideInInspector] public int[,] m_highScores;
	[HideInInspector] public int m_lastCompletedLevel = 0;
	StreamWriter m_streamWriter;
	StreamReader m_streamReader;


    [Header("Settings")]
    public AudioSource m_musicAudioSource;
    public AudioSource m_feverAudioSource;
    public AudioSource m_soundEffectAudioSource;
    public Toggle m_fullscreenToggle;
    public Toggle m_colorblindToggle;
    public int m_volumeSavePrecision = 2;

    void UpdateSaveFile()
	{
		// create the save file, or open and clear it if it already exists
		m_streamWriter = File.CreateText(m_fullSavePath);

		string line = "";

		// get the music volume (with decimal places as per m_volumeSavePrecision) and add it to the first line of the save file
		line += m_musicAudioSource.volume.ToString("n" + m_volumeSavePrecision.ToString() + ",");
        // get the fever volume (with decimal places as per m_volumeSavePrecision) and add it to the first line of the save file
        line += m_feverAudioSource.volume.ToString("n" + m_volumeSavePrecision.ToString() + ",");
        // get the sound effect volume (with decimal places as per m_volumeSavePrecision) and add it to the first line of the save file
        line += m_soundEffectAudioSource.volume.ToString("n" + m_volumeSavePrecision.ToString() + ",");
		// add the fullscreen mode to the first line of the save file as a boolean
		line += (Screen.fullScreenMode == FullScreenMode.FullScreenWindow).ToString() + ",";
		// add the colorblind setting to the first line of the save file as a boolean
		line += m_colorblindToggle.isOn.ToString();

        // loop for each level set 
        for (int i = 0; i < m_levelSetCount; ++i)
		{
			// reset the line string
			line = "";
			
			// loop for each level
			for (int j = 0; j < m_levelsPerSet; ++j)
			{
				// add the high score to the line
				line += m_highScores[i, j].ToString() + ",";
			}
			
			// store the high scores of the level set in the save file
			m_streamWriter.WriteLine(line);
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
			// create a variable to store the data from each line. Get the first line
			string line = m_streamReader.ReadLine();
            
			// if the line is empty
            if (line == null || line == "")
            {
                // TEMP
                print("File not found, creating file");

                // close the file for reading
                m_streamReader.Dispose();
                // create the save file
                UpdateSaveFile();
				// exit this function
				return;

            }
            // otherwise, if the line is not empty
            else
            {
                // loop through the line
                for (int j = 0; j < line.Length; ++j)
                {
					int settingIndex = 0;
					string settingValue = "";

					// loop through the line
					for (int i = 0; i < line.Length; ++i)
					{
						if (line[j] == ',')
						{
							switch (settingIndex)
							{
								case 0:
									m_musicAudioSource.volume = float.Parse(settingValue);
									break;
                                case 1:
                                    m_feverAudioSource.volume = float.Parse(settingValue);
                                    break;
                                case 2:
                                    m_soundEffectAudioSource.volume = float.Parse(settingValue);
                                    break;
                                case 3:
									m_fullscreenToggle.isOn = bool.Parse(settingValue);
                                    break;
                                case 4:
									m_colorblindToggle.isOn = bool.Parse(settingValue);
                                    break;
                            }

							// move to the next setting
							++settingIndex;
							// reset the setting value variable
							settingValue = "";
						}
						else
						{
							// add the character to the current high score value
							settingValue += line[i];
						}
					}
				}
            }

            // loop for each level set
            for (int i = 0; i < m_levelSetCount; ++i)
			{
				// read the next line
				line = m_streamReader.ReadLine();
				
				// if the line is empty, the reader has gathered all data required
				if (line == null || line == "")
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
					for (int j = 0; j < line.Length; ++j)
					{
						if (line[j] == ',')
						{
							// add the high score to the high scores array
							m_highScores[i, levelNumber] = int.Parse(highScore);
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
            // TEMP
            print("File not found, creating file");

            // create the save file
            UpdateSaveFile();
        }
	}
	
    // Start is called before the first frame update
    void Start()
    {
		// determine the save file location using the application's save location and the specified location for the save file
		m_fullSavePath = Application.dataPath + "/" + m_saveFilePath;
		// initialise the high scores array to have an element for each level in the game
        m_highScores = new int[m_levelSetCount, m_levelsPerSet];
		// get the high score info from the save file, if the data exists
		ReadSaveFile();
    }

    // Update is called once per frame
    void Update()
    {
		// TEMP
		if (Input.GetKeyDown(KeyCode.K))
		{
			UpdateSaveFile();
		}
    }
}
