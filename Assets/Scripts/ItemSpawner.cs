using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;

public class ItemSpawner : MonoBehaviour
{
    [SerializeField] private GameObject[] itemPrefabs; // Assign the item prefabs in the Inspector
    [SerializeField] private GameObject[] enemyPrefabs;  
    [SerializeField] private Transform player; // Assign the player's Transform in the Inspector
    private int itemCount = 20; // Number of items to spawn
    private int enemyCount = 10;
    private float gridSize = 10.0f; // Size of each grid square
    private float itemHeightOffset = 0.1f; // Height adjustment for items above the ground
    private float enemyHeightOffset = 0f;
    private Vector2 mazeSize = new Vector2(12, 12); // The full size of the maze (outer grid)

    void Start()
    {
        SpawnItems();
        SpawnEnemies();
    }

    void SpawnItems()
    {
        int spawnAreaStartX = 1; // Start of the inner grid (avoiding outer corridor)
        int spawnAreaStartZ = 1;
        int spawnAreaEndX = (int)mazeSize.x - 2; // End of the inner grid
        int spawnAreaEndZ = (int)mazeSize.y - 2;

        for (int i = 0; i < itemCount; i++)
        {
            // Generate a random position in the inner 10x10 area
            int randomGridX = Random.Range(spawnAreaStartX, spawnAreaEndX + 1);
            int randomGridZ = Random.Range(spawnAreaStartZ, spawnAreaEndZ + 1);

            // Convert grid position to world position, centering within the grid square
            Vector3 itemPosition = new Vector3(
                (randomGridX * gridSize) + (gridSize / 2), // Center X
                itemHeightOffset, // Height adjustment to avoid being buried
                (randomGridZ * gridSize) + (gridSize / 2)  // Center Z
            );

            // Select a random item prefab
            GameObject randomItemPrefab = itemPrefabs[Random.Range(0, itemPrefabs.Length)];

            // Instantiate the item at the calculated position
            GameObject newItem = Instantiate(randomItemPrefab, itemPosition, Quaternion.identity);

            // Assign the player reference to the item's ItemPositioning script
            ItemPositioning itemPositioning = newItem.GetComponent<ItemPositioning>();
            if (itemPositioning != null)
            {
                itemPositioning.player = player; // Set the player reference
            }
        }

    }

        private void SpawnEnemies() {
            int spawnAreaStartX = 1;
            int spawnAreaStartZ = 1;
            int spawnAreaEndX = (int)mazeSize.x - 2;
            int spawnAreaEndZ = (int)mazeSize.y - 2;
            
            for (int i = 0; i < enemyCount; i++) {
                
                int randomGridX = Random.Range(spawnAreaStartX, spawnAreaEndX);
                int randomGridZ = Random.Range(spawnAreaStartZ, spawnAreaEndZ);

                Vector3 enemyPosition = new Vector3(
                    (randomGridX * gridSize) + (gridSize / 2), //Center X
                    enemyHeightOffset,
                    (randomGridZ * gridSize) + (gridSize / 2) //Center Z
                );

                GameObject randomEnemy = enemyPrefabs[Random.Range(0, enemyPrefabs.Length)];

                Instantiate(randomEnemy, enemyPosition, Quaternion.identity);


            }
        }

}
