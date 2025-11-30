using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
File name: IsaacTear.cs
Summary: Manages the collision and duration of a tear created by Isaac
Creation Date: 03/06/2024
Last Modified: 30/11/2025
*/
public class IsaacTear : MonoBehaviour
{
    [HideInInspector] public PlayerControls m_playerControls;
    [HideInInspector] public float m_duration = 0.0f;
    float m_timer = 0.0f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // increase the timer
        m_timer += Time.deltaTime;

        // if the tear has lasted its full duration
        if (m_timer >= m_duration)
        {
            // destroy this tear
            Destroy(gameObject);
        }
    }

    private void OnCollisionEnter2D(Collision2D a_collision)
    {
        // if the hit collider is a peg
        if (a_collision.gameObject.GetComponent<Peg>() != null)
        {
            // tell the peg that it has been hit
            a_collision.gameObject.GetComponent<Peg>().Hit();
        }

        // have player controls destroy this tear
        m_playerControls.RemoveProjectile(gameObject);
    }
}
