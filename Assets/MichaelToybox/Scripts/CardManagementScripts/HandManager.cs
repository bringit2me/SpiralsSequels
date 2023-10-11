using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandManager : MonoBehaviour
{
    public GameObject handHolder;

    public void AddCardToHand(BaseCard card, DeckManager deck)
    {
        BaseCard temp = Instantiate(card, handHolder.transform);
        temp.deck = deck;
        temp.cardRef = card;
    }

    /// <summary>
    /// Adds all cards that are a child of the hand object to their decks discard pile
    /// Destroys the card
    /// </summary>
    public void DiscardHand()
    {
        foreach(BaseCard card in handHolder.transform.GetComponentsInChildren<BaseCard>())
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
    }
}
