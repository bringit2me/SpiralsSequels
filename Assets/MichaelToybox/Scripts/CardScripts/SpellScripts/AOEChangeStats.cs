using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AOEChangeStats : BaseAOESpell
{
    [Header("Stat Change")]
    [SerializeField] int attackChange;
    [SerializeField] int healthChange;

    public override void Cast()
    {
        base.Cast();
        foreach (BaseCard card in targets)
        {
            if (card.GetComponent<BaseMinion>() == true)
            {
                card.GetComponent<BaseMinion>().ChangeAttack(attackChange);
                card.GetComponent<BaseMinion>().ChangeHealth(healthChange);
            }
            else if (card.GetComponent<BaseHero>() == true)
            {
                card.GetComponent<BaseHero>().ChangeAttack(attackChange);
                card.GetComponent<BaseHero>().ChangeHealth(healthChange);
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
                    value += card.GetComponent<BaseMinion>().CalculateAttackChange(attackChange);
                    value += card.GetComponent<BaseMinion>().CalculateHealthChange(healthChange);
                }
                else if (card.GetComponent<BaseHero>() == true)
                {
                    value += card.GetComponent<BaseHero>().CalculateAttackChange(attackChange) * 2; //2x multiplier for giving attack to a hero
                    value += card.GetComponent<BaseHero>().CalculateHealthChange(healthChange);
                }
            }
            else //target is on the opposite team
            {
                if (card.GetComponent<BaseMinion>() == true)
                {
                    value -= card.GetComponent<BaseMinion>().CalculateAttackChange(attackChange);
                    value -= card.GetComponent<BaseMinion>().CalculateHealthChange(healthChange);
                }
                else if (card.GetComponent<BaseHero>() == true)
                {
                    value -= card.GetComponent<BaseHero>().CalculateAttackChange(attackChange) * 2;//2x multiplier for giving attack to a hero
                    value -= card.GetComponent<BaseHero>().CalculateHealthChange(healthChange);
                }
            }
        }

        value -= manaCost; //subtracts mana cost
        value += valueBoostAI; //adds in value boost

        return value;
    }
}
