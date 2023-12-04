using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectAOEChangeStats : BaseEffect
{
    [Header("Stat Change Info")]
    [SerializeField] TargetingInfo targetTeam;
    [SerializeField] int attackChange;
    [SerializeField] int healthChange;

    public override void SetupEffectEntry()
    {
        base.SetupEffectEntry();

        string desc = "";

        if (attackChange > 0)
        {
            desc += "+" + attackChange;
        }
        else if (attackChange < 0)
        {
            desc += "-" + attackChange;
        }

        desc += "attack ";

        if (healthChange > 0)
        {
            desc += "+" + healthChange;
        }
        else if (healthChange < 0)
        {
            desc += "-" + healthChange;
        }

        desc += "health";

        cardEffectEntry.description = desc;
    }

    public override void TriggerEffect()
    {
        //base.TriggerEffect();

        List<BaseCard> targets = combatManager.GetTargets(team, targetTeam); //gets targets

        foreach (BaseCard card in targets)
        {
            if (card == null) //null card reference
                continue; //go to next

            BaseMinion minion = card.GetComponent<BaseMinion>();
            BaseHero hero = card.GetComponent<BaseHero>();

            if (minion == true)
            {
                //Add stat change entry ot card. Also sets card effect entry (extra description to show when hovering card)
                minion.visualManager.AddStatChangeEntry(minion.attack + minion.CalculateAttackChange(attackChange), minion.health + minion.CalculateHealthChange(healthChange), cardEffectEntry);
                triggerAnimCopy.cardVisualsToUpdate.Add(minion); //adds card to updater (updates card visuals after animation)

                minion.ChangeAttack(attackChange);
                minion.ChangeHealth(healthChange);
            }
            else if (hero == true && hero.isDead == false)
            {
                //Add stat change entry ot card. Also sets card effect entry (extra description to show when hovering card)
                hero.visualManager.AddStatChangeEntry(hero.attack + hero.CalculateAttackChange(attackChange), hero.health + hero.CalculateHealthChange(healthChange), cardEffectEntry);
                triggerAnimCopy.cardVisualsToUpdate.Add(hero); //adds card to updater (updates card visuals after animation)

                hero.ChangeAttack(attackChange);
                hero.ChangeHealth(healthChange);
            }
        }

        if (targets.Count > 0 && triggerAnimCopy != null)
        {
            triggerAnimCopy.target = targets[0].gameObject; //sets target
            anim.PlayAnimation(triggerAnimCopy); //plays animation
        }

    }

    public override int CalculateEffectValueAI()
    {
        int value = 0;
        List<BaseCard> targets = combatManager.GetTargets(team, targetTeam); //gets all targets of the spell

        BaseEnemyAI ai = combatManager.enemyAI;

        bool effectsFriendlyHero = false;
        bool effectsFriendlyMinions = false;
        bool effectsPlayerHero = false;
        bool effectsPlayerMinions = false;

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
                    value += minion.CalculateAttackChange(attackChange);
                    value += minion.CalculateHealthChange(healthChange);
                    effectsFriendlyMinions = true;
                }
                else if (hero == true && hero.isDead == false)
                {
                    value += hero.CalculateAttackChange(attackChange) * 2; //2x multiplier for modifying hero attack
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
                    value -= hero.CalculateAttackChange(attackChange) * 3;//3x multiplier for modifying hero attack of player hero
                    value -= hero.CalculateHealthChange(healthChange);
                    effectsPlayerHero = true;
                }
            }
        }

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
