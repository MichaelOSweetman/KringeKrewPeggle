using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/*
    File name: PegManager.cs
    Summary: Manages a set of pegs and determines which are orange, purple, green and blue. It also determines the amount of points they give, as well as when they are removed as a result of being hit
    Creation Date: 09/10/2023
    Last Modified: 25/03/2024
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

    [Header("Player Controls")]
    public PlayerControls m_playerControls;

    [Header("Peg Colours")]
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

    [Header("Score")]
    public GameObject m_pegScoreTextPrefab;
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
    int m_score = 0;

    [Header("Free Ball From Score")]
    public RectTransform m_freeBallProgressBar;
    public RawImage m_freeBallProgressBarBackground;
    public int[] m_freeBallScores;
    public Color[] m_freeBallProgressBarColors;
    RawImage m_freeBallProgressBarImage;
    float m_freeBallProgressBarHeight = 0.0f;
    int m_freeBallsAwarded = 0;

    [Header("Victory")]
    public GameObject m_bucket;
    public GameObject m_victoryBuckets;
    public GameObject m_NearVictoryDetectorPrefab;

    Peg[] m_activePegs;
    Queue<Peg> m_hitPegs;
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

    public void AddScore(int a_scoreIncrease)
    {
        // increase the score
        m_score += a_scoreIncrease;
        // update the score text
        m_scoreText.text = m_score.ToString();
    }

    public void ResolveTurn()
    {
        // add the score gained in this shoot phase to the total score
        AddScore(m_currentShootPhaseScore);
        // reset the current shoot phase score tracker
        m_currentShootPhaseScore = 0;
        // reset the free balls awarded
        m_freeBallsAwarded = 0;
        // reset the free ball progress bar
        UpdateFreeBallProgressBar();
        // assign a random blue peg to be purple
        ReplacePurplePeg();
        // set the GameState to Round Set Up as the Turn has resolved
        m_playerControls.m_currentGameState = PlayerControls.GameState.TurnSetUp;
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
            m_playerControls.FreeBall(true);
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
        // if the peg is in the active list, it has not been hit yet
        if (m_activePegs[a_pegID])
        {
            // add the peg to the hit pegs queue
            m_hitPegs.Enqueue(m_activePegs[a_pegID]);

            // set the peg's colour to the hit version of its colour
            SetPegType(m_activePegs[a_pegID], m_activePegs[a_pegID].m_pegType, true);


            // determine the points gained from hitting this peg with the current score multiplier
            switch (m_activePegs[a_pegID].m_pegType)
            {
                case PegType.Blue:
                    m_hitPegScore = m_baseBluePegScore * m_scoreMultipliers[m_scoreMultiplierIndex];
                    // remove the peg from the list of active blue pegs
                    m_activeBluePegs.Remove(m_activePegs[a_pegID]);
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
                    }
                    // if this was not the last orange peg
                    else
                    {
                        // if this was the second last orange peg
                        if (m_hitOrangePegs == m_startingOrangePegCount - 1)
                        {
                            // loop through the active pegs
                            for (int i = 0; i < m_activePegs.Length; ++i)
                            {
                                // if this is not the peg that was just hit, it is active and it is orange
                                if (i != a_pegID && m_activePegs[i] != null && m_activePegs[i].m_pegType == PegType.Orange)
                                {
                                    // create a near victory detector
                                    GameObject nearVictoryDetector = Instantiate(m_NearVictoryDetectorPrefab);
                                    // set the detectors position to the orange pegs position
                                    nearVictoryDetector.transform.position = m_activePegs[i].transform.position;
                                    // give the victory detector access to the camera used by Player Controls
                                    nearVictoryDetector.GetComponent<NearVictoryDetector>().m_cameraZoom = m_playerControls.m_cameraZoom;

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
                    m_playerControls.m_greenPegPower(PlayerControls.PowerFunctionMode.Trigger, m_activePegs[a_pegID].transform.position);
                    break;

            }

            // instantiate the peg score text prefab
            GameObject scoreText = Instantiate(m_pegScoreTextPrefab) as GameObject;
            // set the text's parent to be the canvas
            scoreText.transform.SetParent(m_canvas.transform, false);
            // set the text to display the score gained for hitting the peg
            scoreText.GetComponent<Text>().text = m_hitPegScore.ToString();
            // position the text using the screen position of the hit peg and the text's position offset as stored in the prefab
            scoreText.transform.position = m_camera.WorldToScreenPoint(m_activePegs[a_pegID].transform.position) + m_pegScoreTextPrefab.transform.position;

            // update the score for this shoot phase with the score gained from the hit peg
            UpdatePhaseScore(m_hitPegScore);

            // remove the peg from the active peg array
            m_activePegs[a_pegID] = null;
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

    // Start is called before the first frame update
    void Start()
    {
        // get the current height of the free ball progress bar
        m_freeBallProgressBarHeight = m_freeBallProgressBar.sizeDelta.y;
        // get the raw image component of the progress bar
        m_freeBallProgressBarImage = m_freeBallProgressBar.GetComponent<RawImage>();
        // initialise the progress bar height
        UpdateFreeBallProgressBar();

        // create an array to store the different score multipliers
        m_scoreMultipliers = new int[5] { 1, 2, 3, 5, 10 };
        // create an array to store the orange peg thresholds at which the score multiplier will increase, with the last value as unreachable
        // TEMP { 10, 15, 19, 22 }
        m_multiplierIncreaseThresholds = new int[5] { 7, 9, 11, 13, m_startingOrangePegCount + 1 };

        // create an array the size of the amount of children the peg manager has
        m_activePegs = new Peg[transform.childCount];
        // create a list to store all active blue pegs
        m_activeBluePegs = new List<Peg>();
        // initialise the hit pegs queue
        m_hitPegs = new Queue<Peg>();

        // create a hash set to store the IDs of all pegs that will start as orange or green
        HashSet<int> orangeAndGreenPegIDs = new HashSet<int>();
        // create an integer variable to store the ID for the peg to be added to the hash set
        int randomPegID = 0;
                
        // loop for each starting orange and green peg
        for (int i = 0; i < m_startingOrangePegCount + m_startingGreenPegCount; ++i)
        {
            // get a random peg ID that has not yet been assigned to a peg
            do
            {
                randomPegID = Random.Range(0, transform.childCount);
            }
            while (orangeAndGreenPegIDs.Contains(randomPegID));
        
            // add the ID to to the hash set
            orangeAndGreenPegIDs.Add(randomPegID);
            // add the peg to the active pegs array
            m_activePegs[randomPegID] = transform.GetChild(randomPegID).gameObject.GetComponent<Peg>();
            // set the peg's type to orange, or green if all orange pegs have been assigned
            SetPegType(m_activePegs[randomPegID], (i < m_startingOrangePegCount) ? PegType.Orange : PegType.Green, false);
        }

        // loop for each child
        for (int i = 0; i < transform.childCount; ++i)
        {
            // if the peg has not yet been assigned, it is should start as a blue peg
            if (m_activePegs[i] == null)
            {
                // add the child to the array of active pegs
                m_activePegs[i] = transform.GetChild(i).gameObject.GetComponent<Peg>();
                // add the peg to the list of active blue pegs
                m_activeBluePegs.Add(m_activePegs[i]);
            }
        }

        // assign a random blue peg to be purple
        ReplacePurplePeg();
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
