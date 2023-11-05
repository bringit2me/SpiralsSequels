using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine;
using TMPro;

public class BaseMinion : BaseCard
{
    [Header("Stats")]
    public int attack;
    public int maxHealth;
    public int health;
    public bool canAttack = false;
    public bool targetable = true;
    [Header("Other")]
    public int spellDamage = 0;
    public bool taunt = false;
    [Header("Triggers")]
    public UnityEvent onPlay;
    public UnityEvent onDeath;
    public UnityEvent beforeAttack;
    public UnityEvent afterAttack;
    public UnityEvent startOfTurn;
    public UnityEvent endOfTurn;
    [Header("UI References")]
    [SerializeField] TMP_Text nameText;
    [SerializeField] TMP_Text descriptionText;
    [SerializeField] TMP_Text manaText;
    [SerializeField] TMP_Text attackText;
    [SerializeField] TMP_Text healthText;
    [Header("AI Minion")]
    public int deathValueBoostAI = 2;
    [Header("Minion Attack Anim")]
    public CardAnimationClip attackAnimClip;

    public virtual void Start()
    {
        SetupCardText();
        //If this minion starts out played
        if(isPlayed == true)
        {
            this.GetComponent<Draggable>().enabled = false; //disables draggable (handles dragging from hand)
            this.GetComponent<MinionCombatTarget>().enabled = true; //enables minion combat target
        }

        anim = GameObject.FindObjectOfType<CardAnimationManager>();
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
    }

    public virtual void UpdateHealth()
    {
        healthText.text = "" + health;
    }

    public override void Played(PlayerManager playerManager)
    {
        base.Played(playerManager);

        ReducePlayerMana(); //reduces player mana

        this.GetComponent<Draggable>().enabled = false; //disables draggable (handles dragging from hand)
        this.GetComponent<MinionCombatTarget>().enabled = true; //enables minion combat target
        deck.discardPile.Add(selfCardRef); //adds the minion to the discard pile
        handAnimClip.targetPos = playerManager.minionZone.GetNextCardPosition();
        anim.PlayAnimation(handAnimClip);
    }

    public virtual void AttackMinion(BaseMinion target)
    {
        target.TakeDamage(attack);
        canAttack = false;
        PlayAttackAnim(target);
    }
    public virtual void AttackHero(BaseHero target)
    {
        target.TakeDamage(attack);
        canAttack = false;
        PlayAttackAnim(target);
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
            value = 0;

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
        GameObject.FindObjectOfType<CombatManager>().UpdateAllCardsInPlay(); //updates cards in play
        Destroy(this.gameObject,5f);
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
        if(taunt == true)
            value += 1;
        //if the minion can attack this turn
        if (canAttack == true)
            value += 1;

        value -= manaCost;
        value += valueBoostAI; //adds in value boost

        if (ai.playstyle == EnemyPlaystyle.AGGRESSIVE) //checks if AI is agressive
            value = (int)(value * ValueToPercent(ai.aggroValue));
        else if (ai.playstyle == EnemyPlaystyle.MID_RANGE) //checks if AI is midrange
            value = (int)(value * ValueToPercent(ai.midRangeValue));
        else if (taunt == true && ai.playstyle == EnemyPlaystyle.DEFENSIVE) //checks if minion has taunt and AI is defensive
            value = (int)(value * ValueToPercent(ai.defenseValue));

        return value;
    }

    public virtual int CalculateDeathValue()
    {
        return attack + deathValueBoostAI;
    }
}
