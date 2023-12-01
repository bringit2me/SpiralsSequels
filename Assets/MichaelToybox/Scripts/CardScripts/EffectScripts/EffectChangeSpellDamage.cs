using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectChangeSpellDamage : BaseEffect
{
    [Header("Spell Damage Change")]
    public int spellDamageChange;
    [Space]
    public bool targetHero;

    public override void TriggerEffect()
    {
        if (minion != null && targetHero == false) //we have a minion reference
        {
            //changes spell damage
            minion.ChangeSpellDamage(spellDamageChange);
            combatManager.UpdateSpellDamage(minion.team); //Updates spell damage for the team
        }
        else if (hero != null) //we have a hero reference
        {
            ///changes spell damage
            hero.ChangeSpellDamage(spellDamageChange);
            combatManager.UpdateSpellDamage(hero.team); //Updates spell damage for the team
        }

        base.TriggerEffect();
    }

    public override int CalculateEffectValueAI()
    {
        int value = 0;

        if (minion != null && targetHero == false) //we have a minion reference
        {
            value += minion.CalculateSpellDamageChange(spellDamageChange);
        }
        else if (hero != null) //we have a hero reference
        {
            //3x multiplier for hero spell damage
            value += hero.CalculateSpellDamageChange(spellDamageChange) * 3;

        }

        return value;
    }
}
