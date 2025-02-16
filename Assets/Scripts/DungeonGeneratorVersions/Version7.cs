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
        // Step 1: Generate a full pyramid maze
        Dictionary<int, List<Vector3>> mazePositions = new Dictionary<int, List<Vector3>>();

        for (int floor = 1; floor <= floors; floor++)
        {
            int blockCount = floor;
            float startX = -((blockCount - 1) * blockWidth / 2);
            mazePositions[floor] = new List<Vector3>();

            for (int i = 0; i < blockCount; i++)
            {
                float xPos = startX + i * blockWidth;
                float yPos = (floor - 1) * blockHeight;
                Vector3 position = new Vector3(xPos, yPos, 0);

                GameObject blockPrefab = blockPrefabs[Random.Range(0, blockPrefabs.Length)];
                Instantiate(blockPrefab, position, Quaternion.identity);
                mazePositions[floor].Add(position);

                yield return new WaitForSeconds(0.2f);
            }
        }

        // Step 2: Remove blocks while maintaining rules and avoiding isolated blocks
        System.Random random = new System.Random();
        for (int floor = 3; floor <= 6; floor++)
        {
            int minBlocks = floor switch
            {
                3 => 2,
                4 => 3,
                5 => 3,
                6 => 5,
                _ => floor
            };
            int maxBlocks = floor;
            int targetCount = random.Next(minBlocks, maxBlocks + 1);

            int safetyCounter = 10000;
            while (mazePositions[floor].Count > targetCount && safetyCounter-- > 0)
            {
                int indexToRemove = random.Next(mazePositions[floor].Count);
                Vector3 blockToRemove = mazePositions[floor][indexToRemove];

                // Check vertical connectivity
                bool hasConnectionBelow = mazePositions.ContainsKey(floor + 1) &&
                    mazePositions[floor + 1].Exists(pos => Mathf.Abs(pos.x - blockToRemove.x) <= blockWidth / 2);

                bool hasConnectionAbove = mazePositions.ContainsKey(floor - 1) &&
                    mazePositions[floor - 1].Exists(pos => Mathf.Abs(pos.x - blockToRemove.x) <= blockWidth / 2);

                // Check horizontal neighbors
                bool hasLeftNeighbor = mazePositions[floor].Exists(pos => Mathf.Approximately(pos.x, blockToRemove.x - blockWidth));
                bool hasRightNeighbor = mazePositions[floor].Exists(pos => Mathf.Approximately(pos.x, blockToRemove.x + blockWidth));

                // Check if removing would isolate blocks below
                bool blocksBelowDepend = mazePositions.ContainsKey(floor + 1) && mazePositions[floor + 1].Exists(pos =>
                    Mathf.Abs(pos.x - blockToRemove.x) <= blockWidth / 2 &&
                    !mazePositions[floor].Exists(parent => Mathf.Approximately(parent.x, pos.x)));

                // Only remove if not isolated vertically or horizontally and doesn't isolate lower blocks
                if ((floor == 6 || hasConnectionBelow) && hasConnectionAbove && (hasLeftNeighbor || hasRightNeighbor) && !blocksBelowDepend)
                {
                    mazePositions[floor].RemoveAt(indexToRemove);

                    foreach (var obj in GameObject.FindObjectsOfType<Transform>())
                    {
                        if (Vector3.Distance(obj.position, blockToRemove) < 0.01f)
                        {
                            Destroy(obj.gameObject);
                            break;
                        }
                    }
                }
            }

            if (safetyCounter <= 0)
            {
                Debug.LogWarning($"Floor {floor}: Block removal aborted to prevent infinite loop.");
            }
        }
    }
} 