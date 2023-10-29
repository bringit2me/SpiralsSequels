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

        foreach (BaseCard card in targets)
        {
            if (card.team == ai.team) //if the target is on the same team as the AI
            {
                if (card.GetComponent<BaseMinion>() == true)
                {
                    value += card.GetComponent<BaseMinion>().CalculateHeal(healValue);
                }
                else if (card.GetComponent<BaseHero>() == true)
                {
                    value += card.GetComponent<BaseHero>().CalculateHeal(healValue);
                }
            }
            else //target is on the opposite team
            {
                if (card.GetComponent<BaseMinion>() == true)
                {
                    value -= card.GetComponent<BaseMinion>().CalculateHeal(healValue);
                }
                else if (card.GetComponent<BaseHero>() == true)
                {
                    value -= card.GetComponent<BaseHero>().CalculateHeal(healValue);
                }
            }
        }

        value -= manaCost; //subtracts mana cost

        return value;
    }
}
