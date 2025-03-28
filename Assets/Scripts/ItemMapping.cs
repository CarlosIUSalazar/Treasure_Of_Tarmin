using UnityEngine;

[CreateAssetMenu(fileName = "ItemMapping", menuName = "Inventory/ItemMapping")]
public class ItemMapping : ScriptableObject
{
    public string itemName; // A unique name for the item
    public GameObject item3DPrefab; // Reference to the 3D prefab
    public Texture2D item2DSprite; // Reference to the 2D sprite
    public GameObject ammo; //Which ammo this weapon will shoot
    public bool isContainer;
    public bool isShield;
    public bool isArmor;
    public bool isTreasure;
    public bool isWarWeapon;
    public bool isSpiritualWeapon;
    public bool isHelmet;
    public bool isBreastPlate;
    public bool isGauntlet;
    public bool isHauberk;
    public bool isRing;
    public bool isPotion;
    public bool isKey;
    public int warAttackPower;
    public int spiritualAttackPower;
    public int warDefense;
    public int spiritualDefense;
}
