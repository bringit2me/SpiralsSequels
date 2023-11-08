using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectChangeStats : BaseEffect
{
    [Header("Stat Change")]
    public int attackChange;
    public int healthChange;

    public override void TriggerEffect()
    {
        if(minion != null) //we have a minion reference
        {
            //changes attack
            minion.ChangeAttack(attackChange);
            minion.UpdateAttack();
            //changes health
            minion.ChangeHealth(healthChange);
            minion.UpdateHealth();
        }
        else if (hero != null) //we have a hero reference
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

        if (minion != null) //we have a minion reference
        {
            minion.CalculateAttackChange(attackChange);
            minion.CalculateHealthChange(healthChange);
        }
        else if (hero != null) //we have a hero reference
        {

            hero.CalculateAttackChange(attackChange);
            hero.CalculateHealthChange(healthChange);

        }

        return value;
    }
}
