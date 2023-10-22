using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMinionZone : PlayerMinionZone
{
    //changed from player version to check combat manager for enemyMinions instead of playerMinions
    public override bool CheckZoneFull()
    {
        if (combatManager.enemyMinions.Count < maxMinions)
            return true;
        return false;
    }
}
