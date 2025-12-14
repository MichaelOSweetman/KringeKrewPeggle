using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/*
    File name: SashaPower.cs
    Summary: Manages the power gained from the green peg when playing as Sasha
    Creation Date: 01/06/2025
    Last Modified: 15/12/2025
*/
public class SashaPower : GreenPegPower
{
    public GameObject m_UIArrowPrefab;
    public Texture m_activeArrowTexture;
    public float m_beatDelay = 0.5f;
    public float m_gracePeriod = 0.25f;
    public float m_moveDistance = 1.0f;
    public float m_moveSpeed = 6.0f;
    public Vector2 m_modifiedGravity = Vector2.zero;
    public Color m_downBeatColor;
    public Color m_upBeatColor;
    Color m_defaultArrowColor;
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
    Vector2 m_defaultGravity = Vector2.zero;

    MusicManager m_musicManager;
    public AudioClip m_sashaPowerMusic;
    public float m_fadeDuration = 0.75f;

    public override void Initialize()
    {
        // create the ui arrow and set its parent to be the parent of the power charges text so they are on the canvas
        m_UIArrow = Instantiate(m_UIArrowPrefab, m_powerChargesText.rectTransform.parent).GetComponent<RawImage>();
        // store the arrow's current texture as the active texture
        m_inactiveArrowTexture = m_UIArrow.texture;
        // store the arrow's current colour as the default colour
        m_defaultArrowColor = m_UIArrow.color;
        // hide the UI Arrow
        m_UIArrow.gameObject.SetActive(false);

        // get the current level's peg container
        m_pegContainer = m_playerControls.m_pegManager.m_currentPegContainer;
        // store its current position as the default position
        m_pegContainer.transform.position = m_containerDefaultPosition;

        // get the music manager through the peg manager
        m_musicManager = m_playerControls.m_UIManager.m_musicManager;

        // store the current gravity as the default gravity
        m_defaultGravity = Physics2D.gravity;
    }

    public override void Trigger(Vector3 a_greenPegPosition)
    {
        // if there are 0 power charges and the power was not about to be resolved (and therefore already active)
        if (m_powerCharges == 0 && !m_playerControls.m_resolvePowerNextTurn)
        {
            // show the UI Arrow
            m_UIArrow.gameObject.SetActive(true);

            // have the music manager play the power music
            m_musicManager.PlayNow(true, m_sashaPowerMusic);

            // initialise the timer
            m_timer = 0.0f;

            // modify the gravity
            Physics2D.gravity = m_modifiedGravity;
        }

        // add the charges
        ModifyPowerCharges(m_gainedPowerCharges);
    }

    public override bool OnShoot()
    {
        // if there are power charges
        if (m_powerCharges > 0)
        {
            // have the music manager play the power music
            m_musicManager.PlayNow(true, m_sashaPowerMusic);

            // initialise the timer
            m_timer = 0.0f;
        }

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

                // return the gravity to its default state
                Physics2D.gravity = m_defaultGravity;
            }


            // have the music manager fade back to the playlist
            m_musicManager.FadeToPlaylist(m_fadeDuration);
        }

        // ensure the pegs are at their default position
        m_pegContainer.position = m_containerDefaultPosition;
        m_atDefaultPosition = true;

        // reset the arrow colour
        m_UIArrow.color = m_defaultArrowColor;
    }

    public override void Reload()
    {
        // reset the power charges
        ResetPowerCharges();
        // ensure the peg container is at the default position
        m_pegContainer.transform.position = m_containerDefaultPosition;
        m_atDefaultPosition = true;

        // have the music manager stop the power music
        m_musicManager.SwitchToPlaylist();

        // reset the arrow colour
        m_UIArrow.color = m_defaultArrowColor;

        // set the gravity to its default state
        Physics2D.gravity = m_defaultGravity;
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

        // reset the UI arrow texture to its active texture
        m_UIArrow.texture = m_activeArrowTexture;

        // determine the direction UI arrow should face
        if (Input.GetButton("Use Power Up Primary") || Input.GetButton("Use Power Up Secondary"))
        {
            m_UIArrow.transform.up = Vector3.up;
        }
        else if (Input.GetButton("Use Power Down Primary") || Input.GetButton("Use Power Down Secondary"))
        {
            m_UIArrow.transform.up = Vector3.down;
        }
        else if (Input.GetButton("Use Power Left Primary") || Input.GetButton("Use Power Left Secondary"))
        {
            m_UIArrow.transform.up = Vector3.left;
        }
        else if (Input.GetButton("Use Power Right Primary") || Input.GetButton("Use Power Right Secondary"))
        {
            m_UIArrow.transform.up = Vector3.right;
        }
        else
        {
            // set the UI arrow texture to be the inactive texture if no input is detected
            m_UIArrow.texture = m_inactiveArrowTexture;
        }

        // if the ball is in play and there are power charges
        if (m_playerControls.m_ballInPlay && m_powerCharges > 0)
        {
            // increase the timer
            m_timer += Time.unscaledDeltaTime;

            // determine the direction the pegs should move
            if (Input.GetButtonDown("Use Power Up Primary") || Input.GetButtonDown("Use Power Up Secondary"))
            {
                m_direction = Vector3.up;
            }
            else if (Input.GetButtonDown("Use Power Down Primary") || Input.GetButtonDown("Use Power Down Secondary"))
            {
                m_direction = Vector3.down;
            }
            else if (Input.GetButtonDown("Use Power Left Primary") || Input.GetButtonDown("Use Power Left Secondary"))
            {
                m_direction = Vector3.left;
            }
            else if (Input.GetButtonDown("Use Power Right Primary") || Input.GetButtonDown("Use Power Right Secondary"))
            {
                m_direction = Vector3.right;
            }
            else
            {
                m_direction = Vector3.zero;
            }

            // if the 'down beat' has just occured in the song, accounting for the grace period
            if (m_timer >= m_beatDelay - (m_gracePeriod * 0.5f) || m_timer <= m_gracePeriod * 0.5f)
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

                // if the timer has surpassed the beat delay
                if (m_timer > m_beatDelay)
                {
                    // reset the timer
                    m_timer -= m_beatDelay;
                    // set the UI arrow to its downbeat colour
                    m_UIArrow.color = m_downBeatColor;
                }
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

                // set the UI arrow to its up beat colour
                m_UIArrow.color = m_upBeatColor;
            }
        }
    }
}
