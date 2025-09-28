using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
    File name: Music.cs
    Summary: Controls the music being played throughout the game
    Creation Date: 25/08/2025
    Last Modified: 29/09/2025
*/
public class Music : MonoBehaviour
{
    public List<AudioClip> m_songs;
    [HideInInspector] public AudioClip m_victoryMusic;
    public AudioClip m_defaultVictoryMusic;
    AudioSource m_audioSource;
    Queue<int> m_playlist;
    List<int> m_songIDs;
    bool m_paused = false;

    public void PlayVictoryMusic()
    {
        // set the audio source's audio clip to the victory music
        m_audioSource.clip = m_victoryMusic;
        m_audioSource.Play();
    }

    public void PlayNow(AudioClip a_clip, float a_time = 0.0f)
    {
        // set the audio source's clip to the new clip
        m_audioSource.clip = a_clip;
        // set the starting time of the clip
        m_audioSource.time = a_time;
        // play the clip
        m_audioSource.Play();
    }

    public void ShufflePlay()
    {
        // stop the music if its playing
        if (m_audioSource.isPlaying)
        {
            m_audioSource.Stop();
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
        m_audioSource.clip = m_songs[m_playlist.Peek()];
        m_audioSource.Play();
        // store that the music is not paused
        m_paused = false;
    }

    public void SetLoop(bool a_loop)
    {
        // set the audio source to loop or not as per the argument boolean
        m_audioSource.loop = a_loop;
    }

    public float GetTime()
    {
        // return the time in seconds into the current clip
        return m_audioSource.time;
    }

    public AudioClip GetCurrentSong()
    {
        // return the current song as an audio clip
        return m_audioSource.clip;
    }

    public void SetVolume(float a_volume)
    {
        // update the audio source volume with the new volume
        m_audioSource.volume = a_volume;
    }

    public void Pause()
    {
        // pause the audio source
        m_audioSource.Pause();
        // store that the music is paused
        m_paused = true;
    }

    public void Unpause()
    { 
        // unpause the audio source
        m_audioSource.UnPause();
        // store that the music is not paused
        m_paused = false;
    }

    void Awake()
    {
        // get the audio source from this game object
        m_audioSource = GetComponent<AudioSource>();

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
        // if the audio source is not playing anything, the playlist is not empty and the music is not paused, the current song has finished
        if (!m_audioSource.isPlaying && m_playlist.Count > 0 && !m_paused)
        {
            // add the finished song to the end of the queue and remove it from the top
            m_playlist.Enqueue(m_playlist.Dequeue());

            // set the audio source's audio clip to the first song in the queue and play it
            m_audioSource.clip = m_songs[m_playlist.Peek()];
            m_audioSource.Play();
        }
    }
}
