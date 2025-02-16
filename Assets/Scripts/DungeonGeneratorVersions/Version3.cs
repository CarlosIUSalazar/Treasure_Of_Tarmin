using System.Collections.Generic;
using UnityEngine;

public class MazeGenerator : MonoBehaviour
{
    public GameObject[] blockPrefabs; // Assign blue, green, tan prefabs here
    public int floors = 6;
    public float blockWidth = 2f;
    public float blockHeight = 0.5f;

    private void Start()
    {
        GenerateMaze();
    }

    void GenerateMaze()
    {
        for (int floor = 1; floor <= floors; floor++)
        {
            int maxBlocks = floor;
            int blockCount = (floor <= 2) ? floor : Random.Range(1, maxBlocks + 1);

            float startX = -((maxBlocks - 1) * blockWidth / 2);

            List<Vector3> placedPositions = new List<Vector3>();

            for (int i = 0; i < blockCount; i++)
            {
                float xPos = startX + i * blockWidth;
                float yPos = (floor - 1) * blockHeight;

                // Ensure vertical linking by snapping to the closest block below
                if (floor > 1)
                {
                    float parentY = yPos - blockHeight;
                    float closestX = xPos;
                    placedPositions.Add(new Vector3(closestX, yPos, 0));
                }

                Vector3 position = new Vector3(xPos, yPos, 0);
                GameObject blockPrefab = blockPrefabs[Random.Range(0, blockPrefabs.Length)];
                Instantiate(blockPrefab, position, Quaternion.identity);
            }
        }
    }
} 
