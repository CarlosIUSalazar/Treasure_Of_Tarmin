using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DungeonGenerator : MonoBehaviour
{
    [Header("Block Variants (3 Colors × 3 Leg Types)")]
    // BLUE
    public GameObject MazeBlue2Stairs;       // 2 legs
    public GameObject MazeBlue1StairLeft;    // left leg only
    public GameObject MazeBlue1StairRight;   // right leg only

    // GREEN
    public GameObject MazeGreen2Floors;      // 2 legs
    public GameObject MazeGreen1FloorLeft;   // left leg only
    public GameObject MazeGreen1FloorRight;  // right leg only

    // TAN
    public GameObject MazeTan2Stairs;        // 2 legs
    public GameObject MazeTan1FloorLeft;     // left leg only
    public GameObject MazeTan1FloorRight;    // right leg only

    [Header("Spacing & Layout")]
    public float blockWidth = 2f;        // Horizontal distance between adjacent slots
    public float verticalSpacing = 0.8f; // Vertical distance between floors
    public float generationDelay = 0.2f; // Delay (seconds) between spawning each block

    // We have 6 floors total
    private int[] floorSlotCounts = {1, 2, 3, 4, 5, 6};

    // Min/Max blocks per floor
    // Index 0 => Floor 1, index 5 => Floor 6
    private int[] minBlocksPerFloor = {1, 2, 2, 3, 3, 5};
    private int[] maxBlocksPerFloor = {1, 2, 3, 4, 5, 6};

    private void Start()
    {
        // Use a coroutine so we can yield between blocks
        StartCoroutine(GeneratePyramid());
    }

    /// <summary>
    /// Main routine: builds each floor from bottom (floorIndex=0) to top (floorIndex=5).
    /// </summary>
    private IEnumerator GeneratePyramid()
    {
        bool[] prevFloorSlots = null; // which slots were filled on the floor below

        for (int floorIndex = 0; floorIndex < 6; floorIndex++)
        {
            int countThisFloor = Random.Range(minBlocksPerFloor[floorIndex],
                                              maxBlocksPerFloor[floorIndex] + 1);

            // Choose which slots on this floor to fill, ensuring connectivity
            bool[] chosenSlots = PickFloorSlots(floorIndex, countThisFloor, prevFloorSlots);

            // Instantiate blocks in the chosen slots
            for (int slotIndex = 0; slotIndex < chosenSlots.Length; slotIndex++)
            {
                if (!chosenSlots[slotIndex]) 
                    continue;

                // Compute the position
                Vector3 pos = GetBlockPosition(floorIndex, slotIndex);

                // Decide which legs are needed (left/right)
                bool leftConnected = (floorIndex > 0 && slotIndex > 0 && prevFloorSlots[slotIndex - 1]);
                bool rightConnected = (floorIndex > 0 && slotIndex < prevFloorSlots.Length && prevFloorSlots[slotIndex]);

                // Pick a random color + correct leg variant
                GameObject prefab = ChooseBlockVariant(leftConnected, rightConnected);

                // Instantiate
                Instantiate(prefab, pos, Quaternion.identity, transform);

                // Delay so we can visualize the generation
                yield return new WaitForSeconds(generationDelay);
            }

            prevFloorSlots = chosenSlots;
        }
    }

    /// <summary>
    /// Given whether this slot is connected on the left/right side, 
    /// and choosing a random color, return the appropriate prefab.
    /// </summary>
    private GameObject ChooseBlockVariant(bool leftLeg, bool rightLeg)
    {
        // Pick color: 0=Blue, 1=Green, 2=Tan
        int colorIndex = Random.Range(0, 3);

        // If no legs at all (floor 1 or fallback), we can either:
        //  - pick "2 legs" anyway
        //  - or define a new "0-leg" prefab if you have one.
        // For now, we'll treat "no legs" as if "2 legs" is used, or you can do something else.

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
            // No legs => fallback to 2 legs (or define a dedicated 0-leg prefab if you have one)
            switch (colorIndex)
            {
                case 0: return MazeBlue2Stairs;
                case 1: return MazeGreen2Floors;
                default: return MazeTan2Stairs;
            }
        }
    }

    /// <summary>
    /// Picks which slots on the given floorIndex are occupied,
    /// ensuring 'countToPlace' blocks and vertical connectivity 
    /// (no isolated blocks).
    /// </summary>
    private bool[] PickFloorSlots(int floorIndex, int countToPlace, bool[] prevFloorSlots)
    {
        int totalSlots = floorSlotCounts[floorIndex];
        bool[] result = new bool[totalSlots];

        // Floors 1 & 2 are fixed: 
        if (floorIndex == 0) // Floor 1 => 1 slot, always filled
        {
            result[0] = true;
            return result;
        }
        if (floorIndex == 1) // Floor 2 => 2 slots, always filled
        {
            result[0] = true;
            result[1] = true;
            return result;
        }

        // For floors 3..6, randomly choose subsets that satisfy connectivity
        int attempts = 0;
        const int MAX_ATTEMPTS = 1000;

        while (attempts < MAX_ATTEMPTS)
        {
            attempts++;

            // Reset
            for (int i = 0; i < totalSlots; i++)
                result[i] = false;

            // Randomly pick 'countToPlace' distinct slots
            List<int> allSlots = new List<int>();
            for (int i = 0; i < totalSlots; i++)
                allSlots.Add(i);
            Shuffle(allSlots);

            // Mark chosen slots
            for (int i = 0; i < countToPlace; i++)
                result[allSlots[i]] = true;

            // Check connectivity
            bool valid = true;
            for (int i = 0; i < totalSlots; i++)
            {
                if (result[i])
                {
                    if (!IsConnected(i, prevFloorSlots))
                    {
                        valid = false;
                        break;
                    }
                }
            }

            if (valid)
                return result;
        }

        // Fallback (rare): force every slot that can connect to be filled
        Debug.LogWarning($"Could not find a valid random subset for floor {floorIndex+1} after {MAX_ATTEMPTS} attempts.");
        for (int i = 0; i < totalSlots; i++)
            result[i] = IsConnected(i, prevFloorSlots);
        return result;
    }

    /// <summary>
    /// True if the block at 'slotIndex' on this floor has at least one block
    /// directly below it or below-left.
    /// </summary>
    private bool IsConnected(int slotIndex, bool[] belowFloor)
    {
        if (belowFloor == null) return true; // Floor 1 has no below

        bool same = (slotIndex < belowFloor.Length && belowFloor[slotIndex]);
        bool left = (slotIndex - 1 >= 0 && belowFloor[slotIndex - 1]);

        return (same || left);
    }

    /// <summary>
    /// Centered position for a block in floorIndex, slotIndex.
    /// floorIndex=0 => bottom floor (Floor 1).
    /// </summary>
    private Vector3 GetBlockPosition(int floorIndex, int slotIndex)
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
    private void Shuffle<T>(List<T> list)
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
