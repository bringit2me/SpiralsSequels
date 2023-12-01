using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeManaSpell : BaseSpell
{
    [Header("Mana Change")]
    [SerializeField] int manaChange;

    public override void Cast()
    {
        base.Cast();

        playerManager.mana += manaChange;
        playerManager.UpdateManaText();

        anim.PlayAnimation(playAnimCopy); //plays animation

        EndCast();
    }

    public override int CalculateValueAI(BaseEnemyAI ai)
    {
        int value = 0;

        value += manaChange; //increases value by mana change

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
