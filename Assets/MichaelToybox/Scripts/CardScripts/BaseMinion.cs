using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class BaseMinion : BaseCard
{
    public int attack;
    public int health;
    public bool canAttack = false;
    public Team team = Team.PLAYER;
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
        Debug.Log("Minion Played: " + name);
    }
}
