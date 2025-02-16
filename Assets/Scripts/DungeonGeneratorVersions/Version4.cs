using System.Collections.Generic;
using UnityEngine;

public class MazeGenerator : MonoBehaviour
{
    public GameObject[] blockPrefabs; // Assign blue, green, tan prefabs here
    public int floors = 6;
    public float blockWidth = 2f;
    public float blockHeight = 0.8f;

    private void Start()
    {
        if (blockPrefabs == null || blockPrefabs.Length == 0)
        {
            Debug.LogError("No block prefabs assigned! Maze generation aborted.");
            return;
        }
        GenerateMaze();
    }

    void GenerateMaze()
    {
        for (int floor = 1; floor <= floors; floor++)
        {
            int maxBlocks = floor;
            int blockCount;

            if (floor == 6)
            {
                blockCount = Random.Range(5, 7); // Always 5 or 6 blocks on top floor
            }
            else if (floor <= 2)
            {
                blockCount = floor;
            }
            else
            {
                blockCount = Random.Range(1, maxBlocks + 1);
            }

            float startX = -((maxBlocks - 1) * blockWidth / 2);

            List<Vector3> placedPositions = new List<Vector3>();
            int safetyCounter = 1000; // Prevent infinite loops

            while (placedPositions.Count < blockCount && safetyCounter > 0)
            {
                safetyCounter--;
                float xPos = startX + Random.Range(0, maxBlocks) * blockWidth;
                float yPos = (floor - 1) * blockHeight;

                Vector3 position = new Vector3(xPos, yPos, 0);

                // Ensure vertical linking by checking if there's a block below
                if (floor > 1)
                {
                    float parentY = yPos - blockHeight;
                    bool hasParent = false;

                    foreach (var pos in placedPositions)
                    {
                        if (Mathf.Abs(pos.x - xPos) <= blockWidth / 2)
                        {
                            hasParent = true;
                            break;
                        }
                    }

                    if (!hasParent)
                    {
                        // Check for potential parents from the previous floor
                        foreach (var pos in GameObject.FindObjectsByType<Transform>(FindObjectsInactive.Include, FindObjectsSortMode.None))
                        {
                            if (Mathf.Abs(pos.position.x - xPos) <= blockWidth / 2 && Mathf.Approximately(pos.position.y, parentY))
                            {
                                hasParent = true;
                                break;
                            }
                        }
                    }

                    if (!hasParent) continue; // Skip if no vertical link possible
                }

                if (!placedPositions.Exists(p => Mathf.Approximately(p.x, xPos)))
                {
                    GameObject blockPrefab = blockPrefabs[Random.Range(0, blockPrefabs.Length)];
                    Instantiate(blockPrefab, position, Quaternion.identity);
                    placedPositions.Add(position);
                }
            }

            if (safetyCounter <= 0)
            {
                Debug.LogWarning("Maze generation stopped early to prevent infinite loop.");
            }
        }
    }
} 
