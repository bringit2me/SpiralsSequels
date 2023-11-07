using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinionMultiplyStatChange : BaseMinion
{
    [Header("Stat Change Multiplier")]
    public float multiplier = 2;
    public override int CalculateAttackChange(int value)
    {
        if (value > 0)
            value = (int)(value * multiplier);

        return base.CalculateAttackChange(value);
    }

    public override int CalculateHealthChange(int value)
    {
        if (value > 0)
            value = (int)(value * multiplier);

        return base.CalculateHealthChange(value);
    }

    public override int CalculateValueAI(BaseEnemyAI ai)
    {
        int value = base.CalculateValueAI(ai);

        value += (int)(multiplier * 1.5f);

        return value;
    }
}
