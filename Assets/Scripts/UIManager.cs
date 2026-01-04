using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/*
    File name: UIManager.cs
    Summary: Manages UI buttons and transitions
    Creation Date: 29/01/2024
    Last Modified: 05/01/2026
*/

public class Flicker
{
    enum UIElementType
    {
        Text,
        BarHeight,
        Render
    }

    UIElementType m_UIElementType = UIElementType.Text;
    UIManager m_UIManager;
    float m_timer = 0.0f;
    float m_flickerCounter = 0;

    RawImage m_UIRenderer = null;
    Text m_text = null;
    RectTransform m_rect = null;

    float m_oldHeight = 0.0f;
    float m_newHeight = 0.0f;

    public Flicker(Text a_text, UIManager a_UIManager)
    {
        // store the UI manager
        m_UIManager = a_UIManager;
        // store that the flickering UI element is text
        m_text = a_text;
        m_UIElementType = UIElementType.Text;
    }

    public Flicker(RectTransform a_rect, UIManager a_UIManager, float a_oldHeight, float a_newHeight)
    {
        // store the UI manager
        m_UIManager = a_UIManager;
        // store that the flickering UI element is a rect transfrom's height
        m_rect = a_rect;
        m_UIElementType = UIElementType.BarHeight;

        // store the heights that the bar should toggle between
        m_oldHeight = a_oldHeight;
        m_newHeight = a_newHeight;
    }

    public Flicker(RawImage a_UIRenderer, UIManager a_UIManager)
    {
        // store the UI manager
        m_UIManager = a_UIManager;
        // store that the flickering UI element is a renderer
        m_UIRenderer = a_UIRenderer;
        m_UIElementType = UIElementType.Render;
    }

    void ResolveFlicker()
    {
        switch (m_UIElementType)
        {
            case UIElementType.Text:
                // set the colour of the text to be the inactive colour if it has flickered an even number of times, or the active colour otherwise
                m_text.color = (m_flickerCounter % 2 == 0) ? m_UIManager.m_inactiveMultiplierText : m_UIManager.m_activeMultiplierText;
                break;
            case UIElementType.BarHeight:
                // set the height of the bar to be the old height if it has flickered an even number of times, or the new height otherwise
                m_rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, (m_flickerCounter % 2 == 0) ? m_oldHeight : m_newHeight);
                break;
            case UIElementType.Render:
                // toggle the active state of the renderer;
                m_UIRenderer.enabled = !m_UIRenderer.enabled;
                break;
        }
    }

    public bool Update()
    {
        // increase the timer
        m_timer += Time.unscaledDeltaTime;

        // if enough time has elapsed for the flicker to occur
        if (m_timer >= m_UIManager.m_flickerInterval)
        {
            // increase the counter for how many flickers have
            ++m_flickerCounter;
            // reset the timer to wait for the next flicker interval
            m_timer -= m_UIManager.m_flickerInterval;

            // have the effect of the flicker occur, depending on the type of UI element this corresponds to
            ResolveFlicker();

            // if the UI element has flickered enough times
            if (m_flickerCounter == m_UIManager.m_flickerCount)
            {
                // return that the flickering has elapsed and the object can be removed from the flickering list
                return true;
            }
        }
        // return that the flickering has not yet elapsed
        return false;
    }

}

public class UIManager : MonoBehaviour
{
    [System.Serializable] public struct CharacterAssets
    {
        public GameObject m_playerIconPrefab;
        public AudioClip m_victoryMusic;
    }

    public PlayerControls m_playerControls;
    public LauncherRotation m_launcherRotation;
    public LevelManager m_levelManager;
    public PegManager m_pegManager;
	public Dialogue m_dialogue;
    public Camera m_camera;
    public MusicManager m_musicManager;

    [Header("UI Screens")]
    public GameObject m_levelComplete;
    public GameObject m_tryAgain;
	public GameObject m_dialogueScreen;
    public GameObject m_pauseMenu;
    public GameObject m_characterSelect;
    public RawImage m_helpScreen;

    [Header("UI Elements")]
    public Toggle m_fullscreenToggle;
    public Toggle m_colorblindToggle;
    public Slider m_musicVolumeSlider;
    public Slider m_feverVolumeSlider;
    public Slider m_soundEffectVolumeSlider;
    public Text m_ballCountText;
    public float m_freeBallTextDuration = 2.0f;
    float m_freeBallTextTimer = 0.0f;
    int m_activeNonPauseSubMenus = 0;

    [Header("Character Select")]
    public GameObject m_launcher;
    public RectTransform m_selectedBorder;
    public CharacterAssets[] m_characters;
    public Text m_powerChargesText;
    GameObject m_playerIcon;

    [Header("Help Screen")]
    public Texture[] m_helpPages;
    public float m_helpScreenMoveSpeed = 7500.0f;
    int m_helpPageNumber = 0;
    bool m_moveHelpScreen;
    Vector3 m_helpOnScreenPosition = Vector3.zero;
    Vector3 m_helpTargetPosition = Vector3.zero;

    [Header("Free Ball Progress Bar")]
    public RectTransform m_freeBallProgressBar;
    public RawImage m_freeBallProgressBarBackground;
    public Color[] m_freeBallProgressBarColors;
    RawImage m_freeBallProgressBarImage;
    float m_freeBallProgressBarHeight = 0.0f;

    [Header("Fever Meter")]
    public GameObject m_multiplierThresholdPrefab;
    public GameObject m_barLinePrefab;
    public RectTransform m_feverMeter;
    public Color m_activeMultiplierText;
    [HideInInspector] public Color m_inactiveMultiplierText;
    float m_feverMeterHeight = 0.0f;
    float m_feverMeterBlockHeight = 0.0f;
    Text[] m_feverMeterMultiplierTexts;

    [Header("Score")]
    public Text m_levelScoreText;
    public Text m_topScoreText;
    public GameObject m_popUpTextPrefab;
    public Transform m_popUpTextContainer;
    SaveFile m_saveFile;
    int m_selectedCharacterID = 0;

    [Header("Flickering")]
    public int m_flickerCount = 5;
    public float m_flickerInterval = 0.3f;
    List<Flicker> m_flickeringUIElements;

    // TEMP - have in struct for character art assets
    [Header("TEMP")]
    public RawImage m_gameOverlay;

    public void LockInCharacter(bool a_useLevelDefault = false)
    {
        // enable the peg launcher
        TogglePegLauncher(true);

        // reset the ball count text and timer
        m_freeBallTextTimer = 0.0f;
        UpdateBallCountText();

        // set the character select screen to be inactive if it was active
        m_characterSelect.SetActive(false);
        // store that this sub menu is no longer active
        --m_activeNonPauseSubMenus;

        // if the level's default character should be used
        if (a_useLevelDefault)
        {
            // set the selected character ID to the level's default
            m_selectedCharacterID = m_levelManager.m_stages[GlobalSettings.m_currentStageID].m_defaultPowerID;
        }

        // if the selected character ID is different to the currently loaded character
        if (!ReferenceEquals(m_playerIcon, m_characters[m_selectedCharacterID].m_playerIconPrefab))  // TEMP //m_playerIcon.Equals(m_characterPrefabs[m_selectedCharacterID]))
        {
            // destroy the current player icon
            Destroy(m_playerIcon);

            // set the player icon to be the prefab for the corresponding character
            m_playerIcon = Instantiate(m_characters[m_selectedCharacterID].m_playerIconPrefab, m_launcher.transform);

            // give player controls access to the character's power
            m_playerControls.m_power = m_playerIcon.GetComponent<GreenPegPower>();

            // give the character's power script access to player controls and the power charges text
            m_playerControls.m_power.m_playerControls = m_playerControls;
            m_playerControls.m_power.m_powerChargesText = m_powerChargesText;

            // set the victory music to this character's victory music
            m_musicManager.m_victoryMusic = m_characters[m_selectedCharacterID].m_victoryMusic;

            // TEMP
            // set corresponding art assets for character, etc

            // initialise the power
            m_playerControls.m_power.Initialize();
        }
    }

    public void SelectCharacter(int a_characterID)
    {
        // if the character ID is greater than the amount of characters, the random option has been selected
        if (a_characterID >= m_characters.Length)
        {
            // get a random character
            m_selectedCharacterID = Random.Range(0, m_characters.Length);
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

    public void ShowCharacterSelectScreen()
    {
        // show the character select menu
        m_characterSelect.SetActive(true);
        // add this screen to the sub menu count
        ++m_activeNonPauseSubMenus;
        // disable the peg launcher
        TogglePegLauncher(false);
    }

    public void LevelOver(bool a_won)
    {
        // disable the peg launcher
        TogglePegLauncher(false);

        // if the player won the level
        if (a_won)
        {
            // if the stored top score for this level is lower than or equal to the scored achieved
           if (m_saveFile.m_topScores[GlobalSettings.m_currentStageID, GlobalSettings.m_currentLevelID] <= m_pegManager.m_score)
            {
                // update the top score
                m_saveFile.m_topScores[GlobalSettings.m_currentStageID, GlobalSettings.m_currentLevelID] = m_pegManager.m_score;

                // update the top score text
                UpdateTopScoreText();
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

        // increase the active submenu count
        ++m_activeNonPauseSubMenus;
    }

    public void CloseDialogueScreen()
    {
        // enable the peg launcher
        TogglePegLauncher(true);
        // hide the dialogue screen
        m_dialogueScreen.SetActive(false);
    }

	public void SwitchToDialogue(int a_dialogueIndex)
	{
        // disable the peg launcher
        TogglePegLauncher(false);
        // show the dialogue screen
        m_dialogueScreen.SetActive(true);
		// give the dialogue script the dialogue set to run through
		m_dialogue.m_dialogueIndex = a_dialogueIndex;
	}
    
    public void DisplayFreeBallText()
    {
        // update the ball count text with a message denoting that a free ball has been given
        m_ballCountText.text = "Free Ball!";
        // start a timer to determine how long the ballCountText should display the "Free Ball!" text
        m_freeBallTextTimer = m_freeBallTextDuration;
    }

    public void UpdateBallCountText()
    {
        // if the ball count text is not currently  displaying the 'Free Ball!' text
        if (m_freeBallTextTimer <= 0.0f)
        {
            // set the ball count text to display the current ball count as per the player controls component
            m_ballCountText.text = m_playerControls.m_ballCount.ToString();
        }
    }

    public void UpdateFeverMeterMultiplier(int a_multiplierID)
    {
        // have this multiplier text flicker
        m_flickeringUIElements.Add(new Flicker(m_feverMeterMultiplierTexts[a_multiplierID], this));
    }

    public void FlickerFeverMeter(int a_oldHitOrangePegCount, int a_newHitOrangePegCount)
    {
        // set the fever meter to flicker between its old and new height
        m_flickeringUIElements.Add(new Flicker(m_feverMeter, this, a_oldHitOrangePegCount * m_feverMeterBlockHeight, a_newHitOrangePegCount * m_feverMeterBlockHeight));
    }

    public void UpdateFeverMeter(int a_hitOrangePegs)
    {
        // modify the fever meter to be representative of the amount of orange pegs that have been hit, relative to the original orange peg total
        m_feverMeter.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, a_hitOrangePegs * m_feverMeterBlockHeight);
    }

    public void InitialiseFeverMeter(int a_orangePegCount, int[] a_scoreMultipliers, int[] a_multiplierIncreaseThresholds)
    {
        // create an index to iterate through the a_multiplierIncreaseThresholds array
        int increaseThresholdIndex = 0;
        
        // determine the height of the blocks that represent the orange pegs on the fever meter
        m_feverMeterBlockHeight = 1.0f/(float)a_orangePegCount * m_feverMeterHeight;

        // initialse the fever meter multiplier text array with the amount of score multipliers there are
        m_feverMeterMultiplierTexts = new Text[a_scoreMultipliers.Length];

        // loop for each orange peg
        for (int i = 1; i < a_orangePegCount; ++i)
        {
            GameObject barLine = null;

            // if the current amount of orange pegs is the next multiplier increase threshold
            if (i == a_multiplierIncreaseThresholds[increaseThresholdIndex])
            {
                // instantiate the bar line with the multiplier threshold prefab
                barLine = Instantiate(m_multiplierThresholdPrefab);
                // get the text component of the multiplier threshold and store it in the multiplier text array
                m_feverMeterMultiplierTexts[increaseThresholdIndex] = barLine.GetComponentInChildren<Text>();
                // set the text of the threshold to the score multiplier set by the threshold
                m_feverMeterMultiplierTexts[increaseThresholdIndex].text = "x" + a_scoreMultipliers[increaseThresholdIndex + 1];
                // have the increaseThresholdIndex point to the next increase threshold
                ++increaseThresholdIndex;
            }
            // if the current amount of orange pegs does not respond to the next multiplier increase threshold
            else
            {
                // instantiate the bar line with the normal bar line prefab
                barLine = Instantiate(m_barLinePrefab);
            }

            // set the bar line's parent to be the fever meter background
            barLine.transform.SetParent(m_feverMeter.parent, false);
            // position the bar line along the fever meter to split up the fever meter into blocks for each orange peg
            barLine.transform.localPosition = Vector3.up * i * m_feverMeterBlockHeight;
        }
    }

    public void UpdateFreeBallProgressBar(int a_currentShotScore, int a_freeBallsAwarded)
    {
        // if the max amount of free balls have been awarded
        if (a_freeBallsAwarded >= m_freeBallProgressBarColors.Length)
        {
            // set the colours of the background and foreground to the final colour
            m_freeBallProgressBarBackground.color = m_freeBallProgressBarColors[m_freeBallProgressBarColors.Length];
            m_freeBallProgressBarImage.color = m_freeBallProgressBarColors[m_freeBallProgressBarColors.Length];
        }
        else
        {
            // set the colours of the background and foreground based on the amount of free balls that have been awarded
            m_freeBallProgressBarBackground.color = m_freeBallProgressBarColors[a_freeBallsAwarded];
            m_freeBallProgressBarImage.color = m_freeBallProgressBarColors[a_freeBallsAwarded + 1];
        }

        // modify the free ball progress bar to be representative of the score progress of reaching the next free ball milestone
        m_freeBallProgressBar.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, Mathf.Clamp01((float)(a_currentShotScore - ((a_freeBallsAwarded == 0) ? 0 : m_pegManager.m_freeBallScores[a_freeBallsAwarded - 1])) / (float)(m_pegManager.m_freeBallScores[a_freeBallsAwarded] - ((a_freeBallsAwarded == 0) ? 0 : m_pegManager.m_freeBallScores[a_freeBallsAwarded - 1]))) * m_freeBallProgressBarHeight);
    }

    public void UpdateTopScoreText()
    {
        // set the text to be the top score stored for the current level
        m_topScoreText.text = m_saveFile.m_topScores[GlobalSettings.m_currentStageID, GlobalSettings.m_currentLevelID].ToString("#,#");
    }

    public void UpdateLevelScoreText(int a_score)
    {
        // set the level score text to the new value
        m_levelScoreText.text = a_score.ToString("#,#");
    }

    public void DestroyPopUpTexts()
    {
        // loop for each pop up text in the container
        for (int i = m_popUpTextContainer.transform.childCount - 1; i >= 0; --i)
        {
            // destroy the pop up text
            Destroy(m_popUpTextContainer.transform.GetChild(i).gameObject);
        }
    }

    public void DisplayPopUpText(string a_text, Vector3 a_position, bool a_usePegOffset)
    {
        // instantiate the pop up text prefab
        GameObject popUpText = Instantiate(m_popUpTextPrefab);
        // set the text's parent to be the peg score text container
        popUpText.transform.SetParent(m_popUpTextContainer, false);
        // set the text to display
        popUpText.GetComponent<Text>().text = a_text;
        // position the text
        popUpText.transform.position = m_camera.WorldToScreenPoint(a_position);

        // if the peg offset should be used
        if (a_usePegOffset)
        {
            // apply the offset as stored in the prefab's transform's position
            popUpText.transform.position += m_popUpTextPrefab.transform.position;
        }
    }

    public void ReloadGameUI()
    {
        // reset the ball count text display
        m_ballCountText.text = m_playerControls.m_ballCount.ToString();
        // reset the power charge display to 0
        m_powerChargesText.text = 0.ToString();
        // reset the level score display to 0
        UpdateLevelScoreText(0);
        // reset the fever meter to 0
        UpdateFeverMeter(0);
    }

    public void NextLevel()
    {
        // enable the peg launcher
        TogglePegLauncher(true);
        // load the next level
        m_levelManager.LoadNextLevel();
        // hide the level complete screen
        m_levelComplete.SetActive(false);
        // store that this sub menu is no longer active
        --m_activeNonPauseSubMenus;
    }

    public void RetryLevel()
    {
        // enable the peg launcher
        TogglePegLauncher(true);
        // reload the current level
        m_levelManager.LoadLevel(GlobalSettings.m_currentStageID, GlobalSettings.m_currentLevelID);
        // if this function was called by the try again submenu
        if (m_tryAgain.activeSelf)
        {
            // hide the try again screen
            m_tryAgain.SetActive(false);
            // store that the ty again sub menu is no longer active
            --m_activeNonPauseSubMenus;
        }
        // if this function was instead called by the pause menu
        else
        {
            // make the pause menu inactive
            m_pauseMenu.SetActive(false);
        }
    }

    public void UpdateMusicVolume()
    {
        // store the new music volume in the global variable
        GlobalSettings.m_musicVolume = m_musicVolumeSlider.value;
        // have the music manager update its audiosource's volume
        m_musicManager.SetVolume();
    }

    public void UpdateFeverVolume()
    {
        // store the new fever volume in the global variable
        GlobalSettings.m_feverVolume = m_feverVolumeSlider.value;
    }

    public void UpdateSoundEffectVolume()
    {
        // store the new sound effect volume in the global variable
        GlobalSettings.m_soundEffectVolume = m_soundEffectVolumeSlider.value;
    }

    public void HelpOK()
    {
        // set the target for the help screen's position to be the left of the screen
        m_helpTargetPosition = m_helpOnScreenPosition + (Vector3.left * Screen.width);
        // store that the help screen needs to move to its new position
        m_moveHelpScreen = true;
        // store that this sub menu is no longer active for the purposes of toggling the pause menu
        --m_activeNonPauseSubMenus;
    }

    public void HelpPrevious()
    {
        // decreatse the help page number
        --m_helpPageNumber;
        // if the page number is now less than 0
        if (m_helpPageNumber < 0)
        {
            // set the page number to the last
            m_helpPageNumber = m_helpPages.Length - 1;
        }
        // set the help screen texture to the new page
        m_helpScreen.texture = m_helpPages[m_helpPageNumber];
    }

    public void HelpNext()
    {
        // increase the help page number
        ++m_helpPageNumber;
        // if the page number has now surpassed the page count
        if (m_helpPageNumber > m_helpPages.Length - 1)
        {
            // set the page number to the first
            m_helpPageNumber = 0;
        }
        // set the help screen texture to the new page
        m_helpScreen.texture = m_helpPages[m_helpPageNumber];
    }

    public void ShowHelpScreen()
    {
        // set the help screen texture to the first
        m_helpPageNumber = 0;
        m_helpScreen.texture = m_helpPages[m_helpPageNumber];

        // set the help screen position to the right of the screen
        m_helpScreen.transform.position = m_helpOnScreenPosition + (Vector3.right * Screen.width);

        // show the help screen
        m_helpScreen.gameObject.SetActive(true);
        // add to the count of active sub menus
        ++m_activeNonPauseSubMenus;

        // store that the help screen needs to move to its new position
        m_moveHelpScreen = true;
        // set the target for the help screen's position to be the on screen position
        m_helpTargetPosition = m_helpOnScreenPosition;
    }

    public void FullscreenToggle()
    {
        // set the fullscreen mode to fullscreen or windowed, as per the toggle's value
        Screen.fullScreenMode = (m_fullscreenToggle.isOn) ? FullScreenMode.FullScreenWindow : FullScreenMode.Windowed;
    }

    public void ColorblindToggle()
    {
        // store the new colourblind mode in the global variable
        GlobalSettings.m_colorblindMode = m_colorblindToggle.isOn;
        // update the peg colourblind icons
        m_pegManager.UpdateColorblindIcons();
    }

    public void TogglePauseMenu()
    {
        // if there are no other sub menus active
        if (m_activeNonPauseSubMenus == 0)
        {
            // swap the active state of the peg launcher
            TogglePegLauncher(!m_playerControls.enabled);
            // swap the active state of the pause menu
            m_pauseMenu.SetActive(!m_pauseMenu.activeSelf);
        }
    }

    void TogglePegLauncher(bool a_enabled)
    {
        m_playerControls.enabled = a_enabled;
        m_launcherRotation.enabled = a_enabled;
    }

    void Awake()
    {
        // get the save file component
        m_saveFile = GetComponent<SaveFile>();

        // store the help screen's current position as the on screen position
        m_helpOnScreenPosition = m_helpScreen.transform.position;

        // get the current height of the free ball progress bar
        m_freeBallProgressBarHeight = m_freeBallProgressBar.sizeDelta.y;
        // get the raw image component of the progress bar
        m_freeBallProgressBarImage = m_freeBallProgressBar.GetComponent<RawImage>();
        // initialise the progress bar height
        m_freeBallProgressBar.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 0.0f);

        // get the current height of the fever meter
        m_feverMeterHeight = m_feverMeter.sizeDelta.y;
        // initialise the fever meter height
        m_feverMeter.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 0.0f);
        // get the colour from the multiplier threshold prefab as the inactive multiplier threshold text colour
        m_inactiveMultiplierText = m_multiplierThresholdPrefab.GetComponentInChildren<Text>().color;

        // initialise the flickering UI elements list
        m_flickeringUIElements = new List<Flicker>();
    }

    private void Start()
    {
        // initialise the color blind setting
        m_colorblindToggle.isOn = GlobalSettings.m_colorblindMode;

        // initialise volume
        UpdateMusicVolume();
        UpdateFeverVolume();
        UpdateSoundEffectVolume();
    }

    void Update()
    {
        if (Input.GetButtonDown("Toggle Menu"))
        {
            TogglePauseMenu();
        }

        // if the help screen should be moved
        if (m_moveHelpScreen)
        {
            // move the help screen left at a speed determined by the help screen move speed
            m_helpScreen.transform.position += Vector3.left * Time.unscaledDeltaTime * m_helpScreenMoveSpeed;
            // if the help screen has reached its target position
            if (m_helpScreen.transform.position.x <= m_helpTargetPosition.x)
            {
                // set the help screen to be exactly at the target position
                m_helpScreen.transform.position = m_helpTargetPosition;

                // store that the help screen no longer needs to move
                m_moveHelpScreen = false;

                // if the help screen is now off screen
                if (m_helpScreen.transform.position.x > m_helpOnScreenPosition.x + (Screen.width * 0.5f) || m_helpScreen.transform.position.x < m_helpOnScreenPosition.x - (Screen.width * 0.5f))
                {
                    // set the help screen to be inactive
                    m_helpScreen.gameObject.SetActive(false);
                }
            }
        }

        // if the Free Ball Text Timer is active
        if (m_freeBallTextTimer > 0.0f)
        {
            // reduce the timer
            m_freeBallTextTimer -= Time.unscaledDeltaTime;

            // if the timer has ran out
            if (m_freeBallTextTimer <= 0.0f)
            {
                // set it to -1 to ensure it is not treated as still running 
                m_freeBallTextTimer = -1.0f;
                // replace the ball Count Text with the ball count
                UpdateBallCountText();
            }
        }

        // loop for each flickering UI element
        for (int i = 0; i < m_flickeringUIElements.Count; ++i)
        {
            // update this flickering UI element. If has finished updating
            if (m_flickeringUIElements[i].Update())
            {
                // remove the element from the list
                m_flickeringUIElements.RemoveAt(i);
                // set the iterator back a step since this element was removed from the array
                --i;
            }
        }
    }
}