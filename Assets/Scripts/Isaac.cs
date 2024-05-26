using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/*
    File name: Isaac.cs
    Summary: Manages the Player's ability to control Isaac's movement, shooting and bomb placement, as well as managing its limited duration
    Creation Date: 20/05/2024
    Last Modified: 27/05/2024
*/
public class Isaac : MonoBehaviour
{
	[HideInInspector] public PlayerControls m_playerControls;
    public int m_health = 6;
	public float m_timeBeforeHealthLoss = 1.0f;
	public float m_moveSpeed = 5.0f;
	public GameObject m_isaacBombPrefab;
	public int m_bombCount = 1;
	public GameObject m_isaacTearPrefab;
	public float m_fireRate = 0.25f;
	Vector3 m_displacement = Vector3.zero;
	float m_healthTimer = 0.0f;
	float m_fireRateTimer = 0.0f;
	
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
		// increase the health timer
		m_healthTimer += Time.unscaledDeltaTime;
		
		// if enough time has passed to reduce health
		if (m_healthTimer >= m_timeBeforeHealthLoss)
		{
			// reset the health timer
			m_healthTimer -= m_timeBeforeHealthLoss;
			// reduce Isaac's health
			--m_health;
			
			// if Isaac's health has reached 0
			if (m_health == 0)
			{
				// have player controls destroy Isaac
				m_playerControls.RemoveBall();
			}
		}
		
		// reset the displacement vector
		m_displacement = Vector2.zero;
		
		// determine the displacement of Isaac this frame
        if (Input.GetAxis("Move Isaac Vertical") > 0)
		{
			m_displacement += Vector3.up * m_moveSpeed;
		}
		else if (Input.GetAxis("Move Isaac Vertical") < 0)
		{
			m_displacement -= Vector3.up * m_moveSpeed;
		}

		if (Input.GetAxis("Move Isaac Horizontal") < 0)
		{
			m_displacement -= Vector3.right * m_moveSpeed;
		}
		else if (Input.GetAxis("Move Isaac Horizontal") > 0)
		{
			m_displacement += Vector3.right * m_moveSpeed;
		}

		// apply the displacement to Isaac
		transform.position += m_displacement * Time.unscaledDeltaTime;
		
		// increase the fire rate timer
		m_fireRateTimer += Time.unscaledDeltaTime;
		
		// if enough time has passed for Isaac to shoot a tear
		if (m_fireRateTimer >= m_fireRate)
		{
			// if Isaac should shoot a tear up
			if (Input.GetAxis("Shoot Isaac's Tears Vertical") > 0)
			{
				// create a copy of the tear prefab
				GameObject tear = Instantiate(m_isaacTearPrefab);
				// reset the fire rate timer
				m_fireRateTimer = 0.0f;
			}
			// otherwise, if Isaac should shoot a tear down
			else if (Input.GetAxis("Shoot Isaac's Tears Vertical") < 0)
			{
				// create a copy of the tear prefab
				GameObject tear = Instantiate(m_isaacTearPrefab);
				// reset the fire rate timer
				m_fireRateTimer = 0.0f;
			}
			// otherwise, if Isaac should shoot a tear left
			else if (Input.GetAxis("Shoot Isaac's Tears Horizontal") < 0)
			{
				// create a copy of the tear prefab
				GameObject tear = Instantiate(m_isaacTearPrefab);
				// reset the fire rate timer
				m_fireRateTimer = 0.0f;
			}
			// otherwise, if Isaac should shoot a tear right
			else if (Input.GetAxis("Shoot Isaac's Tears Horizontal") > 0)
			{
				// create a copy of the tear prefab
				GameObject tear = Instantiate(m_isaacTearPrefab);
				// reset the fire rate timer
				m_fireRateTimer = 0.0f;
			}
		}
		
		// if Isaac has at least 1 bomb and the Place Bomb button was pressed
		if (m_bombCount > 0 && Input.GetButtonDown("Place Isaac's Bomb"))
		{
			// create a copy of the IsaacBomb prefab
			GameObject bomb = Instantiate(m_isaacBombPrefab);
			// position the bomb on Isaac
			bomb.transform.position = transform.position;
			// reduce the bomb count by 1
			--m_bombCount;
		}
    }
}
