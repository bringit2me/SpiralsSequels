using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine;
using TMPro;

[RequireComponent(typeof(CardVisualManager))]
public class BaseMinion : BaseCard
{
    [Header("Stats")]
    public int attack;
    [HideInInspector] public int baseAttack;
    public int maxHealth;
    [HideInInspector] public int baseMaxHealth;
    public int health;
    public bool canAttack = false;
    [Header("Other")]
    public int spellDamage = 0;
    public bool taunt = false;
    [Header("Triggers")]
    public List<BaseEffect> onPlay; //called when minion is played
    public List<BaseEffect> onDeath; //(Testing) called when minion dies
    public List<BaseEffect> afterAttack; // called after attacking
    public List<BaseEffect> startOfTurn; //called at the start of the player turn
    public List<BaseEffect> endOfTurn; //called at the end of the player turn
    public List<BaseEffect> actionTakenInHand; //called whenever a card is played while this is in the hand
    [Header("AI Minion")]
    public int deathValueBoostAI = 2;
    [Header("Minion Attack Anim")]
    public BaseAnimationClip attackAnimClip;
    protected BaseAnimationClip attackAnimCopy;

    public void Start()
    {
        visualManager = this.GetComponent<CardVisualManager>(); //gets reference to visual manager

        //sets base numbers
        baseAttack = attack;
        baseMaxHealth = maxHealth;

        SetupCardText();
        //If this minion starts out played
        if (isPlayed == true)
        {
            this.GetComponent<Draggable>().enabled = false; //disables draggable (handles dragging from hand)
            this.GetComponent<MinionCombatTarget>().enabled = true; //enables minion combat target
        }

        attackAnimCopy = Instantiate(attackAnimClip); //creates copy
        attackAnimCopy.card = this; //sets anim card reference

        playAnimCopy = Instantiate(playAnimClip); //creates copy
        playAnimCopy.card = this; //sets anim card reference
    }

    // --- CARD SETUP ---

    /// <summary>
    /// Sends card info to visual manager (updates all text)
    /// </summary>
    public override void SetupCardText()
    {
        base.SetupCardText();

        visualManager.UpdateName(name);
        visualManager.UpdateDescription(description);
        visualManager.UpdateMana(manaCost, true);
        visualManager.UpdateAttack(attack, true);
        visualManager.UpdateHealth(health, true);
        //canAttack = false;
    }

    public override void Played(PlayerManager playerManager)
    {
        base.Played(playerManager);

        ReducePlayerMana(); //reduces player mana

        this.GetComponent<Draggable>().enabled = false; //disables draggable (handles dragging from hand)
        this.GetComponent<MinionCombatTarget>().enabled = true; //enables minion combat target
        if(playSendToDiscard == true) //if we want to send the card to the discard pile on play
            AddToDiscardPile(); //adds the minion to the discard pile
        playAnimCopy.targetPos = playerManager.minionZone.GetNextCardPosition();
        anim.PlayAnimation(playAnimCopy);

        TriggerOnPlayEffects(); //calls onPlay effects
        combatManager.AddSpellDamage(this); //updates spell damage
    }

    public override void Created(PlayerManager playerManager)
    {
        //is played to true
        isPlayed = true;
        this.playerManager = playerManager; //sets player manager reference
        this.GetComponent<Draggable>().enabled = false; //disables draggable (handles dragging from hand)
        this.GetComponent<MinionCombatTarget>().enabled = true; //enables minion combat target

        combatManager.AddSpellDamage(this); //updates spell damage

        base.Created(playerManager);
    }

    public virtual void AttackMinion(BaseMinion target)
    {
        target.visualManager.AddStatChangeEntry(0, false, 0, false, target.health - target.CalculateTakeDamage(attack), true, null);
        this.visualManager.AddStatChangeEntry(0, false, 0, false, this.health - this.CalculateTakeDamage(target.attack), true, null);
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
        if(target.GetComponent<BaseMinion>() == true)
            attackAnimCopy.cardVisualsToUpdate.Add(this);
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
        this.transform.SetParent(transform.root);
        methodCall = combatManager.UpdateAllCardsInPlay;
        StartCoroutine(TriggerMethodEndOfFrame(methodCall)); //updates cards in play (1 frame later)
        
        //loops through each death effect and calls them
        foreach (BaseEffect effect in onDeath)
        {
            methodCall = effect.TriggerEffect;
            StartCoroutine(TriggerMethodEndOfFrame(methodCall));
        }

        combatManager.AddSpellDamage(this); //updates spell damage
        Destroy(this.gameObject, 5f);
    }

    // --- CALLING EFFECTS ---

    /// <summary>
    /// Triggers all afterAttack effects
    /// </summary>
    public virtual void TriggerAfterAttackEffects()
    {
        foreach (BaseEffect effect in afterAttack)
        {
            methodCall = effect.TriggerEffect;
            StartCoroutine(TriggerMethodEndOfFrame(methodCall));
        }
    }
    /// <summary>
    /// Triggers all onPlay effects
    /// </summary>
    public virtual void TriggerOnPlayEffects()
    {
        foreach (BaseEffect effect in onPlay)
        {
            methodCall = effect.TriggerEffect;
            StartCoroutine(TriggerMethodEndOfFrame(methodCall));
        }
    }


    //--- AI EVALUATION ---

    public override int CalculateValueAI(BaseEnemyAI ai)
    {
        int value = 0;
        //Adds stats
        value += attack;
        value += health;
        //Adds spell damage
        value += spellDamage;
        //Adds 1 if the minion has taunt
        if (taunt == true)
            value += 1;
        //if the minion can attack this turn
        if (canAttack == true)
            value += 1;

        value -= manaCost; //takes away mana cost
        value += CalculateEffectValues();
        value += valueBoostAI; //adds in value boost

        if (ai.playstyle == EnemyPlaystyle.AGGRESSIVE) //checks if AI is agressive
            value = (int)(value * ValueToPercent(ai.aggroValue));
        else if (ai.playstyle == EnemyPlaystyle.MID_RANGE) //checks if AI is midrange
            value = (int)(value * ValueToPercent(ai.midRangeValue));
        else if (taunt == true && ai.playstyle == EnemyPlaystyle.DEFENSIVE) //checks if minion has taunt and AI is defensive
            value = (int)(value * ValueToPercent(ai.defenseValue));

        return value;
    }

    public override int CalculateEffectValues()
    {
        int value = 0;

        foreach (BaseEffect effect in onPlay)
            value += effect.CalculateEffectValueAI();

        foreach (BaseEffect effect in onDeath)
            value += effect.CalculateEffectValueAI();

        foreach (BaseEffect effect in afterAttack)
            value += effect.CalculateEffectValueAI();

        foreach (BaseEffect effect in startOfTurn)
            value += effect.CalculateEffectValueAI();

        foreach (BaseEffect effect in endOfTurn)
            value += effect.CalculateEffectValueAI();

        return value;
    }

    public virtual int CalculateDeathValue()
    {
        return attack + deathValueBoostAI;
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
        foreach (BaseEffect effect in onPlay)
        {
            if (effect != null)
                //sets up effect with a hero reference, minion reference, and no spell reference
                effect.SetupEffect(hero, this, null, playerManager);
        }
        foreach (BaseEffect effect in onDeath)
        {
            if (effect != null)
                //sets up effect with a hero reference, minion reference, and no spell reference
                effect.SetupEffect(hero, this, null, playerManager);
        }
        foreach (BaseEffect effect in afterAttack)
        {
            if (effect != null)
                //sets up effect with a hero reference, minion reference, and no spell reference
                effect.SetupEffect(hero, this, null, playerManager);
        }
        foreach (BaseEffect effect in startOfTurn)
        {
            if (effect != null)
                //sets up effect with a hero reference, minion reference, and no spell reference
                effect.SetupEffect(hero, this, null, playerManager);
        }
        foreach (BaseEffect effect in endOfTurn)
        {
            if (effect != null)
                //sets up effect with a hero reference, minion reference, and no spell reference
                effect.SetupEffect(hero, this, null, playerManager);
        }
        foreach (BaseEffect effect in actionTakenInHand)
        {
            if (effect != null)
                //sets up effect with a hero reference, minion reference, and no spell reference
                effect.SetupEffect(hero, this, null, playerManager);
        }
    }
}
