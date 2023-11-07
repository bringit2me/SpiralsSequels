using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class BaseSpell : BaseCard
{
    bool isBeingCast = false;
    [Header("Effect Triggers")]
    public List<BaseEffect> onPlay;//(NOT IMPLEMENTED) called when the spell is played
    public List<BaseEffect> actionTakenInHand;//(NOT IMPLEMENTED) called whenever a card is played while this is in the hand
    [Header("UI References")]
    [SerializeField] TMP_Text nameText;
    [SerializeField] TMP_Text descriptionText;
    [SerializeField] TMP_Text manaText;

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
        SetupAllEffects(); //sets up all effects
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
        combatManager.ActionTakenTrigger(); //calls action taken trigger

        TriggerOnPlayEffects(); //triggers after play effects
    }

    /// <summary>
    /// Stop cast is called when the player manually stops the spell cast before it finishes
    /// </summary>
    public virtual void StopCastEarly()
    {
        isBeingCast = false;
        isPlayed = false;
        this.transform.SetParent(playerManager.handManager.handHolder.transform);

        //Turns on raycast blocking (so mouse can detect the spell again)
        GetComponent<CanvasGroup>().blocksRaycasts = true;
        playerManager.handManager.ReturnCardToHand(this, deck);
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

    // --- CALLING EFFECTS ---

    /// <summary>
    /// triggers all onPlay effects
    /// </summary>
    public virtual void TriggerOnPlayEffects()
    {
        foreach (BaseEffect effect in onPlay)
            effect.TriggerEffect();
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
    /// Sets up all effects on the card with all references (only passes in references we can get off of the current card)
    /// </summary>
    public override void SetupAllEffects()
    {
        foreach(BaseEffect effect in onPlay)
        {
            //sets up effect with a hero reference, no minion reference, and spell reference
            effect.SetupEffect(hero, null, this);
        }
        foreach (BaseEffect effect in actionTakenInHand)
        {
            //sets up effect with a hero reference, no minion reference, and spell reference
            effect.SetupEffect(hero, null, this);
        }
    }
}
