using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardAnimationManager : MonoBehaviour
{
    public bool isAnimating = false;
    public List<CardAnimationClip> animationList;

    private void Update()
    {
        if(animationList.Count > 0)
        {
            isAnimating = true;
            if (animationList[0].animationFinished == true) //if the animation is finished
                animationList.RemoveAt(0); //remove first animation
            else //we are not done animating
                animationList[0].Execute(); //execute the animation
        }
        else
        {
            isAnimating = false;
        }
    }

    public void PlayAnimation(CardAnimationClip animClip)
    {
        animClip.animationFinished = false;
        animClip.animating = false;
        animClip.SetupAnim();
        animationList.Add(animClip);
    }

    public GameObject InstantiateEffect(GameObject obj, Vector3 pos, Vector3 euler)
    {
        return Instantiate(obj, pos, Quaternion.Euler(euler));
    }

    public Vector3 GetLookAtEuler(GameObject start, Vector3 lookPoint)
    {
        return Vector3.zero;
    }
}

[System.Serializable]
public class CardAnimationClip
{
    public bool animating;
    public bool animationFinished = false;
    public BaseCard card;
    public GameObject target;
    public Vector3 targetPos;
    [Header("Anim Parameters")]
    public bool moveCard; //moves card
    public bool moveEffect; //moves effect
    public bool startHero; //effect starts at hero
    public bool startCard; //effect starts at card
    public bool lookAtTargetPos;
    public bool reverseAnimWhenDone = false; //reverses animation when it is finishes
    [Header("Effect Info")]
    public float animSpeed = 1f;
    public GameObject effect;

    Vector3 startPosEffect;
    Vector3 startPosCard;
    Vector3 startCardEuler;
    GameObject createdEffect;
    float currentTime;
    bool reverseAnim;

    CardAnimationManager anim;

    public void SetupAnim()
    {
        anim = GameObject.FindFirstObjectByType<CardAnimationManager>();

        if (target != null) //if we do not have a target pos but do have a target
            targetPos = target.transform.position; //set target pos to targets transform position



        //if we want to move the effect and start at the hero
        if (moveEffect == true && startHero && card.hero != null)
        {
            createdEffect = anim.InstantiateEffect(effect, card.hero.transform.position, Quaternion.identity.eulerAngles);
            startPosEffect = createdEffect.transform.position;
        }
        //if we want to move the effect and start at the card
        else if (moveEffect == true && startCard)
        {
            createdEffect = anim.InstantiateEffect(effect, card.transform.position, Quaternion.identity.eulerAngles);
            startPosEffect = createdEffect.transform.position;
        }

        startPosCard = card.transform.position; //sets card start pos
        startCardEuler = card.transform.eulerAngles; //gets cards starting euler angles (rotation)

        if (lookAtTargetPos == true)
        {
            card.transform.eulerAngles = anim.GetLookAtEuler(card.gameObject, targetPos);

            if (createdEffect != null)
                createdEffect.GetComponent<RectTransform>().LookAt(target.transform);
        }

        reverseAnim = false;
        animating = true;
    }

    public void Execute()
    {
        if (animating == false) //if we are not animating
            return; //end code

        if (reverseAnim == true)
        {
            currentTime -= Time.deltaTime * animSpeed;
            if (moveCard == true)
            {
                card.transform.position = Vector3.Lerp(startPosCard, targetPos, currentTime);
            }
            if (createdEffect != null)
            {
                createdEffect.transform.position = Vector3.Lerp(startPosEffect, targetPos, currentTime);
            }
            if (currentTime <= 0)
            {
                animationFinished = true;
            }
        }
        else if (animationFinished == false)
        {
            currentTime += Time.deltaTime * animSpeed;

            if(moveCard == true)
            {
                card.transform.position = Vector3.Lerp(startPosCard, targetPos, currentTime);
            }
            if(createdEffect != null)
            {
                createdEffect.transform.position = Vector3.Lerp(startPosEffect, targetPos, currentTime);
            }
            if(currentTime >= 1 && reverseAnimWhenDone == false)
            {
                animationFinished = true;
            }
            else if (currentTime >= 1)
            {
                reverseAnim = true;
                currentTime = 1;
            }
        }
    }
}
