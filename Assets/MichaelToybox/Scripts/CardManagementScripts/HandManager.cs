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
        foreach(BaseCard card in handCards)
        {
            if(card.deck != null)
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
}
