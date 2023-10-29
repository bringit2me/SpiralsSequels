using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseAOESpell : BaseSpell
{
    [Header("Targeting Info")]
    public TargetingInfo targetTeam;
    public List<BaseCard> targets;

    public override void Cast()
    {
        base.Cast();
        targets = combatManager.GetTargets(team, targetTeam);
    }
}
