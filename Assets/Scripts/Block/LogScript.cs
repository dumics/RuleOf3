using UnityEngine;

public class LogScript : MonoBehaviour
{
    public LogicScript logic;

    private float timer = 0f;
    private float timeToGainPoint = 0.5f;
    private bool playerOnLog = false;

    void Start()
    {
        logic = GameObject.FindGameObjectWithTag("Logic").GetComponent<LogicScript>();
    }

    void Update()
    {
        if (playerOnLog)
        {
            timer += Time.deltaTime;

            if (timer >= timeToGainPoint)
            {
                logic.addScore(1); 
                timer = 0f;
            }
        }
    }


    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.layer == 6)
        {

            playerOnLog = true;
            timer = 0f;

        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.layer == 6) // Player layer
        {
            playerOnLog = false;
            timer = 0f; 
        }
    }
}
