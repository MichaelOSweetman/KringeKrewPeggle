using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
    File name: BallOTron.cs
    Summary: Manages the balls within the Ball-O-Tron UI display
    Creation Date: 19/01/2026
    Last Modified: 01/06/2026
*/
public class BallOTron : MonoBehaviour
{
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

/*
Need to fake the physics as Unity isn't accurate enough 
 
balls are connected as one cohesive unit

on launch
top two balls are disconnected
highest launches to top
second most does slight bounce then reconnects



this shouldn't be attached to a collider for deleting - no collider need exist, topball should be destroyed when high up enough
*/