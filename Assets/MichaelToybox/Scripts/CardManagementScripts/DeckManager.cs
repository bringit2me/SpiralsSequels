using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeckManager : MonoBehaviour
{
    public List<BaseCard> deck;
    public List<BaseCard> discardPile;
    [SerializeField] HandManager handManager;

    private void Start()
    {
        ShuffleDeck();
    }

    /// <summary>
    /// Draws a card from the deck. If the deck runs out of cards, it combines the deck and the discard pile
    /// </summary>
    public void DrawCard()
    {
        if(deck.Count <= 0) //no cards in deck
        {
            CombineDeck();
        }
    }

    /// <summary>
    /// Shuffles the deck
    /// </summary>
    public void ShuffleDeck()
    {
        deck.Shuffle();
    }

    /// <summary>
    /// Use this to reset the deck (when it runs out of cards)
    /// 
    /// What it does:
    /// Combines cards in the deck with cards in the discard pile.
    /// Clears discard pile
    /// Shuffles deck
    /// </summary>
    public void CombineDeck()
    {
        //loops through each card in the discard pile
        foreach(BaseCard card in discardPile)
        {
            deck.Add(card); //adds card to deck
        }

        discardPile.Clear(); //clears discard pile

        ShuffleDeck(); //shuffles deck
    }
}
