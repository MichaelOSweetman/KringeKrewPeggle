using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/*
    File name: PlayerControls.cs
    Summary: Manages the player's ability to shoot the ball and speed up time, as well as to make use of the different powers
    Creation Date: 01/10/2023
    Last Modified: 30/09/2024
*/
public class PlayerControls : MonoBehaviour
{
    public enum GameState
    {
        TurnSetUp,
        Shoot,
        BallInPlay,
        ResolveTurn,
    }

    public enum PowerFunctionMode
    {
        Trigger,
        SetUp,
        OnShoot,
        Resolve,
		Reload
    }


    [HideInInspector] public GameState m_currentGameState = GameState.Shoot;

    [Header("Other Scripts")]
    public UIManager m_UIManager;
    public PegManager m_pegManager;
    public CameraZoom m_cameraZoom;
    public LauncherRotation m_LauncherLookControls;

    [Header("UI")]
    public Text m_ballCountText;
    public float m_freeBallTextDuration = 2.0f;
    float m_freeBallTextTimer = 0.0f;

    [Header("Ball")]
    public GameObject m_ballPrefab;
    public float m_ballLaunchSpeed;
    public byte m_startingBallCount = 10;
    public float m_ballKillFloor = -7.0f;
    [HideInInspector] public GameObject m_ball = null;
    int m_ballCount = 0;

    [Header("Green Pegs")]
    public Text m_PowerChargesText;
    public delegate void GreenPegPower(PowerFunctionMode a_powerFunctionMode, Vector3 a_greenPegPosition);
    public GreenPegPower m_greenPegPower;
    [HideInInspector] public int m_powerCharges = 0;
    [HideInInspector] public bool m_setUpPowerNextTurn = false;
    bool m_resolvePowerNextTurn = false;

    [Header("Ben Power")]
    public int m_benPowerChargesGained = 1;
	public GameObject m_IsaacPrefab;
	bool m_spawnIsaac = false;

    [Header("Daniel Power")]
    public GameObject m_wasp;
    public float m_searchRadius = 5.0f;
	[HideInInspector] public List<GameObject> m_wasps;

    [Header("Ethen Power")]
    public int m_ethenPowerChargesGained = 2;
    public GameObject m_lines;
    public GameObject m_linePrefab;
    public GameObject m_endDrawButton;
    public GameObject m_clearButton;
    public GameObject m_inkResourceBarBackground;
    public float m_maxInk = 500.0f;
    public float m_minValidSquareMouseMovement = 0.05f;
    [HideInInspector] public float m_ink = 0.0f;
    [HideInInspector] public bool m_drawing = false;
    bool m_lineBegun = false;
    LineRenderer m_currentLineRenderer;
    RectTransform m_inkResourceBar;
    float m_inkResourceBarMaxWidth = 0.0f;
    Vector3 m_previousMousePosition = Vector3.zero;
	
	[Header("Jack Power")]
	public int m_jackPowerChargesGained = 4;
	int m_communistPegScore = 0;
	int m_defaultBluePegScore = 0;
    int m_defaultOrangePegScore = 0;
    int m_defaultPurplePegScore = 0;
    int m_defaultGreenPegScore = 0;
	
	[Header("Kevin Power")]
    public int m_kevinPowerChargesGained = 2;
    public GameObject m_scopeOverlay;
    public float m_forceToBall = 2000.0f;
    public float m_scopedTimeScale = 0.3f;
	
    [Header("Loki Power")]
    public int m_lokiPowerChargesGained = 2;
    public LineRenderer m_lokiPowerCord;
    public float m_maxCordLength = 5.0f;
    public GameObject m_hook;
    public float m_hookLaunchSpeed = 30.0f;
    public float m_pullSpeed = 7.5f;
    GameObject m_connectionPoint;
    bool m_connectedToPeg = false;
	
	[Header("Mateja Power")]
    public GameObject m_matejaPrefab;
    GameObject m_mateja = null;

	[Header("Phoebe Power")]
	public int m_phoebePowerChargesGained = 3;
	public GameObject m_bocconciniPrefab;
    List<Bocconcini> m_bocconcinis;

    [Header("Sweets Power")]
    public int m_sweetsPowerChargesGained = 3;
    public GameObject m_launcher;
    public GameObject m_topPlayAreaCollider;
    public GameObject m_gameOverlay;
    public GameObject m_hillsideGameOverlay;

    [Header("Buckets")]
    public MoveToPoints m_bucket;
    public GameObject m_victoryBuckets;

    [Header("Time Scale")]
    public float m_spedUpTimeScale = 5.0f;
    public float m_nearVictoryTimeScale = 0.25f;
    [HideInInspector] public float m_defaultTimeScale = 1.0f;
    [HideInInspector] float m_defaultDeltaTime = 0.02f;

    [Header("Audio")]
    public AudioSource m_audioSource;
    public AudioClip[] m_freeBallSounds;

    void BenPower(PowerFunctionMode a_powerFunctionMode, Vector3 a_greenPegPosition)
    {
		// if the green peg has been triggered
		if (a_powerFunctionMode == PowerFunctionMode.Trigger)
		{
            // add the charges
            ModifyPowerCharges(m_benPowerChargesGained);
            // spawn isaac instead of the ball next turn
            m_spawnIsaac = true;
        }
        // otherwise, if isaac was just spawned
        else if (a_powerFunctionMode == PowerFunctionMode.OnShoot)
        {
            // reduce the power charges by 1
            ModifyPowerCharges(-1);
            // disable the bucket
            m_bucket.gameObject.SetActive(false);
            // if there are now 0 charges
            if (m_powerCharges == 0)
            {
                // have the power resolve at the start of next turn
                m_resolvePowerNextTurn = true;
            }
        }
        // otherwise, if the power should be reloaded, or if the power should resolved as the player has expended all power charges
		else if (a_powerFunctionMode == PowerFunctionMode.Reload || (a_powerFunctionMode == PowerFunctionMode.Resolve && m_powerCharges == 0))
		{
			// ensure the ball is spawned instead of Isaac next shoot phase
			m_spawnIsaac = false;
            m_bucket.gameObject.SetActive(true);
		}
		
        
        // TEMP
        print("BenPower() called");
    }

    void DanielPower(PowerFunctionMode a_powerFunctionMode, Vector3 a_greenPegPosition)
    {
        // if the green peg has been triggered
        if (a_powerFunctionMode == PowerFunctionMode.Trigger)
        {
            // get an array of all colliders around the green peg
            Collider2D[] CollidersInRange = Physics2D.OverlapCircleAll(new Vector2(a_greenPegPosition.x, a_greenPegPosition.y), m_searchRadius);

            // loop for each collider
            for (int i = 0; i < CollidersInRange.Length; ++i)
            {
                Peg peg = CollidersInRange[i].GetComponent<Peg>();
                // if the collider has the peg component and hasn't been hit
                if (peg && !peg.m_hit)
                {
                    // create a wasp
                    GameObject wasp = Instantiate(m_wasp) as GameObject;
					// add the wasp to the wasp list
					m_wasps.Add(wasp);
                    // position it on the green peg
                    wasp.transform.position = a_greenPegPosition;
                    // give the wasp the peg as its target
                    wasp.GetComponent<Wasp>().m_targetPeg = peg;
                    // give the wasp this gameobject's PlayerControls script so it can access the current time scale
                    wasp.GetComponent<Wasp>().m_playerControls = this;
                }
            }
        }
		// otherwise, if the power should be reloaded
		else if (a_powerFunctionMode == PowerFunctionMode.Reload)
		{
			// destroy all remaining wasps
			while(m_wasps.Count > 0)
			{
				Destroy(m_wasps[m_wasps.Count - 1]);
				m_wasps.RemoveAt(m_wasps.Count - 1);
			}
		}
			

        // TEMP
        print("DanielPower() called");
    }

    public void EndDrawButtonPressed()
    {
        // turn off the drawing UI elements
        m_endDrawButton.SetActive(false);
        m_clearButton.SetActive(false);
        m_inkResourceBarBackground.SetActive(false);

        // take the player out of drawing mode
        m_drawing = false;

        // turn on the LookAtCursor component of the launcher
        m_LauncherLookControls.enabled = true;

        // reduce the amount of power charges
        ModifyPowerCharges(-1);

        // if there are still power charges
        if (m_powerCharges > 0)
        {
            // have the drawing power get set up next turn
            m_setUpPowerNextTurn = true;
        }
    }

    public void ClearDrawingButtonPressed()
    {
        // reset the amount of ink
        m_ink = m_maxInk;
        UpdateInkResourceBar();

        // destroy all lines
        DestroyLines();

    }

    public void UpdateLine(Vector3 a_newLinePoint)
    {
        // ensure the z component of the new point is 0
        a_newLinePoint.z = 0;

        // add the new position to the current line
        ++m_currentLineRenderer.positionCount;
        m_currentLineRenderer.SetPosition(m_currentLineRenderer.positionCount - 1, a_newLinePoint);
    }

    public void UpdateInkResourceBar()
    {
        // modify the ink resource bar to be representative of the amount of ink remaining relative to the max ink
        m_inkResourceBar.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, Mathf.Clamp01(m_ink / m_maxInk) * m_inkResourceBarMaxWidth);
    }

    public void DestroyLines(bool a_onlyDestroyIfHit = false)
    {
        // loop for each line
        for (int i = m_lines.transform.childCount - 1; i >= 0; --i)
        {
            // if the line should be destroyed regardless or if the line has been hit and the only destroy if hit arguement is true
            if (!a_onlyDestroyIfHit || m_lines.transform.GetChild(i).GetComponent<Line>().m_hit)
            {
                // destroy the current line
                Destroy(m_lines.transform.GetChild(i).gameObject);
            }
        }
    }

    void EthenPower(PowerFunctionMode a_powerFunctionMode, Vector3 a_greenPegPosition)
    {
        // if the green peg has been triggered
        if (a_powerFunctionMode == PowerFunctionMode.Trigger)
        {
            // if there are 0 power charges
            if (m_powerCharges == 0)
            {
                // have the power set up next turn
                m_setUpPowerNextTurn = true;
            }
            // add the charges
            ModifyPowerCharges(m_ethenPowerChargesGained);
        }
        // otherwise, if the power should be set up
        else if (a_powerFunctionMode == PowerFunctionMode.SetUp)
        {
            // set the UI elements required for drawing to be active
            m_endDrawButton.SetActive(true);
            m_clearButton.SetActive(true);
            m_inkResourceBarBackground.SetActive(true);

            // reset the ink meter
            m_ink = m_maxInk;
            UpdateInkResourceBar();

            // turn off the LookAtCursor component of the launcher
            m_LauncherLookControls.enabled = false;

            // put the player in drawing mode
            m_drawing = true;

            // initialise the previous mouse position variable
            m_previousMousePosition = Input.mousePosition;
        }
		// otherwise, if the power should be reloaded
		else if (a_powerFunctionMode == PowerFunctionMode.Reload)
		{
			// Destroy all lines
			DestroyLines();
			
			// turn off the drawing UI elements
			m_endDrawButton.SetActive(false);
			m_clearButton.SetActive(false);
			m_inkResourceBarBackground.SetActive(false);
			
			// take the player out of drawing mode
			m_drawing = false;
			
			// store that a line has not begun being drawn
			m_lineBegun = false;
			
			// ensure the LookAtCursor component of the launcher is on
			m_LauncherLookControls.enabled = true;
		}
		

        // TEMP
        print("EthenPower() called");
    }
	
    void JackPower(PowerFunctionMode a_powerFunctionMode, Vector3 a_greenPegPosition)
    {
		// if the green peg has been triggered
        if (a_powerFunctionMode == PowerFunctionMode.Trigger)
        {
            // if there are 0 power charges
            if (m_powerCharges == 0)
            {
                // have the power set up next turn
                m_setUpPowerNextTurn = true;
            }
            // add the charges
            ModifyPowerCharges(m_jackPowerChargesGained);
        }
		// otherwise, if the green peg power is to be set up
		else if (a_powerFunctionMode == PowerFunctionMode.SetUp)
		{
			// get the average of the scores of all the active pegs
			m_communistPegScore = m_pegManager.GetAverageActivePegScore();
			
			// replace the score gained from these pegs with this average
			m_pegManager.m_baseBluePegScore = m_communistPegScore;
			m_pegManager.m_baseOrangePegScore = m_communistPegScore;
			m_pegManager.m_basePurplePegScore = m_communistPegScore;
			m_pegManager.m_baseGreenPegScore = m_communistPegScore;
		}
		// otherwise, if the player has just shot a ball
        else if (a_powerFunctionMode == PowerFunctionMode.OnShoot)
        {
            // reduce the power charges by 1
            ModifyPowerCharges(-1);
            // if there are now 0 charges
            if (m_powerCharges == 0)
            {
                // have the power resolve at the start of next turn
                m_resolvePowerNextTurn = true;
            }
        }
		// otherwise, if the green peg power should be reloaded or resolved as the player has run out of power charges
		else if (a_powerFunctionMode == PowerFunctionMode.Reload || ( a_powerFunctionMode == PowerFunctionMode.Resolve && m_powerCharges == 0))
		{
			// return the score gained from pegs to their default bases
			m_pegManager.m_baseBluePegScore = m_defaultBluePegScore;
			m_pegManager.m_baseOrangePegScore = m_defaultOrangePegScore;
			m_pegManager.m_basePurplePegScore = m_defaultPurplePegScore;
			m_pegManager.m_baseGreenPegScore = m_defaultGreenPegScore;
		}
		
        // TEMP
        print("JackPower() called");
    }

    void KevinPower(PowerFunctionMode a_powerFunctionMode, Vector3 a_greenPegPosition)
    {
        // if the green peg has been triggered
        if (a_powerFunctionMode == PowerFunctionMode.Trigger)
        {
            // add the charges
            ModifyPowerCharges(m_kevinPowerChargesGained);
        }
		// otherwise, if the power should be reloaded
		else if (a_powerFunctionMode == PowerFunctionMode.Reload)
		{
			// tell the camera to return to its default state instantly
            m_cameraZoom.ReturnToDefault(true);
            // hide the scope overlay
            m_scopeOverlay.SetActive(false);
		}

        // TEMP
        print("KevinPower() called");
    }

    public void ConnectHook(GameObject a_connectionPeg)
    {
        // set the connection point of the cord to be the peg
        m_connectionPoint = a_connectionPeg;
        // store that the end of the cord has been connected to a peg
        m_connectedToPeg = true;
        // make the hook inactive as the peg has replaced it
        m_hook.SetActive(false);
        // expend a power charge
        ModifyPowerCharges(-1);
    }

    void LokiPower(PowerFunctionMode a_powerFunctionMode, Vector3 a_greenPegPosition)
    {
        // if the green peg has been triggered
        if (a_powerFunctionMode == PowerFunctionMode.Trigger)
        {
            // add the charges
            ModifyPowerCharges(m_lokiPowerChargesGained);
        }
		// otherwise, if the power should be reloaded
		else if (a_powerFunctionMode == PowerFunctionMode.Reload)
		{
			// clear the connection point
			m_connectionPoint = null;
			
			// make the cord and hook inactive
			m_lokiPowerCord.gameObject.SetActive(false);
			m_hook.gameObject.SetActive(false);
			
			// store that the ball is not connected to a peg
			m_connectedToPeg = false;
		}
        
        // TEMP
        print("LokiPower() called");
    }

    void MatejaPower(PowerFunctionMode a_powerFunctionMode, Vector3 a_greenPegPosition)
    {
        // if the green peg has been triggered or if the green peg power is to be set up
        if (a_powerFunctionMode == PowerFunctionMode.Trigger || a_powerFunctionMode == PowerFunctionMode.SetUp)
        {
            // if Mateja does not currently exist
            if (m_mateja == null)
            {
                // create the mateja game object
                m_mateja = Instantiate(m_matejaPrefab) as GameObject;
                // give it this instance of player controls so it can access the time scale
                Mateja matejaScript = m_mateja.GetComponent<Mateja>();
                matejaScript.m_playerControls = this;
                // give it the bucket and victory bucket game objects
                matejaScript.m_bucket = m_bucket.gameObject;
                matejaScript.m_victoryBuckets = m_victoryBuckets;
            }
            // if there is already a Mateja
            else
            {
                // have another Mateja be created next turn
                m_setUpPowerNextTurn = true;
            }
        }
		// otherwise, if the power should be reloaded
		else if (a_powerFunctionMode == PowerFunctionMode.Reload)
		{
			// destroy Mateja
			Destroy(m_mateja);
		}

        // TEMP
        print("MatPower() called");
    }

    void ToggleHillside()
    {
        // flip the bucket around the x axis
        m_bucket.transform.position = new Vector3(m_bucket.transform.position.x, -m_bucket.transform.position.y);
        m_bucket.transform.rotation = Quaternion.Euler(m_bucket.transform.rotation.eulerAngles.x, m_bucket.transform.rotation.eulerAngles.y, m_bucket.transform.rotation.eulerAngles.z + 180.0f);

        // invert the y axis values of the positions of the bucket's MoveToPoints script
        m_bucket.m_firstPosition.y *= -1.0f;
        m_bucket.m_secondPosition.y *= -1.0f;
        m_bucket.m_targetPosition.y *= -1.0f;

        // flip the victory buckets around the x axis
        m_victoryBuckets.transform.position = new Vector3(m_victoryBuckets.transform.position.x, -m_victoryBuckets.transform.position.y);
        m_victoryBuckets.transform.rotation = Quaternion.Euler(m_victoryBuckets.transform.rotation.eulerAngles.x, m_victoryBuckets.transform.rotation.eulerAngles.y, m_victoryBuckets.transform.rotation.eulerAngles.z + 180.0f);

        // flip the launcher around the x axis
        m_launcher.transform.position = new Vector3(m_launcher.transform.position.x, -m_launcher.transform.position.y);
        m_launcher.transform.rotation = Quaternion.Euler(m_launcher.transform.rotation.eulerAngles.x, m_launcher.transform.rotation.eulerAngles.y, m_launcher.transform.rotation.eulerAngles.z + 180.0f);

        // invert the rotation center of the launcher
        m_LauncherLookControls.m_validRotationCentre *= -1.0f;

        // flip the Top Play Area Collider around the x axis
        m_topPlayAreaCollider.transform.position = new Vector3(m_topPlayAreaCollider.transform.position.x, -m_topPlayAreaCollider.transform.position.y);
        m_topPlayAreaCollider.transform.rotation = Quaternion.Euler(m_topPlayAreaCollider.transform.rotation.eulerAngles.x, m_topPlayAreaCollider.transform.rotation.eulerAngles.y, m_topPlayAreaCollider.transform.rotation.eulerAngles.z + 180.0f);

        // swap the active state of the game overlays
        m_gameOverlay.SetActive(!m_gameOverlay.activeSelf);
        m_hillsideGameOverlay.SetActive(!m_hillsideGameOverlay.activeSelf);

        // invert gravity
        Physics2D.gravity *= -1;
    }

    void SweetsPower(PowerFunctionMode a_powerFunctionMode, Vector3 a_greenPegPosition)
    {
        // if the green peg has been triggered
        if (a_powerFunctionMode == PowerFunctionMode.Trigger)
        {
            // if there are 0 power charges
            if (m_powerCharges == 0)
            {
                // have the power set up next turn
                m_setUpPowerNextTurn = true;
            }
            // add the charges
            ModifyPowerCharges(m_sweetsPowerChargesGained);
        }
        // otherwise, if the player has just shot a ball
        else if (a_powerFunctionMode == PowerFunctionMode.OnShoot)
        {
            // reduce the power charges by 1
            ModifyPowerCharges(-1);
            // if there are now 0 charges
            if (m_powerCharges == 0)
            {
                // have the power resolve at the start of next turn
                m_resolvePowerNextTurn = true;
            }
        }
        // otherwise, if the green peg power is to be set up or resolved as the player has run out of power charges
        else if (a_powerFunctionMode == PowerFunctionMode.SetUp || (a_powerFunctionMode == PowerFunctionMode.Resolve && m_powerCharges == 0))
        {
			// flip the bucket, launcher and gravity
            ToggleHillside();
        }
		// otherwise, if the power should be reloaded
		else if (a_powerFunctionMode == PowerFunctionMode.Reload)
		{
			// if the power has flipped gravity to be positive
			if (Physics2D.gravity.y > 0)
			{
				// flip the bucket, launcher and gravity back to default positions
				ToggleHillside();
			}
		}

        // TEMP
        print("SweetsPower() called");
    }

    void PhoebePower(PowerFunctionMode a_powerFunctionMode, Vector3 a_greenPegPosition)
    {
		if (a_powerFunctionMode == PowerFunctionMode.Trigger)
		{
			// if there are 0 power charges
			if (m_powerCharges == 0)
			{
                // have the power set up next turn
                m_setUpPowerNextTurn = true;
            }
            // add the charges
            ModifyPowerCharges(m_phoebePowerChargesGained);

		}
        // otherwise, if the green peg power is to be set up
        else if (a_powerFunctionMode == PowerFunctionMode.SetUp)
        {
            // set up the bocconcini list
            m_bocconcinis = new List<Bocconcini>();

            // loop for each peg in the current level
            foreach (Peg peg in m_pegManager.m_pegs)
            {
                // if the peg is not set to null, it is active
                if (peg != null)
                {
                    // create a Bocconcini
                    GameObject bocconcini = Instantiate(m_bocconciniPrefab) as GameObject;
                    // set its parent to be the peg after creation so its scale isn't modified
                    bocconcini.transform.parent = peg.transform;
                    // position the bocconcini at the peg's position
                    bocconcini.transform.position = peg.transform.position;
                    // add it to the list of bocconcinis
                    m_bocconcinis.Add(bocconcini.GetComponent<Bocconcini>());
                }
            }
        }
        // otherwise, if the player has just shot a ball
        else if (a_powerFunctionMode == PowerFunctionMode.OnShoot)
        {
            // reduce the power charges by 1
            ModifyPowerCharges(-1);
            // if there are now 0 charges
            if (m_powerCharges == 0)
            {
                // have the power resolve at the start of next turn
                m_resolvePowerNextTurn = true;
            }
        }
		// otherwise, if the bocconcini list has been initialised and the power should be reloaded or resolved as the player has run out of power charges
		else if (m_bocconcinis != null && (a_powerFunctionMode == PowerFunctionMode.Reload || (a_powerFunctionMode == PowerFunctionMode.Resolve && m_powerCharges == 0)))
		{
            // loop through the bocconcini list
            for (int i = 0; i < m_bocconcinis.Count; ++i)
            {
                // if the bocconcini hasn't already been destroyed
                if (m_bocconcinis[i] != null)
                {
                    // replace it with the peg it replaced
                    m_bocconcinis[i].ReenableParentPeg();
                }
            }
		}
		
        // TEMP
        print("PhoebePower() called");
    }

    public void ModifyPowerCharges(int a_modifier)
    {
        // increase the power charges by the modifier
        m_powerCharges += a_modifier;
        // update the UI text
        m_PowerChargesText.text = m_powerCharges.ToString();
    }

	public void RemoveBall()
	{
		// if there is a mateja in play
        if (m_mateja != null)
        {
            // have mateja launch back up
            m_mateja.GetComponent<Mateja>().JiuJitsuBall(m_ball);
            // destroy the ball
            Destroy(m_ball);
        }
        // if there is not a mateja in play
        else
        {
            // if the ball count is over 0
            if (m_ballCount > 0)
            {
                // resolve the turn
                ResolveTurn();
            }
            // if the player has run out of balls
            else
            {
                // destroy the ball
                Destroy(m_ball);
                // tell the UI Manager that the level is over and the player failed
                m_UIManager.LevelOver(false);
            }
        }
	}

    GameObject Shoot()
    {
        // switch the time scale back to default
        ModifyTimeScale(m_defaultTimeScale);
		
		GameObject Ball = null;
		
		// if Isaac should be spawned instead of the ball
		if (m_spawnIsaac)
		{
			// create a copy of the Isaac prefab
			Ball = Instantiate(m_IsaacPrefab);
			// set its position to be the same as this game object
			Ball.transform.position = transform.position;
			// give Isaac this component
			Ball.GetComponent<Isaac>().m_playerControls = this;
		}
		// if the ball should be spawned, not Isaac
		else
		{
			// create a copy of the ball prefab
			Ball = Instantiate(m_ballPrefab) as GameObject;
			// set its position to be the same as this game object
			Ball.transform.position = transform.position;
			// apply the launch speed force to the ball, in the direction this gameobject is facing
			Ball.GetComponent<Rigidbody2D>().AddForce(transform.up * m_ballLaunchSpeed, ForceMode2D.Impulse);
			// give the ball the peg manager
			Ball.GetComponent<Ball>().m_pegManager = m_pegManager;
			// give the ball this component
			Ball.GetComponent<Ball>().m_playerControls = this;
		}
        

        // reduce the ball count by one as a ball has been expended
        --m_ballCount;
        // update the ball count text
        m_ballCountText.text = m_ballCount.ToString();
        // set the game state to Ball In Play
        m_currentGameState = GameState.BallInPlay;
        // return the ball gameobject
        return Ball;
    }

    public void ResolveTurn()
    {
        // if the ball is in play
        if (m_ball != null)
        {
            // destroy it
            Destroy(m_ball);
        }

        // set the game state to Resolve Turn
        m_currentGameState = GameState.ResolveTurn;

        // tell the peg manager to clear all the hit pegs. If there were no pegs to clear give the player a 50% chance to get back a free ball
        if (!m_pegManager.ClearHitPegs() && Random.Range(0,2) == 1)
        {
            // give the player a free ball without playing the free ball sound
            FreeBall(false, false);
        }

        // hide the Loki Power Cord if it is visible
        m_lokiPowerCord.gameObject.SetActive(false);

        // disable the hook
        m_hook.SetActive(false);

        // destroy any active lines
        DestroyLines();

        // tell the camera to return to default in case it had moved while the ball was in play
        m_cameraZoom.ReturnToDefault();
        // tell the peg manager to resolve the turn
        m_pegManager.ResolveTurn();

        // if the power is the Phoebe power, there are power charges, and the bocconcini array has been initialised
        if (m_greenPegPower == PhoebePower && m_powerCharges > 0 && m_bocconcinis != null)
        {
            // loop through the bocconcini list
            for (int i = 0; i < m_bocconcinis.Count; ++i)
            {
                // if the bocconcini's parent peg is still active
                if (m_bocconcinis[i].transform.parent.gameObject.activeSelf)
                {
                    // update the color of the boccocinis
                    m_bocconcinis[i].CopyParentPegColor();
                }
            }
        }
    }

    public void FreeBall(bool a_playSound = true, bool a_showFreeBallText = false)
    {
        // increase the ball count by 1 as a ball has been gained
        ++m_ballCount;

        // if the free ball sound effect should be played
        if (a_playSound)
        {
            // play the free ball sound that corresponds to the amount of free balls earned this round 
            m_audioSource.PlayOneShot(m_freeBallSounds[m_pegManager.m_freeBallsAwarded]);
        }

        // if the free ball text should shown
        if (a_showFreeBallText)
        {
            // update the ball count text with a message denoting that a free ball has been given
            m_ballCountText.text = "Free Ball!";
            // start a timer to determine how long the ballCountText should display the "Free Ball!" text
            m_freeBallTextTimer = m_freeBallTextDuration;
        }
        // otherwise, if the Free Ball Text is not currently being shown
        else if (m_freeBallTextTimer <= 0.0f)
        {
            // update the ball count text with the amount of balls available
            m_ballCountText.text = m_ballCount.ToString();
        }
    }

    public void ModifyTimeScale(float a_newTimeScale)
    {
        // set the time scale to the new value
        Time.timeScale = a_newTimeScale;
        // ensure fixedUpdate is called with the same frequency regardless of time scale so physics remains smooth
        Time.fixedDeltaTime = m_defaultDeltaTime * Time.timeScale;
    }

    public void Reload()
    {
        // reset the ball count
        m_ballCount = m_startingBallCount;
        m_ballCountText.text = m_ballCount.ToString();

        // reset the power charges
        m_powerCharges = 0;
        m_PowerChargesText.text = m_powerCharges.ToString();
		
		// reload the power
		m_greenPegPower(PowerFunctionMode.Reload, Vector3.zero);

        // set the game state to the shoot phase
        m_currentGameState = GameState.Shoot;
		
		// destroy the ball if one exists
		if (m_ball != null)
		{
		    Destroy(m_ball);
		}
		
		// reset the time scale
		ModifyTimeScale(m_defaultTimeScale);
    }

    void Start()
    {
        // initialise default time variables
        m_defaultTimeScale = Time.timeScale;
        m_defaultDeltaTime = Time.fixedDeltaTime;

        // initialise the ball count
        m_ballCount = m_startingBallCount;

        // get the child of the ink resource bar background as the ink resource bar
        m_inkResourceBar = m_inkResourceBarBackground.transform.GetChild(0).GetComponent<RectTransform>();

        // get the width of the ink resource bar background
        m_inkResourceBarMaxWidth = m_inkResourceBarBackground.GetComponent<RectTransform>().sizeDelta.x;

		// store the base peg scores of each type for the Jack Power
		m_defaultBluePegScore = m_pegManager.m_baseBluePegScore;
		m_defaultOrangePegScore = m_pegManager.m_baseOrangePegScore;
		m_defaultPurplePegScore = m_pegManager.m_basePurplePegScore;
		m_defaultGreenPegScore = m_pegManager.m_baseGreenPegScore;

        // TEMP
        m_greenPegPower = DanielPower;
    }

    void Update()
    {
        // TEMP
        if (Input.GetButton("Speed Up Time"))
        {
            // change the timescale to the sped up timescale
            ModifyTimeScale(m_spedUpTimeScale);
        }
        // otherwise, if the Speed Up Time button has been released
        else if (Input.GetButtonUp("Speed Up Time"))
        {
            // reset the timescale
            ModifyTimeScale(m_defaultTimeScale);
        }
        // TEMP

        // if the Free Ball Text Timer is active
        if (m_freeBallTextTimer > 0.0f)
        {
            // reduce the timer
            m_freeBallTextTimer -= Time.unscaledDeltaTime;

            // if the timer has ran out
            if (m_freeBallTextTimer <= 0.0f)
            {
                // set it to 0
                m_freeBallTextTimer = 0.0f;
                // replace the ball Count Text with the ball count
                m_ballCountText.text = m_ballCount.ToString();
            }
        }

        // if the green peg power is Kevin's and the show sniper scope button has been released
        if (m_greenPegPower == KevinPower && Input.GetButtonUp("Show Sniper Scope"))
        {
            // tell the camera to return to its default state
            m_cameraZoom.ReturnToDefault();
            // hide the scope overlay
            m_scopeOverlay.SetActive(false);
            // reset the time scale
            ModifyTimeScale(m_defaultTimeScale);
        }

        // if the current game state is Turn Set Up
        if (m_currentGameState == GameState.TurnSetUp)
        {
            // if the power should be set up this turn
            if (m_setUpPowerNextTurn)
            {
                // set up the power for this turn
                m_greenPegPower(PowerFunctionMode.SetUp, Vector3.zero);
                // set the Set Up Power Next Turn flag to false as the power has now been set up
                m_setUpPowerNextTurn = false;
            }
            else if (m_resolvePowerNextTurn)
            {
                // resolve the power
                m_greenPegPower(PowerFunctionMode.Resolve, Vector3.zero);
                // set the Resolve Power Next Turn flag to false as the power has now been resolved
                m_resolvePowerNextTurn = false;
            }

            // TEMP
            m_currentGameState = GameState.Shoot;
        }
        // if the current game state is Shoot
        else if (m_currentGameState == GameState.Shoot)
        {
            // if the power is Ethen's power and drawing mode is on
            if (m_drawing && m_greenPegPower == EthenPower)
            {
                // if there is ink remaining and the Shoot / Use Power input is currently pressed
                if (m_ink > 0.0f && Input.GetButton("Shoot / Use Power"))
                {
                    // if this was the first frame that the Shoot / Use Power input was pressed
                    if (Input.GetButtonDown("Shoot / Use Power"))
                    {
                        // store that the player has started drawing a line
                        m_lineBegun = true;
                    }
                    // otherwise, if the mouse has moved enough since last frame
                    else if ((Input.mousePosition - m_previousMousePosition).sqrMagnitude >= m_minValidSquareMouseMovement)
                    {
                        // if the player started drawing a line previously
                        if (m_lineBegun == true)
                        {
                            // create a new line object and make it a child of the lines game object
                            GameObject line = Instantiate(m_linePrefab, m_lines.transform) as GameObject;

                            // store the line renderer for the current line
                            m_currentLineRenderer = line.GetComponent<LineRenderer>();

                            // Update the line with the previous point
                            UpdateLine(Camera.main.ScreenToWorldPoint(m_previousMousePosition));

                            // Update the line with the new point
                            UpdateLine(Camera.main.ScreenToWorldPoint(Input.mousePosition));

                            // store that the line has no longer begun being drawn
                            m_lineBegun = false;
                        }

                        // reduce the amount of ink the player has based on the amount the mouse moved
                        m_ink -= (Input.mousePosition - m_previousMousePosition).magnitude;

                        // update the ink resource bar
                        UpdateInkResourceBar();

                        // Update the line with the new point
                        UpdateLine(Camera.main.ScreenToWorldPoint(Input.mousePosition));
                    }
                }
                // otherwise, if a line has been drawn, the line doesn't currently have a collider and the Shoot / Use Power input was released
                else if (m_currentLineRenderer != null && m_currentLineRenderer.gameObject.GetComponent<EdgeCollider2D>() == null && m_currentLineRenderer.positionCount > 0 && Input.GetButtonUp("Shoot / Use Power"))
                {
                    // add an edge collider to the line
                    EdgeCollider2D collider = m_currentLineRenderer.gameObject.AddComponent<EdgeCollider2D>();

                    // convert the line renderer points to vector2
                    Vector2[] points = new Vector2[m_currentLineRenderer.positionCount];
                    for (int i = 0; i < points.Length; ++i)
                    {
                        points[i] = new Vector2(m_currentLineRenderer.GetPosition(i).x, m_currentLineRenderer.GetPosition(i).y);
                    }

                    // give the vector2 line points to the collider
                    collider.points = points;
                }

                // store the mouse position for next frame
                m_previousMousePosition = Input.mousePosition;
            }
            // if the power is not Ethens or if the drawing mode isn't on
            else
            {
                // if the Shoot / Use Power input has been detected
                if (Input.GetButtonDown("Shoot / Use Power"))
                {
                    // shoot a ball
                    m_ball = Shoot();

                    // if there are power charges
                    if (m_powerCharges > 0)
                    {
                        // have the power trigger its On Shoot effect
                        m_greenPegPower(PowerFunctionMode.OnShoot, Vector3.zero);
                    }
                }
                // TEMP
                /*
                // otherwise, if the Speed Up Time input has been detected
                else if (Input.GetButton("Speed Up Time"))
                {
                    // change the timescale to the sped up timescale
                    ModifyTimeScale(m_spedUpTimeScale);
                }
                // otherwise, if the Speed Up Time button has been released
                else if (Input.GetButtonUp("Speed Up Time"))
                {
                    // reset the timescale
                    ModifyTimeScale(m_defaultTimeScale);
                }
                */
            }
        }
        // if the current gamestate is Ball In Play
        else if (m_currentGameState == GameState.BallInPlay)
        {
            // if the green peg power is Kevin's and there are power charges
            if (m_powerCharges > 0 && m_greenPegPower == KevinPower)
            {
                // if the show sniper scope button has been pressed
                if (Input.GetButtonDown("Show Sniper Scope"))
                {
                    // tell the camera to zoom and track the cursor
                    m_cameraZoom.ZoomAndTrack();
                    // show the scope overlay
                    m_scopeOverlay.SetActive(true);
                    // set the time scale to the scoped time scale
                    ModifyTimeScale(m_scopedTimeScale);
                }

                // if the shoot / use power button has been pressed and the camera is at max zoom
                if (Input.GetButtonDown("Shoot / Use Power") && m_cameraZoom.m_atMaxZoom)
                {
                    // reduce the power charges by 1
                    ModifyPowerCharges(-1);
                    // if there are now 0 charges
                    if (m_powerCharges == 0)
                    {
                        // have the power resolve at the start of next turn
                        m_resolvePowerNextTurn = true;
                    }

                    // if the camera is looking at a point on the ball
                    if (m_ball.GetComponent<Collider2D>().bounds.Contains(new Vector3(Camera.main.transform.position.x, Camera.main.transform.position.y, m_ball.transform.position.z)))
                    {
                        // shoot the ball in the direction opposite of where it got shot from, with a magnitude determined by m_forceToBall
                        m_ball.GetComponent<Rigidbody2D>().AddForce((m_ball.transform.position - Camera.main.transform.position).normalized * m_forceToBall, ForceMode2D.Impulse);
                    }
                }
            }
            // otherwise, if the green peg power is Loki's
            else if (m_greenPegPower == LokiPower)
            {
                // if the shoot / use power button is currently pressed
                if (Input.GetButton("Shoot / Use Power"))
                {
                    // if there are power charges and this is the first frame the shoot / use power button has been pressed
                    if (m_powerCharges > 0 && Input.GetButtonDown("Shoot / Use Power"))
                    {
                        // initialise the hook
                        m_hook.SetActive(true);
                        m_hook.transform.position = m_ball.transform.position;

                        // have the end of the cord be the hook
                        m_connectionPoint = m_hook;

                        // shoot the hook towards the cursor
                        m_hook.GetComponent<Rigidbody2D>().AddForce((Camera.main.ScreenToWorldPoint(Input.mousePosition) - m_ball.transform.position).normalized * m_hookLaunchSpeed, ForceMode2D.Impulse);

                        // have the cord be active
                        m_lokiPowerCord.gameObject.SetActive(true);

                        // store that the ball is not currently connected to a peg
                        m_connectedToPeg = false;
                    }

                    if (m_connectedToPeg)
                    {
                        // TEMP
                        m_ball.GetComponent<Rigidbody2D>().AddForce((m_connectionPoint.transform.position - m_ball.transform.position).normalized * m_pullSpeed, ForceMode2D.Force);
                    }
                    

                    // if the cord is currently active
                    if (m_lokiPowerCord.gameObject.activeSelf)
                    {
                        // if the connection point is null or inactive, or if the cord has gone beyond its max length
                        if (m_connectionPoint == null || !m_connectionPoint.activeSelf || (m_ball.transform.position - m_hook.transform.position).sqrMagnitude >= m_maxCordLength * m_maxCordLength)
                        {
                            // make the cord and hook inactive
                            m_lokiPowerCord.gameObject.SetActive(false);
                            m_hook.gameObject.SetActive(false);

                            // store that the ball is not connected to a peg
                            m_connectedToPeg = false;
                        }
                        // if the cord is still not too long
                        else
                        {
                            // draw a line between the ball and the connection point
                            m_lokiPowerCord.SetPosition(0, m_ball.transform.position);
                            m_lokiPowerCord.SetPosition(1, m_connectionPoint.transform.position);
                        }
                    }
                }
                else
                {
                    // disable the cord
                    m_lokiPowerCord.gameObject.SetActive(false);
                    // disable the hook
                    m_hook.SetActive(false);
                    // store that the ball is not connected to a peg
                    m_connectedToPeg = false;
                }
            }

            // if the ball is in play and has fallen low enough (or high enough with the Sweets Power)
            if (m_ball != null && (m_ball.transform.position.y <= m_ballKillFloor || m_ball.transform.position.y >= -m_ballKillFloor))
            {
				// remove the ball from play
				RemoveBall();
            }
        }
    }
}
