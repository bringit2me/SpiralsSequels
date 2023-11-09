using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectDamage : BaseEffect
{
    [Header("Damage Amount")]
    public int damage;
    [Space]
    public bool targetHero;

    public override void TriggerEffect()
    {
        if (minion != null && targetHero == false) //we have a minion reference
        {
            //Restores minions health
            minion.TakeDamage(damage);
        }
        else if (hero != null) //we have a hero reference
        {
            ///Restores heroes health
            hero.TakeDamage(damage);
        }

        base.TriggerEffect();
    }

    public override int CalculateEffectValueAI()
    {
        int value = 0;

        if (minion != null && targetHero == false) //we have a minion reference
        {
            //Restores minions health
            value += minion.CalculateTakeDamage(damage);
        }
        else if (hero != null) //we have a hero reference
        {
            ///Restores heroes health
            value += hero.CalculateTakeDamage(damage);
        }

        return value;
    }
}
