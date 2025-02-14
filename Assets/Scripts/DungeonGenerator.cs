using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Multiple6BlockPatterns : MonoBehaviour
{
    [Header("Block Variants (3 Colors × 3 Leg Types)")]
    // BLUE
    public GameObject MazeBlue2Stairs;
    public GameObject MazeBlue1StairLeft;
    public GameObject MazeBlue1StairRight;

    // GREEN
    public GameObject MazeGreen2Floors;
    public GameObject MazeGreen1FloorLeft;
    public GameObject MazeGreen1FloorRight;

    // TAN
    public GameObject MazeTan2Stairs;
    public GameObject MazeTan1FloorLeft;
    public GameObject MazeTan1FloorRight;

    [Header("Treasure Prefabs for Floor 1 (bottom)")]
    public GameObject MazeTan2StairsTreasure;
    public GameObject MazeGreen2StairsTreasure;

    [Header("Spacing & Layout")]
    public float verticalSpacing = 0.8f; 
    public float generationDelay = 0.2f;

    // We'll define bounding boxes for each floor in floorPositions[i].
    // i=0 => floor1 => 1 slot
    // i=1 => floor2 => 2 slots
    // i=2 => floor3 => 5 slots
    // i=3 => floor4 => 7 slots
    // i=4 => floor5 => 9 slots
    // i=5 => floor6 => 11 slots
    private List<(float xLeft, float xRight)>[] floorPositions;

    // We'll store multiple patterns in a list
    private List<bool[][]> sixBlockTopPatterns = new List<bool[][]>();

    void Awake()
    {
        // 1) define manual bounding boxes for each floor
        DefineManualFloorPositions();

        // 2) define the patterns
        BuildPatterns();
    }

    void Start()
    {
        // 3) pick a pattern (random or specific) and spawn it
        // Example: pick random
        int index = Random.Range(0, sixBlockTopPatterns.Count);

        // If you want to specifically test pattern #2, do: int index=1;
        // or pattern #1 => index=0, etc.

        bool[][] chosenPattern = sixBlockTopPatterns[index];
        StartCoroutine(SpawnPattern(chosenPattern));
    }

    /// <summary>
    /// Manually define x-centers for each floor:
    /// floor1 => x=0
    /// floor2 => x=-1, +1
    /// floor3 => x=-2..+2 => 5 slots
    /// floor4 => x=-3..+3 => 7 slots
    /// floor5 => x=-4..+4 => 9 slots
    /// floor6 => x=-5..+5 => 11 slots
    /// Each bounding box is 2 wide => [xCenter-1..xCenter+1].
    /// </summary>
    void DefineManualFloorPositions()
    {
        floorPositions = new List<(float,float)>[6];

        floorPositions[0] = BuildFloorSlots(new float[]{ 0f });
        floorPositions[1] = BuildFloorSlots(new float[]{ -1f, +1f });
        floorPositions[2] = BuildFloorSlots(new float[]{ -2f, -1f, 0f, +1f, +2f });
        floorPositions[3] = BuildFloorSlots(new float[]{ -3f, -2f, -1f, 0f, +1f, +2f, +3f });
        floorPositions[4] = BuildFloorSlots(new float[]{ -4f, -3f, -2f, -1f, 0f, +1f, +2f, +3f, +4f });
        floorPositions[5] = BuildFloorSlots(new float[]{ -5f, -4f, -3f, -2f, -1f, 0f, +1f, +2f, +3f, +4f, +5f });
    }

    List<(float,float)> BuildFloorSlots(float[] xCenters)
    {
        var list = new List<(float,float)>();
        foreach (float xc in xCenters)
        {
            float half = 1f; // bounding box half => total width=2
            list.Add((xc - half, xc + half));
        }
        return list;
    }

    /// <summary>
    /// Build and store your multiple 6-block-top patterns in a list. 
    /// Pattern #1 and Pattern #2 from your chat logs.
    /// </summary>
    void BuildPatterns()
    {
        // PATTERN #1 Skull 6 blocks
        bool[][] pattern1 = new bool[6][];

        // floor1 => 1 slot
        pattern1[0] = new bool[1] { true };

        // floor2 => 2 slots
        pattern1[1] = new bool[2] { true, true };

        // floor3 => 5 slots => [true, false, false, false, true]
        pattern1[2] = new bool[5] { true, false, false, false, true };

        // floor4 => 7 slots => [true, false, true, false, true, false, true]
        pattern1[3] = new bool[7] { true, false, true, false, true, false, true };

        // floor5 => 9 slots => [true, false, false, false, true, false, false, false, true]
        pattern1[4] = new bool[9] { true, false, false, false, true, false, false, false, true };

        // floor6 => 11 => [true, false, true, false, true, false, true, false, true, false, true]
        pattern1[5] = new bool[11] { true, false, true, false, true, false, true, false, true, false, true };

        sixBlockTopPatterns.Add(pattern1);

        // PATTERN #2 Toilet 6 blocks
        bool[][] pattern2 = new bool[6][];

        pattern2[0] = new bool[1] { true };
        pattern2[1] = new bool[2] { true, true };

        // floor3 => 5 => [true, false, false, false, true]
        pattern2[2] = new bool[5] { true, false, false, false, true };

        // floor4 => 7 => [true, false, false, false, false, true, true]
        pattern2[3] = new bool[7] { true, false, false, false, true, false, true };

        // floor5 => 9 => [true, false, true, false, false, true, false, false, true]
        pattern2[4] = new bool[9] { true, false, true, false, false, true, false, false, true };

        // floor6 => 11 => [true, false, true, false, true, false, true, false, true, false, true]
        pattern2[5] = new bool[11] { true, false, true, false, true, false, true, false, true, false, true };

        sixBlockTopPatterns.Add(pattern2);
    }

    IEnumerator SpawnPattern(bool[][] pattern)
    {
        // spawn from top (i=5) down to bottom (i=0)
        for (int i=5; i>=0; i--)
        {
            bool[] floorSlots = pattern[i];
            for (int s=0; s<floorSlots.Length; s++)
            {
                if (!floorSlots[s]) continue;

                // bounding box
                (float L, float R) = floorPositions[i][s];
                float xCenter = 0.5f*(L+R);
                float y = i*verticalSpacing;
                Vector3 pos = new Vector3(xCenter, y, 0f);

                if (i==0)
                {
                    // bottom => treasure
                    Instantiate(PickTreasurePrefab(), pos, Quaternion.identity, transform);
                }
                else
                {
                    // normal block => color + leg variant
                    GameObject prefab = PickBlockVariant(i, s, pattern);
                    Instantiate(prefab, pos, Quaternion.identity, transform);
                }

                yield return new WaitForSeconds(generationDelay);
            }
        }
    }

    GameObject PickTreasurePrefab()
    {
        if (Random.Range(0,2)==0)
            return MazeTan2StairsTreasure;
        else
            return MazeGreen2StairsTreasure;
    }

    GameObject PickBlockVariant(int floorIndex, int slotIndex, bool[][] pattern)
    {
        (float L, float R) = floorPositions[floorIndex][slotIndex];
        float mid = 0.5f*(L+R);

        bool leftLeg=false, rightLeg=false;
        if (floorIndex>0)
        {
            bool[] below = pattern[floorIndex-1];
            for (int s=0; s<below.Length; s++)
            {
                if (!below[s]) continue;
                (float bL, float bR) = floorPositions[floorIndex-1][s];
                if (DoOverlap(L, mid, bL, bR)) leftLeg=true;
                if (DoOverlap(mid,R, bL, bR)) rightLeg=true;
            }
        }

        // random color
        int colorIndex = Random.Range(0,3);

        if (leftLeg && rightLeg)
        {
            switch(colorIndex)
            {
                case 0: return MazeBlue2Stairs;
                case 1: return MazeGreen2Floors;
                default: return MazeTan2Stairs;
            }
        }
        else if (leftLeg && !rightLeg)
        {
            switch(colorIndex)
            {
                case 0: return MazeBlue1StairLeft;
                case 1: return MazeGreen1FloorLeft;
                default: return MazeTan1FloorLeft;
            }
        }
        else if (!leftLeg && rightLeg)
        {
            switch(colorIndex)
            {
                case 0: return MazeBlue1StairRight;
                case 1: return MazeGreen1FloorRight;
                default: return MazeTan1FloorRight;
            }
        }
        else
        {
            // no overlap => default 2
            switch(colorIndex)
            {
                case 0: return MazeBlue2Stairs;
                case 1: return MazeGreen2Floors;
                default: return MazeTan2Stairs;
            }
        }
    }

    bool DoOverlap(float L1, float R1, float L2, float R2)
    {
        return (R1> L2) && (L1< R2);
    }
}
