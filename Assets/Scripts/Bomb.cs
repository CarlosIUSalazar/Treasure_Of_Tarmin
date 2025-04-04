using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class Bomb : MonoBehaviour
{
    [SerializeField] private GameObject smokePrefab;
    public int explosivePower = 20;
    PlayerGridMovement playerGridMovement;
    Player player;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerGridMovement = GameObject.Find("Player").GetComponent<PlayerGridMovement>();
        player = GameObject.Find("Player").GetComponent<Player>();
        playerGridMovement.isWaitingForBombToExplode = true;
        //playerGridMovement.HideDirectionalButtons();
        //playerGridMovement.HideBackwardButton();
        StartCoroutine(BombSequence());
    }


IEnumerator BombSequence() {
    //Freeze player in place for 1.5sec by hiding the arrow buttons inlcuding backstep

    //Play bomb sound
    yield return new WaitForSeconds(2f);
    //Explode and destroy the bomb and spawn the smoke
    MeshRenderer childRenderer = GetComponentInChildren<MeshRenderer>();
    if (childRenderer != null)
    {
        childRenderer.enabled = false;
    }
    Instantiate(smokePrefab, transform.position, Quaternion.identity);
    player.ModifyPhysicalStrength(-explosivePower);
    //playerGridMovement.ShowDirectionalButtons();
    //playerGridMovement.ShowBackwardButton();
    playerGridMovement.isWaitingForBombToExplode = false;
    Destroy(gameObject);
}

    // Update is called once per frame
    void Update()
    {
        
    }
}
