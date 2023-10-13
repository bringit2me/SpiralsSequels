using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetDamageSpell : BaseTargetSpell
{
    [Header("Damage")]
    [SerializeField] int damage;

    public override void CastAtTarget()
    {
        if(target.GetComponent<BaseMinion>() == true)
        {
            target.GetComponent<BaseMinion>().TakeDamage(damage);
        }
        if(target.GetComponent<BaseHero>() == true)
        {
            target.GetComponent<BaseHero>().TakeDamage(damage);
        }

        base.CastAtTarget();
    }
}
