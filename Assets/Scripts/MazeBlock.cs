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
    }

    public void SetPlayerCursorActive(bool isActive)
    {
        if (playerCursor != null)
        {
            playerCursor.SetActive(isActive);
        }
    }
}
