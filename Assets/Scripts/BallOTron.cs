using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
    File name: BallOTron.cs
    Summary: Manages the balls within the Ball-O-Tron UI display
    Creation Date: 19/01/2026
    Last Modified: 24/01/2026
*/
public class BallOTron : MonoBehaviour
{
    enum LaunchState
    { 
        Stationary,
        Dropping,
        Launched
    }


    Stack<Rigidbody2D> m_balls;
    public GameObject m_ballPrefab;
    public Rigidbody2D m_ballHolder;
    public float m_spawnHeight = 4.5f;
    public float m_holderDropDistance = 0.15f;
    public float m_holderDropSpeed = 1.0f;
    public float m_launchForce = 3.0f;
    Vector3 m_holderDefaultPosition = Vector3.zero;
    LaunchState m_launchState = LaunchState.Stationary;


    public void AddBall()
    {
        // create a ball, make it a child of the Ball-O-Tron, get its Rigidbody component and add it to the ball stack
        m_balls.Push((Instantiate(m_ballPrefab, transform) as GameObject).GetComponent<Rigidbody2D>());
        // position the ball at the spawn point
        m_balls.Peek().transform.position = transform.position + Vector3.up * m_spawnHeight; 
    }

    public void LaunchBall()
    {
        // unlock the ball holder's y position
        m_ballHolder.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezeRotation;
        // store that the ball holder should be dropping in anticipation for the launch
        m_launchState = LaunchState.Dropping;        
    }

    private void Awake()
    {
        // initialise the balls stack
        m_balls = new Stack<Rigidbody2D>();
        // store the default position of the ball holder
        m_holderDefaultPosition = m_ballHolder.transform.position;
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        // TEMP
        if (Input.GetKeyDown(KeyCode.B))
        {
            AddBall();
        }
        if (Input.GetKeyDown(KeyCode.L))
        {
            LaunchBall();
        }

        // if the ball holder should be dropping
        if (m_launchState == LaunchState.Dropping)
        {
            // move the ball holder down by its speed
            m_ballHolder.transform.position += Vector3.down * m_holderDropSpeed * Time.unscaledDeltaTime;
            // if the ball holder has moved low enough
            if (m_ballHolder.transform.position.y <= m_holderDefaultPosition.y - m_holderDropDistance)
            {
                // apply the launch force to the ball holder
                m_ballHolder.AddForce(Vector2.up * m_launchForce);
                // store that the launch has occured
                m_launchState = LaunchState.Launched;
            }
        }
        // otherwise, if the launch has occured
        else if (m_launchState == LaunchState.Launched)
        {
            // if the ball holder has gone up back to its default height
            if (m_ballHolder.transform.position.y >= m_holderDefaultPosition.y)
            {
                // reset its position
                m_ballHolder.transform.position = m_holderDefaultPosition;
                // reset its velocity
                m_ballHolder.velocity = Vector2.zero;
                // lock its y position
                m_ballHolder.constraints = RigidbodyConstraints2D.FreezeAll;
            }

            // if the first ball in the stack has gone up to the spawn height
            if (m_balls.Peek().transform.position.y >= transform.position.y + m_spawnHeight)
            {
                // remove the first ball in the stack and destroy it
                Destroy(m_balls.Pop());
                // store that the launch state is now stationary
                m_launchState = LaunchState.Stationary;
            }
            
        }
    }
}
