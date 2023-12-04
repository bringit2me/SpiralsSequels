using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectHeal : BaseEffect
{
    [Header("Heal Amount")]
    public int healValue;
    [Space]
    public bool targetHero;

    public override void TriggerEffect()
    {
        if (minion != null && targetHero == false) //we have a minion reference
        {
            //Add stat change entry ot card. Also sets card effect entry (extra description to show when hovering card)
            minion.visualManager.AddStatChangeEntry(0, false, 0, false, minion.health + minion.CalculateHeal(healValue), true, null);
            triggerAnimCopy.cardVisualsToUpdate.Add(minion); //adds card to updater (updates card visuals after animation)
            //Restores minions health
            minion.Heal(healValue);
        }
        else if (hero != null) //we have a hero reference
        {
            //Add stat change entry ot card. Also sets card effect entry (extra description to show when hovering card)
            hero.visualManager.AddStatChangeEntry(0, false, 0, false, hero.health + hero.CalculateHeal(healValue), true, null);
            triggerAnimCopy.cardVisualsToUpdate.Add(hero); //adds card to updater (updates card visuals after animation)
            ///Restores heroes health
            hero.Heal(healValue);
        }

        base.TriggerEffect();
    }

    public override int CalculateEffectValueAI()
    {
        int value = 0;

        if (minion != null && targetHero == false) //we have a minion reference
        {
            //Restores minions health
            value += minion.CalculateHeal(healValue);
        }
        else if (hero != null) //we have a hero reference
        {
            ///Restores heroes health
            value += hero.CalculateHeal(healValue);
        }

        return value;
    }
}
