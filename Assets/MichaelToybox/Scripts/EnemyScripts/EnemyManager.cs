using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : PlayerManager
{
    public override void Awake()
    {
        //gets references in the scene
        combatManager = GameObject.FindObjectOfType<CombatManager>();
        handManager = this.GetComponent<HandManager>();
        minionZone = this.GetComponentInChildren<EnemyMinionZone>();
    }

    public override IEnumerator StartTurnDraw()
    {
        foreach (DeckManager deck in heroDecks) //loops through each hero deck
        {
            if (deck.hero != null && deck.hero.health <= 0) //if the decks hero is dead
                continue; //goes to next entry in the foreach loop

            for (int i = 0; i < drawCountHero; i++) //loops for each time we want to draw from the hero deck
            {
                deck.DrawCard(handManager); //draws from the heroes deck
                yield return new WaitForSeconds(drawDelay);
            }
        }

        for (int i = 0; i < drawCountNeutral; i++) //loops for each time we want to draw from the neutral deck
        {
            neutralDeck.DrawCard(handManager); //draws from the neurtral deck
            yield return new WaitForSeconds(drawDelay);
        }
    }
}
