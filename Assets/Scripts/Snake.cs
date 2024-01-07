using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Snake : MonoBehaviour
{
    private Vector2 _direction = Vector2.zero;

    [SerializeField] private Transform segmantPrefab;
    [SerializeField] private Transform BufferSegmentSpawnPoint;
    private List<Transform> _segments = new List<Transform>();

    bool SnakeAlive;

    private void Awake()
    {
        SnakeAlive = true;
        _segments.Add(this.transform);
    }

    private void Update()
    {
        GetInput(ref _direction);
    }

    private void FixedUpdate()
    {
        if(SnakeAlive == true)
        {
            SnakeMovement();
        }
        
    }

    private void GetInput(ref Vector2 Direction)
    {
        if(Input.GetKeyDown(KeyCode.UpArrow) && Direction != Vector2.down)
        {
            Direction = Vector2.up;
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow) && Direction != Vector2.up)
        {
            Direction = Vector2.down;
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow) && Direction != Vector2.left)
        {
            Direction = Vector2.right;
        }
        else if(Input.GetKeyDown(KeyCode.LeftArrow) && Direction != Vector2.right)
        {
            Direction = Vector2.left;
        }

    }

    private void SnakeMovement()
    {
        for (int i = _segments.Count - 1; i > 0; i--)
        {
            _segments[i].position = _segments[i - 1].position;
        }

        this.transform.position =
            new Vector2(Mathf.Round(this.transform.position.x) + _direction.x,
            Mathf.Round(this.transform.position.y) + _direction.y);
    }

    private void Grow()
    {
        Transform segment = Instantiate(this.segmantPrefab);
        _segments.Add(segment);
        segment.position = BufferSegmentSpawnPoint.position;
    }
    private void Shrink()
    {
        Destroy(_segments[_segments.Count - 1].gameObject);
        _segments.RemoveAt(_segments.Count - 1);
        
        if (_segments.Count == 0)
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
        SnakeAlive = false;
        yield return new WaitForSeconds(3);
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "GainFood")
        {
            Grow();
        }

        else if(other.tag == "LossFood")
        {
            Shrink();
        }

        else if (other.tag == "Snake")
        {
            DeathReset();
        }
    }
}
