using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AOEHealSpell : BaseAOESpell
{
    [Header("Heal Value")]
    [SerializeField] int healValue;

    public override void Cast()
    {
        base.Cast();
        foreach (BaseCard card in targets)
        {
            if (card.GetComponent<BaseMinion>() == true)
            {
                card.GetComponent<BaseMinion>().Heal(healValue);
            }
            else if (card.GetComponent<BaseHero>() == true)
            {
                card.GetComponent<BaseHero>().Heal(healValue);
            }
        }
        EndCast();
    }
}
