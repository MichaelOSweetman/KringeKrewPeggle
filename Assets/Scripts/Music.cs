using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
    File name: Music.cs
    Summary: Controls the music being played throughout the game
    Creation Date: 25/08/2025
    Last Modified: 25/08/2025
*/
public class Music : MonoBehaviour
{
    public List<AudioClip> m_songs;
    AudioSource m_audioSource;
    Queue<int> m_playlist;
    List<int> m_songIDs;

    void ShufflePlay()
    {
        // stop the music
        m_audioSource.Stop();

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
    }

    // Start is called before the first frame update
    void Start()
    {
        // get the audio source from this game object
        m_audioSource = GetComponent<AudioSource>();

        // initialise the playlist queue
        m_playlist = new Queue<int>();

        // initialise the song IDs list
        m_songIDs = new List<int>();

        // TEMP
        ShufflePlay();
    }

    // Update is called once per frame
    void Update()
    {
        // if the current song has finished
        if (!m_audioSource.isPlaying)
        {
            // add the finished song to the end of the queue and remove it from the top
            m_playlist.Enqueue(m_playlist.Dequeue());

            // set the audio source's audio clip to the first song in the queue and play it
            m_audioSource.clip = m_songs[m_playlist.Peek()];
            m_audioSource.Play();
        }
    }
}
