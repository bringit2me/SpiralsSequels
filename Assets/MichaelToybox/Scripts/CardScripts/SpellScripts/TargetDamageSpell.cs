using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetDamageSpell : BaseTargetSpell
{
    [Header("Damage")]
    [SerializeField] int damage;

    public override void CastAtTarget()
    {

        int damageValue = damage + playerManager.spellDamage; //increases damage by spell damage

        if (target.GetComponent<BaseMinion>() == true)
        {
            target.GetComponent<BaseMinion>().TakeDamage(damageValue);
            playAnimClip.target = target.gameObject;
            CardAnimationClip clip = new CardAnimationClip();
            clip.CopyClip(playAnimClip);
            anim.PlayAnimation(clip);
        }
        if(target.GetComponent<BaseHero>() == true)
        {
            target.GetComponent<BaseHero>().TakeDamage(damageValue);
            playAnimClip.target = target.gameObject;
            CardAnimationClip clip = new CardAnimationClip();
            clip.CopyClip(playAnimClip);
            anim.PlayAnimation(clip);
        }

        base.CastAtTarget();
    }

    public override CardValueEntry CalculateValueAI(BaseEnemyAI ai)
    {
        CardValueEntry entry = new CardValueEntry();
        entry.card = this;
        List<BaseCard> targets = combatManager.GetTargets(team, targetTeam); //gets all potential targets of the spell

        int damageValue = damage + playerManager.spellDamage; //increases damage by spell damage

        foreach (BaseCard card in targets)
        {
            if (card == null)
                continue;

            int value = 0;
            //Gets minion reference. if card is not a minion it will be null
            BaseMinion minion = card.GetComponent<BaseMinion>();
            //Gets hero reference. if card is not a hero it will be null
            BaseHero hero = card.GetComponent<BaseHero>();

            int cardAttack = 0;
            int cardHealth = 0;

            if (card.team == ai.team) //if the target is on the same team as the AI
            {
                //NOTE: For cards on the same team as the AI we use negative numbers because we do not want the AI to kill its own minions/heroes
                if (minion != null)
                {
                    //changes value
                    value = -minion.CalculateTakeDamage(damageValue);
                    //if the spell kills the target
                    if(minion.CalculateTakeDamage(damageValue) >= minion.health)
                    {
                        value -= minion.CalculateDeathValue();
                    }
                    //gets stats from minion
                    cardAttack = minion.attack;
                    cardHealth = minion.health;
                }
                else if (hero != null && hero.isDead == false)
                {
                    //changes value
                    value = -hero.CalculateTakeDamage(damageValue);
                    //if the spell kills the target
                    if (hero.CalculateTakeDamage(damageValue) >= hero.health)
                    {
                        value -= (hero.attack + 1) * 100; //multiplies hero attack by 100 (will hyper prioritize dead heroes)
                    }
                    //gets stats from hero
                    cardAttack = hero.attack;
                    cardHealth = hero.health;
                }
            }
            else //target is on the opposite team
            {
                if (minion != null)
                {
                    //changes value
                    value = minion.CalculateTakeDamage(damageValue);
                    //if the spell kills the target
                    if (minion.CalculateTakeDamage(damageValue) == minion.health)
                    {
                        value += minion.CalculateDeathValue();
                    }
                    else if (minion.CalculateTakeDamage(damageValue) >= minion.health)
                    {
                        value = minion.maxHealth - minion.health + minion.CalculateDeathValue();
                    }
                    //gets stats from minion
                    cardAttack = minion.attack;
                    cardHealth = minion.health;
                }
                else if (hero != null && hero.isDead == false)
                {
                    //changes value
                    value = hero.CalculateTakeDamage(damageValue);
                    //if the spell kills the target
                    if (hero.CalculateTakeDamage(damageValue) >= hero.health)
                    {
                        value += (hero.attack + 1) * 100; //multiplies hero attack by 100 (will hyper prioritize dead heroes)
                    }
                    //gets stats from hero
                    cardAttack = hero.attack;
                    cardHealth = hero.health;
                }
            }

            value += valueBoostAI; //adds in value boost
            value -= manaCost; //subtracts mana costs

            //checks if AI is agressive and the target is a hero
            if (ai.playstyle == EnemyPlaystyle.AGGRESSIVE && hero != null)
                value = (int)(value * ValueToPercent(ai.aggroValue));
            //checks if AI is mid range and the target is a minion and the minion dies
            if (ai.playstyle == EnemyPlaystyle.MID_RANGE && minion != null && (minion.CalculateTakeDamage(damageValue) >= minion.health))
                value = (int)(value * ValueToPercent(ai.midRangeValue));
            //checks if AI is defensive and the target is a minion
            if (ai.playstyle == EnemyPlaystyle.DEFENSIVE && minion != null)
                value = (int)(value * ValueToPercent(ai.defenseValue));


            if (value > entry.value) //if the target has a higher value
            {
                entry.value = value;
                entry.target = card;
            }
            else if(value == entry.value && entry.target != null) //if the target and a previously found best target have an equal value
            {
                if (entry.target.GetComponent<BaseMinion>() == true) //previously found best target is a minion
                {
                    BaseMinion entryMinion = entry.target.GetComponent<BaseMinion>();
                    if (entryMinion.attack < cardAttack) //new found target has more attack
                    {
                        entry.value = value;
                        entry.target = card;
                    }
                    else if (entryMinion.attack == cardAttack && entryMinion.health > cardHealth) //new found target has equal attack but less health
                    {
                        entry.value = value;
                        entry.target = card;
                    }
                }
                else if (entry.target.GetComponent<BaseHero>() == true) //previously found best target is hero
                {
                    BaseHero entryHero = entry.target.GetComponent<BaseHero>();
                    if (entryHero.attack < cardAttack) //new found target has more attack
                    {
                        entry.value = value;
                        entry.target = card;
                    }
                    else if (entryHero.attack == cardAttack && entryHero.health > cardHealth) //new found target has equal attack but less health
                    {
                        entry.value = value;
                        entry.target = card;
                    }
                }
            }
        }

        return entry;
    }
}
