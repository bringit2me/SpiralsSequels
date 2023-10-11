using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseCard : MonoBehaviour
{
    public new string name;
    [TextArea(5, 15)]
    public string description;
    public BaseCard cardRef;
    [HideInInspector] public DeckManager deck;
    [Header("Stats")]
    public int manaCost;
    public bool isPlayed = false;

    public virtual void AddToDiscardPile()
    {
        deck.discardPile.Add(cardRef);
    }

    public virtual void Played(PlayerManager playerManager)
    {
        //lowers players mana by the cost
        playerManager.mana -= manaCost;
        //Updates mana text
        playerManager.UpdateManaText();
        //sets is played to true
        isPlayed = true;
    }
}

public enum Team
{
    PLAYER,
    ENEMY
}
