using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ATTACK ANIMATION", menuName = "AnimationClip/AttackAnimation", order = 0)]
public class AttackAnimation : BaseAnimationClip
{
    Vector3 startPosCard;

    public override void SetupAnim()
    {
        base.SetupAnim();
        startPosCard = card.transform.position;
        targetPos = target.transform.position;
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
            card.transform.position = Vector3.Lerp(targetPos, startPosCard, currentTime - 1);
        }
        else
        {
            card.transform.position = startPosCard;
            AnimationFinished();
        }
    }
}
