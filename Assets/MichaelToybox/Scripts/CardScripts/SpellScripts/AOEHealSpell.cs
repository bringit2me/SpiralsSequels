using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AOEHealSpell : BaseAOESpell
{
    [Header("Heal Value")]
    [SerializeField] int healValue;

    public override void Cast()
    {
        base.Cast();
        foreach (BaseCard card in targets)
        {
            if (card.GetComponent<BaseMinion>() == true)
            {
                card.GetComponent<BaseMinion>().Heal(healValue);
            }
            else if (card.GetComponent<BaseHero>() == true)
            {
                card.GetComponent<BaseHero>().Heal(healValue);
            }
        }
        EndCast();
    }

    public override int CalculateValueAI(BaseEnemyAI ai)
    {
        int value = 0;
        List<BaseCard> targets = combatManager.GetTargets(team, targetTeam); //gets all targets of the spell

        bool effectsFriendlyHero = false;
        bool effectsFriendlyMinions = false;
        bool effectsPlayerHero = false;
        bool effectsPlayerMinions = false;

        foreach (BaseCard card in targets)
        {

            if (card.team == ai.team) //if the target is on the same team as the AI
            {
                if (card.GetComponent<BaseMinion>() == true)
                {
                    value += card.GetComponent<BaseMinion>().CalculateHeal(healValue);
                    if(card.GetComponent<BaseMinion>().CalculateHeal(healValue) > 0)
                        effectsFriendlyMinions = true;
                }
                else if (card.GetComponent<BaseHero>() == true)
                {
                    value += card.GetComponent<BaseHero>().CalculateHeal(healValue);
                    if (card.GetComponent<BaseHero>().CalculateHeal(healValue) > 0)
                        effectsFriendlyHero = true;
                }
            }
            else //target is on the opposite team
            {
                if (card.GetComponent<BaseMinion>() == true)
                {
                    value -= card.GetComponent<BaseMinion>().CalculateHeal(healValue);
                    if (card.GetComponent<BaseMinion>().CalculateHeal(healValue) > 0)
                        effectsPlayerMinions = true;
                }
                else if (card.GetComponent<BaseHero>() == true)
                {
                    value -= card.GetComponent<BaseHero>().CalculateHeal(healValue);
                    if (card.GetComponent<BaseHero>().CalculateHeal(healValue) > 0)
                        effectsPlayerHero = true;
                }
            }


        }

        //checks if AI is agressive and an enemy hero is effected
        if (ai.playstyle == EnemyPlaystyle.AGGRESSIVE && effectsPlayerHero == true)
            value = value - 2;
        //checks if AI is mid range and a friendly minion is effected
        if (ai.playstyle == EnemyPlaystyle.MID_RANGE && effectsFriendlyMinions == true)
            value = (int)(value * ValueToPercent(ai.midRangeValue));
        //checks if AI is defensive and a friendly hero is effected
        if (ai.playstyle == EnemyPlaystyle.DEFENSIVE && effectsFriendlyHero == true)
            value = (int)(value * ValueToPercent(ai.defenseValue));

        value += valueBoostAI; //adds in value boost
        value -= manaCost; //subtracts mana cost

        return value;
    }
}
