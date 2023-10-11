using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class BaseMinion : BaseCard
{
    public int attack;
    public int health;
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

    public virtual void SetupCardText()
    {
        nameText.text = name;
        descriptionText.text = description;
        UpdateMana();
        UpdateAttack();
        UpdateHealth();
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
}
