using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetChangeStats : BaseTargetSpell
{
    [Header("Damage")]
    [SerializeField] int attackChange;
    [SerializeField] int healthChange;

    public override void CastAtTarget()
    {
        if (target.GetComponent<BaseMinion>() == true)
        {
            target.GetComponent<BaseMinion>().ChangeAttack(attackChange);
            target.GetComponent<BaseMinion>().ChangeHealth(healthChange);
        }
        if (target.GetComponent<BaseHero>() == true)
        {
            target.GetComponent<BaseHero>().ChangeAttack(attackChange);
            target.GetComponent<BaseHero>().ChangeHealth(healthChange);
        }

        base.CastAtTarget();
    }
}
