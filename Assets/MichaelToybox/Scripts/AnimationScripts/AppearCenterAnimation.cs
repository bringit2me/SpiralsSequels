using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "APPEAR CENTER ANIMATION", menuName = "AnimationClip/AppearCenterAnimation", order = 0)]
public class AppearCenterAnimation : BaseAnimationClip
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
        createdEffect = Instantiate(effect, new Vector3(960, 540, 0), Quaternion.identity);

        //Flipping the effect based on if it is hitting a player or enemy card
        if(target != null && target.GetComponent<BaseCard>() == true && target.GetComponent<BaseCard>().team == Team.PLAYER)
        {
            createdEffect.transform.eulerAngles = new Vector3(-180, 0, 0);
        }
        else
        {
            createdEffect.transform.eulerAngles = new Vector3(0, 0, 0);
        }
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
