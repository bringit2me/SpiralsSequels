using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class BaseSpell : BaseCard
{
    bool isBeingCast = false;
    [Header("UI References")]
    [SerializeField] TMP_Text nameText;
    [SerializeField] TMP_Text descriptionText;
    [SerializeField] TMP_Text manaText;
    protected CombatManager combatManager;

    public virtual void Start()
    {
        SetupCardText();
        combatManager = GameObject.FindObjectOfType<CombatManager>();
    }

    public virtual void Update()
    {
        if(isBeingCast == true)
        {
            ExecuteCast();
        }
    }

    public override void Played(PlayerManager playerManager)
    {
        base.Played(playerManager);
        Cast();
        this.transform.position = new Vector3(1800, 445, 0);
    }

    /// <summary>
    /// Cast is called when a spell is first cast
    /// </summary>
    public virtual void Cast()
    {
        isBeingCast = true;
    }

    /// <summary>
    /// Execute cast is called every frame while the spell is active
    /// </summary>
    public virtual void ExecuteCast()
    {
        if(Input.GetKeyDown(KeyCode.Mouse1)) //right click
        {
            StopCastEarly();
        }
    }

    /// <summary>
    /// End cast is called when the spell cast ends
    /// </summary>
    public virtual void EndCast()
    {
        AddToDiscardPile(); //adds card to discard pile
        Destroy(this.gameObject);
        isBeingCast = false;

        ReducePlayerMana();
    }

    /// <summary>
    /// Stop cast is called when the player manually stops the spell cast before it finishes
    /// </summary>
    public virtual void StopCastEarly()
    {
        isBeingCast = false;
        isPlayed = false;
        this.transform.SetParent(playerManager.handManager.handHolder.transform);
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
