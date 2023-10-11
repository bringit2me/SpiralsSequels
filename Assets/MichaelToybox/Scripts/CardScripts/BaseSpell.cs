using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class BaseSpell : BaseCard
{

    [Header("UI References")]
    [SerializeField] TMP_Text nameText;
    [SerializeField] TMP_Text descriptionText;
    [SerializeField] TMP_Text manaText;

    public virtual void Start()
    {
        SetupCardText();
    }

    public override void Played(PlayerManager playerManager)
    {
        base.Played(playerManager);
        Cast();
    }

    /// <summary>
    /// Cast is called when a spell is first cast
    /// </summary>
    public virtual void Cast()
    {
        Debug.Log("Cast: " + name);
    }

    /// <summary>
    /// Execute cast is called every frame while the spell is active
    /// </summary>
    public virtual void ExecuteCast()
    {

    }

    /// <summary>
    /// End cast is called when the spell cast ends
    /// </summary>
    public virtual void EndCast()
    {
        Destroy(this.gameObject);
    }

    /// <summary>
    /// Stop cast is called
    /// </summary>
    public virtual void StopCast()
    {
        //TODO: Return this to hand
    }

    public virtual void SetupCardText()
    {
        nameText.text = name;
        descriptionText.text = description;
        UpdateMana();
    }

    public virtual void UpdateMana()
    {
        manaText.text = "" + manaCost;
    }
}
