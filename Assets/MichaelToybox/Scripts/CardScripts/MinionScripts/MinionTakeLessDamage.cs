using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinionTakeLessDamage : BaseMinion
{
    [Header("Damage Reduction")]
    public int damageReduction = 1;
    public int minimumValue = 1;

    public override int CalculateTakeDamage(int value)
    {
        value = Mathf.Clamp(value - damageReduction, minimumValue, value);

        return base.CalculateTakeDamage(value);
    }

    public override int CalculateValueAI(BaseEnemyAI ai)
    {
        int value = base.CalculateValueAI(ai);

        value += damageReduction;

        return value;
    }
}
