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
}
