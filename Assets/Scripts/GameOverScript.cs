using UnityEngine;

public class GameOverScript : MonoBehaviour
{
    public enum GameOverReason
    {
        LeftBorder,
        FlyKill,
        OutOfHealth
    }

    public GameOverReason reason;

    private LogicScript logic;

    void Start()
    {
        logic = GameObject.FindGameObjectWithTag("Logic").GetComponent<LogicScript>();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.layer == 6) 
        {
            logic.GameOver(reason);
        }
    }

    
}
