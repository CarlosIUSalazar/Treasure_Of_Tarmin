using UnityEngine;

public class Player : MonoBehaviour
{
    private int totalMaxPhysicalStrength = 199;
    private int totalMaxSpiritualStrength = 99;
    private int totalMaxPhysicalArmor = 199;
    private int totalMaxSpiritualArmor = 99;
    
    public int currentMaxPhysicalStrength = 90;
    public int currentMaxSpiritualStrength = 50;
    public int physicalStrength;
    public int physicalArmor;
    public int physicalWeapon;
    public int spiritualStrength;
    public int spiritualArmor;
    public int spiritualWeapon;
    public int score;
    public int arrows;
    public int food;
    public int floor;
    public bool canRest = false;
    
    //Events to notify UI changes
    public delegate void OnStatChanged();
    public event OnStatChanged OnPlayerStatsUpdated;
    GameManager gameManager;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        InitializeValues();
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

    void Update()
    {
        CheckIfCanRest();
    }

    private void InitializeValues() {
        physicalStrength = 90;
        physicalArmor = 0;
        physicalWeapon = 10;
        spiritualStrength = 50;
        spiritualArmor = 5;
        spiritualWeapon = 1;
        score = 0;
        arrows = 10;
        food = 10;
        floor = 1;

        //Trigger UI update at start
        OnPlayerStatsUpdated?.Invoke();
    }

    public void ModifyPhysicalStrength(int amount)
    {
        physicalStrength += amount;
        Debug.Log($"Player PhysicalStrength: {physicalStrength}");

        if (physicalStrength <= 0)
        {
            Die();
        }

        OnPlayerStatsUpdated?.Invoke();
    }
    
    public void ModifyArrows(int amount) {
        arrows += amount;
        if (arrows < 0) {
            arrows = 0;
        }
        Debug.Log($"Arrows Left = {arrows}");
        OnPlayerStatsUpdated?.Invoke();
    }

    public void ModifyFood(int amount) {
        food += amount;
        OnPlayerStatsUpdated?.Invoke();
    }

    private void Die()
    {
        Debug.Log("Player Defeated!");
        // Add logic for player death, like restarting the level or showing a game-over screen
        gameObject.SetActive(false);
    }

    public void CheckIfCanRest() {
        if (((!gameManager.isFighting && (physicalStrength < currentMaxPhysicalStrength)) || (!gameManager.isFighting && (spiritualStrength < currentMaxSpiritualStrength))) && food > 0) {
            canRest = true;
        } else {
            canRest = false;
        }
    }

    public void Rest() {
        // Resting.  Resting brings the player current health up towards the current Max Health by using
        // the availalbe food (flour) units
        Debug.Log("Player Rested");
        if (physicalStrength < currentMaxPhysicalStrength && food > 0) {
            while (physicalStrength < currentMaxPhysicalStrength && food > 0) {
                physicalStrength++;
                food--;
            }
        }
        canRest = false;
        OnPlayerStatsUpdated?.Invoke();
    }
}