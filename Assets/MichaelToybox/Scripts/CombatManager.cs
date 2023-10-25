using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatManager : MonoBehaviour
{
    [Header("Player Information")]
    public PlayerManager playerManager;
    public PlayerMinionZone playerMinionZone;
    public List<BaseHero> playerHeroes;
    public List<BaseMinion> playerMinions;
    public GameObject heroSlot1;
    public GameObject heroSlot2;
    public GameObject heroSlot3;

    [Header("AI Information")]
    public EnemyManager enemyManager;
    public EnemyMinionZone enemyMinionZone;
    public List<BaseHero> enemyHeroes;
    public List<BaseMinion> enemyMinions;
    public GameObject enemyHolder;
    [SerializeField] BoardStateInformation boardInfo;

    [Header("Combat State")]
    public CombatState  state = CombatState.STARTING;
    public int turnCount = 0;

    private void Start()
    {
        boardInfo = GameObject.FindObjectOfType<BoardStateInformation>();
        StartCombat();
    }

    public void StartCombat()
    {
        state = CombatState.STARTING;

        //Gets enemy references
        //we are getting these references on combat start because I plan to load new combat scenerios under the enemyHolder object
        enemyManager = enemyHolder.GetComponentInChildren<EnemyManager>(); //enemy manager reference
        enemyMinionZone = enemyHolder.GetComponentInChildren<EnemyMinionZone>(); //enemy minion zone reference
        enemyHeroes.Clear(); //clears enemy hero list
        foreach (BaseHero hero in enemyHolder.GetComponentsInChildren<BaseHero>()) //gets new heroes
            enemyHeroes.Add(hero);

        //Updates minion zones
        enemyMinionZone.RefreshMinionsInZoneList();
        playerMinionZone.RefreshMinionsInZoneList();

        turnCount = 1; //resets turn count variable
        StartPlayerTurn(); //starts player turn (player goes first)
    }

    public void StartPlayerTurn()
    {
        //TODO: Trigger start of turn effects for the player

        boardInfo.UpdateBoardStateInfo(); //updates board state information. used for enemy AI

        state = CombatState.PLAYER_TURN;
        playerManager.StartTurn();
        playerMinionZone.EnableMinionAttacks(); //enables minions in the combat zone to attack
        foreach (BaseHero hero in playerHeroes)
            hero.canAttack = true;

    }

    public void EndPlayerTurn()
    {
        state = CombatState.PLAYER_END;
        playerManager.EndTurn();

        //TODO: trigger end of turn effects for the player

        StartEnemyTurn();
    }

    public void StartEnemyTurn()
    {
        //TODO: Trigger start of turn effects for the enemy

        state = CombatState.ENEMY_TURN;

        enemyManager.StartTurn();
        enemyMinionZone.EnableMinionAttacks(); //enables minions in the combat zone to attack
        foreach (BaseHero hero in enemyHeroes)
            hero.canAttack = true;

        StartCoroutine(TempEndEnemyTurnDelay());
    }

    public void EndEnemyTurn()
    {
        state = CombatState.ENEMY_END;
        turnCount++;

        enemyManager.EndTurn();

        //TODO: trigger end of turn effects for the enemy

        StartPlayerTurn();
    }

    /// <summary>
    /// Checks which turn it is based on the team passed in
    /// Example: if passed in team is PLAYER and the combat state is PLAYER_TURN then return true
    /// </summary>
    /// <param name="team"></param>
    /// <returns></returns>
    public bool CheckTurn(Team team)
    {
        if (team == Team.PLAYER && state == CombatState.PLAYER_TURN)
            return true;
        if (team == Team.ENEMY && state == CombatState.ENEMY_TURN)
            return true;
        return false;
    }

    IEnumerator TempEndEnemyTurnDelay()
    {
        yield return new WaitForSeconds(1.5f);
        EndEnemyTurn();
    }

    /// <summary>
    /// Returns all minions and heroes in play
    /// </summary>
    /// <returns></returns>
    public List<BaseCard> GetAllInPlay()
    {
        List<BaseCard> allCards = new List<BaseCard>();

        //Gets all player heroes
        foreach (BaseCard card in playerHeroes)
            allCards.Add(card);
        //Gets all player minions
        foreach (BaseCard card in playerMinions)
            allCards.Add(card);
        //Gets all enemy heroes
        foreach (BaseCard card in enemyHeroes)
            allCards.Add(card);
        //Gets all enemy minions
        foreach (BaseCard card in enemyMinions)
            allCards.Add(card);

        return allCards;
    }
    /// <summary>
    /// Returns all player minions and player heroes in play
    /// </summary>
    /// <returns></returns>
    public List<BaseCard> GetAllInPlayPlayer()
    {
        List<BaseCard> allCards = new List<BaseCard>();

        //Gets all player heroes
        foreach (BaseCard card in playerHeroes)
            allCards.Add(card);
        //Gets all player minions
        foreach (BaseCard card in playerMinions)
            allCards.Add(card);

        return allCards;
    }
    /// <summary>
    /// Returns all enemy minions and enemy heroes in play
    /// </summary>
    /// <returns></returns>
    public List<BaseCard> GetAllInPlayEnemy()
    {
        List<BaseCard> allCards = new List<BaseCard>();
        //Gets all enemy heroes
        foreach (BaseCard card in enemyHeroes)
            allCards.Add(card);
        //Gets all enemy minions
        foreach (BaseCard card in enemyMinions)
            allCards.Add(card);

        return allCards;
    }
}


public enum Team
{
    PLAYER,
    ENEMY,
    NONE
}

public enum TargetingInfo
{
    ANY_OR_ALL,
    SAME,
    OPPOSITE,
    ANY_MINION,
    SAME_MINION,
    OPPOSITE_MINION,
    ANY_HERO,
    SAME_HERO,
    OPPOSITE_HERO
}

public enum CombatState
{
    STARTING,
    PLAYER_TURN_STARTING,
    PLAYER_TURN,
    PLAYER_TURN_ENDING,
    PLAYER_END,
    ENEMY_TURN_STARTING,
    ENEMY_TURN,
    ENEMY_TURN_ENDING,
    ENEMY_END,
    ENDED,
    OFF
}
