using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
    File name: Mateja.cs
    Summary: Launches the ball back up and sends it back down the first time it falls to the kill floor
    Creation Date: 25/12/2023
    Last Modified: 31/12/2023
*/
public class Mateja : MonoBehaviour
{
    enum MatejaState
    {
        Spawned,
        Ready,
        Launched,
        Slammed
    }


    [HideInInspector] public PlayerControls m_playerControls;
    [HideInInspector] public GameObject m_bucket;
    [HideInInspector] public GameObject m_victoryBuckets;
    public Vector3 m_spawnPosition = Vector3.zero;
    public Vector3 m_readiedPosition = Vector3.zero;
    public float m_maxValidSquaredDistanceFromReadiedPosition = 0.05f;
    public float m_moveToReadySpeed = 2.0f;
    public float m_launchForce = 5.0f;
    public float m_slamSpeed = 5.0f;
    public float m_rotationSpeed = 5.0f;
    MatejaState m_matejaState = MatejaState.Spawned;
    float m_gravityScale = 0.0f;
    Rigidbody2D m_rigidbody;
    GameObject m_matejaBody;
    Collider2D m_collider;
    GameObject m_heldBall;

    public void JiuJitsuBall(GameObject a_ball)
    {
        // set Mateja's position to the be the ball's
        transform.position = a_ball.transform.position;
        // turn the collider on
        m_collider.enabled = true;
        // turn the held ball on
        m_heldBall.SetActive(true);
        // reset the gravity scale
        m_rigidbody.gravityScale = m_gravityScale;
        // launch Mateja in the direction the ball came from
        m_rigidbody.AddForce(a_ball.GetComponent<Rigidbody2D>().velocity.normalized * -m_launchForce, ForceMode2D.Impulse);
        // store that Mateja has been launched
        m_matejaState = MatejaState.Launched;
    }

    // Start is called before the first frame update
    void Start()
    {
        // move to the spawn position
        transform.position = m_spawnPosition;

        // get the rigidbody
        m_rigidbody = GetComponent<Rigidbody2D>();

        // get the collider and turn it off
        m_collider = GetComponent<Collider2D>();
        m_collider.enabled = false;

        // store the gravity scale
        m_gravityScale = m_rigidbody.gravityScale;

        // set the gravity scale to 0
        m_rigidbody.gravityScale = 0.0f;

        // get the mateja body
        m_matejaBody = transform.GetChild(0).gameObject;

        // get the first child of Mateja's body as the held ball and turn it off
        m_heldBall = m_matejaBody.transform.GetChild(0).gameObject;
        m_heldBall.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        // if Mateja has not yet reached the readied position
        if (m_matejaState == MatejaState.Spawned)
        {
            // move Mateja towards the ready position using the move to ready speed
            transform.position = Vector3.MoveTowards(transform.position, m_readiedPosition, m_moveToReadySpeed * Time.deltaTime * m_playerControls.m_timeScale);

            // if Mateja is close enough to the readied position
            if ((transform.position - m_readiedPosition).sqrMagnitude <= m_maxValidSquaredDistanceFromReadiedPosition)
            {
                // set his position to the readied position
                transform.position = m_readiedPosition;
                // store that Mateja is ready
                m_matejaState = MatejaState.Ready;
            }
        }
        // otherwise, if Mateja has been launched
        else if (m_matejaState == MatejaState.Launched)
        {
            // if Mateja has started going down
            if (m_rigidbody.velocity.y <= 0)
            {
                // set the gravity scale to 0
                m_rigidbody.gravityScale = 0.0f;
                // store that Mateja has been slammed
                m_matejaState = MatejaState.Slammed;
            }
            else
            {
                // rotate Mateja's body as he launches
                m_matejaBody.transform.Rotate(Vector3.forward * m_rotationSpeed * Time.deltaTime * m_playerControls.m_timeScale);
            }
        }
        // otherwise, if Mateja has been slammed
        else if (m_matejaState == MatejaState.Slammed)
        {
            // move Mateja towards the bucket
            transform.position = Vector3.MoveTowards(transform.position, m_bucket.transform.position, m_slamSpeed * Time.deltaTime * m_playerControls.m_timeScale);
        }
    }

    private void OnTriggerEnter2D(Collider2D a_collision)
    {
        // if Mateja has slammed into the bucket
        if (m_matejaState == MatejaState.Slammed && a_collision.transform.parent == m_bucket.transform)
        {
            // give the player a free ball
            m_playerControls.FreeBall();
            // resolve the turn
            m_playerControls.ResolveTurn();
            // destroy Mateja
            Destroy(this.gameObject);
        }
        // otherwise, if the hit collider is a peg
        else if (a_collision.GetComponent<Peg>() != null)
        {
            // tell the peg that it has been hit
            a_collision.GetComponent<Peg>().Hit();
            //TEMP don't collide with the peg
        }
    }
}

