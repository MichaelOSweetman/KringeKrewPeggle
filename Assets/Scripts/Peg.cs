using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
    File name: Peg.cs
    Summary: Detects if the ball has hit this peg and reports to the peg manager 
    Creation Date: 09/10/2023
    Last Modified: 15/04/2024
*/
public class Peg : MonoBehaviour
{
    PegManager m_pegManager;
    [HideInInspector] public PegManager.PegType m_pegType = PegManager.PegType.Blue;
    [HideInInspector] public bool m_hit = false;
    [HideInInspector] public int m_pegID = 0;

    public void Hit()
    {
        // if the peg hadn't been hit before
        if (!m_hit)
        {
            // tell the peg manager to resolve the hit with this peg
            m_pegManager.ResolveHit(m_pegID);
        }

        // store that this peg has been hit
        m_hit = true;
    }

    // Start is called before the first frame update
    void Start()
    {
        // store that this peg's manager is it's parent
        m_pegManager = transform.GetComponentInParent<PegManager>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter2D(Collision2D a_collision)
    {
        // if this peg was hit by a ball
        if (a_collision.gameObject.CompareTag("Ball"))
        {
            // have this peg be hit
            Hit();
        }
    }
}
