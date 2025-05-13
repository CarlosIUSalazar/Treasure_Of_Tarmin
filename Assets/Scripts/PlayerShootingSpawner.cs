using System;
using System.Collections;
using UnityEngine;

public class PlayerShootingSpawner : MonoBehaviour
{
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private Transform spawnPoint; // Assign this to the position where the arrow should appear
    [SerializeField] private bool isPlayerAttacking = false;
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


    public void ShootAtEnemy(Transform enemy)
    {
        Debug.Log("Attack button pressed before if");
        if (gameManager.isPlayersTurn && !isPlayerAttacking)
        {
            Debug.Log("Attack button pressed");
            isPlayerAttacking = true; // Prevent additional attack calls
            ItemMapping currentItemMapping = inventoryManager.FigureOutCurrentRightHandItemMapping();
            GameObject ammo;

            if (currentItemMapping == null)
            {
                Debug.LogWarning("No current item mapping found! Make sure an item is selected.");
                Debug.Log("No current item mapping found! Make sure an item is selected");
                gameManager.SetPlayerMessage("No Weapon Selected!");
                isPlayerAttacking = false;
                return;
            }

            if (currentItemMapping.ammo != null) 
            {
                ammo = currentItemMapping.ammo;
            } 
            else 
            {
                Debug.LogWarning($"Item '{currentItemMapping.itemName}' does not have ammo assigned!");
                Debug.Log($"Item '{currentItemMapping.itemName}' does not have ammo assigned!");
                gameManager.SetPlayerMessage("No Weapon Selected!");
                isPlayerAttacking = false;
                return;
            }

            Debug.Log("Current Ammo is: " + currentItemMapping.ammo);

            // Deduct one arrow from the player's inventory if user holds Bow or Crossbow only, if not turns is forfeit
            if (inventoryManager.rightHandSlot.texture.name.Contains("Bow") || 
                inventoryManager.rightHandSlot.texture.name.Contains("Crossbow")) {
                    if  (player.arrows <= 0) {// Only shoot if the player has arrows
                        Debug.Log("No Arrows Left!");
                        gameManager.PlayBeepSoundEffect();
                        gameManager.SetPlayerMessage("No Arrows Left!");
                        playerGridMovement.HideActionButton();
                        gameManager.isPlayersTurn = false; // Switch to the enemy's turn
                        gameManager.isEnemysTurn = true; // Switch to the enemy's turn
                        // let them try again next turn
                        isPlayerAttacking = false;
                        return;
                    } else {
                        player.ModifyArrows(-1);
                    }
            }
    
            Debug.Log("Shot prefab is " + currentItemMapping.ammo.name);
            if (currentItemMapping.ammo.name.Contains("Fireball"))
                gameManager.PlayFireballSoundEffect();
            if (currentItemMapping.ammo.name.Contains("Lightning")) 
                gameManager.PlayThunderSoundEffect();


            // Instantiate the dart (projectile) with an additional rotation of 220 degrees in Y
            GameObject projectile = Instantiate(
                currentItemMapping.ammo,
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
            Debug.Log("Shot " + currentItemMapping.ammo);

            StartCoroutine(ResetPlayerAttackFlag());
            //ConsumeItem();


            //BRIBE MECHANIC
            if (currentItemMapping.isContainer == true) {
                Debug.Log("Enters Bribe mechanic");
                StartCoroutine(DelayBribeEscape());
                //playerGridMovement.MoveBackwards(true);
                //gameManager.isFighting = false;
                //playerGridMovement.HideActionButton();
                //gameManager.isPlayersTurn = true;
                return;
            }

            gameManager.isPlayersTurn = false;
            //gameManager.isEnemysTurn = true; // Switch to the enemy's turn
            playerGridMovement.HideActionButton();
            gameManager.isEnemysTurn = true; // Switch to the enemy's turn
        }
    }


    public void CheckWeaponBreakingChance(ItemMapping currentItemMapping) {
        // only multi‐use weapons can break
        if (!currentItemMapping.isMultiUseWeapon) 
            return;

        // determine break % based on exact rarity, not string.Contains
        int breakChancePercent = 0;
        if (currentItemMapping.isWarWeapon)
        {
            if (currentItemMapping.name.Contains("Tan"))    breakChancePercent = 2;
            else if (currentItemMapping.name.Contains("Orange")) breakChancePercent = 3;
            else if (currentItemMapping.name.Contains("Blue"))   breakChancePercent = 5;
            else if (currentItemMapping.name.Contains("Grey"))   breakChancePercent = 7;
            else if (currentItemMapping.name.Contains("Yellow")) breakChancePercent = 9;
            else if (currentItemMapping.name.Contains("White"))  breakChancePercent = 15;
        }
        else if (currentItemMapping.isSpiritualWeapon)
        {
            if (currentItemMapping.name.Contains("Blue"))   breakChancePercent = 2;
            else if (currentItemMapping.name.Contains("Grey"))   breakChancePercent = 3;
            else if (currentItemMapping.name.Contains("White"))  breakChancePercent = 5;
            else if (currentItemMapping.name.Contains("Pink"))   breakChancePercent = 7;
            else if (currentItemMapping.name.Contains("Red"))    breakChancePercent = 9;
            else if (currentItemMapping.name.Contains("Purple")) breakChancePercent = 12;
        }

        // roll a number 0..99
        int roll = UnityEngine.Random.Range(0, 100);
        if (roll < breakChancePercent)
        {
            gameManager.SetPlayerMessage("Your Weapon Broke!");
            inventoryManager.EmptyRightHand();
        }
    } 


    // public int MultiUseWeaponFired() {
    //     ItemMapping currentItemMapping = FigureOutCurrentRightHandItemMapping();
    //     if (currentItemMapping.isMultiUseWeapon) {
    //         Debug.Log("Using MULTIUSE WEAPON");
    //         float damageWar = currentItemMapping.warAttackPower;
    //         float damageSpiritual = currentItemMapping.spiritualAttackPower;
    //         float damage = (damageWar > damageSpiritual) ? damageWar : damageSpiritual;
    //         float bonusDamage = UnityEngine.Random.Range(damage * 0.05f, damage * 0.25f);
    //         damage = damage + bonusDamage;
    //         int multiUseAttackDamage = Mathf.RoundToInt(damage);
    //         Debug.Log("Damage to Enemy with Multiuse is " + multiUseAttackDamage);
    //         //gameManager.activeEnemy.TakeDamage(attackDamage);
    //         //enemy.TakeDamage(attackDamage);
    //         return multiUseAttackDamage;
    //     } else {
    //         return 0; //No Multiuse weapon in hand
    //     }
    // }


    IEnumerator ResetPlayerAttackFlag()
    {
        yield return new WaitForSeconds(1f); // Adjust delay as needed
        isPlayerAttacking = false;
    }


    IEnumerator DelayBribeEscape() {
        yield return new WaitForSeconds(0.8f);
        playerGridMovement.MoveBackwards(true);
        gameManager.SetPlayerMessage("Successfully Bribed!");
    }

    // public void ConsumeItem() {
    //     if (inventoryManager.rightHandSlot.texture.name.Contains("Bow") || 
    //         inventoryManager.rightHandSlot.texture.name.Contains("Crossbow") ||
    //         inventoryManager.rightHandSlot.texture.name.Contains("Spell") || 
    //         inventoryManager.rightHandSlot.texture.name.Contains("Scroll")) {
    //             return;
    //         } else {
    //         inventoryManager.EmptyRightHand();
    //     }
    // }
}
