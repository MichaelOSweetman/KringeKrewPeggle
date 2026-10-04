using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

/*
	File name: SaveReader.cs
	Summary: reads the save file in the main menu
	Creation Date: 22/07/2024
	Last Modified: 05/10/2026
*/
public class SaveReader : MonoBehaviour
{
    public MainMenuManager m_mainMenuManager;
    public string m_saveFilePath = "";
    public string m_fileName = "SaveFile";
    public string m_fileType = ".txt";
    public int m_maxSaves = 3;
    public bool m_alwaysUpdateColorblind = true;
    string m_fullSavePath = "";
    StreamReader m_streamReader;
    int[,] m_readScores;

    public string GetSaveFileName(int a_saveID)
    {
        // if a file exists for this ID
        if (File.Exists(m_fullSavePath + m_fileName + a_saveID + m_fileType))
        {
            // open the file
            m_streamReader = File.OpenText(m_fullSavePath + m_fileName + a_saveID + m_fileType);
            // read and store the first line
            string saveFileName = m_streamReader.ReadLine();
            // close the file
            m_streamReader.Dispose();
            // return the save file name
            return saveFileName;
        }
        else
        {
            return "";
        }
    }

    public void DeleteSaveFile(int a_saveID)
    {
        // delete the file
        File.Delete(m_fullSavePath + m_fileName + a_saveID + m_fileType);
    }

    public bool ReadSaveFile(int a_saveID = 0, bool a_applySettings = true)
    {
        // store that the active save file is now this file
        GlobalSettings.m_currentSavePath = m_fullSavePath + m_fileName + a_saveID + m_fileType;

        // if the save file exists
        if (File.Exists(GlobalSettings.m_currentSavePath))
        {
            // open the file for reading
            m_streamReader = File.OpenText(GlobalSettings.m_currentSavePath);
            // create a variable to store the data from each line. Get the first line
            string line = m_streamReader.ReadLine();

            // if the line is not empty
            if (line != null && line != "")
            {
                //store the first line as the save name
                GlobalSettings.m_saveName = line;

                // read the next line, which stores setting data
                line = m_streamReader.ReadLine();

                // if all settings should be applied, or just the colourblind setting
                if (a_applySettings || m_alwaysUpdateColorblind)
                {
                    // store the settings index
                    int settingIndex = 0;
                    // store the characters associated with the current setting
                    string settingValue = "";

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
                                    GlobalSettings.m_colorblindMode = (settingValue == "1");
                                    // update the colourblind toggle's state to reflect whether it is active
                                    m_mainMenuManager.SetColorblindToggle();
                                    // if the other settings should not be applied, stop reading the line prematurely
                                    i = (a_applySettings) ? i : line.Length;                                   
                                    break;
                                case 1:
                                    GlobalSettings.m_musicVolume = float.Parse(settingValue);
                                    break;
                                case 2:
                                    GlobalSettings.m_feverVolume = float.Parse(settingValue);
                                    break;
                                case 3:
                                    GlobalSettings.m_soundEffectVolume = float.Parse(settingValue);
                                    break;
                                case 4:
                                    // set the fullscreen mode to fullscreen or windowed, as per the stored value
                                    Screen.fullScreenMode = (settingValue == "1") ? FullScreenMode.FullScreenWindow : FullScreenMode.Windowed;
                                    // update the fullscreen toggle's state to reflect whether it is active
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
                }

                // store the last stage and level as the current
                GlobalSettings.m_currentStageID = GlobalSettings.m_stageCount;
                GlobalSettings.m_currentLevelID = GlobalSettings.m_levelsPerStage;

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
                                //if the score for this level is empty
                                if (topScore == "-1")
                                {
                                    // store this stage and level as the current
                                    GlobalSettings.m_currentStageID = i;
                                    GlobalSettings.m_currentLevelID = j;
                                    // exit the stage loop prematurely as this and each subsequent score can be left as -1
                                    i = GlobalSettings.m_stageCount;
                                    // exit the level loop
                                    break;
                                }

                                // add the top score to the scores array
                                m_readScores[i, levelNumber] = int.Parse(topScore);
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

                // store the read scores in the global settings top scores array
                GlobalSettings.m_topScores = m_readScores;

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
        // initialise the scores array to have an element for each level in the game
        m_readScores = new int[GlobalSettings.m_stageCount, GlobalSettings.m_levelsPerStage];
        // set the initial scores in the array to -1
        for (int i = 0; i < GlobalSettings.m_stageCount; ++i)
        {
            for (int j = 0; j < GlobalSettings.m_levelsPerStage; ++j)
            {
                m_readScores[i,j] = -1;
            }
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        // determine most recently accessed save
        // read most recently accessed save, apply saved settings
        // TEMP
        ReadSaveFile();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}