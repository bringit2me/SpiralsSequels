using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardStateInformation : MonoBehaviour
{
    CombatManager combatManager;
    [Header("Generic Info Player")]
    public int playerMinionCount;
    public int playerTotalAttack;
    public int playerTotalHeroAttack;
    public int playerTotalHealth;
    public int playerTotalHeroHealth;
    [Header("Generic Info Enemy")]
    public int enemyMinionCount;
    public int enemyTotalAttack;
    public int enemyTotalHeroAttack;
    public int enemyTotalHealth;
    public int enemyTotalHeroHealth;

    private void Start()
    {
        //gets combat manager reference
        combatManager = GameObject.FindObjectOfType<CombatManager>();
    }

    /// <summary>
    /// Updates information on the board state. such as attack totals.
    /// PLANNED: this information is planned to be used with the enemy ai to check for things and influence its behavior.
    /// Example: if playerTotalAttack is greater than enemyTotalHeroHealth then it will try to play defensive (remove minions / heal heroes) to avoid dying
    /// </summary>
    public void UpdateBoardStateInfo()
    {
        UpdatePlayerGenericInfo();
        UpdateEnemyGenericInfo();
    }

    /// <summary>
    /// Updates generic player info
    /// Minion count
    /// Total attack and total hero attack
    /// Total health and total hero health
    /// </summary>
    void UpdatePlayerGenericInfo()
    {
        playerMinionCount = 0;
        playerTotalAttack = 0;
        playerTotalHeroAttack = 0;
        playerTotalHealth = 0;
        playerTotalHeroHealth = 0;

        foreach(BaseMinion minion in combatManager.playerMinions) //loops through player minions
        {
            playerTotalAttack += minion.attack;
            playerTotalHealth += minion.health;
            playerMinionCount += 1;
        }

        foreach (BaseHero hero in combatManager.playerHeroes) //loops through player heroes
        {
            playerTotalAttack += hero.attack;
            playerTotalHeroAttack += hero.attack;
            playerTotalHealth += hero.health;
            playerTotalHeroHealth += hero.health;
        }
    }

    /// <summary>
    /// Updates generic enemy info
    /// Minion count
    /// Total attack and total hero attack
    /// Total health and total hero health
    /// </summary>
    void UpdateEnemyGenericInfo()
    {
        enemyMinionCount = 0;
        enemyTotalAttack = 0;
        enemyTotalHeroAttack = 0;
        enemyTotalHealth = 0;
        enemyTotalHeroHealth = 0;

        foreach (BaseMinion minion in combatManager.enemyMinions) //loops through enemy minions
        {
            enemyTotalAttack += minion.attack;
            enemyTotalHealth += minion.health;
            enemyMinionCount += 1;
        }

        foreach (BaseHero hero in combatManager.enemyHeroes) //loops through enemy heroes
        {
            enemyTotalAttack += hero.attack;
            enemyTotalHeroAttack += hero.attack;
            enemyTotalHealth += hero.health;
            enemyTotalHeroHealth += hero.health;
        }
    }
}
