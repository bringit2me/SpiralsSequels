using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandManager : MonoBehaviour
{
    public GameObject handHolder;
    public PlayerManager playerManager;
    public List<BaseCard> handCards;

    public void AddCardToHand(BaseCard card, DeckManager deck)
    {
        BaseCard temp = Instantiate(card, handHolder.transform); //creates card
        temp.playerManager = playerManager; //sets player manager reference
        temp.deck = deck; //sets deck reference
        temp.selfCardRef = card; //sets self card reference
        temp.team = playerManager.team; //sets team 
        temp.hero = deck.hero; //sets card hero reference
        handCards.Add(temp); //adss card to hand list

        temp.SetupAllEffects(); //sets up effects on the card
    }

    public void CreateCardInHand(BaseCard card, BaseHero hero)
    {
        BaseCard temp = Instantiate(card, handHolder.transform); //creates card
        temp.playerManager = playerManager; //sets player manager reference
        temp.selfCardRef = card; //sets self card reference
        temp.team = playerManager.team; //sets team 
        temp.playSendToDiscard = false; //we do not want to send the card to the discard pile on play
        temp.hero = hero; //sets hero reference
        handCards.Add(temp); //adss card to hand list

        temp.SetupAllEffects(); //sets up effects on the card
    }

    public void ReturnCardToHand(BaseCard card, DeckManager deck)
    {
        handCards.Add(card);
    }

    /// <summary>
    /// Adds all cards that are in the handCards list to their decks discard pile
    /// Destroys the card
    /// </summary>
    public void DiscardHand()
    {
        foreach(BaseCard card in handCards) //loops through each card in the hand
        {
            if(card.deck != null) //if the card has a deck
            {
                card.AddToDiscardPile();
            }
            else
            {
                Debug.LogWarning("MICHAEL WARN: Card discarded without a discard pile");
            }

            Destroy(card.gameObject);
        }

        handCards.Clear();
    }

    public void RemoveCardFromHand(BaseCard card)
    {
        handCards.Remove(card);
    }

    /// <summary>
    /// Checks if the hand is full. returns true if there is 15 or more cards in hand
    /// </summary>
    /// <returns></returns>
    public bool CheckHandFull()
    {
        if (handCards.Count >= 15)
            return true;
        return false;
    }
}
