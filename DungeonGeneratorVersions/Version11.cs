using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DungeonGenerator : MonoBehaviour
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

    [Header("Treasure Prefabs for Floor 1")]
    public GameObject MazeTan2StairsTreasure;   
    public GameObject MazeGreen2StairsTreasure;

    [Header("Spacing & Layout")]
    public float blockWidth = 2f;        
    public float verticalSpacing = 0.8f; 
    public float generationDelay = 0.2f; 

    // Floor slot counts: floorIndex=0 => 1 slot, floorIndex=5 => 6 slots
    private int[] floorSlotCounts = {1, 2, 3, 4, 5, 6};

    // Min/Max blocks per floor
    // index 0 => Floor1, index 5 => Floor6
    private int[] minBlocksPerFloor = {1, 2, 2, 3, 3, 5};
    private int[] maxBlocksPerFloor = {1, 2, 3, 4, 5, 6};

    // We want total blocks in [17..19]
    private const int MIN_TOTAL = 17;
    private const int MAX_TOTAL = 19;

    // How many times to try a random layout before giving up
    private const int MAX_GLOBAL_ATTEMPTS = 200;

    void Start()
    {
        // Start a coroutine that tries to find a valid maze with 17..19 blocks
        StartCoroutine(GenerateMazeUntil17To19());
    }

    /// <summary>
    /// Tries multiple times to generate a valid layout with 17..19 blocks.
    /// Once found, spawns it in the scene with the usual delay.
    /// </summary>
    IEnumerator GenerateMazeUntil17To19()
    {
        int attempts = 0;
        bool success = false;
        bool[][] finalLayout = null;

        while (!success && attempts < MAX_GLOBAL_ATTEMPTS)
        {
            attempts++;

            // Generate a random layout (floors 1..6) in memory
            bool[][] layout = GenerateRandomLayout();

            // Count total blocks
            int totalBlocks = CountBlocks(layout);

            if (totalBlocks >= MIN_TOTAL && totalBlocks <= MAX_TOTAL)
            {
                success = true;
                finalLayout = layout;
            }
        }

        if (!success)
        {
            Debug.LogWarning($"Could not generate a layout with {MIN_TOTAL}..{MAX_TOTAL} blocks " +
                             $"after {MAX_GLOBAL_ATTEMPTS} attempts. Using fallback or doing nothing.");
            yield break;
        }

        // If we got here, finalLayout has 17..19 blocks. Now spawn it in real time.
        yield return SpawnLayout(finalLayout);
    }

    /// <summary>
    /// Generates a single random layout (floors 1..6) in memory, obeying:
    /// - min/max per floor
    /// - connectivity
    /// - treasure on floor1
    /// Returns a 2D array [floorIndex][slotIndex], where true=occupied.
    /// </summary>
    bool[][] GenerateRandomLayout()
    {
        // We'll store 6 floors, each floorIndex => bool[]
        bool[][] floors = new bool[6][];

        bool[] prevFloorSlots = null; // floor below
        for (int floorIndex = 0; floorIndex < 6; floorIndex++)
        {
            int countThisFloor = Random.Range(minBlocksPerFloor[floorIndex],
                                              maxBlocksPerFloor[floorIndex] + 1);

            bool[] chosenSlots = PickFloorSlots(floorIndex, countThisFloor, prevFloorSlots);
            floors[floorIndex] = chosenSlots;

            prevFloorSlots = chosenSlots;
        }

        return floors;
    }

    /// <summary>
    /// Instantiates the final layout in the scene with the 0.2s delay per block.
    /// </summary>
    IEnumerator SpawnLayout(bool[][] layout)
    {
        for (int floorIndex = 0; floorIndex < 6; floorIndex++)
        {
            bool[] floorSlots = layout[floorIndex];
            for (int slotIndex = 0; slotIndex < floorSlots.Length; slotIndex++)
            {
                if (!floorSlots[slotIndex])
                    continue;

                Vector3 pos = GetBlockPosition(floorIndex, slotIndex);

                if (floorIndex == 0)
                {
                    // Floor 1 => treasure block
                    Instantiate(PickTreasurePrefab(), pos, Quaternion.identity, transform);
                }
                else
                {
                    // Floors 2..6 => pick left/right connectivity
                    bool[] prevFloorSlots = (floorIndex > 0) ? layout[floorIndex - 1] : null;
                    bool leftConnected = (prevFloorSlots != null &&
                                          slotIndex > 0 &&
                                          prevFloorSlots[slotIndex - 1]);
                    bool rightConnected = (prevFloorSlots != null &&
                                           slotIndex < prevFloorSlots.Length &&
                                           prevFloorSlots[slotIndex]);

                    GameObject blockPrefab = ChooseBlockVariant(leftConnected, rightConnected);
                    Instantiate(blockPrefab, pos, Quaternion.identity, transform);
                }

                yield return new WaitForSeconds(generationDelay);
            }
        }
    }

    /// <summary>
    /// Returns the total number of occupied slots across all 6 floors.
    /// </summary>
    int CountBlocks(bool[][] layout)
    {
        int total = 0;
        for (int f = 0; f < layout.Length; f++)
        {
            bool[] floorSlots = layout[f];
            for (int s = 0; s < floorSlots.Length; s++)
            {
                if (floorSlots[s]) total++;
            }
        }
        return total;
    }

    /// <summary>
    /// Picks the occupied slots for a given floorIndex, ensuring 'countToPlace'
    /// blocks and vertical connectivity to the previous floor.
    /// </summary>
    bool[] PickFloorSlots(int floorIndex, int countToPlace, bool[] prevFloorSlots)
    {
        int totalSlots = floorSlotCounts[floorIndex];
        bool[] result = new bool[totalSlots];

        // Floor 1 => exactly 1 slot, always in slot 0
        if (floorIndex == 0)
        {
            result[0] = true;
            return result;
        }
        // Floor 2 => 2 slots, always [0,1]
        if (floorIndex == 1)
        {
            result[0] = true;
            result[1] = true;
            return result;
        }

        // For floors 3..6, randomly pick subsets
        int attempts = 0;
        const int MAX_ATTEMPTS = 1000;
        while (attempts < MAX_ATTEMPTS)
        {
            attempts++;
            for (int i = 0; i < totalSlots; i++)
                result[i] = false;

            // Randomly choose 'countToPlace' distinct slots
            List<int> slots = new List<int>();
            for (int i = 0; i < totalSlots; i++)
                slots.Add(i);
            Shuffle(slots);

            for (int i = 0; i < countToPlace; i++)
                result[slots[i]] = true;

            // Check connectivity
            bool valid = true;
            for (int i = 0; i < totalSlots; i++)
            {
                if (result[i] && !IsConnected(i, prevFloorSlots))
                {
                    valid = false;
                    break;
                }
            }

            if (valid)
                return result;
        }

        // Fallback if we never found a valid subset
        Debug.LogWarning($"Could not find a valid subset for floor {floorIndex+1} after {MAX_ATTEMPTS} tries. Fallback: connect any possible slot.");
        for (int i = 0; i < totalSlots; i++)
            result[i] = IsConnected(i, prevFloorSlots);

        return result;
    }

    /// <summary>
    /// True if slotIndex on the current floor can connect to at least one
    /// occupied slot below (same index or index-1).
    /// </summary>
    bool IsConnected(int slotIndex, bool[] belowFloor)
    {
        if (belowFloor == null) return true; // no floor below => floor1
        bool same = (slotIndex < belowFloor.Length && belowFloor[slotIndex]);
        bool left = (slotIndex - 1 >= 0 && belowFloor[slotIndex - 1]);
        return (same || left);
    }

    /// <summary>
    /// Randomly pick either the Tan or Green treasure block for Floor 1.
    /// </summary>
    GameObject PickTreasurePrefab()
    {
        if (Random.Range(0, 2) == 0)
            return MazeTan2StairsTreasure;
        else
            return MazeGreen2StairsTreasure;
    }

    /// <summary>
    /// Choose among the 9 normal variants (3 colors × [left-only, right-only, both]) 
    /// based on connectivity. 
    /// </summary>
    GameObject ChooseBlockVariant(bool leftLeg, bool rightLeg)
    {
        // Pick color: 0=Blue, 1=Green, 2=Tan
        int colorIndex = Random.Range(0, 3);

        if (leftLeg && rightLeg)
        {
            // 2 legs
            switch (colorIndex)
            {
                case 0: return MazeBlue2Stairs;
                case 1: return MazeGreen2Floors;
                default: return MazeTan2Stairs;
            }
        }
        else if (leftLeg && !rightLeg)
        {
            // Left leg only
            switch (colorIndex)
            {
                case 0: return MazeBlue1StairLeft;
                case 1: return MazeGreen1FloorLeft;
                default: return MazeTan1FloorLeft;
            }
        }
        else if (!leftLeg && rightLeg)
        {
            // Right leg only
            switch (colorIndex)
            {
                case 0: return MazeBlue1StairRight;
                case 1: return MazeGreen1FloorRight;
                default: return MazeTan1FloorRight;
            }
        }
        else
        {
            // No legs => default to 2-legs 
            // (or define a "no-leg" prefab if you have one)
            switch (colorIndex)
            {
                case 0: return MazeBlue2Stairs;
                case 1: return MazeGreen2Floors;
                default: return MazeTan2Stairs;
            }
        }
    }

    /// <summary>
    /// Computes a centered position for the given floorIndex and slotIndex.
    /// floorIndex=0 => bottom floor.
    /// </summary>
    Vector3 GetBlockPosition(int floorIndex, int slotIndex)
    {
        int totalSlots = floorSlotCounts[floorIndex];
        float totalRowWidth = (totalSlots - 1) * blockWidth;
        float x = -totalRowWidth / 2f + (slotIndex * blockWidth);
        float y = floorIndex * verticalSpacing;
        return new Vector3(x, y, 0f);
    }

    /// <summary>
    /// Fisher-Yates shuffle of a list in-place.
    /// </summary>
    void Shuffle<T>(List<T> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            int rand = Random.Range(i, list.Count);
            T temp = list[i];
            list[i] = list[rand];
            list[rand] = temp;
        }
    }
}
