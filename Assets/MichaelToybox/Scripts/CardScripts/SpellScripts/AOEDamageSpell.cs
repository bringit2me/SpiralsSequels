using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AOEDamageSpell : BaseAOESpell
{
    [Header("Damage")]
    [SerializeField] int damage;

    public override void Cast()
    {
        base.Cast();
        foreach(BaseCard card in targets)
        {
            if (card.GetComponent<BaseMinion>() == true)
            {
                card.GetComponent<BaseMinion>().TakeDamage(damage);
            }
            else if (card.GetComponent<BaseHero>() == true)
            {
                card.GetComponent<BaseHero>().TakeDamage(damage);
            }
        }
        EndCast();
    }

    public override int CalculateValueAI(BaseEnemyAI ai)
    {
        int value = 0;
        List<BaseCard> targets = combatManager.GetTargets(team,targetTeam); //gets all targets of the spell

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
                    value -= card.GetComponent<BaseMinion>().CalculateTakeDamage(damage);
                    effectsFriendlyMinions = true;
                }
                else if (card.GetComponent<BaseHero>() == true)
                {
                    value -= card.GetComponent<BaseHero>().CalculateTakeDamage(damage);
                    effectsFriendlyHero = true;
                }
            }
            else //target is on the opposite team
            {
                if (card.GetComponent<BaseMinion>() == true)
                {
                    value += card.GetComponent<BaseMinion>().CalculateTakeDamage(damage);
                    effectsPlayerMinions = true;
                }
                else if (card.GetComponent<BaseHero>() == true)
                {
                    value += card.GetComponent<BaseHero>().CalculateTakeDamage(damage);
                    effectsPlayerHero = true;
                }
            }
        }

        //checks if AI is agressive and an enemy hero is effected
        if (ai.playstyle == EnemyPlaystyle.AGGRESSIVE && effectsPlayerHero == true)
            value = (int)(value * ValueToPercent(ai.aggroValue));
        //checks if AI is mid range and a enemy minion is effected
        if (ai.playstyle == EnemyPlaystyle.MID_RANGE && effectsPlayerMinions == true)
            value = (int)(value * ValueToPercent(ai.midRangeValue));
        //checks if AI is defensive and a enemy minion is effected
        if (ai.playstyle == EnemyPlaystyle.DEFENSIVE && effectsPlayerMinions == true)
            value = (int)(value * ValueToPercent(ai.defenseValue));

        value += valueBoostAI; //adds in value boost
        value -= manaCost; //subtracts mana cost

        return value;
    }
}
