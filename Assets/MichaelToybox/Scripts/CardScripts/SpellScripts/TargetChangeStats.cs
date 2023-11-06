using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetChangeStats : BaseTargetSpell
{
    [Header("Stat Change")]
    [SerializeField] int attackChange;
    [SerializeField] int healthChange;

    public override void CastAtTarget()
    {
        if (target.GetComponent<BaseMinion>() == true)
        {
            target.GetComponent<BaseMinion>().ChangeAttack(attackChange);
            target.GetComponent<BaseMinion>().ChangeHealth(healthChange);
            playAnimClip.target = target.gameObject;
            CardAnimationClip clip = new CardAnimationClip();
            clip.CopyClip(playAnimClip);
            anim.PlayAnimation(clip);
        }
        if (target.GetComponent<BaseHero>() == true)
        {
            target.GetComponent<BaseHero>().ChangeAttack(attackChange);
            target.GetComponent<BaseHero>().ChangeHealth(healthChange);
            playAnimClip.target = target.gameObject;
            CardAnimationClip clip = new CardAnimationClip();
            clip.CopyClip(playAnimClip);
            anim.PlayAnimation(clip);
        }

        base.CastAtTarget();
    }

    public override CardValueEntry CalculateValueAI(BaseEnemyAI ai)
    {
        CardValueEntry entry = new CardValueEntry();
        entry.card = this;
        List<BaseCard> targets = combatManager.GetTargets(team, targetTeam); //gets all potential targets of the spell

        foreach (BaseCard card in targets)
        {
            int value = 0;
            //Gets minion reference. if card is not a minion it will be null
            BaseMinion minion = card.GetComponent<BaseMinion>();
            //Gets hero reference. if card is not a hero it will be null
            BaseHero hero = card.GetComponent<BaseHero>();

            int cardAttack = 0;
            int cardHealth = 0;

            if (card.team == ai.team) //if the target is on the same team as the AI
            {
                if (minion != null)
                {
                    //changes value
                    value += minion.CalculateAttackChange(attackChange);
                    value += minion.CalculateHealthChange(healthChange);
                    //if the spell kills the target
                    if (minion.health + minion.CalculateHealthChange(healthChange) <= 0)
                    {
                        value -= minion.CalculateDeathValue();
                    }
                    //gets stats from minion
                    cardAttack = minion.attack;
                    cardHealth = minion.health;
                }
                else if (hero != null && hero.isDead == false)
                {
                    //changes value
                    value += hero.CalculateAttackChange(attackChange) * 2; //2x multiplier for giving attack to a hero
                    value += hero.CalculateHealthChange(healthChange);
                    //if the spell kills the target hero
                    if (hero.health + hero.CalculateHealthChange(healthChange) <= 0)
                    {
                        value -= (hero.attack + 1) * 100; //multiplies hero attack by 100 (will hyper prioritize dead heroes)
                    }
                    //gets stats from hero
                    cardAttack = hero.attack;
                    cardHealth = hero.health;
                }
            }
            else //target is on the opposite team
            {
                //NOTE: For cards on the other team, we want to make the calculated values negative. so that +2 attack has a -2 value. and that -2 health has a +2 value.
                if (minion != null)
                {
                    //changes value
                    value -= minion.CalculateAttackChange(attackChange);
                    value -= minion.CalculateHealthChange(healthChange);
                    //if the spell kills the target
                    if (minion.health + minion.CalculateHealthChange(healthChange) <= 0)
                    {
                        value += minion.CalculateDeathValue();
                    }
                    //gets stats from minion
                    cardAttack = minion.attack;
                    cardHealth = minion.health;
                }
                else if (hero != null && hero.isDead == false)
                {
                    //changes value
                    value -= hero.CalculateAttackChange(attackChange);
                    value -= hero.CalculateHealthChange(healthChange);
                    //if the spell kills the target hero
                    if (hero.health + hero.CalculateHealthChange(healthChange) <= 0)
                    {
                        value += (hero.attack + 1) * 100; //multiplies hero attack by 100 (will hyper prioritize dead heroes)
                    }
                    //gets stats from hero
                    cardAttack = hero.attack;
                    cardHealth = hero.health;
                }
            }

            value -= manaCost; //subtracts mana cost
            value += valueBoostAI; //adds in value boost

            //checks if AI is agressive, spell gives attack, and target is on the same team
            if (ai.playstyle == EnemyPlaystyle.AGGRESSIVE && attackChange > 0 && card.team == playerManager.team)
                value = (int)(value * ValueToPercent(ai.aggroValue));
            //checks if AI is mid range, target is a minion, the stat changes are positive/neutral, and target is on the same team
            if (ai.playstyle == EnemyPlaystyle.MID_RANGE && minion != null && (attackChange + healthChange) > -1 && card.team == playerManager.team)
                value = (int)(value * ValueToPercent(ai.midRangeValue));
            //checks if AI is defensive, target is a hero or taunt minion, health change is positive, and target is on the same team
            if (ai.playstyle == EnemyPlaystyle.DEFENSIVE && (hero != null || minion.taunt == true) && healthChange > 0 && card.team == playerManager.team)
                value = (int)(value * ValueToPercent(ai.defenseValue));


            if (value > entry.value) //if the target has a higher value
            {
                entry.value = value;
                entry.target = card;
            }
            else if (value == entry.value && entry.target != null) //if the target and a previously found best target have an equal value
            {
                if (entry.target.GetComponent<BaseMinion>() == true) //previously found best target is a minion
                {
                    BaseMinion entryMinion = entry.target.GetComponent<BaseMinion>();
                    if(hero != null) //new found target is a hero
                    {
                        entry.value = value;
                        entry.target = card;
                    }
                    else if (entryMinion.health < cardHealth) //new found target has more health
                    {
                        entry.value = value;
                        entry.target = card;
                    }
                    else if (entryMinion.health == cardHealth && entryMinion.attack < cardAttack) //new found target has equal health but more attack
                    {
                        entry.value = value;
                        entry.target = card;
                    }
                }
                else if (entry.target.GetComponent<BaseHero>() == true) //previously found best target is hero
                {
                    BaseHero entryHero = entry.target.GetComponent<BaseHero>();
                    if (entryHero.health < cardHealth) //new found target has more health
                    {
                        entry.value = value;
                        entry.target = card;
                    }
                    else if (entryHero.health == cardHealth && entryHero.attack < cardAttack) //new found target has equal health but more attack
                    {
                        entry.value = value;
                        entry.target = card;
                    }
                }
            }
        }

        return entry;
    }
}
