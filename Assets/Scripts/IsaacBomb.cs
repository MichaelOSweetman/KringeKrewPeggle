using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
    File name: IsaacBomb.cs
    Summary: Manages a bomb created by Isaac, which explodes after a set time, hitting all pegs in its blast radius
    Creation Date: 27/05/2024
    Last Modified: 03/06/2024
*/
public class IsaacBomb : MonoBehaviour
{
    public float m_duration = 2.0f;
    public float m_radius = 1.5f;
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

        // if the bomb has lasted its full duration
        if (m_timer >= m_duration)
        {
            // get an array of all colliders around the bomb
            Collider2D[] CollidersInRange = Physics2D.OverlapCircleAll(new Vector2(transform.position.x, transform.position.y), m_radius);

            // loop for each collider
            for (int i = 0; i < CollidersInRange.Length; ++i)
            {
                // if the collider is a peg, hit it
                CollidersInRange[i].GetComponent<Peg>().Hit();
            }

            // destroy this bomb
            Destroy(this.gameObject);
        }
    }
}
