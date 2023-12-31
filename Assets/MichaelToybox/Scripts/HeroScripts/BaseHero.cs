using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class BaseHero : BaseCard
{
    [Header("Deck")]
    public DeckManager heroDeck;
    [Header("Stats")]
    public int attack;
    [HideInInspector] public int baseAttack;
    public int maxHealth;
    [HideInInspector] public int baseMaxHealth;
    public int health;
    public bool canAttack = false;
    public bool isDead = false;
    [Header("Other")]
    public int spellDamage = 0;
    public bool taunt = false;
    [Header("Triggers")]
    public List<BaseEffect> onDeath; //(Testing) called when minion dies
    public List<BaseEffect> afterAttack; //called after attacking
    public List<BaseEffect> startOfTurn; //called at the end of the player turn
    public List<BaseEffect> endOfTurn; //called at the end of the player turn
    public List<BaseEffect> actionTakenInHand; //called whenever a card is played while this is in the hand
    [Header("UI References")]
    [SerializeField] TMP_Text nameText;
    [SerializeField] TMP_Text attackText;
    [SerializeField] TMP_Text healthText;
    [SerializeField] Color defaultTextColor = Color.black;
    [SerializeField] Color negativeTextColor = Color.red;
    [SerializeField] Color positiveTextColor = new Color(0, 1f, 0);
    [Header("Hero Power")]
    public BaseHeroPower heroPower;
    [Header("Hero Attack Anim")]
    public BaseAnimationClip attackAnimClip;
    protected BaseAnimationClip attackAnimCopy;
    [Header("AI Info")]
    public int aggroValueBoost = 0;
    public int midRangeValueBoost = 0;
    public int defenseValueBoost = 0;
    public BaseCard[] startingCards;

    private void Start()
    {
        visualManager = this.GetComponent<CardVisualManager>(); //gets reference to visual manager

        //sets base numbers
        baseAttack = attack;
        baseMaxHealth = maxHealth;

        SetupCardText();
        isPlayed = true;
        anim = GameObject.FindObjectOfType<CardAnimationManager>(); //finds animator

        attackAnimCopy = Instantiate(attackAnimClip); //creates copy
        attackAnimCopy.card = this; //sets anim card reference
    }

    public void SetBaseStats()
    {
        //sets base numbers
        baseAttack = attack;
        baseMaxHealth = maxHealth;
    }

    public override void SetupCardText()
    {
        base.SetupCardText();

        visualManager.UpdateName(name);
        visualManager.UpdateDescription(description);
        visualManager.UpdateMana(manaCost, true);
        visualManager.UpdateAttack(attack, true);
        visualManager.UpdateHealth(health, true);
    }

    public virtual void AttackMinion(BaseMinion target)
    {
        target.visualManager.AddStatChangeEntry(0, false, 0, false, target.health - target.CalculateTakeDamage(attack), true, null);
        target.TakeDamage(attack);
        canAttack = false;
        PlayAttackAnim(target);
        combatManager.ActionTakenTrigger(); //calls action taken trigger
        TriggerAfterAttackEffects(); //calls all afterAttack effects
    }
    public virtual void AttackHero(BaseHero target)
    {
        target.visualManager.AddStatChangeEntry(0, false, 0, false, target.health - target.CalculateTakeDamage(attack), true, null);
        target.TakeDamage(attack);
        canAttack = false;
        PlayAttackAnim(target);
        combatManager.ActionTakenTrigger(); //calls action taken trigger
        TriggerAfterAttackEffects(); //calls all afterAttack effects
    }

    public virtual void PlayAttackAnim(BaseCard target)
    {
        attackAnimCopy.target = target.gameObject;
        attackAnimCopy.cardVisualsToUpdate.Add(target);
        anim.PlayAnimation(attackAnimCopy);
    }

    // --- TAKING DAMAGE ---

    public virtual void TakeDamage(int value)
    {
        int calculatedValue = CalculateTakeDamage(value);

        health -= calculatedValue;

        if (health <= 0)
        {
            Dead();
        }
    }

    public virtual int CalculateTakeDamage(int value)
    {
        if (value < 0)
            value = 0;
        return value;
    }

    // --- HEALING ---

    public virtual void Heal(int value)
    {
        int calculatedValue = CalculateHeal(value);

        health += calculatedValue;

        health = Mathf.Clamp(health, 0, maxHealth);
    }

    public virtual int CalculateHeal(int value)
    {
        if (value < 0)
            value = 0;


        if (health + value > maxHealth)
            value = maxHealth - health;

        return value;
    }

    // --- CHANGING STATS ---

    public virtual void ChangeAttack(int value)
    {
        attack += CalculateAttackChange(value);

        if (attack < 0)
            attack = 0;
    }

    public virtual int CalculateAttackChange(int value)
    {
        if (attack + value < 0)
            value = -attack;

        return value;
    }

    public virtual void ChangeHealth(int value)
    {
        health += CalculateHealthChange(value);
        maxHealth += value;

        if (health <= 0)
        {
            Dead();
        }
    }

    public virtual int CalculateHealthChange(int value)
    {
        return value;
    }

    public virtual void ChangeSpellDamage(int value)
    {
        spellDamage += CalculateSpellDamageChange(value);
    }

    public virtual int CalculateSpellDamageChange(int value)
    {
        if (spellDamage + value < 0) //if the spell damage change would make spell damage negative
            value = -spellDamage; //sets  value to negative spell damage (will make spell damage 0)
        return value;
    }

    public virtual void Dead()
    {
        isDead = true;
        GameObject.FindObjectOfType<CombatManager>().CheckTeamLost(); //checks if a team lost on hero death

        //loops through each death effect and calls them
        foreach (BaseEffect effect in onDeath)
        {
            methodCall = effect.TriggerEffect;
            StartCoroutine(TriggerMethodEndOfFrame(methodCall));
        }
    }

    // --- CALLING EFFECTS ---

    /// <summary>
    /// Triggers all afterAttack effects
    /// </summary>
    public virtual void TriggerAfterAttackEffects()
    {
        foreach (BaseEffect effect in afterAttack)
            effect.TriggerEffect();
    }

    //--- AI EVALUATION ---

    /// <summary>
    /// Returns 1 for every 10 missing health
    /// Example: 30 health and 60 max health == 3
    /// Example 2: 35 health and 60 max health == 2
    /// </summary>
    /// <returns></returns>
    public virtual int CalculateMissingHealthValue()
    {
        return Mathf.FloorToInt((maxHealth - health) * 0.1f);
    }

    //--- SETTING UP EFFECTS ---

    /// <summary>
    /// NOT IMPLEMENTED | Adds effect to a specified list on the card
    /// </summary>
    /// <param name="effect"></param>
    public virtual void AddEffect(BaseEffect effect)
    {

    }

    /// <summary>
    /// Sets up all effects on the card with references (only passes in references we can get off of the current card)
    /// </summary>
    public override void SetupAllEffects()
    {
        if (playerManager == null)
            Debug.Log("No Player Manager");

        foreach (BaseEffect effect in onDeath)
        {
            if (effect != null)
                //sets up effect with a hero reference, minion reference, and no spell reference
                effect.SetupEffect(this, null, null, playerManager);
        }
        foreach (BaseEffect effect in afterAttack)
        {
            if (effect != null)
                //sets up effect with a hero reference, minion reference, and no spell reference
                effect.SetupEffect(this, null, null, playerManager);
        }
        foreach (BaseEffect effect in startOfTurn)
        {
            if (effect != null)
                //sets up effect with a hero reference, minion reference, and no spell reference
                effect.SetupEffect(this, null, null, playerManager);
        }
        foreach (BaseEffect effect in endOfTurn)
        {
            if (effect != null)
                //sets up effect with a hero reference, minion reference, and no spell reference
                effect.SetupEffect(this, null, null, playerManager);
        }
        foreach (BaseEffect effect in actionTakenInHand)
        {
            if (effect != null)
                //sets up effect with a hero reference, minion reference, and no spell reference
                effect.SetupEffect(this, null, null, playerManager);
        }
    }
}
