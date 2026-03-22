using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
    File name: GameManager
    Summary: Manages the pacing of the game and oversees large game systems
    Creation Date: 16/03/2026
    Last Modified: 23/03/2026
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

    [Header("Audio")]
    public AudioClip[] m_freeBallSounds;

    [Header("TEMP UNSORTED")]
    bool m_allow0PegFreeBall = false;

    enum GameState
    { 
         Menu,
         Paused,
         PreShot,
         Shooting,
         MidShot,
         PostShot,
         Reloading
    }

    GameState m_gameState = GameState.Menu;

    // PlayerControls ballInPlay flag should be made redundant by GameState enumerator
    // Investigate potential issue with resolving power before setting up
    // Consider renaming GreenPegPower to magic power, as per in game help page
    // Do consistency pass on terminology; shot vs turn vs phase
    // should ball trajectory be drawn on canvas like ball-o-tron?

    // playercontrols reload is called after pegmanager loads a level
    // LevelManager load level prompts functions in UIManager, prompts UI manager to reload and prompts PegManager to load the level. also shows dialogue based on level info and has music manager shuffle play
    // The toggling of the peg launcher should perhaps be managed here instead of UI manager
    // UI manager next level toggles peg launcher and has level manager load next level
    // UI manager retry level toggles peg launcher and reloads current level
    // toggle pause menu is in UI manager, maybe ought to be in player controls
    // pegmanager prompts music manager to play victory music when last peg is hit
    // player controls player projectile container should probably be managed here


    void InitialiseCharacter()
    {
        // triggered by choosing character from character select screen or from loading into level in adventure mode (UIManager)

        // UI Manager:
        // store specific power
        // give power script access
        // update victory music
        // update other associated assets
        // prompt power to initialise
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
        ++m_playerControls.m_ballCount;

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

    void ResetLevel() // perhaps call reload
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
        // show ball trajectory line (PlayerControls)
        // reset allow0PegFreeBall flag (PlayerControls)
        // prompt power to set up if it was flagged to (PlayerControls)
        // prompt Ball-O-Tron to launch a ball (Player Controls)
        // prompt UIManager to display low ball count warning text if applicable (Player Controls)


    }

    void Shooting() // rename, presently misnomer?
    {
        // triggered by shooting the ball

        // switch GameState to MidShot
        // have UIManager update ball count text
    }


    void ResolveShot()
    {
        // triggered from 0 projectiles remaining in mid shot phase (PlayerControls)

        // PlayerControls:
        // prompt power to resolve if it was flagged to
        // prompt CameraZoom to return camera to default state
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
            case GameState.PreShot:
                break;
            case GameState.MidShot:
                break;
            case GameState.PostShot:
                // if x and y are finished, switch to pre shot
                break;
            case GameState.Reloading:
                break;
        }
    }
}
