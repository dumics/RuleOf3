using UnityEngine;

public class BackgroundMoveScript : MonoBehaviour
{

    
    public float moveSpeed = 5f;
    public float deadZone = -30f; // x coordinate when destroying object
    public float spawnTrigger = 0f; // X coordinate to trigger next spawn
    public float spawnCoordinateY = 0f;
    

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        transform.position = transform.position + (Vector3.left * moveSpeed) * Time.deltaTime;

        if (transform.position.x < deadZone)
        {
            Destroy(gameObject);
        }
    }
}
