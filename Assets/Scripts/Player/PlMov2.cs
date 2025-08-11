using UnityEngine;

public class PlMov2 : MonoBehaviour
{
    [Header("Movement Parameters")]
    [SerializeField] private float speed;
    [SerializeField] private float jumpPower;
    [SerializeField] private float size;
    private float horizontalInput;
    private int facingRight;

    [Header("Healt")]
    [SerializeField] public float health;
    [SerializeField] private bool dead = false;

    [Header("Attack")]
    [SerializeField] private float damage;
    [SerializeField] private float attackSpeed = 0.6f;
    [SerializeField] private float attackRange;
    [SerializeField] private float attackCooldown = 2f; // 2s cooldown
    [SerializeField] private float attackCooldownTimer;

    [HideInInspector] private bool isAttacking = false;
    [HideInInspector] private float attackTimer = 0f;
    [HideInInspector] private bool freezRotation = false; // freeze rotation during attack

    [SerializeField] public Transform attackPos;
    [SerializeField] public LayerMask whatIsEnemies;

    [Header("Coyote Time")]
    [SerializeField] private float coyoteTime; //How much time the player can hang in the air before jumping
    private float coyoteCounter; //How much time passed since the player ran off the edge

    [Header("Multiple Jumps")]
    [SerializeField] private int extraJumps;
    private int jumpCounter;

    [Header("Layers")]
    [SerializeField] private LayerMask groundLayer;

    #region Sound
    [Header("Sound")]
    [SerializeField] private AudioClip jumpSound;
    [SerializeField] private AudioClip deadSound;
    [SerializeField] private AudioClip attackSound;
    #endregion

    #region Spawn position
    private float spawnPosX = -5;
    private float spawnPosY = -4; 
    private float spawnPosZ = 1;
    #endregion

    private Rigidbody2D body;
    private Animator anim;
    private BoxCollider2D boxCollider;
    private LogicScript logic;

    private void Awake()
    {
        //Grab references for rigidbody and animator from object
        body = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        boxCollider = GetComponent<BoxCollider2D>();
        logic = GameObject.FindGameObjectWithTag("Logic").GetComponent<LogicScript>();
    }

    private void Start()
    {
        logic.ChangeHealth(health);
    }

    private void Update()
    {
        if (dead) return; // jump out if player is dead

        horizontalInput = Input.GetAxis("Horizontal");

        //Flip player when moving left-right
        if (horizontalInput > 0.01f && !freezRotation)
        {
            transform.localScale = new Vector3(size, size, 1);
            facingRight = 1;
        }
        else if (horizontalInput < -0.01f && !freezRotation)
        {
            transform.localScale = new Vector3(-size, size, 1);
            facingRight = -1;
        }

        #region Animator parameters
        //Set animator parameters
        anim.SetBool("run", horizontalInput != 0);
        anim.SetBool("grounded", isGrounded());
        anim.SetFloat("jump", body.linearVelocityY);
        #endregion

        #region Jump
        //Jump
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.W))
            Jump();

        //Adjustable jump height
        if ((Input.GetKeyUp(KeyCode.Space) || Input.GetKeyUp(KeyCode.W)) && body.linearVelocityY > 0)
            body.linearVelocity = new Vector2(body.linearVelocityX, body.linearVelocityY / 2);
         


        body.gravityScale = 7;
        body.linearVelocity = new Vector2(horizontalInput * speed, body.linearVelocityY);

        if (isGrounded())
        {
            coyoteCounter = coyoteTime; //Reset coyote counter when on the ground
            jumpCounter = extraJumps; //Reset jump counter to extra jump value
        }
        else
            coyoteCounter -= Time.deltaTime; //Start decreasing coyote counter when not on the ground
        #endregion  

        // Attack
        if (Input.GetKeyDown(KeyCode.F) && attackCooldownTimer <= 0)
        {
            Attack();
            attackCooldownTimer = attackCooldown;
        }
        else
        {
            if (attackCooldownTimer > 0)
            {
                attackCooldownTimer -= Time.deltaTime;
            }
        }

    }

    private void Jump()
    {
        if (coyoteCounter <= 0 && jumpCounter <= 0) return;
        //If coyote counter is 0 or less and don't have any extra jumps don't do anything

        SoundManager.instance.PlaySound(jumpSound);


        if (isGrounded())
            body.linearVelocity = new Vector2(body.linearVelocityX, jumpPower);
        else
        {
            //If not on the ground and coyote counter bigger than 0 do a normal jump
            if (coyoteCounter > 0)
                body.linearVelocity = new Vector2(body.linearVelocityX, jumpPower);
            else
            {
                if (jumpCounter > 0) //If we have extra jumps then jump and decrease the jump counter
                {
                    body.linearVelocity = new Vector2(body.linearVelocityX, jumpPower);
                    jumpCounter--;
                }
            }
        }

        //Reset coyote counter to 0 to avoid double jumps
        coyoteCounter = 0;
    }

    private bool isGrounded()
    {
        RaycastHit2D raycastHit = Physics2D.BoxCast(boxCollider.bounds.center, boxCollider.bounds.size, 0, Vector2.down, 0.1f, groundLayer);
        return raycastHit.collider != null;
    }

    public bool canAttack()
    {
        return horizontalInput == 0 && isGrounded();
    }

    public void Attack()
    {
        if (isAttacking) return; // prevent spam
        freezRotation = true;
        isAttacking = true;
        anim.SetTrigger("attack");
        SoundManager.instance.PlaySound(attackSound);
        DealDamage();
    }

    public void EndAttack()
    {
        isAttacking = false;
        anim.ResetTrigger("attack");
        freezRotation = false;
        UnityEngine.Debug.Log("EndAttack();");
    }

    public void DealDamage()
    {
        UnityEngine.Debug.Log("DealDamage();");
        
        // Attack logic
        Collider2D[] enemiesToDamage = Physics2D.OverlapCircleAll(attackPos.position, attackRange, whatIsEnemies);

        for (int i = 0; i < enemiesToDamage.Length; i++)
        {
            // all enemy's to take damage 
            enemiesToDamage[i].GetComponent<EnemyScript>().TakeDamage(damage, (enemiesToDamage[i].transform.position - transform.position).normalized);
        }
    }

    public void TakeDamage(float damage)
    {
        health -= damage;
        logic.ChangeHealth(health); // update health on screen
        if (health <= 0f)
        {
            SoundManager.instance.PlaySound(deadSound);
            dead = true;
            logic.GameOver(GameOverScript.GameOverReason.OutOfHealth);
        }
    }

    public void BorderDamage()
    {
        health = 0;
        logic.ChangeHealth(health);
        SoundManager.instance.PlaySound(deadSound);
        dead = true;
    }

    public void RestartPlayer()
    {
        Vector3 spawnPosition = new Vector3(spawnPosX, spawnPosY, spawnPosZ);
        body.transform.position = spawnPosition;
        health = 100f;
        logic.ChangeHealth(health);
        dead = false;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(attackPos.position, attackRange);
    }
}
