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
    int baseAttack;
    public int maxHealth;
    int baseMaxHealth;
    public int health;
    public bool canAttack = false;
    public bool targetable = true;
    public bool isDead = false;
    [Header("Other")]
    public int spellDamage = 0;
    public bool taunt = false;
    [Header("Triggers")]
    public List<BaseEffect> afterAttack; // (NOT IMPLEMENTED) called after attacking
    public List<BaseEffect> startOfTurn; //(NOT IMPLEMENTED) called at the end of the player turn
    public List<BaseEffect> endOfTurn; //(NOT IMPLEMENTED) called at the end of the player turn
    public List<BaseEffect> actionTakenInHand; //(NOT IMPLEMENTED) called whenever a card is played while this is in the hand
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
    public CardAnimationClip attackAnimClip;
    [Header("AI Info")]
    public int aggroValueBoost = 0;
    public int midRangeValueBoost = 0;
    public int defenseValueBoost = 0;
    public BaseCard[] startingCards;

    private void Start()
    {
        //sets base numbers
        baseAttack = attack;
        baseMaxHealth = maxHealth;

        SetupCardText();
        isPlayed = true;
        anim = GameObject.FindObjectOfType<CardAnimationManager>(); //finds animator
    }
    public virtual void SetupCardText()
    {
        nameText.text = name;
        UpdateAttack();
        UpdateHealth();
    }

    public virtual void UpdateAttack()
    {
        attackText.text = "" + attack;

        if (attack > baseAttack)
            attackText.color = positiveTextColor;
        else
            attackText.color = defaultTextColor;
    }

    public virtual void UpdateHealth()
    {
        healthText.text = "" + health;

        if (health > baseMaxHealth && health == maxHealth) //card health greater than default and at maxHealth
            healthText.color = positiveTextColor;
        else if (health < maxHealth) //card is damaged
            healthText.color = negativeTextColor;
        else //card not damaged
            healthText.color = defaultTextColor;

    }

    public virtual void AttackMinion(BaseMinion target)
    {
        target.TakeDamage(attack);
        canAttack = false;
        PlayAttackAnim(target);
        combatManager.ActionTakenTrigger(); //calls action taken trigger
        TriggerAfterAttackEffects(); //calls all afterAttack effects
    }
    public virtual void AttackHero(BaseHero target)
    {
        target.TakeDamage(attack);
        canAttack = false;
        PlayAttackAnim(target);
        combatManager.ActionTakenTrigger(); //calls action taken trigger
        TriggerAfterAttackEffects(); //calls all afterAttack effects
    }

    public virtual void PlayAttackAnim(BaseCard target)
    {
        attackAnimClip.target = target.gameObject;
        anim.PlayAnimation(attackAnimClip);
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

        UpdateHealth();
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

        UpdateHealth();
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

        UpdateAttack();
    }

    public virtual int CalculateAttackChange(int value)
    {
        if (attack + value < 0)
            value = attack;

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

        UpdateHealth();
    }

    public virtual int CalculateHealthChange(int value)
    {
        return value;
    }

    public virtual void Dead()
    {
        isDead = true;
        GameObject.FindObjectOfType<CombatManager>().CheckTeamLost(); //checks if a team lost on hero death
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
