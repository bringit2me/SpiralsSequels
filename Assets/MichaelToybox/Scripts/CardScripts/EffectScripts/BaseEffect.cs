using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseEffect : MonoBehaviour
{
    protected Team team;
    [Header("Animation")]
    public CardAnimationClip clip;
    [Header("AI Value")]
    public int valueBoostAI;
    //References
    [SerializeField] protected BaseHero hero;
    [SerializeField] protected BaseMinion minion;
    [SerializeField] protected BaseSpell spell;
    protected CardAnimationManager anim;
    protected CombatManager combatManager;
    protected PlayerManager playerManager;

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
    public virtual void SetupEffect(BaseHero h, BaseMinion m, BaseSpell s, PlayerManager pM)
    {
        hero = h;
        minion = m;
        spell = s;
        playerManager = pM;
        team = playerManager.team;

        //calls to setup animation
        SetupAnimation();
    }

    /// <summary>
    /// Sets up animation based on passed in information
    /// </summary>
    public virtual void SetupAnimation()
    {
        if (minion != null)
            clip.card = spell;
        else if (spell != null)
            clip.card = spell;
        else if (hero != null)
            clip.card = hero;
    }

    public virtual void TriggerEffect()
    {
        //Debug.Log("Triggered Effect: " + this.GetType().Name);

        //TODO: trigger animation
    }

    public virtual int CalculateEffectValueAI()
    {
        return valueBoostAI;
    }

    public float ValueToPercent(float value)
    {
        return 1 + (value / 100);
    }
}
