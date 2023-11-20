using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardAnimationManager : MonoBehaviour
{
    public bool isAnimating = false;
    public List<BaseAnimationClip> animationList;
    [HideInInspector] public CombatManager combatManager;
    //[Header("Animation ID")]
    //public int animID = 1;

    private void Awake()
    {
        combatManager = GameObject.FindObjectOfType<CombatManager>();
        //animID = 1;
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
                    animationList[0].StartAnim();
                }
                else //animation is started
                {
                    animationList[0].Execute(); //execute the animation
                }

                ////INFO FOR MULTIPLE ANIMATIONS TO HAPPEN AT THE SAME TIME
                //bool nextAnimSameID = true;
                //int count = 1;
                //int currentAnimationID = animationList[0].animID;
                //while(nextAnimSameID == true) //while the next animation shares the same ID
                //{
                //    if (count > animationList.Count - 1) //hit end of the animation list
                //    {
                //        nextAnimSameID = false;
                //        return;
                //    }

                //    //if the anim id is greater than 0 (ID less than 0 does not have multiple parts)
                //    if (currentAnimationID > 0)
                //    {
                //        if (currentAnimationID == animationList[count].animID)
                //        {
                //            if (animationList[count].animating == false) //animation hasnt started yet
                //            {
                //                animationList[count].SetupAnim();
                //            }
                //            else //animation is started
                //            {
                //                animationList[count].Execute(); //execute the animation
                //            }
                //        }
                //        else
                //        {
                //            nextAnimSameID = false;
                //        }
                //    }
                //    else
                //    {
                //        nextAnimSameID = false;
                //    }

                //    count++;
                //}
            }
        }
        else
        {
            isAnimating = false;
        }
    }
    /*
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
    */
    public void PlayAnimation(BaseAnimationClip animClip)
    {
        //creates copy
        BaseAnimationClip newClip = Instantiate(animClip);

        newClip.animationFinished = false;
        newClip.animating = false;
        newClip.SetupAnim(); //sets up animation
        animationList.Add(newClip);
    }

    public void ResetAnimationManager()
    {
        animationList.Clear();
    }

    public Vector3 GetLookAtEuler(GameObject start, Vector3 lookPoint)
    {
        return Vector3.zero;
    }
}