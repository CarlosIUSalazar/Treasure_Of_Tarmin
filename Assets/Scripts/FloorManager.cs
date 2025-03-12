using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using Unity.VisualScripting;
using UnityEngine;

public class FloorManager : MonoBehaviour
{
    // [SerializeField] private Transform player; // Assign player's Transform in the Inspector
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
    [SerializeField] private GameObject mazeSpawnPoint1;
    [SerializeField] private GameObject mazeSpawnPoint2;
    [SerializeField] private GameObject mazeSpawnPoint3;
    [SerializeField] private GameObject mazeSpawnPoint4;
    [SerializeField] private GameObject[] mazeSetsPrefabs;
    Player player;
    PlayerGridMovement playerGridMovement;
    MazeBlock currentNeighbourLeft;
    MazeBlock currentNeighbourRight;
    MazeBlock currentNeighbourBelowLeft;
    MazeBlock currentNeighbourBelowRight;
    MazeGenerator mazeGenerator;
    private float gridSize = 10.0f; // Size of each grid square
    private float itemHeightOffset = 0.1f; // Height adjustment for items above the ground
    private float enemyHeightOffset = 0f; // Height adjustment for enemies
    private Vector2 mazeSize = new Vector2(12, 12); // The full size of the maze (outer grid)

    // Keep track of occupied grid positions
    private HashSet<Vector2Int> occupiedGridPositions = new HashSet<Vector2Int>();
    private const int maxSpawnAttempts = 100; // Limit to prevent infinite loops

    void Start()
    {
        player = GameObject.Find("Player").GetComponent<Player>();
        mazeGenerator = GameObject.Find("MazeGenerator").GetComponent<MazeGenerator>();
        playerGridMovement = GameObject.Find("Player").GetComponent<PlayerGridMovement>();
        GenerateMazeSets();
    }


    private void GenerateMazeSets() {
        // Maze1 Vector3(114.669998,-444.940002,505.359985)
        // Maze2 Vector3(114.669998,-444.940002,555.359985)
        // Maze3 Vector3(164.669998,-444.940002,505.359985)
        // Maze4 Vector3(164.669998,-444.940002,555.359985)
        int mazeIndex = Random.Range(0,15);
        // Instantiate(mazeSetsPrefabs[mazeIndex],new Vector3(114.669998f,-444.940002f,505.359985f),Quaternion.identity);
        // mazeIndex = Random.Range(0,15);
        // Instantiate(mazeSetsPrefabs[mazeIndex],new Vector3(114.669998f,-444.940002f,555.359985f),Quaternion.identity);
        // mazeIndex = Random.Range(0,15);
        // Instantiate(mazeSetsPrefabs[mazeIndex],new Vector3(164.669998f,-444.940002f,505.359985f),Quaternion.identity);
        // mazeIndex = Random.Range(0,15);
        // Instantiate(mazeSetsPrefabs[mazeIndex],new Vector3(164.669998f,-444.940002f,555.359985f),Quaternion.identity);

        Instantiate(mazeSetsPrefabs[mazeIndex],mazeSpawnPoint1.transform.localPosition,Quaternion.identity);
        mazeIndex = Random.Range(0,15);
        Instantiate(mazeSetsPrefabs[mazeIndex],mazeSpawnPoint2.transform.localPosition,Quaternion.identity);
        mazeIndex = Random.Range(0,15);
        Instantiate(mazeSetsPrefabs[mazeIndex],mazeSpawnPoint3.transform.localPosition,Quaternion.identity);
        mazeIndex = Random.Range(0,15);
        Instantiate(mazeSetsPrefabs[mazeIndex],mazeSpawnPoint4.transform.localPosition,Quaternion.identity);

        // Instantiate(mazeSetsPrefabs[mazeIndex],mazeSpawnPoint1.transform.position,Quaternion.identity);
        // mazeIndex = Random.Range(0,15);
        // Instantiate(mazeSetsPrefabs[mazeIndex],mazeSpawnPoint2.transform.position,Quaternion.identity);
        // mazeIndex = Random.Range(0,15);
        // Instantiate(mazeSetsPrefabs[mazeIndex],mazeSpawnPoint3.transform.position,Quaternion.identity);
        // mazeIndex = Random.Range(0,15);
        // Instantiate(mazeSetsPrefabs[mazeIndex],mazeSpawnPoint4.transform.position,Quaternion.identity);

        // Instantiate(mazeSetsPrefabs[mazeIndex],new Vector3(114.559998f,0,505.26001f),Quaternion.identity);
        // mazeIndex = Random.Range(0,15);
        // Instantiate(mazeSetsPrefabs[mazeIndex],new Vector3(114.559998f,0,555.26001f),Quaternion.identity);
        // mazeIndex = Random.Range(0,15);
        // Instantiate(mazeSetsPrefabs[mazeIndex],new Vector3(164.559998f,0,505.26001f),Quaternion.identity);
        // mazeIndex = Random.Range(0,15);
        // Instantiate(mazeSetsPrefabs[mazeIndex],new Vector3(164.559998f,0,555.26001f),Quaternion.identity);

        // Instantiate(mazeSetsPrefabs[mazeIndex],Vector3.zero,Quaternion.identity);
        // mazeIndex = Random.Range(0,15);
        // Instantiate(mazeSetsPrefabs[mazeIndex],Vector3.zero,Quaternion.identity);
        // mazeIndex = Random.Range(0,15);
        // Instantiate(mazeSetsPrefabs[mazeIndex],Vector3.zero,Quaternion.identity);
        // mazeIndex = Random.Range(0,15);
        // Instantiate(mazeSetsPrefabs[mazeIndex],Vector3.zero,Quaternion.identity);
    }


    public void PopulateCurrentNeighbours(MazeBlock currentBlock) {
        //currentPlayerBlock = currentBlock ?? currentPlayerBlock;
        currentNeighbourLeft      = currentBlock.neighborLeft;
        currentNeighbourRight     = currentBlock.neighborRight;
        currentNeighbourBelowLeft = currentBlock.neighborBelowLeft;
        currentNeighbourBelowRight= currentBlock.neighborBelowRight;

        Debug.Log("currentPlayerBlock is " + mazeGenerator.currentPlayerBlock + " At " + (mazeGenerator.currentPlayerBlock?.ToString() ?? "null"));
        Debug.Log("CurrentNeighbourLeft is " + currentNeighbourLeft + " At " + (currentNeighbourLeft?.gridCoordinate.ToString() ?? "null"));
        Debug.Log("CurrentNeighbourRight is " + currentNeighbourRight + " At " + (currentNeighbourRight?.gridCoordinate.ToString() ?? "null"));
        Debug.Log("CurrentNeighbourBelowLeft is " + currentNeighbourBelowLeft + " At " + (currentNeighbourBelowLeft?.gridCoordinate.ToString() ?? "null"));
        Debug.Log("CurrentNeighbourBelowRight is " + currentNeighbourBelowRight + " At " + (currentNeighbourBelowRight?.gridCoordinate.ToString() ?? "null"));
    }


    public void MovePlayerToNewMaze(string corridorDoorSide) { // Crossing Corridor Door
        MazeBlock targetBlock = null;
        if (corridorDoorSide == "CorridorDoorWest") {
            targetBlock = currentNeighbourLeft;
        } else if (corridorDoorSide == "CorridorDoorEast") {
            targetBlock = currentNeighbourRight;
        }
        if (targetBlock == null) {
            Debug.LogWarning("No neighboring block found for door: " + corridorDoorSide);
            return;
        }

        // Update the current block in the maze generator.
        mazeGenerator.UpdatePlayerCursor(targetBlock);
        PopulateCurrentNeighbours(mazeGenerator.currentPlayerBlock);

        // Get the player's current rotation for comparison.
        float currentY = player.transform.eulerAngles.y;

        // Assign default values (so the variables are always initialized)
        Vector3 newPosition = player.transform.position;
        Quaternion newRotation = player.transform.rotation;

        // Determine the new position/rotation based on the door crossed and the player’s current facing.
        if (corridorDoorSide == "CorridorDoorWest") {
            // If crossing the West door:
            if (Mathf.Abs(Mathf.DeltaAngle(currentY, 90)) < 10f) { // Facing South
                newPosition = new Vector3(65, 2.5f, 115);
                newRotation = Quaternion.Euler(0, 90, 0);
            } else if (Mathf.Abs(Mathf.DeltaAngle(currentY, 270)) < 10f) { // Facing North
                newPosition = new Vector3(55, 2.5f, 115);
                newRotation = Quaternion.Euler(0, -90, 0);
            }

            playerGridMovement.UpdateMinimapCursor(1.1f);
        }
        else if (corridorDoorSide == "CorridorDoorEast") {
            // If crossing the East door:
            if (Mathf.Abs(Mathf.DeltaAngle(currentY, 90)) < 10f) { // Facing South
                newPosition = new Vector3(65, 2.5f, 5);
                newRotation = Quaternion.Euler(0, 90, 0);
            } else if (Mathf.Abs(Mathf.DeltaAngle(currentY, 270)) < 10f) { // Facing North
                newPosition = new Vector3(55, 2.5f, 5);
                newRotation = Quaternion.Euler(0, -90, 0);
            }
        }

        //IF PLAYER IS ON EVEN FLOOR NEED TO LOWER THE CURSOR VERTICALLY NOT ONLY MOVE HORIZONTALLY WHEN CROSSING CORRIDOR DOORS
        if (player.floor % 2 == 0){ 
            Transform cursorTransform = mazeGenerator.currentPlayerBlock.playerCursor.transform;
            Vector3 pos = cursorTransform.position; // get the current position
            pos.y -= 12f; // subtract 12 from y
            cursorTransform.position = pos; // assign the modified vector back
        }
        
        // Now update the player's transform immediately.
        player.GetComponent<PlayerGridMovement>().SetPlayerImmediatePosition(newPosition, newRotation);
        
        // Now regenerate the floor contents based on the new block.
        GenerateFloorContents(mazeGenerator.currentPlayerBlock.colorType, mazeGenerator.currentPlayerBlock.gridCoordinate, mazeGenerator.currentPlayerBlock, corridorDoorSide);
    }


    public void MoveCursorVerticallyDown(RaycastHit hit) { //Descending using a Ladder
        GameObject item = hit.collider.gameObject;
        Vector3 newPosition = player.transform.position;
        Quaternion newRotation = player.transform.rotation;
        float floorCursorPositionOffset; // Position 1 is origin West (x +0) z=5.  Posiiton 2 is ladder west (x +0.3) z=35, 3 = Ladder East (x + 0.8) z=85, 4 = End of Maze east (x + 1.1) z=115
        if (player.floor % 2 == 0) { // Changing to a different block
            Debug.Log("Used Ladder from Even Floor " + player.floor);
            if (item.name.Contains("East")) { //Checking that the ladder prefab name contains East
                
                if (currentNeighbourBelowLeft == currentNeighbourBelowRight) { // If its a double stacked floor stay on East side
                    floorCursorPositionOffset = 0.8f; 
                } else {
                    floorCursorPositionOffset = 0.3f; // If its not a double stacked, descend to the West ladder side of new block 
                    // Now update the player's transform immediately.
                    player.GetComponent<PlayerGridMovement>().SetPlayerImmediatePosition(new Vector3(newPosition.x, newPosition.y, newPosition.z =35), newRotation);
                }
                //Move Left Down
                mazeGenerator.UpdatePlayerCursor(currentNeighbourBelowRight); //This updates the mazeGenerator.currentPlayerBlock
                Debug.Log("Player Descended to " + currentNeighbourBelowRight);
                player.ModifyFloorNumber();
                PopulateCurrentNeighbours(mazeGenerator.currentPlayerBlock);
                GenerateFloorContents(mazeGenerator.currentPlayerBlock.colorType,mazeGenerator.currentPlayerBlock.gridCoordinate,mazeGenerator.currentPlayerBlock, "NoCorridorDoorUsed");                
                playerGridMovement.UpdateMinimapCursor(floorCursorPositionOffset);
                return;
            } else if (item.name.Contains("West")) { ////Checking that the ladder prefab name contains West
                
                if (currentNeighbourBelowLeft == currentNeighbourBelowRight) {
                    floorCursorPositionOffset = 0.3f; // If its a double stacked floor stay on West side
                } else {
                    floorCursorPositionOffset = 0.8f; // If its not a double stacked, descend to the East ladder side of new block 
                    player.GetComponent<PlayerGridMovement>().SetPlayerImmediatePosition(new Vector3(newPosition.x, newPosition.y, newPosition.z =85), newRotation);
                }
                
                //Move Right Down
                mazeGenerator.UpdatePlayerCursor(currentNeighbourBelowLeft);
                Debug.Log("Player Descended to " + currentNeighbourBelowLeft);
                player.ModifyFloorNumber();

                PopulateCurrentNeighbours(mazeGenerator.currentPlayerBlock);
                GenerateFloorContents(mazeGenerator.currentPlayerBlock.colorType,mazeGenerator.currentPlayerBlock.gridCoordinate,mazeGenerator.currentPlayerBlock, "NoCorridorDoorUsed");
                playerGridMovement.UpdateMinimapCursor(floorCursorPositionOffset); //Shift X position on minimap to match the Ladder used
                return;
            }
        } else { //Descending within the same block
            Debug.Log("Used Ladder from Odd Floor " + player.floor);
            GameObject ladder = hit.collider.gameObject;
            Debug.Log("The Ladder hit nme is " + ladder.name);
            //This will be called whenever the user descend from an ODD floor, so the top floor within a block.
            // Im still not sure why substracting 12 worked, its not the Y transform that i can see in the inspector, but for now I'll leave it.
            Transform cursorTransform = mazeGenerator.currentPlayerBlock.playerCursor.transform;
            Vector3 pos = cursorTransform.position; // get the current position
            pos.y -= 12f; // subtract 12 from y
            cursorTransform.position = pos; // assign the modified vector back
            player.ModifyFloorNumber();

            //PopulateCurrentNeighbours(currentPlayerBlock);
            GenerateFloorContents(mazeGenerator.currentPlayerBlock.colorType,mazeGenerator.currentPlayerBlock.gridCoordinate,mazeGenerator.currentPlayerBlock, "NoCorridorDoorUsed");
            return;
        }
    }


    private void SpawnObjects(GameObject[] prefabs, int count, float heightOffset, string type, bool isItem)
    {
        Debug.Log("Spawning Objects of type: " + type);
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
            //Debug.Log("prefabs in FooorManager:" + prefabs);
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


    public void GenerateFloorContents(BlockColorType blockColor, Vector2Int startPosition, MazeBlock currentBlock, string corridorDoorSide)
    {
        Debug.Log($"Generating floor contents for {currentBlock.name} with color {blockColor} at {startPosition}");
        // (1) Clear previous floor content (you can implement ClearFloorContents() to destroy all spawned items, enemies, doors, ladders, etc.)
        ClearFloorContents();
        // (2) Determine how many items/enemies to spawn and which prefab arrays to use based on blockColor.
        int itemCount, enemyCount;
        GameObject[] itemPrefabs, enemyPrefabs;

        Debug.Log("currentBlock is: " + currentBlock);
        Debug.Log("Current Block horizontal neighbour left: " + currentBlock.neighborLeft);
        Debug.Log("Current Neigour Right: " + currentBlock.neighborRight);
        Debug.Log("Current Block vertical neighbour below left: " + currentBlock.neighborBelowLeft); 
        Debug.Log(" Neigour Below Right: " + currentBlock.neighborBelowRight);
        Debug.Log("Current Block horizontal neighbour left color: " + currentBlock.neighborLeft?.colorType ?? "None");
        Debug.Log("Current Block horizontal neighbour right color: " + currentBlock.neighborRight?.colorType ?? "None");

        /////
        /// SPAWNER OF CORRIDOR DOORS
        /////

        if (currentBlock.neighborLeft != null && corridorDoorSide != "CorridorDoorEast") {
            if (currentBlock.neighborLeft.colorType == BlockColorType.Blue) {
                westDoorBlue.SetActive(true);
            } else if (currentBlock.neighborLeft.colorType == BlockColorType.Green) {
                westDoorGreen.SetActive(true);
            } else if (currentBlock.neighborLeft.colorType == BlockColorType.Tan) {
                westDoorTan.SetActive(true);
            }
        }

        if (currentBlock.neighborRight != null && corridorDoorSide != "CorridorDoorWest") {
            if (currentBlock.neighborRight.colorType == BlockColorType.Blue) {
                eastDoorBlue.SetActive(true);
            } else if (currentBlock.neighborRight.colorType == BlockColorType.Green) {
                eastDoorGreen.SetActive(true);
            } else if (currentBlock.neighborRight.colorType == BlockColorType.Tan) {
                eastDoorTan.SetActive(true);
            }
        }
        //////
        ///SPAWNER OF LADDERS
        /////
        if (player.floor % 2 == 0) {
            Debug.Log("Spawning Even Floor Ladders");
            if (currentBlock.neighborBelowLeft != null) {
                SpawnLadder("West");
            } else if (currentBlock.neighborBelowRight != null) {
                SpawnLadder("East");
            }
        } else {
            Debug.Log("Spawning Odd Floor Ladders");
            SpawnLadder("West");
            SpawnLadder("East");
        }

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

        occupiedGridPositions.Clear(); //Maybe not needed

        // Spawn items and enemies at positions inside the current MazeBlock
        SpawnObjects(itemPrefabs, itemCount, itemHeightOffset, "Item", true);//, currentBlock.transform);
        SpawnObjects(enemyPrefabs, enemyCount, enemyHeightOffset, "Enemy", false);//, currentBlock.transform);

    }


    private void SpawnLadder(string side) {
        int randomInt = Random.Range(0, westLadders.Length);

        if (side == "East")
        {
            eastLadders[randomInt].SetActive(true);

            // Now mark the ladder’s grid cell as occupied
            GameObject ladderObj = eastLadders[randomInt];
            Vector3 ladderPos = ladderObj.transform.position;
            int gx = Mathf.FloorToInt(ladderPos.x / gridSize);
            int gz = Mathf.FloorToInt(ladderPos.z / gridSize);
            MarkGridAsOccupied(new Vector2Int(gx, gz));
        }
        else if (side == "West")
        {
            westLadders[randomInt].SetActive(true);

            // Mark the ladder’s grid cell as occupied
            GameObject ladderObj = westLadders[randomInt];
            Vector3 ladderPos = ladderObj.transform.position;
            int gx = Mathf.FloorToInt(ladderPos.x / gridSize);
            int gz = Mathf.FloorToInt(ladderPos.z / gridSize);
            MarkGridAsOccupied(new Vector2Int(gx, gz));
        }
    }


    private void ClearFloorContents() {
        string[] itemEnemyTags = new string[] { "Item", "Enemy" };

        foreach (string tag in itemEnemyTags)
        {
            GameObject[] objects = GameObject.FindGameObjectsWithTag(tag);
            foreach (GameObject obj in objects)
            {
                Destroy(obj);
            }
        }
        Debug.Log("Destroyed Foor Items and Enemies");

        string[] laddersDoorsTags = new string[] { "Ladder", "CorridorDoorEast", "CorridorDoorWest" };

        foreach (string tag in laddersDoorsTags)
        {
            GameObject[] objects = GameObject.FindGameObjectsWithTag(tag);
            foreach (GameObject obj in objects)
            {
                obj.SetActive(false);
            }
        }
        Debug.Log("Deactivated Foor Ladders and Corridor Doors");
    }
}
