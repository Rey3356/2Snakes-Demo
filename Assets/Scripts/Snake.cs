using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.SceneManagement;
using static UnityEngine.Rendering.DebugUI;

public class Snake : MonoBehaviour
{
    [Header("This Snake's Specs")]
    [SerializeField] public int SnakeID;
    [SerializeField] private int SnakeScore;
    [SerializeField] private float SnakeSpeed = 15f;
    [SerializeField] private float speedMultiplier = 1f;

    [Header("This Snake's Movement Related Operational Properties")]
    [SerializeField] private ScreenBounds screenBounds;
    private Vector2 _direction = Vector2.zero;
    private float nextUpdate;

    [Header("This Snake's Segment Prefab")]
    private Vector2 BufferSegmentSpawnPoint;
    private List<Transform> _segments = new List<Transform>();
    [SerializeField] private Transform segmantPrefab;

    [Header("This Snake's PowerUp Status")]
    [SerializeField] private bool SpeedUp;
    [SerializeField] public bool Shield; //Status to be accesible for both snakes
    [SerializeField] private bool twoX;

    [Header("This Snake's Audio Source and Audio Clips")]
    [SerializeField] private AudioSource BasicClip;
    [SerializeField] private List<AudioClip> BasicClips = new List<AudioClip>();
    [SerializeField] private AudioSource PowerUpClip;
    [SerializeField] private List<AudioClip> PowerUpClips = new List<AudioClip>();

    [SerializeField] public bool GameEndedOnSnake;

    private ScoreManager ScoreM; //ScoreManager Reference

    #region CACHEING_CRITICALS
    private void Awake()
    {
        screenBounds = GameObject.FindGameObjectWithTag("ScreenWrapCollider").GetComponent<ScreenBounds>();
        ScoreM = GameObject.FindGameObjectWithTag("ScoreManager").GetComponent<ScoreManager>();
        _segments.Add(this.transform);
    }

    private void Start()
    {
        SnakeScore = 0;
        GameEndedOnSnake = false;
        BufferSegmentSpawnPoint = new Vector2(100, 100);
    } 
    #endregion

    #region UPDATES - Getting Inputs and Setting Movement details per FU
    private void Update()
    {
        setInput();
        GetInput(ref _direction);
    }

    private void FixedUpdate()
    {
        // Wait until the next update before proceeding
        if (Time.time < nextUpdate)
        {
            return;
        }

        if (GameEndedOnSnake == false)
        {
            SnakeMovement();
        }

        // Set the next update time based on the speed
        if (SpeedUp)
        {
            speedMultiplier = 2.0f;
        }
        else
        {
            speedMultiplier = 1.0f;
        }
        nextUpdate = Time.time + (1f / (SnakeSpeed * speedMultiplier));
    } 
    #endregion

    #region INPUTTING

    private List<KeyCode> Keys = new List<KeyCode>(); 
    private void setInput()
    {
        // 0 is UP, 1 is DOWN, 2 is RIGHT, 3 is LEFT
        switch(SnakeID)
        {
            case 1:
                Keys.Add(KeyCode.W);
                Keys.Add(KeyCode.S);
                Keys.Add(KeyCode.D);
                Keys.Add(KeyCode.A);
                break;

            case 2:
                Keys.Add(KeyCode.UpArrow);
                Keys.Add(KeyCode.DownArrow);
                Keys.Add(KeyCode.RightArrow);
                Keys.Add(KeyCode.LeftArrow);
                break;

            default:
                Debug.Log("Invalid Snake ID!");
                break;

        }
    }
    private void GetInput(ref Vector2 Direction)
    {
        if (Input.GetKeyDown(Keys[0]) && Direction != Vector2.down)
        {   
            BasicClip.clip = BasicClips[0];
            BasicClip.Play();
            Direction = Vector2.up;
        }
        else if (Input.GetKeyDown(Keys[1]) && Direction != Vector2.up)
        {
            BasicClip.clip = BasicClips[0];
            BasicClip.Play();
            Direction = Vector2.down;
        }
        else if (Input.GetKeyDown(Keys[2]) && Direction != Vector2.left)
        {
            BasicClip.clip = BasicClips[0];
            BasicClip.Play();
            Direction = Vector2.right;
        }
        else if(Input.GetKeyDown(Keys[3]) && Direction != Vector2.right)
        {
            BasicClip.clip = BasicClips[0];
            BasicClip.Play();
            Direction = Vector2.left;
        }

    }

    #endregion

    #region MOVEMENT
    private void SnakeMovement()
    {
        //For each Subsequent Segment to follow
        for (int i = _segments.Count - 1; i > 0; i--)
        {
            _segments[i].position = _segments[i - 1].position;
        }

        //Base Movement logic
        Vector3 tempPosition = new Vector2(Mathf.Round(this.transform.position.x) + _direction.x,
        Mathf.Round(this.transform.position.y) + _direction.y);
        if (screenBounds.AmIOutOfBounds(tempPosition))
        {
            Vector2 newPosition = screenBounds.CalculateWrappedPosition(tempPosition);
            this.transform.position = newPosition;
        }
        else
        {
            this.transform.position = tempPosition;
        }

    }
    #endregion

    #region SNAKE_SIZE
    private void Grow()
    {
        Transform segment = Instantiate(this.segmantPrefab);
        segment.position = BufferSegmentSpawnPoint;
        _segments.Add(segment);
        segment.transform.SetParent(this.transform.parent);

    }

    private void Shrink()
    {
            Destroy(_segments[_segments.Count - 1].gameObject);
            _segments.RemoveAt(_segments.Count - 1);
    }
    #endregion

    #region POWERUP_COROUTINES
    IEnumerator SpeedingUp()
    {
        SpeedUp = true;
        PowerUpClip.clip = PowerUpClips[0];
        PowerUpClip.Play();
        yield return new WaitForSecondsRealtime(8);
        SpeedUp = false;
        PowerUpClip.Stop();
    }
    IEnumerator ShieldsUP()
    {
        Shield = true;
        PowerUpClip.clip = PowerUpClips[1];
        PowerUpClip.Play();
        yield return new WaitForSecondsRealtime(8);
        Shield = false;
        PowerUpClip.Stop();
    }

    IEnumerator DoubleScore()
    {
        twoX = true;
        PowerUpClip.clip = PowerUpClips[2];
        PowerUpClip.Play();
        yield return new WaitForSecondsRealtime(8);
        twoX = false;
        PowerUpClip.Stop();
    }
    #endregion

    #region COLLISIONS_LOGIC

    bool temp;
    private void OnTriggerEnter2D(Collider2D other)
    {

        #region ForFoodCollisions
        if (other.tag == "GainFood")
        {
            if (twoX)
            {
                SnakeScore += 2;
                BasicClip.clip = BasicClips[1];
                BasicClip.Play();
                ScoreM.setScore(SnakeID, SnakeScore);
                Grow();
            }
            else
            {
                SnakeScore += 1;
                BasicClip.clip = BasicClips[1];
                BasicClip.Play();
                ScoreM.setScore(SnakeID, SnakeScore);
                Grow();

            }

        }

        if (other.tag == "LossFood" && !Shield)
        {
            SnakeScore -= 1;
            if (_segments.Count == 1)
            {
                BasicClip.clip = BasicClips[3];
                BasicClip.Play();
                GameEndedOnSnake = true;
                GameObject.FindGameObjectWithTag("Snake").GetComponent<Snake>().GameEndedOnSnake = true;

            }
            else
            {
                BasicClip.clip = BasicClips[2];
                BasicClip.Play();
                ScoreM.setScore(SnakeID, SnakeScore);
                Shrink();
            }          

        }
        #endregion

        #region ForPowerUpCollisions
        if (other.tag == "SpeedUp")
        {
            Debug.Log("SPEED UPPPP for " + SnakeID);
            StartCoroutine(SpeedingUp());

        }

        if (other.tag == "Shield")
        {
            Debug.Log("SHIELDS UPPPP for" + SnakeID);
            StartCoroutine(ShieldsUP());
        }

        if (other.tag == "2X")
        {
            Debug.Log("ScoreDoubles for " + SnakeID);
            StartCoroutine(DoubleScore());
        }
        #endregion

        #region GameEndingScenarios 
        //*other than eating loss food at 1 unit of snake length

        //Scenario 1 : Snake bites Self
        if (((SnakeID == 1 && other.tag == "SnakeSeg") || (SnakeID == 2 && other.tag == "Snake'Seg")) && !Shield)
        {
            BasicClip.clip = BasicClips[3];
            BasicClip.Play();
            GameEndedOnSnake = true;
            GameObject.FindGameObjectWithTag("Snake").GetComponent<Snake>().GameEndedOnSnake = true;
        }

        //Scenario 2 : Snake Bites the Other Snake's Body without Shield
        if ((SnakeID == 1 && other.tag == "Snake'Seg") || (SnakeID == 2 && (other.tag == "SnakeSeg")))
        {

            switch (SnakeID)
            {

                case 1:
                    temp = GameObject.FindGameObjectWithTag("Snake'").GetComponent<Snake>().Shield;
                    break;
                case 2:
                    temp = GameObject.FindGameObjectWithTag("Snake").GetComponent<Snake>().Shield;
                    break;
                default:
                    break;
            }
            if (!temp)
            {
                GameObject.FindGameObjectWithTag("Snake").GetComponent<Snake>().GameEndedOnSnake = true;
                GameEndedOnSnake = true;
            }

        }

        //Scenario 3 : Snake Bites the Other Snake's Head without Shield
        if ((SnakeID == 1 && other.tag == "Snake'") || (SnakeID == 2 && (other.tag == "Snake")))
        {

            switch (SnakeID)
            {

                case 1:
                    temp = GameObject.FindGameObjectWithTag("Snake'").GetComponent<Snake>().Shield;
                    break;
                case 2:
                    temp = GameObject.FindGameObjectWithTag("Snake").GetComponent<Snake>().Shield;
                    break;
                default:
                    break;
            }
            if (!temp)
            {
                GameObject.FindGameObjectWithTag("Snake").GetComponent<Snake>().GameEndedOnSnake = true;
                GameEndedOnSnake = true;
            }

        }

        #endregion
    }
    #endregion

}