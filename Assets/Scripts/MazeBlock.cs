// MazeBlock.cs
using UnityEngine;

public enum BlockColorType { Blue, Green, Tan }

public class MazeBlock : MonoBehaviour
{
    public Vector2Int gridCoordinate;
    public BlockColorType colorType;
    public int stairsCount;
    
    public MazeBlock neighborLeft;
    public MazeBlock neighborRight;
    public MazeBlock neighborBelowLeft;
    public MazeBlock neighborBelowRight;
    
    // For vertical neighbor logic:
    public BlockLegType blockLegType; // <--- store exactly which leg type (TwoLeg, OneLegLeft, etc.)
    
    // (Optional) track which slot on this floor
    public int patternSlot;
    
    // For debugging
    public string blockName;

    public string mazeName;           // Maze name (e.g., "TheHive")

    public GameObject playerCursor;

    public bool isActiveBlock = false; // NEW FLAG
    public Vector3 cursorDefaultInitialLocalPos; //Saves x=-0.6, y=0.5, z=0.2

    private void Awake()
    {
        // Attempt to find a child named "MapPlayerDot"
        Transform dotChild = transform.Find("MapPlayerDot.vox");
        if (dotChild != null)
        {
            playerCursor = dotChild.gameObject;
            playerCursor.SetActive(false); // Start all dots as hidden
        }
        else
        {
            Debug.LogWarning($"MapPlayerDot.vox child not found on {name}.");
        }

        cursorDefaultInitialLocalPos = playerCursor.transform.localPosition;
    }

    public void SetPlayerCursorActive(bool isActive)
    {
        if (isActive == false) {
            ResetPlayerCursorOnBlock();
        }
        isActiveBlock = isActive;  // Track which block is active
        if (playerCursor != null)
        {
            playerCursor.SetActive(isActive);
        }
    }

    public void ResetPlayerCursorOnBlock() {
        playerCursor.transform.localPosition = cursorDefaultInitialLocalPos;
    }
}
