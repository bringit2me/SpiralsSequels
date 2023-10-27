using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMinionZone : PlayerMinionZone
{
    public override void Start()
    {
        //gets references
        combatManager = GameObject.FindObjectOfType<CombatManager>();
        playerManager = this.GetComponentInParent<EnemyManager>();
    }

    //changed from player version to check combat manager for enemyMinions instead of playerMinions
    public override bool CheckZoneFull()
    {
        if (combatManager.enemyMinions.Count < maxMinions)
            return true;
        return false;
    }

    public override void RefreshMinionsInZoneList()
    {
        combatManager.enemyMinions.Clear(); //clears minion list

        foreach (BaseMinion card in transform.GetComponentsInChildren<BaseMinion>()) //loops through all child objects of type BaseMinion
        {
            combatManager.enemyMinions.Add(card);
            card.zone = this;
        }
    }
}
