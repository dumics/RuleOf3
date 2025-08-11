using UnityEngine;

public class StarScript : MonoBehaviour
{

    public int starAward = 30;

    private LogicScript logic;

    [SerializeField] private AudioClip starCollectedSound;
   
    void Start()
    {
        logic = GameObject.FindGameObjectWithTag("Logic").GetComponent<LogicScript>();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if(other.gameObject.layer == 6)
        { 
            Destroy(gameObject);
            logic.addScore(starAward);
            SoundManager.instance.PlaySound(starCollectedSound);
        }
    }
}
