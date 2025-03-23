using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/*
    File name: UIManager.cs
    Summary: Manages UI buttons and transitions
    Creation Date: 29/01/2024
    Last Modified: 24/03/2025
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

    [Header("Character Select")]
    public GameObject m_launcher;
    public RectTransform m_selectedBorder;
    public GameObject[] m_characterPrefabs;
    public Text m_powerChargesText;
    GameObject m_playerIcon;

    bool m_newHighScore = false;
    SaveFile m_saveFile;
    int m_selectedCharacterID = 0;

    public void LockInCharacter()
    {
        // enable the player controls
        m_playerControls.enabled = true;
        
        // set the player icon to be the prefab for the corresponding character
        m_playerIcon = Instantiate(m_characterPrefabs[m_selectedCharacterID], m_launcher.transform) as GameObject;

        // give player controls access to the character's power
        m_playerControls.m_power = m_playerIcon.GetComponent<GreenPegPower>();

        // give the character's power script access to player controls and the power charges text
        m_playerControls.m_power.m_playerControls = m_playerControls;
        m_playerControls.m_power.m_powerChargesText = m_powerChargesText;

        // initialise the power
        m_playerControls.m_power.Initialize();

        // TEMP
        // set corresponding art assets for character, victory music, etc

        // set the character select screen to be inactive
        m_characterSelect.SetActive(false);
    }

    public void SelectCharacter(int a_characterID)
    {
        // if the character ID is greater than the amount of characters, the random option has been selected
        if (a_characterID >= m_characterPrefabs.Length)
        {
            // get a random character
            m_selectedCharacterID = Random.Range(0, m_characterPrefabs.Length);
        }
        else
        {
            // set the selected character to the character clicked on
            m_selectedCharacterID = a_characterID;
        }

        // TEMP
        m_selectedBorder.position = EventSystem.current.currentSelectedGameObject.transform.position;

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
           if (m_saveFile.m_highScores[GlobalSettings.m_currentStageID, GlobalSettings.m_currentLevelID] <= m_pegManager.m_score)
            {
                // update the highscore
                m_saveFile.m_highScores[GlobalSettings.m_currentStageID, GlobalSettings.m_currentLevelID] = m_pegManager.m_score;
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
        m_pegManager.LoadLevel(GlobalSettings.m_currentStageID, GlobalSettings.m_currentLevelID);
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

        // initialise volume
        UpdateMusicVolume();
        UpdateFeverVolume();
        UpdateSoundEffectVolume();

        // initialse the color blind setting
        m_colorblindToggle.isOn = GlobalSettings.m_colorblindMode;

        // if the game scene has been launched in quickplay mode
        if (!GlobalSettings.m_adventureMode)
        {
            // show the character select menu
            m_characterSelect.SetActive(true);
            // disable the player controls
            m_playerControls.enabled = false;
        }
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
