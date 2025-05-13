using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine.TextCore.LowLevel;

public class GameManager : MonoBehaviour
{
    public DifficultyLevel CurrentDifficulty => GameSettings.SelectedDifficulty;

    [SerializeField] public bool isGameActive = false;
    [SerializeField] public bool isExploring = false;
    [SerializeField] public bool isFighting = false;
    [SerializeField] public bool isPlayersTurn = true;
    [SerializeField] public bool isEnemysTurn = false;
    public MazeBlock currentMazeBlock;
    public string currentMazeBlockColor;
    public int currentFloor;
    public bool isFreeAttackPhase = false;
    public bool isPassiveFight = false;
    public bool ambushInProgress = false;
    public int WarHPBookMultiplier = 1;
    public int SpiritualHPBookMultiplier = 1;
    private int completedFull255MazeFloorLoops = 0;

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

    [SerializeField] private AudioClip beepClip;  
    [SerializeField] private AudioClip bombClip;   
    [SerializeField] private AudioClip clickClip;  
    [SerializeField] private AudioClip deathClip;   
    [SerializeField] private AudioClip fireballClip;   
    [SerializeField] private AudioClip gameWinClip;   
    [SerializeField] private AudioClip ladderClip;   
    [SerializeField] private AudioClip punkClip;
    [SerializeField] private AudioClip resurrectClip;   
    [SerializeField] private AudioClip roarAmbushClip;   
    [SerializeField] private AudioClip[] roarClips;   
    [SerializeField] private AudioClip swooshClip;   
    [SerializeField] private AudioClip thunderClip;   
    [SerializeField] private AudioClip whooshClip;      
    [SerializeField] private AudioClip whooshEndClip;   

    private AudioSource audioSource;

    public Enemy activeEnemy;
    private Player player;  
    PlayerGridMovement playerGridMovement;  
    ViewSwitcher viewSwitcher;
    MazeGenerator mazeGenerator;
    public bool isMazeTransparent = false; // Special book
    public bool isSmallPinkPotionActive = false; //Helps find better loot
    public bool isSmallPurplePotionActive = false; //Hides Enemies
    

    void Start()
    {
        viewSwitcher = GameObject.Find("ViewSwitcher").GetComponent<ViewSwitcher>();
        player = GameObject.Find("Player").GetComponent<Player>();
        //floorManager = GameObject.Find("FloorManager").GetComponent<FloorManager>();
        playerGridMovement = GameObject.Find("Player").GetComponent<PlayerGridMovement>();
        mazeGenerator = GameObject.Find("MazeGenerator").GetComponent<MazeGenerator>();
        audioSource = GetComponent<AudioSource>();
        enemyHPText.gameObject.SetActive(false);

        //Subscribe to Player's stat update event
        player.OnPlayerStatsUpdated += UpdateUI;

        //Update UI Initially
        UpdateUI();
        Debug.Log($"Game started with difficulty: {CurrentDifficulty}");
        mapBackButton.gameObject.SetActive(false); //Hide this button during the intro sequence

        StartCoroutine(ViewMapUponGameStart());
    }


    IEnumerator ViewMapUponGameStart(){
        viewSwitcher.SwitchToMapAndArmorView();
        yield return new WaitForSeconds(3f);
        viewSwitcher.SwitchToGameView();
        PlayLadderSoundEffect();
        mapBackButton.gameObject.SetActive(true); //Reenable this back button after initial sequence
    }


    public void UpdateCurrentMazeBlockType(MazeBlock currentPlayerMazeBlock) {
        currentMazeBlock = currentPlayerMazeBlock;
        currentMazeBlockColor = currentPlayerMazeBlock.colorType.ToString();
        Debug.Log("GameManager, Current MazeBlock is" + currentMazeBlock);
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
        floorText.text = $"{currentFloor}";

        menuPhysicalArmorText.text = $"{player.physicalArmor}";
        menuSpiritualArmorText.text = $"{player.spiritualArmor}";
    }


    public void CompletedMazesCounterIncrease() {
        completedFull255MazeFloorLoops++;
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
        PlayDeathSoundEffect();
        mazeGenerator.currentPlayerBlock.playerCursor.transform.localScale = new Vector3 (2,2,2);
        viewSwitcher.SwitchToMapAndArmorView();
        mapBackButton.gameObject.SetActive(false);
    }


    public void HideAllEnemies(bool isEffectAccrossFloors) { // Small Purple Potion effect 
        //These 4 lines works so that you can use the purple potion and wear off within the same floor, witout them oyu need to change floors for it to wear off
        if (isSmallPurplePotionActive == false) {
            StartCoroutine(PurplePotionTimer());
            isSmallPurplePotionActive = true;
        }
        
        PlayWhooshSoundEffect();

        GameObject[] allEnemies = GameObject.FindGameObjectsWithTag("Enemy");
        List<GameObject> nonMinotaurEnemies = new List<GameObject>();
        
        //Filter out the Minotaur and EvilDoors if exists on current floor
        for (int i = 0; i < allEnemies.Length; i++) {
            Debug.Log("Enemy #"+i+ " is " + allEnemies[i].name);
            if (allEnemies[i].name != "Minotaur.vox(Clone)" && allEnemies[i].name != "EvilDoorTan" && allEnemies[i].name != "EvilDoorBlue" && allEnemies[i].name != "EvilDoorYellow") {
                Debug.Log("Added enemy " + allEnemies[i].name);
                nonMinotaurEnemies.Add(allEnemies[i]);
            }
        }

        for (int i = 0; i < nonMinotaurEnemies.Count; i++) {
            GameObject enemy = nonMinotaurEnemies[i];
            BoxCollider boxCollider = enemy.GetComponent<BoxCollider>();

            if (boxCollider != null) {
                boxCollider.enabled = false;
            }

            //Get default named child gameobject where the mesh renderer is
            Transform defaultChild = enemy.transform.Find("default");
            if (defaultChild != null) {
                MeshRenderer mesh = defaultChild.GetComponent<MeshRenderer>();
                mesh.enabled = false;
            }
        }

        //Start a coroutine to disable the potion effect.  If the player has the potion active and changes floor, do not trigger a new timer
        if (isEffectAccrossFloors == true && isSmallPurplePotionActive == true) {
            StartCoroutine(PurplePotionTimer());
        }
    }


    IEnumerator PurplePotionTimer() { //After Small Purple Potion wears off
        yield return new WaitForSeconds(300f); 
        ShowAllEnemies();
        Debug.Log("Purple Small Potion Wears Off!");
    }

    private void ShowAllEnemies() {
        isSmallPurplePotionActive = false;
        GameObject[] allEnemies = GameObject.FindGameObjectsWithTag("Enemy");
        List<GameObject> nonMinotaurEnemies = new List<GameObject>();

        PlayWhooshEndSoundEffect();    
        //Filter out the Minotaur and EvilDoors if exists on current floor
        for (int i = 0; i < allEnemies.Length; i++) {
            Debug.Log("Enemy #"+i+ " is " + allEnemies[i].name);
            if (allEnemies[i].name != "Minotaur.vox(Clone)" && allEnemies[i].name != "EvilDoorTan" && allEnemies[i].name != "EvilDoorBlue" && allEnemies[i].name != "EvilDoorYellow") {
                Debug.Log("Added enemy " + allEnemies[i].name);
                nonMinotaurEnemies.Add(allEnemies[i]);
            }
        }

        for (int i = 0; i < nonMinotaurEnemies.Count; i++) {
            GameObject enemy = nonMinotaurEnemies[i];
            BoxCollider boxCollider = enemy.GetComponent<BoxCollider>();

            if (boxCollider != null) {
                boxCollider.enabled = true;
            }

            //Get default named child gameobject where the mesh renderer is
            Transform defaultChild = enemy.transform.Find("default");
            if (defaultChild != null) {
                MeshRenderer mesh = defaultChild.GetComponent<MeshRenderer>();
                mesh.enabled = true;
            }
        }

        //IF A MONSTER RE-SPAWNS WITH THE PLAYER DESTROY THE MONSTER (ITS OK BECAUSE IT WONT BE THE MINOTAUR)
        GameObject player = GameObject.FindWithTag("Player");
        foreach (GameObject enemy in nonMinotaurEnemies) {
            Debug.Log("Checking overlapping with " + enemy.name);
            if (Vector3.Distance(enemy.transform.position, player.transform.position) < 3f) {
                Debug.Log("Enemy overlapped, destroyed " + enemy.name);
                Destroy(enemy);
            }
        }
    }


    public void UpdateHPBooksStatus() {
        if (WarHPBookMultiplier == 2) {
            player.currentSpiritualBookCurrentCapHP = 29;

            physicalStrengthText.color = Color.blue;
            spiritualStrengthText.color = Color.white;
        }
        
        if (SpiritualHPBookMultiplier == 2) {
            player.currentWarBookCurrentCapHP = 49;

            spiritualStrengthText.color = Color.blue;
            physicalStrengthText.color = Color.white;
        }
        
        if (WarHPBookMultiplier == 3) {
            player.currentSpiritualBookCurrentCapHP = 29;

            physicalStrengthText.color = Color.yellow;
            spiritualStrengthText.color = Color.white;
        }

        if (SpiritualHPBookMultiplier == 3) {
            player.currentWarBookCurrentCapHP = 49;

            spiritualStrengthText.color = Color.yellow;
            physicalStrengthText.color = Color.white;
        }

        if (WarHPBookMultiplier == 4) {
            player.currentSpiritualBookCurrentCapHP = 29;
            physicalStrengthText.color = Color.cyan;
            spiritualStrengthText.color = Color.white;
        }

        if (SpiritualHPBookMultiplier == 4) {
            player.currentWarBookCurrentCapHP = 49;

            spiritualStrengthText.color = Color.cyan;
            physicalStrengthText.color = Color.white;
        }
    }

    public void PlayBeepSoundEffect() {  //Error sound
        audioSource.PlayOneShot(beepClip);
    }

    public void PlayBombSoundEffect() {  
        audioSource.PlayOneShot(bombClip);
    }

    public void PlayClickSoundEffect() { //Door open
        audioSource.PlayOneShot(clickClip);
    }

    public void PlayDeathSoundEffect() {
        audioSource.PlayOneShot(deathClip);
    }

    public void PlayFireballSoundEffect() {
        audioSource.PlayOneShot(fireballClip);
    }

    public void PlayGameWinSoundEffect() {
        audioSource.PlayOneShot(gameWinClip);
    }

    public void PlayLadderSoundEffect() {
        audioSource.PlayOneShot(ladderClip);
    }

    public void PlayPunkSoundEffect() {
        audioSource.PlayOneShot(punkClip);
    }

    public void PlayResurrectSoundEffect() {
        audioSource.PlayOneShot(resurrectClip);
    }

    public void PlayRoarAmbushSoundEffect() {
        audioSource.PlayOneShot(roarAmbushClip);
    }

    public void PlayRoarSoundEffect() {
        int randomIndex;
        randomIndex = UnityEngine.Random.Range(0,roarClips.Length);
        audioSource.PlayOneShot(roarClips[randomIndex]);
    }

    public void PlaySwooshSoundEffect() {
        audioSource.PlayOneShot(swooshClip);
    }

    public void PlayThunderSoundEffect() {
        audioSource.PlayOneShot(thunderClip);
    }

    public void PlayWhooshSoundEffect() {
        audioSource.PlayOneShot(whooshClip);
    }

    public void PlayWhooshEndSoundEffect() {
        audioSource.PlayOneShot(whooshEndClip);
    }
}



