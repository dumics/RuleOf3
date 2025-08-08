using UnityEngine;

public class StarScript : MonoBehaviour
{

    public LogicScript logic;
    public Animator animator;
        
   
    void Start()
    {
        logic = GameObject.FindGameObjectWithTag("Logic").GetComponent<LogicScript>();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if(other.gameObject.layer == 6)
        {
            logic.addScore(3);
            animator.SetTrigger("Collected");
            Destroy(gameObject,0.1f);
        }
    }
}
