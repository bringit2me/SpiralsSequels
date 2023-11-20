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
    public float animationTime = 1f;

    CardAnimationManager anim;
    protected float currentTime;

    public virtual void SetupAnim()
    {
        anim = GameObject.FindFirstObjectByType<CardAnimationManager>(); //gets animation manager
    }

    public virtual void StartAnim()
    {
        currentTime = 0;
        animating = true;
    }

    public virtual void Execute()
    {

    }

    public virtual void AnimationFinished()
    {
        animating = false;
        animationFinished = true;
    }
}
