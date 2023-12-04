using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ATTACK ANIMATION", menuName = "AnimationClip/AttackAnimation", order = 0)]
public class AttackAnimation : BaseAnimationClip
{
    Vector3 startPosCard;
    bool updatedTarget = false;

    public override void SetupAnim()
    {
        base.SetupAnim();
        startPosCard = card.transform.position;
        targetPos = target.transform.position;
        updatedTarget = false;
        currentTime = 0;
    }

    public override void Execute()
    {
        currentTime += Time.deltaTime / animationTime;

        if (currentTime < 1)
        {
            card.transform.position = Vector3.Lerp(startPosCard, targetPos, currentTime);
        }
        else if (currentTime < 2)
        {
            if (updatedTarget == false)
            {
                UpdateTargetVisuals();
                updatedTarget = true;
            }
            card.transform.position = Vector3.Lerp(targetPos, startPosCard, currentTime - 1);
        }
        else
        {
            card.transform.position = startPosCard;
            this.AnimationFinished();
        }
    }

    public override void AnimationFinished()
    {
        animating = false;
        animationFinished = true;
    }

    public override void UpdateTargetVisuals()
    {
        base.UpdateTargetVisuals();
        Debug.Log("Updating Visuals");
    }
}
