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
    public int maxHealth;
    public int health;
    public bool canAttack = false;
    public bool targetable = true;
    [Header("UI References")]
    [SerializeField] TMP_Text nameText;
    [SerializeField] TMP_Text attackText;
    [SerializeField] TMP_Text healthText;
    [Header("Hero Power")]
    public BaseHeroPower heroPower;

    private void Start()
    {
        SetupCardText();
        isPlayed = true;
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
    }

    public virtual void UpdateHealth()
    {
        healthText.text = "" + health;
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
    /// <summary>
    /// Damages hero
    /// </summary>
    /// <param name="value"></param>
    public virtual void TakeDamage(int value)
    {
        if (value > 0)
            health -= value;

        if (health <= 0)
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
        //TODO: Figure something out
        //Destroy(this.gameObject);
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
