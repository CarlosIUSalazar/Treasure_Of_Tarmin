using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MazeGenerator : MonoBehaviour
{
    public GameObject[] blockPrefabs; // Prefabs: Blue, Green, Tan
    public float blockWidth = 2f;
    public float blockHeight = 0.8f;
    private int maxFloors = 6;

    // List to track positions of blocks
    private List<List<Vector3>> floorPositions = new List<List<Vector3>>();

    void Start()
    {
        StartCoroutine(GenerateMazeWithDelay());
    }

    private IEnumerator GenerateMazeWithDelay()
    {
        floorPositions.Clear();

        // Generate floors from top (6th) to bottom (1st)
        for (int floor = maxFloors; floor >= 1; floor--)
        {
            int blockCount = GetBlockCount(floor);
            List<Vector3> currentFloor = new List<Vector3>();

            // Calculate Y position so floor 6 is at the top and floor 1 at the bottom
            float y = floor * blockHeight;
            float xStart = -(blockCount - 1) * blockWidth / 2;

            for (int i = 0; i < blockCount; i++)
            {
                float x = xStart + i * blockWidth;

                // Allow half positions for floors 3, 4, 5, and 6
                if (floor >= 3 && Random.value > 0.5f && i < blockCount - 1)
                {
                    x += blockWidth / 2;
                }

                Vector3 position = new Vector3(x, y, 0);

                // Prevent overlapping
                if (IsOverlapping(floorPositions, position)) continue;

                currentFloor.Add(position);
                GameObject prefab = blockPrefabs[Random.Range(0, blockPrefabs.Length)];
                Instantiate(prefab, position, Quaternion.identity);
                yield return new WaitForSeconds(0.2f);
            }

            floorPositions.Add(currentFloor);
            yield return null;

            // Ensure vertical connectivity
            if (floor < maxFloors)
            {
                EnsureVerticalConnectivity(floorPositions, floor);
            }
        }

        Debug.Log("Maze generation complete.");
    }

    private int GetBlockCount(int floor)
    {
        switch (floor)
        {
            case 1: return 1;
            case 2: return 2;
            case 3: return Random.Range(2, 4);
            case 4: return Random.Range(3, 5);
            case 5: return Random.Range(3, 6);
            case 6: return Random.Range(5, 7);
            default: return 1;
        }
    }

    private bool IsOverlapping(List<List<Vector3>> floorPositions, Vector3 position)
    {
        foreach (var floor in floorPositions)
        {
            foreach (var pos in floor)
            {
                if (Vector3.Distance(pos, position) < blockWidth * 0.6f) // Slight buffer for clarity
                {
                    return true;
                }
            }
        }
        return false;
    }

private void EnsureVerticalConnectivity(List<List<Vector3>> floorPositions, int currentFloor)
{
    if (currentFloor >= maxFloors || floorPositions.Count < 2) return;

    // Adjust indexing to match bottom-up order
    var aboveFloor = floorPositions[maxFloors - currentFloor];
    var belowFloor = floorPositions[maxFloors - currentFloor - 1];

    for (int i = 0; i < aboveFloor.Count; i++)
    {
        Vector3 blockAbove = aboveFloor[i];
        bool hasConnection = false;

        foreach (var blockBelow in belowFloor)
        {
            if (Mathf.Abs(blockAbove.x - blockBelow.x) <= blockWidth)
            {
                hasConnection = true;
                break;
            }
        }

        // If no connection found, move the block to align with the nearest block below
        if (!hasConnection && belowFloor.Count > 0)
        {
            Vector3 closestBlock = GetClosestBlock(blockAbove, belowFloor);
            aboveFloor[i] = new Vector3(closestBlock.x, blockAbove.y, blockAbove.z);
        }
    }
}


    private Vector3 GetClosestBlock(Vector3 block, List<Vector3> floorBelow)
    {
        Vector3 closest = floorBelow[0];
        float minDistance = float.MaxValue;

        foreach (var below in floorBelow)
        {
            float distance = Mathf.Abs(block.x - below.x);
            if (distance < minDistance)
            {
                minDistance = distance;
                closest = below;
            }
        }

        return closest;
    }
}
