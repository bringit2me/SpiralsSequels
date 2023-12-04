using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectAOEDamage : BaseEffect
{
    [Header("Damage Info")]
    [SerializeField] TargetingInfo targetTeam;
    [SerializeField] bool improvedBySpellDamage;
    [SerializeField] int damage;

    public override void TriggerEffect()
    {
        //base.TriggerEffect();

        List<BaseCard> targets = combatManager.GetTargets(playerManager.team, targetTeam); //gets targets

        int damageValue = damage;
        if (improvedBySpellDamage == true) //if this effect is 
            damageValue = damage + playerManager.spellDamage; //adds in spell damage

        foreach (BaseCard card in targets)
        {

            BaseMinion minion = card.GetComponent<BaseMinion>();
            BaseHero hero = card.GetComponent<BaseHero>();

            if (minion == true)
            {
                //Add stat change entry ot card. Also sets card effect entry (extra description to show when hovering card)
                minion.visualManager.AddStatChangeEntry(0, false, 0, false, minion.health - minion.CalculateTakeDamage(damage), true, null);
                triggerAnimCopy.cardVisualsToUpdate.Add(minion); //adds card to updater (updates card visuals after animation)

                minion.TakeDamage(damageValue);
            }
            else if (hero == true && hero.isDead == false)
            {
                //Add stat change entry ot card. Also sets card effect entry (extra description to show when hovering card)
                hero.visualManager.AddStatChangeEntry(0, false, 0, false, hero.health - hero.CalculateTakeDamage(damage), true, null);
                triggerAnimCopy.cardVisualsToUpdate.Add(hero); //adds card to updater (updates card visuals after animation)

                hero.TakeDamage(damageValue);
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

        int damageValue = damage;
        if (improvedBySpellDamage == true) //if this effect is 
            damageValue = damage + playerManager.spellDamage;

        foreach (BaseCard card in targets)
        {
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

        return value;
    }
}
