using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using Unity.VisualScripting;
using UnityEngine;
using System.Linq;

public class FloorManager : MonoBehaviour
{
    // [SerializeField] private Transform player; // Assign player's Transform in the Inspector
    // [SerializeField] private GameObject[] warMonsterPrefabs;       // Green: Warriors, beasts, etc.
    // [SerializeField] private GameObject[] spiritualMonsterPrefabs; // Blue: Ghosts, spirits, etc.
    [SerializeField] private GameObject[] warEnemies1to2;
    [SerializeField] private GameObject[] warEnemies3to4;
    [SerializeField] private GameObject[] warEnemies5to6;
    [SerializeField] private GameObject[] warEnemies7to8;
    [SerializeField] private GameObject[] warEnemies9to12;

    [SerializeField] private GameObject[] spiritualEnemies1to2;
    [SerializeField] private GameObject[] spiritualEnemies3to4;
    [SerializeField] private GameObject[] spiritualEnemies5to6;
    [SerializeField] private GameObject[] spiritualEnemies7to8;
    [SerializeField] private GameObject[] spiritualEnemies9to12;

    [SerializeField] private GameObject [] horribleEnemies5to6;
    [SerializeField] private GameObject [] horribleEnemies7to8;
    [SerializeField] private GameObject [] horribleEnemies9to10;
    [SerializeField] private GameObject [] horribleEnemies11to12;
    
    [SerializeField] private GameObject[] defensiveItems1to4;
    [SerializeField] private GameObject[] defensiveItems5to8;
    [SerializeField] private GameObject[] defensiveItems9to12;

    [SerializeField] private GameObject[] defensiveItems1to2;
    [SerializeField] private GameObject[] defensiveItems3to4;
    [SerializeField] private GameObject[] defensiveItems4to5;
    [SerializeField] private GameObject[] defensiveItems7to12;

    [SerializeField] private GameObject[] warMonsters1to4;
    [SerializeField] private GameObject[] warMonsters5to8;
    [SerializeField] private GameObject[] warMonsters9to12;
    
    [SerializeField] private GameObject[] spiritualMonsters1to4;
    [SerializeField] private GameObject[] spiritualMonsters5to8;
    [SerializeField] private GameObject[] spiritualMonsters9to12;
    
    [SerializeField] private GameObject[] warWeapons1to4;
    [SerializeField] private GameObject[] warWeapons5to8;
    [SerializeField] private GameObject[] warWeapons9to12;
    
    [SerializeField] private GameObject[] spiritualWeapons1to4;
    [SerializeField] private GameObject[] spiritualWeapons5to8;
    [SerializeField] private GameObject[] spiritualWeapons9to12;

    [SerializeField] private GameObject[] warWeapons1to2;
    [SerializeField] private GameObject[] warWeapons3to4;
    [SerializeField] private GameObject[] warWeapons4to5;
    [SerializeField] private GameObject[] warWeapons7to12;    
    
    [SerializeField] private GameObject[] spiritualWeapons1to2;
    [SerializeField] private GameObject[] spiritualWeapons3to4;
    [SerializeField] private GameObject[] spiritualWeapons4to5;
    [SerializeField] private GameObject[] spiritualWeapons7to12;

    [SerializeField] private GameObject[] quiverPrefab;       
    [SerializeField] private GameObject[] flourPrefab; 
    [SerializeField] private GameObject[] minotaurPrefab;

    //[SerializeField] private GameObject[] containers;   //Containers
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
    [SerializeField] private GameObject[] mazeSetsEvilDoorPrefabs;

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
    MazeGenerator mazeGenerator;
    ContainerLootGenerator containerLootGenerator;
    DifficultyLevel selectedDifficulty = GameSettings.SelectedDifficulty;
    private int BossFloor {
        get {
            return selectedDifficulty switch {
                DifficultyLevel.VeryHard => 12,
                DifficultyLevel.Hard     => 8,
                DifficultyLevel.Normal   => 4,
                DifficultyLevel.Easy     => 2,
                _                        => 12
            };
        }
    }


    MazeBlock currentNeighbourLeft;
    MazeBlock currentNeighbourRight;
    MazeBlock currentNeighbourBelowLeft;
    MazeBlock currentNeighbourBelowRight;
    private float gridSize = 10.0f; // Size of each grid square
    private float itemHeightOffset = 0.1f; // Height adjustment for items above the ground
    private float enemyHeightOffset = 0f; // Height adjustment for enemies
    private Vector2 mazeSize = new Vector2(12, 12); // The full size of the maze (outer grid)

    // Keep track of occupied grid positions
    private HashSet<Vector2Int> occupiedGridPositions = new HashSet<Vector2Int>();
    private List<Vector2Int> availableGridPositions;

    private const int maxSpawnAttempts = 100; // Limit to prevent infinite loops

    void Start()
    {
        player = GameObject.Find("Player").GetComponent<Player>();
        mazeGenerator = GameObject.Find("MazeGenerator").GetComponent<MazeGenerator>();
        playerGridMovement = GameObject.Find("Player").GetComponent<PlayerGridMovement>();
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        containerLootGenerator = GetComponent<ContainerLootGenerator>(); //This script is attached to FloorManager
    }


    private void GenerateMazeSets() {
        // Generate 4 Maze Sets
        int randomIndex;
        // Spawn maze set at mazeSpawnPoint1
        //mazeSetsEvilDoorPrefabs
        //mazeSetsPrefabs
        int mazeIndex = Random.Range(0, mazeSetsPrefabs.Length);
        int randomYRotation = Random.Range(0, 4) * 90; // 0, 90, 180, or 270 degrees
        Instantiate(mazeSetsPrefabs[mazeIndex],
                    mazeSpawnPoint1.transform.position,
                    Quaternion.Euler(0, randomYRotation, 0));

        // Spawn maze set at mazeSpawnPoint2
        mazeIndex = Random.Range(0, mazeSetsPrefabs.Length);
        randomYRotation = Random.Range(0, 4) * 90;
        Instantiate(mazeSetsPrefabs[mazeIndex],
                    mazeSpawnPoint2.transform.position,
                    Quaternion.Euler(0, randomYRotation, 0));

        // Spawn maze set at mazeSpawnPoint3
        mazeIndex = Random.Range(0, mazeSetsPrefabs.Length);
        randomYRotation = Random.Range(0, 4) * 90;
        Instantiate(mazeSetsPrefabs[mazeIndex],
                    mazeSpawnPoint3.transform.position,
                    Quaternion.Euler(0, randomYRotation, 0));

        // Spawn maze set at mazeSpawnPoint4
        mazeIndex = Random.Range(0, mazeSetsPrefabs.Length);
        randomYRotation = Random.Range(0, 4) * 90;
        Instantiate(mazeSetsPrefabs[mazeIndex],
                    mazeSpawnPoint4.transform.position,
                    Quaternion.Euler(0, randomYRotation, 0));
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
    
        gameManager.UpdateCurrentMazeBlockType(mazeGenerator.currentPlayerBlock);
    }


    public void MovePlayerToNewMazeViaCorridorDoor(string corridorDoorSide) { // Crossing Corridor Door
        MazeBlock targetBlock = null;
        
        if (gameManager.enemyHPText.gameObject != null) {
            gameManager.enemyHPText.gameObject.SetActive(false); // DID THIS TO PREVENT SHOWING A RANDOM 0 WHEN CHANGING FLOOR GLITCH
        }

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
        if (gameManager.currentFloor % 2 == 0){ 
            Transform cursorTransform = mazeGenerator.currentPlayerBlock.playerCursor.transform;
            Vector3 pos = cursorTransform.position; // get the current position
            pos.y -= 12f; // subtract 12 from y
            cursorTransform.position = pos; // assign the modified vector back
        }
        
        // Now update the player's transform immediately.
        player.GetComponent<PlayerGridMovement>().SetPlayerImmediatePosition(newPosition, newRotation);
        
        // Now regenerate the floor contents based on the new block.
        GenerateFloorContents(mazeGenerator.currentPlayerBlock.colorType, mazeGenerator.currentPlayerBlock.gridCoordinate, mazeGenerator.currentPlayerBlock, corridorDoorSide);

        if (gameManager.isSmallPurplePotionActive == true) {
            gameManager.HideAllEnemies(true);
        }
    }


    public void MoveCursorVerticallyDown(RaycastHit hit) { //Descending using a Ladder
        GameObject item = hit.collider.gameObject;
        Vector3 newPosition = player.transform.position;
        Quaternion newRotation = player.transform.rotation;
        float floorCursorPositionOffset; // Position 1 is origin West (x +0) z=5.  Posiiton 2 is ladder west (x +0.3) z=35, 3 = Ladder East (x + 0.8) z=85, 4 = End of Maze east (x + 1.1) z=115
        
        if (gameManager.enemyHPText.gameObject != null) {
            gameManager.enemyHPText.gameObject.SetActive(false); // DID THIS TO PREVENT SHOWING A RANDOM 0 WHEN CHANGING FLOOR GLITCH
        }

        if (gameManager.currentFloor == 255) {
            LoopPlayerToTopGamePlus();
            return;
        }

        if (gameManager.currentFloor >= BossFloor)
        {
            Debug.Log("Descending past boss floor " + gameManager.currentFloor);

            //Reest the cursor of the Minotaur treasure floor
            mazeGenerator.currentPlayerBlock.SetPlayerCursorActive(false);

            // 1) Move the floor counter
            player.ModifyFloorNumber();

            // 2) Pick a random color
            var allColors = (BlockColorType[]) System.Enum.GetValues(typeof(BlockColorType));
            gameManager.currentMazeBlock.colorType = allColors[Random.Range(0, allColors.Length)];

            // 3) Recompute neighbours
            PopulateCurrentNeighbours(mazeGenerator.currentPlayerBlock);

            // 4) Regenerate floor contents (two ladders, no corridor-doors)
            GenerateFloorContents(
                gameManager.currentMazeBlock.colorType,
                mazeGenerator.currentPlayerBlock.gridCoordinate,
                mazeGenerator.currentPlayerBlock,
                "NoCorridorDoorUsed"
            );

            // 5) Skip all further positioning/minimap updates
            return;
        }



        if (gameManager.currentFloor % 2 == 0) { // Changing to a different block
            Debug.Log("Used Ladder from Even Floor " + gameManager.currentFloor);
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

                // LOGIC FOR AFTER FLOOR 12
                if (gameManager.currentFloor >= BossFloor){
                    // pick a random BlockColorType
                    var all = (BlockColorType[]) System.Enum.GetValues(typeof(BlockColorType));
                    gameManager.currentMazeBlock.colorType = all[Random.Range(0, all.Length)];
                }

                PopulateCurrentNeighbours(mazeGenerator.currentPlayerBlock);
                GenerateFloorContents(mazeGenerator.currentPlayerBlock.colorType,mazeGenerator.currentPlayerBlock.gridCoordinate,mazeGenerator.currentPlayerBlock, "NoCorridorDoorUsed");                
                playerGridMovement.UpdateMinimapCursor(floorCursorPositionOffset);
                //Purple small potion effect check
                if (gameManager.isSmallPurplePotionActive == true) {
                    gameManager.HideAllEnemies(true);
                }
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

                // LOGIC FOR AFTER FLOOR 12
                if (gameManager.currentFloor >= BossFloor){
                    // pick a random BlockColorType
                    var all = (BlockColorType[]) System.Enum.GetValues(typeof(BlockColorType));
                    gameManager.currentMazeBlock.colorType = all[Random.Range(0, all.Length)];
                }

                PopulateCurrentNeighbours(mazeGenerator.currentPlayerBlock);
                GenerateFloorContents(mazeGenerator.currentPlayerBlock.colorType,mazeGenerator.currentPlayerBlock.gridCoordinate,mazeGenerator.currentPlayerBlock, "NoCorridorDoorUsed");
                playerGridMovement.UpdateMinimapCursor(floorCursorPositionOffset); //Shift X position on minimap to match the Ladder used
                //Purple small potion effect check
                if (gameManager.isSmallPurplePotionActive == true) {
                    gameManager.HideAllEnemies(true);
                }
                return;
            }
        } else { //Descending within the same block
            Debug.Log("Used Ladder from Odd Floor " + gameManager.currentFloor);
            GameObject ladder = hit.collider.gameObject;
            Debug.Log("The Ladder hit nme is " + ladder.name);
            //This will be called whenever the user descend from an ODD floor, so the top floor within a block.
            // Im still not sure why substracting 12 worked, its not the Y transform that i can see in the inspector, but for now I'll leave it.
            Transform cursorTransform = mazeGenerator.currentPlayerBlock.playerCursor.transform;
            Vector3 pos = cursorTransform.position; // get the current position
            pos.y -= 12f; // subtract 12 from y
            cursorTransform.position = pos; // assign the modified vector back
            player.ModifyFloorNumber();

            // LOGIC FOR AFTER FLOOR 12
            if (gameManager.currentFloor >= BossFloor){
                // pick a random BlockColorType
                var all = (BlockColorType[]) System.Enum.GetValues(typeof(BlockColorType));
                gameManager.currentMazeBlock.colorType = all[Random.Range(0, all.Length)];
            }

            //PopulateCurrentNeighbours(currentPlayerBlock);
            GenerateFloorContents(mazeGenerator.currentPlayerBlock.colorType,mazeGenerator.currentPlayerBlock.gridCoordinate,mazeGenerator.currentPlayerBlock, "NoCorridorDoorUsed");
            //Purple small potion effect check
            if (gameManager.isSmallPurplePotionActive == true) {
                gameManager.HideAllEnemies(true);
            }
            return;
        }
    }


    private void InitializeAvailablePositions()
    {
        // Set up the available grid positions based on your mazeSize and spawn boundaries.
        availableGridPositions = new List<Vector2Int>();
        int spawnAreaStartX = 1;
        int spawnAreaStartZ = 1;
        int spawnAreaEndX = (int)mazeSize.x - 2;
        int spawnAreaEndZ = (int)mazeSize.y - 2;

        for (int x = spawnAreaStartX; x <= spawnAreaEndX; x++)
        {
            for (int z = spawnAreaStartZ; z <= spawnAreaEndZ; z++)
            {
                availableGridPositions.Add(new Vector2Int(x, z));
            }
        }
    }


    // Returns a reserved grid position and removes it from availableGridPositions.
    private Vector2Int ReserveGridPosition()
    {
        if (availableGridPositions.Count == 0)
        {
            Debug.LogWarning("No available grid positions left!");
            return new Vector2Int(0, 0); // Fallback or error handling.
        }
        int index = Random.Range(0, availableGridPositions.Count);
        Vector2Int pos = availableGridPositions[index];
        availableGridPositions.RemoveAt(index);
        return pos;
    }


    // Converts a grid position to a world position.
    private Vector3 GridToWorld(Vector2Int gridPos, float heightOffset)
    {
        return new Vector3(
            gridPos.x * gridSize + gridSize * 0.5f,
            heightOffset,
            gridPos.y * gridSize + gridSize * 0.5f
        );
    }


    // Modified SpawnObjects that uses ReserveGridPosition.
    private void SpawnObjects(GameObject[] prefabs, int count, float heightOffset, string type, bool isItem)
    {
        Debug.Log("Spawning Objects of type: " + type);
        for (int i = 0; i < count; i++)
        {
            // Reserve a grid position from the pool.
            Vector2Int reservedPos = ReserveGridPosition();

            // Convert the reserved grid position to world position.
            Vector3 worldPosition = GridToWorld(reservedPos, heightOffset);

            // Select and instantiate a random prefab.
            GameObject randomPrefab = prefabs[Random.Range(0, prefabs.Length)];
            GameObject spawnedObject = Instantiate(randomPrefab, worldPosition, Quaternion.identity);

            // Remove unwanted components if needed.
            Projectile projectileComponent = spawnedObject.GetComponent<Projectile>();
            if (projectileComponent != null)
            {
                Destroy(projectileComponent);
            }
        }
    }


    private void SpawnLadder(string side)
    {
        // Pick a ladder from the designated array.
        int randomInt = Random.Range(0, (side == "East" ? eastLadders.Length : westLadders.Length));
        GameObject ladderObj = (side == "East" ? eastLadders[randomInt] : westLadders[randomInt]);

        // Activate it.
        ladderObj.SetActive(true);

        // Use its preset position.
        Vector3 worldPosition = ladderObj.transform.position;

        // Mark its grid cell as taken by removing it from availableGridPositions.
        int gx = Mathf.FloorToInt(worldPosition.x / gridSize);
        int gz = Mathf.FloorToInt(worldPosition.z / gridSize);
        Vector2Int ladderGrid = new Vector2Int(gx, gz);
        if (availableGridPositions != null && availableGridPositions.Contains(ladderGrid))
        {
            availableGridPositions.Remove(ladderGrid);
        }
    }


    private void ReserveFixedLadderPositions()
    {
        // Suppose your fixed ladder positions are determined by the ladder prefabs’ positions.
        // Loop through the ladder arrays and remove their grid cell from availableGridPositions.
        foreach (GameObject ladder in westLadders)
        {
            int gx = Mathf.FloorToInt(ladder.transform.position.x / gridSize);
            int gz = Mathf.FloorToInt(ladder.transform.position.z / gridSize);
            Vector2Int pos = new Vector2Int(gx, gz);
            availableGridPositions.Remove(pos);
        }
        foreach (GameObject ladder in eastLadders)
        {
            int gx = Mathf.FloorToInt(ladder.transform.position.x / gridSize);
            int gz = Mathf.FloorToInt(ladder.transform.position.z / gridSize);
            Vector2Int pos = new Vector2Int(gx, gz);
            availableGridPositions.Remove(pos);
        }
    }


////////
////// LEGACY METHODS NO LONGER NEEDED UNLESS I EVER WANT TO ENFORCE extra spacing (for example, to keep enemies a certain distance from each other) 
///////
    // private bool IsPositionOccupied(Vector2Int position)
    // {
    //     return occupiedGridPositions.Contains(position);
    // }

    // private void MarkGridAsOccupied(Vector2Int position)
    // {
    //     occupiedGridPositions.Add(position);
    // }


    // // Check if the position is directly adjacent (N, S, E, W) to any enemy
    // private bool IsDirectlyAdjacentToEnemy(Vector2Int position)
    // {
    //     Vector2Int[] adjacentOffsets = { new Vector2Int(0, 1), new Vector2Int(0, -1), new Vector2Int(1, 0), new Vector2Int(-1, 0) };
    //     foreach (var offset in adjacentOffsets)
    //     {
    //         Vector2Int adjacentPosition = position + offset;
    //         if (occupiedGridPositions.Contains(adjacentPosition))
    //         {
    //             return true;
    //         }
    //     }
    //     return false;
    // }


    // // Check if the position is adjacent (including diagonals) to any enemy
    // private bool IsAdjacentToEnemy(Vector2Int position)
    // {
    //     Vector2Int[] adjacentOffsets = {
    //         new Vector2Int(0, 1), new Vector2Int(0, -1), new Vector2Int(1, 0), new Vector2Int(-1, 0), // Direct neighbors
    //         new Vector2Int(1, 1), new Vector2Int(-1, -1), new Vector2Int(1, -1), new Vector2Int(-1, 1) // Diagonals
    //     };
    //     foreach (var offset in adjacentOffsets)
    //     {
    //         Vector2Int adjacentPosition = position + offset;
    //         if (occupiedGridPositions.Contains(adjacentPosition))
    //         {
    //             return true;
    //         }
    //     }
    //     return false;
    // }


    public void GenerateFloorContents(BlockColorType blockColor, Vector2Int startPosition, MazeBlock currentBlock, string corridorDoorSide)
    {
        occupiedGridPositions.Clear(); //Clear previous occupied positions so thigns keep spawning on new floors
        Debug.Log($"Generating floor contents for {currentBlock.name} with color {blockColor} at {startPosition}");
        // (1) Clear previous floor content (you can implement ClearFloorContents() to destroy all spawned items, enemies, doors, ladders, etc.)
        ClearFloorContents();
        GenerateMazeSets();
         // Initialize the pool of available grid positions for the new floor.
        InitializeAvailablePositions();
        
        ReserveFixedLadderPositions();

        gameManager.UpdateCurrentMazeBlockType(currentBlock);

        // (2) Determine how many items/enemies to spawn and which prefab arrays to use based on blockColor.
        int itemCount, enemyCount, quiverCount, flourCount, defensiveItemsCount, weaponsCount;
        GameObject[] itemPrefabs = null, enemyPrefabs = null, defensiveItemsPrefabs = null, warWeaponsPrefabs = null, spiritualWeaponsPrefabs = null;

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
        if (gameManager.currentFloor >= BossFloor) {  //DONT SPAWN CORRIDOR DOORS FROM LEVEL 12
            westDoorTan.SetActive(false);
            westDoorGreen.SetActive(false);
            westDoorBlue.SetActive(false);
            eastDoorTan.SetActive(false);
            eastDoorGreen.SetActive(false);
            eastDoorBlue.SetActive(false);
        }

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
        if (gameManager.currentFloor >= BossFloor) {
            // From floor 13 on, always two ladders and no corridor doors
            SpawnLadder("West");
            SpawnLadder("East");
        }
        else if (gameManager.currentFloor % 2 == 0) {
            // Even floor: spawn one ladder based on neighboring blocks.
            if (currentBlock.neighborBelowLeft != null)
                SpawnLadder("West");
            else if (currentBlock.neighborBelowRight != null)
                SpawnLadder("East");
        } else {
            // Odd floor: always spawn both.
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

        //Generate 5 containers and store in containerPrefabArray
        GameObject[] containerPrefabArray = new GameObject[5];
        for (int i = 0; i < containerPrefabArray.Length; i++) {
            containerPrefabArray[i] = containerLootGenerator.GenerateAContainer(gameManager.currentFloor);
            Debug.Log("Container is: " + containerPrefabArray[i]);
        }

        quiverCount = 1;
        flourCount = 2;
        enemyCount = 8;
        defensiveItemsCount = 2;
        weaponsCount = 8;

        //////
        // GREEN FLOOR
        if (blockColor == BlockColorType.Green && gameManager.currentFloor <= 4) {
            //defensiveItemsPrefabs = defensiveItems1to4;
            //warWeaponsPrefabs = warWeapons1to4;  
            //enemyPrefabs = warMonsters1to4;
        } else if (blockColor == BlockColorType.Green && gameManager.currentFloor >= 5 && gameManager.currentFloor <= 8) {
            //defensiveItemsPrefabs = defensiveItems5to8;
            //warWeaponsPrefabs = warWeapons5to8;
            //enemyPrefabs = warMonsters5to8;
        } else if (blockColor == BlockColorType.Green && gameManager.currentFloor >= 9) {
            //defensiveItemsPrefabs = defensiveItems9to12;
            //warWeaponsPrefabs = warWeapons9to12;
            //enemyPrefabs = warMonsters9to12;
        }
        //////
        // BLUE FLOOR
        if (blockColor == BlockColorType.Blue && gameManager.currentFloor <= 4) {
            //defensiveItemsPrefabs = defensiveItems1to4;
            //spiritualWeaponsPrefabs = spiritualWeapons1to4;
            //enemyPrefabs = spiritualMonsters1to4;
        } else if (blockColor == BlockColorType.Blue && gameManager.currentFloor >= 5 && gameManager.currentFloor <= 8) {
            //defensiveItemsPrefabs = defensiveItems5to8;
            //spiritualWeaponsPrefabs = spiritualWeapons5to8;
            //enemyPrefabs = spiritualMonsters5to8;
        } else if (blockColor == BlockColorType.Blue && gameManager.currentFloor >= 9) {
            //defensiveItemsPrefabs = defensiveItems9to12;
            //spiritualWeaponsPrefabs = spiritualWeapons9to12;
            //enemyPrefabs = spiritualMonsters9to12;
        }
        //////
        // TAN FLOOR
        if (blockColor == BlockColorType.Tan && gameManager.currentFloor <= 4) {
            //defensiveItemsPrefabs = defensiveItems1to4;
            //warWeaponsPrefabs = warWeapons1to4;  
            //spiritualWeaponsPrefabs = spiritualWeapons1to4;
            //enemyPrefabs = warMonsters1to4.Concat(spiritualMonsters1to4).ToArray();
        } else if (blockColor == BlockColorType.Tan && gameManager.currentFloor >= 5 && gameManager.currentFloor <= 8) {
            //defensiveItemsPrefabs = defensiveItems5to8;
            //warWeaponsPrefabs = warWeapons5to8;
            //spiritualWeaponsPrefabs = spiritualWeapons5to8;
            //enemyPrefabs = warMonsters5to8.Concat(spiritualMonsters5to8).ToArray();
        } else if (blockColor == BlockColorType.Tan && gameManager.currentFloor >= 9) {
            //defensiveItemsPrefabs = defensiveItems9to12;
            //warWeaponsPrefabs = warWeapons9to12;
            //spiritualWeaponsPrefabs = spiritualWeapons9to12;
            //enemyPrefabs = warMonsters9to12.Concat(spiritualMonsters9to12).ToArray();
        }

        // Spawn items and enemies at positions inside the current MazeBlock
        //  1 Quiver
        SpawnObjects(quiverPrefab, quiverCount,itemHeightOffset,"Item",true);//, currentBlock.transform);
        //  2 Sacks of Flour
        SpawnObjects(flourPrefab, flourCount,itemHeightOffset,"Item",true);

        // Spawn Minotaur at floor 12, 16 and then randomly after. On lower diffiuclties is similar, boss floor + 4 then anytime
        switch (selectedDifficulty)
        {
            case DifficultyLevel.VeryHard:
            case DifficultyLevel.Hard:
            case DifficultyLevel.Normal:
            case DifficultyLevel.Easy:
            {
                // first boss at BossFloor, second guaranteed at BossFloor+4
                int firstBoss  = BossFloor;
                int secondBoss = firstBoss + 4;

                if (gameManager.currentFloor == firstBoss ||
                    gameManager.currentFloor == secondBoss)
                {
                    // guaranteed 1 minotaur
                    SpawnObjects(minotaurPrefab, 1, enemyHeightOffset, "enemy", false);
                }
                else if (gameManager.currentFloor > secondBoss)
                {
                    // from then on, roll 0–2 each floor
                    SpawnObjects(minotaurPrefab, Random.Range(0, 3), enemyHeightOffset, "enemy", false);
                }
                break;
            }
        }

        //WEAPONS & ENEMIES SPAWN
        // OLD LOGIC WITH ARRAYS OF 4 FLOORS EACH:
        // if (blockColor == BlockColorType.Tan) {
        //     //  4 Weapons War
        //     SpawnObjects(warWeaponsPrefabs, 4,itemHeightOffset,"item",true);
        //     //  4 Weapons Spiritual
        //     SpawnObjects(spiritualWeaponsPrefabs, 4,itemHeightOffset,"item",true);
        //     // 8 Mixed enemies
        //     SpawnObjects(enemyPrefabs, 8, enemyHeightOffset, "Enemy", false);//, currentBlock.transform);
        //     // 4 Spiritual enemies
        //     //SpawnObjects(spiritualMonsterPrefabs, 4, enemyHeightOffset, "Enemy", false);//, currentBlock.transform);
        // } else if (blockColor == BlockColorType.Green) {
        //     //  8 Weapons War
        //     SpawnObjects(warWeaponsPrefabs, weaponsCount,itemHeightOffset,"item",true);
        //     // 4 War enemies
        //     SpawnObjects(enemyPrefabs, enemyCount, enemyHeightOffset, "Enemy", false);//, currentBlock.transform);
        // } else if (blockColor == BlockColorType.Blue) {
        //     //  8 Weapons Spiritual
        //     SpawnObjects(spiritualWeaponsPrefabs, weaponsCount,itemHeightOffset,"item",true);
        //     // 4 Spiritual enemies
        //     SpawnObjects(enemyPrefabs, enemyCount, enemyHeightOffset, "Enemy", false);//, currentBlock.transform);
        // }


        /////
        /// SPAWN FLOOR WEAPONS
        // --- pick the right weapon pools by floor ---
        int floor = gameManager.currentFloor;
        GameObject[] warWeaponsPool;
        GameObject[] spiritualWeaponsPool;

        if (floor <= 2)
        {
            warWeaponsPool        = warWeapons1to2;
            spiritualWeaponsPool  = spiritualWeapons1to2;
        }
        else if (floor <= 4)
        {
            warWeaponsPool        = warWeapons3to4;
            spiritualWeaponsPool  = spiritualWeapons3to4;
        }
        else if (floor <= 6)
        {
            // (your “4to5” array here covers floors 5–6)
            warWeaponsPool        = warWeapons4to5;
            spiritualWeaponsPool  = spiritualWeapons4to5;
        }
        else
        {
            warWeaponsPool        = warWeapons7to12;
            spiritualWeaponsPool  = spiritualWeapons7to12;
        }


        // ——— NEW WEAPON SPAWN ———
        if (blockColor == BlockColorType.Tan) {
            // split evenly
            SpawnObjects(warWeaponsPool,        weaponsCount/2, itemHeightOffset, "item", true);
            SpawnObjects(spiritualWeaponsPool,  weaponsCount/2, itemHeightOffset, "item", true);
        }
        else if (blockColor == BlockColorType.Green) {
            SpawnObjects(warWeaponsPool,        weaponsCount,   itemHeightOffset, "item", true);
        }
        else if (blockColor == BlockColorType.Blue) {
            SpawnObjects(spiritualWeaponsPool,  weaponsCount,   itemHeightOffset, "item", true);
        }



        // if (blockColor == BlockColorType.Tan) {
        //     //  4 Weapons War
        //     SpawnObjects(warWeaponsPrefabs, 4,itemHeightOffset,"item",true);
        //     //  4 Weapons Spiritual
        //     SpawnObjects(spiritualWeaponsPrefabs, 4,itemHeightOffset,"item",true);
        // }  else if (blockColor == BlockColorType.Green) {
        //     //  8 Weapons War
        //     SpawnObjects(warWeaponsPrefabs, weaponsCount,itemHeightOffset,"item",true);
        // } else if (blockColor == BlockColorType.Blue) {
        //     SpawnObjects(spiritualWeaponsPrefabs, weaponsCount,itemHeightOffset,"item",true);
        // }


        // ——— NEW WEAPONS & ENEMIES SPAWN ———
        const int TOTAL_ENEMIES = 8;
        //int floor = gameManager.currentFloor;

        // 1) Figure out how many Horrible monsters to spawn
        int horribleCount = 0;
        GameObject[] horriblePool = null;
        if      (floor >= 6 && floor <= 7)  { horribleCount = 1; horriblePool = horribleEnemies5to6; }
        else if (floor >= 8 && floor <= 9)  { horribleCount = 2; horriblePool = horribleEnemies7to8; }
        else if (floor >= 10 && floor <= 11) { horribleCount = 3; horriblePool = horribleEnemies9to10; }
        else if (floor >= 12){ horribleCount = 3; horriblePool = horribleEnemies11to12; }

        // 2) Build the enemy base pool depending on floor and block color
        GameObject[] basePool;
        switch(blockColor)
        {
            case BlockColorType.Green:
                if      (floor <= 2) basePool = warEnemies1to2;
                else if (floor <= 4) basePool = warEnemies3to4;
                else if (floor <= 6) basePool = warEnemies5to6;
                else if (floor <= 8) basePool = warEnemies7to8;
                else                 basePool = warEnemies9to12;
                break;

            case BlockColorType.Blue:
                if      (floor <= 2) basePool = spiritualEnemies1to2;
                else if (floor <= 4) basePool = spiritualEnemies3to4;
                else if (floor <= 6) basePool = spiritualEnemies5to6;
                else if (floor <= 8) basePool = spiritualEnemies7to8;
                else                 basePool = spiritualEnemies9to12;
                break;

            case BlockColorType.Tan:
                GameObject[] warPool, spiritPool;
                if      (floor <= 2) { warPool = warEnemies1to2; spiritPool = spiritualEnemies1to2; }
                else if (floor <= 4) { warPool = warEnemies3to4; spiritPool = spiritualEnemies3to4; }
                else if (floor <= 6) { warPool = warEnemies5to6; spiritPool = spiritualEnemies5to6; }
                else if (floor <= 8) { warPool = warEnemies7to8; spiritPool = spiritualEnemies7to8; }
                else                 { warPool = warEnemies9to12; spiritPool = spiritualEnemies9to12; }
                basePool = warPool.Concat(spiritPool).ToArray();
                break;

            default:
                basePool = new GameObject[0];
                break;
        }

        // 3) Spawn Horrible monsters first
        if (horribleCount > 0 && horriblePool != null)
        {
            SpawnObjects(horriblePool, horribleCount, enemyHeightOffset, "Enemy", false);
        }

        // 4) Spawn normal monsters
        int normalCount = TOTAL_ENEMIES - horribleCount;
        if (normalCount > 0)
        {
            SpawnObjects(basePool, normalCount, enemyHeightOffset, "Enemy", false);
        }

        //  2 Defensive Items war and spirtual mixed OK
        // pick your defensive‐item pool purely by floor number
        //GameObject[] defensiveItemsPrefabs;
        //int floor = gameManager.currentFloor;

        if (floor <= 2)
        {
            defensiveItemsPrefabs = defensiveItems1to2;
        }
        else if (floor <= 4)
        {
            defensiveItemsPrefabs = defensiveItems3to4;
        }
        else if (floor <= 6)
        {
            defensiveItemsPrefabs = defensiveItems4to5;  // if that really represents floors 5–6
        }
        else
        {
            defensiveItemsPrefabs = defensiveItems7to12;
        }

        // 2 Defensive Items
        SpawnObjects(defensiveItemsPrefabs, defensiveItemsCount, itemHeightOffset, "item", true);

        //  5 Containers
        SpawnObjects(containerPrefabArray, 5,itemHeightOffset,"Container",true);

        if (gameManager.isMazeTransparent) {
            playerGridMovement.MakeMazeSetsTransparent();
            // The original Coroutine to deactivate this spell stays active so no need to worry about it here.
        }
    }


    private void ClearFloorContents() {
        string[] itemEnemyTags = new string[] { "Item", "Enemy", "MazeSet", "MazeEntranceDoorWall", "Container", "Smoke" };

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


    public void LoopPlayerToTopGamePlus() {
        Debug.Log("GamePLUS!");

        // 0) Update Completed Maze Counter
        gameManager.CompletedMazesCounterIncrease();

        // 1) Reset floor counter 
        player.ModifyFloorNumber();

        // 2) Clear the old cursor AND its active‐flag
        var oldBlock = mazeGenerator.currentPlayerBlock;

        oldBlock.ResetPlayerCursorOnBlock(); //Returns Cursor Dot back to initial position on block that is no longer active


        if (oldBlock != null)
            oldBlock.SetPlayerCursorActive(false); 
            oldBlock.ResetPlayerCursorOnBlock();


        // 3) Jump back to the designated start block
        MazeBlock start = mazeGenerator.startBlock;

        mazeGenerator.currentPlayerBlock = start;
        
         // 4) Teleport the player GameObject to that top‐floor location
        player.PlacePlayerAtStartOnGamePlus();


       // 5) Regenerate floor contents & recompute neighbours
        gameManager.currentMazeBlock = start;
        gameManager.UpdateCurrentMazeBlockType(start);
        
        GenerateFloorContents(
            mazeGenerator.startBlock.colorType,
            mazeGenerator.startBlock.gridCoordinate,
            mazeGenerator.startBlock,
            "NoCorridorDoorUsed"
        );
        PopulateCurrentNeighbours(mazeGenerator.startBlock);

        // 7) hook back into your cursor-move routines
        mazeGenerator.UpdatePlayerCursor(start);

    }

}
