using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
    File name: BallOTron.cs
    Summary: Manages the balls within the Ball-O-Tron UI display
    Creation Date: 19/01/2026
    Last Modified: 07/09/2026
*/
public class BallOTron : MonoBehaviour
{
    enum LaunchState
    {
        Idle,
        Retracting,
        Launching
    }

    [Header("Ball Spawning")]
    public GameObject m_BallOTronBallPrefab;
    public float m_spawnHeight = 4.5f;
    public float m_destroyHeight = 5.0f;
    public float m_lowestAllowedVerticalVelocity = 0.01f;
    public float m_ballStationaryConversionDelay = 0.1f;
    float m_ballStopTimer = 0.0f;

    [Header("Ball Launching")]
    public Rigidbody2D m_ballHolder;
    public float m_holderDropDistance = 0.4f;
    public float m_holderDropSpeed = 1.0f;
    public float m_holderLaunchForce = 10.0f;
    public float m_topBallLaunchForce = 3.0f;
    public float m_secondBallLaunchForce = 0.3f;
    public float m_finalBallMass = 1.15f;
    Vector3 m_holderDefaultPosition;
    SpringJoint2D m_holderSpring;
    LaunchState m_launchState = LaunchState.Idle;
    Rigidbody2D m_launchedBall = null;
    int m_waitingToLaunch = 0;
    float m_defaultBallMass = 0.0f;

    [Header("Ball Group")]
    Vector3 m_ballGroupDefaultPosition = Vector3.zero;
    Rigidbody2D m_ballGroupRigidbody;
    BoxCollider2D m_ballGroupCollider;
    Vector2 m_ballGroupColliderSize = Vector2.zero;
    Stack<Rigidbody2D> m_groupBalls;
    Queue<Rigidbody2D> m_newBalls;

    public void LaunchBall()
    {
        // if there are balls in the Ball-O-Tron
        if (m_groupBalls.Count > 0)
        {
            // if there are no new balls and the launcher is idle, the Ball-O-Tron is ready to launch
            if (m_newBalls.Count == 0 && m_launchState == LaunchState.Idle)
            {
                // set the launch state to retracting
                m_launchState = LaunchState.Retracting;
                // disable the ball holder spring
                m_holderSpring.enabled = false;

                // remove a ball from the waiting to launch counter if the counter isn't at 0
                m_waitingToLaunch -= (m_waitingToLaunch == 0) ? 0 : 1;
            }
            // if there are still new balls that are not yet part of the main ball group, or if the launcher is not idle, the Ball-O-Tron is not ready to launch
            else
            {
                // add a ball is to the waiting to launch counter
                ++m_waitingToLaunch;
            }
        }
    }

    public void AddBall()
    {
        // create a new ball
        Rigidbody2D newBall = Instantiate(m_BallOTronBallPrefab, transform.parent).GetComponent<Rigidbody2D>();
        // add the ball to the new balls queue
        m_newBalls.Enqueue(newBall);
        // position the ball at the spawn point
        newBall.transform.position = transform.position + Vector3.up * m_spawnHeight; // most recently made
    }

    public void SetBallCount(int a_ballCount)
    {
        // loop for each excess ball
        while (m_groupBalls.Count > a_ballCount)
        {
            // remove the top ball from the group balls stack and destroy it
            Destroy(m_groupBalls.Pop().gameObject);
        }

        // loop for each ball that needs to be added 
        for (int i = m_groupBalls.Count; i < a_ballCount; ++i)
        {
            // create the ball
            AddBall();
            // convert the ball to a placeholder ball in the main ball group
            ConvertToPlaceholderBall();
        }
        
        // resize the ball group
        ResizeBallGroup();
    }

    void ResizeBallGroup()
    {
        // if the ball group collider is enabled but there are 0 balls in the group
        if (m_ballGroupCollider.enabled && m_groupBalls.Count == 0)
        {
            // disable the collider
            m_ballGroupCollider.enabled = false;
            // disable the ball group physics
            m_ballGroupRigidbody.isKinematic = true;
            // ensure the ball group is not affected by physics that had been previously affecting it
            m_ballGroupRigidbody.velocity = Vector2.zero;
            // ensure the ball group is at its default position
            transform.position = m_ballGroupDefaultPosition;
        }
        // otherwise if the ball group collider is disabled, or if there is more than 1 ball in the group
        else
        {
            // if the ball group collider is currently disabled but there are balls in the group
            if (!m_ballGroupCollider.enabled && m_groupBalls.Count > 0)
            {
                // enable the collider
                m_ballGroupCollider.enabled = true;
                // enable the ball group physics
                m_ballGroupRigidbody.isKinematic = false;
            }

            // scale the collider size to the total height of each of the balls in the group ball stack
            m_ballGroupColliderSize.y = m_BallOTronBallPrefab.transform.localScale.y * m_groupBalls.Count;
            // apply the collider scale
            m_ballGroupCollider.size = m_ballGroupColliderSize;
            // position the ball group to be on top of the ball holder
            m_ballGroupCollider.offset = Vector2.up * (0.5f * m_ballGroupCollider.size);
        }
    }

    void ConvertToPlaceholderBall()
    {
        // set the ball to the default ball mass in case it was modified
        m_newBalls.Peek().mass = m_defaultBallMass;
        // stop physics from affecting the ball
        m_newBalls.Peek().isKinematic = true;
        // disable the ball's collider
        m_newBalls.Peek().GetComponent<Collider2D>().enabled = false;
        // position the ball on the top of the ball group
        m_newBalls.Peek().transform.position = transform.position + Vector3.up * ((m_groupBalls.Count + 0.5f) * m_newBalls.Peek().transform.localScale.y);
        // make the ball a child of the ball group
        m_newBalls.Peek().transform.parent = transform;
        // move the ball from the new balls queue to the group balls stack
        m_groupBalls.Push(m_newBalls.Dequeue());
    }



    private void Awake()
    {
        // initialise the m_groupBalls stack
        m_groupBalls = new Stack<Rigidbody2D>();
        // initialise the m_newBalls queue
        m_newBalls = new Queue<Rigidbody2D>();
        // get the ball stack default position
        m_ballGroupDefaultPosition = transform.position;
        // get the ball stack rigidbody
        m_ballGroupRigidbody = GetComponent<Rigidbody2D>();
        // get the ball stack collider
        m_ballGroupCollider = GetComponent<BoxCollider2D>();
        // store the initial ball group collider size
        m_ballGroupColliderSize = m_ballGroupCollider.size;
        // start with the ball group unaffected by phyiscs
        m_ballGroupRigidbody.isKinematic = true;
        // start with the ball group collider disabled
        m_ballGroupCollider.enabled = false;
        // store the default ball holder position
        m_holderDefaultPosition = m_ballHolder.transform.position;
        // get the spring component from the ball holder
        m_holderSpring = m_ballHolder.GetComponent<SpringJoint2D>();
        // store the default ball mass
        m_defaultBallMass = m_BallOTronBallPrefab.GetComponent<Rigidbody2D>().mass;
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
        if (Input.GetKeyDown(KeyCode.S))
        {
            SetBallCount(10);
        }
        if (Input.GetKeyDown(KeyCode.Keypad0))
        {
            SetBallCount(0);
        }
        // TEMP

        if (Time.timeScale > 0.0f)
        {
            // if there is a launched ball and it has surpassed the destroy height
            if (m_launchedBall != null && m_launchedBall.transform.position.y > m_destroyHeight)
            {
                // destroy the launched ball
                Destroy(m_launchedBall.gameObject);

                // if there are now 0 new balls and a ball was waiting to launch
                if (m_newBalls.Count == 0 && m_waitingToLaunch > 0)
                {
                    // launch it
                    LaunchBall();
                }
            }

            // if the launch state is Launching
            if (m_launchState == LaunchState.Launching)
            {
                // if the ball holder has returned to its default position
                if (m_ballHolder.transform.position.y >= m_holderDefaultPosition.y)
                {
                    // set the ball holder's position to its default position
                    m_ballHolder.transform.position = m_holderDefaultPosition;
                    // set the ball holder's velocity to 0
                    m_ballHolder.velocity = Vector3.zero;
                    // store that the launch state is now Idle
                    m_launchState = LaunchState.Idle;

                    // make the ball group a child of its original parent again
                    m_ballGroupRigidbody.transform.parent = m_ballHolder.transform.parent;

                    // if there is still at least one ball in the ball group
                    if (m_groupBalls.Count > 0)
                    {
                        // set the ball group to be affected by physics again
                        m_ballGroupRigidbody.isKinematic = false;
                    }

                    // enable the ball holder spring
                    m_holderSpring.enabled = true;
                }
            }
            // otherwise, if the launch state is Retracting
            else if (m_launchState == LaunchState.Retracting)
            {
                // move the ball holder down 
                m_ballHolder.transform.position -= Vector3.up * m_holderDropSpeed * Time.unscaledDeltaTime;

                // if the ball holder has moved down enough
                if (m_ballHolder.transform.position.y <= m_holderDefaultPosition.y - m_holderDropDistance)
                {
                    // pop the top ball from the group balls stack
                    m_launchedBall = m_groupBalls.Pop();
                    // make the ball affected by physics
                    m_launchedBall.isKinematic = false;
                    // make the ball a child of the Ball-O-Tron rather than the ball group
                    m_launchedBall.transform.parent = transform.parent;
                    // apply an upwards impulse force to the top ball
                    m_launchedBall.AddForce(Vector3.up * m_topBallLaunchForce, ForceMode2D.Impulse);

                    // if there is at least 1 other ball
                    if (m_groupBalls.Count > 0)
                    {
                        // make the second ball affected by physics
                        m_groupBalls.Peek().isKinematic = false;

                        // if there is exactly one other ball
                        if (m_groupBalls.Count == 1)
                        {
                            // set the ball's mass to the FinalBallMass so it's trajectory better matches the secondary ball trajectory at other ball counts
                            m_groupBalls.Peek().mass = m_finalBallMass;
                        }

                        // enable the ball's collider
                        m_groupBalls.Peek().GetComponent<Collider2D>().enabled = true;
                        // make the ball a child of the Ball-O-Tron rather than the ball group
                        m_groupBalls.Peek().transform.parent = transform.parent;
                        // apply an upwards impulse force to the ball
                        m_groupBalls.Peek().AddForce(Vector3.up * m_secondBallLaunchForce, ForceMode2D.Impulse);
                        // store the ball as a new ball for the purposes of returning it to the group ball stack
                        m_newBalls.Enqueue(m_groupBalls.Pop());
                    }

                    // resize the ball group to correspond to the new ball count
                    ResizeBallGroup();

                    // if there is still at least one ball in the ball group
                    if (m_groupBalls.Count > 0)
                    {
                        // make the ball group a child of the ball holder
                        m_ballGroupRigidbody.transform.parent = m_ballHolder.transform;

                        // prevent the ball group from being affected by physics
                        m_ballGroupRigidbody.isKinematic = true;
                    }

                    // position the ball holder exactly at its designated drop height
                    m_ballHolder.transform.position = m_holderDefaultPosition - Vector3.up * m_holderDropDistance;
                    // apply an upwards impulse force to the ball holder
                    m_ballHolder.AddForce(Vector3.up * m_holderLaunchForce, ForceMode2D.Impulse);

                    // store that the launch state is now Launching
                    m_launchState = LaunchState.Launching;
                }
            }

            // if there is a new ball and it has stopped falling
            if (m_newBalls.Count > 0 && Mathf.Abs(m_newBalls.Peek().velocity.y) < m_lowestAllowedVerticalVelocity)
            {
                // increase the timer
                m_ballStopTimer += Time.unscaledDeltaTime;

                // if the max time that the ball can be low velocity has been reached
                if (m_ballStopTimer >= m_ballStationaryConversionDelay)
                {
                    // convert the ball to a placeholder ball that is part of the main ball group
                    ConvertToPlaceholderBall();
                    // have the ball group collider resize to account for the new ball
                    ResizeBallGroup();
                    // reset the timer
                    m_ballStopTimer = 0.0f;

                    // if there are now 0 new balls and a ball was waiting to launch
                    if (m_newBalls.Count == 0 && m_waitingToLaunch > 0)
                    {
                        // launch it
                        LaunchBall();
                    }

                }
            }
            // if the ball's velocity is high enough
            else
            {
                // reset the timer
                m_ballStopTimer = 0.0f;
            }
        }
    }
}