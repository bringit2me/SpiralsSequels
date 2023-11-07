using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectChangeHeroStats : BaseEffect
{
    [Header("Stat Change")]
    public int attackChange;
    public int healthChange;

    public override void TriggerEffect()
    {
        if (hero != null) //we have a hero reference
        {
            //changes attack
            hero.ChangeAttack(attackChange);
            hero.UpdateAttack();
            //changes health
            hero.ChangeHealth(healthChange);
            hero.UpdateHealth();
        }

        base.TriggerEffect();
    }

    public override int CalculateEffectValueAI()
    {
        int value = 0;

        value += attackChange * 2;
        value += healthChange;

        return value;
    }
}
