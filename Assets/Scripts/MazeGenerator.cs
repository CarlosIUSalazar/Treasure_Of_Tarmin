using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum BlockLegType { TwoLeg, OneLegLeft, OneLegRight }
public enum DifficultyLevel { VeryHard, Hard, Normal, Easy }

public class MazeGenerator : MonoBehaviour
{
    [Header("Difficulty")]
    public DifficultyLevel difficulty = DifficultyLevel.Hard;

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
    public float generationDelay = 0.2f;
    [Tooltip("Horizontal spacing between slot centers")]
    public float horizontalSpacing = 0.65f;

    // floorPositions will be built based on the number of floors (which depends on difficulty).
    private List<(float xLeft, float xRight)>[] floorPositions;

    // Maze patterns and their predetermined leg type arrays.
    private List<bool[][]> allPatterns = new List<bool[][]>();
    private List<BlockLegType[][]> allPatternLegTypes = new List<BlockLegType[][]>();

    // For VeryHard mode, we have individual names; for other difficulties, we use one name.
    private string[] veryHardNames = { "Skull", "Toilet", "Floating Mickey", "Pong Paddle", "Peace and Love", "The Bow", "Cat Paw", "Olimpic Rings" };
    private string hardName = "Hard Maze";
    private string normalName = "Normal Maze";
    private string easyName = "Easy Maze";

    private int chosenPatternIndex = 0;

    void Awake()
    {
        // Build the appropriate maze patterns based on difficulty.
        switch (difficulty)
        {
            case DifficultyLevel.VeryHard:
                BuildVeryHardPatterns();
                break;
            case DifficultyLevel.Hard:
                BuildHardPatterns();
                break;
            case DifficultyLevel.Normal:
                BuildNormalPatterns();
                break;
            case DifficultyLevel.Easy:
                BuildEasyPatterns();
                break;
        }
    }

    void Start()
    {
        chosenPatternIndex = Random.Range(0, allPatterns.Count);
        bool[][] chosenPattern = allPatterns[chosenPatternIndex];
        string diffPatternName = "";
        if (difficulty == DifficultyLevel.VeryHard)
            diffPatternName = veryHardNames[chosenPatternIndex];
        else if (difficulty == DifficultyLevel.Hard)
            diffPatternName = hardName;
        else if (difficulty == DifficultyLevel.Normal)
            diffPatternName = normalName;
        else if (difficulty == DifficultyLevel.Easy)
            diffPatternName = easyName;

        Debug.Log("Selected Maze: " + diffPatternName);
        StartCoroutine(SpawnPattern(chosenPattern, allPatternLegTypes[chosenPatternIndex]));
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

        // Pattern H2: "TheZorro" (3 Blocks Top) OK
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
    /// Spawns the maze pattern using the predetermined leg type array.
    /// </summary>
    IEnumerator SpawnPattern(bool[][] pattern, BlockLegType[][] legTypes)
    {
        // Spawn floors from top (highest index) down to floor 0.
        for (int i = pattern.Length - 1; i >= 0; i--)
        {
            bool[] floorSlots = pattern[i];
            for (int s = 0; s < floorSlots.Length; s++)
            {
                if (!floorSlots[s])
                    continue;

                (float L, float R) = floorPositions[i][s];
                float xCenter = 0.5f * (L + R);
                float y = i * verticalSpacing;
                Vector3 pos = new Vector3(xCenter, y, 0f);

                if (i == 0)
                {
                    Instantiate(PickTreasurePrefab(), pos, Quaternion.identity, transform);
                }
                else
                {
                    BlockLegType type = legTypes[i][s];
                    GameObject prefab = GetPrefabForLegType(type);
                    Instantiate(prefab, pos, Quaternion.identity, transform);
                }
                yield return new WaitForSeconds(generationDelay);
            }
        }
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

    // (No other unused methods.)
}
