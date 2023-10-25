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
    public BaseCard selfCardRef;
    [HideInInspector] public PlayerManager playerManager;
    [HideInInspector] public DeckManager deck;
    [HideInInspector] public PlayerMinionZone zone;
    [Header("Stats")]
    public int manaCost;
    public bool isPlayed = false;

    //references
    protected PointerEventData eventData;

    /// <summary>
    /// Adds the card to the discard pile
    /// </summary>
    public virtual void AddToDiscardPile()
    {
        deck.discardPile.Add(selfCardRef);
    }

    /// <summary>
    /// Called when the card is played. sets isPlayed to true
    /// </summary>
    /// <param name="playerManager"></param>
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
