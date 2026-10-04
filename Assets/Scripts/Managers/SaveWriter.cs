


using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

/*
	File name: SaveWriter.cs
	Summary: writes to the save file in the gameplay scene
	Creation Date: 22/07/2024
	Last Modified: 05/10/2026
*/
public class SaveWriter : MonoBehaviour
{
    public int m_volumeSavePrecision = 2;
    StreamWriter m_streamWriter;

    public void UpdateSaveFile(int a_saveID = 0)
    {
        // create the save file, or open and clear it if it already exists
        m_streamWriter = File.CreateText(GlobalSettings.m_currentSavePath);

        // add the save name to the top of the save file
        m_streamWriter.WriteLine(GlobalSettings.m_saveName);
        
        // add the colorblind setting to the first line of the save file as a boolean
        string line = ((GlobalSettings.m_colorblindMode) ? "1" : "0") + ",";
        // get the music volume (with decimal places as per m_volumeSavePrecision) and add it to the first line of the save file
        line += GlobalSettings.m_musicVolume.ToString("N" + m_volumeSavePrecision) + ",";
        // get the fever volume (with decimal places as per m_volumeSavePrecision) and add it to the first line of the save file
        line += GlobalSettings.m_feverVolume.ToString("N" + m_volumeSavePrecision) + ",";
        // get the sound effect volume (with decimal places as per m_volumeSavePrecision) and add it to the first line of the save file
        line += GlobalSettings.m_soundEffectVolume.ToString("N" + m_volumeSavePrecision) + ",";
        // add the fullscreen mode to the first line of the save file as a boolean
        line += ((Screen.fullScreenMode == FullScreenMode.FullScreenWindow) ? "1" : "0") + ",";


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
                line += GlobalSettings.m_topScores[i, j].ToString() + ",";
            }

            // store the high scores of the stage in the save file
            m_streamWriter.WriteLine(line);
        }

        // close the file
        m_streamWriter.Dispose();

        // TEMP
        print("File accessed and written to");
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

/*
 X Open Game
 - /Main Menu/
 - Read most recently accessed save file
 X Determine latest scored level to infer next level for adventure mode
 X Apply saved settings
 X If player opens the change save sub menu and selects a different save
       X load selected save
       X only update colourblind toggle (have inspector toggle for this "always update colourblind")
 - /Game Scene/
 if new high score or current level/stage changes
    - save to file, including settings changes
- when game closes
       - save game changes

X ReadSaveFile needs optional applying settings
X Save File should maybe be split into a script for main menu and game scene 
    X main menu reads and stores in global settings
    X game scene updates new data and saves changes
 */