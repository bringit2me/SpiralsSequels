using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AOEChangeStats : BaseAOESpell
{
    [Header("Stat Change")]
    [SerializeField] int attackChange;
    [SerializeField] int healthChange;

    public override void Cast()
    {
        base.Cast();
        foreach (BaseCard card in targets)
        {
            if (card.GetComponent<BaseMinion>() == true)
            {
                card.GetComponent<BaseMinion>().ChangeAttack(attackChange);
                card.GetComponent<BaseMinion>().ChangeHealth(healthChange);
            }
            else if (card.GetComponent<BaseHero>() == true)
            {
                card.GetComponent<BaseHero>().ChangeAttack(attackChange);
                card.GetComponent<BaseHero>().ChangeHealth(healthChange);
            }
        }
        EndCast();
    }
}
