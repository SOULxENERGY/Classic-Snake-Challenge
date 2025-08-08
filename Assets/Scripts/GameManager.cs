using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManager : MonoBehaviour
{
    [SerializeField] private TMP_Text pauseAndPlayText;
    [SerializeField] private TMP_Text scoreText;
    [SerializeField] private TMP_Text highScoreText;
    [SerializeField] private GameObject deathMessage;

    private float score;
    private float highScore;

    private const string HighScoreKey = "HighScore";

    private void Start()
    {
        if (!pauseAndPlayText)
        {
            Debug.LogError("Assign pause and play text from inspector");
        }

        Time.timeScale = 0;
        pauseAndPlayText.text = "PLAY";

        // Load saved high score
        highScore = PlayerPrefs.GetFloat(HighScoreKey, 0);

        //setting the score in ui
        scoreText.text = $"Score: {score}";
        highScoreText.text = $"HighScore: {highScore}";

        // Subscribe to snake's eat event
        Snake.singleton.eatEvent += (float scoreValue) =>
        {
            score = scoreValue;

            if (score > highScore)
            {
                highScore = score;
            }

            scoreText.text = $"Score: {score}";
            highScoreText.text = $"HighScore: {highScore}";
        };

        Snake.singleton.deathEvent += () =>
        {
            deathMessage.SetActive(true);
        };
    }

    public void Restart()
    {
        deathMessage.SetActive(false);
        SaveHighScore();
        SceneManager.LoadScene(0);
    }

    private void OnApplicationQuit()
    {
        SaveHighScore();
    }

    private void OnDestroy()
    {
        SaveHighScore();
    }

    private void SaveHighScore()
    {
        PlayerPrefs.SetFloat(HighScoreKey, highScore);
        PlayerPrefs.Save();
    }

    public void PauseAndPlay()
    {
        if (Time.timeScale > 0.005f)
        {
            Time.timeScale = 0;
            pauseAndPlayText.text = "PLAY";
        }
        else
        {
            Time.timeScale = 1;
            pauseAndPlayText.text = "PAUSE";
        }
    }
}
