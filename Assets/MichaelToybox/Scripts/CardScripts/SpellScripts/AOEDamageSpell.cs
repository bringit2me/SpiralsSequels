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

        foreach (BaseCard card in targets)
        {
            if (card.team == ai.team) //if the target is on the same team as the AI
            {
                if (card.GetComponent<BaseMinion>() == true)
                {
                    value -= card.GetComponent<BaseMinion>().CalculateTakeDamage(damage);
                }
                else if (card.GetComponent<BaseHero>() == true)
                {
                    value -= card.GetComponent<BaseHero>().CalculateTakeDamage(damage);
                }
            }
            else //target is on the opposite team
            {
                if (card.GetComponent<BaseMinion>() == true)
                {
                    value += card.GetComponent<BaseMinion>().CalculateTakeDamage(damage);
                }
                else if (card.GetComponent<BaseHero>() == true)
                {
                    value += card.GetComponent<BaseHero>().CalculateTakeDamage(damage);
                }
            }
        }

        value += valueBoostAI; //adds in value boost
        value -= manaCost; //subtracts mana cost

        return value;
    }
}
