using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
    File name: PegManager.cs
    Summary: Manages a set of pegs and determines which are orange, purple, green and blue. It also determines the amount of points they give, as well as when they are removed as a result of being hit
    Creation Date: 09/10/2023
    Last Modified: 09/10/2023
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
    Peg[] m_bluePegs;
    Peg m_purplePeg = null;

    public float m_clearHitPegDelay = 0.25f;
    bool m_clearHitPegQueue = false;
    float m_clearHitPegQueueTimer = 0.0f;


    public void ResolveHit(int a_pegID)
    {
        // if the peg is in the active list, it has not been hit yet
        if (m_activePegs[a_pegID])
        {
            // add the peg to the hit pegs queue
            m_hitPegs.Enqueue(m_activePegs[a_pegID]);

            // set the peg's color to the hit version
            switch (m_activePegs[a_pegID].m_pegType)
            {
                case PegType.Blue:
                    m_activePegs[a_pegID].GetComponent<SpriteRenderer>().color = m_hitBluePegColor;
                    break;
                case PegType.Orange:
                    m_activePegs[a_pegID].GetComponent<SpriteRenderer>().color = m_hitOrangePegColor;
                    break;
                case PegType.Purple:
                    m_activePegs[a_pegID].GetComponent<SpriteRenderer>().color = m_hitPurplePegColor;
                    break;
                case PegType.Green:
                    m_activePegs[a_pegID].GetComponent<SpriteRenderer>().color = m_hitGreenPegColor;
                    break;
            }

            // remove the peg from the active peg array
            m_activePegs[a_pegID] = null;
        }
    }

    public void ClearHitPegs()
    {
        // set the clear hit peg queue flag to true so the queue starts emptying
        m_clearHitPegQueue = true;
    }

    // Start is called before the first frame update
    void Start()
    {
        // create an array the size of the amount of children the peg manager has
        m_activePegs = new Peg[transform.childCount];
        // initialise the hit pegs queue
        m_hitPegs = new Queue<Peg>();

        // loop for each child
        for (int i = 0; i < transform.childCount; ++i)
        {
            // add the child to the list of active pegs
            m_activePegs[i] = transform.GetChild(i).gameObject.GetComponent<Peg>();
        }
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
                }
            }
        }
    }
}
