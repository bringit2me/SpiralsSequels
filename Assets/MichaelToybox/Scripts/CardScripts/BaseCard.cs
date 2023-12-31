using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class BaseCard : MonoBehaviour
{
    public new string name;
    [TextArea(5, 15)]
    public string description;
    public Rarity rarity = Rarity.NONE;
    public Team team;
    public BaseCard selfCardRef;
    public bool targetable = true;
    [HideInInspector] public PlayerManager playerManager;
    [HideInInspector] public DeckManager deck;
    [HideInInspector] public PlayerMinionZone zone;
    [HideInInspector] public BaseHero hero;
    [HideInInspector] public CardAnimationManager anim;
    protected CombatManager combatManager;
    public bool playSendToDiscard = true;
    [Header("AI")]
    public int valueBoostAI = 0;
    [Header("Animation")]
    public BaseAnimationClip playAnimClip;
    protected BaseAnimationClip playAnimCopy;
    [Header("Custom Variables")]
    [SerializeField] protected bool useVariable = false;
    [SerializeField] protected bool addToVariable = false;
    [SerializeField] protected bool subtractFromVariable = false;
    [SerializeField] protected bool useOnlyOne = false;
    [Space]
    [SerializeField] protected string variableName;
    [Space]
    [SerializeField] protected int variableIncrease;
    [Space]
    [SerializeField] protected int variableDecrease;
    [Space]
    [SerializeField] protected int amountPerVariable = 0;
    [Header("Stats")]
    public int manaCost;
    [HideInInspector] public int baseManaCost;
    public bool isPlayed = false;

    //references
    protected PointerEventData eventData;
    protected delegate void MethodCall();
    protected MethodCall methodCall;
    public CardVisualManager visualManager;
    public virtual void Awake()
    {
        anim = GameObject.FindObjectOfType<CardAnimationManager>(); //gets reference to animation manager
        combatManager = GameObject.FindObjectOfType<CombatManager>(); //gets reference to combat manager
        visualManager = this.GetComponent<CardVisualManager>(); //gets reference to visual manager

        playAnimCopy = Instantiate(playAnimClip); //creates copy
        playAnimCopy.card = this; //sets anim card reference

        baseManaCost = manaCost;
    }


    /// <summary>
    /// Adds the card to the discard pile
    /// </summary>
    public virtual void AddToDiscardPile()
    {
        deck.discardPile.Add(selfCardRef);

        if(deck.GetComponent<SideboardDeckManager>() == true) //if the deck is the side board deck
        {
            deck.CombineDeck(); //combines discard and draw piles;
        }
    }

    /// <summary>
    /// Called when the card is played. sets isPlayed to true
    /// </summary>
    /// <param name="playerManager"></param>
    public virtual void Played(PlayerManager playerManager)
    {
        //sets is played to true
        isPlayed = true;
        this.playerManager = playerManager;
        playerManager.handManager.RemoveCardFromHand(this);
        SetupAllEffects(); //sets up all effects
        combatManager.ActionTakenTrigger(); //calls action taken trigger
    }

    public virtual void Created(PlayerManager playerManager)
    {
        //sets is played to true
        isPlayed = true;
        this.playerManager = playerManager; //sets player manager reference
        team = this.playerManager.team; //sets team
        SetupAllEffects(); //sets up all effects
        combatManager.ActionTakenTrigger(); //calls action taken trigger
    }

    /// <summary>
    /// Takes mana from the playerManager's mana and updates the mana text
    /// </summary>
    public virtual void ReducePlayerMana()
    {
        //lowers players mana by the cost
        playerManager.mana -= manaCost;
        //Updates mana text
        playerManager.UpdateManaText();
    }

    public virtual int CalculateValueAI(BaseEnemyAI ai)
    {
        return -1;
    }

    public virtual int CalculateEffectValues()
    {
        return 0;
    }

    public float ValueToPercent(float value)
    {
        return 1 + (value / 100);
    }

    public virtual void SetupAllEffects()
    {

    }

    protected virtual IEnumerator TriggerMethodEndOfFrame(MethodCall method)
    {
        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();
        method();

    }

    public virtual void ChangeManaCost(int value)
    {
        manaCost += CalculateManaCostChange(value);
    }
    public virtual int CalculateManaCostChange(int value)
    {
        if (manaCost + value < 0)
            value = -manaCost;

        return value;
    }

    public virtual void SetupCardText()
    {
        visualManager.GetUIReferences();
    }

}
