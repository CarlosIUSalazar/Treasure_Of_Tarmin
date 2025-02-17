using System.Collections.Generic;
using UnityEngine;
using System.Collections;

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
        StartCoroutine(GenerateMaze());
    }

    IEnumerator GenerateMaze()
    {
        for (int floor = 1; floor <= floors; floor++)
        {
            int blockCount;

            switch (floor)
            {
                case 1:
                    blockCount = 1;
                    break;
                case 2:
                    blockCount = 2;
                    break;
                case 3:
                    blockCount = Random.Range(2, 4); // 2 or 3
                    break;
                case 4:
                    blockCount = Random.Range(3, 5); // 3 or 4
                    break;
                case 5:
                    blockCount = Random.Range(3, 6); // 3, 4, or 5
                    break;
                case 6:
                    blockCount = Random.Range(5, 7); // 5 or 6
                    break;
                default:
                    blockCount = 1;
                    break;
            }

            float startX = -((blockCount - 1) * blockWidth / 2);

            List<Vector3> placedPositions = new List<Vector3>();
            int safetyCounter = 1000; // Prevent infinite loops

            while (placedPositions.Count < blockCount && safetyCounter > 0)
            {
                safetyCounter--;
                float xPos = startX + placedPositions.Count * blockWidth;
                float yPos = (floor - 1) * blockHeight;

                Vector3 position = new Vector3(xPos, yPos, 0);

                // Ensure vertical linking by checking if there's a block below
                if (floor > 1)
                {
                    float parentY = yPos - blockHeight;
                    bool hasParent = false;

                    foreach (var pos in GameObject.FindObjectsByType<Transform>(FindObjectsInactive.Include, FindObjectsSortMode.None))
                    {
                        if (Mathf.Abs(pos.position.x - xPos) <= blockWidth / 2 && Mathf.Approximately(pos.position.y, parentY))
                        {
                            hasParent = true;
                            break;
                        }
                    }

                    // Force connections on edges
                    if (floor > 1 && !hasParent)
                    {
                        float edgeOffset = blockWidth / 2;
                        foreach (var pos in GameObject.FindObjectsByType<Transform>(FindObjectsInactive.Include, FindObjectsSortMode.None))
                        {
                            if (Mathf.Abs(pos.position.x - (xPos - edgeOffset)) <= blockWidth / 2 ||
                                Mathf.Abs(pos.position.x - (xPos + edgeOffset)) <= blockWidth / 2)
                            {
                                hasParent = true;
                                break;
                            }
                        }
                    }

                    if (!hasParent) continue; // Skip if no vertical link possible
                }

                GameObject blockPrefab = blockPrefabs[Random.Range(0, blockPrefabs.Length)];
                Instantiate(blockPrefab, position, Quaternion.identity);
                placedPositions.Add(position);
                yield return new WaitForSeconds(0.2f); // Add delay for visualization
            }

            if (placedPositions.Count < blockCount)
            {
                Debug.LogWarning($"Floor {floor} did not reach the required block count of {blockCount}. Adding missing blocks.");

                int missingBlocks = blockCount - placedPositions.Count;
                for (int i = 0; i < missingBlocks; i++)
                {
                    float xPos = startX + placedPositions.Count * blockWidth;
                    float yPos = (floor - 1) * blockHeight;
                    Vector3 position = new Vector3(xPos, yPos, 0);

                    GameObject blockPrefab = blockPrefabs[Random.Range(0, blockPrefabs.Length)];
                    Instantiate(blockPrefab, position, Quaternion.identity);
                    placedPositions.Add(position);
                    yield return new WaitForSeconds(0.2f);
                }
            }

            if (safetyCounter <= 0)
            {
                Debug.LogWarning("Maze generation stopped early to prevent infinite loop.");
            }
        }
    }
} 
