using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SideboardDeckManager : MonoBehaviour
{
    DeckManager deckManager;
    HandManager handManager;
    List<BaseCard> cards = new List<BaseCard>();
    [Header("Sideboard Display Slots")]
    public GameObject displaySlot1;
    public GameObject displaySlot2;
    public GameObject displaySlot3;

    private void Start()
    {
        deckManager = this.GetComponent<DeckManager>(); //gets deck manager
        handManager = this.GetComponentInParent<HandManager>(); //gets hand manager
    }

    /// <summary>
    /// Displays the top 3 cards of the deck on the screen
    /// </summary>
    public void DisplayCards()
    {
        deckManager.ShuffleDeck(); //shuffles deck
        cards = deckManager.GetTopThree(); //gets top 3 cards of the deck

        if (cards.Count > 0)
        {
            BaseCard card1 = Instantiate(cards[0], displaySlot1.transform); //creates card
            card1.selfCardRef = cards[0]; //sets self card ref
            card1.transform.localScale = new Vector3(2, 2, 2); //makes card bigger
            card1.GetComponent<Draggable>().enabled = false; //disables draggable
            card1.gameObject.AddComponent<ClickAddToHand>().card = card1; //enables card and sets reference
        }

        if (cards.Count > 1)
        {
            BaseCard card2 = Instantiate(cards[1], displaySlot2.transform); //creates card
            card2.selfCardRef = cards[1]; //sets self card ref
            card2.transform.localScale = new Vector3(2, 2, 2); //makes card bigger
            card2.GetComponent<Draggable>().enabled = false; //disables draggable
            card2.gameObject.AddComponent<ClickAddToHand>().card = card2; //enables card and sets reference
        }

        if (cards.Count > 2)
        {
            BaseCard card3 = Instantiate(cards[2], displaySlot3.transform); //creates card
            card3.selfCardRef = cards[2]; //sets self card ref
            card3.transform.localScale = new Vector3(2, 2, 2); //makes card bigger
            card3.GetComponent<Draggable>().enabled = false; //disables draggable
            card3.gameObject.AddComponent<ClickAddToHand>().card = card3; //enables card and sets reference
        }
    }

    public void CardSelected(BaseCard card)
    {

        //removes card from deck
        deckManager.RemoveCard(card);
        //adds card to hand
        handManager.AddCardToHand(card.selfCardRef, deckManager);

        //Removes cards in the display slots
        if(displaySlot1.transform.childCount > 0)
            Destroy(displaySlot1.transform.GetChild(0).gameObject);
        if (displaySlot2.transform.childCount > 0)
            Destroy(displaySlot2.transform.GetChild(0).gameObject);
        if (displaySlot3.transform.childCount > 0)
            Destroy(displaySlot3.transform.GetChild(0).gameObject);
    }

    //Adds card to deck. then shuffles it
    public void AddCardToDeck(BaseCard card)
    {
        deckManager.AddCardToDeck(card);
    }
}
