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

    public virtual void Start()
    {
        SetupCardText();
    }

    // --- CARD SETUP ---

    public virtual void SetupCardText()
    {
        nameText.text = name;
        descriptionText.text = description;
        UpdateMana();
        UpdateAttack();
        UpdateHealth();
        canAttack = false;
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
    }

    public virtual void AttackMinion(BaseMinion target)
    {
        target.TakeDamage(attack);
        canAttack = false;
    }
    public virtual void AttackHero(BaseHero target)
    {
        target.TakeDamage(attack);
        canAttack = false;
    }
    public virtual void TakeDamage(int value)
    {
        if(value > 0)
            health -= value;

        if(health <= 0)
        {
            Dead();
        }

        UpdateHealth();
    }

    public virtual void Heal(int value)
    {
        health = Mathf.Clamp(health + value, 0, maxHealth);

        UpdateHealth();
    }

    public virtual void Dead()
    {
        Destroy(this.gameObject);
    }

    public virtual void ChangeAttack(int value)
    {
        attack += value;

        UpdateAttack();
    }

    public virtual void ChangeHealth(int value)
    {
        health += value;
        maxHealth += value;

        if (health <= 0)
        {
            Dead();
        }

        UpdateHealth();
    }
}
