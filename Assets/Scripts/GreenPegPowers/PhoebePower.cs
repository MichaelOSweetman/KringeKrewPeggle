using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
	File name: SweetsPower.cs
	Summary: Manages the power gained from the green peg when playing as Phoebe
	Creation Date: 27/01/2025
	Last Modified: 27/01/2025
*/
public class PhoebePower : GreenPegPower
{
	public int m_phoebePowerChargesGained = 3;
	public GameObject m_bocconciniPrefab;
	List<Bocconcini> m_bocconcinis;

	public override void SetUp()
	{
        // set up the bocconcini list
        m_bocconcinis = new List<Bocconcini>();

        // loop for each peg in the current level
        foreach (Peg peg in m_playerControls.m_pegManager.m_pegs)
        {
            // if the peg is not set to null, it is active
            if (peg != null)
            {
                // create a Bocconcini
                GameObject bocconcini = Instantiate(m_bocconciniPrefab) as GameObject;
                // set its parent to be the peg after creation so its scale isn't modified
                bocconcini.transform.parent = peg.transform;
                // position the bocconcini at the peg's position
                bocconcini.transform.position = peg.transform.position;
                // add it to the list of bocconcinis
                m_bocconcinis.Add(bocconcini.GetComponent<Bocconcini>());
            }
        }
    }

    void RemoveBocconcini()
    {
        // loop through the bocconcini list
        for (int i = 0; i < m_bocconcinis.Count; ++i)
        {
            // if the bocconcini hasn't already been destroyed
            if (m_bocconcinis[i] != null)
            {
                // replace it with the peg it replaced
                m_bocconcinis[i].ReenableParentPeg();
            }
        }
    }

	public override void Resolve()
	{
        if (m_bocconcinis != null && m_playerControls.m_powerCharges == 0)
        {
            RemoveBocconcini();
        }
	}

	public override void Reload()
	{
        if (m_bocconcinis != null)
        {
            RemoveBocconcini();
        }
	}
}
