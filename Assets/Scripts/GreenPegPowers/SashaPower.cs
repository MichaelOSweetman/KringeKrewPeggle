using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/*
    File name: SashaPower.cs
    Summary: Manages the power gained from the green peg when playing as Sasha
    Creation Date: 01/06/2025
    Last Modified: 30/06/2025
*/
public class SashaPower : GreenPegPower
{
    public GameObject m_UIArrowPrefab;
    public Texture m_activeArrowTexture;
    public float m_beatDelay = 0.5f;
    public float m_gracePeriod = 0.25f;
    public float m_moveDistance = 1.0f;
    public float m_moveSpeed = 6.0f;
    RawImage m_UIArrow;
    Texture m_inactiveArrowTexture;
    Transform m_pegContainer;
    bool m_atDefaultPosition = true;
    bool m_lerp = false;
    Vector3 m_containerDefaultPosition = Vector3.zero;
    Vector3 m_direction = Vector3.zero;
    Vector3 m_startPosition = Vector3.zero;
    Vector3 m_lerpTarget = Vector3.zero;
    float m_timer = 0.0f;
    float m_lerpTimer = 0.0f;

    // TEMP
    AudioSource m_audioSource;
    bool m_up = true;

    public override void Initialize()
    {
        // TEMP
        m_audioSource = m_playerControls.m_UIManager.m_musicAudioSource;

        // create the ui arrow and set its parent to be the parent of the power charges text so they are on the canvas
        m_UIArrow = Instantiate(m_UIArrowPrefab, m_powerChargesText.rectTransform.parent).GetComponent<RawImage>();
        // store the arrow's current texture as the active texture
        m_inactiveArrowTexture = m_UIArrow.texture;
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
    }

    public override bool OnShoot()
    {
        // play music

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

        // temp
        m_UIArrow.color = Color.white;
    }

    public override void Update()
    {
        // if the pegs should be lerping
        if (m_lerp)
        {
            // increase the lerp timer
            m_lerpTimer += Time.unscaledDeltaTime * m_moveSpeed;

            // if the lerp timer has surpassed 1, the lerp is complete
            if (m_lerpTimer >= 1.0f)
            {
                // set the peg container to its destination position
                m_pegContainer.position = m_lerpTarget;

                // store that the lerp is complete
                m_lerp = false;
            }
            // if the lerp is ongoing
            else
            {
                // lerp the pegs from the default position to a point in the direction and a distance away as specified by m_direction and m_moveDistance, at a speed determined by m_moveSpeed
                m_pegContainer.position = Vector3.Lerp(m_startPosition, m_lerpTarget, m_lerpTimer);
            }

        }

        // if the ball is in play and there are power charges
        if (true)//m_playerControls.m_ballInPlay && m_powerCharges > 0)
        {
            // increase the timer
            m_timer += Time.unscaledDeltaTime;

            // set the UI arrow texture to its active texture
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
                // set the UI arrow texture to be the inactive texture if no input is detected
                m_UIArrow.texture = m_inactiveArrowTexture;
            }

            // rotate the UI arrow based on the input direction
            m_UIArrow.transform.up = m_direction;


            // if the 'down beat' has just occured in the song
            if (m_timer > m_beatDelay)
            {
                // if the pegs are not already moving and the UI arrow has its active texture, meaning a direction has been supplied and the pegs should move
                if (!m_lerp && m_UIArrow.texture == m_activeArrowTexture)
                {
                    // have the pegs lerp in the specified direction
                    m_lerp = true;
                    m_lerpTimer = 0.0f;
                    m_startPosition = m_pegContainer.position;
                    m_lerpTarget = m_containerDefaultPosition + m_direction * m_moveDistance;

                    // store that the pegs are not at their default position
                    m_atDefaultPosition = false;
                }

                // reset the timer
                m_timer -= m_beatDelay;

                // temp
                m_UIArrow.color = Color.yellow;
                m_audioSource.pitch = 0.25f;
                m_audioSource.Play();
                m_up = true;
            }
            // if the 'up beat' has just occured in the song
            else if (m_timer > m_beatDelay * 0.5f)
            {
                // if the pegs are not already moving and the pegs are not at their default position
                if (!m_lerp && !m_atDefaultPosition)
                {
                    // have the pegs return to their default position
                    m_lerp = true;
                    m_lerpTimer = 0.0f;
                    m_startPosition = m_pegContainer.position;
                    m_lerpTarget = m_containerDefaultPosition;

                    // store that the pegs are at their default position
                    m_atDefaultPosition = true;
                }

                // temp
                m_UIArrow.color = Color.blue;
                if (m_up)
                {
                    m_audioSource.pitch = 0.5f;
                    m_audioSource.Play();
                    m_up = false;
                }
            }
        }
    }
}
