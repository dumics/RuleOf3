using Unity.VisualScripting;
using UnityEngine;

public class StarScript : MonoBehaviour
{

    public int starAward = 30;

    public LogicScript logic;
    public Animator animator;

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
            //animator.SetTrigger("Collected");
        }
    }
}
