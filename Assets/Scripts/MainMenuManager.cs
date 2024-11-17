using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/*
    File name: MainMenuManager.cs
    Summary: Manages the UI of the main menu screen
    Creation Date: 03/11/2024
    Last Modified: 18/11/2024
*/
public class MainMenuManager : MonoBehaviour
{
    [Header("Sub Menus")]
    public GameObject m_titleScreen;
    public GameObject m_mainMenu;
    public GameObject m_quitMenu;
    public GameObject m_optionsMenu;
    public GameObject m_changeSaveMenu;

    [Header("Options")]
    public Slider m_musicVolumeSlider;
    public Slider m_feverVolumeSlider;
    public Slider m_soundEffectVolumeSlider;
    public Toggle m_fullscreenToggle;
    public Toggle m_colorblindToggle;
    float m_musicVolume = 0.0f;
    float m_feverVolume = 0.0f;
    float m_soundEffectVolume = 0.0f;
    bool m_colorblind = false;

    [Header("Speech Bubble")]
    public GameObject m_changeSaveButton;
    public Text m_speechBubbleText;
    public string m_defaultText = "Welcome Back Sweets!";
    public string m_quitHoverText = "See you again soon!";
    public string m_optionsHoverText = "Change Settings";
    public string m_adventureHoverText = "Continue your adventure";


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

    public void ClickToPlay()
    {
        // turn off the title screen objects
        m_titleScreen.SetActive(false);
        // turn on the title screen objects
        m_mainMenu.SetActive(true);
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
        m_musicVolume = m_musicVolumeSlider.value;
    }

    public void UpdateFeverVolume()
    {
        m_feverVolume = m_feverVolumeSlider.value;
    }

    public void UpdateSoundEffectVolume()
    {
        m_soundEffectVolume = m_soundEffectVolumeSlider.value;
    }

    public void FullscreenToggle()
    {
        // set the fullscreen mode to fullscreen or windowed, as per the toggle's value
        Screen.fullScreenMode = (m_fullscreenToggle.isOn) ? FullScreenMode.FullScreenWindow : FullScreenMode.Windowed;
    }

    public void ColorblindToggle()
    {
        m_colorblind = m_colorblindToggle.isOn;
    }

    public void OptionsMenuBack()
    {
        // close the options menu
        m_optionsMenu.SetActive(false);
    }

    public void Adventure()
    {
        // load the Gameplay scene
        SceneManager.LoadSceneAsync(m_GameplaySceneID);
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
}
