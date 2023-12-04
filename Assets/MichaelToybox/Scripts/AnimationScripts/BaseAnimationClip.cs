using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[CreateAssetMenu(fileName = "Base Card Animation", menuName = "AnimationClip/BaseAnimation", order = -1)]
public class BaseAnimationClip : ScriptableObject
{
    public bool animating; 
    public bool animationFinished = false; 
    public BaseCard card; 
    public GameObject target;
    public Vector3 targetPos;
    [Header("Animation Info")]
    public List<BaseCard> cardVisualsToUpdate;
    public float animationTime = 1f;

    CardAnimationManager anim;
    protected float currentTime;

    public virtual void SetupAnim()
    {
        anim = GameObject.FindFirstObjectByType<CardAnimationManager>(); //gets animation manager
    }

    public virtual void StartAnim()
    {
        animating = true;
    }

    public virtual void Execute()
    {

    }

    public virtual void AnimationFinished()
    {
        animating = false;
        animationFinished = true;
        //updates visuals of all cards to update
        UpdateTargetVisuals();
    }

    public virtual void UpdateTargetVisuals()
    {

        //updates visuals of all cards to update
        foreach (BaseCard card in cardVisualsToUpdate)
        {
            if (card != null || card.visualManager != null)
                card.visualManager.UpdateStatChanges();
            else
                Debug.Log("No Card Ref");
        }

        if (cardVisualsToUpdate.Count < 0)
            Debug.Log("No cards to update");
    }
}
