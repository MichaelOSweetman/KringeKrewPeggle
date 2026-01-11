using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
	File name: LevelManager.cs
	Summary: Manages the loading of specific levels and stages of the game
	Creation Date: 27/04/2025
	Last Modified: 12/01/2026
*/
public class LevelManager : MonoBehaviour
{
    [System.Serializable] public struct Level
    {
        public GameObject m_level;
        public int m_dialogueIndex;
    }

    [System.Serializable] public struct Stage
    {
        public GameObject m_stageContainer;
        public Level[] m_levels;
        public int m_defaultPowerID;
    }

    public PegManager m_pegManager;
    public UIManager m_uiManager;
    public MusicManager m_musicManager;
    public Stage[] m_stages;

    public void LoadNextLevel()
    {
        // if the current level is the last level of its stage
        if (GlobalSettings.m_currentLevelID > m_stages[GlobalSettings.m_currentStageID].m_levels.Length)
        {
            // if the current stage is the last stage
            if (GlobalSettings.m_currentStageID > m_stages.Length)
            {
                // load the first level of the first stage
                LoadLevel(0, 0);
            }
            // if the current level is not the last level of the last stage
            else
            {
                // load the first level of the next stage
                LoadLevel(GlobalSettings.m_currentStageID + 1, 0);
            }
        }
        // if the current level is not the last level of its stage
        else
        {
            // load the level of the next level but the current stage
            LoadLevel(GlobalSettings.m_currentStageID, GlobalSettings.m_currentLevelID + 1);
        }
    }

    public void LoadLevel(int a_stageID, int a_levelID)
    {
        // store the argument stage and level as the current level
        GlobalSettings.m_currentStageID = a_stageID;
        GlobalSettings.m_currentLevelID = a_levelID;

        // make each stage inactive
        for (int i = 0; i < m_stages.Length; ++i)
        {
            // set the stage gameobject, which is the parent transform of its levels, to be inactive
            m_stages[i].m_stageContainer.SetActive(false);
        }

        // make the current stage active
        m_stages[a_stageID].m_stageContainer.SetActive(true);

        // make each level within the current stage inactive
        for (int i = 0; i < m_stages[a_stageID].m_levels.Length; ++i)
        {
            m_stages[a_stageID].m_levels[i].m_level.SetActive(false);
        }

        // make the current level active
        m_stages[a_stageID].m_levels[a_levelID].m_level.SetActive(true);

        // if the game is in adventure mode
        if (GlobalSettings.m_adventureMode)
        {
            // have the UI Manager set assets for the default character of the level's stage
            m_uiManager.LockInCharacter(true);

            // if this level has a valid dialogue index
            if (m_stages[a_stageID].m_levels[a_levelID].m_dialogueIndex >= 0)
            {
                // have the ui manager switch to the dialogue screen and use the dialogue specified by the level's dialogue index
                m_uiManager.SwitchToDialogue(m_stages[a_stageID].m_levels[a_levelID].m_dialogueIndex);
            }
        }
        // if the game is not in adventure mode
        else
        {
            // Have the UI Manager show the character select screen
            m_uiManager.ShowCharacterSelectScreen();
        }

        // reshuffle the music to play during the level
        m_musicManager.ShufflePlay();

        // have the UI manager reset game UI components for the new level
        m_uiManager.ReloadGameUI();

        // have the UI Manager update the top score text
        m_uiManager.UpdateTopScoreText();

        // load the pegs of the level
        m_pegManager.LoadLevel(m_stages[a_stageID].m_levels[a_levelID].m_level.transform);
    }

    // Start is called before the first frame update
    void Start()
    {
        // load the current level
        LoadLevel(GlobalSettings.m_currentStageID, GlobalSettings.m_currentLevelID);
    }

    // Update is called once per frame
    void Update()
    {
        // TEMP
        if (Input.GetKeyDown(KeyCode.P))
        {
            LoadNextLevel();
        }
    }
}
