using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectAOEHeal : BaseEffect
{
    Team team;
    [Header("Heal Info")]
    [SerializeField] TargetingInfo targetTeam;
    [SerializeField] int healValue;

    public override void SetupEffect(BaseHero h, BaseMinion m, BaseSpell s)
    {
        base.SetupEffect(h, m, s);

        //Gets team
        if (minion != null)
            team = minion.team;
        else if (hero != null)
            team = hero.team;
        else if (spell != null)
            team = spell.team;
    }

    public override void TriggerEffect()
    {
        base.TriggerEffect();

        List<BaseCard> targets = combatManager.GetTargets(team, targetTeam); //gets targets

        int animID = anim.GetAnimationID(); //gets ID for animation

        foreach (BaseCard card in targets)
        {
            bool cardEffected = false;

            if (card.GetComponent<BaseMinion>() == true)
            {
                card.GetComponent<BaseMinion>().Heal(healValue);
                cardEffected = true;
            }
            else if (card.GetComponent<BaseHero>() == true && card.GetComponent<BaseHero>().isDead == false)
            {
                card.GetComponent<BaseHero>().Heal(healValue);
                cardEffected = true;
            }

            if (cardEffected == true)
            {
                //Calls animation on target
                //playAnimClip.target = card.gameObject; //sets target
                //playAnimClip.animID = animID; //sets anim ID
                //CardAnimationClip clip = new CardAnimationClip(); //creates new clip
                //clip.CopyClip(playAnimClip); //copies play clip to new clip
                //anim.PlayAnimation(clip); //plays new clip
            }
        }
    }

    public override int CalculateEffectValueAI()
    {
        int value = 0;
        List<BaseCard> targets = combatManager.GetTargets(team, targetTeam); //gets all targets of the spell

        BaseEnemyAI ai = combatManager.enemyAI;

        bool effectsFriendlyHero = false;
        bool effectsFriendlyMinions = false;
        bool effectsPlayerHero = false;
        bool effectsPlayerMinions = false;

        foreach (BaseCard card in targets)
        {
            BaseMinion minion = card.GetComponent<BaseMinion>();
            BaseHero hero = card.GetComponent<BaseHero>();

            if (card.team == ai.team) //if the target is on the same team as the AI
            {
                if (minion == true)
                {
                    value += minion.CalculateHeal(healValue);
                    if (minion.CalculateHeal(healValue) > 0)
                        effectsFriendlyMinions = true;
                }
                else if (hero == true && hero.isDead == false)
                {
                    value += hero.CalculateHeal(healValue);
                    if (hero.CalculateHeal(healValue) > 0)
                        effectsFriendlyHero = true;
                }
            }
            else //target is on the opposite team
            {
                if (minion == true)
                {
                    value -= minion.CalculateHeal(healValue);
                    if (minion.CalculateHeal(healValue) > 0)
                        effectsPlayerMinions = true;
                }
                else if (hero == true && hero.isDead == false)
                {
                    value -= hero.CalculateHeal(healValue);
                    if (hero.CalculateHeal(healValue) > 0)
                        effectsPlayerHero = true;
                }
            }


        }

        //checks if AI is agressive and an enemy hero is effected
        if (ai.playstyle == EnemyPlaystyle.AGGRESSIVE && effectsPlayerHero == true)
            value = value - 2;
        //checks if AI is mid range and a friendly minion is effected
        if (ai.playstyle == EnemyPlaystyle.MID_RANGE && effectsFriendlyMinions == true)
            value = (int)(value * ValueToPercent(ai.midRangeValue));
        //checks if AI is defensive and a friendly hero is effected
        if (ai.playstyle == EnemyPlaystyle.DEFENSIVE && effectsFriendlyHero == true)
            value = (int)(value * ValueToPercent(ai.defenseValue));

        value += valueBoostAI; //adds in value boost


        return value;
    }
}
