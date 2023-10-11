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

    public virtual void AddToDiscardPile()
    {
        deck.discardPile.Add(cardRef);
    }
}
