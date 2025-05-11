


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
	Last Modified: 12/05/2025
*/
public class SaveFile : MonoBehaviour
{
    public string m_saveFilePath = "";
    public int m_maxSaves = 3;
    string m_fullSavePath = "";
    [HideInInspector] public int m_SaveFileCount;
    [HideInInspector] public int[,] m_topScores;
    [HideInInspector] public int m_lastCompletedLevel = 0;
    StreamWriter m_streamWriter;
    StreamReader m_streamReader;

    [Header("Save File Data")]
    public string m_saveName = "";
    public AudioSource m_musicAudioSource;
    public AudioSource m_feverAudioSource;
    public AudioSource m_soundEffectAudioSource;
    public Toggle m_fullscreenToggle;
    public Toggle m_colorblindToggle;
    public int m_volumeSavePrecision = 2;
    string m_fileName = "SaveFile";
    string m_fileType = ".txt";

    public string GetSaveFileName(int a_saveID)
    {
        // if a file exists for this slot
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

    public void UpdateSaveFile(int a_saveID = 0)
    {
        // create the save file, or open and clear it if it already exists
        m_streamWriter = File.CreateText(m_fullSavePath + m_fileName + a_saveID + m_fileType);

        // add the save name to the top of the save file
        m_streamWriter.WriteLine(m_saveName);

        // get the music volume (with decimal places as per m_volumeSavePrecision) and add it to the first line of the save file
        string line = m_musicAudioSource.volume.ToString("N" + m_volumeSavePrecision) + ",";
        // get the fever volume (with decimal places as per m_volumeSavePrecision) and add it to the first line of the save file
        line += m_feverAudioSource.volume.ToString("N" + m_volumeSavePrecision) + ",";
        // get the sound effect volume (with decimal places as per m_volumeSavePrecision) and add it to the first line of the save file
        line += m_soundEffectAudioSource.volume.ToString("N" + m_volumeSavePrecision) + ",";
        // add the fullscreen mode to the first line of the save file as a boolean
        line += (Screen.fullScreenMode == FullScreenMode.FullScreenWindow) ? "1" : "0" + ",";
        // add the colorblind setting to the first line of the save file as a boolean
        line += (m_colorblindToggle.isOn) ? "1" : "0" + ",";

        // write the line into the save file
        m_streamWriter.WriteLine(line);

        // loop for each stage
        for (int i = 0; i < GlobalSettings.m_stageCount; ++i)
        {
            // reset the line string
            line = "";

            // loop for each level within a stage
            for (int j = 0; j < GlobalSettings.m_levelsPerStage; ++j)
            {
                // add the top score to the line
                line += m_topScores[i, j].ToString() + ",";
            }

            // store the high scores of the stage in the save file
            m_streamWriter.WriteLine(line);
        }

        // close the file
        m_streamWriter.Dispose();

        // TEMP
        print("File accessed and written to");
    }

    public bool ReadSaveFile(int a_saveID = 0)
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
                m_saveName = line;

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
                                m_musicAudioSource.volume = float.Parse(settingValue);
                                break;
                            case 1:
                                m_feverAudioSource.volume = float.Parse(settingValue);
                                break;
                            case 2:
                                m_soundEffectAudioSource.volume = float.Parse(settingValue);
                                break;
                            case 3:
                                m_fullscreenToggle.isOn = (settingValue == "1");
                                break;
                            case 4:
                                m_colorblindToggle.isOn = (settingValue == "1");
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
                                m_topScores[i, levelNumber] = int.Parse(topScore);
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

    void Awake()
    {
        // determine the save file location using the application's save location and the specified location for the save file
        m_fullSavePath = Application.dataPath + "/" + m_saveFilePath;
        // initialise the high scores array to have an element for each level in the game
        m_topScores = new int[GlobalSettings.m_stageCount, GlobalSettings.m_levelsPerStage];

        // read the current save, if it does not exist
        if (!ReadSaveFile(GlobalSettings.m_currentSaveID))
        {
            // TEMP
            print("could not read file");
            // CREATE NEW SAVE
        }
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

