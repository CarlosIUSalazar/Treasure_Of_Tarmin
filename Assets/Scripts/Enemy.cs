//sing Unity.Mathematics;
using System.Collections;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    //[SerializeField] private string enemyName = "WhiteSkeleton";
    [SerializeField] private int enemyBaseHP = 50;
    [SerializeField] private GameObject treasureOfTarminPrefab;
    GameManager gameManager;
    PlayerGridMovement playerGridMovement;
    Player player;
    PlayerShootingSpawner playerShootingSpawner;
    public GameObject smokePrefab; // Assign SmokePrefab in the Inspector
    public int currentEnemyHP;
    private float maxInteractionDistance = 5f;
    public float gridSize = 10.0f; //Size of each grid step

    // Detection / Ambush
    [Header("Ambush Settings")]
    //public float detectionDistance = 10f;     // How far the enemy can see forward
    public float detectionTimeRequired = 2f; // How many seconds the player must stay in sight
    private float timePlayerInSight = 0f;
    private bool hasAmbushed = false;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        playerGridMovement = GameObject.Find("Player").GetComponent<PlayerGridMovement>();
        player = GameObject.Find("Player").GetComponent<Player>();
        playerShootingSpawner = GameObject.Find("PlayerShootingSpawner").GetComponent<PlayerShootingSpawner>();

        currentEnemyHP = Random.Range(0,15) + enemyBaseHP;
        gameManager.UpdateEnemyHP(currentEnemyHP);
    }

    // Update is called once per frame
    void Update()
    {
        //DetectPlayerForAmbush();
    }


// private void DetectPlayerForAmbush()
// {
//     // Skip detection for EvilDoor types.
//     if (playerGridMovement.IsEvilDoor(gameObject.name))
//         return;
    
//     // If already fighting or another ambush is in progress, don't detect.
//     if (gameManager.isFighting || gameManager.ambushInProgress)
//         return;
    
//     float detectionRadius = 11f; // How far the enemy "sees"
//     Vector3 rayOrigin = transform.position + new Vector3(0, 1.2f, 0f);
//     bool detected = false;
    
//     int numRays = 12; // Cast a ray every 30° (360 / 12)
//     int layerMask = ~LayerMask.GetMask("Enemy"); // Exclude enemy colliders.
    
//     for (int i = 0; i < numRays; i++)
//     {
//         float angle = i * (360f / numRays);
//         // Calculate the direction in local space.
//         Vector3 dir = transform.TransformDirection(Quaternion.Euler(0, angle, 0) * Vector3.forward);
//         // Offset the origin slightly so it doesn't hit our own colliders.
//         float selfOffset = 0.5f;
//         Vector3 adjustedOrigin = rayOrigin + dir * selfOffset;
        
//         Debug.DrawRay(adjustedOrigin, dir * detectionRadius, Color.Lerp(Color.blue, Color.cyan, (float)i / numRays));
        
//         if (Physics.Raycast(adjustedOrigin, dir, out RaycastHit hit, detectionRadius, layerMask))
//         {
//             Debug.Log($"Ray {i} (angle {angle}°) hit {hit.collider.name}");
//             if (hit.collider.CompareTag("Player"))
//             {
//                 Debug.Log($"Player detected at local angle {angle}°.");
//                 detected = true;
//                 break; // No need to check further.
//             }
//         }
//     }
    
//     if (detected)
//     {
//         timePlayerInSight += Time.deltaTime;
//         if (timePlayerInSight >= detectionTimeRequired)
//         {
//             Debug.Log("AMBUSHED!!");
//             gameManager.ambushInProgress = true;
//             hasAmbushed = true;
//             AmbushPlayer();
//         }
//     }
//     else
//     {
//         timePlayerInSight = 0f;
//     }
// }





//     private void AmbushPlayer()
//     {
//         Debug.Log($"Ambush! {gameObject.name} attacks first!");
        
//         // Set global ambush flag.
//         gameManager.ambushInProgress = true;
        
//         // Calculate the direction for the player to face.
//         Vector3 dirToEnemy = transform.position - player.transform.position;
//         dirToEnemy.y = 0; // Keep rotation on the same horizontal plane.
        
//         // Gradually rotate the player to face the enemy.
//         StartCoroutine(RotatePlayerToFaceEnemy(dirToEnemy, 0.5f));
        
//         // Set up the fight so the enemy goes first.
//         gameManager.isFighting = true;
//         gameManager.SetActiveEnemy(this);
//         gameManager.isPlayersTurn = false;    // Player does NOT act first.
//         gameManager.isEnemysTurn = true;        // Enemy acts first.
//         gameManager.isFreeAttackPhase = false;
        
//         // Make sure the enemy's HP is shown on the UI.
//         gameManager.enemyHPText.gameObject.SetActive(true);
//         gameManager.UpdateEnemyHP(currentEnemyHP);
        
//         // Set up the action button for the ambush scenario.
//         playerGridMovement.ShowActionButton();
//         playerGridMovement.actionButtonText.text = "Attack";
//         playerGridMovement.actionButton.onClick.RemoveAllListeners();
//         playerGridMovement.actionButton.onClick.AddListener(() => {
//             gameManager.isPlayersTurn = true;
//             gameManager.isFreeAttackPhase = false;
//             gameManager.isPassiveFight = false; // Commit to full fight.
//             playerGridMovement.HideDirectionalButtons();
//             playerShootingSpawner.ShootAtEnemy(transform);
//         });
//     }


//     private IEnumerator RotatePlayerToFaceEnemy(Vector3 targetDirection, float duration)
//     {
//         Quaternion startRotation = player.transform.rotation;
//         Quaternion endRotation = Quaternion.LookRotation(targetDirection.normalized);
//         float elapsed = 0f;
//         while (elapsed < duration)
//         {
//             player.transform.rotation = Quaternion.Slerp(startRotation, endRotation, elapsed / duration);
//             elapsed += Time.deltaTime;
//             yield return null;
//         }
//         player.transform.rotation = endRotation;
//     }


    public void TakeDamage(int damage) {
        currentEnemyHP -= damage;
        Debug.Log("Enemy current HP is " + currentEnemyHP);
        gameManager.UpdateEnemyHP(currentEnemyHP);
        if (currentEnemyHP <= 0) {
            Die();
        }
    }

    public void Die() {
        Debug.Log("Enemy Defeated: " + gameObject.name);
        if (gameObject.name == "Minotaur.vox(Clone)") {
            Instantiate(treasureOfTarminPrefab, transform.position, Quaternion.identity);
        }


        Destroy(gameObject);
        gameManager.isFighting = false;
        playerGridMovement.HideActionButton();
        // Trigger exploration mode after combat ends
        gameManager.isExploring = true;
        // Refresh UI immediately after combat
        gameManager.SetActiveEnemy(null);  // Clear active enemy
        Instantiate(smokePrefab, transform.position, Quaternion.identity);
        gameManager.enemyHPText.gameObject.SetActive(false);
    }
}
