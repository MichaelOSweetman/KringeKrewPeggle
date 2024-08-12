using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
    File name: CenterEyes.cs
    Summary: Replaces the images of the player character's eyes with one where the pupils are centered when the cursor is on the launcher
    Creation Date: 29/07/2024
    Last Modified: 12/08/2024
*/
public class CenterEyes : MonoBehaviour
{
    public Sprite m_rotatingEye;
    public Sprite m_centeredEye;
    public SpriteRenderer m_leftEye;
    public SpriteRenderer m_rightEye;
    RotateToBall m_leftEyeRotator;
    RotateToBall m_rightEyeRotator;

    private void OnMouseEnter()
    {
		// if the eyes are tracking the cursor as opposed to the ball
		if (!m_leftEyeRotator.m_targetingBall)
        {
			// replace the sprite of the eyes with the centered eye sprite
			m_leftEye.sprite = m_centeredEye;
			m_rightEye.sprite = m_centeredEye;
	
			//  turn off the eye rotator scripts
			m_leftEyeRotator.enabled = false;
			m_rightEyeRotator.enabled = false;
		}
    }

    private void OnMouseExit()
    {

        // replace the sprite of the eyes with the rotating eye sprite
        m_leftEye.sprite = m_rotatingEye;
        m_rightEye.sprite = m_rotatingEye;

        // turn on the eye rotator scripts
        m_leftEyeRotator.enabled = true;
        m_rightEyeRotator.enabled = true;
    }

    // Start is called before the first frame update
    void Start()
    {
        // get the RotateToBall components from the eyes
        m_leftEyeRotator = m_leftEye.GetComponent<RotateToBall>();
        m_rightEyeRotator = m_rightEye.GetComponent<RotateToBall>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
