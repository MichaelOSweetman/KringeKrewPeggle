using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
	File name: DanielPower.cs
	Summary: Manages the power gained from the green peg when playing as Daniel
	Creation Date: 27/01/2025
	Last Modified: 08/12/2025
*/
public class DanielPower : GreenPegPower
{
    public GameObject m_wasp;
    public float m_searchRadius = 5.0f;
    [HideInInspector] public List<Wasp> m_wasps;

    bool isTargetPeg(int a_pegID)
    {
        // loop for each wasp
        for (int i = 0; i < m_wasps.Count; ++i)
        {
            // if the wasp is targeting the peg with the argument peg ID
            if (m_wasps[i].m_targetPeg.m_pegID == a_pegID)
            {
                // return true, that the argument peg ID is that of a targeted peg
                return true;
            }
        }
        // return false, that the argument peg ID is not that of a targeted peg
        return false;
    }

    public override void Trigger(Vector3 a_greenPegPosition)
	{
        // get an array of all colliders around the green peg
        Collider2D[] CollidersInRange = Physics2D.OverlapCircleAll(new Vector2(a_greenPegPosition.x, a_greenPegPosition.y), m_searchRadius);

        // loop for each collider
        for (int i = 0; i < CollidersInRange.Length; ++i)
        {
            Peg peg = CollidersInRange[i].GetComponent<Peg>();
            // if the collider has the peg component, hasn't been hit and isn't already a targeted peg
            if (peg != null && !peg.m_hit && !isTargetPeg(peg.m_pegID))
            {
                // create a wasp and add it to the player projectiles container
                GameObject wasp = Instantiate(m_wasp, m_playerControls.m_playerProjectilesContainer);
                // add the wasp to the wasp list
                m_wasps.Add(wasp.GetComponent<Wasp>());
                // position it on the green peg
                wasp.transform.position = a_greenPegPosition;
                // give the wasp the peg as its target
                m_wasps[m_wasps.Count - 1].m_targetPeg = peg;
                // give the wasp access to this script
                m_wasps[m_wasps.Count - 1].m_danielPower = this;
            }
        }
    }

    public override void Reload()
	{
        // destroy all remaining wasps
        while (m_wasps.Count > 0)
        {
            Destroy(m_wasps[m_wasps.Count - 1]);
            m_wasps.RemoveAt(m_wasps.Count - 1);
        }
    }
}
