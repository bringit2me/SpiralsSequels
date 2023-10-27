using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseTargetSpell : BaseSpell
{
    [Header("Targeting Info")]
    public GameObject target;
    public TargetingInfo targetTeam;

    public override void ExecuteCast()
    {
        base.ExecuteCast();

        //gets clicked target from the playerManager
        target = playerManager.GetClickTarget(targetTeam,team);

        if(target != null) //if we have a target
        {
            CastAtTarget(); //end cast
        }
    }

    /// <summary>
    /// The effect that happens when the spell has a target
    /// </summary>
    public virtual void CastAtTarget()
    {
        //WHEN INHERITING. THIS IS WHERE YOU DO STUFF WITH THE TARGET (Examples: damage target, heal target, give target stats)

        EndCast();
    }
}