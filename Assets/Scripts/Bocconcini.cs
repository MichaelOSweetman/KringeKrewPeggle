using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
    File name: Bocconcini.cs
    Summary: Manages the Bocconcini pegs created by PhoebePower
    Creation Date: 01/07/2024
    Last Modified: 01/07/2024
*/
public class Bocconcini : MonoBehaviour
{
    SpriteRenderer m_renderer;
    Peg m_originalPeg;
    Collider2D m_originalPegCollider;
    SpriteRenderer m_originalPegRenderer;
    bool m_hit = false;

    public void SetOriginalPeg(Peg a_peg)
    {
        m_originalPeg = a_peg;
        m_originalPegCollider = a_peg.GetComponent<Collider2D>();
        m_originalPegRenderer = a_peg.GetComponent<SpriteRenderer>();

        // set this bocconcini's position to be the original peg's position
        transform.position = m_originalPeg.transform.position;

        // turn off the original peg's collider and renderer
        m_originalPegCollider.enabled = false;
        m_originalPegRenderer.enabled = false;

        // copy the original peg's color
        CopyOriginalPegColor();
    }

    public void ReturnOriginalPeg()
    {
        // turn on the original peg's collider and renderer
        m_originalPegCollider.enabled = true;
        m_originalPegRenderer.enabled = true;

        // destroy this bocconcini
        Destroy(this.gameObject);
    }

    public void CopyOriginalPegColor()
    {
        // if the renderer has not yet been set up
        if (m_renderer == null)
        {
            // get the renderer component for this bocconcini
             m_renderer = GetComponent<SpriteRenderer>();
        }

        // set this bocconcini's color to the color of its original peg
        m_renderer.color = m_originalPegRenderer.color;
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        /*
         TEMP determine a better way to do this
         */

        // if the bocconcini has been hit and the original peg is no longer active
        if (m_hit && m_originalPeg.gameObject.activeSelf == false)
        {
            // destroy this bocconcini
            Destroy(this.gameObject);
        }
    }

    private void OnCollisionEnter2D(Collision2D a_collision)
    {
        // if this bocconcini was hit by a ball
        if (a_collision.gameObject.CompareTag("Ball"))
        {
            // have the original peg be hit
            m_originalPeg.Hit();
            // store that the bocconcini has been hit
            m_hit = true;
            // copy the original peg's colour so it changes to its hit colour
            CopyOriginalPegColor();
        }
    }
}
