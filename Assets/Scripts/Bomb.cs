using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class Bomb : MonoBehaviour
{
    [SerializeField] private EnemyMapping enemyMapping;  // assign via inspector
    [SerializeField] private GameObject smokePrefab;
    [HideInInspector] public int explosivePower = 50;  //So other classes can modify but not the editor
    PlayerGridMovement playerGridMovement;
    Player player;
    GameManager gameManager;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerGridMovement = GameObject.Find("Player").GetComponent<PlayerGridMovement>();
        player = GameObject.Find("Player").GetComponent<Player>();
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        playerGridMovement.isWaitingForBombToExplode = true;
        StartCoroutine(BombSequence());
    }


    IEnumerator BombSequence() {
        //Freeze player in place for 1.5sec by hiding the arrow buttons inlcuding backstep
        gameManager.PlayBombSoundEffect();
        yield return new WaitForSeconds(1.1f);
        //Explode and destroy the bomb and spawn the smoke
        MeshRenderer childRenderer = GetComponentInChildren<MeshRenderer>();
        if (childRenderer != null)
        {
            childRenderer.enabled = false;
        }
        Instantiate(smokePrefab, transform.position, Quaternion.identity);
        player.ModifyPhysicalStrength(-explosivePower);
        playerGridMovement.isWaitingForBombToExplode = false;
        Destroy(gameObject);
    }
}
