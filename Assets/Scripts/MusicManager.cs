using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
    File name: MusicManager.cs
    Summary: Controls the music being played throughout the game
    Creation Date: 25/08/2025
    Last Modified: 06/10/2025
*/
public class MusicManager : MonoBehaviour
{
    public List<AudioClip> m_songs;
    [HideInInspector] public AudioClip m_victoryMusic;
    public AudioClip m_defaultVictoryMusic;
    AudioSource[] m_audioSources;
    Queue<int> m_playlist;
    List<int> m_songIDs;
    bool m_playlistPaused = false;
    [HideInInspector] public AudioSource m_activeAudioSource;

    public void PlayVictoryMusic(int a_sourceID)
    {
        // set the audio source's audio clip to the victory music
        m_audioSources[a_sourceID].clip = m_victoryMusic;
        m_audioSources[a_sourceID].Play();

        // set this audio source to be the active audio source
        m_activeAudioSource = m_audioSources[a_sourceID];
    }

    public void PlayNow(int a_sourceID, AudioClip a_clip, float a_time = 0.0f)
    {
        // set the audio source's clip to the new clip
        m_audioSources[a_sourceID].clip = a_clip;
        // set the starting time of the clip
        m_audioSources[a_sourceID].time = a_time;
        // play the clip
        m_audioSources[a_sourceID].Play();

        // set this audio source to be the active audio source
        m_activeAudioSource = m_audioSources[a_sourceID];
    }

    public void ShufflePlay()
    {
        // stop the music if its playing
        if (m_audioSources[0].isPlaying)
        {
            m_audioSources[0].Stop();
        }

        // clear the playlist
        m_playlist.Clear();

        // reset the song IDs list
        m_songIDs.Clear();
        for (int i = 0; i < m_songs.Count; ++i)
        {
            m_songIDs.Add(i); // { 0, 1, 2, 3, 4}
        }

        // loop for each song
        for (int i = 0; i < m_songs.Count; ++i)
        {
            // get a random index between of the song ID list
            int randomIndex = Random.Range(0, m_songIDs.Count - 1);
            // add the songID at this index to the playlist
            m_playlist.Enqueue(m_songIDs[randomIndex]);
            // remove this song ID from the song ID list
            m_songIDs.RemoveAt(randomIndex); // TEMP - Ideally do this with a linked list
        }

        // set the audio source's audio clip to the first song in the queue and play it
        m_audioSources[0].clip = m_songs[m_playlist.Peek()];
        m_audioSources[0].Play();

        // set this audio source to be the active audio source
        m_activeAudioSource = m_audioSources[0];

        // store that the playlist is not paused
        m_playlistPaused = false;
    }

    public void SetLoop(int a_sourceID, bool a_loop)
    {
        // set the audio source to loop or not as per the argument boolean
        m_audioSources[a_sourceID].loop = a_loop;
    }

    public float GetTime(int a_sourceID)
    {
        // return the time in seconds into the current clip
        return m_audioSources[a_sourceID].time;
    }

    public AudioClip GetCurrentSong(int a_sourceID)
    {
        // return the current song as an audio clip
        return m_audioSources[a_sourceID].clip;
    }

    public void SetVolume(float a_volume)
    {
        // loop for each audio source
        for (int i = 0; i < m_audioSources.Length; ++i)
        {
            // update the audio source volume with the new volume
            m_audioSources[i].volume = a_volume;
        }
    }

    public void UnpauseActiveAudioSource()
    {
        m_activeAudioSource.UnPause();

        // if the active audio source is the playlist audio source
        if (m_activeAudioSource == m_audioSources[0])
        {
            // store that the playlist is no longer paused
            m_playlistPaused = false;
        }
    }

    public void PauseAll()
    {
        // loop for each audio source
        for (int i = 0; i < m_audioSources.Length; ++i)
        {
            // pause the audio source
            m_audioSources[i].Pause();
        }
        // store that the playlist is paused
        m_playlistPaused = true;
    }

    public void Pause(int a_sourceID)
    {
        // pause the audio source
        m_audioSources[a_sourceID].Pause();
        // if the active audio source is the playlist audio source
        if (m_activeAudioSource == m_audioSources[0])
        {
            // store that the playlist is paused
            m_playlistPaused = true;
        }
    }

    public void Unpause(int a_sourceID)
    { 
        // unpause the audio source
        m_audioSources[a_sourceID].UnPause();
        // if the active audio source is the playlist audio source
        if (m_activeAudioSource == m_audioSources[0])
        {
            // store that the playlist is no longer paused
            m_playlistPaused = false;
        }

        // set this audio source to be the active audio source
        m_activeAudioSource = m_audioSources[a_sourceID];
    }

    void Awake()
    {
        // get the audio sources from this game object
        m_audioSources = GetComponents<AudioSource>();

        // store the first audio source as the active audio source
        m_activeAudioSource = m_audioSources[0];

        // initialise the playlist queue
        m_playlist = new Queue<int>();

        // initialise the song IDs list
        m_songIDs = new List<int>();

        // set the victory music to the default if it hasn't already been set
        if (m_victoryMusic == null)
        {
            m_victoryMusic = m_defaultVictoryMusic;
        }
    }

    // Update is called once per frame
    void Update()
    {   
        // if the audio source responsible for shuffle play is not playing anything, the playlist is not empty and the music is not paused, the current song has finished
        if (!m_audioSources[0].isPlaying && m_playlist.Count > 0 && !m_playlistPaused)
        {
            // add the finished song to the end of the queue and remove it from the top
            m_playlist.Enqueue(m_playlist.Dequeue());

            // set the audio source's audio clip to the first song in the queue and play it
            m_audioSources[0].clip = m_songs[m_playlist.Peek()];
            m_audioSources[0].Play();
        }
    }
}
