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
            else if (card.GetComponent<BaseHero>() == true && card.GetComponent<BaseHero>().isDead == false)
            {
                card.GetComponent<BaseHero>().ChangeAttack(attackChange);
                card.GetComponent<BaseHero>().ChangeHealth(healthChange);
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
        List<BaseCard> targets = combatManager.GetTargets(team, targetTeam); //gets all targets of the spell

        bool effectsFriendlyHero = false;
        bool effectsFriendlyMinions = false;
        bool effectsPlayerHero = false;
        bool effectsPlayerMinions = false;

        foreach (BaseCard card in targets)
        {
            BaseMinion minion = card.GetComponent<BaseMinion>();
            BaseHero hero = card.GetComponent<BaseHero>();

            if (card.team == ai.team) //if the target is on the same team as the AI
            {
                if (minion == true)
                {
                    value += minion.CalculateAttackChange(attackChange);
                    value += minion.CalculateHealthChange(healthChange);
                    effectsFriendlyMinions = true;
                }
                else if (hero == true && hero.isDead == false)
                {
                    value += hero.CalculateAttackChange(attackChange) * 2; //2x multiplier for giving attack to a hero
                    value += hero.CalculateHealthChange(healthChange);
                    effectsFriendlyHero = true;
                }
            }
            else //target is on the opposite team
            {
                if (minion == true)
                {
                    value -= minion.CalculateAttackChange(attackChange);
                    value -= minion.CalculateHealthChange(healthChange);
                    effectsPlayerMinions = true;
                }
                else if (hero == true && hero.isDead == false)
                {
                    value -= hero.CalculateAttackChange(attackChange) * 3;//3x multiplier for giving attack to a hero
                    value -= hero.CalculateHealthChange(healthChange);
                    effectsPlayerHero = true;
                }
            }
        }

        value -= manaCost; //subtracts mana cost
        value += valueBoostAI; //adds in value boost

        //checks if AI is agressive, friendly character is effected, and attack is increased
        if (ai.playstyle == EnemyPlaystyle.AGGRESSIVE && (effectsFriendlyMinions == true || effectsFriendlyHero == true) && attackChange > 0)
            value = value - 2;
        //checks if AI is mid range, friendly minion is effected, and stats are increased
        if (ai.playstyle == EnemyPlaystyle.MID_RANGE && effectsFriendlyMinions == true && (attackChange + healthChange) > 0)
            value = (int)(value * ValueToPercent(ai.midRangeValue));
        //checks if AI is defensive, friendly hero is effected, and health is increased
        if (ai.playstyle == EnemyPlaystyle.DEFENSIVE && effectsFriendlyHero == true && healthChange > 0)
            value = (int)(value * ValueToPercent(ai.defenseValue));
        //checks if AI is defensive, enemy character is effected, and attack is lowered
        else if (ai.playstyle == EnemyPlaystyle.DEFENSIVE && (effectsPlayerMinions == true || effectsPlayerHero == true) && attackChange < 0)
            value = (int)(value * ValueToPercent(ai.defenseValue));

        return value;
    }
}
