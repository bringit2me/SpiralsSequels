using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine;
using TMPro;

public class BaseMinion : BaseCard
{
    [Header("Stats")]
    public int attack;
    int baseAttack;
    public int maxHealth;
    int baseMaxHealth;
    public int health;
    public bool canAttack = false;
    public bool targetable = true;
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
    [Header("UI References")]
    [SerializeField] TMP_Text nameText;
    [SerializeField] TMP_Text descriptionText;
    [SerializeField] TMP_Text manaText;
    [SerializeField] TMP_Text attackText;
    [SerializeField] TMP_Text healthText;
    [SerializeField] Color defaultTextColor = Color.black;
    [SerializeField] Color negativeTextColor = Color.red;
    [SerializeField] Color positiveTextColor = new Color(0,1f,0);
    [Header("AI Minion")]
    public int deathValueBoostAI = 2;
    [Header("Minion Attack Anim")]
    public BaseAnimationClip attackAnimClip;
    protected BaseAnimationClip attackAnimCopy;

    public void Start()
    {
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
    }

    // --- CARD SETUP ---

    public virtual void SetupCardText()
    {
        nameText.text = name;
        descriptionText.text = description;
        UpdateMana();
        UpdateAttack();
        UpdateHealth();
        //canAttack = false;
    }

    public virtual void UpdateMana()
    {
        manaText.text = "" + manaCost;
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
        
        if(health > baseMaxHealth && health == maxHealth) //card health greater than default and at maxHealth
            healthText.color = positiveTextColor;
        else if (health < maxHealth) //card is damaged
            healthText.color = negativeTextColor;
        else //card not damaged
            healthText.color = defaultTextColor;
        
    }

    public override void Played(PlayerManager playerManager)
    {
        base.Played(playerManager);

        ReducePlayerMana(); //reduces player mana

        this.GetComponent<Draggable>().enabled = false; //disables draggable (handles dragging from hand)
        this.GetComponent<MinionCombatTarget>().enabled = true; //enables minion combat target
        deck.discardPile.Add(selfCardRef); //adds the minion to the discard pile
        playAnimClip.targetPos = playerManager.minionZone.GetNextCardPosition();
        anim.PlayAnimation(playAnimCopy);

        TriggerOnPlayEffects(); //calls onPlay effects
        combatManager.UpdateSpellDamage(this); //updates spell damage
    }

    public override void Created(PlayerManager playerManager)
    {
        //is played to true
        isPlayed = true;
        this.playerManager = playerManager; //sets player manager reference
        this.GetComponent<Draggable>().enabled = false; //disables draggable (handles dragging from hand)
        this.GetComponent<MinionCombatTarget>().enabled = true; //enables minion combat target

        combatManager.UpdateSpellDamage(this); //updates spell damage

        base.Created(playerManager);
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
        attackAnimCopy.target = target.gameObject;
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
        this.transform.SetParent(transform.root);
        methodCall = combatManager.UpdateAllCardsInPlay;
        StartCoroutine(TriggerMethodEndOfFrame(methodCall)); //updates cards in play (1 frame later)
        foreach (BaseEffect effect in onDeath)
        {
            methodCall = effect.TriggerEffect;
            StartCoroutine(TriggerMethodEndOfFrame(methodCall));
        }

        combatManager.UpdateSpellDamage(this); //updates spell damage
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

        foreach (BaseEffect effect in actionTakenInHand)
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
