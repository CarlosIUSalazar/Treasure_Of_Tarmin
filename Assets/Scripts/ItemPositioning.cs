using UnityEngine;

public class ItemPositioning : MonoBehaviour
{
    [Header("Grid Settings")]
    public float gridSize        = 10f;
    public float offsetDistance = 4.5f;
    private Vector3 gridCenter;

    [Header("Smoothing")]
    [Tooltip("Radial speed at the grid-edge (when t=0)")]
    public float radialSpeedMin =  2f;
    [Tooltip("Radial speed at the grid-center (when t=1)")]
    public float radialSpeedMax = 20f;
    [Tooltip("If player is this close to center on spawn, snap immediately")]
    public float snapDistanceThreshold = 0.05f;

    private float currentDistance = 0f;

    Transform player;

    void Start()
    {
        gridCenter = transform.position;
        player     = GameObject.FindWithTag("Player")?.transform;
        if (player == null) Debug.LogError("No Player tagged!");
    }

    void Update()
    {
        if (player == null) return;

        //––– 0) are we still in the same square? –––
        int px = Mathf.FloorToInt(player.position.x / gridSize);
        int pz = Mathf.FloorToInt(player.position.z / gridSize);
        int ix = Mathf.FloorToInt(gridCenter.x    / gridSize);
        int iz = Mathf.FloorToInt(gridCenter.z    / gridSize);
        bool sameSquare = (px == ix && pz == iz);

        if (!sameSquare)
        {
            // snap back immediately
            currentDistance   = 0f;
            transform.position = gridCenter;
            return;
        }

        //––– 1) compute flat distance to center (optional now, for your smoothing factor) –––
        Vector2 pp = new Vector2(player.position.x, player.position.z);
        Vector2 gc = new Vector2(gridCenter.x,       gridCenter.z);
        float distToCenter  = Vector2.Distance(pp, gc);

        //––– 2) normalize 0 at edge → 1 at center (you already tweaked this factor) –––
        float halfGrid = gridSize * 2f; //MODIFY THIS VALUE TO CHANGE THE SMOOTHENING EFFECT AS THE PLAYER APPROACHES THE ITEM
        float t        = Mathf.Clamp01((halfGrid - distToCenter) / halfGrid);

        //––– 3) dynamic target distance –––
        float targetDist = t * offsetDistance;

        //––– 4) instant snap on spawn collision –––
        if (Mathf.Approximately(currentDistance, 0f)
            && distToCenter < snapDistanceThreshold)
        {
            currentDistance = targetDist;
        }
        else
        {
            //––– 5) speed ramps up as t→1 –––
            float speed = Mathf.Lerp(radialSpeedMin, radialSpeedMax, t);
            currentDistance = Mathf.MoveTowards(
                currentDistance,
                targetDist,
                speed * Time.deltaTime
            );
        }

    //––– 6) rotate instantly in front of player –––
    Vector3 dir = player.forward;
    dir.y = 0f;
    if (dir.sqrMagnitude < 0.01f) dir = Vector3.forward;
    dir.Normalize();

    transform.position = gridCenter + dir * currentDistance;
    }
}
    // bool IsPlayerInSameGridSquare()
    // {
    //     int px = Mathf.FloorToInt(player.position.x / gridSize);
    //     int pz = Mathf.FloorToInt(player.position.z / gridSize);
    //     int ix = Mathf.FloorToInt(gridCenter.x       / gridSize);
    //     int iz = Mathf.FloorToInt(gridCenter.z       / gridSize);
    //     return px == ix && pz == iz;
    // }

