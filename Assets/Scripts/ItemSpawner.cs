using UnityEngine;

public class ItemSpawner : MonoBehaviour
{
    public GameObject[] itemPrefabs; // Assign the item prefabs in the Inspector
    public Transform player; // Assign the player's Transform in the Inspector
    public int itemCount = 5; // Number of items to spawn
    public float gridSize = 10.0f; // Size of each grid square
    private float itemHeightOffset = 1f; // Height adjustment for items above the ground
    public Vector2 mazeSize = new Vector2(12, 12); // The full size of the maze (outer grid)

    void Start()
    {
        SpawnItems();
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
}
