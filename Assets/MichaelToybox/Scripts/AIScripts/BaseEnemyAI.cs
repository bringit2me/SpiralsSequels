using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class BaseEnemyAI : MonoBehaviour
{
    public Team team;
    [Header("AI Brain Bools")]
    public bool isAITurn;
    public bool detectsLethal;
    public bool detectsWillDieNextTurn;
    public bool thinksItIsLosing;

    [Header("AI Playstyle Values")]
    public EnemyPlaystyle playstyle;
    public int aggroValueBoost = 0;
    public float aggroValue;
    public int midRangeValueBoost = 0;
    public float midRangeValue;
    public int defenseValueBoost = 0;
    public float defenseValue;
    [Header("Hand Card Values")]
    [SerializeField] List<CardValueEntry> handCardValues = new List<CardValueEntry>();
    [SerializeField] CardValueEntry highestValueCard;
    [Header("Character Attack Values")]
    [SerializeField] List<CardValueEntry> characterAttackValues = new List<CardValueEntry>();
    [SerializeField] CardValueEntry highestAttackValue;

    //References
    protected CombatManager combatManager;
    protected BoardStateInformation boardInfo;
    //References (enemy encounter prefab)
    protected EnemyManager enemyManager;
    protected EnemyMinionZone minionZone;
    protected HandManager hand;

    public virtual void Start()
    {
        //gets references
        combatManager = GameObject.FindObjectOfType<CombatManager>();
        boardInfo = GameObject.FindObjectOfType<BoardStateInformation>();
        //gets references to scripts in enemy encounter prefab
        enemyManager = this.GetComponent<EnemyManager>();
        minionZone = this.GetComponentInChildren<EnemyMinionZone>();
        hand = this.GetComponent<HandManager>();
        //Sets team
        team = enemyManager.team;
    }

    public virtual void Update()
    {

    }

    //--- TURN MANAGEMENT ---

    public virtual void StartTurn()
    {
        isAITurn = true;

        //DeterminePlaystyle(); //Determines its playstyle based on the current board state
        //CalculateHandCardValues(); //calculates the value of each card in its hand. the highest value card will be played

        StartCoroutine(StartTurnDelay());
    }

    public virtual IEnumerator PlayCards()
    {
        while(true)
        {
            if (highestValueCard.value < 0) //no good card to play
                break; //exit loop

            //plays cards
            if (highestValueCard.isMinion == true) //if our highest value card is a minion
            {
                minionZone.PlayMinionToZone(highestValueCard.card);
                yield return new WaitForSeconds(0.5f);
            }
            else //highest value card is not a minion
            {
                if(highestValueCard.target != null) //if the card requires a target
                {
                    highestValueCard.card.GetComponent<BaseTargetSpell>().target = highestValueCard.target;
                    highestValueCard.card.Played(enemyManager);
                }
                else //card does not require a target
                {
                    highestValueCard.card.Played(enemyManager); //plays the spell
                }
                yield return new WaitForSeconds(0.5f);
            }

            yield return new WaitForEndOfFrame();

            //reevaluates its card values and playstyle after making a play
            DeterminePlaystyle(); //Determines its playstyle based on the current board state
            CalculateHandCardValues(); //calculates the value of each card in its hand. the highest value card will be played
            Debug.Log("Recalculated Playstyle and Hand Values");

            if (highestValueCard.value < 0) //no good card to play
                break; //exit loop

            yield return new WaitForEndOfFrame();
        }

        StartCoroutine(AttackWithCharacters());
    }

    public virtual IEnumerator AttackWithCharacters()
    {
        //reevaluates its card values and playstyle after making a play
        DeterminePlaystyle(); //Determines its playstyle based on the current board state
        CalculateCharacterAttackValues(); //calculates the value of each card in its hand. the highest value card will be played
        Debug.Log("Recalculated Playstyle and Character Attack Values");

        while (true)
        {
            if (highestAttackValue.value < 0) //no good card to play
                break; //exit loop

            //plays cards
            if (highestAttackValue.card.GetComponent<BaseMinion>() == true) //if our highest value card is a minion
            {
                //attacks the target
                highestAttackValue.card.GetComponent<BaseMinion>().GetComponent<MinionCombatTarget>().AttackTarget(highestAttackValue.target.gameObject);
                yield return new WaitForSeconds(0.5f);
            }
            else if (highestAttackValue.card.GetComponent<BaseHero>() == true) //if our highest value card is a hero
            {
                //attacks the target
                highestAttackValue.card.GetComponent<BaseHero>().GetComponent<HeroCombatTarget>().AttackTarget(highestAttackValue.target.gameObject);
                yield return new WaitForSeconds(0.5f);
            }
            else
            {
                Debug.LogWarning("Tried to attack with a card that is not a minion or hero");
            }

            yield return new WaitForEndOfFrame();

            //reevaluates its card values and playstyle after making a play
            DeterminePlaystyle(); //Determines its playstyle based on the current board state
            CalculateCharacterAttackValues(); //calculates the value of each card in its hand. the highest value card will be played
            Debug.Log("Recalculated Playstyle and Character Attack Values");

            if (highestAttackValue.value < 0) //no good card to play
                break; //exit loop

            yield return new WaitForEndOfFrame();
        }

        EndTurn();
    }

    public virtual void EndTurn()
    {
        isAITurn = false;

        combatManager.EndEnemyTurn();
    }

    IEnumerator StartTurnDelay()
    {
        yield return new WaitForSeconds(0.125f * (enemyManager.drawCountNeutral + enemyManager.drawCountHero));
        DeterminePlaystyle(); //Determines its playstyle based on the current board state
        CalculateHandCardValues(); //calculates the value of each card in its hand. the highest value card will be played
        yield return new WaitForSeconds(1f);
        StartCoroutine(PlayCards());
    }

    // --- PLAYING CARDS ---

    // --- EVALUATING PLAYSTYLE ---
    // Evaluation Equation   y = (100x) / (50 + x) + boost
    // All playstyles are a value between 0 and 100

    public virtual void DeterminePlaystyle()
    {
        //Updates board info
        boardInfo.UpdateBoardStateInfo();
        //Determines each playstyle value
        DetermineAggroValue();
        DetermineMidRangeValue();
        DetermineDefenseValue();

        //Checks which playstyle value is highest
        if (aggroValue > midRangeValue && aggroValue > defenseValue) //if aggro is the highest value
            playstyle = EnemyPlaystyle.AGGRESSIVE;
        else if (midRangeValue >= aggroValue && midRangeValue >= defenseValue) //if mid range is the highest value
            playstyle = EnemyPlaystyle.MID_RANGE;
        else if(defenseValue > aggroValue && defenseValue > midRangeValue) //if defense if the highest value
            playstyle = EnemyPlaystyle.DEFENSIVE;
    }

    public virtual void DetermineAggroValue()
    {
        int rawValue = 0;

        //subtracts enemies total attack from players total attack
        rawValue += boardInfo.enemyTotalAttack - boardInfo.playerTotalAttack;

        aggroValue = (100f * rawValue) / (100f + rawValue); //calculates aggro value
        aggroValue += aggroValueBoost; // adds in aggro boost
        aggroValue = Mathf.Clamp(aggroValue, 0, 100); //clamps it between 0 and 100
    }

    public virtual void DetermineMidRangeValue()
    {
        int rawValue = 0;
        //subtracts enemy total minion attack from players total attack (minion + hero atk)
        rawValue += boardInfo.enemyTotalMinionAttack - boardInfo.playerTotalAttack;
        //subtracts enemy total minion health from player total minion health
        rawValue += boardInfo.enemyTotalMinionHealth - boardInfo.playerTotalMinionHealth;

        midRangeValue = (100f * rawValue) / (100f + rawValue); //calculates mid range value
        midRangeValue += midRangeValueBoost; // adds in mid range boost
        midRangeValue = Mathf.Clamp(midRangeValue, 0, 100); //clamps it between 0 and 100
    }

    public virtual void DetermineDefenseValue()
    {
        int rawValue = 0;
        //subtracts players minion count from enemy minion count (checks if player has more minions)
        rawValue += boardInfo.playerMinionCount - boardInfo.enemyMinionCount; //maybe add a multiplier to this
        //subtracts the max health of all enemy heroes with the current health of all enemy heroes
        rawValue += (int)((boardInfo.enemyTotalHeroMaxHealth - boardInfo.enemyTotalHeroHealth) * 0.25f); //fiddle with multiplier on this

        //TODO: is dead calculation (if dead, set defense value to 101)

        defenseValue = (100f * rawValue) / (100f + rawValue); //calculates defense value
        defenseValue += defenseValueBoost; // adds in defense boost
        defenseValue = Mathf.Clamp(defenseValue, 0, 100); //clamps it between 0 and 100
    }

    // --- CALCULATING CARD VALUE ---

    public virtual void CalculateHandCardValues()
    {
        handCardValues.Clear();
        highestValueCard = new CardValueEntry();

        foreach (BaseCard card in hand.handCards)
        {
            CardValueEntry entry = new CardValueEntry(); //creates CardValueEntry
            entry.card = card; //sets entry card reference
            handCardValues.Add(entry); //adds reference to hand card values

            if (card.manaCost <= enemyManager.mana) //if we have enough mana to play the card
            {
                if (card.GetComponent<BaseMinion>() == true && minionZone.CheckZoneFull() == true)
                {
                    BaseMinion minion = card.GetComponent<BaseMinion>();
                    entry.value = minion.CalculateValueAI(this);
                    entry.isMinion = true;
                }
                else if (card.GetComponent<BaseTargetSpell>() == true)
                {
                    BaseTargetSpell targetSpell = card.GetComponent<BaseTargetSpell>();
                    CardValueEntry tempEntry = targetSpell.CalculateValueAI(this);
                    entry.value = tempEntry.value;
                    entry.target = tempEntry.target;
                }
                else if (card.GetComponent<BaseAOESpell>() == true)
                {
                    BaseAOESpell aoeSpell = card.GetComponent<BaseAOESpell>();
                    entry.value = aoeSpell.CalculateValueAI(this);
                }
                //this is used for any future card types
                //else if (card.GetComponent<NAME>() == true)
                //{
                //    NAME nameSpell = card.GetComponent<NAME>();
                //}

                //checks if it is a higher value play than a previous card
                if(entry.value > highestValueCard.value)
                {
                    highestValueCard = entry;
                }
            }
        }
    }

    public virtual void CalculateCharacterAttackValues()
    {
        characterAttackValues.Clear();
        highestAttackValue = new CardValueEntry();

        //Loops through all enemy minions
        foreach (BaseMinion minion in combatManager.enemyMinions)
        {
            CardValueEntry entry = CalculateMinionsBestAttack(minion); //creates CardValueEntry and calculates minions best attack
            characterAttackValues.Add(entry); //adds reference to hand card values

            if (entry.value > highestAttackValue.value) //new higher attack value than the current highest attack value
            {
                highestAttackValue = entry;
            }
        }
        //Loops through all enemy heroes
        foreach (BaseHero hero in combatManager.enemyHeroes)
        {
            CardValueEntry entry = CalculateHeroBestAttack(hero); //creates CardValueEntry
            characterAttackValues.Add(entry); //adds reference to hand card values

            if (entry.value > highestAttackValue.value) //new higher attack value than the current highest attack value
            {
                highestAttackValue = entry;
            }
        }
    }

    public virtual CardValueEntry CalculateMinionsBestAttack(BaseMinion minion)
    {
        CardValueEntry entry = new CardValueEntry();
        entry.card = minion;

        if (minion.canAttack == false) //if the minion cannot attack
            return entry;

        //loops through all cards in play
        foreach(BaseCard card in combatManager.allCardsInPlay)
        {
            if(card.team != team) //card is on another team
            {
                int value = 0;
                bool survivesMinionCombat = false;
                //Gets minion reference. if card is not a minion it will be null
                BaseMinion targetMinion = card.GetComponent<BaseMinion>();
                //Gets hero reference. if card is not a hero it will be null
                BaseHero targetHero = card.GetComponent<BaseHero>();

                if (targetMinion != null) //card is a minion
                {
                    //changes value
                    value += targetMinion.CalculateTakeDamage(minion.attack);
                    //if the minion kills the target
                    if (targetMinion.CalculateTakeDamage(minion.attack) == targetMinion.health)
                    {
                        value += targetMinion.CalculateDeathValue(); //adds in a death modifier for when the card dies
                        value += targetMinion.attack; //adds in the attack of the killed card
                    }
                    //minion over kills the target
                    else if (targetMinion.CalculateTakeDamage(minion.attack) >= targetMinion.health)
                    {
                        value = targetMinion.maxHealth - targetMinion.health + targetMinion.CalculateDeathValue();
                        value += targetMinion.attack;
                    }

                    //--- calculates returning damage ---

                    //if the returning damage kills the target
                    if (minion.CalculateTakeDamage(targetMinion.attack) >= minion.health)
                    {
                        value -= minion.health; //lowers value by health
                        value -= minion.CalculateDeathValue(); //lowers value by death value
                    }
                    else //returning damage does not kill the minion
                    {
                        //if the ai is midrange
                        if (playstyle == EnemyPlaystyle.MID_RANGE)
                        {
                            survivesMinionCombat = true;
                        }
                        else //ai not midrange
                        {
                            value -= minion.CalculateTakeDamage(targetMinion.attack);
                        }
                    }
                }
                else if (targetHero != null) //card is a hero
                {
                    //changes value
                    value += targetHero.CalculateTakeDamage(minion.attack);
                    //if the minion kills the target
                    if (targetHero.CalculateTakeDamage(minion.attack) >= targetHero.health)
                    {
                        value += (targetHero.attack + 1) * 100; //multiplies hero attack by 100 (will hyper prioritize dead heroes)
                    }

                }

                //AI is agressive and hits a hero
                if(playstyle == EnemyPlaystyle.AGGRESSIVE && targetHero != null)
                {
                    value = (int)(value * ValueToPercent(aggroValue));
                }
                //AI is mid range, hits a minion, and minion survives the attack
                else if (playstyle == EnemyPlaystyle.MID_RANGE && targetMinion != null && survivesMinionCombat == true)
                {
                    value = (int)(value * ValueToPercent(midRangeValue));
                }
                //AI is defensive and hits a minion
                else if (playstyle == EnemyPlaystyle.DEFENSIVE && targetMinion != null)
                {
                    value = (int)(value * ValueToPercent(defenseValue));
                }


                if(value > entry.value)
                {
                    entry.value = value;
                    entry.target = card;
                }
            }
        }

        return entry;
    }

    public virtual CardValueEntry CalculateHeroBestAttack(BaseHero hero)
    {
        CardValueEntry entry = new CardValueEntry();
        entry.card = hero;

        if (hero.canAttack == false) //if the hero cannot attack
            return entry;

        //loops through all cards in play
        foreach (BaseCard card in combatManager.allCardsInPlay)
        {
            if (card.team != team) //card is on another team
            {
                int value = 0;
                //Gets minion reference. if card is not a minion it will be null
                BaseMinion targetMinion = card.GetComponent<BaseMinion>();
                //Gets hero reference. if card is not a hero it will be null
                BaseHero targetHero = card.GetComponent<BaseHero>();

                if (targetMinion != null) //card is a minion
                {
                    //changes value
                    value += targetMinion.CalculateTakeDamage(hero.attack);
                    //if the hero kills the target
                    if (targetMinion.CalculateTakeDamage(hero.attack) == targetMinion.health)
                    {
                        value += targetMinion.CalculateDeathValue(); //adds in a death modifier for when the card dies
                        value += targetMinion.attack; //adds in the attack of the killed card
                    }
                    //hero over kills the target
                    else if (targetMinion.CalculateTakeDamage(hero.attack) >= targetMinion.health)
                    {
                        value = targetMinion.maxHealth - targetMinion.health + targetMinion.CalculateDeathValue();
                        value += targetMinion.attack;
                    }

                    //--- Heroes do not take or deal returning damage ---
                }
                else if (targetHero != null) //card is a hero
                {
                    //changes value
                    value += targetHero.CalculateTakeDamage(hero.attack);
                    //if the hero kills the target hero
                    if (targetHero.CalculateTakeDamage(hero.attack) >= targetHero.health)
                    {
                        value += (targetHero.attack + 1) * 100; //multiplies hero attack by 100 (will hyper prioritize dead heroes)
                    }

                }

                //AI is agressive and hits a hero
                if (playstyle == EnemyPlaystyle.AGGRESSIVE && targetHero != null)
                {
                    value = (int)(value * ValueToPercent(aggroValue));
                }
                //AI is mid range and hits a minion
                else if (playstyle == EnemyPlaystyle.MID_RANGE && targetMinion != null)
                {
                    value = (int)(value * ValueToPercent(midRangeValue));
                }
                //AI is defensive and hits a minion
                else if (playstyle == EnemyPlaystyle.DEFENSIVE && targetMinion != null)
                {
                    value = (int)(value * ValueToPercent(defenseValue));
                }


                if (value > entry.value)
                {
                    entry.value = value;
                    entry.target = card;
                }
            }
        }

        return entry;
    }

    public float ValueToPercent(float value)
    {
        return 1 + (value / 100);
    }
}

[System.Serializable]
public class CardValueEntry
{
    public BaseCard card;
    /// <summary>
    /// The calculated value of the card. The AI will play the highest value card in its hand
    /// </summary>
    public float value;
    public BaseCard target;
    public bool isMinion = false;

    /// <summary>
    /// Base Constructor for CardValueEntry. card = null. value = -1. target = null. isMinion = false
    /// value should be set. AI will not play cards with a negative value
    /// </summary>
    public CardValueEntry()
    {
        card = null;
        value = -1;
        target = null;
        isMinion = false;
    }
}

public enum EnemyPlaystyle
{
    AGGRESSIVE,
    MID_RANGE,
    DEFENSIVE
}
