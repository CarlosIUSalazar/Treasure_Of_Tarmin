// using System.Collections.Generic;
// using UnityEngine;

// public class DungeonGenerator : MonoBehaviour
// {
//     public GameObject[] blueBlocks;
//     public GameObject[] greenBlocks;
//     public GameObject[] tanBlocks;
    
//     private List<GameObject[]> blockSets;
//     private List<GameObject> spawnedBlocks = new List<GameObject>();
    
//     public int maxFloors = 12;
//     public float xSpacing = 1.8f;
//     public float ySpacing = 0.4f;
    
//     void Start()
//     {
//         Debug.Log("Generating Dungeon");
//         blockSets = new List<GameObject[]> { blueBlocks, greenBlocks, tanBlocks };
//         Debug.Log("Number of block sets: " + blockSets.Count);
//         GenerateDungeon();
//     }

//     void GenerateDungeon()
//     {
//         int currentBlocks = Random.Range(5, 7); // Top row must have 5 or 6 blocks
//         float startX = -(currentBlocks / 2.0f) * xSpacing;

//         List<Vector3> lastRowPositions = new List<Vector3>();

//         for (int floor = 0; floor < maxFloors; floor += 2)
//         {
//             List<Vector3> currentRowPositions = new List<Vector3>();

//             // Ensure second-to-last row always has 2 blocks
//             if (floor == maxFloors - 4) currentBlocks = 2;
//             // Ensure bottom row always has 1 centered block
//             if (floor == maxFloors - 2) currentBlocks = 1;

//             float rowStartX = -(currentBlocks / 2.0f) * xSpacing;
//             List<float> xPositions = new List<float>();
            
//             for (int i = 0; i < currentBlocks; i++)
//             {
//                 float xOffset = rowStartX + i * xSpacing;
//                 // Introduce controlled random horizontal spacing but ensure connectivity
//                 if (i > 0 && Random.value < 0.3f && i < currentBlocks - 1)
//                 {
//                     xOffset += xSpacing * 0.5f;
//                 }
//                 xPositions.Add(xOffset);
//             }

//             foreach (float xPos in xPositions)
//             {
//                 Vector3 position = new Vector3(xPos, -floor * ySpacing, 0);
//                 Debug.Log("Placing block at " + position);
//                 currentRowPositions.Add(position);
//             }

//             EnsureRowConnectivity(lastRowPositions, currentRowPositions);
            
//             foreach (Vector3 pos in currentRowPositions)
//             {
//                 if (!IsOverlapping(pos, currentRowPositions)) // Prevents overlaps in X or Y
//                 {
//                     SpawnBlock(pos, floor == maxFloors - 2, lastRowPositions);
//                 }
//             }

//             lastRowPositions = new List<Vector3>(currentRowPositions);
//             if (floor < maxFloors - 4) currentBlocks = Mathf.Max(2, currentBlocks - 1);
//         }
//     }

//     void EnsureRowConnectivity(List<Vector3> lastRow, List<Vector3> currentRow)
//     {
//         if (lastRow.Count == 0) return;
        
//         HashSet<float> lastRowXPositions = new HashSet<float>();
//         foreach (Vector3 pos in lastRow)
//         {
//             lastRowXPositions.Add(pos.x);
//         }

//         bool hasConnection = false;
//         foreach (Vector3 pos in currentRow)
//         {
//             if (lastRowXPositions.Contains(pos.x) || lastRowXPositions.Contains(pos.x - xSpacing) || lastRowXPositions.Contains(pos.x + xSpacing))
//             {
//                 hasConnection = true;
//                 break;
//             }
//         }

//         if (!hasConnection)
//         {
//             Vector3 reconnectPos = lastRow[Random.Range(0, lastRow.Count)];
//             Debug.Log("Ensuring connectivity by adding block at " + reconnectPos);
//             currentRow.Add(new Vector3(reconnectPos.x, reconnectPos.y - ySpacing, 0));
//         }
//     }

//     bool IsOverlapping(Vector3 position, List<Vector3> rowPositions)
//     {
//         foreach (Vector3 existingPos in rowPositions)
//         {
//             if (Vector3.Distance(existingPos, position) < xSpacing * 0.9f)
//             {
//                 return true;
//             }
//         }
//         return false;
//     }

//     void SpawnBlock(Vector3 position, bool isBottomFloor, List<Vector3> lastRowPositions)
//     {
//         GameObject[] selectedSet = blockSets[Random.Range(0, blockSets.Count)];
//         GameObject prefab = selectedSet[0];
        
//         if (isBottomFloor)
//         {
//             // Properly center bottom block between the two blocks above it
//             if (lastRowPositions.Count == 2)
//             {
//                 float centerX = (lastRowPositions[0].x + lastRowPositions[1].x) / 2.0f;
//                 position.x = centerX;
//             }
//         }
        
//         Debug.Log("Instantiating block at " + position);
//         GameObject block = Instantiate(prefab, position, Quaternion.identity, transform);
//         spawnedBlocks.Add(block);
//     }
// }




using System.Collections.Generic;
using UnityEngine;

public class DungeonGenerator : MonoBehaviour
{
    public GameObject[] blueBlocks;
    public GameObject[] greenBlocks;
    public GameObject[] tanBlocks;
    
    private List<GameObject[]> blockSets;
    private List<GameObject> spawnedBlocks = new List<GameObject>();
    
    public int maxFloors = 12;
    public float xSpacing = 1.8f;
    public float ySpacing = 0.4f;
    
    void Start()
    {
        blockSets = new List<GameObject[]> { blueBlocks, greenBlocks, tanBlocks };
        GenerateDungeon();
    }

    void GenerateDungeon()
    {
        int currentBlocks = Random.Range(5, 7); // Top row must have 5 or 6 blocks
        float startX = -(currentBlocks / 2.0f) * xSpacing;

        List<int> lastRowIndices = new List<int>(); // Keep track of last row's indices to ensure connectivity

        for (int floor = 0; floor < maxFloors; floor += 2)
        {
            List<int> currentRowIndices = new List<int>();

            // Ensure second-to-last row always has 2 blocks
            if (floor == maxFloors - 4) currentBlocks = 2;
            // Ensure bottom row always has 1 centered block
            if (floor == maxFloors - 2) currentBlocks = 1;

            for (int i = 0; i < currentBlocks; i++)
            {
                Vector3 position = new Vector3(startX + i * xSpacing, -floor * ySpacing, 0);
                SpawnBlock(position, i, currentBlocks, floor == maxFloors - 2);
                currentRowIndices.Add(i);
            }

            // Ensure connectivity: At least one block from last row should connect to the current row
            if (floor > 0 && !EnsureRowConnectivity(lastRowIndices, currentRowIndices))
            {
                int reconnectIndex = lastRowIndices[Random.Range(0, lastRowIndices.Count)];
                Vector3 reconnectPosition = new Vector3(startX + reconnectIndex * xSpacing, -floor * ySpacing, 0);
                SpawnBlock(reconnectPosition, reconnectIndex, currentBlocks, floor == maxFloors - 2);
                currentRowIndices.Add(reconnectIndex);
            }
            
            lastRowIndices = new List<int>(currentRowIndices);
            if (floor < maxFloors - 4) currentBlocks = Mathf.Max(2, currentBlocks - 1); // Reduce block count per row gradually
            startX = -(currentBlocks / 2.0f) * xSpacing;
        }
    }

    bool EnsureRowConnectivity(List<int> lastRow, List<int> currentRow)
    {
        foreach (int idx in lastRow)
        {
            if (currentRow.Contains(idx) || currentRow.Contains(idx - 1) || currentRow.Contains(idx + 1))
                return true;
        }
        return false;
    }

    void SpawnBlock(Vector3 position, int index, int total, bool isBottomFloor)
    {
        GameObject[] selectedSet = blockSets[Random.Range(0, blockSets.Count)];
        GameObject prefab = selectedSet[0]; // Default block
        
        if (isBottomFloor)
        {
            prefab = selectedSet[0]; // Ensure last row always has a block
            GameObject lastBlock = Instantiate(prefab, position + new Vector3(0.9f,0,0), Quaternion.identity, transform);
            spawnedBlocks.Add(lastBlock);
            return;
        }
        else if (index == 0)
        {
            prefab = selectedSet[1]; // Leftmost variation
        }
        else if (index == total - 1)
        {
            prefab = selectedSet[2]; // Rightmost variation
        }
        
        GameObject block = Instantiate(prefab, position, Quaternion.identity, transform);
        spawnedBlocks.Add(block);
    }
}