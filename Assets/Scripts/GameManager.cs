using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{

    [SerializeField] private bool isGameActive = false;
    [SerializeField] private bool isExploring = false;
    [SerializeField] private bool isFighting = false;
    [SerializeField] private bool isPlayersTurn = true;
    [SerializeField] private bool isEnemysTurn = false;
    [SerializeField] private TextMeshProUGUI physicalStrengthText;
    [SerializeField] private TextMeshProUGUI physicalArmorText;
    [SerializeField] private TextMeshProUGUI physicalWeaponText;
    [SerializeField] private TextMeshProUGUI spiritualStrengthText;
    [SerializeField] private TextMeshProUGUI spiritualArmorText;
    [SerializeField] private TextMeshProUGUI spiritualWeaponText;
    public Enemy activeEnemy;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetPhysicalStrength(int modifier) {
        
    }

    public void SetActiveEnemy(Enemy enemy) {
        activeEnemy = enemy;
    }
}
