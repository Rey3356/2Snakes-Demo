using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    [SerializeField] public TextMeshProUGUI Score1;
    [SerializeField] public TextMeshProUGUI Score2;

    [SerializeField] public int P1Score;
    [SerializeField] public int P2Score;

    public bool stop;

    private void Start()
    {
        Score1.SetText("0");
        Score2.SetText("0");
    }

    public void setScore(int id, int score)
    {
        if (!stop)
        {
            if (id == 1)
            {
                P1Score = score;
                Score1.text = score.ToString();
            }

            else if (id == 2)
            {
                P2Score = score;
                Score2.text = score.ToString();
            }
        }
    }

}
