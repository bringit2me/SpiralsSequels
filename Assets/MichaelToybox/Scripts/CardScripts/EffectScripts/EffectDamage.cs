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
            //Add stat change entry ot card. Also sets card effect entry (extra description to show when hovering card)
            minion.visualManager.AddStatChangeEntry(0, false, 0, false, minion.health - minion.CalculateTakeDamage(damage), true, null);
            triggerAnimCopy.cardVisualsToUpdate.Add(minion); //adds card to updater (updates card visuals after animation)

            //Restores minions health
            minion.TakeDamage(damageValue);
        }
        else if (hero != null) //we have a hero reference
        {
            //Add stat change entry ot card. Also sets card effect entry (extra description to show when hovering card)
            hero.visualManager.AddStatChangeEntry(0, false, 0, false, hero.health - hero.CalculateTakeDamage(damage), true, null);
            triggerAnimCopy.cardVisualsToUpdate.Add(hero); //adds card to updater (updates card visuals after animation)

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
            if(minion.team == this.team) //same team
                value -= minion.CalculateTakeDamage(damageValue); //subtract
            else //other team
                value += minion.CalculateTakeDamage(damageValue); //add
            
        }
        else if (hero != null) //we have a hero reference
        {
            if (hero.team == this.team) //same team
                value -= hero.CalculateTakeDamage(damageValue); //subtract
            else //other team
                value += hero.CalculateTakeDamage(damageValue); //add
        }

        return value;
    }
}
