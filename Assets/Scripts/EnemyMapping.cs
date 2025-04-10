//https://docs.google.com/spreadsheets/d/1jqqI34sGs0kx1J_uOj5Jc2wu2Sv4D0Um0ZMVFC0thjk/edit?gid=1462241840#gid=1462241840
using UnityEngine;

[CreateAssetMenu(fileName = "EnemyMapping", menuName = "Inventory/EnemyMapping")]
public class EnemyMapping : ScriptableObject
{
    [Header("Identification")]
    public string enemyName;
    
    [Header("Base Health")]
    public int baseWarHP;
    public int baseSpiritualHP;
    
    [Header("Base Stats")]
    public int baseAttack;
    public int baseDefense;
    
    [Header("Variant Modifiers")]
    [Tooltip("Multiplier for enemy stats based on the enemy's color variant (e.g., 1.0 for white, 1.3 for grey, 1.5 for orange)")]
    public float colorMultiplier;
    public int shieldBonus;
    
    [Header("Scaling Factors Per Floor")]
    public float hpPerFloor;
    public float attackPerFloor;
    public float defensePerFloor;
    
    [Header("Enemy Type Flags")]
    public bool isWar;
    public bool isSpiritual;
    public bool isHorrible;
    public bool isMinotaur;
    
    [Header("Projectile Prefabs")]
    public GameObject projectilePrefab;
    public GameObject secondaryProjectilePrefab;
}