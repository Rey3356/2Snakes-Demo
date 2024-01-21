using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Device;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField] private List<Snake> Snakes = new List<Snake>();
    [SerializeField] private ScoreManager ScoreM;
    [SerializeField] private ItemsManager ItemsM;
    [SerializeField] private TextMeshProUGUI P1Score;
    [SerializeField] private TextMeshProUGUI P2Score;
    [SerializeField] private TextMeshProUGUI Winner;
    [SerializeField] private AudioSource SfxManager;
    [SerializeField] private AudioClip ButtonSfx;

    bool gameEnded;

    [SerializeField] private GameObject GOScreen;
    [SerializeField] private GameObject PauseScreen;

    // Start is called before the first frame update
    void Start()
    {
        Time.timeScale = 1.0f;
        gameEnded = false;
        GOScreen.SetActive(false);
    }

    void Update()
    {
        if(!gameEnded)
        {
            if(Snakes.Count > 1)
            {
                foreach (var snake in Snakes)
                {
                    if (snake.GameEndedOnSnake)
                    {
                        gameEnded = true;
                        ScoreM.stop = true;
                        ItemsM.stop = true;
                        UpdateScoresOnScreen2(P1Score, P2Score);
                        DecideWinner();
                        StartCoroutine(GameEnder());

                    }

                }
            }
            else
            {
                if (Snakes[0].GameEndedOnSnake)
                {
                    gameEnded = true;
                    ScoreM.stop = true;
                    ItemsM.stop = true;
                    UpdateScoresOnScreen(P1Score);
                    StartCoroutine(GameEnder());

                }
                
            }

            if(Input.GetKeyDown(KeyCode.Escape))
            {
                if(PauseScreen.activeSelf == true)
                {    
                    ActivatePauseScreen(0);
                }
                else
                {
                    ActivatePauseScreen(1);
                }

            }
            
        }
    }

    public void ActivatePauseScreen(int a)
    {
        if(a == 0)
        {
            Time.timeScale = 1.0f;
            PauseScreen.SetActive(false);
        }
        if (a == 1)
        {
            Time.timeScale = 0.0f;
            PauseScreen.SetActive(true);
        }

    }

    public void Reloader()
    {     
        SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().name);
    }

    public void Levelloader(int BI)
    {
        SceneManager.LoadSceneAsync(BI);
    }


    public void AppQuitter()
    {
        UnityEngine.Application.Quit();
    }

    private void UpdateScoresOnScreen(TextMeshProUGUI s1)
    {
        s1.text = ScoreM.P1Score.ToString();
    }

    private void UpdateScoresOnScreen2(TextMeshProUGUI s1, TextMeshProUGUI s2)
    {
        s1.text = ScoreM.P1Score.ToString();
        s2.text = ScoreM.P2Score.ToString();
    }


    private void DecideWinner()
    {
        if(ScoreM.P1Score > ScoreM.P2Score)
        {
            Winner.color = new Color(57f, 255f, 20f, 255f);
            Winner.text = "Player 1 Wins!";
        }
        else if(ScoreM.P1Score < ScoreM.P2Score)
        {
            Winner.color = new Color(255f, 0f, 0f, 255f);
            Winner.text = "Player 2 Wins!";
        }
        else
        {
            Winner.color = new Color(0f, 0f, 255f, 255f);
            Winner.text = "It's a Tie!";
        }
    }

    IEnumerator GameEnder()
    {
        Debug.Log("GameEnd!");
        yield return new WaitForSecondsRealtime(5);
        GOScreen.SetActive(true);
    }
}
