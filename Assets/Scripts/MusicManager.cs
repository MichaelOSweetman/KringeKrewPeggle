using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
    File name: MusicManager.cs
    Summary: Controls the music being played throughout the game
    Creation Date: 25/08/2025
    Last Modified: 20/10/2025
*/
public class MusicManager : MonoBehaviour
{
    public List<AudioClip> m_songs;
    public float m_playlistDelay = 2.0f;
    [HideInInspector] public AudioClip m_victoryMusic;
    AudioSource m_playlistSource;
    AudioSource m_secondarySource;
    AudioSource m_activeSource;
    AudioSource m_fadeTarget;
    Queue<int> m_playlist;
    List<int> m_songIDs;
    bool m_fading = false;
    float m_fadeTimer = 0.0f;
    float m_fadeDuration = 0.0f;

    public void SwitchToPlaylist()
    {
        // store that the audio sources are not fading from one to the other
        m_fading = false;

        // ensure the playlist audio source is unpaused
        m_playlistSource.UnPause();
        // stop the secondary source
        m_secondarySource.Stop();
        // store that the playlist source is the active source
        m_activeSource = m_playlistSource;
        // ensure both audio sources are set to the correct volume
        m_playlistSource.volume = m_secondarySource.volume = GlobalSettings.m_musicVolume;
    }

    void InitialiseFade(float a_fadeDuration)
    {
        // store that the song fading is occuring
        m_fading = true;
        // store the fade duration
        m_fadeDuration = a_fadeDuration;
        // reset the fade timer
        m_fadeTimer = 0.0f;

        // unpause the fade target audio source
        m_fadeTarget.UnPause();
        // set the fade target audio source volume to 0
        m_fadeTarget.volume = 0.0f;
    }

    public void FadeToPlaylist(float a_fadeDuration)
    {
        m_fadeTarget = m_playlistSource;
        InitialiseFade(a_fadeDuration);
    }

    public void FadeToActive(float a_fadeDuration)
    {
        m_fadeTarget = m_activeSource;
        InitialiseFade(a_fadeDuration);
    }

    public void PlayNow(bool a_loop, AudioClip a_clip = null, float a_time = 0.0f)
    {
        // pause the playlist audio source
        m_playlistSource.Pause();

        // set the secondary audio source's clip to be the argument clip, using the victory music if no argument is provided
        m_secondarySource.clip = (a_clip == null) ? m_victoryMusic : a_clip;
        // set the start time of the clip
        m_secondarySource.time = a_time;
        // set the secondary source's loop state using the a_loop argument
        m_secondarySource.loop = a_loop;
        // play the clip
        m_secondarySource.Play();
        // store that the secondary source is now the active source
        m_activeSource = m_secondarySource;
    }

    public void ShufflePlay()
    {
        // stop the music if its playing from either source
        m_playlistSource.Stop();
        m_secondarySource.Stop();

        // clear the playlist
        m_playlist.Clear();

        // reset the song IDs list
        m_songIDs.Clear();
        for (int i = 0; i < m_songs.Count; ++i)
        {
            m_songIDs.Add(i);
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
        m_playlistSource.clip = m_songs[m_playlist.Peek()];
        m_playlistSource.Play();

        // store that the playlist source is the active source
        m_activeSource = m_playlistSource;
    }

    public void SetVolume()
    {
        // if two songs are presently fading from one to another
        if (m_fading)
        {
            // stop the fade prematurely, updating the volume of the audio sources as part of the process
            SwitchToPlaylist();
        }
        // if fading is not currently occuring
        else
        {
            // update both audio source volumes with the gloval settings stored music volume
            m_playlistSource.volume = m_secondarySource.volume = GlobalSettings.m_musicVolume;
        }
    }

    public void Pause()
    {
        // pause the active audio source
        m_activeSource.Pause();
    }

    public void Unpause()
    {
        // unpause the active audio source
        m_activeSource.UnPause();
    }

    void Awake()
    {
        // get the first audio source from this game object and store it as the playlist audio source
        m_playlistSource = GetComponents<AudioSource>()[0];
        // get the second audio source from this game object and store it as the secondary audio source
        m_secondarySource = GetComponents<AudioSource>()[1];
        // store that the playlist source is the active source
        m_activeSource = m_playlistSource;

        // initialise the playlist queue
        m_playlist = new Queue<int>();

        // initialise the song IDs list
        m_songIDs = new List<int>();
    }

    // Update is called once per frame
    void Update()
    {   
        // if the audio sources are fading from one to the other
        if (m_fading)
        {
            // increase the fade timer by the time passed since last frame
            m_fadeTimer += Time.unscaledDeltaTime;

            // if the duration of the fade has elapsed
            if (m_fadeTimer >= m_fadeDuration)
            {
                // if the fade target is the playlist source
                if (m_fadeTarget == m_playlistSource)
                {
                    // conclude the fade by switching to the playlist audio source
                    SwitchToPlaylist();
                }
                // if the fade target is the secondary source
                else
                {
                    // store that the audio sources are not fading from one to the other
                    m_fading = false;
                    // ensure both audio sources are set to the correct volume
                    m_playlistSource.volume = m_secondarySource.volume = GlobalSettings.m_musicVolume;
                }
            }
            // if the duration has not elapsed
            else
            {
                // set the fade target source's volume to be the fraction of how far along the timer is to the full fade duration being complete, such that it is silent at 0 seconds and full volume when the duration has elapsed
                m_fadeTarget.volume = (m_fadeTimer / m_fadeDuration) * GlobalSettings.m_musicVolume;
                // set the other source's volume to be full volume minus the fraction of how far along the timer is to the full fade duration being complete, such that it is full volume at 0 seconds and silent when the duration has elapsed
                (m_fadeTarget == m_playlistSource ? m_secondarySource : m_playlistSource).volume = GlobalSettings.m_musicVolume - m_fadeTarget.volume;
            }
        }

        // if the playlist audio source has played the clip for its full duration and the playlist is not empty
        if (m_playlistSource.time >= m_playlistSource.clip.length && m_playlist.Count > 0)
        {
            // add the finished song to the end of the queue and remove it from the top
            m_playlist.Enqueue(m_playlist.Dequeue());

            // set the audio source's audio clip to the first song in the queue and play it after a delay
            m_playlistSource.clip = m_songs[m_playlist.Peek()];
            m_playlistSource.PlayDelayed(m_playlistDelay);
        }
    }
}
