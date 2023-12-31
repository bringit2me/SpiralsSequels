using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseEffect : MonoBehaviour
{
    protected Team team;
    [Header("Animation")]
    public BaseAnimationClip triggerAnimClip;
    protected BaseAnimationClip triggerAnimCopy;
    [Header("AI Value")]
    public int valueBoostAI;
    [Header("Custom Variables")]
    [SerializeField] protected bool useVariable = false;
    [SerializeField] protected bool addToVariable = false;
    [SerializeField] protected bool subtractFromVariable = false;
    [SerializeField] protected bool useOnlyOne = false;
    [Space]
    [SerializeField] protected string variableName;
    [Space]
    [SerializeField] protected int variableIncrease;
    [Space]
    [SerializeField] protected int variableDecrease;
    [Space]
    [SerializeField] protected int amountPerVariable = 1;
    //References
    [SerializeField] protected BaseHero hero;
    [SerializeField] protected BaseMinion minion;
    [SerializeField] protected BaseSpell spell;
    protected CardAnimationManager anim;
    protected CombatManager combatManager;
    [SerializeField] protected PlayerManager playerManager;
    protected CardEffectEntry cardEffectEntry;

    public virtual void Awake()
    {
        //Gets animation manager reference
        anim = GameObject.FindObjectOfType<CardAnimationManager>();
        combatManager = GameObject.FindObjectOfType<CombatManager>();
        if (triggerAnimClip != null)
            triggerAnimCopy = Instantiate(triggerAnimClip);

        SetupEffectEntry();
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

        if(triggerAnimClip != null)
            //calls to setup animation
            SetupAnimation();
    }

    /// <summary>
    /// Sets up animation based on passed in information
    /// </summary>
    public virtual void SetupAnimation()
    {
        if (minion != null)
            triggerAnimCopy.card = minion;
        else if (spell != null)
            triggerAnimCopy.card = spell;
        else if (hero != null)
            triggerAnimCopy.card = hero;
    }

    public virtual void SetupEffectEntry()
    {
        cardEffectEntry = new CardEffectEntry();
        cardEffectEntry.name = this.GetComponentInParent<BaseCard>().name;
        cardEffectEntry.description = "NONE";
    }

    public virtual void TriggerEffect()
    {
        if (triggerAnimClip != null)
            //plays animation
            anim.PlayAnimation(triggerAnimCopy);
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
