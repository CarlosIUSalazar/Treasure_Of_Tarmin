using System.Collections;
using UnityEngine;

public class PlayerAmbushDetection : MonoBehaviour
{
    [Header("Ambush Settings")]
    public float detectionDistance = 13f;       // How far the rays extend.
    public float detectionTimeRequired = 3f;      // How long an enemy must remain in view.
    
    private float timeEnemyInSight = 0f;
    public bool ambushTriggered = false;
    private float currentDetectionThreshold = 0f;

    private GameManager gameManager;
    private PlayerGridMovement playerGridMovement;
    private PlayerShootingSpawner playerShootingSpawner;
    ViewSwitcher viewSwitcher;

    void Start()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        playerGridMovement = GameObject.Find("Player").GetComponent<PlayerGridMovement>();
        playerShootingSpawner = GameObject.Find("PlayerShootingSpawner").GetComponent<PlayerShootingSpawner>();
        viewSwitcher = GameObject.Find("ViewSwitcher").GetComponent<ViewSwitcher>();
    }
    
    void Update()
    {
        DetectPlayerForAmbush();
    }

    private void DetectPlayerForAmbush() {
                // Skip detection if already fighting or if another ambush is in progress.
        if (gameManager.isFighting || gameManager.ambushInProgress)
        {
            timeEnemyInSight = 0f;
            return;
        }
        
        // We'll cast four rays: forward, back, right, and left.
        Vector3 origin = transform.position + new Vector3(0, 1f, 0); // Adjust the Y offset as needed.
        Vector3[] directions = new Vector3[4];
        //directions[0] = transform.forward;    // Forward (normal detection)
        directions[1] = -transform.forward;   // Back
        directions[2] = transform.right;      // Right
        directions[3] = -transform.right;     // Left

        bool detected = false;
        // Check each ray.
        foreach (Vector3 dir in directions)
        {
            Debug.DrawRay(origin, dir * detectionDistance, Color.blue);
            if (Physics.Raycast(origin, dir, out RaycastHit hit, detectionDistance))
            {
                // Only consider hits on enemies.
                if (hit.collider.CompareTag("Enemy"))
                {
                    // Skip detection if this enemy is an EvilDoor.
                    if (playerGridMovement.IsEvilDoor(hit.collider.gameObject.name)) {
                        return;
                    }
    
                    Debug.Log("Player ambush detection: Enemy detected in direction " + dir);
                    detected = true;
                    break;
                }
            }
        }
        
        if (detected)
        {
                        // If this is the first detection event, set a random threshold.
            if (currentDetectionThreshold == 0f)
                currentDetectionThreshold = Random.Range(2.5f, 4.5f);

            Debug.Log("currentDetectionThreshold = " + currentDetectionThreshold);
            timeEnemyInSight += Time.deltaTime;
            if (timeEnemyInSight >= detectionTimeRequired && !ambushTriggered)
            {
                ambushTriggered = true;
                // We'll now get the first enemy hit (by checking again) and trigger the ambush.
                foreach (Vector3 dir in directions)
                {
                    if (Physics.Raycast(origin, dir, out RaycastHit enemyHit, detectionDistance))
                    {
                        if (enemyHit.collider.CompareTag("Enemy"))
                        {
                            TriggerAmbush(enemyHit.collider.GetComponent<Enemy>());
                            break;
                        }
                    }
                }
            }
        }
        else
        {
            timeEnemyInSight = 0f;
            ambushTriggered = false;
        }
    }
    
    private void TriggerAmbush(Enemy enemy)
    {
        if (enemy == null)
            return;
        
        gameManager.PlayRoarAmbushSoundEffect();
        // Set the global ambush flag so no other ambush triggers.
        gameManager.ambushInProgress = true;
        viewSwitcher.SwitchToGameView();
        // Rotate the player gradually to face the enemy.
        Vector3 dirToEnemy = enemy.transform.position - transform.position;
        dirToEnemy.y = 0; // Keep it horizontal.
        StartCoroutine(RotatePlayerToFaceEnemy(dirToEnemy, 0.5f));
        
        // Set up the fight so that the enemy attacks first.
        gameManager.isFighting = true;
        gameManager.SetActiveEnemy(enemy);
        gameManager.isPlayersTurn = false;
        gameManager.isEnemysTurn = true;
        gameManager.isFreeAttackPhase = false;
        
        // Show the enemy’s HP on the UI.
        gameManager.enemyHPText.gameObject.SetActive(true);
        gameManager.UpdateEnemyHP(enemy.currentEnemyHP);
        
        // Set up the ambush action button.
        playerGridMovement.ShowActionButton();
        playerGridMovement.actionButtonText.text = "Attack";
        playerGridMovement.actionButton.onClick.RemoveAllListeners();
        playerGridMovement.actionButton.onClick.AddListener(() => {
            //playerGridMovement.actionButton.interactable = false; // Disable immediately to block spamming
            gameManager.isPlayersTurn = true;
            gameManager.isFreeAttackPhase = false;
            gameManager.isPassiveFight = false;
            playerGridMovement.HideDirectionalButtons();
            playerShootingSpawner.ShootAtEnemy(enemy.transform);
        });
    }
    
    private IEnumerator RotatePlayerToFaceEnemy(Vector3 targetDirection, float duration)
    {
        Quaternion startRotation = transform.rotation;
        Quaternion endRotation = Quaternion.LookRotation(targetDirection.normalized);
        
        playerGridMovement.HideDirectionalButtons();
        
        float elapsed = 0f;
        while (elapsed < duration)
        {
            transform.rotation = Quaternion.Slerp(startRotation, endRotation, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        transform.rotation = endRotation;
    }
}
