using System.Collections.Generic;
using UnityEngine;

public class FloorManager : MonoBehaviour
{
    // [SerializeField] private GameObject[] itemPrefabs; // Assign item prefabs in the Inspector
    // [SerializeField] private GameObject[] enemyPrefabs; // Assign enemy prefabs in the Inspector
    // [SerializeField] private Transform player; // Assign player's Transform in the Inspector
    // private int itemCount = 8; // Number of items to spawn
    // private int enemyCount = 8; // Number of enemies to spawn
    [SerializeField] private GameObject[] warMonsterPrefabs;       // Green: Warriors, beasts, etc.
    [SerializeField] private GameObject[] warItemPrefabs;         // Green: Swords, shields, etc.
    [SerializeField] private GameObject[] spiritualMonsterPrefabs; // Blue: Ghosts, spirits, etc.
    [SerializeField] private GameObject[] spiritualItemPrefabs;   // Blue: Spellbooks, wands, etc.
    [SerializeField] private GameObject[] mixedMonsterPrefabs;    // Tan: Mix of both
    [SerializeField] private GameObject[] mixedItemPrefabs;       // Tan: Mix of both
    [SerializeField] private GameObject westDoorBlue;
    [SerializeField] private GameObject westDoorTan;
    [SerializeField] private GameObject westDoorGreen;
    [SerializeField] private GameObject eastDoorBlue;
    [SerializeField] private GameObject eastDoorTan;
    [SerializeField] private GameObject eastDoorGreen;
    [SerializeField] private GameObject[] westLadders;
    [SerializeField] private GameObject[] eastLadders;

    //[SerializeField] private Transform player; // Assign in Inspector

    private float gridSize = 10.0f; // Size of each grid square
    private float itemHeightOffset = 0.1f; // Height adjustment for items above the ground
    private float enemyHeightOffset = 0f; // Height adjustment for enemies
    private Vector2 mazeSize = new Vector2(12, 12); // The full size of the maze (outer grid)

    // Keep track of occupied grid positions
    private HashSet<Vector2Int> occupiedGridPositions = new HashSet<Vector2Int>();
    private const int maxSpawnAttempts = 100; // Limit to prevent infinite loops

    void Start()
    {
        //Debug.Log($"Maze Size: {mazeSize}");
        //SpawnItems();
        //SpawnEnemies();
    }

    void SpawnItems()
    {
        //SpawnObjects(itemPrefabs, itemCount, itemHeightOffset, "Item", true);
    }

    void SpawnEnemies()
    {
        //SpawnObjects(enemyPrefabs, enemyCount, enemyHeightOffset, "Enemy", false);
    }

    // Called by MazeGenerator after player placement
    public void GenerateFloorContents(BlockColorType blockColor, Vector2Int startPosition)
    {
        Debug.Log("From GenerateFloorContents in FloorManager in block Color: " + blockColor + " At Start Position: " + startPosition);
        int itemCount, enemyCount;
        GameObject[] itemPrefabs, enemyPrefabs;

        // Set counts and prefabs based on block color
        switch (blockColor)
        {
            case BlockColorType.Blue: // Spiritual
                itemCount = 4;
                enemyCount = 3;
                itemPrefabs = spiritualItemPrefabs;
                enemyPrefabs = spiritualMonsterPrefabs;
                break;
            case BlockColorType.Green: // War
                itemCount = 4;
                enemyCount = 3;
                itemPrefabs = warItemPrefabs;
                enemyPrefabs = warMonsterPrefabs;
                break;
            case BlockColorType.Tan: // Mixed
                itemCount = 4;
                enemyCount = 4;
                itemPrefabs = mixedItemPrefabs;
                enemyPrefabs = mixedMonsterPrefabs;
                break;
            default:
                Debug.LogError("Unknown block color!");
                return;
        }

        occupiedGridPositions.Clear();
        occupiedGridPositions.Add(startPosition); // Reserve player’s starting spot

        SpawnObjects(itemPrefabs, itemCount, itemHeightOffset, "Item", true);
        SpawnObjects(enemyPrefabs, enemyCount, enemyHeightOffset, "Enemy", false);
    }



    private void SpawnObjects(GameObject[] prefabs, int count, float heightOffset, string type, bool isItem)
    {
        int spawnAreaStartX = 1;
        int spawnAreaStartZ = 1;
        int spawnAreaEndX = (int)mazeSize.x - 2;
        int spawnAreaEndZ = (int)mazeSize.y - 2;

        //Debug.Log($"Spawning {type}s in range X: [{spawnAreaStartX}, {spawnAreaEndX}], Z: [{spawnAreaStartZ}, {spawnAreaEndZ}]");

        for (int i = 0; i < count; i++)
        {
            Vector2Int randomGridPosition;
            int attempts = 0;

            do
            {
                if (attempts >= maxSpawnAttempts)
                {
                    Debug.LogWarning($"Max spawn attempts reached for {type}s. Skipping {type} spawn.");
                    return;
                }

                int randomGridX = Random.Range(spawnAreaStartX, spawnAreaEndX + 1);
                int randomGridZ = Random.Range(spawnAreaStartZ, spawnAreaEndZ + 1);
                randomGridPosition = new Vector2Int(randomGridX, randomGridZ);
                attempts++;
            } 
            while (IsPositionOccupied(randomGridPosition) || (!isItem && IsAdjacentToEnemy(randomGridPosition)) || (isItem && IsDirectlyAdjacentToEnemy(randomGridPosition)));

            //Debug.Log($"{type} {i + 1} spawned at grid {randomGridPosition}");

            // Mark the grid position as occupied
            MarkGridAsOccupied(randomGridPosition);

            // Convert grid position to world position, centering within the grid square
            Vector3 worldPosition = new Vector3(
                randomGridPosition.x * gridSize + gridSize * 0.5f, // Center X
                heightOffset, // Height adjustment
                randomGridPosition.y * gridSize + gridSize * 0.5f  // Center Z
            );

            //Debug.Log($"{type} {i + 1} world position: {worldPosition}");

            // Select a random prefab
            Debug.Log("prefabs in FooorManager:" + prefabs);
            GameObject randomPrefab = prefabs[Random.Range(0, prefabs.Length)];

            // Instantiate the object at the calculated position
            //Instantiate(randomPrefab, worldPosition, Quaternion.identity);

            // Instantiate the object at the calculated position
            GameObject spawnedObject = Instantiate(randomPrefab, worldPosition, Quaternion.identity);

            // Check if the object has a Projectile component and remove it to avoid Hit effect to the player when picking these up
            Projectile projectileComponent = spawnedObject.GetComponent<Projectile>();
            if (projectileComponent != null)
            {
                Destroy(projectileComponent);
            }

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

    // Check if the position is directly adjacent (N, S, E, W) to any enemy
    private bool IsDirectlyAdjacentToEnemy(Vector2Int position)
    {
        Vector2Int[] adjacentOffsets = { new Vector2Int(0, 1), new Vector2Int(0, -1), new Vector2Int(1, 0), new Vector2Int(-1, 0) };
        foreach (var offset in adjacentOffsets)
        {
            Vector2Int adjacentPosition = position + offset;
            if (occupiedGridPositions.Contains(adjacentPosition))
            {
                return true;
            }
        }
        return false;
    }

    // Check if the position is adjacent (including diagonals) to any enemy
    private bool IsAdjacentToEnemy(Vector2Int position)
    {
        Vector2Int[] adjacentOffsets = {
            new Vector2Int(0, 1), new Vector2Int(0, -1), new Vector2Int(1, 0), new Vector2Int(-1, 0), // Direct neighbors
            new Vector2Int(1, 1), new Vector2Int(-1, -1), new Vector2Int(1, -1), new Vector2Int(-1, 1) // Diagonals
        };
        foreach (var offset in adjacentOffsets)
        {
            Vector2Int adjacentPosition = position + offset;
            if (occupiedGridPositions.Contains(adjacentPosition))
            {
                return true;
            }
        }
        return false;
    }


    public void GenerateFloorContents(BlockColorType blockColor, Vector2Int startPosition, MazeBlock currentBlock)
    {
        Debug.Log($"Generating floor contents for {currentBlock.name} with color {blockColor} at {startPosition}");
        // (1) Clear previous floor content (you can implement ClearFloorContents() to destroy all spawned items, enemies, doors, ladders, etc.)
        ClearFloorContents();
        // (2) Determine how many items/enemies to spawn and which prefab arrays to use based on blockColor.
        int itemCount, enemyCount;
        GameObject[] itemPrefabs, enemyPrefabs;

        Debug.Log("currentBlock is: " + currentBlock);
        Debug.Log("Current Block horizontal neighbour left: " + currentBlock.neighborLeft + " Neigour Right: " + currentBlock.neighborRight);
        Debug.Log("Current Block vertical neighbour below left: " + currentBlock.neighborBelowLeft + " Neigour Below Right: " + currentBlock.neighborBelowRight);



        switch (blockColor)
        {
            case BlockColorType.Blue:
                itemCount = 4;
                enemyCount = 3;
                itemPrefabs = spiritualItemPrefabs;
                enemyPrefabs = spiritualMonsterPrefabs;
                break;
            case BlockColorType.Green:
                itemCount = 4;
                enemyCount = 3;
                itemPrefabs = warItemPrefabs;
                enemyPrefabs = warMonsterPrefabs;
                break;
            case BlockColorType.Tan:
                itemCount = 5;
                enemyCount = 4;
                itemPrefabs = mixedItemPrefabs;
                enemyPrefabs = mixedMonsterPrefabs;
                break;
            default:
                Debug.LogError("Unknown block color!");
                return;
        }

        // Spawn items and enemies at positions inside the current MazeBlock
        //SpawnObjects(itemPrefabs, itemCount, itemHeightOffset, "Item", true, currentBlock.transform);
        //SpawnObjects(enemyPrefabs, enemyCount, enemyHeightOffset, "Enemy", false, currentBlock.transform);

        // (3) Corridor doors:
        // If current block has a horizontal neighbor to the right, spawn an east door.
        if (currentBlock.neighborRight != null)
        {
            //GameObject eastDoorPrefab = GetEastDoorPrefab(currentBlock.neighborRight.colorType);
            // Determine a fixed local spawn position for the east door relative to the block.
            //Vector3 eastDoorPosition = currentBlock.transform.position + new Vector3(doorOffsetX, doorOffsetY, 0);
            //Instantiate(eastDoorPrefab, eastDoorPosition, Quaternion.identity, currentBlock.transform);
        }
        // If current block has a neighbor to the left, spawn a west door.
        if (currentBlock.neighborLeft != null)
        {
            //GameObject westDoorPrefab = GetWestDoorPrefab(currentBlock.neighborLeft.colorType);
            //Vector3 westDoorPosition = currentBlock.transform.position + new Vector3(-doorOffsetX, doorOffsetY, 0);
            //Instantiate(westDoorPrefab, westDoorPosition, Quaternion.identity, currentBlock.transform);
        }

        // (4) Ladders for descending:
        // If there’s a lower neighbor on the left, spawn a ladder on the west side.
        if (currentBlock.neighborBelowLeft != null)
        {
            // Pick one of the available west ladder prefabs at random.
            GameObject ladderWestPrefab = westLadders[Random.Range(0, westLadders.Length)];
            // Use fixed offsets (which you define) relative to the MazeBlock.
            //Vector3 ladderWestPos = currentBlock.transform.position + new Vector3(-ladderOffsetX, ladderOffsetY, ladderOffsetZ);
            //Instantiate(ladderWestPrefab, ladderWestPos, Quaternion.identity, currentBlock.transform);
        }
        // Similarly, if there’s a lower neighbor on the right, spawn a ladder on the east side.
        if (currentBlock.neighborBelowRight != null)
        {
            GameObject ladderEastPrefab = eastLadders[Random.Range(0, eastLadders.Length)];
            //Vector3 ladderEastPos = currentBlock.transform.position + new Vector3(ladderOffsetX, ladderOffsetY, ladderOffsetZ);
            //Instantiate(ladderEastPrefab, ladderEastPos, Quaternion.identity, currentBlock.transform);
        }
    }


    private void ClearFloorContents() {
        return;
    }

}
