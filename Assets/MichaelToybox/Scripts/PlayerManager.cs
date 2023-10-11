using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    [SerializeField] HandManager handManager;
    [Header("Neutral Deck")]
    public int drawCountNeutral = 4;
    public DeckManager neutralDeck;
    [Header("Hero Decks")]
    public int drawCountHero = 2;
    public DeckManager[] heroDecks;

    public void StartTurn()
    {
        StartCoroutine(StartTurnDraw());
    }

    public void EndTurn()
    {
        handManager.DiscardHand();
    }

    IEnumerator StartTurnDraw()
    {
        foreach (DeckManager deck in heroDecks)
        {
            for (int i = 0; i < drawCountHero; i++)
            {
                deck.DrawCard(handManager);
                yield return new WaitForSeconds(0.25f);
            }
        }

        for (int i = 0; i < drawCountNeutral; i++)
        {
            neutralDeck.DrawCard(handManager);
            yield return new WaitForSeconds(0.25f);
        }
    }
}
