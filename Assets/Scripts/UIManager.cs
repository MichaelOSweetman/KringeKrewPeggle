using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/*
    File name: UIManager.cs
    Summary: Manages UI buttons and transitions
    Creation Date: 29/01/2024
    Last Modified: 22/12/2024
*/
public class UIManager : MonoBehaviour
{
    public PlayerControls m_playerControls;
    public PegManager m_pegManager;
	public Dialogue m_dialogue;

    [Header("UI Screens")]
    public GameObject m_levelComplete;
    public GameObject m_tryAgain;
	public GameObject m_dialogueScreen;
    public GameObject m_pauseMenu;
    public GameObject m_characterSelect;

    [Header("UI Elements")]
    public Toggle m_fullscreenToggle;
    public Toggle m_colorblindToggle;
    public Slider m_musicVolumeSlider;
    public Slider m_feverVolumeSlider;
    public Slider m_soundEffectVolumeSlider;

    [Header("Audio")]
    public AudioSource m_musicAudioSource;
    public AudioSource m_feverAudioSource;
    public AudioSource m_soundEffectAudioSource;

    bool m_newHighScore = false;
    SaveFile m_saveFile;
    int m_playableCharacterCount = 9;
    int m_selectedCharacterID = 0;

    public void LockInCharacter()
    {
        // have player controls set the green peg power
        m_playerControls.SetGreenPegPower(m_selectedCharacterID);

        // TEMP
        // set corresponding art assets for character, victory music, etc

        // set the character select screen to be inactive
        m_characterSelect.SetActive(false);
    }

    public void SelectCharacter(int a_characterID)
    {
        // if the character ID is greater than the amount of characters, the random option has been selected
        if (a_characterID >= m_playableCharacterCount)
        {
            // get a random character
            m_selectedCharacterID = Random.Range(0, m_playableCharacterCount);
        }
        else
        {
            // set the selected character to the character clicked on
            m_selectedCharacterID = a_characterID;
        }
        // TEMP
        // Put gold border around small art
        // put set big image to corresponding character art
    }

    public void LevelOver(bool a_won)
    {
        // turn off the player controls
        m_playerControls.enabled = false;

        // if the player won the level
        if (a_won)
        {
            // if the stored highscore for this level is lower than or equal to the scored achieved
           if (m_saveFile.m_highScores[m_pegManager.m_currentLevel.transform.parent.GetSiblingIndex(), m_pegManager.m_currentLevel.transform.GetSiblingIndex()] <= m_pegManager.m_score)
            {
                // update the highscore
                m_saveFile.m_highScores[m_pegManager.m_currentLevel.transform.parent.GetSiblingIndex(), m_pegManager.m_currentLevel.transform.GetSiblingIndex()] = m_pegManager.m_score;
                // store that the player has achieved a new high score
                m_newHighScore = true;
            }
            

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

	public void SwitchToDialogue(int a_dialogueIndex)
	{
		// disable player controls
		m_playerControls.enabled = false;
		// show the dialogue screen
		m_dialogueScreen.SetActive(true);
		// give the dialogue script the dialogue set to run through
		m_dialogue.m_dialogueIndex = a_dialogueIndex;
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

    public void UpdateMusicVolume()
    {
        //store the new music volume in the global variable and apply it to the music audio source
        m_musicAudioSource.volume = GlobalSettings.m_musicVolume = m_musicVolumeSlider.value;
    }

    public void UpdateFeverVolume()
    {
        //store the new fever volume in the global variable and apply it to the fever audio source
        m_feverAudioSource.volume = GlobalSettings.m_feverVolume = m_feverVolumeSlider.value;
    }

    public void UpdateSoundEffectVolume()
    {
        //store the new sound effect volume in the global variable and apply it to the sound effect audio source
        m_soundEffectAudioSource.volume = GlobalSettings.m_soundEffectVolume = m_soundEffectVolumeSlider.value;
    }

    public void FullscreenToggle()
    {
        // set the fullscreen mode to fullscreen or windowed, as per the toggle's value
        Screen.fullScreenMode = (m_fullscreenToggle.isOn) ? FullScreenMode.FullScreenWindow : FullScreenMode.Windowed;
    }

    public void ColorblindToggle()
    {
        // store the new colorblind mode in the global variable
        GlobalSettings.m_colorblindMode = m_colorblindToggle.isOn;
    }

    public void TogglePauseMenu()
    {
        // swap the active state of player controls
        m_playerControls.enabled = !m_playerControls.enabled;
        // swap the active state of the pause menu
        m_pauseMenu.SetActive(!m_pauseMenu.activeSelf);
    }

    // Start is called before the first frame update
    void Start()
    {
        m_saveFile = GetComponent<SaveFile>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Toggle Menu"))
        {
            TogglePauseMenu();
        }
    }
}
