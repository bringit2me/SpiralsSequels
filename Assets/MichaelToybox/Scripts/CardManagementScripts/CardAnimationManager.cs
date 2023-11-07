using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardAnimationManager : MonoBehaviour
{
    public bool isAnimating = false;
    public List<CardAnimationClip> animationList;
    [HideInInspector] public CombatManager combatManager;
    [Header("Animation ID")]
    public int animID = 1;

    private void Awake()
    {
        combatManager = GameObject.FindObjectOfType<CombatManager>();
        animID = 1;
    }

    private void Update()
    {
        if (combatManager.state == CombatState.OFF)
        {
            ResetAnimationManager(); //resets animation manager
            return;
        }

        if(animationList.Count > 0)
        {
            isAnimating = true;
            if (animationList[0].animationFinished == true) //if the animation is finished
                animationList.RemoveAt(0); //remove first animation
            else //we are not done animating
            {
                //SINGLE ANIMATION INFO
                if (animationList[0].animating == false) //animation hasnt started yet
                {
                    animationList[0].SetupAnim();
                }
                else //animation is started
                {
                    animationList[0].Execute(); //execute the animation
                }

                //INFO FOR MULTIPLE ANIMATIONS TO HAPPEN AT THE SAME TIME
                bool nextAnimSameID = true;
                int count = 1;
                int currentAnimationID = animationList[0].animID;
                while(nextAnimSameID == true) //while the next animation shares the same ID
                {
                    Debug.Log("anim 1");
                    if (count > animationList.Count - 1) //hit end of the animation list
                    {
                        nextAnimSameID = false;
                        return;
                    }

                    //if the anim id is greater than 0 (ID less than 0 does not have multiple parts)
                    if (currentAnimationID > 0)
                    {
                        if (currentAnimationID == animationList[count].animID)
                        {
                            if (animationList[count].animating == false) //animation hasnt started yet
                            {
                                animationList[count].SetupAnim();
                            }
                            else //animation is started
                            {
                                animationList[count].Execute(); //execute the animation
                            }
                        }
                        else
                        {
                            nextAnimSameID = false;
                        }
                    }
                    else
                    {
                        nextAnimSameID = false;
                    }

                    count++;
                }
            }
        }
        else
        {
            isAnimating = false;
        }
    }

    /// <summary>
    /// Gets animation ID (animID) and increments the animation ID by 1
    /// </summary>
    /// <returns></returns>
    public int GetAnimationID()
    {
        int temp = animID;
        animID += 1;
        return temp;
    }

    public void PlayAnimation(CardAnimationClip animClip)
    {
        animClip.animationFinished = false;
        animClip.animating = false;
        //animClip.SetupAnim();
        animationList.Add(animClip);
    }

    public void ResetAnimationManager()
    {
        animationList.Clear();
    }

    public GameObject InstantiateEffect(GameObject obj, Vector3 pos, Vector3 euler)
    {
        return Instantiate(obj, pos, Quaternion.Euler(euler));
    }

    public void DestroyEffect(GameObject effect)
    {
        Destroy(effect);
    }

    public Vector3 GetLookAtEuler(GameObject start, Vector3 lookPoint)
    {
        return Vector3.zero;
    }


}

[System.Serializable]
public class CardAnimationClip
{
    public int animID = -1;
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
    public bool startTarget;
    public bool startMiddle;
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

    public void CopyClip(CardAnimationClip clip)
    {
        animID = clip.animID;
        card = clip.card;
        target = clip.target;
        targetPos = clip.targetPos;
        moveCard = clip.moveCard;
        moveEffect = clip.moveEffect;
        startHero = clip.startHero;
        startCard = clip.startCard;
        startTarget = clip.startTarget;
        startMiddle = clip.startMiddle;
        lookAtTargetPos = clip.lookAtTargetPos;
        reverseAnimWhenDone = clip.reverseAnimWhenDone;
        animSpeed = clip.animSpeed;
        effect = clip.effect;
    }

    public void SetupAnim()
    {
        anim = GameObject.FindFirstObjectByType<CardAnimationManager>();

        if (target != null) //if we do not have a target pos but do have a target
            targetPos = target.transform.position; //set target pos to targets transform position



        //if we want to move the effect and start at the hero
        if (moveEffect == true && startHero && card.hero != null)
        {
            createdEffect = anim.InstantiateEffect(effect, card.hero.transform.position, Quaternion.identity.eulerAngles);
            createdEffect.transform.SetParent(anim.transform.GetChild(0));
            startPosEffect = createdEffect.transform.position;
        }
        //want to move effect starting at hero but there is no hero (usually if target spell comes from nuetral deck)
        else if (moveEffect == true && startHero && card.hero == null)
        {
            Vector3 startPos = anim.combatManager.GetTargets(card.team, TargetingInfo.SAME_HERO)[0].transform.position;
            createdEffect = anim.InstantiateEffect(effect, startPos, Quaternion.identity.eulerAngles);
            createdEffect.transform.SetParent(anim.transform.GetChild(0));
            startPosEffect = createdEffect.transform.position;
        }
        else if (moveEffect == true && startTarget && target != null)
        {
            Vector3 startPos = anim.combatManager.GetTargets(card.team, TargetingInfo.SAME_HERO)[0].transform.position;
            createdEffect = anim.InstantiateEffect(effect, targetPos, Quaternion.identity.eulerAngles);
            createdEffect.transform.SetParent(anim.transform.GetChild(0));
            startPosEffect = createdEffect.transform.position;
        }
        //if we want to move the effect and start at the card
        else if (moveEffect == true && startCard)
        {
            createdEffect = anim.InstantiateEffect(effect, card.transform.position, Quaternion.identity.eulerAngles);
            startPosEffect = card.transform.position;
        }

        if (card != null) //if we have a card reference
        {
            startPosCard = card.transform.position; //sets card start pos
            startCardEuler = card.transform.eulerAngles; //gets cards starting euler angles (rotation)
        }

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
                AnimationFinished();
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
                AnimationFinished();
            }
            else if (currentTime >= 1)
            {
                reverseAnim = true;
                currentTime = 1;
            }
        }
    }

    public void AnimationFinished()
    {
        animationFinished = true;

        if (createdEffect != null)
            anim.DestroyEffect(createdEffect);
    }
}
