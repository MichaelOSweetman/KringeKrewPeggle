using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.WSA;

/*
    File name: GameManager
    Summary: Manages the pacing of the game and oversees large game systems
    Creation Date: 16/03/2026
    Last Modified: 06/04/2026
*/
public class GameManager : MonoBehaviour
{
    [Header("Other Scripts")]
    public Dialogue m_dialogue;
    public LevelManager m_levelManager;
    public MusicManager m_musicManager;
    public PegManager m_pegManager;
    public PlayerControls m_playerControls;
    public SaveFile m_saveFile;
    public UIManager m_UIManager;
    public BallTrajectory m_ballTrajectory;
    public CameraZoom m_cameraZoom;

    [Header("Audio")]
    public AudioClip[] m_freeBallSounds;

    [Header("TEMP UNSORTED")]
    bool m_allow0PegFreeBall = false;
    MagicPower m_magicPower = null;
    int m_ballCount = 0;
    int m_characterID = 0;

    [System.Serializable] public struct CharacterAssets
    {
        public GameObject m_playerIconPrefab;
        public AudioClip m_victoryMusic;
    }

    public CharacterAssets[] m_characters;

    enum GameState
    { 
         Menu,
         Paused,
         Reloading,
         PreShot,
         Shooting,
         MidShot,
         PostShot
    }

    GameState m_gameState = GameState.Menu;

    // PlayerControls ballInPlay flag should be made redundant by GameState enumerator
    // Investigate potential issue with resolving power before setting up
    // Do consistency pass on terminology; shot vs turn vs phase
    // should ball trajectory be drawn on canvas like ball-o-tron?
    // properly move ball count from player controls to game manager

    // playercontrols reload is called after pegmanager loads a level
    // LevelManager load level prompts functions in UIManager, prompts UI manager to reload and prompts PegManager to load the level. also shows dialogue based on level info and has music manager shuffle play
    // The toggling of the peg launcher should perhaps be managed here instead of UI manager
    // UI manager next level toggles peg launcher and has level manager load next level
    // UI manager retry level toggles peg launcher and reloads current level
    // toggle pause menu is in UI manager, maybe ought to be in player controls
    // pegmanager prompts music manager to play victory music when last peg is hit
    // player controls player projectile container should probably be managed here
    
    // new Power UI container can be used to reload, clear all children from it
    // perhaps better system than having UI manager instantiate power UI objects for magic power scripts, give power scripts the power UI container directly?
    // look into kevin power scope overlay - how does it get set?
    // toggling peg launcher in UIManager is messy, if multiple pop up screens were open and one was closed, wouldn't the peg launcher be toggled back on despite screens still being present?

    public void InitializeCharacter(int a_characterID = -1)
    {
        // determine the new character ID
        int newCharacterID = 0;

        // if the argument ID is -1
        if (a_characterID == -1)
        {
            // get a random character
            newCharacterID = Random.Range(0, m_characters.Length);
        }
        // otherwise, if the argument ID any other invalid value, use the default stage ID
        else if (a_characterID < -1 || a_characterID >= m_characters.Length)
        {
            // get the default ID for the current stage
            newCharacterID = m_levelManager.m_stages[GlobalSettings.m_currentStageID].m_defaultPowerID;
        }
        // otherwise, if the argument ID is valid
        else
        {
            // use the argument ID
            newCharacterID = a_characterID;
        }

        // if the new ID is not the same as the current ID
        if (m_characterID != newCharacterID)
        { 
            // set the selected character ID as the new ID
            m_characterID = newCharacterID;
            // have the UI manager load the character assets and get the magic power from the loaded prefab
            m_magicPower = m_UIManager.LoadCharacter(m_characters[m_characterID].m_playerIconPrefab).GetComponent<MagicPower>();

            // give the magic power access to player controls, the UI manager and this
            m_magicPower.m_playerControls = m_playerControls;
            m_magicPower.m_UIManager = m_UIManager;
            m_magicPower.m_gameManager = this;

            // initialise the power
            m_magicPower.Initialize();

            // set the victory music to this character's victory music
            m_musicManager.m_victoryMusic = m_characters[m_characterID].m_victoryMusic;
        }
        // if the new ID is the same as the current ID
        else
        {
            // reload the power
            m_magicPower.Reload();
        }
    }

    public void FreeBall(bool a_playSound = true, bool a_showFreeBallText = false, bool a_allow0PegFreeBall = true)
    {
        /// PlayerControls:
        /// increase ball count
        /// play sound
        /// show free ball text, update ball count text
        /// invalidate allow0pegFreeBall flag if specified
        /// add ball to ball-o-tron

        // TEMP - store variable here or perhaps call a 'get' function in PlayerControls
        ++m_ballCount;

        // if the free ball sound effect should be played
        if (a_playSound)
        {
            // play the free ball sound that corresponds to the amount of free balls earned this round, using the sound effect volume
            AudioSource.PlayClipAtPoint(m_freeBallSounds[m_pegManager.m_freeBallsAwarded], Vector3.zero, GlobalSettings.m_soundEffectVolume);
        }

        // prompt the UI Manager to display free ball info to the player
        m_UIManager.FreeBall(a_showFreeBallText);
        
        // if the chance to receive a free ball after hitting 0 pegs should be removed
        if (!a_allow0PegFreeBall)
        {
            // store that the chance has been removed for this round
            m_allow0PegFreeBall = false;
        }
    }

    void LevelOver(bool a_won)
    {
        // triggered when 0 projectiles remaining in mid shot phase and player has no balls remaining (Player Controls)
        
        // UIManager
        // disable peg launcher
        // update save file if won and new high score
        // update top score text if won and new high score
        // show level complete or try again screen based on outcome
    }

    void ResetLevel()
    {
        // PlayerControls:
        // reset ball count
        // reload power
        // destroy ball
        // reset time scale
        // run set up turn function

        // UI Manager:
        // reset ball count text display
        // reset power charge text display
        // reset level score display
        // reset fever meter
        // reset ball-o-tron
    }

    void SetUpShot()
    {
        /// show ball trajectory line (PlayerControls)
        /// reset allow0PegFreeBall flag (PlayerControls)
        /// prompt power to set up if it was flagged to (PlayerControls)
        /// prompt Ball-O-Tron to launch a ball (Player Controls)
        /// prompt UIManager to display low ball count warning text if applicable (Player Controls)

        // show the ball trajectory line
        m_ballTrajectory.ShowLine(true);
        // reset the 0 peg free ball validity flag
        m_allow0PegFreeBall = true;

        // if the power should be set up
        if (m_magicPower.m_setUpNextTurn)
        {
            // set up the power
            m_magicPower.SetUp();
            // store that the power has been set up
            m_magicPower.m_setUpNextTurn = false;
        }

        // prompt the UI manager to display the set up shot phase info to the player
        m_UIManager.SetUpShot(m_ballCount);
    }

    void OnShoot()
    {
        // triggered by shooting the ball

        // switch GameState to MidShot
        // have UIManager update ball count text
    }


    void ResolveShot()
    {
        // triggered from 0 projectiles remaining in mid shot phase (PlayerControls)

        // PlayerControls:
        /// prompt power to resolve if it was flagged to
        /// prompt CameraZoom to return camera to default state
        // prompt PegManager to resolve turn, with allow0pegfreeball flag
        // prompt Power to resolveturn
        // run set up turn function

        // PegManager:
        // prompt all hit pegs to be cleared, run 50/50 on free ball if allowed and no hit pegs
        // add shot score to total score
        // prompt ui manager to display round score
        // prompt ui manager to flicker fever meter
        // store hit peg count
        // reset turn score
        // replace purple peg

        
        // if the power should be resolved
        if (m_magicPower.m_resolveNextTurn)
        {
            // resolve the power
            m_magicPower.ResolvePower();
            // store that the power has been resolved
            m_magicPower.m_resolveNextTurn = false;
        }

        // tell the camera to return to default in case it had moved while the ball was in play
        m_cameraZoom.ReturnToDefault();

    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        switch (m_gameState)
        {
            case GameState.Menu:
                break;
            case GameState.Paused:
                break;
            case GameState.Reloading:
                break;
            case GameState.PreShot:
                break;
            case GameState.MidShot:
                break;
            case GameState.PostShot:
                // if x and y are finished, switch to pre shot
                break;
        }
    }
}
