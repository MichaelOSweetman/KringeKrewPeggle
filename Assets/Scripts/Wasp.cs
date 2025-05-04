using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
    File name: Wasp.cs
    Summary: Manages the movement and behaviour of a wasp created by the Daniel Power
    Creation Date: 27/11/2023
    Last Modified: 04/05/2025
*/
public class Wasp : MonoBehaviour
{
    [HideInInspector] public Peg m_targetPeg;
    [HideInInspector] public DanielPower m_danielPower;
    public float m_speed = 2.0f;
    public float m_bobSpeed = 5.0f;
    public float m_bobAlternateTime = 0.1f;
    SpriteRenderer m_spriteRenderer;
    Vector3 m_bobDirection = Vector3.up;
    float m_timer = 0.0f;

    void Awake()
    {
        // store the wasp's sprite renderer component
        m_spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        // move towards the target
        transform.position = Vector3.MoveTowards(transform.position, m_targetPeg.transform.position, m_speed * Time.deltaTime);

        // bob the wasp
        transform.position += m_bobDirection * m_bobSpeed * Time.deltaTime;

        // increase the timer
        m_timer += Time.deltaTime;
        // if the timer limit has been reached
        if (m_timer >= m_bobAlternateTime)
        {
            // reset the timer
            m_timer = 0.0f;
            // flip the bob direction
            m_bobDirection *= -1.0f;
        }

        // flip the wasp sprite to look in the direction of its target
        m_spriteRenderer.flipX = (m_targetPeg.transform.position.x < transform.position.x);
    }

    private void OnTriggerEnter2D(Collider2D a_collision)
    {
        // if the wasp has collided with its target peg
        if (a_collision.gameObject == m_targetPeg.gameObject)
        {
            // hit the target peg
            m_targetPeg.Hit();
            // remove this wasp from the wasp list
            m_danielPower.m_wasps.Remove(gameObject);
            // destroy this wasp
            Destroy(gameObject);
        }
    }
}
