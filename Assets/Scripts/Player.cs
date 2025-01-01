using UnityEngine;

public class Player : MonoBehaviour
{
    private int maxPhysicalStrength = 199;
    private int maxPhysicalArmor = 199;
    private int maxSpiritualStrength = 99;
    private int maxSpiritualArmor = 99;
    
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
    
    //Events to notify UI changes
    public delegate void OnStatChanged();
    public event OnStatChanged OnPlayerStatsUpdated;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        InitializeValues();
    }

    void Update()
    {}

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
        Debug.Log($"Arrows Left = {arrows}");
        OnPlayerStatsUpdated?.Invoke();
    }

    private void Die()
    {
        Debug.Log("Player Defeated!");
        // Add logic for player death, like restarting the level or showing a game-over screen
        gameObject.SetActive(false);
    }
}