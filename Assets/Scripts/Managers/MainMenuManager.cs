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
	Last Modified: 09/11/2026
*/
public class MainMenuManager : MonoBehaviour
{
    [System.Serializable]public struct PopUpMenu
    {
        public CanvasGroup m_canvasGroup;
        public CursorMoveable m_cursorMoveable;
    }


    [Header("Sub Menus")]
    public GameObject m_titleScreen;
    public GameObject m_mainMenu;
    public GameObject m_levelSelect;

    [Header("Pop Up Menus")]
    public PopUpMenu m_quitMenu;
    public PopUpMenu m_optionsMenu;
    public PopUpMenu m_changeSaveMenu;
    public PopUpMenu m_certificate;
    public PopUpMenu m_deleteSaveConfirmMenu;
    public PopUpMenu m_renameSaveMenu;
    Stack<PopUpMenu> m_popUpStack;

    [Header("UI Components")]
    public Slider m_musicVolumeSlider;
    public Slider m_feverVolumeSlider;
    public Slider m_soundEffectVolumeSlider;
    public Toggle m_fullscreenToggle;
    public Toggle m_colorblindToggle;
    public Button m_navigateLeft;
    public Button m_navigateRight;
    public InputField m_certificateInputField;
    public Text m_certificateDate;
    public Button m_createNewUserButton;
    public GameObject m_cancelCertificateButton;
    public Button m_certificateOKButton;
    public Text m_deleteConfirmWarning;
    public InputField m_renameInputField;
    public Button m_renameOKButton;

    [Header("Saves")]
    public SaveReader m_saveReader;
    public SaveWriter m_saveWriter;
    public Button[] m_saveFileButtons;
    public Color m_inactiveSaveFileButtonColor;
    public Color m_activeSaveFileButtonColor;
    int m_selectedSaveFile = 0;
    int m_storedSaveFiles = 0;

    [Header("Speech Bubble")]
    public GameObject m_changeSaveButton;
    public Text m_speechBubbleText;
    public string m_greetingTextStart = "Welcome Back ";
    public string m_greetingTextEnd = "!";
    public string m_quitHoverText = "See you again soon!";
    public string m_optionsHoverText = "Change Settings";
    public string m_adventureHoverTextStart = "Continue your adventure on Stage ";
    public string m_adventureHoverTextDivider = "-";
    public string m_adventureHoverTextEnd = "!";
    public string m_quickPlayHoverText = "Play individual levels that you have unlocked in Adventure!";

    [Header("Stage Text")]
    public Text m_stageNumberText;
    public string m_stageNumberBaseText = "STAGE ";
    public string[] m_writtenNumbers;

    [Header("Other")]
    public int m_gameplaySceneID = 1;
    public string m_deleteConfirmWarningStart = "This will permanently remove '";
    public string m_deleteConfirmWarningEnd = "' from the player roster!";
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

    public void RenameOK()
    {
        // store the current save ID
        int currentSaveID = GlobalSettings.m_currentSaveID;

        // if the save to be renamed is not the current
        if (currentSaveID != m_selectedSaveFile)
        {
            // load the save file selected to be renamed, without applying settings
            m_saveReader.ReadSaveFile(m_selectedSaveFile, false);
        }

        // change the save name of the save using the input field on the rename pop up menu
        GlobalSettings.m_saveName = m_renameInputField.text;

        // update the save file with the new name
        m_saveWriter.UpdateSaveFile();

        // if the stored save ID is not the current save ID
        if (currentSaveID != GlobalSettings.m_currentSaveID)
        {
            // load the stored save file, without reapplying its settings
            m_saveReader.ReadSaveFile(currentSaveID, false);
        }
        else
        // if the renamed save is the current save
        {
            // update the speech bubble text with the new save name
            SpeechBubbleDefault();
        }

        // update the save's corresponding button with the new name
        UpdateSaveFileButtonText(m_selectedSaveFile, m_renameInputField.text);

        // clear the text of the input field
        m_renameInputField.text = "";

        // hide the rename save menu
        ClosePopUpMenu();
    }

    public void RenameInputChanged()
    {
        // make the rename menu's OK button uninteractable if the input field is empty
        m_renameOKButton.interactable = (m_renameInputField.text != "");
    }

    public void SaveMenuRename()
    {
        // show the rename save pop up menu
        ShowPopUpMenu(m_renameSaveMenu);

        // initialise the rename menu's input field with the save's current name
        m_renameInputField.text = m_saveFileButtons[m_selectedSaveFile].GetComponentInChildren<Text>().text;
        // have the input field selected
        m_renameInputField.Select();
    }

    public void SaveMenuOK()
    {
        // read the selected save file without applying its stored settings
        m_saveReader.ReadSaveFile(m_selectedSaveFile, false);

        // update the speech bubble text with the new save name
        SpeechBubbleDefault();

        // hide the change save menu
        ClosePopUpMenu();
    }

    public void DeleteMenuConfirm()
    {
        // hide the delete confirm menu
        ClosePopUpMenu();

        // set the button of the selected save file to be inactive
        m_saveFileButtons[m_selectedSaveFile].gameObject.SetActive(false);
        // have the save file script delete the corresponding save
        m_saveReader.DeleteSaveFile(m_selectedSaveFile);

        // decrease the stored save counter
        --m_storedSaveFiles;

        // ensure the create new user button can be interacted with
        m_createNewUserButton.interactable = true;

        // if there are now 0 save files
        if (m_storedSaveFiles == 0)
        {
            // store nothing as the current save path for the event in which the user quits the game before making a save
            GlobalSettings.m_currentSavePath = "";
            // hide the change save menu
            ClosePopUpMenu();
            // prompt the user to create a new save file, without the ability to cancel
            CreateNewUser(false);
        }
        // if the selected save file was the current save file
        else if (m_selectedSaveFile == GlobalSettings.m_currentSaveID)
        {
            // loop for each save file button
            for (int i = 0; i < m_saveFileButtons.Length; ++i)
            {
                // if this save file button is active, its associated save file exists
                if (m_saveFileButtons[i].IsActive())
                {
                    // read this save file to be used as the new current save, without changing settings with it
                    m_saveReader.ReadSaveFile(i, false);
                    // update the speech bubble text with the new save name
                    SpeechBubbleDefault();
                    // exit the loop
                    break;
                }
            }
        }
    }

    public void SaveMenuDelete()
    {
        m_deleteConfirmWarning.text = m_deleteConfirmWarningStart + m_saveFileButtons[m_selectedSaveFile].GetComponentInChildren<Text>().text + m_deleteConfirmWarningEnd;

        // show the options pop up menu
        ShowPopUpMenu(m_deleteSaveConfirmMenu);
    }

    public void CertificateOK()
    {
        // save the text provided by the user as the save name
        GlobalSettings.m_saveName = m_certificateInputField.text;

        // loop for each save file button
        for (int i = 0; i < m_saveFileButtons.Length; ++i)
        {
            // if this save file button is inactive, its associated save file does not yet exist
            if (!m_saveFileButtons[i].gameObject.activeSelf)
            {
                // store this save as the current save's ID
                GlobalSettings.m_currentSaveID = i;
                // store this location as the save file's file path
                GlobalSettings.m_currentSavePath = m_saveReader.GetFilePath(i);
                // have the save writer create the save file
                m_saveWriter.UpdateSaveFile();
                // set the save file button corresponding to this save file to be active
                m_saveFileButtons[i].transform.gameObject.SetActive(true);
                // set the save file button's text to be the name of the file
                UpdateSaveFileButtonText(i, GlobalSettings.m_saveName);
                // exit the loop
                break;
            }
        }

        // increase the stored save counter
        ++m_storedSaveFiles;

        // if the number of save files is now at the maximum
        if (m_storedSaveFiles >= m_saveReader.m_maxSaves)
        {
            // prevent the create new user button from being interacted with
            m_createNewUserButton.interactable = false;
        }

        // clear the text of the input field
        m_certificateInputField.text = "";

        // update the speech bubble text with the new save name
        SpeechBubbleDefault();

        // hide the certificate of admission
        ClosePopUpMenu();
    }

    public void CertificateInputChanged()
    {
        // make the certificate's OK button uninteractable if the input field is empty
        m_certificateOKButton.interactable = (m_certificateInputField.text != "");
    }

    public void CreateNewUser(bool a_allowCancel = true)
    {
        // hide the change save menu
        ClosePopUpMenu();

        // show the certificate of admission pop up menu
        ShowPopUpMenu(m_certificate);

        // make the cancel button on the certificate active if cancelling is allowed
        m_cancelCertificateButton.SetActive(a_allowCancel);
    }

    public void ClosePopUpMenu()
    {
        // hide the stack's top pop up menu and remove it from the stack
        m_popUpStack.Pop().m_canvasGroup.gameObject.SetActive(false);
        // if the stack is not empty
        if (m_popUpStack.Count > 0)
        {
            // make the pop up menu that is now at the top of the stack interactable and moveable
            m_popUpStack.Peek().m_canvasGroup.interactable = true;
            m_popUpStack.Peek().m_cursorMoveable.enabled = true;
        }
    }

    public void ShowPopUpMenu(PopUpMenu a_popUpMenu)
    {
        // if the stack is not empty
        if (m_popUpStack.Count > 0)
        {
            // make the pop up menu currently at the top of the stack uninteractable and unmoveable
            m_popUpStack.Peek().m_canvasGroup.interactable = false;
            m_popUpStack.Peek().m_cursorMoveable.enabled = false;
        }
        // make the argument pop up menu interactable
        a_popUpMenu.m_canvasGroup.interactable = true;
        // make the argument pop up menu cursor moveable
        a_popUpMenu.m_cursorMoveable.enabled = true;
        // add the argument pop up menu to the stack
        m_popUpStack.Push(a_popUpMenu);
        // show the argument pop up menu
        a_popUpMenu.m_canvasGroup.gameObject.SetActive(true);
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
        // if there are no pop up menus currently active
        if (m_popUpStack.Count == 0)
        {
            // make the current save the selected save in the change save menu
            ChooseSaveButton(GlobalSettings.m_currentSaveID);

            // show the change save pop up menu
            ShowPopUpMenu(m_changeSaveMenu);
        }
    }

    public void QuitButton()
    {
        // show the quit pop up menu
        ShowPopUpMenu(m_quitMenu);
    }

    public void QuitGame()
    {
        // quit the game
        Application.Quit();
    }

    public void OptionsButton()
    {
        // if there are no pop up menus currently active
        if (m_popUpStack.Count == 0)
        {
            // show the options pop up menu
            ShowPopUpMenu(m_optionsMenu);
        }
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

    public void SetFullscreenToggle()
    {
        m_fullscreenToggle.isOn = (Screen.fullScreenMode == FullScreenMode.FullScreenWindow) ? true : false;
    }

    public void FullscreenToggleChanged()
    {
        // set the fullscreen mode to fullscreen or windowed, as per the toggle's value
        Screen.fullScreenMode = (m_fullscreenToggle.isOn) ? FullScreenMode.FullScreenWindow : FullScreenMode.Windowed;
    }

    public void SetColorblindToggle()
    {
        m_colorblindToggle.isOn = GlobalSettings.m_colorblindMode;
    }

    public void ColorblindToggleChanged()
    {
        GlobalSettings.m_colorblindMode = m_colorblindToggle.isOn;
    }

    public void LoadGameplayScene(bool a_adventure)
    {
        // store whether the gameplay scene should be loaded in adventure mode or quickplay mode
        GlobalSettings.m_adventureMode = a_adventure;

        // load the Gameplay scene
        SceneManager.LoadSceneAsync(m_gameplaySceneID);
    }

    public void RandomLevel()
    {
        // TEMP limit to levels completed in adventure

        // get a random stage
        GlobalSettings.m_currentStageID = Random.Range(0, GlobalSettings.m_stageCount - 1);
        // get a random level
        GlobalSettings.m_currentLevelID = Random.Range(0, GlobalSettings.m_levelsPerStage - 1);
        // load the gameplay scene in quickplay mode
        LoadGameplayScene(false);
    }

    public void NavigateStage(int a_modifier)
    {
        // if the new stage ID would be within valid bounds
        if (GlobalSettings.m_currentStageID + a_modifier >= 0 && GlobalSettings.m_currentStageID < GlobalSettings.m_stageCount)
        {
            // set the stage ID to the new value
            GlobalSettings.m_currentStageID += a_modifier;

            // TEMP update level images

            // update stage number text
            m_stageNumberText.text = m_stageNumberBaseText + m_writtenNumbers[GlobalSettings.m_currentStageID];

            // enable/disable the navigate buttons depending on if there is a stage in that direction of the current stage
            m_navigateLeft.interactable = (GlobalSettings.m_currentStageID > 0);
            m_navigateRight.interactable = (GlobalSettings.m_currentStageID < GlobalSettings.m_stageCount - 1);

        }
    }

    public void SelectLevel(int a_levelID)
    {
        // set the level ID to load in the gameplay scene
        GlobalSettings.m_currentLevelID = a_levelID;
        // load the gameplay scene in quickplay mode
        LoadGameplayScene(false);
    }

    public void SpeechBubbleDefault()
    {
        // update the speech bubble text
        m_speechBubbleText.text = m_greetingTextStart + "\n" + GlobalSettings.m_saveName + m_greetingTextEnd;
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
        m_speechBubbleText.text = m_adventureHoverTextStart + GlobalSettings.m_currentStageID + m_adventureHoverTextDivider + GlobalSettings.m_currentLevelID + m_adventureHoverTextEnd;
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

    private void Awake()
    {
        // initialise the pop up menu stack
        m_popUpStack = new Stack<PopUpMenu>();

        // show the current date on the certificate of admission
        m_certificateDate.text = System.DateTime.Today.ToString("d");
    }

    // Start is called before the first frame update
    void Start()
    {
        // initialise volume sliders
        m_musicVolumeSlider.value = GlobalSettings.m_musicVolume;
        m_feverVolumeSlider.value = GlobalSettings.m_feverVolume;
        m_soundEffectVolumeSlider.value = GlobalSettings.m_soundEffectVolume;

        // loop for each save file
        for (int i = 0; i < m_saveReader.m_maxSaves; ++i)
        {
            // get the file name for this save file
            string fileName = m_saveReader.GetSaveFileName(i);

            // if there is no file
            if (fileName == "")
            {
                // set the save file button corresponding to this save file to be inactive
                m_saveFileButtons[i].transform.gameObject.SetActive(false);
            }
            else
            {
                // set the save file button's text to be the name of the file
                UpdateSaveFileButtonText(i, fileName);
                // store that a save file has been found
                ++m_storedSaveFiles;
            }
        }

        // if no save file was found, prompt the user to create a new save file
        if (m_storedSaveFiles == 0)
        {
            // prompt the user to create a new save file, without the ability to cancel
            CreateNewUser(false);
        }
        // if a save file was found
        else
        {
            // if the number of save files is at the maximum
            if (m_storedSaveFiles >= m_saveReader.m_maxSaves)
            {
                // prevent the create new user button from being interacted with
                m_createNewUserButton.interactable = false;
            }

            // update the speech bubble text
            m_speechBubbleText.text = m_greetingTextStart + GlobalSettings.m_saveName + m_greetingTextEnd;
        }

    }

    // Update is called once per frame
    void Update()
    {
        // if the player presses the menu button
        if (Input.GetButtonDown("Toggle Menu"))
        {
            // trigger the function triggered by pressing the quit button
            QuitButton();
        }

        //TEMP
        if (Input.GetKeyDown(KeyCode.Alpha9))
        {
            print(GlobalSettings.m_currentSavePath);
        }
    }
}

