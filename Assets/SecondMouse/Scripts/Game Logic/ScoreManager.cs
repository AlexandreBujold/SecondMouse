using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ScoreManager : MonoBehaviour
{

    public static ScoreManager instance;

    public UnityEvent ScoreIncrease = new UnityEvent();

    TMPro.TextMeshPro scoreText;


    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(this);
        }
        ScoreIncrease.AddListener(UpdateScoreText);
    }

    public int score;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }


    public void AddScore(int amount)
    {
        AddScore(amount, 1f);
    }

    public void AddScore(int amount, float multiplier)
    {
        score += Mathf.RoundToInt(amount * multiplier);
        ScoreIncrease.Invoke();
    }

    public void ResetScore()
    {
        score = 0;
    }

    public void UpdateScoreText()
    {
        if (scoreText != null)
        {
            scoreText.SetText(score.ToString());
        }
    }
}
