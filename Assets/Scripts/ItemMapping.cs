using UnityEngine;

[CreateAssetMenu(fileName = "ItemMapping", menuName = "Inventory/ItemMapping")]
public class ItemMapping : ScriptableObject
{
    public string itemName; // A unique name for the item
    public GameObject item3DPrefab; // Reference to the 3D prefab
    public Texture2D item2DSprite; // Reference to the 2D sprite
    public GameObject ammo; //Which ammo this weapon will shoot
    public bool isContainer;
    public bool isLocked;
    public bool isTan;
    public bool isOrange;
    public bool isBlue;
    public bool isShield;
    public bool isArmor;
    public bool isTreasure;
    public bool isQuiver;
    public bool isFlour;
    public bool isWarWeapon;
    public bool isSpiritualWeapon;
    public bool isHelmet;
    public bool isBreastPlate;
    public bool isGauntlet;
    public bool isHauberk;
    public bool isRing;
    public bool isPotion;
    public bool isKey;
    public bool isBomb;
    public int warAttackPower;
    public int spiritualAttackPower;
    public int warDefense;
    public int spiritualDefense;
}
