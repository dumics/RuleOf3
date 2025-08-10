using UnityEngine;

public class EnemyScript : MonoBehaviour
{
    [SerializeField] private float health;
    [SerializeField] private float damage;

    void Update()
    {
        if(health <= 0)
        {
            Destroy(gameObject);
        }
    }

    public void TakeDamage(float damage)
    {
        health -= damage;
        Debug.Log("Damage taken!!");
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.layer == 6)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                PlMov2 scr = player.GetComponent<PlMov2>();
                if (scr != null)
                {
                    scr.TakeDamage(damage);
                }
            }
        }
    }

}
