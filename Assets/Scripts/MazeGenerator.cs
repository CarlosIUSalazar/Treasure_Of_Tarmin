using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;

public enum BlockLegType { TwoLeg, OneLegLeft, OneLegRight }
public enum DifficultyLevel { VeryHard, Hard, Normal, Easy }

// Place this struct outside the MazeGenerator class.
public struct VerticalNeighborMapping
{
    // For the given floor’s active blocks (in order of appearance in the boolean pattern),
    // these values indicate which active block (by its active index) in the lower floor this block connects to.
    // Use –1 to indicate “no connection.”
    public int leftNeighborIndex;
    public int rightNeighborIndex;
}


public class MazeGenerator : MonoBehaviour
{
    public MazeBlock currentPlayerBlock; // Track the current block where playerCursor is
    FloorManager floorManager;
    GameManager gameManager;
    // [Header("Difficulty")]
    // public DifficultyLevel difficulty = DifficultyLevel.VeryHard;
    private DifficultyLevel difficulty;

    // Store top-floor blocks for player placement
    private List<MazeBlock> topFloorBlocks = new List<MazeBlock>();

    [Header("Block Variants (3 Colors × 3 Leg Types)")]
    // BLUE variants:
    public GameObject MazeBlue2Stairs;
    public GameObject MazeBlue1StairLeft;
    public GameObject MazeBlue1StairRight;
    // GREEN variants:
    public GameObject MazeGreen2Floors;
    public GameObject MazeGreen1FloorLeft;
    public GameObject MazeGreen1FloorRight;
    // TAN variants:
    public GameObject MazeTan2Stairs;
    public GameObject MazeTan1FloorLeft;
    public GameObject MazeTan1FloorRight;

    [Header("Treasure Prefabs for Floor 1 (bottom)")]
    public GameObject MazeTan2StairsTreasure;
    public GameObject MazeGreen2StairsTreasure;

    [Header("Spacing & Layout")]
    public float verticalSpacing = 0.84f;
    public float generationDelay = 0;
    [Tooltip("Horizontal spacing between slot centers")]
    public float horizontalSpacing = 0.63f;

    // floorPositions will be built based on the number of floors (which depends on difficulty).
    private List<(float xLeft, float xRight)>[] floorPositions;

    // Maze patterns and their predetermined leg type arrays.
    private List<bool[][]> allPatterns = new List<bool[][]>();
    private List<BlockLegType[][]> allPatternLegTypes = new List<BlockLegType[][]>();

    // For VeryHard mode, we have individual names; for other difficulties, we use one name.
    private string[] veryHardNames = { "Skull", "Toilet", "Floating Mickey", "Pong Paddle", "Peace and Love", "The Bow", "Cat Paw", "Olimpic Rings" };
    private string[] hardNames = { "TheShield", "TheZorro", "TheHive", "TheCherries", "TheGrapes" };
    private string[] normalNames = { "Normal Maze" }; // New array for Normal pattern
    private string[] easyNames = { "Easy Maze" }; // New array for Easy pattern

    private int chosenPatternIndex = 0;

    private Dictionary<int, VerticalNeighborMapping[][]> verticalNeighborMappings = new Dictionary<int, VerticalNeighborMapping[][]>();


    void Awake()
    {
        // Clear previous data to prevent overlap
        verticalNeighborMappings.Clear();
        allPatterns.Clear();
        allPatternLegTypes.Clear();

        // Use the difficulty from GameSettings
        DifficultyLevel selectedDifficulty = GameSettings.SelectedDifficulty;

    // --- HARD-CODED VERTICAL NEIGHBOR MAPPING FOR THE "SKULL" PATTERN (pattern index 0) ---
    // (The following mapping is written in terms of active blocks on each floor.)
    // Very Hard mappings (0–7)
    VerticalNeighborMapping[][] mappingPattern1 = new VerticalNeighborMapping[6][];
    // Floor 0: no lower neighbor.
    mappingPattern1[0] = null;

    // Floor 1: pat1[1] = { true, true } → two active blocks.
    mappingPattern1[1] = new VerticalNeighborMapping[2];
    mappingPattern1[1][0] = new VerticalNeighborMapping { leftNeighborIndex = 0, rightNeighborIndex = 0 };
    mappingPattern1[1][1] = new VerticalNeighborMapping { leftNeighborIndex = 0, rightNeighborIndex = 0 };

    // Floor 2: pat1[2] = { true, false, false, false, true } → active blocks at indices 0 and 4.
    mappingPattern1[2] = new VerticalNeighborMapping[2];
    mappingPattern1[2][0] = new VerticalNeighborMapping { leftNeighborIndex = 0, rightNeighborIndex = 0 }; // for active slot 0
    mappingPattern1[2][1] = new VerticalNeighborMapping { leftNeighborIndex = 1, rightNeighborIndex = 1 }; // for active slot 4

    // Floor 3: pat1[3] = { true, false, true, false, true, false, true } → active at slots 0,2,4,6.
    mappingPattern1[3] = new VerticalNeighborMapping[4];
    mappingPattern1[3][0] = new VerticalNeighborMapping { leftNeighborIndex = 0, rightNeighborIndex = 0 }; // from slot 0 → lower active index 0
    mappingPattern1[3][1] = new VerticalNeighborMapping { leftNeighborIndex = 0, rightNeighborIndex = 0 }; // from slot 2 → lower active index 0
    mappingPattern1[3][2] = new VerticalNeighborMapping { leftNeighborIndex = 1, rightNeighborIndex = 1 }; // from slot 4 → lower active index 1
    mappingPattern1[3][3] = new VerticalNeighborMapping { leftNeighborIndex = 1, rightNeighborIndex = 1 }; // from slot 6 → lower active index 1

    // Floor 4: pat1[4] = { true, false, false, false, true, false, false, false, true } → active at slots 0,4,8.
    mappingPattern1[4] = new VerticalNeighborMapping[3];
    mappingPattern1[4][0] = new VerticalNeighborMapping { leftNeighborIndex = 0, rightNeighborIndex = 0 }; // from slot 0 → Floor 3 active index 0 (block at column 3)
    mappingPattern1[4][1] = new VerticalNeighborMapping { leftNeighborIndex = 2, rightNeighborIndex = 1 }; // from slot 4 → **Modified**: left → active index 2 (block at column 7), right → active index 1 (block at column 5)
    mappingPattern1[4][2] = new VerticalNeighborMapping { leftNeighborIndex = 3, rightNeighborIndex = 3 }; // from slot 8 → Floor 3 active index 3 (block at column 9)

    // Floor 5: pat1[5] = { true, false, true, false, true, false, true, false, true, false, true } → active at slots 0,2,4,6,8,10.
    mappingPattern1[5] = new VerticalNeighborMapping[6];
    mappingPattern1[5][0] = new VerticalNeighborMapping { leftNeighborIndex = 0, rightNeighborIndex = 0 }; // from slot 0 → Floor 4 active index 0
    mappingPattern1[5][1] = new VerticalNeighborMapping { leftNeighborIndex = 0, rightNeighborIndex = 0 }; // from slot 2 → Floor 4 active index 0
    mappingPattern1[5][2] = new VerticalNeighborMapping { leftNeighborIndex = 1, rightNeighborIndex = 1 }; // from slot 4 → Floor 4 active index 1
    mappingPattern1[5][3] = new VerticalNeighborMapping { leftNeighborIndex = 1, rightNeighborIndex = 1 }; // from slot 6 → Floor 4 active index 1
    mappingPattern1[5][4] = new VerticalNeighborMapping { leftNeighborIndex = 2, rightNeighborIndex = 2 }; // from slot 8 → Floor 4 active index 2
    mappingPattern1[5][5] = new VerticalNeighborMapping { leftNeighborIndex = 2, rightNeighborIndex = 2 }; // from slot 10 → Floor 4 active index 2

    // Store the mapping for pattern index 0.
    verticalNeighborMappings[0] = mappingPattern1;


    // "Toilet" (index 1)
    VerticalNeighborMapping[][] mappingPattern2 = new VerticalNeighborMapping[6][];
    // Floor 0: No lower neighbor.
    mappingPattern2[0] = null;

    // Floor 1: Two active blocks – both connect down to Floor0’s only block.
    mappingPattern2[1] = new VerticalNeighborMapping[2];
    mappingPattern2[1][0] = new VerticalNeighborMapping { leftNeighborIndex = 0, rightNeighborIndex = 0 };
    mappingPattern2[1][1] = new VerticalNeighborMapping { leftNeighborIndex = 0, rightNeighborIndex = 0 };

    // Floor 2: Two active blocks – let the first connect to Floor1’s active index 0, and the second to active index 1.
    mappingPattern2[2] = new VerticalNeighborMapping[2];
    mappingPattern2[2][0] = new VerticalNeighborMapping { leftNeighborIndex = 0, rightNeighborIndex = 0 };
    mappingPattern2[2][1] = new VerticalNeighborMapping { leftNeighborIndex = 1, rightNeighborIndex = 1 };

    // Floor 3: Three active blocks – map as follows:
    mappingPattern2[3] = new VerticalNeighborMapping[3];
    mappingPattern2[3][0] = new VerticalNeighborMapping { leftNeighborIndex = 0, rightNeighborIndex = 0 };
    mappingPattern2[3][1] = new VerticalNeighborMapping { leftNeighborIndex = 1, rightNeighborIndex = 1 };
    mappingPattern2[3][2] = new VerticalNeighborMapping { leftNeighborIndex = 1, rightNeighborIndex = 1 };

    // Floor 4: Four active blocks – map them to Floor3’s three active blocks.
    mappingPattern2[4] = new VerticalNeighborMapping[4];
    mappingPattern2[4][0] = new VerticalNeighborMapping { leftNeighborIndex = 0, rightNeighborIndex = 0 };
    mappingPattern2[4][1] = new VerticalNeighborMapping { leftNeighborIndex = 0, rightNeighborIndex = 0 };
    mappingPattern2[4][2] = new VerticalNeighborMapping { leftNeighborIndex = 1, rightNeighborIndex = 1 };
    mappingPattern2[4][3] = new VerticalNeighborMapping { leftNeighborIndex = 2, rightNeighborIndex = 2 };

    // Floor 5: Six active blocks – map them to Floor4’s four active blocks.
    mappingPattern2[5] = new VerticalNeighborMapping[6];
    mappingPattern2[5][0] = new VerticalNeighborMapping { leftNeighborIndex = 0, rightNeighborIndex = 0 };
    mappingPattern2[5][1] = new VerticalNeighborMapping { leftNeighborIndex = 1, rightNeighborIndex = 0 };
    mappingPattern2[5][2] = new VerticalNeighborMapping { leftNeighborIndex = 1, rightNeighborIndex = 1 };
    mappingPattern2[5][3] = new VerticalNeighborMapping { leftNeighborIndex = 2, rightNeighborIndex = 2 };
    mappingPattern2[5][4] = new VerticalNeighborMapping { leftNeighborIndex = 3, rightNeighborIndex = 3 };
    mappingPattern2[5][5] = new VerticalNeighborMapping { leftNeighborIndex = 3, rightNeighborIndex = 3 };

    // Finally, store the mapping (assuming your Toilet maze is at pattern index 1):
    verticalNeighborMappings[1] = mappingPattern2;


    // "Floating Mickey" (index 2)
    // ----- Vertical mapping for Pattern3: "FLOATING MICKY" -----
    VerticalNeighborMapping[][] mappingPattern3 = new VerticalNeighborMapping[6][];

    // Floor 0: no lower neighbor.
    mappingPattern3[0] = null;

    // Floor 1 (2 active blocks):
    mappingPattern3[1] = new VerticalNeighborMapping[2];
    mappingPattern3[1][0] = new VerticalNeighborMapping { leftNeighborIndex = 0, rightNeighborIndex = 0 };
    mappingPattern3[1][1] = new VerticalNeighborMapping { leftNeighborIndex = 0, rightNeighborIndex = 0 };

    // Floor 2 (2 active blocks):
    mappingPattern3[2] = new VerticalNeighborMapping[2];
    mappingPattern3[2][0] = new VerticalNeighborMapping { leftNeighborIndex = 0, rightNeighborIndex = 0 };
    mappingPattern3[2][1] = new VerticalNeighborMapping { leftNeighborIndex = 1, rightNeighborIndex = 1 };

    // Floor 3 (4 active blocks):
    mappingPattern3[3] = new VerticalNeighborMapping[4];
    mappingPattern3[3][0] = new VerticalNeighborMapping { leftNeighborIndex = 0, rightNeighborIndex = 0 };
    mappingPattern3[3][1] = new VerticalNeighborMapping { leftNeighborIndex = 0, rightNeighborIndex = 0 };
    mappingPattern3[3][2] = new VerticalNeighborMapping { leftNeighborIndex = 1, rightNeighborIndex = 1 };
    mappingPattern3[3][3] = new VerticalNeighborMapping { leftNeighborIndex = 1, rightNeighborIndex = 1 };

    // Floor 4 (4 active blocks):
    mappingPattern3[4] = new VerticalNeighborMapping[4];
    mappingPattern3[4][0] = new VerticalNeighborMapping { leftNeighborIndex = 0, rightNeighborIndex = 0 };
    mappingPattern3[4][1] = new VerticalNeighborMapping { leftNeighborIndex = 1, rightNeighborIndex = 1 };
    mappingPattern3[4][2] = new VerticalNeighborMapping { leftNeighborIndex = 2, rightNeighborIndex = 2 };
    mappingPattern3[4][3] = new VerticalNeighborMapping { leftNeighborIndex = 3, rightNeighborIndex = 3 };

    // Floor 5 (5 active blocks):
    mappingPattern3[5] = new VerticalNeighborMapping[5];
    mappingPattern3[5][0] = new VerticalNeighborMapping { leftNeighborIndex = 0, rightNeighborIndex = 0 };
    mappingPattern3[5][1] = new VerticalNeighborMapping { leftNeighborIndex = 0, rightNeighborIndex = 0 };
    mappingPattern3[5][2] = new VerticalNeighborMapping { leftNeighborIndex = 1, rightNeighborIndex = 1 };
    mappingPattern3[5][3] = new VerticalNeighborMapping { leftNeighborIndex = 2, rightNeighborIndex = 2 };
    mappingPattern3[5][4] = new VerticalNeighborMapping { leftNeighborIndex = 3, rightNeighborIndex = 3 };

    // Finally, store the mapping for pattern index 2:
    verticalNeighborMappings[2] = mappingPattern3;



    // Create the mapping array for Pattern4 ("Pong Paddle")
    VerticalNeighborMapping[][] mappingPattern4 = new VerticalNeighborMapping[6][];
    // Floor 0: no lower neighbor.
    mappingPattern4[0] = null;

    // Floor 1 (2 active blocks, both connect to Floor0’s only block):
    mappingPattern4[1] = new VerticalNeighborMapping[2];
    mappingPattern4[1][0] = new VerticalNeighborMapping { leftNeighborIndex = 0, rightNeighborIndex = 0 };
    mappingPattern4[1][1] = new VerticalNeighborMapping { leftNeighborIndex = 0, rightNeighborIndex = 0 };

    // Floor 2 (2 active blocks; Floor2 active 0→Floor1 active 0, active 1→Floor1 active 1):
    mappingPattern4[2] = new VerticalNeighborMapping[2];
    mappingPattern4[2][0] = new VerticalNeighborMapping { leftNeighborIndex = 0, rightNeighborIndex = 0 };
    mappingPattern4[2][1] = new VerticalNeighborMapping { leftNeighborIndex = 1, rightNeighborIndex = 1 };

    // Floor 3 (3 active blocks; Floor3 active 0→Floor2 active 0, 
    mappingPattern4[3] = new VerticalNeighborMapping[3];
    mappingPattern4[3][0] = new VerticalNeighborMapping { leftNeighborIndex = 0, rightNeighborIndex = 0 };
    mappingPattern4[3][1] = new VerticalNeighborMapping { leftNeighborIndex = 1, rightNeighborIndex = 0 };
    mappingPattern4[3][2] = new VerticalNeighborMapping { leftNeighborIndex = 1, rightNeighborIndex = 1 };

    // Floor 4 (4 active blocks; map them one‐to‐one with Floor3, except split the middle):
    mappingPattern4[4] = new VerticalNeighborMapping[4];
    mappingPattern4[4][0] = new VerticalNeighborMapping { leftNeighborIndex = 0, rightNeighborIndex = 0 };
    mappingPattern4[4][1] = new VerticalNeighborMapping { leftNeighborIndex = 1, rightNeighborIndex = 1 };
    mappingPattern4[4][2] = new VerticalNeighborMapping { leftNeighborIndex = 2, rightNeighborIndex = 1 };
    mappingPattern4[4][3] = new VerticalNeighborMapping { leftNeighborIndex = 2, rightNeighborIndex = 2 };

    // Floor 5 (5 active blocks; for example):
    mappingPattern4[5] = new VerticalNeighborMapping[5];
    mappingPattern4[5][0] = new VerticalNeighborMapping { leftNeighborIndex = 0, rightNeighborIndex = 0 };
    mappingPattern4[5][1] = new VerticalNeighborMapping { leftNeighborIndex = 0, rightNeighborIndex = 0 };
    mappingPattern4[5][2] = new VerticalNeighborMapping { leftNeighborIndex = 1, rightNeighborIndex = 1 };
    mappingPattern4[5][3] = new VerticalNeighborMapping { leftNeighborIndex = 3, rightNeighborIndex = 2 };
    mappingPattern4[5][4] = new VerticalNeighborMapping { leftNeighborIndex = 3, rightNeighborIndex = 3 };

    verticalNeighborMappings[3] = mappingPattern4;



    // Create the vertical mapping for Pattern5 ("Peace and Love")
    VerticalNeighborMapping[][] mappingPattern5 = new VerticalNeighborMapping[6][];

    // Floor 0: no lower neighbor.
    mappingPattern5[0] = null;

    // Floor 1 (2 active blocks):
    mappingPattern5[1] = new VerticalNeighborMapping[2];
    mappingPattern5[1][0] = new VerticalNeighborMapping { leftNeighborIndex = 0, rightNeighborIndex = 0 };
    mappingPattern5[1][1] = new VerticalNeighborMapping { leftNeighborIndex = 0, rightNeighborIndex = 0 };

    // Floor 2 (2 active blocks – from positions 0 and 4):
    mappingPattern5[2] = new VerticalNeighborMapping[2];
    mappingPattern5[2][0] = new VerticalNeighborMapping { leftNeighborIndex = 0, rightNeighborIndex = 0 };
    mappingPattern5[2][1] = new VerticalNeighborMapping { leftNeighborIndex = 1, rightNeighborIndex = 1 };

    // Floor 3 (4 active blocks – from positions 0,2,4,6):
    mappingPattern5[3] = new VerticalNeighborMapping[4];
    mappingPattern5[3][0] = new VerticalNeighborMapping { leftNeighborIndex = 0, rightNeighborIndex = 0 };
    mappingPattern5[3][1] = new VerticalNeighborMapping { leftNeighborIndex = 0, rightNeighborIndex = 0 };
    mappingPattern5[3][2] = new VerticalNeighborMapping { leftNeighborIndex = 1, rightNeighborIndex = 1 };
    mappingPattern5[3][3] = new VerticalNeighborMapping { leftNeighborIndex = 1, rightNeighborIndex = 1 };

    // Floor 4 (5 active blocks – from positions 0,2,4,6,8):
    mappingPattern5[4] = new VerticalNeighborMapping[5];
    mappingPattern5[4][0] = new VerticalNeighborMapping { leftNeighborIndex = 0, rightNeighborIndex = 0 };
    mappingPattern5[4][1] = new VerticalNeighborMapping { leftNeighborIndex = 1, rightNeighborIndex = 0 };
    mappingPattern5[4][2] = new VerticalNeighborMapping { leftNeighborIndex = 2, rightNeighborIndex = 1 };
    mappingPattern5[4][3] = new VerticalNeighborMapping { leftNeighborIndex = 3, rightNeighborIndex = 2 };
    mappingPattern5[4][4] = new VerticalNeighborMapping { leftNeighborIndex = 3, rightNeighborIndex = 3 };

    // Floor 5 (6 active blocks – from positions 0,2,4,6,8,10):
    mappingPattern5[5] = new VerticalNeighborMapping[6];
    mappingPattern5[5][0] = new VerticalNeighborMapping { leftNeighborIndex = 0, rightNeighborIndex = 0 };
    mappingPattern5[5][1] = new VerticalNeighborMapping { leftNeighborIndex = 1, rightNeighborIndex = 1 };
    mappingPattern5[5][2] = new VerticalNeighborMapping { leftNeighborIndex = 3, rightNeighborIndex = 2};
    mappingPattern5[5][3] = new VerticalNeighborMapping { leftNeighborIndex = 4, rightNeighborIndex = 3 };
    mappingPattern5[5][4] = new VerticalNeighborMapping { leftNeighborIndex = 4, rightNeighborIndex = 4 };

    // Finally, store this mapping under the appropriate pattern index (for example, if Pattern5 is index 4):
    verticalNeighborMappings[4] = mappingPattern5;


    // Create the vertical neighbor mapping for Pattern6 ("The Bow")
    // (Remember: these numbers refer to the active block order on the lower floor.)
    VerticalNeighborMapping[][] mappingPattern6 = new VerticalNeighborMapping[6][];

    // Floor 0: no lower neighbor.
    mappingPattern6[0] = null;

    // Floor 1 (2 active blocks): Both blocks on Floor 1 connect down to the single block on Floor 0.
    mappingPattern6[1] = new VerticalNeighborMapping[2];
    mappingPattern6[1][0] = new VerticalNeighborMapping { leftNeighborIndex = 0, rightNeighborIndex = 0 };
    mappingPattern6[1][1] = new VerticalNeighborMapping { leftNeighborIndex = 0, rightNeighborIndex = 0 };

    // Floor 2 (2 active blocks): Map Floor 2 active block 0 to Floor 1 active 0, and block 1 to Floor 1 active 1.
    mappingPattern6[2] = new VerticalNeighborMapping[2];
    mappingPattern6[2][0] = new VerticalNeighborMapping { leftNeighborIndex = 0, rightNeighborIndex = 0 };
    mappingPattern6[2][1] = new VerticalNeighborMapping { leftNeighborIndex = 1, rightNeighborIndex = 1 };

    // Floor 3 (3 active blocks): With only 2 blocks below, we “split” the middle connection.
    mappingPattern6[3] = new VerticalNeighborMapping[3];
    mappingPattern6[3][0] = new VerticalNeighborMapping { leftNeighborIndex = 0, rightNeighborIndex = 0 };
    mappingPattern6[3][1] = new VerticalNeighborMapping { leftNeighborIndex = 0, rightNeighborIndex = 0 };
    mappingPattern6[3][2] = new VerticalNeighborMapping { leftNeighborIndex = 1, rightNeighborIndex = 1 };

    // Floor 4 (4 active blocks): Map the 4 blocks from Floor 4 onto the 3 active blocks on Floor 3.
    mappingPattern6[4] = new VerticalNeighborMapping[4];
    mappingPattern6[4][0] = new VerticalNeighborMapping { leftNeighborIndex = 0, rightNeighborIndex = 0 };
    mappingPattern6[4][1] = new VerticalNeighborMapping { leftNeighborIndex = 1, rightNeighborIndex = 0 };
    mappingPattern6[4][2] = new VerticalNeighborMapping { leftNeighborIndex = 2, rightNeighborIndex = 2 };
    mappingPattern6[4][3] = new VerticalNeighborMapping { leftNeighborIndex = 2, rightNeighborIndex = 2 };

    // Floor 5 (5 active blocks): Map these onto the 4 active blocks on Floor 4.
    mappingPattern6[5] = new VerticalNeighborMapping[5];
    mappingPattern6[5][0] = new VerticalNeighborMapping { leftNeighborIndex = 0, rightNeighborIndex = 0 };
    mappingPattern6[5][1] = new VerticalNeighborMapping { leftNeighborIndex = 1, rightNeighborIndex = 0 };
    mappingPattern6[5][2] = new VerticalNeighborMapping { leftNeighborIndex = 2, rightNeighborIndex = 2 };
    mappingPattern6[5][3] = new VerticalNeighborMapping { leftNeighborIndex = 3, rightNeighborIndex = 2 };
    mappingPattern6[5][4] = new VerticalNeighborMapping { leftNeighborIndex = 3, rightNeighborIndex = 3 };


    verticalNeighborMappings[5] = mappingPattern6;


    // --- VerticalNeighborMapping for Pattern7 ("Cat Paw") ---
    // (Indices refer to the active block order on that floor.)
    VerticalNeighborMapping[][] mappingPattern7 = new VerticalNeighborMapping[6][];

    // Floor 0: no lower neighbor.
    mappingPattern7[0] = null;

    // Floor 1 (2 active blocks): both connect down to the single block on Floor 0.
    mappingPattern7[1] = new VerticalNeighborMapping[2];
    mappingPattern7[1][0] = new VerticalNeighborMapping { leftNeighborIndex = 0, rightNeighborIndex = 0 };
    mappingPattern7[1][1] = new VerticalNeighborMapping { leftNeighborIndex = 0, rightNeighborIndex = 0 };

    // Floor 2 (2 active blocks): 
    // Map the first active block to Floor 1 active index 0 and the second to Floor 1 active index 1.
    mappingPattern7[2] = new VerticalNeighborMapping[2];
    mappingPattern7[2][0] = new VerticalNeighborMapping { leftNeighborIndex = 0, rightNeighborIndex = 0 };
    mappingPattern7[2][1] = new VerticalNeighborMapping { leftNeighborIndex = 1, rightNeighborIndex = 1 };

    // Floor 3 (4 active blocks): 
    // For simplicity, we map the first two active blocks to Floor 2 active index 0 and the last two to Floor 2 active index 1.
    mappingPattern7[3] = new VerticalNeighborMapping[4];
    mappingPattern7[3][0] = new VerticalNeighborMapping { leftNeighborIndex = 0, rightNeighborIndex = 0 };
    mappingPattern7[3][1] = new VerticalNeighborMapping { leftNeighborIndex = 0, rightNeighborIndex = 0 };
    mappingPattern7[3][2] = new VerticalNeighborMapping { leftNeighborIndex = 1, rightNeighborIndex = 1 };
    mappingPattern7[3][3] = new VerticalNeighborMapping { leftNeighborIndex = 1, rightNeighborIndex = 1 };

    // Floor 4 (4 active blocks):
    // Here we do a one-to-one mapping from Floor 4 to Floor 3.
    mappingPattern7[4] = new VerticalNeighborMapping[4];
    mappingPattern7[4][0] = new VerticalNeighborMapping { leftNeighborIndex = 0, rightNeighborIndex = 0 };
    mappingPattern7[4][1] = new VerticalNeighborMapping { leftNeighborIndex = 1, rightNeighborIndex = 0 };
    mappingPattern7[4][2] = new VerticalNeighborMapping { leftNeighborIndex = 2, rightNeighborIndex = 2 };
    mappingPattern7[4][3] = new VerticalNeighborMapping { leftNeighborIndex = 3, rightNeighborIndex = 3 };

    // Floor 5 (5 active blocks):
    // We have 5 active blocks on Floor 5 and 4 on Floor 4, so we “split” the connections.
    mappingPattern7[5] = new VerticalNeighborMapping[5];
    mappingPattern7[5][0] = new VerticalNeighborMapping { leftNeighborIndex = 0, rightNeighborIndex = 0 };
    mappingPattern7[5][1] = new VerticalNeighborMapping { leftNeighborIndex = 1, rightNeighborIndex = 1 };
    mappingPattern7[5][2] = new VerticalNeighborMapping { leftNeighborIndex = 2, rightNeighborIndex = 2 };
    mappingPattern7[5][3] = new VerticalNeighborMapping { leftNeighborIndex = 3, rightNeighborIndex = 3 };
    mappingPattern7[5][4] = new VerticalNeighborMapping { leftNeighborIndex = 3, rightNeighborIndex = 3 };


    verticalNeighborMappings[6] = mappingPattern7;  // (Replace 6 with the proper index for Pattern7 in your setup)



    // --- VerticalNeighborMapping for Pattern8 ("Olimpic Rings") ---
    // (Indices refer only to the active block order on that floor.)

    VerticalNeighborMapping[][] mappingPattern8 = new VerticalNeighborMapping[6][];

    // Floor 0: no lower neighbor.
    mappingPattern8[0] = null;

    // Floor 1 (2 active blocks): both connect down to Floor 0’s only block.
    mappingPattern8[1] = new VerticalNeighborMapping[2];
    mappingPattern8[1][0] = new VerticalNeighborMapping { leftNeighborIndex = 0, rightNeighborIndex = 0 };
    mappingPattern8[1][1] = new VerticalNeighborMapping { leftNeighborIndex = 0, rightNeighborIndex = 0 };

    // Floor 2 (3 active blocks): map so that the first goes to Floor 1’s block 0, the middle splits between blocks 0 and 1, and the third goes to block 1.
    mappingPattern8[2] = new VerticalNeighborMapping[3];
    mappingPattern8[2][0] = new VerticalNeighborMapping { leftNeighborIndex = 0, rightNeighborIndex = 0 };
    mappingPattern8[2][1] = new VerticalNeighborMapping { leftNeighborIndex = 1, rightNeighborIndex = 0 };
    mappingPattern8[2][2] = new VerticalNeighborMapping { leftNeighborIndex = 1, rightNeighborIndex = 1 };

    // Floor 3 (3 active blocks): a one‐to‐one mapping from Floor 3 to Floor 2.
    mappingPattern8[3] = new VerticalNeighborMapping[3];
    mappingPattern8[3][0] = new VerticalNeighborMapping { leftNeighborIndex = 0, rightNeighborIndex = 0 };
    mappingPattern8[3][1] = new VerticalNeighborMapping { leftNeighborIndex = 1, rightNeighborIndex = 1 };
    mappingPattern8[3][2] = new VerticalNeighborMapping { leftNeighborIndex = 2, rightNeighborIndex = 2 };

    // Floor 4 (4 active blocks): map from Floor 4 to Floor 3. For example:
    mappingPattern8[4] = new VerticalNeighborMapping[4];
    mappingPattern8[4][0] = new VerticalNeighborMapping { leftNeighborIndex = 0, rightNeighborIndex = 0 };  // first active block → Floor 3 block 0
    mappingPattern8[4][1] = new VerticalNeighborMapping { leftNeighborIndex = 1, rightNeighborIndex = 1 };  // second active block splits between Floor 3 blocks 0 and 1
    mappingPattern8[4][2] = new VerticalNeighborMapping { leftNeighborIndex = 1, rightNeighborIndex = 1 };  // third active block → Floor 3 block 1
    mappingPattern8[4][3] = new VerticalNeighborMapping { leftNeighborIndex = 2, rightNeighborIndex = 2 };  // fourth active block → Floor 3 block 2

    // Floor 5 (5 active blocks): map from Floor 5 to Floor 4. For example:
    mappingPattern8[5] = new VerticalNeighborMapping[5];
    mappingPattern8[5][0] = new VerticalNeighborMapping { leftNeighborIndex = 0, rightNeighborIndex = 0 };  // first active block → Floor 4 block 0
    mappingPattern8[5][1] = new VerticalNeighborMapping { leftNeighborIndex = 1, rightNeighborIndex = 0 };  // second active block splits between Floor 4 blocks 0 and 1
    mappingPattern8[5][2] = new VerticalNeighborMapping { leftNeighborIndex = 1, rightNeighborIndex = 1 };  // third active block → Floor 4 block 1
    mappingPattern8[5][3] = new VerticalNeighborMapping { leftNeighborIndex = 2, rightNeighborIndex = 2 };  // fourth active block → Floor 4 block 2
    mappingPattern8[5][4] = new VerticalNeighborMapping { leftNeighborIndex = 3, rightNeighborIndex = 3 };  // fifth active block → Floor 4 block 3

    // Finally, store the mapping (assuming Pattern8 is at index 7 in your dictionary):
    verticalNeighborMappings[7] = mappingPattern8;


    /////////////////
    ///HARD DUNGEONS
    ////////////////

    // --- VerticalNeighborMapping for Pattern9 "The SHIELD" ---
    // (These indices refer to the order of active blocks on the lower floor.)
    // We need one mapping array per floor (floors 0–3):
    VerticalNeighborMapping[][] mappingPattern9 = new VerticalNeighborMapping[4][];

    // Floor 0: no lower neighbor.
    mappingPattern9[0] = null;

    // Floor 1: 2 active blocks → both connect to Floor0’s only block.
    mappingPattern9[1] = new VerticalNeighborMapping[2];
    mappingPattern9[1][0] = new VerticalNeighborMapping { leftNeighborIndex = 0, rightNeighborIndex = 0 };
    mappingPattern9[1][1] = new VerticalNeighborMapping { leftNeighborIndex = 0, rightNeighborIndex = 0 };

    // Floor 2: 2 active blocks → map the first to Floor1’s active block 0 and the second to active block 1.
    mappingPattern9[2] = new VerticalNeighborMapping[2];
    mappingPattern9[2][0] = new VerticalNeighborMapping { leftNeighborIndex = 0, rightNeighborIndex = 0 };
    mappingPattern9[2][1] = new VerticalNeighborMapping { leftNeighborIndex = 1, rightNeighborIndex = 1 };

    // Floor 3: 3 active blocks → 
    mappingPattern9[3] = new VerticalNeighborMapping[4];
    mappingPattern9[3][0] = new VerticalNeighborMapping { leftNeighborIndex = 0, rightNeighborIndex = 0 };
    mappingPattern9[3][1] = new VerticalNeighborMapping { leftNeighborIndex = 0, rightNeighborIndex = 0 };
    mappingPattern9[3][2] = new VerticalNeighborMapping { leftNeighborIndex = 1, rightNeighborIndex = 1 };
    mappingPattern9[3][3] = new VerticalNeighborMapping { leftNeighborIndex = 1, rightNeighborIndex = 1 };

    // Finally, store this mapping under the appropriate pattern index (Pattern6 is index 5 in your VeryHard names):
    verticalNeighborMappings[8] = mappingPattern9; // "TheShield"


    // --- VerticalNeighborMapping for Pattern10 ("TheZorro") ---
    // (This mapping is for a 4–floor maze: floors 0–3.)
    VerticalNeighborMapping[][] mappingPattern10 = new VerticalNeighborMapping[4][];

    // Floor 0: no lower neighbor.
    mappingPattern10[0] = null;

    // Floor 1: 2 active blocks – both simply connect down to the single block on Floor 0.
    mappingPattern10[1] = new VerticalNeighborMapping[2];
    mappingPattern10[1][0] = new VerticalNeighborMapping { leftNeighborIndex = 0, rightNeighborIndex = 0 };
    mappingPattern10[1][1] = new VerticalNeighborMapping { leftNeighborIndex = 0, rightNeighborIndex = 0 };

    // Floor 2: 2 active blocks – map the first to Floor 1’s active block 0 and the second to active block 1.
    mappingPattern10[2] = new VerticalNeighborMapping[2];
    mappingPattern10[2][0] = new VerticalNeighborMapping { leftNeighborIndex = 0, rightNeighborIndex = 0 };
    mappingPattern10[2][1] = new VerticalNeighborMapping { leftNeighborIndex = 1, rightNeighborIndex = 1 };

    // Floor 3: 3 active blocks – here we “split” the middle connection.
    mappingPattern10[3] = new VerticalNeighborMapping[3];
    mappingPattern10[3][0] = new VerticalNeighborMapping { leftNeighborIndex = 0, rightNeighborIndex = 0 };
    mappingPattern10[3][1] = new VerticalNeighborMapping { leftNeighborIndex = 1, rightNeighborIndex = 0 };
    mappingPattern10[3][2] = new VerticalNeighborMapping { leftNeighborIndex = 1, rightNeighborIndex = 1 };

    // (Later you store this array in your dictionary under the appropriate key for Pattern H2.)
    verticalNeighborMappings[9] = mappingPattern10; // "TheZorro"


    // Vertical mapping for Pattern H3: "TheHive"
    // (We have 4 floors: floors 0 to 3.)
    VerticalNeighborMapping[][] mappingPattern11 = new VerticalNeighborMapping[4][];
    // Floor 0: no lower neighbor
    mappingPattern11[0] = null;
    // Floor 1: 2 active blocks → both connect to Floor 0’s only block
    mappingPattern11[1] = new VerticalNeighborMapping[2];
    mappingPattern11[1][0] = new VerticalNeighborMapping { leftNeighborIndex = 0, rightNeighborIndex = 0 }; // Slot 0 → Floor 0 active 0
    mappingPattern11[1][1] = new VerticalNeighborMapping { leftNeighborIndex = 0, rightNeighborIndex = 0 }; // Slot 1 → Floor 0 active 0
    // Floor 2: 3 active blocks
    mappingPattern11[2] = new VerticalNeighborMapping[3];
    mappingPattern11[2][0] = new VerticalNeighborMapping { leftNeighborIndex = 0, rightNeighborIndex = 0 }; // Slot 0 → Floor 1 active 0
    mappingPattern11[2][1] = new VerticalNeighborMapping { leftNeighborIndex = 1, rightNeighborIndex = 0 }; // Slot 2 → Floor 1 active 1 (left), active 0 (right)
    mappingPattern11[2][2] = new VerticalNeighborMapping { leftNeighborIndex = 1, rightNeighborIndex = 1 }; // Slot 4 → Floor 1 active 1
    // Floor 3: 3 active blocks → corrected mapping
    mappingPattern11[3] = new VerticalNeighborMapping[3];
    mappingPattern11[3][0] = new VerticalNeighborMapping { leftNeighborIndex = 0, rightNeighborIndex = 0 }; // Slot 0 → Floor 2 active 0
    mappingPattern11[3][1] = new VerticalNeighborMapping { leftNeighborIndex = 1, rightNeighborIndex = 1 }; // Slot 3 → Floor 2 active 1 (both sides)
    mappingPattern11[3][2] = new VerticalNeighborMapping { leftNeighborIndex = 2, rightNeighborIndex = 2 }; // Slot 6 → Floor 2 active 2

    verticalNeighborMappings[10] = mappingPattern11; // "The Hive"

    //Cherries
    VerticalNeighborMapping[][] mappingPattern12 = new VerticalNeighborMapping[4][];
    mappingPattern12[0] = null;
    mappingPattern12[1] = new VerticalNeighborMapping[2] {
        new VerticalNeighborMapping { leftNeighborIndex = 0, rightNeighborIndex = 0 },
        new VerticalNeighborMapping { leftNeighborIndex = 0, rightNeighborIndex = 0 }
    };
    mappingPattern12[2] = new VerticalNeighborMapping[2] {
        new VerticalNeighborMapping { leftNeighborIndex = 0, rightNeighborIndex = 0 },
        new VerticalNeighborMapping { leftNeighborIndex = 1, rightNeighborIndex = 1 }
    };
    mappingPattern12[3] = new VerticalNeighborMapping[3] {
        new VerticalNeighborMapping { leftNeighborIndex = 0, rightNeighborIndex = 0 },
        new VerticalNeighborMapping { leftNeighborIndex = 1, rightNeighborIndex = 1 },
        new VerticalNeighborMapping { leftNeighborIndex = 1, rightNeighborIndex = 1 }
    };
    verticalNeighborMappings[11] = mappingPattern12; // "The Cherries"
    
    
    //Grapes
    VerticalNeighborMapping[][] mappingPattern13 = new VerticalNeighborMapping[4][];
    mappingPattern13[0] = null;
    mappingPattern13[1] = new VerticalNeighborMapping[2] {
        new VerticalNeighborMapping { leftNeighborIndex = 0, rightNeighborIndex = 0 },
        new VerticalNeighborMapping { leftNeighborIndex = 0, rightNeighborIndex = 0 }
    };
    mappingPattern13[2] = new VerticalNeighborMapping[2] {
        new VerticalNeighborMapping { leftNeighborIndex = 0, rightNeighborIndex = 0 },
        new VerticalNeighborMapping { leftNeighborIndex = 1, rightNeighborIndex = 1 }
    };
    mappingPattern13[3] = new VerticalNeighborMapping[3] {
        new VerticalNeighborMapping { leftNeighborIndex = 0, rightNeighborIndex = 0 },
        new VerticalNeighborMapping { leftNeighborIndex = 0, rightNeighborIndex = 0 },
        new VerticalNeighborMapping { leftNeighborIndex = 1, rightNeighborIndex = 1 }
    };
    verticalNeighborMappings[12] = mappingPattern13; // "The Grapes"


            // Build patterns based on difficulty
        switch (selectedDifficulty)
        {
            case DifficultyLevel.VeryHard:
                BuildVeryHardPatterns();
                break;
            case DifficultyLevel.Hard:
                BuildHardPatterns();
                // Assign Hard mappings correctly
                verticalNeighborMappings[8] = mappingPattern9;  // "TheShield"
                verticalNeighborMappings[9] = mappingPattern10; // "TheZorro"
                verticalNeighborMappings[10] = mappingPattern11; // "TheHive"
                verticalNeighborMappings[11] = mappingPattern12; // "TheCherries"
                verticalNeighborMappings[12] = mappingPattern13; // "TheGrapes"
                break;
            case DifficultyLevel.Normal:
                BuildNormalPatterns();
                verticalNeighborMappings[13] = new VerticalNeighborMapping[2][] {
                null,
                new VerticalNeighborMapping[2] {
                    new VerticalNeighborMapping { leftNeighborIndex = 0, rightNeighborIndex = 0 },
                    new VerticalNeighborMapping { leftNeighborIndex = 0, rightNeighborIndex = 0 }
                }
            };
                break;
            case DifficultyLevel.Easy:
                BuildEasyPatterns();
                verticalNeighborMappings[14] = new VerticalNeighborMapping[1][] { null };
                break;
        }
        difficulty = selectedDifficulty; // Keep this for positioning logic

    }


    void Start()
    {
        floorManager = GameObject.Find("FloorManager").GetComponent<FloorManager>();
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        // chosenPatternIndex = 2; // Test "The Hive" (index 2 in Hard mode)
        // bool[][] chosenPattern = allPatterns[chosenPatternIndex];
        int patternIndex = Random.Range(0, allPatterns.Count);
        bool[][] chosenPattern = allPatterns[patternIndex];
        string diffPatternName = "";
        int mappingIndex = patternIndex;

        switch (GameSettings.SelectedDifficulty) // Use GameSettings here
        {
            case DifficultyLevel.VeryHard:
                diffPatternName = veryHardNames[patternIndex];
                mappingIndex = patternIndex; // 0–7
                break;
            case DifficultyLevel.Hard:
                diffPatternName = hardNames[patternIndex];
                mappingIndex = 8 + patternIndex; // 8–12
                break;
            case DifficultyLevel.Normal:
                diffPatternName = normalNames[patternIndex];
                mappingIndex = 13;
                break;
            case DifficultyLevel.Easy:
                diffPatternName = easyNames[patternIndex];
                mappingIndex = 14;
                break;
        }

        Debug.Log("Selected Maze: " + diffPatternName);
        chosenPatternIndex = mappingIndex; // For vertical neighbor mapping
        StartCoroutine(SpawnPattern(chosenPattern, allPatternLegTypes[patternIndex],diffPatternName)); // Use patternIndex here
    }

    #region Pattern Building

    // ---------------- VERY HARD (6 floors) ----------------
    void BuildVeryHardPatterns()
    {
        int floors = 6;
        floorPositions = new List<(float, float)>[floors];
        floorPositions[0] = BuildFloorSlots(new float[] { 0f });
        floorPositions[1] = BuildFloorSlots(new float[] { -1f, +1f });
        floorPositions[2] = BuildFloorSlots(new float[] { -2f, -1f, 0f, +1f, +2f });
        floorPositions[3] = BuildFloorSlots(new float[] { -3f, -2f, -1f, 0f, +1f, +2f, +3f });
        floorPositions[4] = BuildFloorSlots(new float[] { -4f, -3f, -2f, -1f, 0f, +1f, +2f, +3f, +4f });
        floorPositions[5] = BuildFloorSlots(new float[] { -5f, -4f, -3f, -2f, -1f, 0f, +1f, +2f, +3f, +4f, +5f });

        //Pattern1: "Skull" OK
        bool[][] pat1 = new bool[6][];
        pat1[0] = new bool[1] { true };
        pat1[1] = new bool[2] { true, true };
        pat1[2] = new bool[5] { true, false, false, false, true };
        pat1[3] = new bool[7] { true, false, true, false, true, false, true };
        pat1[4] = new bool[9] { true, false, false, false, true, false, false, false, true };
        pat1[5] = new bool[11] { true, false, true, false, true, false, true, false, true, false, true };
        allPatterns.Add(pat1);
        BlockLegType[][] leg1 = new BlockLegType[6][];
        leg1[0] = new BlockLegType[1] { BlockLegType.TwoLeg };
        leg1[1] = new BlockLegType[2] { BlockLegType.OneLegLeft, BlockLegType.OneLegRight };
        leg1[2] = new BlockLegType[5] { BlockLegType.OneLegLeft, BlockLegType.TwoLeg, BlockLegType.TwoLeg, BlockLegType.TwoLeg, BlockLegType.OneLegRight };
        leg1[3] = new BlockLegType[7] { BlockLegType.OneLegLeft, BlockLegType.TwoLeg, BlockLegType.OneLegRight, BlockLegType.TwoLeg, BlockLegType.OneLegLeft, BlockLegType.TwoLeg, BlockLegType.OneLegRight };
        leg1[4] = new BlockLegType[9] { BlockLegType.OneLegLeft, BlockLegType.TwoLeg, BlockLegType.TwoLeg, BlockLegType.TwoLeg, BlockLegType.TwoLeg, BlockLegType.TwoLeg, BlockLegType.TwoLeg, BlockLegType.TwoLeg, BlockLegType.OneLegRight };
        leg1[5] = new BlockLegType[11] { BlockLegType.OneLegLeft, BlockLegType.TwoLeg, BlockLegType.OneLegRight, BlockLegType.TwoLeg, BlockLegType.OneLegLeft, BlockLegType.TwoLeg, BlockLegType.OneLegRight, BlockLegType.TwoLeg, BlockLegType.OneLegLeft, BlockLegType.TwoLeg, BlockLegType.OneLegRight };
        allPatternLegTypes.Add(leg1);

        //Pattern2: "Toilet" (default rule) OK OK
        bool[][] pat2 = new bool[6][];
        pat2[0] = new bool[1] { true };
        pat2[1] = new bool[2] { true, true };
        pat2[2] = new bool[5] { true, false, false, false, true };
        pat2[3] = new bool[7] { true, false, false, false, true, false, true };
        pat2[4] = new bool[9] { true, false, true, false, false, true, false, false, true };
        pat2[5] = new bool[11] { true, false, true, false, true, false, true, false, true, false, true };
        allPatterns.Add(pat2);
        BlockLegType[][] leg2 = new BlockLegType[6][];
        leg2[0] = new BlockLegType[1] { BlockLegType.TwoLeg };
        leg2[1] = new BlockLegType[2] { BlockLegType.OneLegLeft, BlockLegType.OneLegRight };
        leg2[2] = new BlockLegType[5] { BlockLegType.OneLegLeft, BlockLegType.TwoLeg, BlockLegType.TwoLeg, BlockLegType.TwoLeg, BlockLegType.OneLegRight };
        leg2[3] = new BlockLegType[7] { BlockLegType.OneLegLeft, BlockLegType.TwoLeg, BlockLegType.TwoLeg, BlockLegType.TwoLeg, BlockLegType.OneLegLeft, BlockLegType.TwoLeg, BlockLegType.OneLegRight };
        leg2[4] = new BlockLegType[9] { BlockLegType.OneLegLeft, BlockLegType.TwoLeg, BlockLegType.OneLegRight, BlockLegType.TwoLeg, BlockLegType.TwoLeg, BlockLegType.TwoLeg, BlockLegType.TwoLeg, BlockLegType.TwoLeg, BlockLegType.OneLegRight };
        leg2[5] = new BlockLegType[11] { BlockLegType.OneLegLeft, BlockLegType.TwoLeg, BlockLegType.TwoLeg, BlockLegType.TwoLeg, BlockLegType.OneLegRight, BlockLegType.TwoLeg, BlockLegType.TwoLeg, BlockLegType.TwoLeg, BlockLegType.OneLegLeft, BlockLegType.TwoLeg, BlockLegType.OneLegRight };
        allPatternLegTypes.Add(leg2);

        // Pattern3: "Floating Mickey" (default rule) OK ok
        bool[][] pat3 = new bool[6][];
        pat3[0] = new bool[1] { true };
        pat3[1] = new bool[2] { true, true };
        pat3[2] = new bool[5] { true, false, false, false, true };
        pat3[3] = new bool[7] { true, false, true, false, true, false, true };
        pat3[4] = new bool[9] { true, false, false, true, false, true, false, false, true };
        pat3[5] = new bool[11] { true, false, true, false, true, false, false, true, false, false, true };
        allPatterns.Add(pat3);
        BlockLegType[][] leg3 = new BlockLegType[6][];
        leg3[0] = new BlockLegType[1] { BlockLegType.TwoLeg };
        leg3[1] = new BlockLegType[2] { BlockLegType.OneLegLeft, BlockLegType.OneLegRight };
        leg3[2] = new BlockLegType[5] { BlockLegType.OneLegLeft, BlockLegType.TwoLeg, BlockLegType.TwoLeg, BlockLegType.TwoLeg, BlockLegType.OneLegRight };
        leg3[3] = new BlockLegType[7] { BlockLegType.OneLegLeft, BlockLegType.TwoLeg, BlockLegType.OneLegRight, BlockLegType.TwoLeg, BlockLegType.OneLegLeft, BlockLegType.TwoLeg, BlockLegType.OneLegRight };
        leg3[4] = new BlockLegType[9] { BlockLegType.OneLegLeft, BlockLegType.TwoLeg, BlockLegType.TwoLeg, BlockLegType.TwoLeg, BlockLegType.TwoLeg, BlockLegType.TwoLeg, BlockLegType.TwoLeg, BlockLegType.TwoLeg, BlockLegType.OneLegRight };
        leg3[5] = new BlockLegType[11] { BlockLegType.OneLegLeft, BlockLegType.TwoLeg, BlockLegType.OneLegRight, BlockLegType.TwoLeg, BlockLegType.TwoLeg, BlockLegType.TwoLeg, BlockLegType.TwoLeg, BlockLegType.OneLegRight, BlockLegType.TwoLeg, BlockLegType.TwoLeg, BlockLegType.OneLegRight };
        allPatternLegTypes.Add(leg3);

        // Pattern4: "Pong Paddle" (default rule) OK ok
        bool[][] pat4 = new bool[6][];
        pat4[0] = new bool[1] { true };
        pat4[1] = new bool[2] { true, true };
        pat4[2] = new bool[5] { true, false, false, true, false };
        pat4[3] = new bool[7] { true, false, false, true, false, true, false };
        pat4[4] = new bool[9] { true, false, false, true, false, true, false, true, false };
        pat4[5] = new bool[11] { true, false, true, false, true, false, false, true, false, true, false };
        allPatterns.Add(pat4);
        BlockLegType[][] leg4 = new BlockLegType[6][];
        leg4[0] = new BlockLegType[1] { BlockLegType.TwoLeg };
        leg4[1] = new BlockLegType[2] { BlockLegType.OneLegLeft, BlockLegType.OneLegRight };
        leg4[2] = new BlockLegType[5] { BlockLegType.OneLegLeft, BlockLegType.TwoLeg, BlockLegType.TwoLeg, BlockLegType.TwoLeg, BlockLegType.OneLegRight };
        leg4[3] = new BlockLegType[7] { BlockLegType.OneLegLeft, BlockLegType.TwoLeg, BlockLegType.TwoLeg, BlockLegType.OneLegLeft, BlockLegType.TwoLeg, BlockLegType.OneLegRight, BlockLegType.TwoLeg };
        leg4[4] = new BlockLegType[9] { BlockLegType.OneLegLeft, BlockLegType.TwoLeg, BlockLegType.TwoLeg, BlockLegType.OneLegLeft, BlockLegType.TwoLeg, BlockLegType.TwoLeg, BlockLegType.TwoLeg, BlockLegType.OneLegRight, BlockLegType.TwoLeg };
        leg4[5] = new BlockLegType[11] { BlockLegType.OneLegLeft, BlockLegType.TwoLeg, BlockLegType.OneLegRight, BlockLegType.TwoLeg, BlockLegType.TwoLeg, BlockLegType.TwoLeg, BlockLegType.TwoLeg, BlockLegType.TwoLeg, BlockLegType.TwoLeg, BlockLegType.OneLegRight, BlockLegType.TwoLeg };
        allPatternLegTypes.Add(leg4);

        // Pattern5: "Peace and Love" (custom override on floor 4) OK
        bool[][] pat5 = new bool[6][];
        pat5[0] = new bool[1] { true };
        pat5[1] = new bool[2] { true, true };
        pat5[2] = new bool[5] { true, false, false, false, true };
        pat5[3] = new bool[7] { true, false, true, false, true, false, true };
        pat5[4] = new bool[9] { true, false, true, false, true, false, true, false, true };
        pat5[5] = new bool[11] { true, false, false, true, false, false, true, false, true, false, true };
        allPatterns.Add(pat5);
        BlockLegType[][] leg5 = new BlockLegType[6][];
        leg5[0] = new BlockLegType[1] { BlockLegType.TwoLeg };
        leg5[1] = new BlockLegType[2] { BlockLegType.OneLegLeft, BlockLegType.OneLegRight };
        leg5[2] = new BlockLegType[5] { BlockLegType.OneLegLeft, BlockLegType.TwoLeg, BlockLegType.TwoLeg, BlockLegType.TwoLeg, BlockLegType.OneLegRight };
        leg5[3] = new BlockLegType[7] { BlockLegType.OneLegLeft, BlockLegType.TwoLeg, BlockLegType.OneLegRight, BlockLegType.TwoLeg, BlockLegType.OneLegLeft, BlockLegType.TwoLeg, BlockLegType.OneLegRight };
        leg5[4] = new BlockLegType[9] { BlockLegType.OneLegLeft, BlockLegType.TwoLeg, BlockLegType.TwoLeg, BlockLegType.TwoLeg, BlockLegType.TwoLeg, BlockLegType.TwoLeg, BlockLegType.TwoLeg, BlockLegType.TwoLeg, BlockLegType.OneLegRight };
        leg5[5] = new BlockLegType[11] { BlockLegType.OneLegLeft, BlockLegType.TwoLeg, BlockLegType.TwoLeg, BlockLegType.TwoLeg, BlockLegType.TwoLeg, BlockLegType.TwoLeg, BlockLegType.TwoLeg, BlockLegType.TwoLeg, BlockLegType.TwoLeg, BlockLegType.TwoLeg, BlockLegType.OneLegRight };
        allPatternLegTypes.Add(leg5);

        // Pattern6: "The Bow" (default rule) OK ok
        bool[][] pat6 = new bool[6][];
        pat6[0] = new bool[1] { true };
        pat6[1] = new bool[2] { true, true };
        pat6[2] = new bool[5] { false, true, false, false, true };
        pat6[3] = new bool[7] { false, true, false, true, false, false, true };
        pat6[4] = new bool[9] { false, true, false, true, false, false, true, false, true };
        pat6[5] = new bool[11] { false, true, false, true, false, false, true, false, true, false, true };
        allPatterns.Add(pat6);
        BlockLegType[][] leg6 = new BlockLegType[6][];
        leg6[0] = new BlockLegType[1] { BlockLegType.TwoLeg };
        leg6[1] = new BlockLegType[2] { BlockLegType.OneLegLeft, BlockLegType.OneLegRight };
        leg6[2] = new BlockLegType[5] { BlockLegType.OneLegLeft, BlockLegType.TwoLeg, BlockLegType.TwoLeg, BlockLegType.TwoLeg, BlockLegType.OneLegRight };
        leg6[3] = new BlockLegType[7] { BlockLegType.TwoLeg, BlockLegType.OneLegLeft, BlockLegType.TwoLeg, BlockLegType.OneLegRight, BlockLegType.TwoLeg, BlockLegType.TwoLeg, BlockLegType.OneLegRight };
        leg6[4] = new BlockLegType[9] { BlockLegType.TwoLeg, BlockLegType.OneLegLeft, BlockLegType.TwoLeg, BlockLegType.TwoLeg, BlockLegType.TwoLeg, BlockLegType.TwoLeg, BlockLegType.OneLegLeft, BlockLegType.TwoLeg, BlockLegType.OneLegRight };
        leg6[5] = new BlockLegType[11] { BlockLegType.TwoLeg, BlockLegType.OneLegLeft, BlockLegType.TwoLeg, BlockLegType.TwoLeg, BlockLegType.TwoLeg, BlockLegType.TwoLeg, BlockLegType.OneLegLeft, BlockLegType.TwoLeg, BlockLegType.TwoLeg, BlockLegType.TwoLeg, BlockLegType.OneLegRight };
        allPatternLegTypes.Add(leg6);

        // Pattern7: "Cat Paw" (default rule) OK ok
        bool[][] pat7 = new bool[6][];
        pat7[0] = new bool[1] { true };
        pat7[1] = new bool[2] { true, true };
        pat7[2] = new bool[5] { true, false, false, false, true };
        pat7[3] = new bool[7] { true, false, true, false, true, false, true };
        pat7[4] = new bool[9] { true, false, true, false, false, true, false, false, true };
        pat7[5] = new bool[11] { false, true, false, false, true, false, true, false, true, false, true };
        allPatterns.Add(pat7);
        BlockLegType[][] leg7 = new BlockLegType[6][];
        leg7[0] = new BlockLegType[1] { BlockLegType.TwoLeg };
        leg7[1] = new BlockLegType[2] { BlockLegType.OneLegLeft, BlockLegType.OneLegRight };
        leg7[2] = new BlockLegType[5] { BlockLegType.OneLegLeft, BlockLegType.TwoLeg, BlockLegType.TwoLeg, BlockLegType.TwoLeg, BlockLegType.OneLegRight };
        leg7[3] = new BlockLegType[7] { BlockLegType.OneLegLeft, BlockLegType.TwoLeg, BlockLegType.OneLegRight, BlockLegType.TwoLeg, BlockLegType.OneLegLeft, BlockLegType.TwoLeg, BlockLegType.OneLegRight };
        leg7[4] = new BlockLegType[9] { BlockLegType.OneLegLeft, BlockLegType.TwoLeg, BlockLegType.TwoLeg, BlockLegType.TwoLeg, BlockLegType.TwoLeg, BlockLegType.TwoLeg, BlockLegType.TwoLeg, BlockLegType.TwoLeg, BlockLegType.OneLegRight };
        leg7[5] = new BlockLegType[11] { BlockLegType.TwoLeg, BlockLegType.TwoLeg, BlockLegType.TwoLeg, BlockLegType.TwoLeg, BlockLegType.OneLegRight, BlockLegType.OneLegRight, BlockLegType.TwoLeg, BlockLegType.TwoLeg, BlockLegType.OneLegLeft, BlockLegType.TwoLeg, BlockLegType.OneLegRight };
        allPatternLegTypes.Add(leg7);

        // Pattern8: "Olimpic Rings" (default rule) OK ok
        bool[][] pat8 = new bool[6][];
        pat8[0] = new bool[1] { true };
        pat8[1] = new bool[2] { true, true };
        pat8[2] = new bool[5] { true, false, true, false, true };
        pat8[3] = new bool[7] { true, false, false, true, false, false, true };
        pat8[4] = new bool[9] { true, false, false, true, false, true, false, false, true };
        pat8[5] = new bool[11] { true, false, true, false, true, false, false, true, false, true, false };
        allPatterns.Add(pat8);
        BlockLegType[][] leg8 = new BlockLegType[6][];
        leg8[0] = new BlockLegType[1] { BlockLegType.TwoLeg };
        leg8[1] = new BlockLegType[2] { BlockLegType.OneLegLeft, BlockLegType.OneLegRight };
        leg8[2] = new BlockLegType[5] { BlockLegType.OneLegLeft, BlockLegType.TwoLeg, BlockLegType.TwoLeg, BlockLegType.TwoLeg, BlockLegType.OneLegRight };
        leg8[3] = new BlockLegType[7] { BlockLegType.OneLegLeft, BlockLegType.TwoLeg, BlockLegType.TwoLeg, BlockLegType.TwoLeg, BlockLegType.TwoLeg, BlockLegType.TwoLeg, BlockLegType.OneLegRight };
        leg8[4] = new BlockLegType[9] { BlockLegType.OneLegLeft, BlockLegType.TwoLeg, BlockLegType.TwoLeg, BlockLegType.OneLegLeft, BlockLegType.TwoLeg, BlockLegType.OneLegRight, BlockLegType.TwoLeg, BlockLegType.TwoLeg, BlockLegType.OneLegRight };
        leg8[5] = new BlockLegType[11] { BlockLegType.OneLegLeft, BlockLegType.TwoLeg, BlockLegType.OneLegRight, BlockLegType.TwoLeg, BlockLegType.TwoLeg, BlockLegType.TwoLeg, BlockLegType.TwoLeg, BlockLegType.OneLegRight, BlockLegType.TwoLeg, BlockLegType.TwoLeg, BlockLegType.TwoLeg };
        allPatternLegTypes.Add(leg8);
    }

    // ---------------- HARD MODE (4-floor mazes) ----------------
    void BuildHardPatterns()
    {
        int floors = 4;
        floorPositions = new List<(float, float)>[floors];
        floorPositions[0] = BuildFloorSlots(new float[] { 0f });
        floorPositions[1] = BuildFloorSlots(new float[] { -1f, +1f });
        floorPositions[2] = BuildFloorSlots(new float[] { -2f, -1f, 0f, +1f, +2f });
        floorPositions[3] = BuildFloorSlots(new float[] { -3f, -2f, -1f, 0f, +1f, +2f, +3f });

        // Pattern H1: "TheShield" (4 Blocks Top) OK
        bool[][] hard1 = new bool[4][];
        hard1[0] = new bool[1] { true };
        hard1[1] = new bool[2] { true, true };
        hard1[2] = new bool[5] { true, false, false, false, true };
        hard1[3] = new bool[7] { true, false, true, false, true, false, true };
        allPatterns.Add(hard1);
        BlockLegType[][] hardLeg1 = new BlockLegType[4][];
        hardLeg1[0] = new BlockLegType[1] { BlockLegType.TwoLeg };
        hardLeg1[1] = new BlockLegType[2] { BlockLegType.OneLegLeft, BlockLegType.OneLegRight };
        hardLeg1[2] = new BlockLegType[5] { BlockLegType.OneLegLeft, BlockLegType.TwoLeg, BlockLegType.TwoLeg, BlockLegType.TwoLeg, BlockLegType.OneLegRight };
        hardLeg1[3] = new BlockLegType[7] { BlockLegType.OneLegLeft, BlockLegType.TwoLeg, BlockLegType.OneLegRight, BlockLegType.TwoLeg, BlockLegType.OneLegLeft, BlockLegType.TwoLeg, BlockLegType.OneLegRight };
        allPatternLegTypes.Add(hardLeg1);

        //Pattern H2: "TheZorro" (3 Blocks Top) OK
        bool[][] hard2 = new bool[4][];
        hard2[0] = new bool[1] { true };
        hard2[1] = new bool[2] { true, true };
        hard2[2] = new bool[5] { true, false, false, false, true };
        hard2[3] = new bool[7] { true, false, false, false, true, false, true };
        allPatterns.Add(hard2);
        BlockLegType[][] hardLeg2 = new BlockLegType[4][];
        hardLeg2[0] = new BlockLegType[1] { BlockLegType.TwoLeg };
        hardLeg2[1] = new BlockLegType[2] { BlockLegType.OneLegLeft, BlockLegType.OneLegRight };
        hardLeg2[2] = new BlockLegType[5] { BlockLegType.OneLegLeft, BlockLegType.TwoLeg, BlockLegType.TwoLeg, BlockLegType.TwoLeg, BlockLegType.OneLegRight };
        hardLeg2[3] = new BlockLegType[7] { BlockLegType.OneLegLeft, BlockLegType.TwoLeg, BlockLegType.TwoLeg, BlockLegType.TwoLeg, BlockLegType.OneLegLeft, BlockLegType.TwoLeg, BlockLegType.OneLegRight };
        allPatternLegTypes.Add(hardLeg2);

        // Pattern H3: "TheHive" OK
        bool[][] hard3 = new bool[4][];
        hard3[0] = new bool[1] { true };
        hard3[1] = new bool[2] { true, true };
        hard3[2] = new bool[5] { true, false, true, false, true };
        hard3[3] = new bool[7] { true, false, false, true, false, false, true };
        allPatterns.Add(hard3);
        BlockLegType[][] hardLeg3 = new BlockLegType[4][];
        hardLeg3[0] = new BlockLegType[1] { BlockLegType.TwoLeg };
        hardLeg3[1] = new BlockLegType[2] { BlockLegType.OneLegLeft, BlockLegType.OneLegRight };
        hardLeg3[2] = new BlockLegType[5] { BlockLegType.OneLegLeft, BlockLegType.TwoLeg, BlockLegType.TwoLeg, BlockLegType.TwoLeg, BlockLegType.OneLegRight };
        hardLeg3[3] = new BlockLegType[7] { BlockLegType.OneLegLeft, BlockLegType.TwoLeg, BlockLegType.TwoLeg, BlockLegType.TwoLeg, BlockLegType.TwoLeg, BlockLegType.TwoLeg, BlockLegType.OneLegRight };
        allPatternLegTypes.Add(hardLeg3);

        // Pattern H4: "TheCherries" OK
        bool[][] hard4 = new bool[4][];
        hard4[0] = new bool[1] { true };
        hard4[1] = new bool[2] { true, true };
        hard4[2] = new bool[5] { true, false, false, true, false };
        hard4[3] = new bool[7] { true, false, false, true, false, true, false };
        allPatterns.Add(hard4);
        BlockLegType[][] hardLeg4 = new BlockLegType[4][];
        hardLeg4[0] = new BlockLegType[1] { BlockLegType.TwoLeg };
        hardLeg4[1] = new BlockLegType[2] { BlockLegType.OneLegLeft, BlockLegType.OneLegRight };
        hardLeg4[2] = new BlockLegType[5] { BlockLegType.OneLegLeft, BlockLegType.TwoLeg, BlockLegType.TwoLeg, BlockLegType.TwoLeg, BlockLegType.TwoLeg };
        hardLeg4[3] = new BlockLegType[7] { BlockLegType.OneLegLeft, BlockLegType.TwoLeg, BlockLegType.TwoLeg, BlockLegType.OneLegLeft, BlockLegType.TwoLeg, BlockLegType.OneLegRight, BlockLegType.TwoLeg };
        allPatternLegTypes.Add(hardLeg4);

        // Pattern H5: "TheGrapes" OK
        bool[][] hard5 = new bool[4][];
        hard5[0] = new bool[1] { true };
        hard5[1] = new bool[2] { true, true };
        hard5[2] = new bool[5] { false, true, false, false, true };
        hard5[3] = new bool[7] { false, true, false, true, false, false, true };
        allPatterns.Add(hard5);
        BlockLegType[][] hardLeg5 = new BlockLegType[4][];
        hardLeg5[0] = new BlockLegType[1] { BlockLegType.TwoLeg };
        hardLeg5[1] = new BlockLegType[2] { BlockLegType.OneLegLeft, BlockLegType.OneLegRight };
        hardLeg5[2] = new BlockLegType[5] { BlockLegType.OneLegLeft, BlockLegType.TwoLeg, BlockLegType.TwoLeg, BlockLegType.TwoLeg, BlockLegType.OneLegRight };
        hardLeg5[3] = new BlockLegType[7] { BlockLegType.TwoLeg, BlockLegType.OneLegLeft, BlockLegType.TwoLeg, BlockLegType.OneLegRight, BlockLegType.TwoLeg, BlockLegType.TwoLeg, BlockLegType.OneLegRight };
        allPatternLegTypes.Add(hardLeg5);
    }

    // ---------------- NORMAL MODE (2-floor maze) ----------------
    void BuildNormalPatterns()
    {
        int floors = 2;
        floorPositions = new List<(float, float)>[floors];
        floorPositions[0] = BuildFloorSlots(new float[] { 0f });
        floorPositions[1] = BuildFloorSlots(new float[] { -1f, +1f });

        bool[][] norm = new bool[2][];
        norm[0] = new bool[1] { true };
        norm[1] = new bool[2] { true, true };
        allPatterns.Add(norm);
        BlockLegType[][] normLeg = new BlockLegType[2][];
        normLeg[0] = new BlockLegType[1] { BlockLegType.TwoLeg };
        normLeg[1] = new BlockLegType[2] { BlockLegType.OneLegLeft, BlockLegType.OneLegRight };
        allPatternLegTypes.Add(normLeg);
    }

    // ---------------- EASY MODE (1-floor maze) ----------------
    void BuildEasyPatterns()
    {
        int floors = 1;
        floorPositions = new List<(float, float)>[floors];
        floorPositions[0] = BuildFloorSlots(new float[] { 0f });

        bool[][] easy = new bool[1][];
        easy[0] = new bool[1] { true };
        allPatterns.Add(easy);
        BlockLegType[][] easyLeg = new BlockLegType[1][];
        easyLeg[0] = new BlockLegType[1] { BlockLegType.TwoLeg };
        allPatternLegTypes.Add(easyLeg);
    }

    #endregion

    /// <summary>
    /// Spawns the maze blocks according to the chosen pattern.
    /// Also records each block’s name and its “active slot” (the order in which it appears on that floor).
    /// </summary>
    IEnumerator SpawnPattern(bool[][] pattern, BlockLegType[][] legTypes, string mazeName)
    {
        topFloorBlocks.Clear(); // Reset list each time maze is spawned
        int topFloorIndex = pattern.Length - 1; // Top floor is the last index
        
        // For each floor we will maintain a counter of active blocks.
        for (int i = pattern.Length - 1; i >= 0; i--)
        {
            int activeSlotCounter = 0;
            bool[] floorSlots = pattern[i];
            for (int s = 0; s < floorSlots.Length; s++)
            {
                if (!floorSlots[s])
                    continue;

                (float L, float R) = floorPositions[i][s];
                float xCenter = 0.5f * (L + R);
                float y = i * verticalSpacing;
                Vector3 pos = new Vector3(xCenter, y, 0f);

                GameObject block;
                if (i == 0)
                {
                    block = Instantiate(PickTreasurePrefab(), pos, Quaternion.identity, transform);
                }
                else
                {
                    BlockLegType type = legTypes[i][s];
                    GameObject prefab = GetPrefabForLegType(type);
                    block = Instantiate(prefab, pos, Quaternion.identity, transform);
                }

                // Add MazeBlock component and assign metadata.
                MazeBlock mb = block.GetComponent<MazeBlock>();
                if (mb == null)
                    mb = block.AddComponent<MazeBlock>();

                int column = ComputeColumn(xCenter);
                mb.gridCoordinate = new Vector2Int(column, i);

                // Also record the active slot order on this floor.
                mb.patternSlot = activeSlotCounter;
                activeSlotCounter++;

                // Store the block’s name.
                mb.blockName = block.name;

                // Set the color type based on block’s name.
                if (block.name.Contains("Blue"))
                    mb.colorType = BlockColorType.Blue;
                else if (block.name.Contains("Green"))
                    mb.colorType = BlockColorType.Green;
                else
                    mb.colorType = BlockColorType.Tan;

                // Set stairsCount.
                mb.stairsCount = (i == 0) ? 0 : (legTypes[i][s] == BlockLegType.TwoLeg ? 2 : 1);
                // Also store the exact type:
                mb.blockLegType = (i == 0) ? BlockLegType.TwoLeg : legTypes[i][s];

                mb.mazeName = mazeName; // Assign the maze name to the block

                // Store top-floor blocks
                if (i == topFloorIndex)
                    topFloorBlocks.Add(mb);

                yield return new WaitForSeconds(generationDelay);
            }
        }
        // Now assign vertical neighbor references using our hard-coded mapping.
        AssignVerticalNeighbors();
        AssignHorizontalNeighbors();  // <--- new call here
        ScaleAndPositionFinalMaze();
        PlacePlayerOnTopFloor();
    }


    // New method to place player
    private void PlacePlayerOnTopFloor()
    {
        if (topFloorBlocks.Count == 0)
        {
            Debug.LogError("No top floor blocks found!");
            return;
        }

        // Randomly select a top-floor block
        // Randomly select a top-floor block
        int randomIndex = Random.Range(0, topFloorBlocks.Count);
        MazeBlock startBlock = topFloorBlocks[randomIndex];
        currentPlayerBlock = startBlock;

        // Position the player cursor only on this block
        UpdatePlayerCursor(startBlock);

        // Debug log
        Debug.Log($"Player placed at {startBlock.gridCoordinate} ({startBlock.colorType})");

        // Notify floorManager to generate floor contents
        if (floorManager != null)
        {
            floorManager.GenerateFloorContents(startBlock.colorType, startBlock.gridCoordinate, currentPlayerBlock,  "NoCorridorDoorUsed");
            Debug.Log("Calling GenerateFloorContents");
            floorManager.PopulateCurrentNeighbours(currentPlayerBlock); //Initialize the neigbouring blocks information
        }
        Debug.Log("THIS SHOULD RUN ONLY AT THE BEGGINING ONCE");
    }


    public void UpdatePlayerCursor(MazeBlock newBlock)
    {
        if (gameManager.currentFloor > 12) return;  //Stop moving cursor after floor 12

        // Hide cursor from old block
        if (currentPlayerBlock != null)
            currentPlayerBlock.SetPlayerCursorActive(false);

        // Show cursor on new block
        newBlock.SetPlayerCursorActive(true);
        currentPlayerBlock = newBlock;
    }


    public void MovePlayerCursor(MazeBlock newBlock)
    {
        if (currentPlayerBlock != null && currentPlayerBlock.playerCursor != null)
        {
            currentPlayerBlock.playerCursor.SetActive(false);
        }

        if (newBlock != null)
        {
            currentPlayerBlock = newBlock;
            if (currentPlayerBlock.playerCursor != null)
            {
                currentPlayerBlock.playerCursor.SetActive(true);
            }
        }
    }

    // Example: ComputeColumn maps the world x-coordinate to a grid column.
    int ComputeColumn(float center)
    {
        return Mathf.RoundToInt(center / horizontalSpacing) + 6;
    }
    

    private void ScaleAndPositionFinalMaze() {
        if (difficulty == DifficultyLevel.VeryHard) {
            transform.localPosition = new Vector3(5770, 9633, -1);
        }   else if (difficulty == DifficultyLevel.Hard) {
            transform.localPosition = new Vector3(5770, 9684, -1);
        }   else if (difficulty == DifficultyLevel.Normal) {
            transform.localPosition = new Vector3(5770, 9735, -1);
        }   else if (difficulty == DifficultyLevel.Easy) {
            transform.localPosition = new Vector3(5770, 9760, -1);
        }
        
        transform.localScale = new Vector3(30,30,30);
    }

    GameObject PickTreasurePrefab()
    {
        return (Random.Range(0, 2) == 0) ? MazeTan2StairsTreasure : MazeGreen2StairsTreasure;
    }

    /// <summary>
    /// Returns a prefab (with randomized color) based on the predetermined leg type.
    /// Mapping (mirrored naming):
    ///   OneLegLeft returns the LEFT–facing variant,
    ///   OneLegRight returns the RIGHT–facing variant,
    ///   TwoLeg returns the two–leg variant.
    /// </summary>
    GameObject GetPrefabForLegType(BlockLegType type)
    {
        int colorIndex = Random.Range(0, 3);
        switch (type)
        {
            case BlockLegType.TwoLeg:
                switch (colorIndex)
                {
                    case 0: return MazeBlue2Stairs;
                    case 1: return MazeGreen2Floors;
                    default: return MazeTan2Stairs;
                }
            case BlockLegType.OneLegLeft:
                // OneLegLeft returns the LEFT–facing variant.
                switch (colorIndex)
                {
                    case 0: return MazeBlue1StairLeft;
                    case 1: return MazeGreen1FloorLeft;
                    default: return MazeTan1FloorLeft;
                }
            case BlockLegType.OneLegRight:
                // OneLegRight returns the RIGHT–facing variant.
                switch (colorIndex)
                {
                    case 0: return MazeBlue1StairRight;
                    case 1: return MazeGreen1FloorRight;
                    default: return MazeTan1FloorRight;
                }
            default:
                switch (colorIndex)
                {
                    case 0: return MazeBlue2Stairs;
                    case 1: return MazeGreen2Floors;
                    default: return MazeTan2Stairs;
                }
        }
    }

    // Helper method: BuildFloorSlots.
    private List<(float, float)> BuildFloorSlots(float[] baseXCenters)
    {
        var list = new List<(float, float)>();
        float half = horizontalSpacing * 0.5f;
        foreach (float baseX in baseXCenters)
        {
            float center = baseX * horizontalSpacing;
            list.Add((center - half, center + half));
        }
        return list;
    }

    private void AssignVerticalNeighbors()
    {
        // First, see if we have a mapping for the chosen pattern.
        if (!verticalNeighborMappings.ContainsKey(chosenPatternIndex))
        {
            Debug.LogWarning("No vertical neighbor mapping for pattern index " + chosenPatternIndex);
            return;
        }

        // Retrieve the vertical mapping array for this pattern.
        VerticalNeighborMapping[][] mapping = verticalNeighborMappings[chosenPatternIndex];
        Debug.Log($"Using mapping for pattern index {chosenPatternIndex}");

        // Gather all MazeBlock components in the spawned maze.
        MazeBlock[] blocks = GetComponentsInChildren<MazeBlock>();

        // Group the blocks by floor (based on gridCoordinate.y).
        Dictionary<int, List<MazeBlock>> blocksByFloor = new Dictionary<int, List<MazeBlock>>();
        foreach (MazeBlock mb in blocks)
        {
            int floor = mb.gridCoordinate.y;
            if (!blocksByFloor.ContainsKey(floor))
                blocksByFloor[floor] = new List<MazeBlock>();
            blocksByFloor[floor].Add(mb);
        }

        // We only assign vertical neighbors for floors 1..N, 
        // because floor 0 has no lower neighbor.
        for (int floor = 1; floor < mapping.Length; floor++)
        {
            // The “mapping” for this floor says which active–block indices 
            // connect to which active–block indices on floor–1.
            VerticalNeighborMapping[] floorMapping = mapping[floor];

            // If either floor doesn't exist in blocksByFloor, skip.
            if (!blocksByFloor.ContainsKey(floor) || !blocksByFloor.ContainsKey(floor - 1))
                continue;

            // Sort the blocks on the current floor and the lower floor 
            // by their patternSlot so they line up with the mapping array.
            List<MazeBlock> currentFloorBlocks = blocksByFloor[floor];
            List<MazeBlock> lowerFloorBlocks   = blocksByFloor[floor - 1];

            currentFloorBlocks.Sort((a, b) => a.patternSlot.CompareTo(b.patternSlot));
            lowerFloorBlocks.Sort((a, b) => a.patternSlot.CompareTo(b.patternSlot));

            // Now, for each “active block index” on this floor,
            // use the VerticalNeighborMapping to see which block(s) below it should connect to.
            for (int slotIndex = 0; slotIndex < floorMapping.Length; slotIndex++)
            {
                VerticalNeighborMapping map = floorMapping[slotIndex];

                // If the mapping says leftNeighborIndex == -1, that means “no connection”.
                if (map.leftNeighborIndex == -1 && map.rightNeighborIndex == -1)
                    continue; // skip this block

                // Make sure this slotIndex is valid in currentFloorBlocks
                if (slotIndex >= currentFloorBlocks.Count)
                    continue; 

                MazeBlock currentBlock = currentFloorBlocks[slotIndex];
                BlockLegType blockType = currentBlock.blockLegType;

                // Read the two indices from the mapping.
                int leftIdx  = map.leftNeighborIndex;
                int rightIdx = map.rightNeighborIndex;

                // Based on the block's leg type, assign the relevant neighbor(s).
                switch (blockType)
                {
                    case BlockLegType.TwoLeg:
                        // A two–leg block can have both neighborBelowLeft and neighborBelowRight.
                        if (leftIdx >= 0 && leftIdx < lowerFloorBlocks.Count)
                            currentBlock.neighborBelowRight  = lowerFloorBlocks[leftIdx];  // I changed here neighborBelowLeft for neighborBelowRight because i was getting confused
                        if (rightIdx >= 0 && rightIdx < lowerFloorBlocks.Count)
                            currentBlock.neighborBelowLeft = lowerFloorBlocks[rightIdx];    // I changed here neighborBelowRight for neighborBelowLeft because i was getting confused
                        break;

                    case BlockLegType.OneLegLeft:
                        // A one–leg–left block only sets neighborBelowLeft.
                        if (leftIdx >= 0 && leftIdx < lowerFloorBlocks.Count)
                            currentBlock.neighborBelowRight  = lowerFloorBlocks[leftIdx];   // I changed here neighborBelowLeft for neighborBelowRight because i was getting confused
                        // We ignore rightIdx on purpose, 
                        // even if the mapping data has a non–negative rightIdx.
                        break;

                    case BlockLegType.OneLegRight:
                        // A one–leg–right block only sets neighborBelowRight.
                        if (rightIdx >= 0 && rightIdx < lowerFloorBlocks.Count)
                            currentBlock.neighborBelowLeft = lowerFloorBlocks[rightIdx];    // I changed here neighborBelowRight for neighborBelowLeft because i was getting confused
                        // We ignore leftIdx on purpose.
                        break;
                }
                //Debug.Log($"Floor {floor}, Slot {currentBlock.patternSlot} ({blockType}): " +
                    //$"neighborBelowRight = {(currentBlock.neighborBelowRight != null ? currentBlock.neighborBelowRight.gridCoordinate : "null")}, " +
                    //$"neighborBelowLeft = {(currentBlock.neighborBelowLeft != null ? currentBlock.neighborBelowLeft.gridCoordinate : "null")}");

            }
        }
    }

    private void AssignHorizontalNeighbors()
    {
        // Get every MazeBlock in the maze.
        MazeBlock[] allBlocks = GetComponentsInChildren<MazeBlock>();

        // Group blocks by floor (based on gridCoordinate.y).
        Dictionary<int, List<MazeBlock>> blocksByFloor = new Dictionary<int, List<MazeBlock>>();
        foreach (MazeBlock mb in allBlocks)
        {
            int floor = mb.gridCoordinate.y;
            if (!blocksByFloor.ContainsKey(floor))
                blocksByFloor[floor] = new List<MazeBlock>();
            blocksByFloor[floor].Add(mb);
        }

        // For each floor, sort the blocks by gridCoordinate.x.
        foreach (int floor in blocksByFloor.Keys)
        {
            List<MazeBlock> floorBlocks = blocksByFloor[floor];
            floorBlocks.Sort((a, b) => a.gridCoordinate.x.CompareTo(b.gridCoordinate.x));

            // Now link blocks horizontally only if they are immediately adjacent.
            // (For example, if blocks are at columns 1,3,5,7,9,11 then the difference between consecutive blocks is 2.)
            for (int i = 0; i < floorBlocks.Count - 1; i++)
            {
                MazeBlock leftBlock = floorBlocks[i];
                MazeBlock rightBlock = floorBlocks[i + 1];

                // If the difference is exactly 2, then they are immediate neighbors.
                if (rightBlock.gridCoordinate.x - leftBlock.gridCoordinate.x == 2)
                {
                    leftBlock.neighborRight = rightBlock;
                    rightBlock.neighborLeft = leftBlock;
                }
                else
                {
                    // Otherwise, do not assign a horizontal neighbor between these blocks.
                    leftBlock.neighborRight = null;
                    rightBlock.neighborLeft = null;
                }
            }
        }
    }

}
