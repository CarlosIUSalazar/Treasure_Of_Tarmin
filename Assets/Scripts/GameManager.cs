using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    [SerializeField] public bool isGameActive = false;
    [SerializeField] public bool isExploring = false;
    [SerializeField] public bool isFighting = false;
    [SerializeField] public bool isPlayersTurn = true;
    [SerializeField] public bool isEnemysTurn = false;

    [SerializeField] private TextMeshProUGUI physicalStrengthText;
    [SerializeField] private TextMeshProUGUI physicalArmorText;
    [SerializeField] private TextMeshProUGUI physicalWeaponText;
    [SerializeField] private TextMeshProUGUI spiritualStrengthText;
    [SerializeField] private TextMeshProUGUI spiritualArmorText;
    [SerializeField] private TextMeshProUGUI spiritualWeaponText;
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI arrowsText;
    [SerializeField] private TextMeshProUGUI foodText;
    [SerializeField] private TextMeshProUGUI floorText;

    public Enemy activeEnemy;
    private Player player;    

    void Start()
    {
        player = GameObject.Find("Player").GetComponent<Player>();

        //Subscribe to Player's stat update event
        player.OnPlayerStatsUpdated += UpdateUI;

        //Update UI Initially
        UpdateUI();
    }

    void Update()
    {
        //physicalStrengthText.text = player.ReturnCurrentPhysicalStrength.ToString();
    }

    private void UpdateUI() {
        physicalStrengthText.text = $"{player.physicalStrength}";
        physicalArmorText.text = $"{player.physicalArmor}";
        physicalWeaponText.text = $"{player.physicalWeapon}";

        spiritualStrengthText.text = $"{player.spiritualStrength}";
        spiritualArmorText.text = $"{player.spiritualArmor}";
        spiritualWeaponText.text = $"{player.spiritualWeapon}";

        scoreText.text = $"{player.score}";
        arrowsText.text = $"{player.arrows}";
        foodText.text = $"{player.food}";
        floorText.text = $"{player.floor}";
    }

    public void SetActiveEnemy(Enemy enemy) {
        activeEnemy = enemy;
    }
}