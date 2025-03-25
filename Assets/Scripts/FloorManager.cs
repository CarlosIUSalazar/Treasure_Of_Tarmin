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
    //[SerializeField] private GameObject mazeWallEntranceLight;
    //[SerializeField] private GameObject mazeWallEntranceDark;
    //ENTRANCES TO THE MAZE
    [SerializeField] private GameObject northWestEntranceLightSpawnPoint;
    [SerializeField] private GameObject southWestEntranceLightSpawnPoint;
    [SerializeField] private GameObject southWestEntranceDarkSpawnPoint;
    [SerializeField] private GameObject southEastEntranceDarkSpawnPoint;
    [SerializeField] private GameObject southEastEntranceLightSpawnPoint;
    [SerializeField] private GameObject northEastEntranceLightSpawnPoint;
    [SerializeField] private GameObject northEastEntranceDarkSpawnPoint;
    [SerializeField] private GameObject northWestEntranceDarkSpawnPoint;
    // 4 CONNECTING DOORS BETWEEN MAZE BLOCKS
    [SerializeField] private GameObject MazeConnectDoorPoint1;
    [SerializeField] private GameObject MazeConnectDoorPoint2;
    [SerializeField] private GameObject MazeConnectDoorPoint3;
    [SerializeField] private GameObject MazeConnectDoorPoint4;
    // MAZE DOOR WALLS
    [SerializeField] private GameObject MazeDoorLight;
    [SerializeField] private GameObject MazeSecretDoorLight;
    [SerializeField] private GameObject MazeDoorDark;
    [SerializeField] private GameObject MazeSecretDoorDark;
    // MAZE EYES SPAWN POSITIONS
    [SerializeField] private GameObject EyeSpawnPoint1;
    [SerializeField] private GameObject EyeSpawnPoint2;
    [SerializeField] private GameObject EyeSpawnPoint3;
    [SerializeField] private GameObject EyeSpawnPoint4;
    [SerializeField] private GameObject EyeSpawnPoint5;
    [SerializeField] private GameObject EyeSpawnPoint6;
    [SerializeField] private GameObject EyeSpawnPoint7;
    [SerializeField] private GameObject EyeSpawnPoint8;
    // Eye Prefabs
    [SerializeField] private GameObject WallEyesBluePrefab;
    [SerializeField] private GameObject WallEyesGreenPrefab;
    [SerializeField] private GameObject WallEyesTanPrefab;

    Player player;
    PlayerGridMovement playerGridMovement;
    GameManager gameManager;
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
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
    }


    private void GenerateMazeSets() {
        // Generate 4 Maze Sets
        int randomIndex;
        int mazeIndex = Random.Range(0,15);
        Instantiate(mazeSetsPrefabs[mazeIndex],mazeSpawnPoint1.transform.localPosition,Quaternion.identity);
        mazeIndex = Random.Range(0,15);
        Instantiate(mazeSetsPrefabs[mazeIndex],mazeSpawnPoint2.transform.localPosition,Quaternion.identity);
        mazeIndex = Random.Range(0,15);
        Instantiate(mazeSetsPrefabs[mazeIndex],mazeSpawnPoint3.transform.localPosition,Quaternion.identity);
        mazeIndex = Random.Range(0,15);
        Instantiate(mazeSetsPrefabs[mazeIndex],mazeSpawnPoint4.transform.localPosition,Quaternion.identity);
        
        /////
        // Generate Maze entrance DoorWalls
        // 1. NorthWest Light
        randomIndex = Random.Range(0,3);
        if (randomIndex == 1) {
            Instantiate(MazeDoorLight,northWestEntranceLightSpawnPoint.transform.position,Quaternion.identity);
        } else if (randomIndex == 2) {
            Instantiate(MazeSecretDoorLight,northWestEntranceLightSpawnPoint.transform.position,Quaternion.identity);
        }
        // 2. SouthWest Light
        randomIndex = Random.Range(0,3);
        if (randomIndex == 1) {
            Instantiate(MazeDoorLight,southWestEntranceLightSpawnPoint.transform.position,Quaternion.identity);
        } else if (randomIndex == 2) {
            Instantiate(MazeSecretDoorLight,southWestEntranceLightSpawnPoint.transform.position,Quaternion.identity);
        }
        // 3. SouthWest Dark
        randomIndex = Random.Range(0,3);
        if (randomIndex == 1) {
            Instantiate(MazeDoorDark,southWestEntranceDarkSpawnPoint.transform.position,Quaternion.Euler(0, 270, 0));
        } else if (randomIndex == 2) {
            Instantiate(MazeSecretDoorDark,southWestEntranceDarkSpawnPoint.transform.position,Quaternion.Euler(0, 270, 0));
        }
        // 4. SouthEast Dark
        randomIndex = Random.Range(0,3);
        if (randomIndex == 1) {
            Instantiate(MazeDoorDark,southEastEntranceDarkSpawnPoint.transform.position,Quaternion.Euler(0, 270, 0));
        } else if (randomIndex == 2) {
            Instantiate(MazeSecretDoorDark,southEastEntranceDarkSpawnPoint.transform.position,Quaternion.Euler(0, 270, 0));
        }
        // 5. SouthEast Light
        randomIndex = Random.Range(0,3);
        if (randomIndex == 1) {
            Instantiate(MazeDoorLight,southEastEntranceLightSpawnPoint.transform.position,Quaternion.Euler(0, 180, 0));
        } else if (randomIndex == 2) {
            Instantiate(MazeSecretDoorLight,southEastEntranceLightSpawnPoint.transform.position,Quaternion.Euler(0, 180, 0));
        }
        // 6. NorthEast Light
        randomIndex = Random.Range(0,3);
        if (randomIndex == 1) {
            Instantiate(MazeDoorLight,northEastEntranceLightSpawnPoint.transform.position,Quaternion.Euler(0, 180, 0));
        } else if (randomIndex == 2) {
            Instantiate(MazeSecretDoorLight,northEastEntranceLightSpawnPoint.transform.position,Quaternion.Euler(0, 180, 0));
        }
        // 7. NorthEast Dark
        randomIndex = Random.Range(0,3);
        if (randomIndex == 1) {
            Instantiate(MazeDoorDark,northEastEntranceDarkSpawnPoint.transform.position,Quaternion.Euler(0, 270, 0));
        } else if (randomIndex == 2) {
            Instantiate(MazeSecretDoorDark,northEastEntranceDarkSpawnPoint.transform.position,Quaternion.Euler(0, 270, 0));
        }
        // 8. NorthWest Dark
        randomIndex = Random.Range(0,3);
        if (randomIndex == 1) {
            Instantiate(MazeDoorDark,northWestEntranceDarkSpawnPoint.transform.position,Quaternion.Euler(0, 270, 0));
        } else if (randomIndex == 2) {
            Instantiate(MazeSecretDoorDark,northWestEntranceDarkSpawnPoint.transform.position,Quaternion.Euler(0, 270, 0));
        }

        // SPAWN DOORS CONNECTING INNER MAZES
        // 1. Northside East To West 
        randomIndex = Random.Range(0,5);
        if (randomIndex == 1) {
            Instantiate(MazeDoorLight,MazeConnectDoorPoint1.transform.position,Quaternion.Euler(0, 0, 0));
        } else if (randomIndex == 2) {
            Instantiate(MazeSecretDoorLight,MazeConnectDoorPoint1.transform.position,Quaternion.Euler(0, 0, 0));
        } else if (randomIndex == 3) {
            Instantiate(MazeSecretDoorDark,MazeConnectDoorPoint1.transform.position,Quaternion.Euler(0, 0, 0));
        } else if (randomIndex == 4) {
            Instantiate(MazeSecretDoorDark,MazeConnectDoorPoint1.transform.position,Quaternion.Euler(0, 0, 0));
        }
        // 2. West Side North To South 
        randomIndex = Random.Range(0,5);
        if (randomIndex == 1) {
            Instantiate(MazeDoorLight,MazeConnectDoorPoint2.transform.position,Quaternion.Euler(0, 270, 0));
        } else if (randomIndex == 2) {
            Instantiate(MazeSecretDoorLight,MazeConnectDoorPoint2.transform.position,Quaternion.Euler(0, 270, 0));
        } else if (randomIndex == 3) {
            Instantiate(MazeSecretDoorDark,MazeConnectDoorPoint2.transform.position,Quaternion.Euler(0, 270, 0));
        } else if (randomIndex == 4) {
            Instantiate(MazeSecretDoorDark,MazeConnectDoorPoint2.transform.position,Quaternion.Euler(0, 270, 0));
        }
        // 3. South Side East To West 
        randomIndex = Random.Range(0,5);
        if (randomIndex == 1) {
            Instantiate(MazeDoorLight,MazeConnectDoorPoint3.transform.position,Quaternion.Euler(0, 0, 0));
        } else if (randomIndex == 2) {
            Instantiate(MazeSecretDoorLight,MazeConnectDoorPoint3.transform.position,Quaternion.Euler(0, 0, 0));
        } else if (randomIndex == 3) {
            Instantiate(MazeSecretDoorDark,MazeConnectDoorPoint3.transform.position,Quaternion.Euler(0, 0, 0));
        } else if (randomIndex == 4) {
            Instantiate(MazeSecretDoorDark,MazeConnectDoorPoint3.transform.position,Quaternion.Euler(0, 0, 0));
        }
        // 4. East Side North To South 
        randomIndex = Random.Range(0,5);
        if (randomIndex == 1) {
            Instantiate(MazeDoorLight,MazeConnectDoorPoint4.transform.position,Quaternion.Euler(0, 270, 0));
        } else if (randomIndex == 2) {
            Instantiate(MazeSecretDoorLight,MazeConnectDoorPoint4.transform.position,Quaternion.Euler(0, 270, 0));
        } else if (randomIndex == 3) {
            Instantiate(MazeSecretDoorDark,MazeConnectDoorPoint4.transform.position,Quaternion.Euler(0, 270, 0));
        } else if (randomIndex == 4) {
            Instantiate(MazeSecretDoorDark,MazeConnectDoorPoint4.transform.position,Quaternion.Euler(0, 270, 0));
        }
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


    public void MovePlayerToNewMazeViaCorridorDoor(string corridorDoorSide) { // Crossing Corridor Door
        MazeBlock targetBlock = null;
        if (corridorDoorSide == "CorridorDoorWest") {
            targetBlock = currentNeighbourLeft;
        } else if (corridorDoorSide == "CorridorDoorEast") {
            targetBlock = currentNeighbourRight;
        }
        playerGridMovement.canBackStep = false; // Disable backstep when changing mazes
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
        occupiedGridPositions.Clear(); //Clear previous occupied positions so thigns keep spawning on new floors
        Debug.Log($"Generating floor contents for {currentBlock.name} with color {blockColor} at {startPosition}");
        // (1) Clear previous floor content (you can implement ClearFloorContents() to destroy all spawned items, enemies, doors, ladders, etc.)
        ClearFloorContents();
        GenerateMazeSets();
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
        /// SPAWNER OF WALL EYES 
        /////
        if (currentBlock.colorType == BlockColorType.Blue) {
                Debug.Log("Spawning Blue Eyes");
                Instantiate(WallEyesBluePrefab,EyeSpawnPoint1.transform.localPosition,Quaternion.identity);
                Instantiate(WallEyesBluePrefab,EyeSpawnPoint2.transform.localPosition,Quaternion.identity);
                Instantiate(WallEyesBluePrefab,EyeSpawnPoint3.transform.localPosition,Quaternion.Euler(0,270,0));
                Instantiate(WallEyesBluePrefab,EyeSpawnPoint4.transform.localPosition,Quaternion.Euler(0,270,0));
                Instantiate(WallEyesBluePrefab,EyeSpawnPoint5.transform.localPosition,Quaternion.Euler(0,180,0));
                Instantiate(WallEyesBluePrefab,EyeSpawnPoint6.transform.localPosition,Quaternion.Euler(0,180,0));
                Instantiate(WallEyesBluePrefab,EyeSpawnPoint7.transform.localPosition,Quaternion.Euler(0,90,0));
                Instantiate(WallEyesBluePrefab,EyeSpawnPoint8.transform.localPosition,Quaternion.Euler(0,90,0));
        } else if (currentBlock.colorType == BlockColorType.Green) {
                Debug.Log("Spawning Green Eyes");
                Instantiate(WallEyesGreenPrefab,EyeSpawnPoint1.transform.localPosition,Quaternion.identity);
                Instantiate(WallEyesGreenPrefab,EyeSpawnPoint2.transform.localPosition,Quaternion.identity);
                Instantiate(WallEyesGreenPrefab,EyeSpawnPoint3.transform.localPosition,Quaternion.Euler(0,270,0));
                Instantiate(WallEyesGreenPrefab,EyeSpawnPoint4.transform.localPosition,Quaternion.Euler(0,270,0));
                Instantiate(WallEyesGreenPrefab,EyeSpawnPoint5.transform.localPosition,Quaternion.Euler(0,180,0));
                Instantiate(WallEyesGreenPrefab,EyeSpawnPoint6.transform.localPosition,Quaternion.Euler(0,180,0));
                Instantiate(WallEyesGreenPrefab,EyeSpawnPoint7.transform.localPosition,Quaternion.Euler(0,90,0));
                Instantiate(WallEyesGreenPrefab,EyeSpawnPoint8.transform.localPosition,Quaternion.Euler(0,90,0));
        }   else if (currentBlock.colorType == BlockColorType.Tan) {
                Debug.Log("Spawning Tan Eyes");
                Instantiate(WallEyesTanPrefab,EyeSpawnPoint1.transform.localPosition,Quaternion.identity);
                Instantiate(WallEyesTanPrefab,EyeSpawnPoint2.transform.localPosition,Quaternion.identity);
                Instantiate(WallEyesTanPrefab,EyeSpawnPoint3.transform.localPosition,Quaternion.Euler(0,270,0));
                Instantiate(WallEyesTanPrefab,EyeSpawnPoint4.transform.localPosition,Quaternion.Euler(0,270,0));
                Instantiate(WallEyesTanPrefab,EyeSpawnPoint5.transform.localPosition,Quaternion.Euler(0,180,0));
                Instantiate(WallEyesTanPrefab,EyeSpawnPoint6.transform.localPosition,Quaternion.Euler(0,180,0));
                Instantiate(WallEyesTanPrefab,EyeSpawnPoint7.transform.localPosition,Quaternion.Euler(0,90,0));
                Instantiate(WallEyesTanPrefab,EyeSpawnPoint8.transform.localPosition,Quaternion.Euler(0,90,0));
        }

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

        // -- Item Frequencies per floor (all difficulties) --
        //  2 Ladders
        //  1 Quiver
        //  2 Sacks of Flour
        //  2 Defensive Items
        //  8 Weapons
        //  5 Containers
        //  8 Enemies

        switch (blockColor)
        {
            case BlockColorType.Blue:
                itemCount = 8;
                enemyCount = 12;
                itemPrefabs = spiritualItemPrefabs;
                enemyPrefabs = spiritualMonsterPrefabs;
                break;
            case BlockColorType.Green:
                itemCount = 12;
                enemyCount = 12;
                itemPrefabs = warItemPrefabs;
                enemyPrefabs = warMonsterPrefabs;
                break;
            case BlockColorType.Tan:
                itemCount = 12;
                enemyCount = 12;
                itemPrefabs = mixedItemPrefabs;
                enemyPrefabs = mixedMonsterPrefabs;
                break;
            default:
                Debug.LogError("Unknown block color!");
                return;
        }

        //occupiedGridPositions.Clear(); //Maybe not needed

        // Spawn items and enemies at positions inside the current MazeBlock
        SpawnObjects(itemPrefabs, itemCount, itemHeightOffset, "Item", true);//, currentBlock.transform);
        SpawnObjects(enemyPrefabs, enemyCount, enemyHeightOffset, "Enemy", false);//, currentBlock.transform);

        if (gameManager.isMazeTransparent) {
            playerGridMovement.MakeMazeSetsTransparent();
            // The original Coroutine to deactivate this spell stays active so no need to worry about it here.
        }
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
        string[] itemEnemyTags = new string[] { "Item", "Enemy", "MazeSet", "MazeEntranceDoorWall" };

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

    //////   https://docs.google.com/spreadsheets/d/1jqqI34sGs0kx1J_uOj5Jc2wu2Sv4D0Um0ZMVFC0thjk/edit?gid=0#gid=0
    ///////////////
    /////MONEY BELT
    ///////////////
    /// if (Money Belt Tan) {
    ///     BIG PROBABILITY
    //      Coins gret, Necklace grey, Ingot grey, 
    //      
    //      SMALL PROBABILITY     
    //      Key tan, Lamp grey, Chalice grey
    //
    //      VERY SMALL PROBABILITY
    //      Crown grey
    //
    /// }
    /// if (Money Belt Orange) {
    ///     BIG PROBABILITY
    ///     Coins Gray, Necklace Gray , Ingot Gray , Lamp Gray, 
    /// 
    ///     SMALL PROBABILITY  
    ///     Key tan, Chalice Gray, Crown Gray, Coins Yellow, Neckalce Yellow, Ingot Yellow
    ///     
    ///     VERY SMALL PROBABILITY
    ///     Small blue potion
    /// }
    /// if (Money Belt Blue) {
    ///     BIG PROBABILITY
    ///     Ingot Gray , Lamp Gray, Chalice Gray, Crown Gray, Coins Yellow, Neckalce Yellow, Ingot Yellow
    /// 
    ///     SMALL PROBABILITY  
    ///     Small blue potion, Key tan, Key orange, Chalice Yellow, Crown Yellow, Coins white, Necklace white
    ///     
    ///     VERY SMALL PROBABILITY
    ///     War book blue, Spiritual book blue
    /// }
    /// /////////////
    /// SMALL BAG ///
    /// /////////////
    /// if (Small Bag Tan) {
    ///     BIG PROBABILITY
    //      Coins grey, Necklace grey, Ingot grey, Lamp grey, Chalice grey, 
    //      
    //      SMALL PROBABILITY     
    //      Key tan, Coins Yellow, Neckalce Yellow, Ingot Yellow, Lamp Yellow,
    //
    //      VERY SMALL PROBABILITY
    //       Challice Yellow, Crown Grey
    //
    /// }
    /// if (Small Bag Orange) {
    ///     BIG PROBABILITY
    ///     Key tan, Coins Yellow, Neckalce Yellow, Ingot Yellow, Lamp Yellow,
    /// 
    ///     SMALL PROBABILITY  
    ///     Challice Yellow, Crown Grey, Small blue potion
    ///     
    ///     VERY SMALL PROBABILITY
    ///     Key Orange, Challice yellow, Lamp Yellow, Chalice White, Crown yellow,  War book blue, Spiritual book blue,
    ///
    /// }
    /// if (Small Bag Blue) {
    ///     BIG PROBABILITY
    ///     Key Tan, Crown Grey, Coins Yellow, Neckalce Yellow, Ingot Yellow, Lamp Yellow, Small blue potion
    /// 
    ///     SMALL PROBABILITY  
    ///     Key Orange, Coins White, Neckalce White, Ingot White, Challice yellow, Lamp Yellow,
    ///     
    ///     VERY SMALL PROBABILITY
    ///     Key Blue, Challice yellow, Lamp White, Crown Yellow, War book blue, Spiritual book blue,
    /// }
///////////////
/// LARGE BAG
///////////////
// if (Large Bag Tan) {
//     BIG PROBABILITY
//         Key Tan, Coins Grey, Necklace Grey, Ingot Grey, Lamp Grey, Chalice Grey, Crown Grey

//     SMALL PROBABILITY     
//         Key Orange, Coins Yellow, Necklace Yellow, Ingot Yellow, Lamp Yellow, Chalice Yellow

//     VERY SMALL PROBABILITY
//         Key Orange, Key BlueCrown Yellow, Small Blue Potion
// }

// if (Large Bag Orange) {
//     BIG PROBABILITY
//         Key Tan, Coins Yellow, Necklace Yellow, Ingot Yellow, Lamp Yellow, Chalice Yellow, Crown Yellow

//     SMALL PROBABILITY  
//         Key Orange, Coins White, Necklace White, Ingot White, Chalice White, Crown Grey, War Book Blue, Spiritual Book Blue

//     VERY SMALL PROBABILITY
//         Key Blue, Lamp White, Chalice White, Crown White, Small Pink Potion
// }

// if (Large Bag Blue) {
//     BIG PROBABILITY
//         Key Orange, Coins White, Necklace White, Ingot White, Lamp White, Chalice White, Crown White, Small Blue Potion

//     SMALL PROBABILITY  
//         Key Blue, War Book Blue, Spiritual Book Blue, Large Blue Potion

//     VERY SMALL PROBABILITY
//         War Book Pink, Spiritual Book Pink, Special Book Blue
// }

// ///////////////
// /// BOX
// ///////////////
// if (Box Tan) {
//     BIG PROBABILITY
//         Coins Grey, Necklace Grey, Ingot Grey, Lamp Grey, Chalice Grey

//     SMALL PROBABILITY     
//         Crown Grey, Small Blue Potion, War Book Blue

//     VERY SMALL PROBABILITY
//         Large Blue Potion, Key Tan, Small Pink Potion
// }

// if (Box Orange) {
//     BIG PROBABILITY
//         Key Tan, Coins Yellow, Necklace Yellow, Ingot Yellow, Lamp Yellow, Chalice Yellow, Crown Grey

//     SMALL PROBABILITY  
//         Small Blue Potion, War Book Blue, Spiritual Book Blue, Bomb Weak

//     VERY SMALL PROBABILITY
//         Large Blue Potion, Key Orange, War Book Pink, Spiritual Book Pink, Bomb Mid
// }

// if (Box Blue) {
//     BIG PROBABILITY
//         Key Orange, Coins White, Necklace White, Ingot White, Lamp White, Chalice White, Crown White, Small Blue Potion

//     SMALL PROBABILITY  
//         Key Blue, War Book Blue, Spiritual Book Blue, Large Blue Potion, Bomb Mid

//     VERY SMALL PROBABILITY
//         War Book Pink, Spiritual Book Pink, Special Book Blue, Bomb Strong
// }

// ///////////////
// /// PACK
// ///////////////
// if (Pack Tan) {
//     BIG PROBABILITY
//         Coins Grey, Necklace Grey, Ingot Grey, Lamp Grey, Chalice Grey, Crown Grey, Small Blue Potion

//     SMALL PROBABILITY     
//         Key Tan, War Book Blue, Spiritual Book Blue, Large Blue Potion, Bomb Weak

//     VERY SMALL PROBABILITY
//         Key Orange, Small Pink Potion, War Book Pink, Spiritual Book Pink, Bomb Mid
// }

// if (Pack Orange) {
//     BIG PROBABILITY
//         Key Tan, Coins Yellow, Necklace Yellow, Ingot Yellow, Lamp Yellow, Chalice Yellow, Crown Yellow

//     SMALL PROBABILITY  
//         Key Orange, Small Blue Potion, War Book Blue, Spiritual Book Blue, Large Blue Potion, Bomb Mid

//     VERY SMALL PROBABILITY
//         Key Blue, War Book Pink, Spiritual Book Pink, Special Book Blue, Bomb Strong
// }

// if (Pack Blue) {
//     BIG PROBABILITY
//         Key Orange, Coins White, Necklace White, Ingot White, Lamp White, Chalice White, Crown White, Small Blue Potion, Large Blue Potion

//     SMALL PROBABILITY  
//         Key Blue, War Book Blue, Spiritual Book Blue, Special Book Blue, Bomb Strong

//     VERY SMALL PROBABILITY
//         Special Book Pink, War Book Purple, Spiritual Book Purple
// }

// ///////////////
// /// CHEST
// ///////////////
// if (Chest Tan) {
//     BIG PROBABILITY
//         Coins Grey, Necklace Grey, Ingot Grey, Lamp Grey, Chalice Grey, Crown Grey, Small Blue Potion, Large Blue Potion

//     SMALL PROBABILITY     
//         Key Tan, War Book Blue, Spiritual Book Blue, Special Book Blue, Bomb Weak

//     VERY SMALL PROBABILITY
//         Key Orange, War Book Pink, Spiritual Book Pink, Bomb Mid
// }

// if (Chest Orange) {
//     BIG PROBABILITY
//         Key Tan, Coins Yellow, Necklace Yellow, Ingot Yellow, Lamp Yellow, Chalice Yellow, Crown Yellow, Small Blue Potion, Large Blue Potion

//     SMALL PROBABILITY  
//         Key Orange, War Book Blue, Spiritual Book Blue, Special Book Blue, Bomb Mid

//     VERY SMALL PROBABILITY
//         Key Blue, War Book Pink, Spiritual Book Pink, Special Book Pink, Bomb Strong
// }

// if (Chest Blue) {
//     BIG PROBABILITY
//         Key Orange, Coins White, Necklace White, Ingot White, Lamp White, Chalice White, Crown White, Small Blue Potion, Large Blue Potion, Bomb Strong

//     SMALL PROBABILITY  
//         Key Blue, War Book Blue, Spiritual Book Blue, Special Book Blue, War Book Pink, Spiritual Book Pink, Special Book Pink

//     VERY SMALL PROBABILITY
//         War Book Purple, Spiritual Book Purple, Special Book Purple
// }



using System.Collections.Generic;
using UnityEngine;

public class ContainerLootGenerator : MonoBehaviour
{
    // Dictionary: "ContainerType-Color" => WeightedGroup[]
    private Dictionary<string, WeightedGroup[]> containerLootGroups;

    void Awake()
    {
        InitializeLootGroups();
    }

    private void InitializeLootGroups()
    {
        containerLootGroups = new Dictionary<string, WeightedGroup[]>();

        //////////////////////
        /// MONEY BELT
        //////////////////////
        // MoneyBelt-Tan
        containerLootGroups["MoneyBelt-Tan"] = new WeightedGroup[]
        {
            // BIG probability group
            new WeightedGroup(0.60f, new string[] {
                "Coins-Grey", "Necklace-Grey", "Ingot-Grey"
            }),
            // SMALL probability group
            new WeightedGroup(0.30f, new string[] {
                "Key-Tan", "Lamp-Grey", "Chalice-Grey"
            }),
            // VERY SMALL probability group
            new WeightedGroup(0.10f, new string[] {
                "Crown-Grey"
            })
        };

        // MoneyBelt-Orange
        containerLootGroups["MoneyBelt-Orange"] = new WeightedGroup[]
        {
            // BIG
            new WeightedGroup(0.50f, new string[] {
                "Coins-Grey", "Necklace-Grey", "Ingot-Grey", "Lamp-Grey"
            }),
            // SMALL
            new WeightedGroup(0.35f, new string[] {
                "Key-Tan", "Chalice-Grey", "Crown-Grey",
                "Coins-Yellow", "Necklace-Yellow", "Ingot-Yellow"
            }),
            // VERY SMALL
            new WeightedGroup(0.15f, new string[] {
                "Potion-Small-Blue"
            })
        };

        // MoneyBelt-Blue
        containerLootGroups["MoneyBelt-Blue"] = new WeightedGroup[]
        {
            // BIG
            new WeightedGroup(0.50f, new string[] {
                "Ingot-Grey", "Lamp-Grey", "Chalice-Grey", "Crown-Grey",
                "Coins-Yellow", "Necklace-Yellow", "Ingot-Yellow"
            }),
            // SMALL
            new WeightedGroup(0.35f, new string[] {
                "Potion-Small-Blue", "Key-Tan", "Key-Orange",
                "Chalice-Yellow", "Crown-Yellow", "Coins-White", "Necklace-White"
            }),
            // VERY SMALL
            new WeightedGroup(0.15f, new string[] {
                "Book-War-Blue", "Book-Spiritual-Blue"
            })
        };

        //////////////////////
        /// SMALL BAG
        //////////////////////
        // SmallBag-Tan
        containerLootGroups["SmallBag-Tan"] = new WeightedGroup[]
        {
            // BIG
            new WeightedGroup(0.60f, new string[] {
                "Coins-Grey", "Necklace-Grey", "Ingot-Grey", "Lamp-Grey", "Chalice-Grey"
            }),
            // SMALL
            new WeightedGroup(0.30f, new string[] {
                "Key-Tan", "Coins-Yellow", "Necklace-Yellow", "Ingot-Yellow", "Lamp-Yellow"
            }),
            // VERY SMALL
            new WeightedGroup(0.10f, new string[] {
                "Chalice-Yellow", "Crown-Grey"
            })
        };

        // SmallBag-Orange
        containerLootGroups["SmallBag-Orange"] = new WeightedGroup[]
        {
            // BIG
            new WeightedGroup(0.50f, new string[] {
                "Key-Tan", "Coins-Yellow", "Necklace-Yellow", "Ingot-Yellow", "Lamp-Yellow"
            }),
            // SMALL
            new WeightedGroup(0.35f, new string[] {
                "Chalice-Yellow", "Crown-Grey", "Potion-Small-Blue"
            }),
            // VERY SMALL
            new WeightedGroup(0.15f, new string[] {
                "Key-Orange", "Chalice-Yellow", "Lamp-Yellow", "Chalice-White", "Crown-Yellow",
                "Book-War-Blue", "Book-Spiritual-Blue"
            })
        };

        // SmallBag-Blue
        containerLootGroups["SmallBag-Blue"] = new WeightedGroup[]
        {
            // BIG
            new WeightedGroup(0.50f, new string[] {
                "Key-Tan", "Crown-Grey", "Coins-Yellow", "Necklace-Yellow",
                "Ingot-Yellow", "Lamp-Yellow", "Potion-Small-Blue"
            }),
            // SMALL
            new WeightedGroup(0.35f, new string[] {
                "Key-Orange", "Coins-White", "Necklace-White", "Ingot-White",
                "Chalice-Yellow", "Lamp-Yellow"
            }),
            // VERY SMALL
            new WeightedGroup(0.15f, new string[] {
                "Key-Blue", "Chalice-Yellow", "Lamp-White", "Crown-Yellow",
                "Book-War-Blue", "Book-Spiritual-Blue"
            })
        };

        //////////////////////
        /// LARGE BAG
        //////////////////////
        containerLootGroups["LargeBag-Tan"] = new WeightedGroup[]
        {
            new WeightedGroup(0.60f, new string[] {
                "Coins-Grey", "Necklace-Grey", "Ingot-Grey", "Lamp-Grey",
                "Chalice-Grey", "Crown-Grey"
            }),
            new WeightedGroup(0.30f, new string[] {
                "Key-Tan", "Coins-Yellow", "Necklace-Yellow", "Ingot-Yellow", "Lamp-Yellow"
            }),
            new WeightedGroup(0.10f, new string[] {
                "Potion-Small-Blue", "Potion-Small-Pink"
            })
        };

        containerLootGroups["LargeBag-Orange"] = new WeightedGroup[]
        {
            new WeightedGroup(0.50f, new string[] {
                "Key-Tan", "Coins-Yellow", "Necklace-Yellow", "Ingot-Yellow", "Lamp-Yellow"
            }),
            new WeightedGroup(0.35f, new string[] {
                "Chalice-Yellow", "Crown-Grey", "Potion-Small-Blue", "Potion-Small-Pink"
            }),
            new WeightedGroup(0.15f, new string[] {
                "Key-Orange", "Chalice-White", "Crown-Yellow",
                "Book-War-Pink", "Book-Spiritual-Pink", "Potion-Small-Purple"
            })
        };

        containerLootGroups["LargeBag-Blue"] = new WeightedGroup[]
        {
            new WeightedGroup(0.50f, new string[] {
                "Key-Orange", "Coins-White", "Necklace-White", "Ingot-White", "Lamp-White"
            }),
            new WeightedGroup(0.35f, new string[] {
                "Chalice-Yellow", "Crown-Yellow", "Potion-Small-Purple"
            }),
            new WeightedGroup(0.15f, new string[] {
                "Key-Blue", "Book-War-Purple", "Book-Spiritual-Purple"
            })
        };

        //////////////////////
        /// BOX
        //////////////////////
        containerLootGroups["Box-Tan"] = new WeightedGroup[]
        {
            new WeightedGroup(0.60f, new string[] {
                "Coins-Yellow", "Necklace-Yellow", "Ingot-Yellow", "Lamp-Yellow", "Chalice-Yellow"
            }),
            new WeightedGroup(0.30f, new string[] {
                "Crown-Grey", "Potion-Small-Blue", "Potion-Large-Blue"
            }),
            new WeightedGroup(0.10f, new string[] {
                "Book-War-Blue", "Book-Spiritual-Blue"
            })
        };

        containerLootGroups["Box-Orange"] = new WeightedGroup[]
        {
            new WeightedGroup(0.60f, new string[] {
                "Coins-White", "Necklace-White", "Ingot-White", "Lamp-White", "Chalice-White"
            }),
            new WeightedGroup(0.30f, new string[] {
                "Crown-Yellow", "Potion-Large-Pink"
            }),
            new WeightedGroup(0.10f, new string[] {
                "Book-War-Pink", "Book-Spiritual-Pink"
            })
        };

        containerLootGroups["Box-Blue"] = new WeightedGroup[]
        {
            new WeightedGroup(0.60f, new string[] {
                "Coins-White", "Necklace-White", "Ingot-White", "Lamp-White", "Chalice-White"
            }),
            new WeightedGroup(0.30f, new string[] {
                "Crown-White", "Potion-Large-Purple"
            }),
            new WeightedGroup(0.10f, new string[] {
                "Book-War-Purple", "Book-Spiritual-Purple"
            })
        };

        //////////////////////
        /// PACK
        //////////////////////
        containerLootGroups["Pack-Tan"] = new WeightedGroup[]
        {
            new WeightedGroup(0.60f, new string[] {
                "Coins-White", "Necklace-White", "Ingot-White", "Lamp-White", "Chalice-White"
            }),
            new WeightedGroup(0.30f, new string[] {
                "Crown-White", "Potion-Large-Pink"
            }),
            new WeightedGroup(0.10f, new string[] {
                "Book-War-Pink"
            })
        };

        containerLootGroups["Pack-Orange"] = new WeightedGroup[]
        {
            new WeightedGroup(0.60f, new string[] {
                "Coins-White", "Necklace-White", "Ingot-White", "Lamp-White", "Chalice-White"
            }),
            new WeightedGroup(0.30f, new string[] {
                "Crown-White", "Potion-Large-Purple"
            }),
            new WeightedGroup(0.10f, new string[] {
                "Book-War-Purple"
            })
        };

        containerLootGroups["Pack-Blue"] = new WeightedGroup[]
        {
            new WeightedGroup(0.60f, new string[] {
                "Coins-White", "Necklace-White", "Ingot-White", "Lamp-White", "Chalice-White"
            }),
            new WeightedGroup(0.30f, new string[] {
                "Crown-White", "Potion-Large-Purple"
            }),
            new WeightedGroup(0.10f, new string[] {
                "Book-Special-Blue"
            })
        };

        //////////////////////
        /// CHEST
        //////////////////////
        containerLootGroups["Chest-Tan"] = new WeightedGroup[]
        {
            new WeightedGroup(0.60f, new string[] {
                "Coins-White", "Necklace-White", "Ingot-White", "Lamp-White", "Chalice-White"
            }),
            new WeightedGroup(0.30f, new string[] {
                "Crown-White", "Potion-Large-Purple"
            }),
            new WeightedGroup(0.10f, new string[] {
                "Book-Special-Pink"
            })
        };

        containerLootGroups["Chest-Orange"] = new WeightedGroup[]
        {
            new WeightedGroup(0.60f, new string[] {
                "Coins-White", "Necklace-White", "Ingot-White", "Lamp-White", "Chalice-White"
            }),
            new WeightedGroup(0.30f, new string[] {
                "Crown-White", "Potion-Large-Purple"
            }),
            new WeightedGroup(0.10f, new string[] {
                "Book-Special-Purple"
            })
        };

        containerLootGroups["Chest-Blue"] = new WeightedGroup[]
        {
            new WeightedGroup(0.50f, new string[] {
                "Coins-White", "Necklace-White", "Ingot-White", "Lamp-White", "Chalice-White"
            }),
            new WeightedGroup(0.30f, new string[] {
                "Crown-White", "Potion-Large-Purple", "Key-Blue"
            }),
            new WeightedGroup(0.20f, new string[] {
                "Book-Special-Blue"
            })
        };
    }

    /// <summary>
    /// Returns ONE random item from the container’s WeightedGroups,
    /// picking which group based on group.weight, then picking
    /// a random item from that group’s array.
    /// </summary>
    public string GetRandomItem(string containerType)
    {
        if (!containerLootGroups.ContainsKey(containerType))
        {
            Debug.LogWarning($"No loot table for container type: {containerType}");
            return null;
        }

        WeightedGroup[] groups = containerLootGroups[containerType];

        // 1) Sum all group weights
        float totalWeight = 0f;
        foreach (var g in groups)
            totalWeight += g.weight;

        // 2) Pick a random point
        float r = Random.value * totalWeight;

        // 3) Find which group we fall into
        float cumulative = 0f;
        foreach (var g in groups)
        {
            cumulative += g.weight;
            if (r <= cumulative)
            {
                // Pick a random item from this group
                if (g.items == null || g.items.Length == 0)
                {
                    Debug.LogWarning($"WeightedGroup for {containerType} has no items!");
                    return null;
                }
                int index = Random.Range(0, g.items.Length);
                return g.items[index];
            }
        }

        // Fallback (should never happen if weights are set)
        return null;
    }
}





}
