using UnityEngine;
using System.Collections.Generic;

public class BackgroundSpawnerScript : MonoBehaviour
{
    [System.Serializable]
    public class BackgroundData
    {
        public GameObject prefab;
        public float spawnY;
        public float spawnZ;
        public float moveSpeed;

        [HideInInspector] public GameObject lastSpawned; // track the current instance
    }

    [Header("Background Pieces")]
    public List<BackgroundData> backgrounds;

    public float spawnTriggerX = 0f; // when last piece crosses this, spawn next
    public float pieceWidth = 30f;   // distance to place new piece ahead

    void Start()
    {
        // Spawn initial sequence
        foreach (BackgroundData bg in backgrounds)
        {
            bg.lastSpawned = SpawnPiece(bg, 0f);
        }
    }

    void Update()
    {
        foreach (BackgroundData bg in backgrounds)
        {
            if (bg.lastSpawned != null && bg.lastSpawned.transform.position.x <= spawnTriggerX)
            {
                // Spawn new one ahead of the current
                float newX = bg.lastSpawned.transform.position.x + pieceWidth;
                bg.lastSpawned = SpawnPiece(bg, newX);
            }
        }
    }

    GameObject SpawnPiece(BackgroundData bg, float spawnX)
    {
        Vector3 spawnPos = new Vector3(spawnX, bg.spawnY, bg.spawnZ);
        GameObject piece = Instantiate(bg.prefab, spawnPos, Quaternion.identity);

        BackgroundMoveScript moveScript = piece.GetComponent<BackgroundMoveScript>();
        if (moveScript != null)
        {
            moveScript.moveSpeed = bg.moveSpeed;
        }

        return piece;
    }
}
