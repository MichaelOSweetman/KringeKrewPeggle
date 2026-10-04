using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/*
    File name: GlobalSettings.cs
    Summary: Stores settings and variables that need to be passed between scenes
    Creation Date: 22/12/2024
    Last Modified: 05/10/2026
*/
public class GlobalSettings : MonoBehaviour
{
    public static string  m_currentSavePath = "";
    public static string  m_saveName;
    public static int     m_currentLevelID = 0;
    public static int     m_currentStageID = 0;
    public static int     m_stageCount = 2;
    public static int     m_levelsPerStage = 3;
    public static float   m_musicVolume = 0.0f;
    public static float   m_feverVolume = 0.0f;
    public static float   m_soundEffectVolume = 0.0f;
    public static bool    m_colorblindMode = false;
    public static bool    m_adventureMode = false;
    public static int[,]  m_topScores;
}
