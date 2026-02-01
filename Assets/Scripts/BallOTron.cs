using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
    File name: BallOTron.cs
    Summary: Manages the balls within the Ball-O-Tron UI display
    Creation Date: 19/01/2026
    Last Modified: 02/02/2026
*/
public class BallOTron : MonoBehaviour
{
    public GameObject m_ballPrefab;
    public Rigidbody2D m_ballHolder;
    public float m_spawnHeight = 4.5f;
    public float m_launchForce = 1000.0f;

    private void OnTriggerEnter2D(Collider2D a_collision)
    {
        // if the object that entered this trigger is a sibling of this object
        if (a_collision.transform.parent == transform.parent)
        {
            // destroy the collision object
            Destroy(a_collision.gameObject);
        }
    }

    public void AddBall()
    {
        // create a ball and make it a child of the Ball-O-Tron
        GameObject ball = Instantiate(m_ballPrefab, transform.parent) as GameObject;
        // position the ball at the spawn point
        ball.transform.position = transform.position + Vector3.up * m_spawnHeight; 
    }

    public void LaunchBall()
    {
        // apply the launch force to the ball holder
        m_ballHolder.AddForce(Vector2.up * m_launchForce);
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
    }
}
