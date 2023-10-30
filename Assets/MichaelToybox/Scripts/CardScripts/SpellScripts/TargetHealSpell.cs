using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetHealSpell : BaseTargetSpell
{
    [Header("Heal Value")]
    [SerializeField] int healValue;

    public override void CastAtTarget()
    {
        if (target.GetComponent<BaseMinion>() == true)
        {
            target.GetComponent<BaseMinion>().Heal(healValue);
        }
        if (target.GetComponent<BaseHero>() == true)
        {
            target.GetComponent<BaseHero>().Heal(healValue);
        }

        base.CastAtTarget();
    }

    public override CardValueEntry CalculateValueAI(BaseEnemyAI ai)
    {
        CardValueEntry entry = new CardValueEntry();
        entry.card = this;
        List<BaseCard> targets = combatManager.GetTargets(team, targetTeam); //gets all potential targets of the spell

        foreach (BaseCard card in targets)
        {
            int value = 0;
            //Gets minion reference. if card is not a minion it will be null
            BaseMinion minion = card.GetComponent<BaseMinion>();
            //Gets hero reference. if card is not a hero it will be null
            BaseHero hero = card.GetComponent<BaseHero>();

            int cardAttack = 0;
            int cardHealth = 0;

            if (card.team == ai.team) //if the target is on the same team as the AI
            {
                if (minion != null)
                {
                    //changes value
                    value = minion.CalculateHeal(healValue);

                    //gets stats from minion
                    cardAttack = minion.attack;
                    cardHealth = minion.health;
                }
                else if (hero != null)
                {
                    //changes value
                    value = hero.CalculateHeal(healValue);

                    //gets stats from hero
                    cardAttack = hero.attack;
                    cardHealth = hero.health;
                }
            }
            //NOTE: For cards on the opposite team as the AI we use negative numbers because we do not want the AI to heal enemy units
            else //target is on the opposite team
            {
                if (minion != null)
                {
                    //changes value
                    value = -minion.CalculateHeal(healValue);

                    //gets stats from minion
                    cardAttack = minion.attack;
                    cardHealth = minion.health;
                }
                else if (hero != null)
                {
                    //changes value
                    value = -hero.CalculateHeal(healValue);

                    //gets stats from hero
                    cardAttack = hero.attack;
                    cardHealth = hero.health;
                }
            }

            value += valueBoostAI; //adds in value boost
            value -= manaCost; //subtracts mana cost

            //checks if AI is mid range and the target is a minion
            if (ai.playstyle == EnemyPlaystyle.MID_RANGE && minion != null)
                value = (int)(value * ValueToPercent(ai.midRangeValue));
            //checks if AI is defensive and the target is a hero
            if (ai.playstyle == EnemyPlaystyle.DEFENSIVE && hero != null)
                value = (int)(value * ValueToPercent(ai.defenseValue));


            if (value > entry.value) //if the target has a higher value
            {
                entry.value = value;
                entry.target = card;
            }
            //if the target and a previously found best target have an equal value and the entry has a target
            else if (value == entry.value && entry.target != null)
            {
                if (entry.target.GetComponent<BaseMinion>() == true) //previously found best target is a minion
                {
                    BaseMinion entryMinion = entry.target.GetComponent<BaseMinion>();
                    if (entryMinion.attack < cardAttack) //new found target has more attack
                    {
                        entry.value = value;
                        entry.target = card;
                    }
                    else if (entryMinion.attack == cardAttack && entryMinion.health < cardHealth) //new found target has equal attack but more health
                    {
                        entry.value = value;
                        entry.target = card;
                    }
                }
                else if (entry.target.GetComponent<BaseHero>() == true) //previously found best target is hero
                {
                    BaseHero entryHero = entry.target.GetComponent<BaseHero>();
                    if (entryHero.health > cardHealth) //new found target has equal less health
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
