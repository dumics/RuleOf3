using UnityEngine;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UIElements;

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
    
    private List<GameObject> spawnedObjects = new List<GameObject>();



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

        float lowestPoint = -3.5f;
        float highestPoint = 1f;
        float randomY = Random.Range(lowestPoint, highestPoint);

        Vector3 spawnPos = new Vector3(transform.position.x, randomY, 0);

        SpawnRandomizedBlock(spawnPos);
    }

    void SpawnRandomizedBlock(Vector3 basePosition)
    {
        int randomIndex = Random.Range(0, blockPrefabs.Count);
        GameObject prefab = blockPrefabs[randomIndex];

        if (prefab.CompareTag("Enemy"))
            SpawnEnemy(prefab, basePosition);
        else
            SpawnBlock(prefab, basePosition);
    }

    #region Generating
    void SpawnEnemy(GameObject enemy, Vector3 basePosition)
    {
        EnemyScript enemyScript = enemy.GetComponent<EnemyScript>();
        float scale = Random.Range(enemyScript.minSize, enemyScript.maxSize);


        Vector3 spawnPosition = new Vector3(basePosition.x, enemyScript.spawnY == 0 ? basePosition.y : enemyScript.spawnY , basePosition.z);
        GameObject instance = Instantiate(enemy, spawnPosition, Quaternion.identity);
        spawnedObjects.Add(instance);

        instance.GetComponent<EnemyScript>().moveSpeed = blockSpeed;
        instance.transform.localScale = new Vector3(scale, scale, 1f);

        SpawnStar(instance, scale);

    }

    void SpawnBlock(GameObject block, Vector3 basePosition)
    {
        BlockMoveScript blockScript = block.GetComponent<BlockMoveScript>();
        float scale = Random.Range(blockScript.minSize, blockScript.maxSize);

        Vector3 spawnPosition = new Vector3(basePosition.x, basePosition.y, basePosition.z);
        GameObject instance = Instantiate(block, spawnPosition, Quaternion.identity);
        spawnedObjects.Add(instance);

        instance.GetComponent<BlockMoveScript>().moveSpeed = blockSpeed;
        instance.transform.localScale = new Vector3(scale, scale, 1f);

        float[] angles = { 0f, 15f, 30f, 45f, 60f };
        float randomZRotation = angles[Random.Range(0, angles.Length)];
        instance.transform.rotation = Quaternion.Euler(0f, 0f, randomZRotation);

        SpawnStar(instance, scale);

    }

    void SpawnStar(GameObject instance, float scale)
    {
        if (starPrefab != null && Random.value < starSpawnChance)
        {
            Vector3 starPosition = new Vector3(
                instance.transform.position.x,
                instance.transform.position.y + (scale * 0.5f) + starOffsetY,
                instance.transform.position.z
            );

            GameObject starObj = Instantiate(starPrefab, starPosition, Quaternion.identity);
            starObj.GetComponent<BlockMoveScript>().moveSpeed = blockSpeed;
            spawnedObjects.Add(starObj);
        }
    }
    #endregion
    public void StopSpawner()
    {

        foreach (GameObject obj in spawnedObjects)
        {
            if (obj != null)
                Destroy(obj);
        }
        spawnedObjects.Clear(); 

        gameOver = true;
    }

    public void StartSpawner()
    {
        gameOver = false;
    }

}
