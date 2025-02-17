using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RandomInvertedPyramidGenerator : MonoBehaviour
{
    [Header("Prefabs")]
    // Assign your block prefabs (blue, green, tan, etc.) here
    public GameObject[] blockPrefabs;

    [Header("Spacing & Layout")]
    public float blockWidth = 2f;        // Horizontal distance between adjacent slots
    public float verticalSpacing = 0.8f; // Vertical distance between floors
    public float generationDelay = 0.2f; // Delay between spawning each block

    // Floor 1..6 slot counts (i.e. floor i has i slots).
    private int[] floorSlotCounts = {1, 2, 3, 4, 5, 6};

    // Min/Max blocks per floor (index 0 => floor1, index 5 => floor6)
    private int[] minBlocksPerFloor = {1, 2, 2, 3, 3, 5};
    private int[] maxBlocksPerFloor = {1, 2, 3, 4, 5, 6};

    void Start()
    {
        // Start the coroutine so we can yield after each block
        StartCoroutine(GeneratePyramid());
    }

    /// <summary>
    /// Main routine that builds the inverted pyramid floor by floor.
    /// </summary>
    IEnumerator GeneratePyramid()
    {
        // Keep track of which slots were filled on the previous floor
        // For floor 1, there's no previous floor, so we'll just use null.
        bool[] prevFloorSlots = null;

        // Go from floor = 1 (index=0) up to floor = 6 (index=5)
        for (int floorIndex = 0; floorIndex < 6; floorIndex++)
        {
            // Decide how many blocks this floor should have (within the allowed range)
            int countThisFloor = Random.Range(minBlocksPerFloor[floorIndex],
                                              maxBlocksPerFloor[floorIndex] + 1);

            // Pick slots on this floor that ensure vertical connectivity
            bool[] chosenSlots = PickFloorSlots(
                floorIndex,
                countThisFloor,
                prevFloorSlots
            );

            // Now instantiate the blocks in those chosen slots
            for (int slotIndex = 0; slotIndex < chosenSlots.Length; slotIndex++)
            {
                if (chosenSlots[slotIndex])
                {
                    // Calculate position
                    Vector3 pos = GetBlockPosition(floorIndex, slotIndex);
                    // Randomly pick one of the prefabs
                    GameObject prefab = blockPrefabs[Random.Range(0, blockPrefabs.Length)];
                    // Instantiate
                    Instantiate(prefab, pos, Quaternion.identity, transform);

                    // Wait so we can visualize each block being placed
                    yield return new WaitForSeconds(generationDelay);
                }
            }

            // Update prevFloorSlots for the next iteration
            prevFloorSlots = chosenSlots;
        }
    }

    /// <summary>
    /// Chooses exactly 'countToPlace' slots (true = occupied) on the given floorIndex,
    /// ensuring that each chosen slot is connected to at least one slot from prevFloorSlots.
    /// If floorIndex == 0 (Floor 1), there's no previous floor, so no connectivity check needed.
    /// </summary>
    bool[] PickFloorSlots(int floorIndex, int countToPlace, bool[] prevFloorSlots)
    {
        int totalSlots = floorSlotCounts[floorIndex];
        bool[] result = new bool[totalSlots];

        // If floorIndex == 0 (Floor 1) or floorIndex == 1 (Floor 2), we know the pattern is fixed:
        //   Floor 1 => 1 block in slot 0
        //   Floor 2 => 2 blocks in slots [0,1]
        // But if you'd prefer random placement within that row, remove this special-case logic.
        if (floorIndex == 0)
        {
            // Only 1 slot, must fill it
            result[0] = true;
            return result;
        }
        else if (floorIndex == 1)
        {
            // Exactly 2 slots, fill both
            result[0] = true;
            result[1] = true;
            return result;
        }

        // For floors 3..6, we do random subsets with connectivity checks
        // We'll try up to some maximum attempts to find a valid random set
        int attempts = 0;
        const int MAX_ATTEMPTS = 1000;

        while (attempts < MAX_ATTEMPTS)
        {
            attempts++;

            // Clear the array
            for (int i = 0; i < totalSlots; i++)
                result[i] = false;

            // Randomly pick 'countToPlace' distinct slots
            List<int> allSlots = new List<int>();
            for (int i = 0; i < totalSlots; i++) allSlots.Add(i);
            Shuffle(allSlots); // randomize the order

            // Take the first 'countToPlace' from the shuffled list
            for (int i = 0; i < countToPlace; i++)
            {
                int slot = allSlots[i];
                result[slot] = true;
            }

            // Check connectivity: every chosen slot must connect to at least
            // one slot in prevFloorSlots (slot i or slot i-1).
            bool valid = true;
            for (int i = 0; i < totalSlots; i++)
            {
                if (result[i]) // this floor's block in slot i
                {
                    // Must connect to the floor below
                    if (!IsConnected(i, prevFloorSlots))
                    {
                        valid = false;
                        break;
                    }
                }
            }

            if (valid) return result;
        }

        // If we somehow never found a valid arrangement, just fill everything in a way
        // that definitely connects. (Should be rare/impossible with the constraints.)
        Debug.LogWarning($"Could not find a valid random subset for floor {floorIndex+1}." +
                         " Fallback: forcibly connect all possible slots.");
        for (int i = 0; i < totalSlots; i++)
            result[i] = IsConnected(i, prevFloorSlots);

        return result;
    }

    /// <summary>
    /// Checks if a slot on the current floor is connected to the floor below
    /// by seeing if there's a block in slot i or slot (i-1) of the previous floor.
    /// </summary>
    bool IsConnected(int slotIndex, bool[] belowFloor)
    {
        // If there's no below floor (floorIndex=0), skip check
        if (belowFloor == null) return true;

        bool connectSame   = (slotIndex < belowFloor.Length && belowFloor[slotIndex]);
        bool connectLeft   = (slotIndex - 1 >= 0 && belowFloor[slotIndex - 1]);

        return (connectSame || connectLeft);
    }

    /// <summary>
    /// Shuffles a list in-place (Fisher-Yates).
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

    /// <summary>
    /// Returns a centered world position for a block in floorIndex, slotIndex.
    /// floorIndex=0 => bottom floor (Floor 1).
    /// </summary>
    Vector3 GetBlockPosition(int floorIndex, int slotIndex)
    {
        int totalSlots = floorSlotCounts[floorIndex];

        // Center the row horizontally
        float totalRowWidth = (totalSlots - 1) * blockWidth;
        float x = -totalRowWidth / 2f + (slotIndex * blockWidth);

        // Each floor is stacked up by verticalSpacing
        // floorIndex=0 => y=0, floorIndex=1 => y=verticalSpacing, etc.
        float y = floorIndex * verticalSpacing;

        return new Vector3(x, y, 0f);
    }
}
