using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : PlayerManager
{
    public override void Awake()
    {
        //gets references in the scene
        combatManager = GameObject.FindObjectOfType<CombatManager>();
        handManager = this.GetComponent<HandManager>();
        minionZone = this.GetComponentInChildren<EnemyMinionZone>();
    }
}
