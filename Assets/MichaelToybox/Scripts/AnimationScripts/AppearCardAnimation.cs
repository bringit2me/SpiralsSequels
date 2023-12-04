using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "APPEAR CARD ANIMATION", menuName = "AnimationClip/AppearCardAnimation", order = 0)]
public class AppearCardAnimation : BaseAnimationClip
{
    public GameObject effect;

    Vector3 startPosEffect;
    GameObject createdEffect;

    public override void SetupAnim()
    {
        base.SetupAnim();
    }

    public override void StartAnim()
    {
        base.StartAnim();
        if(target == null)
            createdEffect = Instantiate(effect, card.transform.position, Quaternion.identity);
        else if (target != null)
            createdEffect = Instantiate(effect, target.transform.position, Quaternion.identity);
    }

    public override void Execute()
    {

        currentTime += Time.deltaTime / animationTime;

        if (currentTime > 1)
        {
            AnimationFinished();
        }
    }

    public override void AnimationFinished()
    {
        base.AnimationFinished();
        Destroy(createdEffect);
    }
}
