using System.Collections.Generic;
using UnityEngine;

public class ItemSpawner : MonoBehaviour
{
    [SerializeField] private GameObject[] itemPrefabs; // Assign item prefabs in the Inspector
    [SerializeField] private GameObject[] enemyPrefabs; // Assign enemy prefabs in the Inspector
    [SerializeField] private Transform player; // Assign player's Transform in the Inspector
    private int itemCount = 7; // Number of items to spawn
    private int enemyCount = 7; // Number of enemies to spawn
    private float gridSize = 10.0f; // Size of each grid square
    private float itemHeightOffset = 0.1f; // Height adjustment for items above the ground
    private float enemyHeightOffset = 0f; // Height adjustment for enemies
    private Vector2 mazeSize = new Vector2(12, 12); // The full size of the maze (outer grid)

    // Keep track of occupied grid positions
    private HashSet<Vector2Int> occupiedGridPositions = new HashSet<Vector2Int>();
    private const int maxSpawnAttempts = 100; // Limit to prevent infinite loops

    void Start()
    {
        Debug.Log($"Maze Size: {mazeSize}");
        SpawnItems();
        SpawnEnemies();
    }

    void SpawnItems()
    {
        SpawnObjects(itemPrefabs, itemCount, itemHeightOffset, "Item");
    }

    void SpawnEnemies()
    {
        SpawnObjects(enemyPrefabs, enemyCount, enemyHeightOffset, "Enemy");
    }

    private void SpawnObjects(GameObject[] prefabs, int count, float heightOffset, string type)
    {
        // Correct spawning area for the inner grid (10x10)
        int spawnAreaStartX = 1;
        int spawnAreaStartZ = 1;
        int spawnAreaEndX = (int)mazeSize.x - 2;
        int spawnAreaEndZ = (int)mazeSize.y - 2;

        Debug.Log($"Spawning {type}s in range X: [{spawnAreaStartX}, {spawnAreaEndX}], Z: [{spawnAreaStartZ}, {spawnAreaEndZ}]");

        for (int i = 0; i < count; i++)
        {
            Vector2Int randomGridPosition;
            int attempts = 0;

            // Ensure the position is not already occupied
            do
            {
                if (attempts >= maxSpawnAttempts)
                {
                    Debug.LogWarning($"Max spawn attempts reached for {type}s. Skipping {type} spawn.");
                    return;
                }

                // Generate random grid position within adjusted spawn range
                int randomGridX = Random.Range(spawnAreaStartX, spawnAreaEndX + 1);
                int randomGridZ = Random.Range(spawnAreaStartZ, spawnAreaEndZ + 1);
                randomGridPosition = new Vector2Int(randomGridX, randomGridZ);
                attempts++;
            } while (IsPositionOccupied(randomGridPosition));

            Debug.Log($"{type} {i + 1} spawned at grid {randomGridPosition}");

            // Mark the grid position as occupied
            MarkGridAsOccupied(randomGridPosition);

            // Convert grid position to world position, centering within the grid square
            Vector3 worldPosition = new Vector3(
                randomGridPosition.x * gridSize + gridSize * 0.5f, // Center X
                heightOffset, // Height adjustment
                randomGridPosition.y * gridSize + gridSize * 0.5f  // Center Z
            );

            Debug.Log($"{type} {i + 1} world position: {worldPosition}");

            // Select a random prefab
            GameObject randomPrefab = prefabs[Random.Range(0, prefabs.Length)];

            // Instantiate the object at the calculated position
            Instantiate(randomPrefab, worldPosition, Quaternion.identity);
        }
    }

    private bool IsPositionOccupied(Vector2Int position)
    {
        return occupiedGridPositions.Contains(position);
    }

    private void MarkGridAsOccupied(Vector2Int position)
    {
        occupiedGridPositions.Add(position);
    }
}
