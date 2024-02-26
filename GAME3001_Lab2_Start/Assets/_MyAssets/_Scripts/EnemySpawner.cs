using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public GameObject enemyPrefab; // Prefab of the enemy ship
    public float spawnInterval = 2.0f; // Time interval between enemy spawns
    public float speed = 3.0f; // Adjust the enemy ship speed as needed
    public float minY = 4.3f; // Minimum Y position where enemies spawn
    public float maxY = 6.3f; // Maximum Y position where enemies spawn

    private float nextSpawnTime = 0f;

    void Update()
    {
        // Check if it's time to spawn a new enemy
        if (Time.time >= nextSpawnTime)
        {
            // Calculate a random X position within the screen width
            float randomX = Random.Range(-6.3f, 6.3f);

            // Create a new enemy ship at the random position
            GameObject newEnemy = Instantiate(enemyPrefab, new Vector3(randomX, maxY, 0), Quaternion.identity);

            // Set the enemy's speed
            EnemyShip enemyShipScript = newEnemy.GetComponent<EnemyShip>();
            if (enemyShipScript != null)
            {
                enemyShipScript.speed = speed;
            }

            // Schedule the next enemy spawn
            nextSpawnTime = Time.time + spawnInterval;
        }
    }
}
