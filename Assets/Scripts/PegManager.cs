using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/*
	File name: PegManager.cs
	Summary: Manages a set of pegs and determines which are orange, purple, green and blue. It also determines the amount of points they give, as well as when they are removed as a result of being hit
	Creation Date: 09/10/2023
	Last Modified: 04/05/2025
*/

public class PegManager : MonoBehaviour
{
    public enum PegType
    {
        Blue,
        Orange,
        Purple,
        Green,
    }

    [Header("Other Scripts")]
    public PlayerControls m_playerControls;

    [Header("Peg Visuals")]
    public Color m_bluePegColor;
    public Color m_hitBluePegColor;
    public Color m_orangePegColor;
    public Color m_hitOrangePegColor;
    public Color m_purplePegColor;
    public Color m_hitPurplePegColor;
    public Color m_greenPegColor;
    public Color m_hitGreenPegColor;

    [Header("Starting Pegs")]
    public int m_startingOrangePegCount = 6;
    public int m_startingGreenPegCount = 2;
    int m_levelPegCount;

    [Header("Score")]
    public GameObject m_pegScoreTextPrefab;
    public Transform m_pegScoreTextParent;
    public Camera m_camera;
    public GameObject m_canvas;
    public Text m_scoreText;
    public int m_baseBluePegScore = 10;
    public int m_baseOrangePegScore = 100;
    public int m_basePurplePegScore = 500;
    public int m_baseGreenPegScore = 10;
    int[] m_scoreMultipliers;
    int[] m_multiplierIncreaseThresholds;
    int m_hitPegScore;
    int m_hitOrangePegs = 0;
    int m_scoreMultiplierIndex = 0;
    int m_currentShootPhaseScore = 0;
    [HideInInspector] public int m_score = 0;

    [Header("Free Ball From Score")]
    public RectTransform m_freeBallProgressBar;
    public RawImage m_freeBallProgressBarBackground;
    public int[] m_freeBallScores;
    public Color[] m_freeBallProgressBarColors;
    RawImage m_freeBallProgressBarImage;
    float m_freeBallProgressBarHeight = 0.0f;
    [HideInInspector] public int m_freeBallsAwarded = 0;

    [Header("Victory")]
    public GameObject m_bucket;
    public GameObject m_victoryBuckets;
    public GameObject m_nearVictoryDetector;

    [Header("Sound")]
    public AudioSource m_pegAudioSource;
    public AudioClip[] m_pegHitSounds;
    int m_pegHitPitchIndex = 0;
    public AudioClip m_pegRemoveSound;


    [HideInInspector] public List<Peg> m_pegs;
    [HideInInspector] public Queue<GameObject> m_hitPegs;
    List<Peg> m_activeBluePegs;
    Peg m_purplePeg = null;

    public float m_clearHitPegDelay = 0.25f;
    bool m_clearHitPegQueue = false;
    float m_clearHitPegQueueTimer = 0.0f;

    void SetPegType(Peg a_peg, PegType a_pegType, bool a_hit)
    {
        // set the peg's pegtype
        a_peg.m_pegType = a_pegType;

        // if the peg was hit
        if (a_hit)
        {
            // set the peg's colour to the hit version of its colour
            switch (a_peg.m_pegType)
            {
                case PegType.Blue:
                    a_peg.GetComponent<SpriteRenderer>().color = m_hitBluePegColor;
                    break;
                case PegType.Orange:
                    a_peg.GetComponent<SpriteRenderer>().color = m_hitOrangePegColor;
                    break;
                case PegType.Purple:
                    a_peg.GetComponent<SpriteRenderer>().color = m_hitPurplePegColor;
                    break;
                case PegType.Green:
                    a_peg.GetComponent<SpriteRenderer>().color = m_hitGreenPegColor;
                    break;
            }
        }
        // if the peg was not hit
        else
        {
            // set the peg's colour to the base version of its colour
            switch (a_peg.m_pegType)
            {
                case PegType.Blue:
                    a_peg.GetComponent<SpriteRenderer>().color = m_bluePegColor;
                    break;
                case PegType.Orange:
                    a_peg.GetComponent<SpriteRenderer>().color = m_orangePegColor;
                    break;
                case PegType.Purple:
                    a_peg.GetComponent<SpriteRenderer>().color = m_purplePegColor;
                    break;
                case PegType.Green:
                    a_peg.GetComponent<SpriteRenderer>().color = m_greenPegColor;
                    break;
            }
        }
    }

    public int GetAverageActivePegScore()
    {
        int totalScore = 0;
        int activePegCount = 0;

        // loop for each peg
        for (int i = 0; i < m_pegs.Count; ++i)
        {
            // if the peg is active and not hit
            if (m_pegs[i] != null && !m_pegs[i].m_hit)
            {
                // increase the active peg counter by 1
                ++activePegCount;

                // add score to the total depending on its peg type
                switch (m_pegs[i].m_pegType)
                {
                    case PegType.Blue:
                        totalScore += m_baseBluePegScore;
                        break;
                    case PegType.Orange:
                        totalScore += m_baseOrangePegScore;
                        break;
                    case PegType.Purple:
                        totalScore += m_basePurplePegScore;
                        break;
                    case PegType.Green:
                        totalScore += m_baseGreenPegScore;
                        break;

                }
            }
        }

        // return the average score or 0 if no active pegs were found
        return (activePegCount > 0) ? totalScore / activePegCount : 0;
    }

    public void AddScore(int a_scoreIncrease)
    {
        // increase the score
        m_score += a_scoreIncrease;
        // update the score text
        m_scoreText.text = m_score.ToString();
    }

    public void ResetTurnScore()
    {
        // reset the current shoot phase score tracker
        m_currentShootPhaseScore = 0;
        // reset the free balls awarded
        m_freeBallsAwarded = 0;
        // reset the free ball progress bar
        UpdateFreeBallProgressBar();
        // reset the peg hit sound
        m_pegHitPitchIndex = 0;
    }

    public void ResolveTurn()
    {
        // add the score gained in this shoot phase to the total score
        AddScore(m_currentShootPhaseScore);
        // reset the score trackers for the turn
        ResetTurnScore();
        // assign a random blue peg to be purple
        ReplacePurplePeg();
    }

    void UpdateFreeBallProgressBar()
    {
        // if the max amount of free balls have been awarded
        if (m_freeBallsAwarded >= m_freeBallProgressBarColors.Length)
        {
            // set the colours of the background and foreground to the final colour
            m_freeBallProgressBarBackground.color = m_freeBallProgressBarColors[m_freeBallProgressBarColors.Length];
            m_freeBallProgressBarImage.color = m_freeBallProgressBarColors[m_freeBallProgressBarColors.Length];
        }
        else
        {
            // set the colours of the background and foreground based on the amount of free balls that have been awarded
            m_freeBallProgressBarBackground.color = m_freeBallProgressBarColors[m_freeBallsAwarded];
            m_freeBallProgressBarImage.color = m_freeBallProgressBarColors[m_freeBallsAwarded + 1];
        }

        // modify the free ball progress bar to be representative of the score progress of reaching the next free ball milestone
        m_freeBallProgressBar.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, Mathf.Clamp01((float)(m_currentShootPhaseScore - ((m_freeBallsAwarded == 0) ? 0 : m_freeBallScores[m_freeBallsAwarded - 1])) / (float)(m_freeBallScores[m_freeBallsAwarded] - ((m_freeBallsAwarded == 0) ? 0 : m_freeBallScores[m_freeBallsAwarded - 1]))) * m_freeBallProgressBarHeight);
    }

    void UpdatePhaseScore(int a_scoreIncrease)
    {
        // add the score increase to the phase's total
        m_currentShootPhaseScore += a_scoreIncrease;

        // if the free ball milestone has been reached
        if (m_currentShootPhaseScore >= m_freeBallScores[m_freeBallsAwarded])
        {
            // give the player a free ball and show the free ball text
            m_playerControls.FreeBall(true, true);

            // switch to the next free ball milestone
            ++m_freeBallsAwarded;
        }

        // update the progress bar
        UpdateFreeBallProgressBar();
    }

    void ReplacePurplePeg()
    {
        // if there are active blue pegs remaining
        if (m_activeBluePegs.Count > 0)
        {
            // store the peg that was previously purple
            Peg previousPurplePeg = m_purplePeg;
            // get a random blue peg
            int randomBluePegID = Random.Range(0, m_activeBluePegs.Count);

            // set the purple peg to be a random blue peg
            m_purplePeg = m_activeBluePegs[randomBluePegID];
            // remove the peg from the list of blue pegs
            m_activeBluePegs.RemoveAt(randomBluePegID);

            // if there was a previous purple peg and it is still active
            if (previousPurplePeg && previousPurplePeg.transform.gameObject.activeSelf)
            {
                // set the previous purple peg to be blue
                SetPegType(previousPurplePeg, PegType.Blue, false);
                // add the peg back to the list of active blue pegs
                m_activeBluePegs.Add(previousPurplePeg);
            }

            // set the peg assigned to be purple to be purple
            SetPegType(m_purplePeg, PegType.Purple, false);
        }
    }

    public void ResolveHit(int a_pegID)
    {
        // if the peg is in the peg list, it has not been hit yet
        if (m_pegs[a_pegID])
        {
            // add the peg to the hit pegs queue
            m_hitPegs.Enqueue(m_pegs[a_pegID].gameObject);

            // set the peg's colour to the hit version of its colour
            SetPegType(m_pegs[a_pegID], m_pegs[a_pegID].m_pegType, true);


            // determine the points gained from hitting this peg with the current score multiplier
            switch (m_pegs[a_pegID].m_pegType)
            {
                case PegType.Blue:
                    m_hitPegScore = m_baseBluePegScore * m_scoreMultipliers[m_scoreMultiplierIndex];
                    // remove the peg from the list of active blue pegs
                    m_activeBluePegs.Remove(m_pegs[a_pegID]);
                    break;
                case PegType.Orange:
                    m_hitPegScore = m_baseOrangePegScore * m_scoreMultipliers[m_scoreMultiplierIndex];
                    // increase the hit orange pegs tracker
                    ++m_hitOrangePegs;
                    // if this was the last orange peg
                    if (m_hitOrangePegs == m_startingOrangePegCount)
                    {
                        // replace the bucket with the victory buckets
                        m_bucket.SetActive(false);
                        m_victoryBuckets.SetActive(true);
                        // set the near victory detector to have no parent
                        m_nearVictoryDetector.transform.parent = null;
                        // set the near victory detector to be inactive
                        m_nearVictoryDetector.SetActive(false);
                    }
                    // if this was not the last orange peg
                    else
                    {
                        // if this was the second last orange peg
                        if (m_hitOrangePegs == m_startingOrangePegCount - 1)
                        {
                            // loop through the pegs
                            for (int i = 0; i < m_levelPegCount; ++i)
                            {
                                // if this is not the peg that was just hit, it is active and it is orange
                                if (i != a_pegID && m_pegs[i] != null && m_pegs[i].m_pegType == PegType.Orange)
                                {
                                    //enable the near victory detector
                                    m_nearVictoryDetector.SetActive(true);
                                    // set the detectors position to the orange pegs position
                                    m_nearVictoryDetector.transform.position = m_pegs[i].transform.position;
                                    // make the detector a child of the peg so it follows it if it moves
                                    m_nearVictoryDetector.transform.parent = m_pegs[i].transform;
                                    // exit the loop as the one unhit orange peg has been found
                                    break;
                                }
                            }
                        }
                        // if the threshold for increasing the multiplier has been reached
                        if (m_hitOrangePegs == m_multiplierIncreaseThresholds[m_scoreMultiplierIndex])
                        {
                            // increase the multiplier
                            ++m_scoreMultiplierIndex;
                        }
                    }
                    break;
                case PegType.Purple:
                    m_hitPegScore = m_basePurplePegScore * m_scoreMultipliers[m_scoreMultiplierIndex];
                    break;
                case PegType.Green:
                    m_hitPegScore = m_baseGreenPegScore * m_scoreMultipliers[m_scoreMultiplierIndex];
                    // trigger the player's current power
                    m_playerControls.m_power.Trigger(m_pegs[a_pegID].transform.position);
                    break;

            }

            // instantiate the peg score text prefab
            GameObject scoreText = Instantiate(m_pegScoreTextPrefab) as GameObject;
            // set the text's parent to be the canvas
            scoreText.transform.SetParent(m_pegScoreTextParent, false);
            // set the text to display the score gained for hitting the peg
            scoreText.GetComponent<Text>().text = m_hitPegScore.ToString();
            // position the text using the screen position of the hit peg and the text's position offset as stored in the prefab
            scoreText.transform.position = m_camera.WorldToScreenPoint(m_pegs[a_pegID].transform.position) + m_pegScoreTextPrefab.transform.position;

            // move the audio source to the hit peg
            m_pegAudioSource.transform.position = m_pegs[a_pegID].transform.position;
            // set the sound of the audio source to the current peg hit sound
            m_pegAudioSource.clip = m_pegHitSounds[m_pegHitPitchIndex];
            // play the audio
            m_pegAudioSource.Play();
            // if this peg hit sound is not the last
            if (m_pegHitPitchIndex < m_pegHitSounds.Length - 1)
            {
                // increase the peg hit pitch index so the next audio clip plays next time a hit occurs
                ++m_pegHitPitchIndex;
            }

            // update the score for this shoot phase with the score gained from the hit peg
            UpdatePhaseScore(m_hitPegScore);

            // set the value in this pegs position of the peg array to null to indicate it is no longer active
            m_pegs[a_pegID] = null;
        }
    }

    public bool ClearHitPegs()
    {
        // if at least 1 peg was hit
        if (m_hitPegs.Count > 0)
        {
            // set the clear hit peg queue flag to true so the queue starts emptying
            m_clearHitPegQueue = true;
            // return true, as there were pegs to clear
            return true;
        }
        // return false, as there were no pegs to clear
        return false;
    }

    void SearchForPegs(Transform a_transform)
    {
        // if the transform has the peg container tag
        if (a_transform.CompareTag("PegContainer"))
        {
            // loop for the child's children
            for (int i = 0; i < a_transform.childCount; ++i)
            {
                // check if this transform or its children are pegs
                SearchForPegs(a_transform.GetChild(i));
            }
        }
        // if the transform is not a peg container
        else
        {
            // add the peg to the peg array
            m_pegs.Add(a_transform.GetComponent<Peg>());
            // give the peg its peg ID
            m_pegs[m_pegs.Count - 1].m_pegID = m_levelPegCount;
            // increase the peg count for the level
            ++m_levelPegCount;
        }
    }

    public void LoadLevel(Transform a_transform)
    {
        // ensure all the turn score trackers are at their initial state
        ResetTurnScore();

        // store that no orange pegs have been hit
        m_hitOrangePegs = 0;

        // reset the count for the amount of pegs in the level
        m_levelPegCount = 0;

        // reset the score multiplier index
        m_scoreMultiplierIndex = 0;

        // stop clearing hit pegs
        m_clearHitPegQueue = false;

        // set the near victory detector to have no parent
        m_nearVictoryDetector.transform.parent = null;
        // set the near victory detector to be inactive
        m_nearVictoryDetector.SetActive(false);

        // ensure the victory buckets are inactive and the regular bucket is active
        m_victoryBuckets.SetActive(false);
        m_bucket.SetActive(true);

        // initisialse the peg array and search for pegs to add to it
        m_pegs = new List<Peg>();
        SearchForPegs(a_transform);

        // create a list to store all active blue pegs
        m_activeBluePegs = new List<Peg>();
        // initialise the hit pegs queue
        m_hitPegs = new Queue<GameObject>();

        // create a hash set to store the IDs of all pegs that will start as orange or green
        HashSet<int> orangeAndGreenPegIDs = new HashSet<int>();
        // create an integer variable to store the ID for the peg to be added to the hash set
        int randomPegID = 0;

        // loop for each starting orange and green peg
        for (int i = 0; i < m_startingOrangePegCount + m_startingGreenPegCount; ++i)
        {
            // get a random peg ID that is not yet in the hash set
            do
            {
                randomPegID = Random.Range(0, m_levelPegCount);
            }
            while (orangeAndGreenPegIDs.Contains(randomPegID));

            // add the ID to to the hash set
            orangeAndGreenPegIDs.Add(randomPegID);
            // set the peg's type to orange, or green if all orange pegs have been assigned
            SetPegType(m_pegs[randomPegID], (i < m_startingOrangePegCount) ? PegType.Orange : PegType.Green, false);
        }

        // loop for each peg
        for (int i = 0; i < m_levelPegCount; ++i)
        {
            // if this peg is not an orange or green peg
            if (!orangeAndGreenPegIDs.Contains(i))
            {
                // add the peg to the list of active blue pegs
                m_activeBluePegs.Add(m_pegs[i]);
                // set the peg to be blue
                SetPegType(m_pegs[i], PegType.Blue, false);
            }
        }

        // clear the screen of peg score texts
        for (int i = m_pegScoreTextParent.transform.childCount - 1; i >= 0; --i)
        {
            // destroy the current peg score text
            Destroy(m_pegScoreTextParent.transform.GetChild(i).gameObject);
        }

        // assign a random blue peg to be purple
        ReplacePurplePeg();

        // have the player controls reset for the new level
        m_playerControls.Reload();
    }

    void Awake()
    {
        // get the current height of the free ball progress bar
        m_freeBallProgressBarHeight = m_freeBallProgressBar.sizeDelta.y;
        // get the raw image component of the progress bar
        m_freeBallProgressBarImage = m_freeBallProgressBar.GetComponent<RawImage>();
        // initialise the progress bar height
        m_freeBallProgressBar.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 0.0f);

        // create an array to store the different score multipliers
        m_scoreMultipliers = new int[5] { 1, 2, 3, 5, 10 };
        // create an array to store the orange peg thresholds at which the score multiplier will increase, with the last value as unreachable
        // TEMP { 10, 15, 19, 22 }
        m_multiplierIncreaseThresholds = new int[5] { 7, 9, 11, 13, m_startingOrangePegCount + 1 };
    }

    // Update is called once per frame
    void Update()
    {
        // if the hit peg queue should be cleared
        if (m_clearHitPegQueue)
        {
            // increase the timer
            m_clearHitPegQueueTimer += Time.deltaTime;

            // if the enough time has passed since the last peg was cleared from the queue
            if (m_clearHitPegQueueTimer >= m_clearHitPegDelay)
            {
                // loop until the next peg has not been destroyed
                while (m_hitPegs.Peek() == null)
                {
                    // remove this destroyed gameobject from the queue
                    m_hitPegs.Dequeue();
                }

                // position the peg audio source at the next peg
                m_pegAudioSource.transform.position = m_hitPegs.Peek().transform.position;
                // play the peg remove sound
                m_pegAudioSource.PlayOneShot(m_pegRemoveSound);

                // set the next peg in the queue to be inactive
                m_hitPegs.Dequeue().gameObject.SetActive(false);
                // reset the timer
                m_clearHitPegQueueTimer = 0.0f;

                // if there are no more hit pegs active
                if (m_hitPegs.Count == 0)
                {
                    // set the clear hit peg queue flag to false
                    m_clearHitPegQueue = false;
                }
            }
        }

        // TEMP
        //Debug.Log("Orange Hits: " + m_hitOrangePegs + " | " + "x" +m_scoreMultipliers[m_scoreMultiplierIndex] + " | " + "^ @" + m_multiplierIncreaseThresholds[m_scoreMultiplierIndex]);
    }
}

