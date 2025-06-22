using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/*
        File name: SashaPower.cs
        Summary: Manages the power gained from the green peg when playing as Sasha
        Creation Date: 01/06/2025
        Last Modified: 23/06/2025
*/
public class SashaPower : GreenPegPower
{
    public GameObject m_UIArrowPrefab;
    public Texture m_inactiveArrowTexture;
    RawImage m_UIArrow;
    Texture m_activeArrowTexture;
    public float m_beatDelay = 0.5f;
    public float m_gracePeriod = 0.25f;
    public float m_moveDistance = 1.0f;
    public float m_moveSpeed = 6.0f;
    Transform m_pegContainer;
    Vector3 m_containerDefaultPosition = Vector3.zero;
    Vector3 m_direction = Vector3.zero;
    bool m_atDefaultPosition = true;
    float m_timer = 0.0f;

    // TEMP
    AudioSource m_audioSource;
    float m_musicTimer = 0.0f;
    bool m_music = false;
    bool m_up = true;

    public void TEMPMUSIC()
    {
        m_musicTimer += Time.unscaledDeltaTime;
        if (m_up && m_musicTimer > m_beatDelay)
        {
            m_audioSource.pitch = 0.25f;
            m_audioSource.Play();
            m_musicTimer -= m_beatDelay;
            m_up = false;
        }
        else if (!m_up && m_musicTimer > m_beatDelay * 0.5f)
        {
            m_audioSource.pitch = 0.5f;
            m_audioSource.Play();
            m_up = true;
        }
    }

    public override void Initialize()
    {
        // TEMP
        m_audioSource = m_playerControls.m_UIManager.m_musicAudioSource;

        // create the ui arrow and set its parent to be the parent of the power charges text so they are on the canvas
        m_UIArrow = Instantiate(m_UIArrowPrefab, m_powerChargesText.rectTransform.parent).GetComponent<RawImage>();
        // store the arrow's current texture as the active texture
        m_activeArrowTexture = m_UIArrow.texture;
        // hide the UI Arrow
        m_UIArrow.gameObject.SetActive(false);

        // get from the pegmanager gameobject's transform
        m_pegContainer = m_playerControls.m_pegManager.transform;
        // store its current position as the default position
        m_pegContainer.transform.position = m_containerDefaultPosition;
    }

    public override void Trigger(Vector3 a_greenPegPosition)
    {
        // add the charges
        ModifyPowerCharges(m_gainedPowerCharges);

        // show the UI Arrow
        m_UIArrow.gameObject.SetActive(true);

        // play music
        m_music = true;
    }

    public override bool OnShoot()
    {
        // play music
        m_music = true;

        // return that this function should not override the default shoot function
        return false;
    }

    public override void ResolveTurn()
    {
        // if there are power charges
        if (m_powerCharges > 0)
        {
            // remove one
            ModifyPowerCharges(-1);

            // if there are now zero power charges
            if (m_powerCharges == 0)
            {
                // hide the UI arrow
                m_UIArrow.gameObject.SetActive(false);
            }

        }

        // stop the music
        //TEMP m_music = false;

        // ensure the pegs are at their default position
        m_pegContainer.position = m_containerDefaultPosition;
        m_atDefaultPosition = true;

        // temp
        m_UIArrow.color = Color.white;
    }

    public override void Reload()
    {
        // reset the power charges
        ResetPowerCharges();
        // ensure the peg container is at the default position
        m_pegContainer.transform.position = m_containerDefaultPosition;
        m_atDefaultPosition = true;

        // stop the music
        //TEMP m_music = false;

        // temp
        m_UIArrow.color = Color.white;
    }

    public override void Update()
    {
        // TEMP
        if (m_music)
        {
            TEMPMUSIC();
        }

        // if the ball is in play and there are power charges
        if (true)//m_playerControls.m_ballInPlay && m_powerCharges > 0)
        {
            // increase the timer
            m_timer += Time.unscaledDeltaTime;

            // temp
            m_UIArrow.texture = m_activeArrowTexture;

            // determine the direction the pegs should move
            if (Input.GetAxis("Use Power Vertical Primary") > 0 || Input.GetAxis("Use Power Vertical Secondary") > 0)
            {
                m_direction = Vector3.up;
            }
            else if (Input.GetAxis("Use Power Vertical Primary") < 0 || Input.GetAxis("Use Power Vertical Secondary") < 0)
            {
                m_direction = Vector3.down;
            }
            else if (Input.GetAxis("Use Power Horizontal Primary") < 0 || Input.GetAxis("Use Power Horizontal Secondary") < 0)
            {
                m_direction = Vector3.left;
            }
            else if (Input.GetAxis("Use Power Horizontal Primary") > 0 || Input.GetAxis("Use Power Horizontal Secondary") > 0)
            {
                m_direction = Vector3.right;
            }
            else
            {
                m_direction = Vector3.zero;
                m_UIArrow.texture = m_inactiveArrowTexture;
            }

            // temp
            m_UIArrow.transform.up = m_direction;

            // if the pegs aren't at the default position and the 'up beat' has just occured in the song
            if (!m_atDefaultPosition && m_timer > m_beatDelay * 0.5f)
            {
                // return the pegs to their default position
                m_pegContainer.position = m_containerDefaultPosition;
                m_atDefaultPosition = true;

                // temp
                m_UIArrow.color = Color.blue;
            }
            // if the pegs are at the default position and the 'down beat' has just occured in the song
            else if (m_atDefaultPosition && m_timer > m_beatDelay && m_timer < m_beatDelay + m_gracePeriod)
            {
                // temp
                m_UIArrow.color = Color.yellow;

                // lerp the pegs from the default position to a point in the direction and a distance away as specified by m_direction and m_moveDistance, at a speed determined by m_moveSpeed
                m_pegContainer.position = Vector3.Lerp(m_containerDefaultPosition, m_containerDefaultPosition + m_direction * m_moveDistance, (m_timer - m_beatDelay) * m_moveSpeed);

                // if the time for the lerp has fully elapsed
                if ((m_timer - m_beatDelay) * m_moveSpeed >= 1.0f)
                {
                    // set the the pegs to the end position
                    m_pegContainer.position += m_direction * m_moveDistance;
                    // store that the pegs have moved
                    m_atDefaultPosition = false;
                    // reset the timer
                    m_timer -= m_beatDelay;
                }

            }
            // if the down beat was missed
            else if (m_timer > m_beatDelay + m_gracePeriod)
            {
                // reset the timer so it is in time for the next beat
                m_timer -= m_beatDelay;

                // temp
                m_UIArrow.color = Color.white;
            }
        }
    }
}
