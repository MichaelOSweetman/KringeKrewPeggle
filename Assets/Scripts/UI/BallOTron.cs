using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
    File name: BallOTron.cs
    Summary: Manages the balls within the Ball-O-Tron UI display
    Creation Date: 19/01/2026
    Last Modified: 29/06/2026
*/
public class BallOTron : MonoBehaviour
{

    /*
        ball group falls if emptied
        second ball from top not behaving as intended when launched
     */

    /*
        on launch
        top two balls are disconnected
        highest launches to top
        second most does slight bounce then reconnects

        this shouldn't be attached to a collider for deleting - no collider need exist, topball should be destroyed when high up enough
    */

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
    public float m_holderLaunchForce = 100.0f;
    public float m_topBallLaunchForce = 10.0f;
    public float m_secondBallLaunchForce = 1.0f;
    Vector3 m_holderDefaultPosition;
    SpringJoint2D m_holderSpring;
    LaunchState m_launchState = LaunchState.Idle;
    Rigidbody2D m_launchedBall = null;

    [Header("Ball Group")]
    Rigidbody2D m_ballGroupRigidbody;
    BoxCollider2D m_ballGroupCollider;
    Vector2 m_ballGroupColliderSize = Vector2.zero;
    Stack<Rigidbody2D> m_balls;
    Rigidbody2D m_newBall;
    Collider2D m_newBallCollider;

    public void LaunchBall()
    {
        // set the launch state to retracting
        m_launchState = LaunchState.Retracting;
        // disable the ball holder spring
        m_holderSpring.enabled = false;
    }

    public void AddBall()
    {
        // create a new ball 
        m_newBall = Instantiate(m_BallOTronBallPrefab, transform.parent).GetComponent<Rigidbody2D>();
        // store its collider
        m_newBallCollider = m_newBall.GetComponent<Collider2D>();
        // position the ball at the spawn point
        m_newBall.transform.position = transform.position + Vector3.up * m_spawnHeight;
    }

    void ResizeBallGroup()
    {
        // if the ball group collider is enabled but there are 0 balls in the group
        if (m_ballGroupCollider.enabled && m_balls.Count == 0)
        {
            // disable the collider
            m_ballGroupCollider.enabled = false;
            // disable the ball group physics
            m_ballGroupRigidbody.isKinematic = true;
        }
        // otherwise if the ball group collider is disabled, or if there is more than 1 ball in the group
        else
        {
            // if the ball group collider is currently disabled but there are balls in the group
            if (!m_ballGroupCollider.enabled && m_balls.Count > 0)
            {
                // enable the collider
                m_ballGroupCollider.enabled = true;
                // enable the ball group physics
                m_ballGroupRigidbody.isKinematic = false;
            }

            // scale the collider size to the total height of each of the balls in the ball stack
            m_ballGroupColliderSize.y = m_BallOTronBallPrefab.transform.localScale.y * m_balls.Count;
            // apply the collider scale
            m_ballGroupCollider.size = m_ballGroupColliderSize;
            // position the ball group to be on top of the ball holder
            m_ballGroupCollider.offset = Vector2.up * (0.5f * m_ballGroupCollider.size);
        }
    }

    void ConvertToPlaceholderBall()
    {
        // stop physics from affecting the ball
        m_newBall.isKinematic = true;
        // disable the ball's collider
        m_newBallCollider.enabled = false;
        // position the ball on the top of the ball group
        m_newBall.transform.position = transform.position + Vector3.up * ((m_balls.Count + 0.5f) * m_newBall.transform.localScale.y);
        // make the ball a child of the ball group
        m_newBall.transform.parent = transform;
        // add the ball to the ball group stack
        m_balls.Push(m_newBall);
        // stop storing this ball as the new ball
        m_newBall = null;
        // have the ball group collider resize to account for the new ball
        ResizeBallGroup();
    }

    // Start is called before the first frame update
    void Start()
    {
        // initialise the m_balls stack
        m_balls = new Stack<Rigidbody2D>();
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

        // if there is a launched ball and it has surpassed the destroy height
        if (m_launchedBall != null && m_launchedBall.transform.position.y > m_destroyHeight)
        {
            // destroy the launched ball
            Destroy(m_launchedBall.gameObject);
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
                if (m_balls.Count > 0)
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
                // pop the top ball from the balls stack
                m_launchedBall = m_balls.Pop();
                // make the ball affected by physics
                m_launchedBall.isKinematic = false;
                // make the ball a child of the Ball-O-Tron rather than the ball group
                m_launchedBall.transform.parent = transform.parent;
                // apply an upwards impulse force to the top ball
                m_launchedBall.AddForce(Vector3.up * m_topBallLaunchForce, ForceMode2D.Impulse);

                // if there is a second ball
                if (m_balls.Count > 0)
                {
                    // store the second ball as a new ball for the purposes of returning it to the ball stack
                    m_newBall = m_balls.Pop();
                    // make the ball affected by physics
                    m_newBall.isKinematic = false;
                    // get the balls collider
                    m_newBallCollider = m_newBall.GetComponent<Collider2D>();
                    // enable the balls collider
                    m_newBallCollider.enabled = true;
                    // make the ball a child of the Ball-O-Tron rather than the ball group
                    m_launchedBall.transform.parent = transform.parent;
                    // apply an upwards impulse force to the top ball
                    m_launchedBall.AddForce(Vector3.up * m_secondBallLaunchForce, ForceMode2D.Impulse);
                }

                // resize the ball group to correspond to the new ball count
                ResizeBallGroup();

                // make the ball group a child of the ball holder
                m_ballGroupRigidbody.transform.parent = m_ballHolder.transform;

                // prevent the ball group from being affected by physics
                m_ballGroupRigidbody.isKinematic = true;

                // position the ball holder exactly at its designated drop height
                m_ballHolder.transform.position = m_holderDefaultPosition - Vector3.up * m_holderDropDistance;
                // apply an upwards impulse force to the ball holder
                m_ballHolder.AddForce(Vector3.up * m_holderLaunchForce, ForceMode2D.Impulse);

                // store that the launch state is now Launching
                m_launchState = LaunchState.Launching;
            }
        }

        // if there is a new ball and it has stopped falling
        if (m_newBall != null && Mathf.Abs(m_newBall.velocity.y) < m_lowestAllowedVerticalVelocity)
        {
            // increase the timer
            m_ballStopTimer += Time.unscaledDeltaTime;

            // if the max time that the ball can be low velocity has been reached
            if (m_ballStopTimer >= m_ballStationaryConversionDelay)
            {
                // convert the ball to a placeholder ball that is part of the main ball group
                ConvertToPlaceholderBall();
                // reset the timer
                m_ballStopTimer = 0.0f;
            }
        }
        // if the ball's velocity is high enough
        else
        {
            // reset the timer
            m_ballStopTimer = 0.0f;
        }


        // TEMP
        //print(m_launchState);
    }

    // TEMP

    public void SetBallCount(int a_ballCount)
    {

    }
}
/*
    public GameObject m_ballPrefab;
    public Rigidbody2D m_ballHolder;
    public float m_spawnHeight = -1.0f;
    public float m_minimumLaunchForce = 440.0f;
    public float m_launchForcePerBall = 40.0f;

    // temp
    public Rigidbody2D m_mainBallGroup;
    BoxCollider2D m_mainBallGroupCollider;
    Rigidbody2D m_topBall;
    public float m_lowestAllowedVelocitySquared = 0.01f;
    Vector2 m_colliderSize = Vector2.zero;

    private void OnTriggerEnter2D(Collider2D a_collision)
    {
        // if the object that entered this trigger is a child of this
        if (a_collision.transform.parent == transform)
        {
            // destroy the collision object
            Destroy(a_collision.gameObject);
        }
    }

    void ResizeBallGroup()
    {
        m_colliderSize.y = m_ballPrefab.transform.localScale.y * m_mainBallGroup.transform.childCount;
        m_mainBallGroupCollider.size = m_colliderSize;
    }

    public void AddBall()
    {
        // create a ball and make it a child of the Ball-O-Tron
        m_topBall = Instantiate(m_ballPrefab, transform).GetComponent<Rigidbody2D>();
        // position the ball at the spawn point
        m_topBall.transform.position = transform.position + Vector3.up * m_spawnHeight; 
    }

    public void LaunchBall()
    {
        // apply the launch force to the ball holder, based on the current ball count
        //m_ballHolder.AddForce(Vector2.up * (m_minimumLaunchForce + m_launchForcePerBall * transform.childCount));

        // instantiate prefab ball-o-tron ball at position of last child of ball-o-tron
        // instantiate prefab ball-o-tron ball at position of second last child of ball-o-tron
        // resize main ball group collider
        // launch main group slightly, second ball slightly more and top ball a lot
        // delete top ball upon reaching specified height
        // reattach second ball to main group
    }
    public void SetBallCount(int a_ballCount)
    {
        // if there are more balls than there should be
        //if (transform.childCount > a_ballCount)
        //{
        //    // loop for each excess ball
        //    for (int i = transform.childCount - 1; i >= a_ballCount; --i)
        //    {
        //        // destroy the current ball
        //        Destroy(transform.GetChild(i).gameObject);
        //    }
        //}
        //// otherwise, if there is less than or equal to the desired ball count
        //else
        //{
        //    // loop for each ball to be created, skipping balls that already exist
        //    for (int i = transform.childCount; i < a_ballCount; ++i)
        //    {
        //        // create a ball and make it a child of the Ball-O-Tron
        //        GameObject ball = Instantiate(m_ballPrefab, transform) as GameObject;
        //        // position the ball above the ball holder and any previously created balls
        //        ball.transform.position = m_ballHolder.transform.position + Vector3.up * (m_ballHolder.transform.localScale.y * 0.5f + (i + 0.5f) * m_ballPrefab.transform.localScale.y);
        //    }
        //}
    }

    // Start is called before the first frame update
    void Start()
    {
        m_mainBallGroupCollider = m_mainBallGroup.GetComponent<BoxCollider2D>();
        m_colliderSize = m_mainBallGroupCollider.size;
    }

    // Update is called once per frame
    void Update()
    {
        // TEMP - add ball
        // if the ball's has effectively stopped
        if (m_topBall != null && m_topBall.velocity.sqrMagnitude < m_lowestAllowedVelocitySquared)
        {
            m_topBall = null;
            print("landed");
        }
        //

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
        if (Input.GetKeyDown(KeyCode.J))
        {
            ResizeBallGroup();
        }
    }
}
*/