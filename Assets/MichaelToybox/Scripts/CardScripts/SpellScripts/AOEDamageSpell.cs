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

        int damageValue = damage + playerManager.spellDamage; //increases damage by spell damage

        foreach (BaseCard card in targets)
        {
            if (card == null) //null card reference
                continue; //go to next

            if (card.GetComponent<BaseMinion>() == true)
            {
                card.GetComponent<BaseMinion>().TakeDamage(damageValue);
            }
            else if (card.GetComponent<BaseHero>() == true && card.GetComponent<BaseHero>().isDead == false)
            {
                card.GetComponent<BaseHero>().TakeDamage(damageValue);
            }

        }

        if (targets.Count > 0)
        {
            playAnimCopy.target = targets[0].gameObject; //sets target
            anim.PlayAnimation(playAnimCopy); //plays animation
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

        int damageValue = damage + playerManager.spellDamage; //increases damage by spell damage

        foreach (BaseCard card in targets)
        {
            if (card == null)
                continue;

            BaseMinion minion = card.GetComponent<BaseMinion>();
            BaseHero hero = card.GetComponent<BaseHero>();

            if (card.team == ai.team) //if the target is on the same team as the AI
            {
                if (minion == true)
                {
                    value -= minion.CalculateTakeDamage(damageValue);
                    effectsFriendlyMinions = true;
                }
                else if (hero == true && hero.isDead == false)
                {
                    value -= hero.CalculateTakeDamage(damageValue);
                    effectsFriendlyHero = true;
                }
            }
            else //target is on the opposite team
            {
                if (minion == true)
                {
                    value += minion.CalculateTakeDamage(damageValue);
                    effectsPlayerMinions = true;
                }
                else if (hero == true && hero.isDead == false)
                {
                    value += hero.CalculateTakeDamage(damageValue);
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
