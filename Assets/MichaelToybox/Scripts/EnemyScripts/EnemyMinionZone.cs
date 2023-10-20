using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMinionZone : PlayerMinionZone
{
    public override bool CheckZoneFull()
    {
        if (combatManager.enemyMinions.Count < 10)
            return true;
        return false;
    }
}
