using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LogicScript : MonoBehaviour
{
    public int playerScore;
    public TextMeshProUGUI scoreText;


    [ContextMenu("Increase Score")]
    public void addScore(int score)
    {
        playerScore += score;
        scoreText.text = "Score: " + playerScore.ToString();
    }

}
