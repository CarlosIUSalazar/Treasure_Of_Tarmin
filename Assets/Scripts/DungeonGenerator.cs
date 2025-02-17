// using UnityEngine;
// using System.Collections;
// using System.Collections.Generic;

// public class DungeonGenerator : MonoBehaviour
// {
//     [Header("Block Variants (3 Colors × 3 Leg Types)")]
//     // BLUE
//     public GameObject MazeBlue2Stairs;
//     public GameObject MazeBlue1StairLeft;
//     public GameObject MazeBlue1StairRight;

//     // GREEN
//     public GameObject MazeGreen2Floors;
//     public GameObject MazeGreen1FloorLeft;
//     public GameObject MazeGreen1FloorRight;

//     // TAN
//     public GameObject MazeTan2Stairs;
//     public GameObject MazeTan1FloorLeft;
//     public GameObject MazeTan1FloorRight;

//     [Header("Treasure Prefabs for Floor 1 (bottom)")]
//     public GameObject MazeTan2StairsTreasure;
//     public GameObject MazeGreen2StairsTreasure;

//     [Header("Spacing & Layout")]
//     public float verticalSpacing = 0.8f;
//     public float generationDelay = 0.2f;

//     // We'll define the bounding boxes for each floor in floorPositions[i].
//     // Floor1 => 1 slot
//     // Floor2 => 2 slots
//     // Floor3 => 5 slots
//     // Floor4 => 7 slots
//     // Floor5 => 9 slots
//     // Floor6 => 11 slots
//     private List<(float xLeft, float xRight)>[] floorPositions;

//     // We'll store all 8 patterns in a single list
//     private List<bool[][]> allPatterns = new List<bool[][]>();

//     void Awake()
//     {
//         // 1) Define bounding boxes for each floor
//         DefineManualFloorPositions();

//         // 2) Build all 8 patterns from your message
//         BuildAllPatterns();
//     }

//     void Start()
//     {
//         // 3) Pick one pattern at random
//         int index = Random.Range(0, allPatterns.Count);
//         bool[][] chosen = allPatterns[index];

//         // 4) Spawn it from top down
//         StartCoroutine(SpawnPattern(chosen));
//     }

//     /// <summary>
//     /// We define bounding boxes for floors 1..6 as:
//     /// floor1 => x=0 => 1 slot
//     /// floor2 => x=-1,+1 => 2 slots
//     /// floor3 => x=-2..+2 => 5 slots
//     /// floor4 => x=-3..+3 => 7 slots
//     /// floor5 => x=-4..+4 => 9 slots
//     /// floor6 => x=-5..+5 => 11 slots
//     /// Each slot is 2 wide => [xCenter-1..xCenter+1].
//     /// </summary>
//     void DefineManualFloorPositions()
//     {
//         floorPositions = new List<(float,float)>[6];
//         floorPositions[0] = BuildFloorSlots(new float[]{ 0f });
//         floorPositions[1] = BuildFloorSlots(new float[]{ -1f, +1f });
//         floorPositions[2] = BuildFloorSlots(new float[]{ -2f, -1f, 0f, +1f, +2f });
//         floorPositions[3] = BuildFloorSlots(new float[]{ -3f, -2f, -1f, 0f, +1f, +2f, +3f });
//         floorPositions[4] = BuildFloorSlots(new float[]{ -4f, -3f, -2f, -1f, 0f, +1f, +2f, +3f, +4f });
//         floorPositions[5] = BuildFloorSlots(new float[]{ -5f, -4f, -3f, -2f, -1f, 0f, +1f, +2f, +3f, +4f, +5f });
//     }

//     List<(float,float)> BuildFloorSlots(float[] xCenters)
//     {
//         var list = new List<(float,float)>();
//         foreach (float xc in xCenters)
//         {
//             float half = 1f; // bounding box half => total width=2
//             list.Add((xc - half, xc + half));
//         }
//         return list;
//     }

//     /// <summary>
//     /// Builds all 8 patterns from your chat message. We store them in 'allPatterns'.
//     /// The user can rename them or reorder them as desired.
//     /// </summary>
//     void BuildAllPatterns()
//     {
//         // Pattern1 => "Skull" (6-block top)
//         bool[][] pattern1 = new bool[6][];
//         pattern1[0] = new bool[1] { true };
//         pattern1[1] = new bool[2] { true, true };
//         pattern1[2] = new bool[5] { true, false, false, false, true };
//         pattern1[3] = new bool[7] { true, false, true, false, true, false, true };
//         pattern1[4] = new bool[9] { true, false, false, false, true, false, false, false, true };
//         pattern1[5] = new bool[11]{ true, false, true, false, true, false, true, false, true, false, true };
//         allPatterns.Add(pattern1);

//         // Pattern2 => "Toilet" (6-block top)
//         bool[][] pattern2 = new bool[6][];
//         pattern2[0] = new bool[1] { true };
//         pattern2[1] = new bool[2] { true, true };
//         pattern2[2] = new bool[5] { true, false, false, false, true };
//         pattern2[3] = new bool[7] { true, false, false, false, true, false, true };
//         pattern2[4] = new bool[9] { true, false, true, false, false, true, false, false, true };
//         pattern2[5] = new bool[11]{ true, false, true, false, true, false, true, false, true, false, true };
//         allPatterns.Add(pattern2);

//         // Pattern3 => "floating mickey" (5-block top)
//         bool[][] p3 = new bool[6][];
//         p3[0] = new bool[1] { true };
//         p3[1] = new bool[2] { true, true };
//         p3[2] = new bool[5] { true, false, false, false, true };
//         p3[3] = new bool[7] { true, false, true, false, true, false, true };
//         p3[4] = new bool[9] { true, false, false, true, false, true, false, false, true };
//         p3[5] = new bool[11]{ true, false, true, false, true, false, false, true, false, false, true };
//         allPatterns.Add(p3);

//         // Pattern4 => "pong paddle" (5-block top)
//         bool[][] p4 = new bool[6][];
//         p4[0] = new bool[1] { true };
//         p4[1] = new bool[2] { true, true };
//         p4[2] = new bool[5] { true, false, false, true, false };
//         p4[3] = new bool[7] { true, false, false, true, false, true, false };
//         p4[4] = new bool[9] { true, false, false, true, false, true, false, true, false };
//         p4[5] = new bool[11]{ true, false, true, false, true, false, false, true, false, true, false };
//         allPatterns.Add(p4);

//         // Pattern5 => "peace and love" (5-block top)
//         bool[][] p5 = new bool[6][];
//         p5[0] = new bool[1] { true };
//         p5[1] = new bool[2] { true, true };
//         p5[2] = new bool[5] { true, false, false, false, true };
//         p5[3] = new bool[7] { true, false, true, false, true, false, true };
//         p5[4] = new bool[9] { true, false, true, false, true, false, true, false, true };
//         p5[5] = new bool[11]{ true, false, false, true, false, false, true, false, true, false, true };
//         allPatterns.Add(p5);

//         // Pattern6 => "the bow" (5-block top)
//         bool[][] p6 = new bool[6][];
//         p6[0] = new bool[1] { true };
//         p6[1] = new bool[2] { true, true };
//         p6[2] = new bool[5] { false, true, false, false, true };
//         p6[3] = new bool[7] { false, true, false, true, false, false, true };
//         p6[4] = new bool[9] { false, true, false, true, false, false, true, false, true };
//         p6[5] = new bool[11]{ false, true, false, true, false, false, true, false, true, false, true };
//         allPatterns.Add(p6);

//         // Pattern7 => "cat paw" (5-block top)
//         bool[][] p7 = new bool[6][];
//         p7[0] = new bool[1] { true };
//         p7[1] = new bool[2] { true, true };
//         p7[2] = new bool[5] { true, false, false, false, true };
//         p7[3] = new bool[7] { true, false, true, false, true, false, true };
//         p7[4] = new bool[9] { true, false, true, false, false, true, false, false, true };
//         p7[5] = new bool[11]{ false, true, false, false, true, false, true, false, true, false, true };
//         allPatterns.Add(p7);

//         // Pattern8 => "olimpic rings" (5-block top)
//         bool[][] p8 = new bool[6][];
//         p8[0] = new bool[1] { true };
//         p8[1] = new bool[2] { true, true };
//         p8[2] = new bool[5] { true, false, true, false, true };
//         p8[3] = new bool[7] { true, false, false, true, false, false, true };
//         p8[4] = new bool[9] { true, false, false, true, false, true, false, false, true };
//         p8[5] = new bool[11]{ true, false, true, false, true, false, false, true, false, true, false };
//         allPatterns.Add(p8);
//     }

//     IEnumerator SpawnPattern(bool[][] pattern)
//     {
//         // spawn from top (i=5) down to bottom (i=0)
//         for (int i=5; i>=0; i--)
//         {
//             bool[] floorSlots = pattern[i];
//             for (int s=0; s<floorSlots.Length; s++)
//             {
//                 if (!floorSlots[s]) continue;

//                 // bounding box
//                 (float L, float R) = floorPositions[i][s];
//                 float xCenter = 0.5f*(L+R);
//                 float y = i * verticalSpacing;
//                 Vector3 pos = new Vector3(xCenter, y, 0f);

//                 if (i==0)
//                 {
//                     // bottom => treasure
//                     Instantiate(PickTreasurePrefab(), pos, Quaternion.identity, transform);
//                 }
//                 else
//                 {
//                     // normal block => color+legs
//                     GameObject prefab = PickBlockVariant(i, s, pattern);
//                     Instantiate(prefab, pos, Quaternion.identity, transform);
//                 }

//                 yield return new WaitForSeconds(generationDelay);
//             }
//         }
//     }

//     GameObject PickTreasurePrefab()
//     {
//         // random Tan or Green
//         if (Random.Range(0,2)==0)
//             return MazeTan2StairsTreasure;
//         else
//             return MazeGreen2StairsTreasure;
//     }

// GameObject PickBlockVariant(int floorIndex, int slotIndex, bool[][] pattern)
// {
//     // Get the bounding box for the current block.
//     (float L, float R) = floorPositions[floorIndex][slotIndex];
//     float mid = (L + R) * 0.5f;
    
//     // Compute a weighted "support" value based on the centers of blocks on the floor below.
//     // Positive support indicates that, on average, supporting blocks lie to the left.
//     // Negative support indicates that they lie to the right.
//     float supportSum = 0f;
//     int supportCount = 0;
    
//     if (floorIndex > 0)
//     {
//         bool[] below = pattern[floorIndex - 1];
//         for (int s = 0; s < below.Length; s++)
//         {
//             if (!below[s])
//                 continue;
            
//             (float bL, float bR) = floorPositions[floorIndex - 1][s];
//             float bCenter = (bL + bR) * 0.5f;
//             // Difference: positive if the support is to the left of our midpoint.
//             float diff = mid - bCenter;
//             supportSum += diff;
//             supportCount++;
//         }
//     }
    
//     // Determine average support.
//     float avgSupport = (supportCount > 0) ? (supportSum / supportCount) : 0f;
//     float tolerance = 0.1f; // tweak this as needed
    
//     bool useLeft = false;
//     bool useRight = false;
    
//     if (supportCount > 0)
//     {
//         if (avgSupport > tolerance)
//             useLeft = true;
//         else if (avgSupport < -tolerance)
//             useRight = true;
//         else
//         {
//             // If nearly balanced, treat it as full (two-leg) support.
//             useLeft = true;
//             useRight = true;
//         }
//     }
    
//     // Randomly pick a color index among 0,1,2.
//     int colorIndex = Random.Range(0, 3);
    
//     // Return the appropriate prefab based on support.
//     if (useLeft && useRight)
//     {
//         switch (colorIndex)
//         {
//             case 0: return MazeBlue2Stairs;
//             case 1: return MazeGreen2Floors;
//             default: return MazeTan2Stairs;
//         }
//     }
//     else if (useLeft && !useRight)
//     {
//         switch (colorIndex)
//         {
//             case 0: return MazeBlue1StairLeft;
//             case 1: return MazeGreen1FloorLeft;
//             default: return MazeTan1FloorLeft;
//         }
//     }
//     else if (!useLeft && useRight)
//     {
//         switch (colorIndex)
//         {
//             case 0: return MazeBlue1StairRight;
//             case 1: return MazeGreen1FloorRight;
//             default: return MazeTan1FloorRight;
//         }
//     }
//     else
//     {
//         // No support found (shouldn't happen if things overlap), so default to two-leg.
//         switch (colorIndex)
//         {
//             case 0: return MazeBlue2Stairs;
//             case 1: return MazeGreen2Floors;
//             default: return MazeTan2Stairs;
//         }
//     }
// }

//     bool DoOverlap(float L1, float R1, float L2, float R2)
//     {
//         return (R1> L2) && (L1< R2);
//     }
// }
