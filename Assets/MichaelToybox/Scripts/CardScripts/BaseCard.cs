using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class BaseCard : MonoBehaviour
{
    public new string name;
    [TextArea(5, 15)]
    public string description;
    public Team team;
    public BaseCard cardRef;
    [HideInInspector] public DeckManager deck;
    [Header("Stats")]
    public int manaCost;
    public bool isPlayed = false;

    //references
    protected PlayerManager playerManager;
    protected PointerEventData eventData;

    public virtual void AddToDiscardPile()
    {
        deck.discardPile.Add(cardRef);
    }

    public virtual void Played(PlayerManager playerManager)
    {
        //sets is played to true
        isPlayed = true;
        this.playerManager = playerManager;
    }

    /// <summary>
    /// Takes mana from the playerManager's mana and updates the mana text
    /// </summary>
    public virtual void ReducePlayerMana()
    {
        //lowers players mana by the cost
        playerManager.mana -= manaCost;
        //Updates mana text
        playerManager.UpdateManaText();
    }
}

public enum Team
{
    PLAYER,
    ENEMY,
    NONE
}
