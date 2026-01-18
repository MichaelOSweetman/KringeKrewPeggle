using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
    File name: BallOTron.cs
    Summary: Manages the balls within the Ball-O-Tron UI display
    Creation Date: 19/01/2026
    Last Modified: 19/01/2026
*/
public class BallOTron : MonoBehaviour
{
    Stack<Rigidbody2D> m_balls;
    public GameObject m_ballPrefab;
    public float m_spawnHeight;

    public void AddBall()
    {
        // create a ball, make it a child of the Ball-O-Tron, get its Rigidbody component and add it to the ball stack
        m_balls.Push((Instantiate(m_ballPrefab, transform) as GameObject).GetComponent<Rigidbody2D>());
        // position the ball at the spawn point
        m_balls.Peek().transform.position = Vector3.up * m_spawnHeight; 
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
        
    }
}
