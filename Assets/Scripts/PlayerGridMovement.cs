using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System.Collections;
using Unity.VisualScripting;

public class PlayerGridMovement : MonoBehaviour
{
    [SerializeField] public Button actionButton;
    [SerializeField] private Button dropButton;
    [SerializeField] private Button forwardButton;
    [SerializeField] private Button rotateLeftButton;
    [SerializeField] private Button rotateRightButton;
    [SerializeField] private Button backwardButton;
    [SerializeField] public TextMeshProUGUI actionButtonText;
    [SerializeField] private Button restButton;
    [SerializeField] private TextMeshProUGUI facingOrientationText;

    PlayerShootingSpawner playerShootingSpawner;
    GameManager gameManager;
    Player player;
    Enemy enemy;
    ItemManager itemManager;
    InventoryManager inventoryManager;
    FloorManager floorManager;

    public float gridSize = 10.0f; //Size of each grid step
    public float movementSpeed = 5.0f;
    public bool isMoving = false;
    public bool isRotating = false;
    public bool canBackStep = false;
    private Vector3 gridStart = new Vector3(-5, 2.5f, -5); // Define your custom grid start position
    private Vector3 targetPosition;
    private Quaternion targetRotation;
    private Vector3 playerPreviousPosition;
    private Quaternion playerPreviousRotation;

    public int gridX = 0; // X coordinate in the 12x12 grid
    public int gridZ = 0; // Z coordinate in the 12x12 grid
    public int currentFloor = 0; // Track which floor the player is on
    public RectTransform minimapCursor; // Assign the UI cursor from Unity Editor
    public RectTransform minimapGrid; // Parent object of the minimap grid

    private float maxInteractionDistance = 5f;
    private float doubleClickThreshold = 0.3f;
    private bool waitingForSecondClick = false;
    public bool isWaitingForBombToExplode = false;
    private Collider lastClickedCollider = null;
    private Coroutine singleClickCoroutine = null;

    void Start() {
        player = GameObject.Find("Player").GetComponent<Player>();
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        playerShootingSpawner = GameObject.Find("PlayerShootingSpawner").GetComponent<PlayerShootingSpawner>();
        itemManager = GameObject.Find("ItemManager").GetComponent<ItemManager>();
        inventoryManager = GameObject.Find("GameManager").GetComponent<InventoryManager>();
        floorManager = GameObject.Find("FloorManager").GetComponent<FloorManager>();
        // Snap player to grid center at the start
        transform.position = GetSnappedPosition(transform.position);
        targetPosition = transform.position; // Align targetPosition to snapped position
        // //Start with the backwards button disabled.
        HideBackwardButton();
        HideActionButton();
        gridX = 0; // Player starts at the NW corner (leftmost on minimap)
        gridZ = 0; // Player starts at the top row of the 12x12 grid
        //facingOrientationText.text = "East";
        UpdateFacingOrientation();
    }

    void Update() {
        if (!isMoving && !isRotating) {
            HandleInput();
            //CheckForInteractables();
        }
        MoveToTarget();
        RotateToTarget();
        PlayerRest();

        if (gameManager.isFighting)
        {
            ShowActionButton();
            HideDirectionalButtons();
        } else {
            ShowDirectionalButtons();
            CheckForInteractables();
        }

        if (isWaitingForBombToExplode) {
            HideDirectionalButtons();
            HideBackwardButton();
        }

        if (player.canRest) {
            restButton.gameObject.SetActive(true);
        } else {
            restButton.gameObject.SetActive(false);
        }

        if (canBackStep && !isWaitingForBombToExplode) {
            //backwardButton.gameObject.SetActive(true);
            ShowBackwardButton();
        } else {
            //backwardButton.gameObject.SetActive(false);
            HideBackwardButton();
        }

        // Update UI based on the current fight state:
        if (gameManager.isFighting) {
            if (gameManager.isPassiveFight) {
                // Passive fight: EvilDoor enemy.
                // Directional buttons remain visible so the player can change facing,
                // and the attack button is available until the player commits to attack.
                ShowDirectionalButtons();
                ShowActionButton();
                // Show backstep if it's the player's turn
                if (gameManager.isPlayersTurn) {
                    ShowBackwardButton();
                } else {
                    HideBackwardButton();
                }
            } 
            else { // Normal fight
                if (gameManager.isFreeAttackPhase) {
                    // During free attack phase: show only the attack button.
                    HideDirectionalButtons();
                    ShowActionButton();
                    // You may choose to show the backstep button or not during the free phase.
                } else {
                    // Standard turn-based combat:
                    HideDirectionalButtons();
                    if (gameManager.isPlayersTurn) {
                        ShowActionButton();
                        ShowBackwardButton();
                    } else if (gameManager.isEnemysTurn) {
                        HideActionButton();
                        HideBackwardButton();
                    }
                }
            }
        } else {
            // Not in a fight: exploration mode.
            ShowDirectionalButtons();
            CheckForInteractables();
        }

            // Instead of checking for a button press, check for a mouse click.
        if (Input.GetMouseButtonDown(0))
        {
            // Create a ray from the main camera through the mouse position.
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit, maxInteractionDistance))
            {
                // For items, we can immediately pick them up on a single click.
                // If the ray hits an item, trigger pickup.
                if (hit.collider.CompareTag("Item"))
                {
                    // If it's an item, we pick it up immediately.
                    itemManager.PickUpItem(hit);
                    // Also cancel any pending click for containers
                    if (singleClickCoroutine != null)
                    {
                        StopCoroutine(singleClickCoroutine);
                        waitingForSecondClick = false;
                    }
                }

                // For containers, we want to distinguish single vs. double click.
                else if (hit.collider.CompareTag("Container"))
                {
                    if (waitingForSecondClick && lastClickedCollider == hit.collider)
                    {
                        // Double click detected on the same container.
                        if (singleClickCoroutine != null)
                        {
                            StopCoroutine(singleClickCoroutine);
                        }
                        waitingForSecondClick = false;
                        Debug.Log("Try Open Container");

                        //Obtain item mapping
                        ItemMapping containerInFront;
                        Debug.Log("hit.collider.name" + hit.collider.name);
                        containerInFront = inventoryManager.GetItemMapping(hit.collider.name);
                        Debug.Log("Found" + hit.collider.name);
                        Debug.Log("isLocked?" + containerInFront.isLocked);
                        // Check if it's locked
                        if (!containerInFront.isLocked) {
                            // Open Container
                            OpenContainer(hit.collider);
                            Destroy(hit.collider.gameObject);
                        } else {
                            // Get all keys the player is holding (backpack, left hand, and right hand)
                            List<ItemMapping> allKeys = inventoryManager.GetAllKeys();
                            if (allKeys.Count == 0) {
                                // No key at all
                                Debug.Log("No key found.");
                                gameManager.SetPlayerMessage("It's locked");
                            }
                            else if (inventoryManager.HasKeyForContainer(containerInFront)) {
                                // Player has at least one key that meets the requirements.
                                Debug.Log("Key found, opening container.");
                                OpenContainer(hit.collider);
                                Destroy(hit.collider.gameObject);
                            }
                            else {
                                // The player has a key, but it's not the right quality.
                                Debug.Log("Key(s) present but not sufficient.");
                                gameManager.SetPlayerMessage("Need better key");
                            }
                        }
                    }
                    else
                    {
                        // First click on a container.
                        waitingForSecondClick = true;
                        lastClickedCollider = hit.collider;
                        // Start a coroutine that waits for a double click.
                        singleClickCoroutine = StartCoroutine(WaitForDoubleClick(hit));
                    }
                }


                // If the ray hits a ladder, trigger descend.
                else if (hit.collider.CompareTag("Ladder"))
                {
                    if (gameManager.isFighting) {
                        Debug.Log("Can't Use Ladder while fighting");
                    } else {
                        canBackStep = false;
                        Debug.Log("USED LADDER");
                        Debug.Log("canBackStep is " + canBackStep);
                        floorManager.MoveCursorVerticallyDown(hit);
                    }
                }
            }
        }
        // If there's an active enemy that's a passive (EvilDoor) enemy,
        // check if the player is still facing it. If not, reset the active enemy.
        if (gameManager.activeEnemy != null && gameManager.isPassiveFight)
        {
            Vector3 toEnemy = gameManager.activeEnemy.transform.position - transform.position;
            float angle = Vector3.Angle(transform.forward, toEnemy);
            // If the angle is greater than, say, 45 degrees, the player isn't really facing the door.
            if (angle > 45f)
            {
                gameManager.SetActiveEnemy(null);
                gameManager.enemyHPText.text = "";
            }
        }
    }


    private IEnumerator WaitForDoubleClick(RaycastHit hit)
    {
        // Wait for the double click threshold.
        yield return new WaitForSeconds(doubleClickThreshold);

         // If no second click has been detected, treat it as a single click (pickup the container).
        if (waitingForSecondClick)
        {
            // Call your pickup method for containers.
            itemManager.PickUpItem(hit);
            waitingForSecondClick = false;
            lastClickedCollider = null;
        }
    }

    private void OpenContainer(Collider containerCol)
    {
        // Here you call your method to "open" the container.
        // For example, you might show its contents:
        Debug.Log("Double-click: Opening container " + containerCol.name);
        floorManager.GetComponent<ContainerLootGenerator>().GetRandomItem(containerCol.name);
        //containerLootGenerator.OpenContainer(containerCol.gameObject);
    }


    private void UpdateFacingOrientation() {
        // Get the y-angle (in degrees) and normalize it between 0 and 360.
        float y = transform.eulerAngles.y % 360f;
        if (y < 0) y += 360f;

        // According to our mapping (0° = East, 90° = South, 180° = West, 270° = North)
        if (y < 45f || y >= 315f) {
            facingOrientationText.text = "East";
        }
        else if (y >= 45f && y < 135f) {
            facingOrientationText.text = "South";
        }
        else if (y >= 135f && y < 225f) {
            facingOrientationText.text = "West";
        }
        else if (y >= 225f && y < 315f) {
            facingOrientationText.text = "North";
        }
    }

    public void HideDirectionalButtons()
    {
        forwardButton.gameObject.SetActive(false);
        rotateLeftButton.gameObject.SetActive(false);
        rotateRightButton.gameObject.SetActive(false);
    }

    public void ShowDirectionalButtons()
    {
        forwardButton.gameObject.SetActive(true);
        rotateLeftButton.gameObject.SetActive(true);
        rotateRightButton.gameObject.SetActive(true);
    }

    public void HideActionButton() {
        actionButton.gameObject.SetActive(false);
    }

    public void ShowActionButton() {
        actionButton.gameObject.SetActive(true);
    }

    void HandleInput() {
        if (Input.GetKeyDown(KeyCode.W))
        {
            MoveForward();
        }
        else if (Input.GetKeyDown(KeyCode.S))
        {
            MoveBackwards(false);
        }

        else if (Input.GetKeyDown(KeyCode.A))
        {
            TurnLeft();
        }

        else if (Input.GetKeyDown(KeyCode.D))
        {
            TurnRight();
        }
        else if (Input.GetKeyDown(KeyCode.Space))
        {
            OpenDoorButton();
        }
    }


    public void HideBackwardButton() {
        backwardButton.gameObject.SetActive(false); //Backwards button disabled.
    }


    public void ShowBackwardButton() {
        backwardButton.gameObject.SetActive(true); //Backwards button enabled.
    }

    private bool CanMoveForward() {
        RaycastHit hit;
        float rayDistance = gridSize;

        if (Physics.Raycast(transform.position, transform.forward, out hit, rayDistance)) {
            Debug.Log("Obstacle detected: " + hit.collider.name);
            if (hit.collider.CompareTag("Item")) {
                return true; //Allow to pass through Items
            } else if (hit.collider.CompareTag("CorridorDoor")) {
                return true; //Allow to pass through Corridor Doors
            } else if (hit.collider.CompareTag("CorridorDoorEast")) {
                return true; //Allow to pass through Corridor Doors
            } else if (hit.collider.CompareTag("CorridorDoorWest")) {
                return true; //Allow to pass through Corridor Doors
            } else if (hit.collider.CompareTag("Ladder")) {
                return true; //Allow to pass through Corridor Ladders
            } else {
                return false; // Obstacle found, cannot move forward
            }
        }
        return true;
    }


    public void MoveForward()
    {
        if (isMoving || isRotating)  return; // Prevent movement if already moving or rotating
        if (CanMoveForward()) {
            isMoving = true;
            targetPosition = GetSnappedPosition(transform.position + transform.forward * gridSize);

            Debug.Log($"Moving forward from {transform.position}");

            // Track movement direction
            Vector3 forwardDir = transform.forward.normalized;
            float modifier = 0f;

            // Ensure the movement modifier applies ONLY when moving along the Z-axis (East-West)
            if (Mathf.Abs(forwardDir.z) > 0.9f && Mathf.Abs(forwardDir.x) < 0.1f) 
            {
                modifier = (forwardDir.z > 0) ? 0.1f : -0.1f; // Forward along Z = +0.1, Backward along Z = -0.1
                UpdateMinimapCursor(modifier);
                Debug.Log($"Minimap Cursor Updated with modifier {modifier}");
            }
            else
            {
                Debug.Log("Ignoring minimap cursor movement because movement is along X-axis.");
            }

            // Ensure gridX and gridZ stay within bounds
            gridX = Mathf.Clamp(gridX, 0, 11);
            gridZ = Mathf.Clamp(gridZ, 0, 11);

            EnableBackwardsStep(player.transform.position, player.transform.rotation);
            CheckForInteractables();
        }
    }


    private bool CanMoveForwardWithBlueBook() {
        RaycastHit enemyHit;
        float enemyDetectionDistance = gridSize;  // Detect items within half the grid
        float interactionDistance = gridSize;           // Detect doors/enemies one grid away

        //Vector3 rayOrigin = transform.position + new Vector3(0, -2f, 0); // Slight downward offset
        Vector3 rayOrigin = transform.position + (transform.forward * (gridSize * 0.6f)) + new Vector3(0, 1, 0); 
        Vector3 rayDirection = transform.forward;

        Debug.DrawRay(rayOrigin, rayDirection * enemyDetectionDistance, Color.blue); 

        if (Physics.Raycast(rayOrigin, rayDirection, out enemyHit, enemyDetectionDistance)) {
            if (enemyHit.collider.CompareTag("Enemy")) {
                if (IsEvilDoor(enemyHit.collider.gameObject.name)) {
                    Debug.Log("Evil door on the other side, I can teleport");
                    return true;
                } else {
                    Debug.Log("Enemy found through wall, can't teleport");
                    return false;
                }
            }
        }

        RaycastHit hit;
        float rayDistance = gridSize;
        if (Physics.Raycast(transform.position, transform.forward, out hit, rayDistance)) {
            Debug.Log("Obstacle detected w Blue Book: " + hit.collider.name + " with Tag: " + hit.collider.tag);
            if (hit.collider.CompareTag("Item")) {
                return true; //Allow to pass through Items
            } else if (hit.collider.CompareTag("CorridorDoor")) {
                return true; //Allow to pass through Corridor Doors
            } else if (hit.collider.CompareTag("Door")) {
                return true; //Allow to pass through Doors
            } else if (hit.collider.CompareTag("MazeSet")) {
                return true; //Allow to pass through Maze Walls
            } else if (hit.collider.CompareTag("CorridorDoorEast")) {
                return true; //Allow to pass through Corridor Doors
            } else if (hit.collider.CompareTag("CorridorDoorWest")) {
                return true; //Allow to pass through Corridor Doors
            } else if (hit.collider.CompareTag("Ladder")) {
                return true; //Allow to pass through Corridor Ladders
            } else if (hit.collider.CompareTag("OuterWall")) {
                return false; //Cannot pass through Outside Maze Walls
            } else if (hit.collider.CompareTag("EvilDoorWall")) {
                return false; //Cannot pass through Outside Maze Walls
            } else {
                return false; // Obstacle found, cannot move forward
            }
        }
        return true;
    }


    public void MoveForwardWithBlueBook()
    {
        if (isMoving || isRotating)  return; // Prevent movement if already moving or rotating
        if (CanMoveForwardWithBlueBook()) {
            isMoving = true;
            targetPosition = GetSnappedPosition(transform.position + transform.forward * gridSize);

            Debug.Log($"Moving forward from {transform.position}");

            // Track movement direction
            Vector3 forwardDir = transform.forward.normalized;
            float modifier = 0f;

            // Ensure the movement modifier applies ONLY when moving along the Z-axis (East-West)
            if (Mathf.Abs(forwardDir.z) > 0.9f && Mathf.Abs(forwardDir.x) < 0.1f) 
            {
                modifier = (forwardDir.z > 0) ? 0.1f : -0.1f; // Forward along Z = +0.1, Backward along Z = -0.1
                UpdateMinimapCursor(modifier);
                Debug.Log($"Minimap Cursor Updated with modifier {modifier}");
            }
            else
            {
                Debug.Log("Ignoring minimap cursor movement because movement is along X-axis.");
            }

            // Ensure gridX and gridZ stay within bounds
            gridX = Mathf.Clamp(gridX, 0, 11);
            gridZ = Mathf.Clamp(gridZ, 0, 11);

            EnableBackwardsStep(player.transform.position, player.transform.rotation);
            CheckForInteractables();
        }
    }


    public void UpdateMinimapCursor(float modifier)
    {
        //if (minimapCursor == null || minimapGrid == null) return;
        MazeBlock currentMazeBlock = FindActiveMazeBlock();
        
        if (currentMazeBlock == null)
        {
            Debug.LogWarning("No active MazeBlock found!");
            return;
        } else {
            Debug.Log("In Updateminimap CurrentMazeBlock is: " + currentMazeBlock);
        }

        if (currentMazeBlock.playerCursor == null)
        {
            Debug.LogWarning($"Player cursor missing in block {currentMazeBlock.name}!");
            return;
        } else {
            Debug.Log("In Updateminimap playerCursor is: " + currentMazeBlock.playerCursor);
        }

        // Find the actual PlayerCursor component inside the playerCursor GameObject
        PlayerCursor cursorComponent = currentMazeBlock.playerCursor.GetComponent<PlayerCursor>();
        if (cursorComponent == null)
        {
            Debug.LogWarning($"PlayerCursor script not found on {currentMazeBlock.playerCursor.name}!");
            return;
        }
        // Move the PlayerCursor object (not just playerCursor transform)
        Vector3 cursorPosition = cursorComponent.transform.localPosition;
        cursorPosition.x += modifier;
        cursorComponent.transform.localPosition = cursorPosition;

        Debug.Log($"Updated Minimap Cursor in {currentMazeBlock.name} to X: {cursorPosition.x}");
    }


    public void ResetPlayerCursorOnMiniMapOnResurrection()
    {
        MazeBlock currentMazeBlock = FindActiveMazeBlock();
        PlayerCursor cursorComponent = currentMazeBlock.playerCursor.GetComponent<PlayerCursor>();
        Vector3 cursorPosition = cursorComponent.transform.localPosition;
        cursorPosition.x = -0.6f;
        cursorComponent.transform.localPosition = cursorPosition;
    }


    public MazeBlock FindActiveMazeBlock()
    {
        MazeBlock[] allBlocks = FindObjectsByType<MazeBlock>(FindObjectsSortMode.None);

        foreach (MazeBlock block in allBlocks)
        {
            if (block.isActiveBlock)
            {
                return block; // Return the currently active block
            }
        }
        return null; // No active block found
    }
    
    private void EnableBackwardsStep(Vector3 previousPosition, Quaternion previousRotation) {
        playerPreviousPosition = previousPosition;
        playerPreviousRotation = previousRotation;
        ShowBackwardButton();
        //backwardButton.gameObject.SetActive(true);
        canBackStep = true;
    }

    public void MoveBackwards(bool isBribe)
    {
        Debug.Log("IsMoving " + isMoving + " canStepBack" + canBackStep + " isFighting" + gameManager.isFighting + " isPlayersTurn" + gameManager.isPlayersTurn + " isEnemyTurn" + gameManager.isEnemysTurn);
        if (isMoving || isRotating) return; // Prevent movement if already moving or rotating
        
        if (canBackStep && !gameManager.isFighting) { //StepBack when not fighting
            player.transform.position = playerPreviousPosition;
            player.transform.rotation = playerPreviousRotation;
            canBackStep = false;
        } else if (canBackStep && gameManager.isFighting && gameManager.isPlayersTurn) { //BATTLE ESCAPE CASE:
            int canEscape = Random.Range(0,10); //50 - 50 Chance for escaping the battle
            Debug.Log("CanEscape" + canEscape);

            if (isBribe) {
                canEscape += 10;
                Debug.Log("BRIBED!");
                gameManager.enemyHPText.gameObject.SetActive(false);
            }

            if (canEscape > 5) { // CAN ESCAPE
                Debug.Log("SUCCESSFUL ESCAPE!!");
                gameManager.SetPlayerMessage("Successful Escape!");
                gameManager.isFighting = false;
                HideActionButton();
                player.transform.position = playerPreviousPosition;
                player.transform.rotation = playerPreviousRotation;
                canBackStep = false;
                gameManager.enemyHPText.gameObject.SetActive(false);
                gameManager.ambushInProgress = false;
            } else {    // CAN'T ESCAPE
                Debug.Log("COULDN'T ESCAPE!!");
                gameManager.SetPlayerMessage("Couldn't Escape!");
                gameManager.isPlayersTurn = false;
                gameManager.isEnemysTurn = true;
                canBackStep = true;
            }
        }
    }

    public void TurnLeft()
    {
        if (isMoving || isRotating) return; // Prevent rotation if already moving or rotating
        isRotating = true;
        targetRotation = Quaternion.Euler(0, transform.eulerAngles.y - 90, 0);
    }

    public void TurnRight()
    {
        if (isMoving || isRotating) return; // Prevent rotation if already moving or rotating
        isRotating = true;
        targetRotation = Quaternion.Euler(0, transform.eulerAngles.y + 90, 0);
    }

    private void MoveToTarget()
    {
        if (isMoving)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, movementSpeed * Time.deltaTime);

            // Stop movement and snap to grid when close to target
            if (Vector3.Distance(transform.position, targetPosition) < 0.01f)
            {
                transform.position = GetSnappedPosition(targetPosition); // Ensure exact grid alignment
                isMoving = false;
            }
        }
    }

    private void RotateToTarget()
    {
        if (isRotating && !isMoving) // Ensure rotation occurs only when not moving
        {
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, 180 * Time.deltaTime);
            // Stop rotation and snap to target rotation when close
            if (Quaternion.Angle(transform.rotation, targetRotation) < 0.1f)
            {
                transform.rotation = targetRotation; // Snap to target rotation
                isRotating = false;
            }
        }
        UpdateFacingOrientation();
    }


    private Vector3 GetSnappedPosition(Vector3 position)
    {
        return new Vector3(
            Mathf.Round((position.x - gridStart.x) / gridSize) * gridSize + gridStart.x,
            gridStart.y, // Keep the fixed Y position (1 in this case)
            Mathf.Round((position.z - gridStart.z) / gridSize) * gridSize + gridStart.z
        );
    }


    public void SetPlayerImmediatePosition(Vector3 newPos, Quaternion newRot)
    {
        transform.position = newPos;
        targetPosition = newPos;
        transform.rotation = newRot;
        targetRotation = newRot;
        isMoving = false;
        isRotating = false;
    }


    private void CheckForInteractables()
    {
        if (isRotating && !isMoving) // Added this to prevent engaging with Evil Doors while turning　
            return;

        RaycastHit hit;
        float itemDetectionDistance = gridSize * 0.5f;  // Detect items within half the grid
        float interactionDistance = gridSize;           // Detect doors/enemies one grid away

        //Vector3 rayOrigin = transform.position + new Vector3(0, -2f, 0); // Slight downward offset
        Vector3 rayOrigin = transform.position + new Vector3(0, 1, 0); // Slight downward offset
        Vector3 rayDirection = transform.forward;

        // Short raycast for items
        Debug.DrawRay(rayOrigin, rayDirection * itemDetectionDistance, Color.blue); 
        
        // This is still not working right so giving up for now.
        // if (!Physics.Raycast(rayOrigin, rayDirection, out hit, itemDetectionDistance)) // This is still fucked up
        // {
        //     if (inventoryManager.CheckIfRightHandHasItem()) // Check if player is holding an item
        //     {
        //         dropButton.gameObject.SetActive(true);//dropButtonText.text = "Drop";
        //         dropButton.onClick.RemoveAllListeners();
        //         dropButton.onClick.AddListener(() => inventoryManager.DropAnItem());
        //         //return; // Return to avoid further checks
        //     } else {
        //         dropButton.gameObject.SetActive(false);//dropButtonText.text = "Drop";
        //     }
        // }

        // Long raycast for doors and enemies
        Debug.DrawRay(rayOrigin, rayDirection * interactionDistance, Color.red);

        if (Physics.Raycast(rayOrigin, rayDirection, out hit, interactionDistance))
        {
            //if (hit.collider.CompareTag("Door"))
            //{
                
                // actionButtonText.text = "Open";
                // actionButton.onClick.RemoveAllListeners();
                // actionButton.onClick.AddListener(() => OpenDoor(hit));
            //}
            //else 
            if (hit.collider.CompareTag("Enemy"))
            {
                InitiateFight(hit);
            }
        }
        else
        {
            ResetActionButton();
        }
    }


    // private void OpenDoor(RaycastHit hit) {
    //     DoorController door = hit.collider.GetComponent<DoorController>();
    //     if (door != null) {
    //         door.OpenDoor();
    //     }
    // }


    public Collider CheckForInteractablesAndReturnHitCollider()
    {
        RaycastHit hit;
        float itemDetectionDistance = gridSize * 0.5f;  // Detect items within half the grid
        float interactionDistance = gridSize;           // Detect doors/enemies one grid away

        //Vector3 rayOrigin = transform.position + new Vector3(0, -2f, 0); // Slight downward offset
        Vector3 rayOrigin = transform.position + new Vector3(0, -2, 0); // Slight downward offset
        Vector3 rayDirection = transform.forward;

        // Short raycast for items
        Debug.DrawRay(rayOrigin, rayDirection * itemDetectionDistance, Color.blue); 
        
        // This is still not working right so giving up for now.
        if (Physics.Raycast(rayOrigin, rayDirection, out hit, itemDetectionDistance)) // This is still fucked up
        {
            return hit.collider;
        } 
        return null;
    }


        public RaycastHit CheckForInteractablesAndReturnRaycastHit()
    {
        RaycastHit hit;
        float itemDetectionDistance = gridSize * 0.5f;  // Detect items within half the grid
        float interactionDistance = gridSize;           // Detect doors/enemies one grid away

        //Vector3 rayOrigin = transform.position + new Vector3(0, -2f, 0); // Slight downward offset
        Vector3 rayOrigin = transform.position + new Vector3(0, -2, 0); // Slight downward offset
        Vector3 rayDirection = transform.forward;

        // Short raycast for items
        Debug.DrawRay(rayOrigin, rayDirection * itemDetectionDistance, Color.blue); 
        
        // This is still not working right so giving up for now.
        if (Physics.Raycast(rayOrigin, rayDirection, out hit, itemDetectionDistance)) // This is still fucked up
        {
            return hit;
        } 
        return default;
    }


    public void OpenDoorButton()
    {
        RaycastHit hit;
        float rayDistance = gridSize;
        if (Physics.Raycast(transform.position, transform.forward, out hit, rayDistance))
        {
            if (hit.collider.CompareTag("Door"))
            {
                DoorController door = hit.collider.GetComponent<DoorController>();
                if (door != null)
                {
                    door.OpenDoor();
                }
            }
        }
    }

    private void ResetActionButton() {
        actionButtonText.text = "";
        actionButton.onClick.RemoveAllListeners();
    }

    // private void InitiateFight(RaycastHit hit) {
    //     enemy = hit.collider.GetComponent<Enemy>();
    //     gameManager.UpdateEnemyHP(enemy.currentEnemyHP); // Get the current HP from this enemy and pass it to GameManager for display in UI 
    //     gameManager.enemyHPText.gameObject.SetActive(true); //Make the Emeny HP label appear

    //     if (enemy != null) {
    //         gameManager.isFighting = true; //initiate fight more
    //         gameManager.SetActiveEnemy(enemy); //Register current enemy as active

    //         if (gameManager.isPlayersTurn)
    //         {
    //             ShowActionButton();
    //         }
    //         else
    //         {
    //             HideActionButton();
    //         }

    //         actionButtonText.text = "Attack";
    //         actionButton.onClick.RemoveAllListeners();
    //         actionButton.onClick.AddListener(() => playerShootingSpawner.ShootAtEnemy(enemy.transform));
    //         //backwardButton.gameObject.SetActive(true);
    //         ShowBackwardButton();
    //         backwardButton.onClick.RemoveAllListeners();
    //         backwardButton.onClick.AddListener(() => MoveBackwards(false));
    //     }
    // }


    private void InitiateFight(RaycastHit hit) {
        enemy = hit.collider.GetComponent<Enemy>();
        gameManager.UpdateEnemyHP(enemy.currentEnemyHP);
        gameManager.enemyHPText.gameObject.SetActive(true);

        if (enemy != null) {
            gameManager.isFighting = true;
            gameManager.SetActiveEnemy(enemy);

            // Determine if this enemy is an EvilDoor
            bool isPassiveDoor = IsEvilDoor(enemy.gameObject.name);
            gameManager.isPassiveFight = isPassiveDoor;

            // Reset turn flags and mark free attack phase
            gameManager.isPlayersTurn = false;
            gameManager.isEnemysTurn = false;
            gameManager.isFreeAttackPhase = true;

            // Show the attack button during the free phase
            ShowActionButton();
            actionButtonText.text = "Attack";
            actionButton.onClick.RemoveAllListeners();
            actionButton.onClick.AddListener(() => {
                // When the player attacks:
                //actionButton.interactable = false; // Disable immediately to block spamming
                gameManager.isPlayersTurn = true;
                gameManager.isFreeAttackPhase = false;
                gameManager.isPassiveFight = false; // commit to full fight
                HideDirectionalButtons();           // hide directional controls now
                playerShootingSpawner.ShootAtEnemy(enemy.transform);
            });

            // For non-passive enemies, start the free attack phase coroutine
            if (!isPassiveDoor) {
                StartCoroutine(FreeAttackPhase());
            }
            
            ShowBackwardButton();
            backwardButton.onClick.RemoveAllListeners();
            backwardButton.onClick.AddListener(() => MoveBackwards(false));
        }
    }


    private IEnumerator FreeAttackPhase() {
        float freeAttackDuration = 1f; // Adjust duration as needed
        float elapsed = 0f;
        
        while (elapsed < freeAttackDuration) {
            // If the player attacks during this phase, exit early
            if (gameManager.isPlayersTurn) {
                gameManager.isFreeAttackPhase = false;
                yield break;
            }
            elapsed += Time.deltaTime;
            yield return null;
        }
        
        // Free attack phase expired: enemy wins the initiative
        if (!gameManager.isPlayersTurn) {
            gameManager.isFreeAttackPhase = false;
            gameManager.isEnemysTurn = true;
            HideActionButton();
        }
    }
    

    public bool IsEvilDoor(string enemyName)
    {
        return enemyName.Contains("EvilDoorBlue") || 
            enemyName.Contains("EvilDoorTan") || 
            enemyName.Contains("EvilDoorYellow");
    }


    public void PlayerRest() {
        restButton.onClick.RemoveAllListeners();
        restButton.onClick.AddListener(() => player.Rest());
        restButton.gameObject.SetActive(false);
    }

    
    [Range(0f, 1f)]
    public float transparencyLevel = 0.1f; // Adjust transparency level (0 = fully transparent, 1 = fully opaque)

    public void MakeMazeSetsTransparent()
    {
        List<GameObject> objectsToModify = new List<GameObject>();

        // Find all MazeSet objects
        objectsToModify.AddRange(GameObject.FindGameObjectsWithTag("MazeSet"));

        // Find all Door objects
        objectsToModify.AddRange(GameObject.FindGameObjectsWithTag("Door"));

        // Apply transparency to all objects
        foreach (GameObject obj in objectsToModify)
        {
            Renderer rend = obj.GetComponent<Renderer>();
            if (rend != null)
            {
                Material mat = rend.material; // Get material instance
                SetTransparent(mat, transparencyLevel);
            }
        }
    }


    private void SetTransparent(Material mat, float alpha)
    {
        Color color = mat.color;
        color.a = alpha; // Adjust transparency
        mat.color = color;

        // Change material rendering mode to Transparent
        mat.SetFloat("_Mode", 3);
        mat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
        mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
        mat.SetInt("_ZWrite", 0);
        mat.DisableKeyword("_ALPHATEST_ON");
        mat.EnableKeyword("_ALPHABLEND_ON");
        mat.DisableKeyword("_ALPHAPREMULTIPLY_ON");
        mat.renderQueue = 3000;
    }


    public void RestoreMazeOpacity()
    {
        List<GameObject> objectsToModify = new List<GameObject>();

        // Find all MazeSet objects
        objectsToModify.AddRange(GameObject.FindGameObjectsWithTag("MazeSet"));

        // Find all Door objects
        objectsToModify.AddRange(GameObject.FindGameObjectsWithTag("Door"));

        // Restore original opacity
        foreach (GameObject obj in objectsToModify)
        {
            Renderer rend = obj.GetComponent<Renderer>();
            if (rend != null)
            {
                Material mat = rend.material;
                SetOpaque(mat);
            }
        }
    }


    private void SetOpaque(Material mat)
    {
        Color color = mat.color;
        color.a = 1.0f; // Fully opaque
        mat.color = color;

        // Change material rendering mode back to Opaque
        mat.SetFloat("_Mode", 0);
        mat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
        mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
        mat.SetInt("_ZWrite", 1);
        mat.DisableKeyword("_ALPHABLEND_ON");
        mat.DisableKeyword("_ALPHATEST_ON");
        mat.DisableKeyword("_ALPHAPREMULTIPLY_ON");
        mat.renderQueue = -1;
    }
}