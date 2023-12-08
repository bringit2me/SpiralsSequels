using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectChangeStats : BaseEffect
{
    [Header("Stat Change")]
    public int attackChange;
    public int healthChange;
    [Space]
    public bool targetHero;

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
        if(minion != null && targetHero == false) //we have a minion reference
        {
            //Add stat change entry to card. Also sets card effect entry (extra description to show when hovering card)
            minion.visualManager.AddStatChangeEntry(minion.attack + minion.CalculateAttackChange(attackChange), minion.health + minion.CalculateHealthChange(healthChange), cardEffectEntry);
            triggerAnimCopy.cardVisualsToUpdate.Add(minion); //adds card to updater (updates card visuals after animation)
            //changes attack
            minion.ChangeAttack(attackChange);
            //changes health
            minion.ChangeHealth(healthChange);
        }
        else if (hero != null) //we have a hero reference
        {
            //Add stat change entry to card. Also sets card effect entry (extra description to show when hovering card)
            hero.visualManager.AddStatChangeEntry(hero.attack + hero.CalculateAttackChange(attackChange), hero.health + hero.CalculateHealthChange(healthChange), cardEffectEntry);
            triggerAnimCopy.cardVisualsToUpdate.Add(hero); //adds card to updater (updates card visuals after animation)
            //changes attack
            hero.ChangeAttack(attackChange);
            //changes health
            hero.ChangeHealth(healthChange);
        }

        base.TriggerEffect();
    }

    public override int CalculateEffectValueAI()
    {
        int value = 0;

        if (minion != null && targetHero == false) //we have a minion reference
        {
            value += minion.CalculateAttackChange(attackChange);
            value += minion.CalculateHealthChange(healthChange);
        }
        else if (hero != null) //we have a hero reference
        {

            value += hero.CalculateAttackChange(attackChange) * 2;
            value += hero.CalculateHealthChange(healthChange);

        }

        return value;
    }
}
