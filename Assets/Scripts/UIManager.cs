using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
    File name: UIManager.cs
    Summary: Manages UI buttons and transitions
    Creation Date: 29/01/2024
    Last Modified: 26/02/2024
*/
public class UIManager : MonoBehaviour
{
    public PlayerControls m_playerControls;
    public PegManager m_pegManager;

    [Header("UI Screens")]
    public GameObject m_levelComplete;
    public GameObject m_tryAgain;

    public void LevelOver(bool a_won)
    {
        // turn off the player controls
        m_playerControls.enabled = false;

        // if the player won the level
        if (a_won)
        {
            // show the level complete screen
            m_levelComplete.SetActive(true);
        }
        // if the player lost the level
        else
        {
            // show the try again screen
            m_tryAgain.SetActive(true);
        }
    }

    public void NextLevel()
    {
        // reactivate player controls
        m_playerControls.enabled = true;
        // load the next level
        m_pegManager.LoadNextLevel();
        // hide the level complete screen
        m_levelComplete.SetActive(false);
    }

    public void RetryLevel()
    {
        // reactivate player controls
        m_playerControls.enabled = true;
        // reload the current level
        m_pegManager.LoadLevel(m_pegManager.m_currentLevel);
        // hide the try again screen
        m_tryAgain.SetActive(false);
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
