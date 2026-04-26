using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.WSA;

/*
    File name: GameManager
    Summary: Manages the pacing of the game and oversees large game systems
    Creation Date: 16/03/2026
    Last Modified: 27/04/2026
*/
public class GameManager : MonoBehaviour
{
    [System.Serializable]
    public struct CharacterAssets
    {
        public GameObject m_playerIconPrefab;
        public AudioClip m_victoryMusic;
    }

    public enum GameState
    {
        Menu,
        Reloading,
        PreShot,
        Shooting,
        MidShot,
        PostShot
    }

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

    [Header("Characters")]
    public CharacterAssets[] m_characters;
    [HideInInspector] public MagicPower m_magicPower = null;
    int m_characterID = -1;

    [Header("Player Projectiles")]
    public Transform m_playerProjectilesContainer;
    public byte m_startingBallCount = 10;
    [HideInInspector] public int m_ballCount = 0; // TEMP
    bool m_allow0PegFreeBall = false;

    [Header("Audio")]
    public AudioClip[] m_freeBallSounds;

    bool m_paused = false;
    [HideInInspector] public GameState m_gameState = GameState.Reloading;

    // PlayerControls ballInPlay flag should be made redundant by GameState enumerator
    // Investigate potential issue with resolving power before setting up
    // Do consistency pass on terminology; shot vs turn vs phase
    // should ball trajectory be drawn on canvas like ball-o-tron?
    // look into - using UnityEngine.WSA;
    // consider having the resolveturn ui manager function calls instead be the ui manager's own resolve shot function
    // probably call for player controls to reset its time scale rather than calling a whole reload function

    // playercontrols reload is called after pegmanager loads a level
    // LevelManager load level prompts functions in UIManager, prompts UI manager to reload and prompts PegManager to load the level. also shows dialogue based on level info and has music manager shuffle play
    // load level function in level manager should probably be prompting game manager to resetlevel, rather than peg manager
    // The toggling of the peg launcher should perhaps be managed here instead of UI manager
    // UI manager next level toggles peg launcher and has level manager load next level
    // UI manager retry level toggles peg launcher and reloads current level
    // toggle pause menu is in UI manager, maybe ought to be in player controls
    // pegmanager prompts music manager to play victory music when last peg is hit

    // new Power UI container can be used to reload, clear all children from it - look into if could be used in magic power reload functions
    // perhaps better system than having UI manager instantiate power UI objects for magic power scripts, give power scripts the power UI container directly?
    // look into kevin power scope overlay - how does it get set?
    // toggling peg launcher in UIManager is messy, if multiple pop up screens were open and one was closed, wouldn't the peg launcher be toggled back on despite screens still being present?

    // reset level should reopen the character select screen if in quick play
    // time scale should be stored here but still modified in player controls?

    // if power doesn't need to be set up, skip its game state tracker straight to Shooting
    // Ethen Power may not need to disable playercontrols etc, with new Power State system

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

        // switch the game state to pre shot
        m_gameState = GameState.PreShot;
    }

    public void RemoveProjectile(GameObject a_projectile)
    {
        // set the projectile to have no parent
        a_projectile.transform.parent = null;
        // destroy the projectile
        Destroy(a_projectile);

        // if the projectile count is now equal to 0
        if (m_playerProjectilesContainer.childCount == 0)
        {
            // if the ball count is over 0
            if (m_ballCount > 0)
            {
                // switch the game state to post shot
                m_gameState = GameState.PostShot;
                // resolve the turn
                ResolveShot();
            }
            // if the player has run out of balls
            else
            {
                // switch the game state to menu
                m_gameState = GameState.Menu;
                // have the player lose the level
                LevelLost();
            }
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

    void LevelLost()
    {
        // triggered when 0 projectiles remaining in mid shot phase and player has no balls remaining (Player Controls)

        // UIManager
        // disable peg launcher
        // show try again screen


    }

    public void LevelWon()
    {
        // triggered when ball enters victory buckets

        // UIManager
        // disable peg launcher
        // update save file if new high score
        // update top score text if and new high score
        // show level complete screen
    }

    public void ResetLevel()
    {
        // PlayerControls:
        /// reset ball count
        /// reload power
        /// destroy ball
        /// reset time scale
        // run set up turn function - determine that everything has reloaded

        // UI Manager:
        /// reset ball count text display
        /// reset power charge text display
        /// reset level score display
        /// reset fever meter
        /// reset ball-o-tron

        // reset the ball count
        m_ballCount = m_startingBallCount;

        // if the power has been set
        if (m_magicPower != null)
        { 
            // prompt it to reload
            m_magicPower.Reload();
        }

        // loop for each player projectile
        for (int i = m_playerProjectilesContainer.childCount - 1; i >= 0; --i)
        {
            // destroy the current player projectile
            Destroy(m_playerProjectilesContainer.GetChild(i).gameObject);
        }

        // prompt the player controls to reload
        m_playerControls.Reload();

        // prompt the UI manager to reload the game UI
        m_UIManager.ReloadGameUI();
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

    public void OnShoot()
    {
        /// triggered by shooting the ball or possibly from magic power on shoot function
        /// switch GameState to MidShot
        /// have UIManager update ball count text

        // reduce the ball count by one as a ball has been expended
        --m_ballCount;
        // have the UI Manager update the ball count text
        m_UIManager.UpdateBallCountText();

        // disable the ball trajectory
        m_ballTrajectory.enabled = false;

        // switch GameState to MidShot
        m_gameState = GameState.MidShot;
    }
    
    void ResolveShot()
    {
        // triggered from 0 projectiles remaining in mid shot phase (PlayerControls)

        // PlayerControls:
        /// prompt power to resolve if it was flagged to
        /// prompt CameraZoom to return camera to default state
        /// prompt PegManager to resolve turn, with allow0pegfreeball flag
        // run set up turn function - determine that everything has resolved

        // PegManager:
        // prompt all hit pegs to be cleared
        ///run 50/50 on free ball if allowed and no hit pegs
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

        // tell the peg manager to resolve the turn. If there were no hit pegs this shot and the chance is allowed, give the player a 50% chance to get a free ball
        if (!m_pegManager.ResolveTurn() && m_allow0PegFreeBall && Random.Range(0, 2) == 1)
        {
            // give the player a free ball without playing the free ball sound
            FreeBall(false);
        }
    }

    private void Awake()
    {
        // initialise the ball count
        m_ballCount = m_startingBallCount;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void FixedUpdate()
    {
        if (m_gameState == GameState.Shooting)
        {
            m_ballTrajectory.CreateTrajectoryLine(m_playerControls.m_ballLaunchSpeed);
        }
    }

    // Update is called once per frame
    void Update()
    {
        switch (m_gameState)
        {
            case GameState.Menu:
                break;
            case GameState.Reloading:
                if (m_magicPower.IsReady(GameState.PreShot))
                {

                }
                break;
            case GameState.PreShot:
                if (m_magicPower.IsReady(GameState.Shooting))
                {

                }
                break;
            case GameState.MidShot:
                break;
            case GameState.PostShot:
                if (m_magicPower.IsReady(GameState.PreShot))
                {

                }
                break;
        }

        // TEMP
        print(m_gameState);
    }
}

/*
 * Game State Checklist (to move to next phase): 
 * 
 * Reloading:
 *  Power Reloaded -> pre shot
 *  Ball-O-Tron Reset
 * 
 * PreShot:
 *  Power Set Up -> shooting        [MatejaPower - Mateja needs to move to ground before set up is complete] [EthenPower - Drawing may need to be done first?]
 *  UI ball remaining pop up cleared
 *  
 * Shooting:
 *  Ball Shot
 *  
 * Mid Shot:
 *  All Player Projectiles Destroyed
 *  
 * Post Shot:
 *  Power Resolved -> pre shot
 *  Camera Returned
 *  Hit Pegs Cleared
 */