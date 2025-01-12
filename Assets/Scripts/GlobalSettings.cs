using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/*
    File name: GlobalSettings.cs
    Summary: Stores settings and variables that need to be passed between scenes
    Creation Date: 22/12/2024
    Last Modified: 06/01/2025
*/
public class GlobalSettings : MonoBehaviour
{
    [HideInInspector] public static float m_musicVolume = 0.0f;
    [HideInInspector] public static float m_feverVolume = 0.0f;
    [HideInInspector] public static float m_soundEffectVolume = 0.0f;
    [HideInInspector] public static bool m_colorblindMode = false;
    [HideInInspector] public static bool m_adventureMode = true;
    [HideInInspector] public static int m_currentSaveID = 0;
    [HideInInspector] public static int m_currentStageID = 0;
    [HideInInspector] public static int m_currentLevelID = 0;
    [HideInInspector] public static int m_stageCount = 2;
    [HideInInspector] public static int m_levelsPerSet = 3;
}
