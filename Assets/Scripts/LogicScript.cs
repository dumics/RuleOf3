using UnityEngine;
using UnityEngine.UI;
using TMPro;
using static UnityEditor.Experimental.GraphView.GraphView;

public class LogicScript : MonoBehaviour
{
    public int playerScore;
    private int highScore;

    [Header("")]
    [SerializeField] public BlockSpawnerScript blockSpawner;
    [SerializeField] private GameObject gameOverPanel;

    [Header("Optional UI")]
    public TextMeshProUGUI timerText; // Link in Inspector if you want to show timer
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI highScoreText;

    private bool gameOver = false;
    private float gameTime = 0f;

    private GameObject player;
    private PlMov2 playerSC;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        playerSC = player.GetComponent<PlMov2>();
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

        timerText.text = "";
        scoreText.text = "";

        gameOverPanel.SetActive(true);

        if (blockSpawner != null) blockSpawner.StopSpawner();

        //PlMov2 playerSC = player.GetComponent<PlMov2>();

        string infoText = "";

        switch (reason)
        {
            case GameOverScript.GameOverReason.LeftBorder:

                infoText= "The player is fell out.";
                playerSC.TakeDamage(100); 
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

    public void RestartGame()
    {
        gameOver = false;
        
        blockSpawner.StartSpawner();
        playerSC.RestartPlayer();

        gameOverPanel.SetActive(false);
        gameTime = 0f;
        playerScore = 0;
    }

    public void MainMainu()
    {
        SceneControler.instance.LoadSceneByIndex(0);
    }

    public void StartGame()
    {
        SceneControler.instance.LoadSceneByIndex(1);
    }

}
