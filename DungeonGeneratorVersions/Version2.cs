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
        Debug.Log("Generating Dungeon");
        blockSets = new List<GameObject[]> { blueBlocks, greenBlocks, tanBlocks };
        Debug.Log("Number of block sets: " + blockSets.Count);
        GenerateDungeon();
    }

    void GenerateDungeon()
    {
        int currentBlocks = Random.Range(5, 7); // Top row must have 5 or 6 blocks
        float startX = -(currentBlocks / 2.0f) * xSpacing;

        List<Vector3> lastRowPositions = new List<Vector3>();

        for (int floor = 0; floor < maxFloors; floor += 2)
        {
            List<Vector3> currentRowPositions = new List<Vector3>();

            // Handle second-to-last and bottom rows explicitly
            if (floor == maxFloors - 4) currentBlocks = 2;
            if (floor == maxFloors - 2) currentBlocks = 1;

            float rowStartX = -(currentBlocks / 2.0f) * xSpacing;

            // Adjust spacing for top row
            if (floor == 0)
            {
                if (currentBlocks == 6)
                {
                    rowStartX = -(5 * xSpacing) / 2;
                }
                else if (currentBlocks == 5)
                {
                    rowStartX = -(4.5f * xSpacing) / 2;
                }
            }

            for (int i = 0; i < currentBlocks; i++)
            {
                float xPos = rowStartX + i * xSpacing;

                // Introduce controlled gaps only if it's not the last row
                if (floor < maxFloors - 2 && Random.value < 0.3f && i > 0)
                {
                    xPos += xSpacing * 0.5f; // Add a half-block gap
                }

                Vector3 position = new Vector3(xPos, -floor * ySpacing, 0);

                // Check connectivity and spawn
                if (lastRowPositions.Count == 0 || lastRowPositions.Exists(p => Mathf.Abs(p.x - xPos) <= xSpacing))
                {
                    SpawnBlock(position, floor == maxFloors - 2);
                    currentRowPositions.Add(position);
                }
            }

            lastRowPositions = currentRowPositions;

            // Adjust block count for next row (except for bottom floors)
            if (floor < maxFloors - 4) currentBlocks = Mathf.Max(2, currentBlocks - 1);
        }
    }

    void EnsureRowConnectivity(List<Vector3> lastRow, List<Vector3> currentRow)
    {
        if (lastRow.Count == 0) return;
        
        HashSet<float> lastRowXPositions = new HashSet<float>();
        foreach (Vector3 pos in lastRow)
        {
            lastRowXPositions.Add(pos.x);
        }

        bool hasConnection = false;
        foreach (Vector3 pos in currentRow)
        {
            if (lastRowXPositions.Contains(pos.x) || lastRowXPositions.Contains(pos.x - xSpacing) || lastRowXPositions.Contains(pos.x + xSpacing))
            {
                hasConnection = true;
                break;
            }
        }

        if (!hasConnection)
        {
            Vector3 reconnectPos = lastRow[Random.Range(0, lastRow.Count)];
            Debug.Log("Ensuring connectivity by adding block at " + reconnectPos);
            currentRow.Add(new Vector3(reconnectPos.x, reconnectPos.y - ySpacing, 0));
        }
    }

    bool IsOverlapping(Vector3 position, List<Vector3> rowPositions)
    {
        foreach (Vector3 existingPos in rowPositions)
        {
            if (Vector3.Distance(existingPos, position) < xSpacing * 0.9f)
            {
                return true;
            }
        }
        return false;
    }

    void SpawnBlock(Vector3 position, bool isBottomFloor)
    {
        GameObject[] selectedSet = blockSets[Random.Range(0, blockSets.Count)];
        GameObject prefab = selectedSet[0];

        Debug.Log("Instantiating block at " + position);
        GameObject block = Instantiate(prefab, position, Quaternion.identity, transform);
        spawnedBlocks.Add(block);
    }
}
