using UnityEngine;
using System.Collections.Generic;
using TMPro;

public class BlockSpawnerScript : MonoBehaviour
{

    [Header("Block & Star")]
    public List<GameObject> blockPrefabs;
    public GameObject starPrefab;
    public float starSpawnChance = 0.4f;
    public float starOffsetY = 1.5f;

    [Header("Spawning Timing")]
    public float spawnRate = 2f;
    public float minSpawnRate = 1.4f;
    public float spawnRateDecrease = 0.1f;
    public float difficultyIncreaseRate = 15f;

    [Header("Block's speed")]
    public float blockSpeed = 5f;
    public float maxBlockSpeed = 25f;

    
    private float timer = 0f;
    private float difficultyTimer = 0f;
    private bool gameOver = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {

        if (!gameOver)
        {
            // BLOCK SPAWNING
            if (timer < spawnRate)
            {
                timer += Time.deltaTime;
            }
            else
            {
                SpawnBlock();
                timer = 0;
            }

            // DIFFICULTY INCREASE
            difficultyTimer += Time.deltaTime;
            if (difficultyTimer >= difficultyIncreaseRate)
            {
                spawnRate = Mathf.Max(minSpawnRate, spawnRate - spawnRateDecrease);
                blockSpeed = Mathf.Min(maxBlockSpeed, blockSpeed + 1);
                difficultyTimer = 0;
            }
        }
    }

    void SpawnBlock()
    {
        if (blockPrefabs == null || blockPrefabs.Count == 0)
        {
            Debug.LogWarning("No blocks assigned to blockPrefabs list!");
            return;
        }

        float lowestPoint = -4.5f;
        float highestPoint = 0.5f;
        float randomY = Random.Range(lowestPoint, highestPoint);

        Vector3 spawnPos = new Vector3(transform.position.x, randomY, 0);

        SpawnRandomizedBlock(spawnPos);
    }

    void SpawnRandomizedBlock(Vector3 basePosition)
    {
        int randomIndex = Random.Range(0, blockPrefabs.Count);
        GameObject prefab = blockPrefabs[randomIndex];

        BlockMoveScript blockMoveScript = prefab.GetComponent<BlockMoveScript>();

        float scale = Random.Range(blockMoveScript.minSize, blockMoveScript.maxSize);
        
        BlockMoveScript moveScript = prefab.GetComponent<BlockMoveScript>();
        moveScript.moveSpeed = blockSpeed;

        Vector3 spawnPosition = new Vector3(basePosition.x, basePosition.y, basePosition.z);
        GameObject instance = Instantiate(prefab, spawnPosition, Quaternion.identity);
        
        if (!instance.CompareTag("Enemy"))
        {
            float[] angles = { 0f, 15f, 30f, 45f, 60f };
            float randomZRotation = angles[Random.Range(0, angles.Length)];
            instance.transform.rotation = Quaternion.Euler(0f, 0f, randomZRotation);
        }

        instance.transform.localScale = new Vector3(scale, scale, 1f);

        #region Star Spawn
        if (starPrefab != null && Random.value < starSpawnChance)
        {
            Vector3 starPosition = new Vector3(
                instance.transform.position.x,
                instance.transform.position.y + (scale * 0.5f) + starOffsetY,
                instance.transform.position.z
            );

            Instantiate(starPrefab, starPosition, Quaternion.identity);
            BlockMoveScript starMoveScript = starPrefab.GetComponent<BlockMoveScript>();
            starMoveScript.moveSpeed = blockSpeed;
        }
        #endregion

    }

    public void StopSpawner()
    {
        gameOver = true;
    }

    public void StartSpawner()
    {
        gameOver = false;
    }

}
