using UnityEngine;
using TMPro;
using System.Collections;
using System;

public class GameManager : MonoBehaviour
{
    public DifficultyLevel CurrentDifficulty => GameSettings.SelectedDifficulty;

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
    [SerializeField] public TextMeshProUGUI enemyHPText;
    [SerializeField] public TextMeshProUGUI playerMessages;

    [SerializeField] private TextMeshProUGUI menuPhysicalArmorText;
    [SerializeField] private TextMeshProUGUI menuSpiritualArmorText;

    public Enemy activeEnemy;
    private Player player;  
    PlayerGridMovement playerGridMovement;  
    ViewSwitcher viewSwitcher;
    //FloorManager floorManager;

    void Start()
    {
        viewSwitcher = GameObject.Find("ViewSwitcher").GetComponent<ViewSwitcher>();
        player = GameObject.Find("Player").GetComponent<Player>();
        //floorManager = GameObject.Find("FloorManager").GetComponent<FloorManager>();
        playerGridMovement = GameObject.Find("Player").GetComponent<PlayerGridMovement>();
        enemyHPText.gameObject.SetActive(false);

        //Subscribe to Player's stat update event
        player.OnPlayerStatsUpdated += UpdateUI;

        //Update UI Initially
        UpdateUI();
        Debug.Log($"Game started with difficulty: {CurrentDifficulty}");
        StartCoroutine(ViewMapUponGameStart());
    }

    IEnumerator ViewMapUponGameStart(){
        viewSwitcher.SwitchToMapAndArmorView();
        yield return new WaitForSeconds(4f);
        viewSwitcher.SwitchToGameView();
    }


    void Update()
    {}

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

        menuPhysicalArmorText.text = $"{player.physicalArmor}";
        menuSpiritualArmorText.text = $"{player.spiritualArmor}";
    }

    public void UpdateEnemyHP(int currentHP) {
        enemyHPText.text = $"{currentHP}";
    }

    public void SetActiveEnemy(Enemy enemy) {
        activeEnemy = enemy;
        if (enemy == null) {
            // Return to exploration mode and update UI
            isFighting = false;
            playerGridMovement.HideActionButton();
            isExploring = true;
            playerGridMovement.ShowDirectionalButtons();
        }
    }

    public void SetPlayerMessage(String text) {
        playerMessages.text = text;
        StartCoroutine(ClearPlayerMessage());
    }

    IEnumerator ClearPlayerMessage() {
        yield return new WaitForSeconds(1.5f);    
        playerMessages.text = "";
    }
}