using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ScoreManager : MonoBehaviour
{

    public static ScoreManager instance;
    public int scorePerCloud = 10;
    public float constantMultiplier = 1f;
    private float originalMultiplier;

    public UnityEvent ScoreIncrease = new UnityEvent();

    public TMPro.TextMeshProUGUI scoreText;


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
        originalMultiplier = constantMultiplier;
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

    public void MultiplierActive(float multiplier, float time)
    {
        float original = constantMultiplier;
        SetMultiplier(multiplier);
        Invoke("ResetMultiplier", time);
    }

    private void SetMultiplier(float multiplier)
    {
        constantMultiplier = multiplier;
    }

    private void ResetMultiplier()
    {
        constantMultiplier = originalMultiplier;
    }


    public void AddScore(int amount)
    {
        AddScore(amount, constantMultiplier);
    }

    public void AddScore()
    {
        AddScore(scorePerCloud, constantMultiplier);
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
