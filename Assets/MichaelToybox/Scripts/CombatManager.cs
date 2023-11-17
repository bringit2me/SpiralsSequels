using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatManager : MonoBehaviour
{
    public List<BaseCard> allCardsInPlay;
    [Header("Player Information")]
    public PlayerManager playerManager;
    public HandManager playerHandManager;
    public PlayerMinionZone playerMinionZone;
    private PlayerHeroManager playerHeroManager;
    public List<BaseHero> playerHeroes;
    public List<BaseMinion> playerMinions;
    public GameObject heroSlot1;
    public GameObject heroSlot2;
    public GameObject heroSlot3;

    [Header("AI Information")]
    public RandomEncounter randomEncounter;
    public EnemyManager enemyManager;
    public HandManager enemyHandManager;
    public EnemyMinionZone enemyMinionZone;
    public List<BaseHero> enemyHeroes;
    public List<BaseMinion> enemyMinions;
    public GameObject enemyHolder;
    public BoardStateInformation boardInfo;
    public BaseEnemyAI enemyAI;

    [Header("Combat State")]
    [SerializeField] bool setupCombatOnStart = false;
    public CombatState  state = CombatState.STARTING;
    public int turnCount = 0;
    bool playerGoesFirst = false;

    SwapToCombat combatSwap;
    CardAnimationManager cardAnimationManager;

    // --- COMBAT SETUP ---

    private void Start()
    {
        boardInfo = GameObject.FindObjectOfType<BoardStateInformation>();
        combatSwap = GameObject.FindObjectOfType<SwapToCombat>();
        cardAnimationManager = GameObject.FindObjectOfType<CardAnimationManager>();

        //Resets card animation manager
        cardAnimationManager.ResetAnimationManager();

        if (setupCombatOnStart == true)
            StartCoroutine(SetupCombatManagerDelay());
    }

    IEnumerator SetupCombatManagerDelay()
    {
        yield return new WaitForEndOfFrame();
        SetupCombatManager();
    }

    public void SetupCombatManager()
    {
        state = CombatState.STARTING;

        //Gets enemy references
        //we are getting these references on combat start because I plan to load new combat scenerios under the enemyHolder object
        enemyManager = enemyHolder.GetComponentInChildren<EnemyManager>(); //enemy manager reference
        enemyHandManager = enemyHolder.GetComponentInChildren<HandManager>(); //gets enemy hand manager reference
        enemyMinionZone = enemyHolder.GetComponentInChildren<EnemyMinionZone>(); //enemy minion zone reference
        enemyAI = enemyHolder.GetComponentInChildren<BaseEnemyAI>(); //enemy AI reference
        enemyHeroes.Clear(); //clears enemy hero list
        foreach (BaseHero hero in enemyHolder.GetComponentsInChildren<BaseHero>()) //gets new heroes
            enemyHeroes.Add(hero);

        //Gets hero references
        playerManager = GameObject.Find("PlayerManager").GetComponent<PlayerManager>();
        playerHandManager = playerManager.GetComponent<HandManager>();
        playerHeroManager = GameObject.FindObjectOfType<PlayerHeroManager>();
        //Sets player mana
        playerManager.mana = 0;
        playerManager.manaPerTurn = 2;
        playerManager.manaPerTurnIncrease = 1;
        //Removes hero from each slot
        Destroy(heroSlot1.transform.GetChild(0).gameObject);
        Destroy(heroSlot2.transform.GetChild(0).gameObject);
        Destroy(heroSlot3.transform.GetChild(0).gameObject);
        //clears player hero list
        playerHeroes.Clear();
        //Copies over hero into each slot and adds them to playerHeroes list
        playerHeroes.Add(Instantiate(playerHeroManager.heroes[0],heroSlot1.transform));
        playerHeroes.Add(Instantiate(playerHeroManager.heroes[1], heroSlot2.transform));
        playerHeroes.Add(Instantiate(playerHeroManager.heroes[2], heroSlot3.transform));
        //Sets up player manager with references
        SetupPlayerManager();

        StartCombat();
    }

    public void SetupPlayerManager()
    {
        playerManager.heroes.Clear();
        playerManager.heroDecks.Clear();

        foreach(BaseHero hero in playerHeroes)
        {
            playerManager.heroes.Add(hero);
            playerManager.heroDecks.Add(hero.GetComponent<DeckManager>());
        }
    }

    public void StartCombat()
    {

        //Updates minion zones
        enemyMinionZone.RefreshMinionsInZoneList();
        playerMinionZone.RefreshMinionsInZoneList();
        //Updates mana texts
        playerManager.UpdateManaText();
        enemyManager.UpdateManaText();
        //Updates all cards in play
        UpdateAllCardsInPlay();

        turnCount = 1; //resets turn count variable

        if (Random.Range(0, 2) == 0 || randomEncounter.playerGoesFirst == true) //flips coin
        {
            StartPlayerTurn(); //starts player turn (player goes first)
            playerGoesFirst = true;
            enemyManager.manaPerTurn += 1; //increases enemy mana by 1
        }
        else
        {
            StartEnemyTurn(); //starts enemy turn (enemy goes first)
            playerGoesFirst = false;
            playerManager.manaPerTurn += 1; //increases player mana by 1
        }
    }

    // --- TURN MANAGEMENT ---

    public void StartPlayerTurn()
    {
        //TODO: Trigger start of turn effects for the player

        boardInfo.UpdateBoardStateInfo(); //updates board state information. used for enemy AI

        state = CombatState.PLAYER_TURN;
        playerManager.StartTurn();
        playerMinionZone.EnableMinionAttacks(); //enables minions in the combat zone to attack
        foreach (BaseHero hero in playerHeroes)
            hero.canAttack = true;

        StartOfTurnTrigger(Team.PLAYER); //calls on start of turn effects for the player

    }

    public void EndPlayerTurn()
    {
        if (state == CombatState.PLAYER_TURN) //if it is the player turn
        {
            state = CombatState.PLAYER_END;

            if (playerGoesFirst == false) //if the player went second
                turnCount++;

            playerManager.EndTurn();

            EndOfTurnTrigger(Team.PLAYER); //calls on end of turn effects for the player

            StartEnemyTurn();
        }
    }

    public void StartEnemyTurn()
    {
        //TODO: Trigger start of turn effects for the enemy

        state = CombatState.ENEMY_TURN;

        enemyManager.StartTurn();
        enemyMinionZone.EnableMinionAttacks(); //enables minions in the combat zone to attack
        foreach (BaseHero hero in enemyHeroes)
            hero.canAttack = true;

        enemyAI.StartTurn(); //tells ai to start its turn

        StartOfTurnTrigger(Team.ENEMY); //calls on start of turn effects for the enemy
    }

    public void EndEnemyTurn()
    {
        state = CombatState.ENEMY_END;

        if(playerGoesFirst == true) //if the player went first
            turnCount++;

        enemyManager.EndTurn();

        EndOfTurnTrigger(Team.ENEMY); //calls on end of turn effects for the enemy

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

    // --- TEAM WIN/LOSE CHECK ---

    public void CheckTeamLost()
    {
        if(CheckPlayerAlive() == false)
        //TODO: Player lost stuff (reload main menu scene)
        {
            //TODO: Load death scene or main menu
        }
        else if (CheckEnemyAlive() == false)
        //TODO: Player won
        {
            //copies health from temperary combat heroes to perminant playerHeroManager heroes
            playerHeroManager.CopyCombatHealthToHeroes();

            //Resets combat
            ResetCombat();

            //Swaps back to the game UI (exits combat)
            combatSwap.SwapCombatToGame();
        }
    }

    public bool CheckPlayerAlive()
    {
        bool playerHeroAlive = false;
        foreach (BaseHero hero in playerHeroes) //loops through all player heroes
            if (hero.isDead == false) //a hero is alive
                playerHeroAlive = true;

        return playerHeroAlive;
    }

    public bool CheckEnemyAlive()
    {
        bool enemyHeroAlive = false;
        foreach (BaseHero hero in enemyHeroes) //loops through all enemy heroes
            if (hero.isDead == false) //a hero is alive
                enemyHeroAlive = true;

        return enemyHeroAlive;
    }

    // --- RESETING COMBAT ---

    /// <summary>
    /// Resets combat (clears minions  and hand card)
    /// </summary>
    public void ResetCombat()
    {
        state = CombatState.OFF; //combat manager is off

        //Resets card animation manager
        cardAnimationManager.ResetAnimationManager();

        //discard player hand
        playerManager.handManager.DiscardHand();
        //reshuffles player deck
        playerManager.neutralDeck.CombineDeck();
        //removes all player minions
        foreach (BaseCard card in playerMinions)
            Destroy(card.gameObject);

        //discards enemy hand
        enemyManager.handManager.DiscardHand();
        //removes all enemy minions
        foreach (BaseCard card in enemyMinions)
            Destroy(card.gameObject);

        //Updates all cards in play (resets a lot of card lists)
        UpdateAllCardsInPlay();
    }

    // --- TRIGGERING EFFECTS ---

    /// <summary>
    /// This is called whenever an action is taken
    /// </summary>
    public void ActionTakenTrigger()
    {
        List<BaseCard> cardsToTrigger = GetAllCards(); //gets all hand cards, minions, and heroes in play

        //TODO: This returns a null reference some times. figure out why
        foreach (BaseCard card in cardsToTrigger)
        {
            if (card == null) //card is null
                continue; //go to next

            //tries to get references hero, minion, or spell card
            BaseSpell spell = card.GetComponent<BaseSpell>();
            BaseMinion minion = card.GetComponent<BaseMinion>();
            BaseHero hero = card.GetComponent<BaseHero>();

            if (spell != null) //if card is a spell
            {
                foreach (BaseEffect effect in spell.actionTakenInHand) //loops through all action taken effects on the spell
                    effect.TriggerEffect(); //calls effect to trigger
            }
            else if (minion != null) //if card is a minion
            {
                foreach (BaseEffect effect in minion.actionTakenInHand) //loops through all action taken effects on the minion
                    effect.TriggerEffect(); //calls effect to trigger
            }
            else if (hero != null && hero.isDead == false) //if card is a hero and hero is not dead
            {
                foreach (BaseEffect effect in hero.actionTakenInHand) //loops through all action taken effects on the hero
                    effect.TriggerEffect(); //calls effect to trigger
            }
        }

        if (IsInvoking("UpdateSpellDamage") == true)
            CancelInvoke("UpdateSpellDamage");
        
        Invoke("UpdateSpellDamage",0.25f);
        
    }

    public void StartOfTurnTrigger(Team team)
    {
        List<BaseCard> cardsToTrigger = GetTargets(team, TargetingInfo.SAME);

        foreach (BaseCard card in cardsToTrigger)
        {
            //tries to get references hero or minion
            BaseMinion minion = card.GetComponent<BaseMinion>();
            BaseHero hero = card.GetComponent<BaseHero>();

            if (minion != null) //if card is a minion
            {
                foreach (BaseEffect effect in minion.startOfTurn) //loops through all start of turn effects on the minion
                    effect.TriggerEffect(); //calls effect to trigger
            }
            else if (hero != null && hero.isDead == false) //if card is a hero and hero is not dead
            {
                foreach (BaseEffect effect in hero.startOfTurn) //loops through all start of turn effects on the hero
                    effect.TriggerEffect(); //calls effect to trigger
            }
        }
    }
    public void EndOfTurnTrigger(Team team)
    {
        List<BaseCard> cardsToTrigger = GetTargets(team, TargetingInfo.SAME);

        foreach (BaseCard card in cardsToTrigger)
        {
            //tries to get references hero or minion
            BaseMinion minion = card.GetComponent<BaseMinion>();
            BaseHero hero = card.GetComponent<BaseHero>();

            if (minion != null) //if card is a minion
            {
                foreach (BaseEffect effect in minion.endOfTurn) //loops through all end of turn effects on the minion
                    effect.TriggerEffect(); //calls effect to trigger
            }
            else if (hero != null && hero.isDead == false) //if card is a hero and hero is not dead
            {
                foreach (BaseEffect effect in hero.endOfTurn) //loops through all end of turn effects on the hero
                    effect.TriggerEffect(); //calls effect to trigger
            }
        }
    }

    // --- GETTING TEAM MINION ZONE ---

    public PlayerMinionZone GetMinionZoneByTeam( Team team)
    {
        if(team == Team.PLAYER)
        {
            return playerMinionZone;
        }
        else if (team == Team.ENEMY)
        {
            return enemyMinionZone;
        }

        Debug.Log("MICHAEL WARN: Tried to GetZoneByTeam() without setting team to PLAYEr or ENEMY");
        return null;
    }

    // --- UPDATING SPELL DAMAGE

    public void UpdateSpellDamage()
    {
        List<BaseCard> cards = GetAllInPlay(); //gets all minions and heroes in play

        //Resets spell damage
        playerManager.spellDamage = 0;
        enemyManager.spellDamage = 0;

        foreach(BaseCard card in cards)
        {
            if (card == null) //no card reference
                continue; //skip to next
            BaseMinion minion = card.GetComponent<BaseMinion>(); //tries to get minion reference
            BaseHero hero = card.GetComponent<BaseHero>(); //tries to get hero reference

            if(minion != null && minion.health > 0 && minion.spellDamage > 0)
            {

                if (minion.team == Team.PLAYER) //on the player team
                    playerManager.spellDamage += minion.spellDamage; //increase player spell damage by spell damage on card
                else if (minion.team == Team.ENEMY) //on the enemy team
                    enemyManager.spellDamage += minion.spellDamage; //increase enemy spell damage by spell damage on card

            }
            else if(hero != null && hero.isDead == false && hero.spellDamage > 0)
            {
                Debug.Log(hero.name + " " + hero.spellDamage);

                if (hero.team == Team.PLAYER) //on the player team
                    playerManager.spellDamage += hero.spellDamage; //increase player spell damage by spell damage on card
                else if (hero.team == Team.ENEMY) //on the enemy team
                    enemyManager.spellDamage += hero.spellDamage; //increase enemy spell damage by spell damage on card
            }
        }
    }

    // --- CHECKING / GETTING CARDS IN PLAY ---

    public void UpdateAllCardsInPlay()
    {
        //Updates minion zones
        enemyMinionZone.RefreshMinionsInZoneList();
        playerMinionZone.RefreshMinionsInZoneList();

        allCardsInPlay.Clear(); //clears all cards in play list

            //Gets all player heroes
        foreach (BaseCard card in playerHeroes)
            allCardsInPlay.Add(card);
        //Gets all player minions
        foreach (BaseCard card in playerMinions)
            allCardsInPlay.Add(card);
        //Gets all enemy heroes
        foreach (BaseCard card in enemyHeroes)
            allCardsInPlay.Add(card);
        //Gets all enemy minions
        foreach (BaseCard card in enemyMinions)
            allCardsInPlay.Add(card);
    }

    /// <summary>
    /// Returns all minions and heroes in play. Also gets all cards in each hand
    /// </summary>
    /// <returns></returns>
    public List<BaseCard> GetAllCards()
    {
        List<BaseCard> cards = GetAllInPlay(); //gets all minions and heroes in play

        foreach (BaseCard card in playerHandManager.handCards) //gets all cards in player hand
            if(card != null)
                cards.Add(card);
        foreach (BaseCard card in enemyHandManager.handCards) //gets all cards in enemy hand
            if (card != null)
                cards.Add(card);

        return cards;
    }
    /// <summary>
    /// Returns all minions and heroes in play
    /// </summary>
    /// <returns></returns>
    public List<BaseCard> GetAllInPlay()
    {
        return allCardsInPlay;
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

    /// <summary>
    /// Gets all cards to target based on the targetTeam
    /// </summary>
    /// <returns></returns>
    public virtual List<BaseCard> GetTargets(Team team, TargetingInfo targetTeam)
    {
        List<BaseCard> targetList = new List<BaseCard>();

        //getting all cards in play
        if (targetTeam == TargetingInfo.ANY_OR_ALL)
        {
            targetList = GetAllInPlay();
        }
        //Gets all minions in play
        else if (targetTeam == TargetingInfo.ANY_MINION)
        {
            foreach (BaseCard card in playerMinions)
                targetList.Add(card);
            foreach (BaseCard card in enemyMinions)
                targetList.Add(card);
        }
        //Gets all heroes in play
        else if (targetTeam == TargetingInfo.ANY_HERO)
        {
            foreach (BaseCard card in playerHeroes)
                targetList.Add(card);
            foreach (BaseCard card in enemyHeroes)
                targetList.Add(card);
        }
        //This card is on the player team
        else if (team == Team.PLAYER)
        {
            if (targetTeam == TargetingInfo.SAME)
            {
                targetList = GetAllInPlayPlayer();
            }
            else if (targetTeam == TargetingInfo.SAME_HERO)
            {
                foreach (BaseCard card in playerHeroes)
                    targetList.Add(card);
            }
            else if (targetTeam == TargetingInfo.SAME_MINION)
            {
                foreach (BaseCard card in playerMinions)
                    targetList.Add(card);
            }
            else if (targetTeam == TargetingInfo.OPPOSITE)
            {
                targetList = GetAllInPlayEnemy();
            }
            else if (targetTeam == TargetingInfo.OPPOSITE_HERO)
            {
                foreach (BaseCard card in enemyHeroes)
                    targetList.Add(card);
            }
            else if (targetTeam == TargetingInfo.OPPOSITE_MINION)
            {
                foreach (BaseCard card in enemyMinions)
                    targetList.Add(card);
            }
        }
        //This card is on the enemy team
        else if (team == Team.ENEMY)
        {
            if (targetTeam == TargetingInfo.SAME)
            {
                targetList = GetAllInPlayEnemy();
            }
            else if (targetTeam == TargetingInfo.SAME_HERO)
            {
                foreach (BaseCard card in enemyHeroes)
                    targetList.Add(card);
            }
            else if (targetTeam == TargetingInfo.SAME_MINION)
            {
                foreach (BaseCard card in enemyMinions)
                    targetList.Add(card);
            }
            else if (targetTeam == TargetingInfo.OPPOSITE)
            {
                targetList = GetAllInPlayPlayer();
            }
            else if (targetTeam == TargetingInfo.OPPOSITE_HERO)
            {
                foreach (BaseCard card in playerHeroes)
                    targetList.Add(card);
            }
            else if (targetTeam == TargetingInfo.OPPOSITE_MINION)
            {
                foreach (BaseCard card in playerMinions)
                    targetList.Add(card);
            }
        }

        return targetList;
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
