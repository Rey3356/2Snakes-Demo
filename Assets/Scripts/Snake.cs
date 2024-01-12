using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using static UnityEngine.Rendering.DebugUI;

public class Snake : MonoBehaviour
{

    [SerializeField] public int SnakeID;
    [SerializeField] private int SnakeScore;
    [SerializeField] private bool SnakeLost;


    
    
    [SerializeField] private ScoreManager ScoreM;
    [SerializeField] private bool Shield;
    [SerializeField] private bool twoX;

    private float nextUpdate;
    [SerializeField] private float SnakeSpeed = 15f;
    [SerializeField] private float speedMultiplier = 1f;
    private Vector2 _direction = Vector2.zero;

    [SerializeField] private Transform segmantPrefab;
    [SerializeField] private Vector2 BufferSegmentSpawnPoint;

    private List<Transform> _segments = new List<Transform>();

    [SerializeField] private ScreenBounds screenBounds;

    [SerializeField] private AudioSource InputClip;
    [SerializeField] private AudioSource SnakePowerClip;
    [SerializeField] private List<AudioClip> Aclips = new List<AudioClip>();
    [SerializeField] private List<AudioClip> Pclips = new List<AudioClip>();

    private void Awake()
    {
        screenBounds = GameObject.FindGameObjectWithTag("ScreenWrapCollider").GetComponent<ScreenBounds>();
        ScoreM = GameObject.FindGameObjectWithTag("ScoreManager").GetComponent<ScoreManager>();
        _segments.Add(this.transform);
        BufferSegmentSpawnPoint = new Vector2(100, 100);
    }

    private void Start()
    {
        SnakeScore = 0;
        SnakeLost = true;
    }


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

        if (SnakeLost == true)
        {
            SnakeMovement();
        }

        // Set the next update time based on the speed
        nextUpdate = Time.time + (1f / (SnakeSpeed * speedMultiplier));
    }

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
            InputClip.clip = Aclips[0];
            InputClip.Play();
            Direction = Vector2.up;
        }
        else if (Input.GetKeyDown(Keys[1]) && Direction != Vector2.up)
        {
            InputClip.clip = Aclips[0];
            InputClip.Play();
            Direction = Vector2.down;
        }
        else if (Input.GetKeyDown(Keys[2]) && Direction != Vector2.left)
        {
            InputClip.clip = Aclips[0];
            InputClip.Play();
            Direction = Vector2.right;
        }
        else if(Input.GetKeyDown(Keys[3]) && Direction != Vector2.right)
        {
            InputClip.clip = Aclips[0];
            InputClip.Play();
            Direction = Vector2.left;
        }

    }

    #endregion

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

    private void Grow()
    {
        Transform segment = Instantiate(this.segmantPrefab);
        segment.position = BufferSegmentSpawnPoint;
        _segments.Add(segment);
        segment.transform.SetParent(this.transform.parent);
              
    }

    private void Shrink()
    {
        if (_segments.Count > 1)
        {
            Destroy(_segments[_segments.Count - 1].gameObject);
        _segments.RemoveAt(_segments.Count - 1);
            
        }
        else
        {
            DeathReset();
        }
    }

    private void DeathReset()
    {
        Debug.Log("Death! Game Reset!!");
        
        StartCoroutine(GameEnder());
    }

    IEnumerator GameEnder()
    {
        SnakeLost = false;
        yield return new WaitForSecondsRealtime(5);
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    IEnumerator SpeedingDown()
    {
        speedMultiplier = 2.0f;
        SnakePowerClip.clip = Pclips[0];
        SnakePowerClip.Play();
        yield return new WaitForSecondsRealtime(8);
        speedMultiplier = 1.0f;
        SnakePowerClip.Stop();
    }
    IEnumerator ShieldsUP()
    {
        Shield = true;
        SnakePowerClip.clip = Pclips[1];
        SnakePowerClip.Play();
        yield return new WaitForSecondsRealtime(8);
        Shield = false;
        SnakePowerClip.Stop();
    }

    IEnumerator DoubleScore()
    {
        twoX = true;
        SnakePowerClip.clip = Pclips[2];
        SnakePowerClip.Play();
        yield return new WaitForSecondsRealtime(8);
        twoX = false;
        SnakePowerClip.Stop();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {

        if (other.tag == "GainFood")
        {
            if(twoX)
            {
                SnakeScore += 2;
                InputClip.clip = Aclips[1];
                InputClip.Play();
                ScoreM.setScore(SnakeID, SnakeScore);
                Grow();
            }
            else
            {
                SnakeScore += 1;
                InputClip.clip = Aclips[1];
                InputClip.Play();
                ScoreM.setScore(SnakeID, SnakeScore);
                Grow();

            }
            
        }

        if (other.tag == "LossFood" && !Shield)
        {
                SnakeScore -= 1;
                if (_segments.Count == 1)
                {
                    InputClip.clip = Aclips[3];
                    InputClip.Play();
                }
                else
                {
                    InputClip.clip = Aclips[2];
                    InputClip.Play();
                }

                ScoreM.setScore(SnakeID, SnakeScore);
                Shrink();
            
        }        
        
        if(other.tag == "SpeedUp")
        {
            Debug.Log("SPEED UPPPP for " + SnakeID);           
            StartCoroutine(SpeedingDown());
            
        }

        if(other.tag == "Shield")
        {
            Debug.Log("SHIELDS UPPPP for" + SnakeID);           
            StartCoroutine(ShieldsUP());
        }

        if(other.tag == "2X")
        {
            Debug.Log("ScoreDoubles for " + SnakeID);
            StartCoroutine(DoubleScore());
        }

        if (other.tag == "Snake" || other.tag == "SnakeSeg")
        {
            if(!Shield)
            {
                InputClip.clip = Aclips[3];
                InputClip.Play();
                DeathReset();
            }
            
        }
    }

}
