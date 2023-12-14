using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectAddCardToHand : BaseEffect
{
    [Header("Minions to Create")]
    [SerializeField] List<CardCreateEntry> cardsToCreate;

    public override void TriggerEffect()
    {
        base.TriggerEffect();

        BaseCard card = null;

        //Gets card reference
        if (minion != null)
            card = minion;
        else if (hero != null)
            card = hero;
        else if (spell != null)
            card = spell;
        foreach (CardCreateEntry entry in cardsToCreate) //loops through all cards to be added to hand
        {
            if (playerManager.handManager.CheckHandFull() == true) //if the hand is full
                return; //stop code

            playerManager.handManager.CreateCardInHand(entry.card, hero); //adds card to hand
        }
    }

    public override int CalculateEffectValueAI()
    {
        int value = 0;

        BaseEnemyAI ai = combatManager.enemyAI; //gets AI reference for calculation

        foreach (CardCreateEntry entry in cardsToCreate) //loops through all cards to be created
        {
            if(entry.card.manaCost == 0) //if the card is free
            {
                value += entry.card.CalculateValueAI(ai); //add in the value of the card 
            }
            else if (entry.card.manaCost > playerManager.mana) //if we cannot play the card
            {
                value += 0; //no value added (card will not be played)
            }
            else if (entry.card.CalculateValueAI(ai) > 0) //if the value of the card is above 0 (we get a benefit from playing the card)
            {
                value += 1; //add in 1 value for the card being created
            }
        }

        return value;
    }
}

[System.Serializable]
public class CardCreateEntry
{
    public BaseCard card;
}
