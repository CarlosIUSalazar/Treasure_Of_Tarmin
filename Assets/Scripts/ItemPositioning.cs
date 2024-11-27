using UnityEngine;

public class ItemPositioning : MonoBehaviour
{
    public Transform player; // The player's transform
    public float offsetDistance = 4.5f; // Distance from the center of the grid square to the edge
    public float gridSize = 10.0f; // Size of a grid square

    private Vector3 gridCenter; // The center of the item's grid square

    void Start()
    {
        // Dynamically find the player in the scene
        if (player == null)
        {
            GameObject playerObject = GameObject.FindWithTag("Player");
            if (playerObject != null)
            {
                player = playerObject.transform;
            }
            else
            {
                Debug.LogError("Player not found! Ensure the player has the 'Player' tag.");
            }
        }

        // Store the initial position as the grid center
        gridCenter = transform.position;
    }

    void Update()
    {
        if (player != null && IsPlayerInSameGridSquare())
        {
            UpdateItemPosition();
        }
        else
        {
            // Reset to grid center when player leaves the square
            transform.position = gridCenter;
        }
    }

    bool IsPlayerInSameGridSquare()
    {
        // Determine the player's grid square
        int playerGridX = Mathf.FloorToInt(player.position.x / gridSize);
        int playerGridZ = Mathf.FloorToInt(player.position.z / gridSize);

        // Determine the item's grid square
        int itemGridX = Mathf.FloorToInt(gridCenter.x / gridSize);
        int itemGridZ = Mathf.FloorToInt(gridCenter.z / gridSize);

        // Check if they are in the same grid square
        return playerGridX == itemGridX && playerGridZ == itemGridZ;
    }

    void UpdateItemPosition()
    {
        // Get the player's forward direction
        Vector3 playerForward = player.forward;

        // Normalize the forward direction to get the primary axis (north, south, east, or west)
        if (Mathf.Abs(playerForward.x) > Mathf.Abs(playerForward.z))
        {
            // Facing east or west
            transform.position = gridCenter + new Vector3(Mathf.Sign(playerForward.x) * offsetDistance, 0, 0);
        }
        else
        {
            // Facing north or south
            transform.position = gridCenter + new Vector3(0, 0, Mathf.Sign(playerForward.z) * offsetDistance);
        }
    }
}
