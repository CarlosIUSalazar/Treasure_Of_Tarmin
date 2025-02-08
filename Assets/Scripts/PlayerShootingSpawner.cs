using System;
using UnityEngine;

public class PlayerShootingSpawner : MonoBehaviour
{
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private Transform spawnPoint; // Assign this to the position where the arrow should appear
    private float projectileOffset = 1.5f; // Offset in front of the player
    GameManager gameManager;
    PlayerGridMovement playerGridMovement;
    Player player;
    InventoryManager inventoryManager;
    
    void Start()
    {
        player = GameObject.Find("Player").GetComponent<Player>();
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        playerGridMovement = GameObject.Find("Player").GetComponent<PlayerGridMovement>();
        inventoryManager = GameObject.Find("GameManager").GetComponent<InventoryManager>();
    }

    private GameObject FigureOutCurrentAmmo() {
        if (inventoryManager.rightHandSlot.texture.name != "Transparent") {
            String curentWeaponName = inventoryManager.rightHandSlot.texture.name;
            ItemMapping currentAmmoPrefab = inventoryManager.GetItemMapping(curentWeaponName);
            return currentAmmoPrefab.ammo;
        } else {
            return null;
        }

        
    }

    public void ShootAtEnemy(Transform enemy)
    {
        if (gameManager.isPlayersTurn)
        {
            GameObject ammo = FigureOutCurrentAmmo();

            if (ammo == null) {
                Debug.Log("NO WEAPON SELECTED!");
                return;
            }

            gameManager.isPlayersTurn = false;

            Debug.Log("Current Ammo is: " + ammo);

               // Deduct one arrow from the player's inventory if user holds Bow or Crossbow only, if not turns is forfeit
                if (inventoryManager.rightHandSlot.texture.name.Contains("Bow") || 
                    inventoryManager.rightHandSlot.texture.name.Contains("Crossbow")) {
                        if  (player.arrows <= 0) {// Only shoot if the player has arrows
                            Debug.Log("No Arrows Left!");
                            playerGridMovement.HideActionButton();
                            gameManager.isEnemysTurn = true; // Switch to the enemy's turn
                            return;
                        } else {
                            player.ModifyArrows(-1);
                        }
                }
    
                // Instantiate the dart (projectile) with an additional rotation of 220 degrees in Y
                GameObject projectile = Instantiate(
                    ammo,
                    spawnPoint.position + (spawnPoint.forward * projectileOffset),
                    Quaternion.identity // Use identity rotation; the offset will be handled in Initialize in Projectile.cs
                );

                // Disable the "Billboard" and "ItemPositioning" scripts on the projectile
                Billboard billboard = projectile.GetComponent<Billboard>();
                if (billboard != null) Destroy(billboard);

                ItemPositioning itemPositioning = projectile.GetComponent<ItemPositioning>();
                if (itemPositioning != null) Destroy(itemPositioning);

                Projectile proj = projectile.GetComponent<Projectile>();
                if (proj != null)
                {
                    proj.Initialize(spawnPoint.position, enemy.position + new Vector3(0, 2f, 0)); //2f vertical to shoot higher at the enemy
                }
                Debug.Log("Shot " + ammo);

                ConsumeItem();

            playerGridMovement.HideActionButton();
            gameManager.isEnemysTurn = true; // Switch to the enemy's turn
        }
    }

    private void ConsumeItem() {
        if (inventoryManager.rightHandSlot.texture.name.Contains("Bow") || 
            inventoryManager.rightHandSlot.texture.name.Contains("Crossbow") ||
            inventoryManager.rightHandSlot.texture.name.Contains("Spell") || 
            inventoryManager.rightHandSlot.texture.name.Contains("Scroll")) {
                return;
            } else {
            inventoryManager.EmptyRightHand();
        }
    }
}
