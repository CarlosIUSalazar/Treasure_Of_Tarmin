using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System;
using Unity.VisualScripting;

public class GameManager : MonoBehaviour
{
    public DifficultyLevel CurrentDifficulty => GameSettings.SelectedDifficulty;

    [SerializeField] public bool isGameActive = false;
    [SerializeField] public bool isExploring = false;
    [SerializeField] public bool isFighting = false;
    [SerializeField] public bool isPlayersTurn = true;
    [SerializeField] public bool isEnemysTurn = false;
    public bool isFreeAttackPhase = false;
    public bool isPassiveFight = false;
    public bool ambushInProgress = false;
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
    [SerializeField] private TextMeshProUGUI menuScoreText;
    [SerializeField] private Button mapBackButton;

    public Enemy activeEnemy;
    private Player player;  
    PlayerGridMovement playerGridMovement;  
    ViewSwitcher viewSwitcher;
    MazeGenerator mazeGenerator;
    public bool isMazeTransparent = false; // Special book
    public bool isSmallPinkPotionActive = false; //Helps find better loot

    void Start()
    {
        viewSwitcher = GameObject.Find("ViewSwitcher").GetComponent<ViewSwitcher>();
        player = GameObject.Find("Player").GetComponent<Player>();
        //floorManager = GameObject.Find("FloorManager").GetComponent<FloorManager>();
        playerGridMovement = GameObject.Find("Player").GetComponent<PlayerGridMovement>();
        mazeGenerator = GameObject.Find("MazeGenerator").GetComponent<MazeGenerator>();
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
        menuScoreText.text = $"{player.score}";
        arrowsText.text = $"{player.arrows}";
        foodText.text = $"{player.food}";
        floorText.text = $"{player.floor}";

        menuPhysicalArmorText.text = $"{player.physicalArmor}";
        menuSpiritualArmorText.text = $"{player.spiritualArmor}";
    }


    public void UpdateEnemyHP(int currentEnemyHP) {
        enemyHPText.text = $"{currentEnemyHP}";
        if (currentEnemyHP <= 15) {
            enemyHPText.color = Color.blue;
        } else {
            enemyHPText.color = Color.green;
        }
    }


public void SetActiveEnemy(Enemy enemy)
{
    activeEnemy = enemy;
    if (enemy == null)
    {
        isFighting = false;
        ambushInProgress = false; // Reset global ambush flag.
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


    public void GameOverSequence() {
        mazeGenerator.currentPlayerBlock.playerCursor.transform.localScale = new Vector3 (2,2,2);
        viewSwitcher.SwitchToMapAndArmorView();
        mapBackButton.gameObject.SetActive(false);
    }
}