using UnityEngine;

public class EnemyScript : MonoBehaviour
{

    [Header("Info")]
    [SerializeField] private float health = 10f;
    [SerializeField] private int pointsAward = 10;
    [SerializeField] private float damage = 1f;
    [SerializeField] public float moveSpeed = 3f;
    [SerializeField] private float knockbackForce = 5f;
    [SerializeField] private float knockbackDuration = 0.3f;
    [SerializeField] private bool killOnContact;

    private bool takingDamage = false;

    [Header("Sizing")]
    public float minSize = 0;
    public float maxSize = 0;
    public float spawnY = 0;

    private Rigidbody2D rb;
    private Animator animator;
    private LogicScript logic;
    public AudioClip hurt;

    private bool isKnockedBack = false;
    private float knockbackTimer;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        if (rb == null)
        {
            Debug.LogWarning("Enemy has no Rigidbody2D! Knockback won't work.");
        }
    }    

    void Start()
    {
        logic = GameObject.FindGameObjectWithTag("Logic").GetComponent<LogicScript>();
    }

    void Update()
    {
        if (health <= 0)
        {
            animator.SetTrigger("die");
            return;
        }

        // If not in knockback, move normally
        if (!isKnockedBack)
        {
            transform.position += Vector3.left * moveSpeed * Time.deltaTime;
        }
        else
        {
            knockbackTimer -= Time.deltaTime;
            if (knockbackTimer <= 0)
            {
                isKnockedBack = false;
                rb.linearVelocity = Vector2.zero; // stop knockback
            }
        }
    }

    public void TakeDamage(float damage, Vector2 hitDirection)
    {
        health -= damage;
        //Debug.Log("Damage taken!!");
        SoundManager.instance.PlaySound(hurt);
        animator.SetBool("hit", true);
        takingDamage = true;

        // Apply knockback
        if (rb != null)
        {
            isKnockedBack = true;
            knockbackTimer = knockbackDuration;
            rb.linearVelocity = Vector2.zero; // clear old velocity
            rb.AddForce(hitDirection.normalized * knockbackForce * moveSpeed, ForceMode2D.Impulse);
        }

    }

    public void hitDone()
    {
        animator.SetBool("hit", false);
        takingDamage = false;
    }

    public void dieDone()
    {
        Debug.Log("enemy dead");
        logic.addScore(pointsAward + (int)moveSpeed);
        SoundManager.instance.PlaySound(hurt);
        animator.ResetTrigger("die");
        
        Destroy(gameObject);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Collision with player
        if (collision.gameObject.layer == 6)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player == null) return;

            PlMov2 scr = player.GetComponent<PlMov2>();
            if (scr == null) return;

            // Cannot hurt player while enemy taking damage
            if(!takingDamage) scr.TakeDamage(killOnContact ? 100 : damage);

        }
    }

}
