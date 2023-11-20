using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MOVE CARD ANIMATION", menuName = "AnimationClip/MoveCardAnimation", order = 0)]
public class MoveCardAnimation : BaseAnimationClip
{
    Vector3 startPosCard;

    public override void SetupAnim()
    {
        startPosCard = card.transform.position;
        base.SetupAnim();

        if (targetPos == null)
            targetPos = target.transform.position;
    }

    public override void Execute()
    {
        currentTime += Time.deltaTime / animationTime;

        if (currentTime <= 1)
        {
            card.transform.position = Vector3.Lerp(startPosCard, targetPos, currentTime);
        }
        else
        {
            AnimationFinished();
        }
    }
}
