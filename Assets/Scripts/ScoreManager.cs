using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    [SerializeField] public TextMeshProUGUI Score1;
    [SerializeField] public TextMeshProUGUI Score2;
    public bool stop;

    private void Start()
    {
        Score1.SetText("0");
        Score2.SetText("0");
    }

    public void setScore(int id, int score)
    {
        if (id == 1 ) 
        {
            Score1.text = score.ToString();
        }
        
        else if(id == 2)
        {
            Score2.text = score.ToString();
        }
    }


}
