using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
    File name: PegManager.cs
    Summary: Manages a set of pegs and determines which are orange, purple, green and blue. It also determines the amount of points they give, as well as when they are removed as a result of being hit
    Creation Date: 09/10/2023
    Last Modified: 16/10/2023
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

    public Color m_bluePegColor;
    public Color m_hitBluePegColor;
    public Color m_orangePegColor;
    public Color m_hitOrangePegColor;
    public Color m_purplePegColor;
    public Color m_hitPurplePegColor;
    public Color m_greenPegColor;
    public Color m_hitGreenPegColor;

    public int m_startingOrangePegCount = 6;
    public int m_startingGreenPegCount = 2;
    Peg[] m_activePegs;
    Queue<Peg> m_hitPegs;
    List<Peg> m_activeBluePegs;
    Peg m_purplePeg = null;

    public float m_clearHitPegDelay = 0.25f;
    bool m_clearHitPegQueue = false;
    float m_clearHitPegQueueTimer = 0.0f;

    public void SetPegType(Peg a_peg, PegType a_pegType, bool a_hit)
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

    public void ReplacePurplePeg()
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
            // remove the peg from this list of blue pegs
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
            
            // remove the peg from the list of active blue pegs if it was in it
            m_activeBluePegs.Remove(m_activePegs[a_pegID]);

            // remove the peg from the active peg array
            m_activePegs[a_pegID] = null;

        }
    }

    public void ClearHitPegs()
    {
        // if at least 1 peg was hit
        if (m_hitPegs.Count > 0)
        {
            // set the clear hit peg queue flag to true so the queue starts emptying
            m_clearHitPegQueue = true;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
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
        // create an integer to act as a tracker for how many orange pegs have been added
        
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
        // TEMP
        if (Input.GetKeyDown(KeyCode.Q))
        {
            ClearHitPegs();
        }

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
                    // assign a random blue peg to be purple
                    ReplacePurplePeg();
                }
            }
        }
    }
}
