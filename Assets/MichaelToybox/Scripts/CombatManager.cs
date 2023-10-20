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

    [Header("Combat State")]
    public CombatState  state = CombatState.STARTING;
    public int turnCount = 0;

    private void Start()
    {
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

        turnCount = 1; //resets turn count variable
        StartPlayerTurn(); //starts player turn (player goes first)
    }

    public void StartPlayerTurn()
    {
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
}

public enum TargetingInfo
{
    ANY,
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
    PLAYER_TURN,
    PLAYER_END,
    ENEMY_TURN,
    ENEMY_END,
    ENDED,
    OFF
}
