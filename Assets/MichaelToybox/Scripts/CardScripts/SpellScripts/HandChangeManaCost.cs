using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandChangeManaCost : BaseSpell
{
    [Header("Mana Cost Change")]
    public int manaCostChange = 0;

    public override void Start()
    {
        base.Start();
    }

    public override void SetupEffectEntry()
    {
        base.SetupEffectEntry();
    }

    public override void Cast()
    {
        base.Cast();

        //loops through each card in the hand
        foreach(BaseCard card in playerManager.handManager.handCards)
        {
            //Add stat change entry ot card. Also sets card effect entry (extra description to show when hovering card)
            card.visualManager.AddStatChangeEntry(card.CalculateManaCostChange(manaCostChange), cardEffectEntry);
            playAnimCopy.cardVisualsToUpdate.Add(card); //adds card to updater (updates card visuals after animation)

            card.ChangeManaCost(manaCostChange); //changes the cost by manaCostChange
        }

        anim.PlayAnimation(playAnimCopy); //plays animation

        EndCast();
    }

    public override int CalculateValueAI(BaseEnemyAI ai)
    {
        int value = 0;

        foreach (BaseCard card in playerManager.handManager.handCards)
        {
            int costCalculation = card.CalculateManaCostChange(manaCostChange);

            //if when the cards cost is reduced, it will be less than the remaining mana the AI will have
            if(card.manaCost - costCalculation < playerManager.mana - manaCost)
            {
                value += costCalculation; //increases value by the cost reduced
            }
        }

        value -= manaCost; //subtracts mana cost
        value += valueBoostAI; //adds in value boost

        //NOTE: these are the playstyle modifiers. add these if you find a reason to do so
        //checks if AI is agressive
        //if (ai.playstyle == EnemyPlaystyle.AGGRESSIVE)
        //    value = (int)(value * ValueToPercent(ai.aggroValue));
        //checks if AI is mid range
        //if (ai.playstyle == EnemyPlaystyle.MID_RANGE)
        //    value = (int)(value * ValueToPercent(ai.midRangeValue));
        //checks if AI is defensive
        //if (ai.playstyle == EnemyPlaystyle.DEFENSIVE)
        //    value = (int)(value * ValueToPercent(ai.defenseValue));

        return value;
    }
}
