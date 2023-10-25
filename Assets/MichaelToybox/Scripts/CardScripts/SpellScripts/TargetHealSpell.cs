using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetHealSpell : BaseTargetSpell
{
    [Header("Heal Value")]
    [SerializeField] int healValue;

    public override void CastAtTarget()
    {
        if (target.GetComponent<BaseMinion>() == true)
        {
            target.GetComponent<BaseMinion>().Heal(healValue);
        }
        if (target.GetComponent<BaseHero>() == true)
        {
            target.GetComponent<BaseHero>().Heal(healValue);
        }

        base.CastAtTarget();
    }
}
