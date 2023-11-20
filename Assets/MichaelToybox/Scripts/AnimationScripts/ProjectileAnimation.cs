using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PROJECTILE ANIMATION", menuName = "AnimationClip/ProjectileAnimation", order = 0)]
public class ProjectileAnimation : BaseAnimationClip
{
    public bool startHero = true;
    public GameObject effect;

    Vector3 startPosEffect;
    GameObject createdEffect;

    public override void SetupAnim()
    {
        base.SetupAnim();

        if (targetPos == null)
            targetPos = target.transform.position;

        if (startHero == true && card.hero != null)
        {
            startPosEffect = card.hero.transform.position;
        }
        else
        {
            startPosEffect = card.transform.position;
        }
    }

    public override void StartAnim()
    {
        base.StartAnim();
        createdEffect = Instantiate(effect, startPosEffect, Quaternion.identity);
    }

    public override void Execute()
    {

        currentTime += Time.deltaTime / animationTime;

        if(currentTime <= 1)
        {
            createdEffect.transform.position = Vector3.Lerp(startPosEffect, targetPos, currentTime);
        }
        else
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
