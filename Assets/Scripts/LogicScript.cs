using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEditor.SearchService;
using Mono.Cecil;

public class LogicScript : MonoBehaviour
{

    [Header("Prefabs for Gameplay Scene")]
    public GameObject playerPrefab;
    public GameObject blockSpawnerPrefab;

    private GameObject playerInstance;
    private GameObject blockSpawnerInstance;

    [Header("Optional UI")]
    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI highScoreText;
    [SerializeField] private TextMeshProUGUI healthText;
    [SerializeField] private GameObject gameOverPanel;

    [SerializeField] private bool gameOver = false;
    private float gameTime = 0f;
    private int playerScore;
    private int highScore;

    private void Awake()
    {
        // Player
        if (playerPrefab != null && playerInstance == null)
            playerInstance = Instantiate(playerPrefab, new Vector3(-5, -4, 1), Quaternion.identity);

        // Block spawner
        if (blockSpawnerPrefab != null && blockSpawnerInstance == null)
                blockSpawnerInstance = Instantiate(blockSpawnerPrefab, new Vector3(20, 0, 0), Quaternion.identity);

    }

    private void Start()
    {
        highScore = PlayerPrefs.GetInt("HighScore");
        highScoreText.text = "HighScore: " + highScore.ToString();
        
    }

    void Update()
    {
        if (!gameOver)
        {
            // GAME TIMER
            gameTime += Time.deltaTime;
            if (timerText != null)
            {
                timerText.text = "Time: " + Mathf.FloorToInt(gameTime).ToString();
            }
        }
    }


    [ContextMenu("Increase Score")]
    public void addScore(int score)
    {
        playerScore += score;
        scoreText.text = "Score: " + playerScore.ToString();

        if (playerScore > PlayerPrefs.GetInt("HighScore"))
        {
            PlayerPrefs.SetInt("HighScore", playerScore);
            highScoreText.text = "HighScore: " + PlayerPrefs.GetInt("HighScore", playerScore).ToString();
        }
    }

    public void GameOver(GameOverScript.GameOverReason reason)
    {
        gameOver = true;

        blockSpawnerInstance.GetComponent<BlockSpawnerScript>().StopSpawner();

        timerText.text = "";
        scoreText.text = "";

        gameOverPanel.SetActive(true);



        string infoText = "";

        switch (reason)
        {
            case GameOverScript.GameOverReason.LeftBorder:

                infoText = "The player is fell out.";
                playerInstance.GetComponent<PlMov2>().BorderDamage();
                break;

            case GameOverScript.GameOverReason.FlyKill:

                infoText = "The player is killed by fly.";
                break;

            case GameOverScript.GameOverReason.OutOfHealth:

                infoText = "The player is out of health.";
                break;
        }

        TextMeshProUGUI[] texts = gameOverPanel.GetComponentsInChildren<TextMeshProUGUI>(true);
        foreach (TextMeshProUGUI t in texts)
        {
            if (t.name == "Score") t.text = "Score: " + playerScore.ToString();
            else if (t.name == "Time") t.text = "Time: " + Mathf.FloorToInt(gameTime).ToString();
            else if (t.name == "Text") t.text = infoText;
            else if (t.name == "HighScore") t.text = "HighScore: " + PlayerPrefs.GetInt("HighScore", playerScore).ToString();
        }



    }

    public void ChangeHealth(float health)
    {
        healthText.text = "Health: " + health.ToString() + " / 100";
    }

    public void RestartGame()
    {

        Debug.Log("Function called!!");
        gameOver = false;

        blockSpawnerInstance.GetComponent<BlockSpawnerScript>().StartSpawner();
        playerInstance.GetComponent<PlMov2>().RestartPlayer();

        gameOverPanel.SetActive(false);
        gameTime = 0f;
        playerScore = 0;
    }



}
