using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
	File name: PhoebePower.cs
	Summary: Manages the power gained from the green peg when playing as Phoebe
	Creation Date: 27/01/2025
	Last Modified: 10/03/2025
*/
public class PhoebePower : GreenPegPower
{
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

    public override bool OnShoot()
    {
        if (m_powerCharges > 0)
        {
            // reduce the power charges by 1
            ModifyPowerCharges(-1);
            // if there are now 0 charges
            if (m_powerCharges == 0)
            {
                // have the power resolve at the start of next turn
                m_playerControls.m_resolvePowerNextTurn = true;
            }
        }

        // return that this function should not override the default ball removal check
        return false;
    }

    public override void ResolveTurn()
    {
        // if there are power charges, and the bocconcini array has been initialised
        if (m_powerCharges > 0 && m_bocconcinis != null)
        {
            // loop through the bocconcini list
            for (int i = 0; i < m_bocconcinis.Count; ++i)
            {
                // if the bocconcini's parent peg is still active
                if (m_bocconcinis[i].transform.parent.gameObject.activeSelf)
                {
                    // update the color of the boccocinis
                    m_bocconcinis[i].CopyParentPegColor();
                }
            }
        }
    }

    public override void Reload()
	{
        if (m_bocconcinis != null)
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

        // reset the power charges
        ResetPowerCharges();
    }
}
