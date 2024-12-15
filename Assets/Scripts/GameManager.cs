using UnityEngine;

public class GameManager : MonoBehaviour
{

    public bool isGameActive = false;
    public bool isExploring = false;
    public bool isFighting = false;
    public bool isPlayersTurn = true;
    public bool isEnemysTurn = false;
    public Enemy activeEnemy;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    
    
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetActiveEnemy(Enemy enemy) {
        activeEnemy = enemy;
    }
}
