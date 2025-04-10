using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class MathGameManager : MonoBehaviour
{
    [Header("UI Elements")]
    public TextMeshProUGUI equationText;
    public Button[] answerButtons;
    public TextMeshProUGUI timerText;
    public TextMeshProUGUI scoreText;
    public GameObject gameOverPanel;
    public TextMeshProUGUI finalScoreText;
    public TextMeshProUGUI feedbackText;

    [Header("Game Settings")]
    public int maxNumber = 10;
    public float gameTime = 60f;

    private int currentCorrectAnswer;
    private int score = 0;
    private float timeRemaining;
    private bool isGameActive = true;
    private int highScore = 0;

    void Start()
    {
        timeRemaining = gameTime;
        gameOverPanel.SetActive(false);
        highScore = PlayerPrefs.GetInt("HighScore", 0);
        GenerateNewEquation();
        UpdateScoreDisplay();
    }

    void Update()
    {
        if (!isGameActive) return;

        timeRemaining -= Time.deltaTime;
        if (timeRemaining < 0) timeRemaining = 0;
        UpdateTimerDisplay();

        if (timeRemaining == 0)
        {
            GameOver();
        }
    }

    void GenerateNewEquation()
    {
        int num1 = Random.Range(1, maxNumber + 1);
        int num2 = Random.Range(1, maxNumber + 1);
        string operation = "";
        int result = 0;

        switch (Random.Range(0, 4))
        {
            case 0:
                result = num1 + num2;
                operation = "+";
                break;
            case 1:
                result = num1 - num2;
                operation = "-";
                break;
            case 2:
                result = num1 * num2;
                operation = "×";
                break;
            case 3:
                while (num1 % num2 != 0)
                {
                    num2 = Random.Range(1, num1 + 1);
                }
                result = num1 / num2;
                operation = "÷";
                break;
        }

        equationText.text = $"{num1} {operation} {num2} = ?";
        currentCorrectAnswer = result;

        List<int> answers = new List<int> { result };
        while (answers.Count < 2)
        {
            int wrongAnswer = result + Random.Range(-3, 4);
            if (wrongAnswer != result && !answers.Contains(wrongAnswer))
                answers.Add(wrongAnswer);
        }

        for (int i = 0; i < answers.Count; i++)
        {
            int temp = answers[i];
            int randomIndex = Random.Range(i, answers.Count);
            answers[i] = answers[randomIndex];
            answers[randomIndex] = temp;
        }

        for (int i = 0; i < answerButtons.Length; i++)
        {
            TMP_Text text = answerButtons[i].GetComponentInChildren<TMP_Text>();
            text.text = answers[i].ToString();

            MathAnswerButton mathBtn = answerButtons[i].GetComponent<MathAnswerButton>();
            if (mathBtn != null)
            {
                mathBtn.answerValue = answers[i];
                mathBtn.gameManager = this;
            }

            AssignAnswer(answerButtons[i], answers[i]);
        }
    }

    void AssignAnswer(Button button, int value)
    {
        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(() => SelectAnswer(value));
    }

    public void SelectAnswer(int selectedAnswer)
    {
        if (!isGameActive) return;

        Debug.Log($"Button clicked with answer: {selectedAnswer}");

        if (selectedAnswer == currentCorrectAnswer)
        {
            score += 100;
            feedbackText.text = "Correct!";
            feedbackText.color = Color.green;
        }
        else
        {
            feedbackText.text = "Incorrect!";
            feedbackText.color = Color.red;
        }

        UpdateScoreDisplay();
        feedbackText.alpha = 1;
        StartCoroutine(HideFeedbackAfterDelay());
        GenerateNewEquation();
    }

    void UpdateScoreDisplay()
    {
        scoreText.text = $"Score: {score}";
    }

    void UpdateTimerDisplay()
    {
        timerText.text = $"Time: {Mathf.FloorToInt(timeRemaining)}s";
    }

    void GameOver()
    {
        isGameActive = false;
        gameOverPanel.SetActive(true);

        if (score > highScore)
        {
            highScore = score;
            PlayerPrefs.SetInt("HighScore", highScore);
            PlayerPrefs.Save();
        }

        finalScoreText.text = $"Final Score: {score}\nHigh Score: {highScore}";
    }

    public void RestartGame()
    {
        score = 0;
        timeRemaining = gameTime;
        isGameActive = true;
        gameOverPanel.SetActive(false);
        UpdateScoreDisplay();
        GenerateNewEquation();
    }

    IEnumerator HideFeedbackAfterDelay()
    {
        yield return new WaitForSeconds(0.5f);
        feedbackText.alpha = 0;
    }
}
