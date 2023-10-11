using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeckManager : MonoBehaviour
{
    public List<BaseCard> deck;
    public List<BaseCard> discardPile;

    private void Start()
    {
        ShuffleDeck();
    }

    /// <summary>
    /// If the deck runs out of cards, it combines the deck and the discard pile
    /// Draws the top card in the deck. 
    /// Tells handManager to add card to hand 
    /// Then removes the top card from the deck list
    /// </summary>
    public void DrawCard(HandManager handManager)
    {
        if(deck.Count <= 0) //no cards in deck
        {
            CombineDeck(); //combines discard and draw deck
        }

        if (deck.Count <= 0) //our deck is empty, even after combining discard and draw deck
            return;

        //creates and adds card in hand
        handManager.AddCardToHand(deck[0],this); //adds card to hand

        deck.RemoveAt(0); //removes first card from deck
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
