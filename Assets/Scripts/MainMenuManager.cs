using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/*
    File name: MainMenuManager.cs
    Summary: Manages the UI of the main menu screen
    Creation Date: 03/11/2024
    Last Modified: 22/12/2024
*/
public class MainMenuManager : MonoBehaviour
{
    [Header("Sub Menus")]
    public GameObject m_titleScreen;
    public GameObject m_mainMenu;
    public GameObject m_levelSelect;

    [Header("Pop Up Menus")]
    public GameObject m_quitMenu;
    public GameObject m_optionsMenu;
    public GameObject m_changeSaveMenu;

    [Header("Options")]
    public Slider m_musicVolumeSlider;
    public Slider m_feverVolumeSlider;
    public Slider m_soundEffectVolumeSlider;
    public Toggle m_fullscreenToggle;
    public Toggle m_colorblindToggle;

    [Header("Saves")]
    public SaveFile m_saveFile;
    public Button[] m_saveFileButtons;
    public Color m_inactiveSaveFileButtonColor;
    public Color m_activeSaveFileButtonColor;
    int m_selectedSaveFile = 0;

    [Header("Speech Bubble")]
    public GameObject m_changeSaveButton;
    public Text m_speechBubbleText;
    public string m_defaultText = "Welcome Back Sweets!";
    public string m_quitHoverText = "See you again soon!";
    public string m_optionsHoverText = "Change Settings";
    public string m_adventureHoverText = "Continue your adventure";
    public string m_quickPlayHoverText = "Play individual levels that you have unlocked in Adventure!";


    [Header("Other")]
    public int m_GameplaySceneID = 0;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void UpdateSaveFileButtonText(int a_saveFileID, string a_saveFileName)
    {
        // ensure the corresponding save file button is active
        m_saveFileButtons[a_saveFileID].transform.gameObject.SetActive(true);
        // update the text of the button by accessing the text component of the child of this button
        m_saveFileButtons[a_saveFileID].GetComponentInChildren<Text>().text = a_saveFileName;
    }

    public void ChooseSaveButton(int a_buttonID)
    {
        // store this button as the selected button
        m_selectedSaveFile = a_buttonID;

        // loop through each save file button
        for (int i = 0; i < m_saveFileButtons.Length; ++i)
        {
            // set the button to the active colour if it was the button selected and the inactive colour otherwise
            m_saveFileButtons[i].GetComponent<Image>().color = (i == a_buttonID) ? m_activeSaveFileButtonColor : m_inactiveSaveFileButtonColor;
        }
    }

    public void SaveMenuOK()
    {
        // read the selected save file selected
        m_saveFile.ReadSaveFile(m_selectedSaveFile);
    }

    public void SaveMenuDelete()
    {
        // set the button of the selected save file to be inactive
        m_saveFileButtons[m_selectedSaveFile].gameObject.SetActive(false);
        // have the save file script delete the corresponding save
        m_saveFile.DeleteSaveFile(m_selectedSaveFile);
    }

    public void SwitchSubMenu(GameObject a_activeSubMenu)
    {
        // turn off all Sub Menus
        m_titleScreen.SetActive(false);
        m_mainMenu.SetActive(false);
        m_levelSelect.SetActive(false);

        // turn on the argument sub menu
        a_activeSubMenu.SetActive(true);
    }

    public void ChangeSaveButton()
    {
        // show the change save menu
        m_changeSaveMenu.SetActive(true);
    }

    public void CancelChangeSave()
    {
        // hide the change save menu
        m_changeSaveMenu.SetActive(false);
    }

    public void QuitButton()
    {
        // show the quit menu
        m_quitMenu.SetActive(true);
    }

    public void QuitGame()
    {
        // quit the game
        Application.Quit();
    }

    public void QuitMenuBack()
    {
        // close the quit menu
        m_quitMenu.SetActive(false);
    }

    public void OptionsButton()
    {
        m_optionsMenu.SetActive(true);
    }

    public void UpdateMusicVolume()
    {
        GlobalSettings.m_musicVolume = m_musicVolumeSlider.value;
    }

    public void UpdateFeverVolume()
    {
        GlobalSettings.m_feverVolume = m_feverVolumeSlider.value;
    }

    public void UpdateSoundEffectVolume()
    {
        GlobalSettings.m_soundEffectVolume = m_soundEffectVolumeSlider.value;
    }

    public void FullscreenToggle()
    {
        // set the fullscreen mode to fullscreen or windowed, as per the toggle's value
        Screen.fullScreenMode = (m_fullscreenToggle.isOn) ? FullScreenMode.FullScreenWindow : FullScreenMode.Windowed;
    }

    public void ColorblindToggle()
    {
        GlobalSettings.m_colorblindMode = m_colorblindToggle.isOn;
    }

    public void OptionsMenuBack()
    {
        // close the options menu
        m_optionsMenu.SetActive(false);
    }

    public void LoadGameplayScene(bool a_adventure)
    {
        // store whether the gameplay scene should be loaded in adventure mode or quickplay mode
        GlobalSettings.m_adventureMode = a_adventure;

        // load the Gameplay scene
        SceneManager.LoadSceneAsync(m_GameplaySceneID);
    }

    public void SelectLevel(int a_levelID)
    {
        // TEMP determine level set from page
        
        // set the level ID to load in the gamplay scene
        GlobalSettings.m_currentLevelID = a_levelID;
        // load the gameplay scene in quickplay mode
        LoadGameplayScene(false);
    }

    public void SpeechBubbleDefault()
    {
        // update the speech bubble text
        m_speechBubbleText.text = m_defaultText;
        // make the change save button active
        m_changeSaveButton.SetActive(true);
    }

    public void SpeechBubbleQuit()
    {
        // update the speech bubble text
        m_speechBubbleText.text = m_quitHoverText;
        // make the change save button inactive
        m_changeSaveButton.SetActive(false);
    }

    public void SpeechBubbleOptions()
    {
        // update the speech bubble text
        m_speechBubbleText.text = m_optionsHoverText;
        // make the change save button inactive
        m_changeSaveButton.SetActive(false);
    }

    public void SpeechBubbleAdventure()
    {
        // update the speech bubble text
        m_speechBubbleText.text = m_adventureHoverText;
        // make the change save button inactive
        m_changeSaveButton.SetActive(false);
    }

    public void SpeechBubbleQuickPlay()
    {
        // update the speech bubble text
        m_speechBubbleText.text = m_quickPlayHoverText;
        // make the change save button inactive
        m_changeSaveButton.SetActive(false);
    }
}
