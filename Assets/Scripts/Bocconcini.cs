using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
    File name: Bocconcini.cs
    Summary: Manages the Bocconcini pegs created by PhoebePower
    Creation Date: 01/07/2024
    Last Modified: 17/02/2025
*/
public class Bocconcini : MonoBehaviour
{
    SpriteRenderer m_renderer;
    Collider2D m_parentPegCollider;
    SpriteRenderer m_parentPegRenderer;
    Peg m_parentPeg;

    public void ReenableParentPeg()
    {
        // turn on the parent peg's collider and renderer
        m_parentPegCollider.enabled = true;
        m_parentPegRenderer.enabled = true;

        // destroy this bocconcini
        Destroy(gameObject);
    }

    public void CopyParentPegColor()
    {
        // set this bocconcini's color to the color of its original peg
        m_renderer.color = m_parentPegRenderer.color;
    }

    // Start is called before the first frame update
    void Start()
    {
		// get the renderer component for this bocconcini
        m_renderer = GetComponent<SpriteRenderer>();
		
		// store the parent peg's collider and renderer and peg components
		m_parentPegCollider = transform.parent.GetComponent<Collider2D>();
		m_parentPegRenderer = transform.parent.GetComponent<SpriteRenderer>();
        m_parentPeg         = transform.parent.GetComponent<Peg>();
		
		// turn off the parent peg's collider and renderer
        m_parentPegCollider.enabled = false;
        m_parentPegRenderer.enabled = false;
		
		// copy the parent peg's color
        CopyParentPegColor();
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnCollisionEnter2D(Collision2D a_collision)
    {
        // if this bocconcini was hit by a ball
        if (a_collision.gameObject.CompareTag("Ball"))
        {
            // have the original peg be hit
            m_parentPeg.Hit();
            // copy the original peg's colour so it changes to its hit colour
            CopyParentPegColor();
        }
    }
}
