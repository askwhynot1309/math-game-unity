using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class MathGameManager : MonoBehaviour
{
    public AstraInputController inputController;
    public GameObject restartButton;
    private FootDetector restartDetector;

    [Header("UI Elements")]
    public TextMeshProUGUI equationText;
    public Button[] answerButtons;
    public TextMeshProUGUI timerText;
    public TextMeshProUGUI scoreText;
    public GameObject gameOverPanel;
    public TextMeshProUGUI finalScoreText;
    public TextMeshProUGUI feedbackText;
    public TextMeshProUGUI highscoreText;

    [Header("Game Settings")]
    public int maxNumber = 10;
    public float gameTime = 60f;

    private int currentCorrectAnswer;
    private int score = 0;
    private float timeRemaining;
    private bool isGameActive = true;

    void Start()
    {
        SoundManager.Instance.PlayMusic();
        timeRemaining = gameTime;
        gameOverPanel.SetActive(false);
        GenerateNewEquation();
        UpdateScoreDisplay();
        StartCoroutine(GameAPI.Instance.GetHighScore(
        score =>
        {
            Debug.Log("Fetched high score: " + score);
            highscoreText.text = "Highscore: " + score.ToString();
        },
        error =>
        {
            Debug.LogError("Failed to fetch high score: " + error);
        }));

        if (inputController == null)
        {
            inputController = FindFirstObjectByType<AstraInputController>();
        }
        if (inputController != null)
        {
            inputController.OnClickEvent.AddListener(HandleFootClick);
        }

        restartDetector = restartButton.GetComponent<FootDetector>();
    }

    void HandleFootClick()
    {
        if (!isGameActive && restartDetector != null && restartDetector.IsFootOver && !restartDetector.hasClicked)
        {
            restartDetector.hasClicked = true;
            RestartGame();
        }
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

        foreach (Button button in answerButtons)
        {
            button.interactable = false;
        }
        Debug.Log($"Button clicked with answer: {selectedAnswer}");

        if (selectedAnswer == currentCorrectAnswer)
        {
            score += 100;
            feedbackText.text = "Correct!";
            feedbackText.color = Color.green;
            SoundManager.Instance.PlayCorrect();
        }
        else
        {
            feedbackText.text = "Incorrect!";
            feedbackText.color = Color.red;
            SoundManager.Instance.PlayWrong();
        }

        UpdateScoreDisplay();
        feedbackText.alpha = 1;
        StartCoroutine(HideFeedbackAfterDelay());
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

        StartCoroutine(GameAPI.Instance.PostPlayHistory(score,
                    onSuccess: () => {
                        Debug.Log("Score posted successfully.");
                    },
                    onError: (error) => {
                        Debug.LogError($"Failed to post score: {error}");
                    }));

        finalScoreText.text = $"Final Score: {score}";

    }

    public void RestartGame()
    {
        score = 0;
        timeRemaining = gameTime;
        isGameActive = true;
        gameOverPanel.SetActive(false);
        UpdateScoreDisplay();
        GenerateNewEquation();
        SoundManager.Instance.PlayMusic();
        StartCoroutine(GameAPI.Instance.GetHighScore(
        score =>
        {
            Debug.Log("Fetched high score: " + score);
            highscoreText.text = "Highscore: " + score.ToString();
        },
        error =>
        {
            Debug.LogError("Failed to fetch high score: " + error);
        }));
        //SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    IEnumerator HideFeedbackAfterDelay()
    {
        yield return new WaitForSeconds(0.5f);
        feedbackText.alpha = 0;
        GenerateNewEquation();
        foreach (Button button in answerButtons)
        {
            button.interactable = true;
        }
    }
}
