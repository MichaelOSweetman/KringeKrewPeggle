using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
	File name: DanielPower.cs
	Summary: Manages the power gained from the green peg when playing as Daniel
	Creation Date: 27/01/2025
	Last Modified: 17/02/2025
*/
public class DanielPower : GreenPegPower
{
    public GameObject m_wasp;
    public float m_searchRadius = 5.0f;
    [HideInInspector] public List<GameObject> m_wasps;

    public override void Trigger(Vector3 a_greenPegPosition)
	{
        // get an array of all colliders around the green peg
        Collider2D[] CollidersInRange = Physics2D.OverlapCircleAll(new Vector2(a_greenPegPosition.x, a_greenPegPosition.y), m_searchRadius);

        // loop for each collider
        for (int i = 0; i < CollidersInRange.Length; ++i)
        {
            Peg peg = CollidersInRange[i].GetComponent<Peg>();
            // if the collider has the peg component and hasn't been hit
            if (peg != null && !peg.m_hit)
            {
                // create a wasp
                GameObject wasp = Instantiate(m_wasp) as GameObject;
                // add the wasp to the wasp list
                m_wasps.Add(wasp);
                // position it on the green peg
                wasp.transform.position = a_greenPegPosition;
                // give the wasp the peg as its target
                wasp.GetComponent<Wasp>().m_targetPeg = peg;
                // give the wasp access to this script
                wasp.GetComponent<Wasp>().m_danielPower = this;
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
