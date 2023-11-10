using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectDamage : BaseEffect
{
    [Header("Damage Amount")]
    public int damage;
    [SerializeField] bool improvedBySpellDamage;
    [Space]
    public bool targetHero;

    public override void TriggerEffect()
    {
        int damageValue = damage;
        if (improvedBySpellDamage == true) //if this effect is 
            damageValue = damage + playerManager.spellDamage; //adds in spell damage

        if (minion != null && targetHero == false) //we have a minion reference
        {
            //Restores minions health
            minion.TakeDamage(damageValue);
        }
        else if (hero != null) //we have a hero reference
        {
            ///Restores heroes health
            hero.TakeDamage(damageValue);
        }

        base.TriggerEffect();
    }

    public override int CalculateEffectValueAI()
    {
        int value = 0;

        int damageValue = damage;
        if (improvedBySpellDamage == true) //if this effect is 
            damageValue = damage + playerManager.spellDamage;

        if (minion != null && targetHero == false) //we have a minion reference
        {
            //Restores minions health
            value += minion.CalculateTakeDamage(damageValue);
        }
        else if (hero != null) //we have a hero reference
        {
            ///Restores heroes health
            value += hero.CalculateTakeDamage(damageValue);
        }

        return value;
    }
}
