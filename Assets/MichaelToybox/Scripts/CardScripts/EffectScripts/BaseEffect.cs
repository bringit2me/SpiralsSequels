using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseEffect : MonoBehaviour
{
    [Header("Animation")]
    public CardAnimationClip effectClip;
    [Header("AI Value")]
    public int valueBoostAI;
    //References
    protected BaseHero hero;
    protected BaseMinion minion;
    protected BaseSpell spell;
    protected CardAnimationManager anim;
    protected CombatManager combatManager;

    public virtual void Awake()
    {
        //Gets animation manager reference
        anim = GameObject.FindObjectOfType<CardAnimationManager>();
        combatManager = GameObject.FindObjectOfType<CombatManager>();
    }

    /// <summary>
    /// Sets up the base effect, not all references are needed
    /// </summary>
    /// <param name="h"></param>
    /// <param name="m"></param>
    /// <param name="s"></param>
    public virtual void SetupEffect(BaseHero h, BaseMinion m, BaseSpell s)
    {
        hero = h;
        minion = m;
        spell = s;

        //calls to setup animation
        SetupAnimation();
    }

    /// <summary>
    /// Sets up animation based on passed in information
    /// </summary>
    public virtual void SetupAnimation()
    {
        if (minion != null)
            effectClip.card = spell;
        else if (spell != null)
            effectClip.card = spell;
        else if (hero != null)
            effectClip.card = hero;
    }

    public virtual void TriggerEffect()
    {
        Debug.Log("Triggered Effect: " + this.GetType().Name);

        //TODO: trigger animation
    }

    public virtual int CalculateEffectValueAI()
    {
        return valueBoostAI;
    }
}
