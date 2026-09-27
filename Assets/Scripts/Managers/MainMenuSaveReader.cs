using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

/*
	File name: MainMenuSaveReader.cs
	Summary: reads the save file in the main menu
	Creation Date: 22/07/2024
	Last Modified: 27/09/2026
*/
public class MainMenuSaveReader : MonoBehaviour
{
    public string m_saveFilePath = "";
    public string m_fileName = "SaveFile";
    public string m_fileType = ".txt";
    public int m_maxSaves = 3;
    public bool m_alwaysUpdateColorblind = true;
    string m_fullSavePath = "";
    StreamWriter m_streamWriter;
    StreamReader m_streamReader;

    public bool ReadSaveFile(int a_saveID = 0, bool a_applySettings = true)
    {
        // store that the active save file is now this file
        GlobalSettings.m_currentSaveID = a_saveID;

        // if the save file exists
        if (File.Exists(m_fullSavePath + m_fileName + a_saveID + m_fileType))
        {
            // open the file for reading
            m_streamReader = File.OpenText(m_fullSavePath + m_fileName + a_saveID + m_fileType);
            // create a variable to store the data from each line. Get the first line
            string line = m_streamReader.ReadLine();

            // if the line is not empty
            if (line != null && line != "")
            {
                int settingIndex = 0;
                string settingValue = "";

                //store the first line as the save name
                GlobalSettings.m_saveName = line;

                // read the next line
                line = m_streamReader.ReadLine();

                // loop through the line
                for (int i = 0; i < line.Length; ++i)
                {
                    // if the current character is a ',' then the data for the current setting has been read and is ready to be applied
                    if (line[i] == ',')
                    {
                        // apply the value to the corresponding setting
                        switch (settingIndex)
                        {
                            case 0:
                                GlobalSettings.m_musicVolume = float.Parse(settingValue);
                                break;
                            case 1:
                                GlobalSettings.m_feverVolume = float.Parse(settingValue);
                                break;
                            case 2:
                                GlobalSettings.m_soundEffectVolume = float.Parse(settingValue);
                                break;
                            case 3:
                                // TEMP - act on value, don't need to store, can be determined from whether window is fullscreen
                                // call for MainMenuManager.cs to change toggle
                                //m_fullscreenToggle.isOn = (settingValue == "1");
                                break;
                            case 4:
                                GlobalSettings.m_colorblindMode = (settingValue == "1");
                                // call for MainMenuManager.cs to change toggle
                                //m_colorblindToggle.isOn = (settingValue == "1");
                                break;
                        }

                        // move to the next setting
                        ++settingIndex;
                        // reset the setting value variable
                        settingValue = "";
                    }
                    else
                    {
                        // add the character to the setting value string
                        settingValue += line[i];
                    }
                }

                // loop for each stage
                for (int i = 0; i < GlobalSettings.m_stageCount; ++i)
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
                        string topScore = "";
                        // loop through the line
                        for (int j = 0; j < line.Length; ++j)
                        {
                            if (line[j] == ',')
                            {
                                // add the top score to the top scores array
                                GlobalSettings.m_topScores[i, levelNumber] = int.Parse(topScore);
                                // move to the next level
                                ++levelNumber;
                                // reset the topscore variable for the next level
                                topScore = "";
                            }
                            else
                            {
                                // add the character to the current top score value
                                topScore += line[j];
                            }
                        }
                    }
                }

                // close the file
                m_streamReader.Dispose();
                return true;
            }

            // close the file
            m_streamReader.Dispose();
        }

        return false;
    }

    private void Awake()
    {
        // determine the save file location using the application's save location and the specified location for the save file
        m_fullSavePath = Application.dataPath + "/" + m_saveFilePath;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
/*
 Move needed functions and variables from SaveFile.cs, modify function to only serve main menu
 remove unneeded functions and variables from SaveFile.cs

global settings top scores array will probably need to be initialised, may need to fill local array and copy it to global settings
add conditional logic to ReadSaveFile to only apply settings changes when specified, with the optional exception of the Colourblind toggle
 */