using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetHealSpell : BaseTargetSpell
{
    [Header("Heal Value")]
    [SerializeField] int heal;

    public override void CastAtTarget()
    {
        //Gets minion reference. if card is not a minion it will be null
        BaseMinion minion = target.GetComponent<BaseMinion>();
        //Gets hero reference. if card is not a hero it will be null
        BaseHero hero = target.GetComponent<BaseHero>();

        int healValue = heal;

        // - Custom Variable Calculation - 
        //if we use the variable in our damage calculation
        if (useVariable == true && combatManager.ReturnVariableLibraryValue(variableName, team) > 0)
        {
            //if we change by an amount per variable
            if (combatManager.ReturnVariableLibraryValue(variableName, team) > 0 && useOnlyOne == false)
            {
                healValue += amountPerVariable * combatManager.ReturnVariableLibraryValue(variableName, team);
            }
            //we use only one
            else
            {
                healValue += amountPerVariable; //increases damage by the variables number
            }
        }

        if (addToVariable == true) //if we increase a variable
            combatManager.ChangeVariableLibrary(variableName, variableIncrease, team);

        if (subtractFromVariable == true) //if we decrease a variable
            combatManager.ChangeVariableLibrary(variableName, variableDecrease, team);
        // - (end) Custom Variable Calculation (end) - 


        if (minion == true)
        {
            //Add stat change entry ot card. Also sets card effect entry (extra description to show when hovering card)
            minion.visualManager.AddStatChangeEntry(0, false, 0, false, minion.health + minion.CalculateHeal(healValue), true, null);
            playAnimCopy.cardVisualsToUpdate.Add(minion); //adds card to updater (updates card visuals after animation)

            //heals minion
            minion.Heal(healValue);

            playAnimCopy.targetPos = target.transform.position;
            anim.PlayAnimation(playAnimCopy);
        }
        if (hero == true)
        {
            //Add stat change entry ot card. Also sets card effect entry (extra description to show when hovering card)
            hero.visualManager.AddStatChangeEntry(0, false, 0, false, hero.health + hero.CalculateHeal(healValue), true, null);
            playAnimCopy.cardVisualsToUpdate.Add(hero); //adds card to updater (updates card visuals after animation)

            //heals hero
            hero.Heal(healValue);

            playAnimCopy.targetPos = target.transform.position;
            anim.PlayAnimation(playAnimCopy);
        }

        base.CastAtTarget();
    }

    public override CardValueEntry CalculateValueAI(BaseEnemyAI ai)
    {
        CardValueEntry entry = new CardValueEntry();
        entry.card = this;
        List<BaseCard> targets = combatManager.GetTargets(team, targetTeam); //gets all potential targets of the spell

        int healValue = heal;

        // - Custom Variable Calculation - 
        //if we use the variable in our damage calculation
        if (useVariable == true && combatManager.ReturnVariableLibraryValue(variableName, team) > 0)
        {
            //if we change by an amount per variable
            if (combatManager.ReturnVariableLibraryValue(variableName, team) > 0 && useOnlyOne == false)
            {
                healValue += amountPerVariable * combatManager.ReturnVariableLibraryValue(variableName, team);
            }
            //we use only one
            else
            {
                healValue += amountPerVariable; //increases damage by the variables number
            }
        }

        if (addToVariable == true) //if we increase a variable
            combatManager.ChangeVariableLibrary(variableName, variableIncrease, team);

        if (subtractFromVariable == true) //if we decrease a variable
            combatManager.ChangeVariableLibrary(variableName, variableDecrease, team);
        // - (end) Custom Variable Calculation (end) - 


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
                if (minion != null)
                {
                    //changes value
                    value = minion.CalculateHeal(healValue);

                    //gets stats from minion
                    cardAttack = minion.attack;
                    cardHealth = minion.health;
                }
                else if (hero != null && hero.isDead == false)
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
                else if (hero != null && hero.isDead == false)
                {
                    //changes value
                    value = -hero.CalculateHeal(healValue);

                    //gets stats from hero
                    cardAttack = hero.attack;
                    cardHealth = hero.health;
                }
            }

            value += valueBoostAI; //adds in value boost
            value += CalculateEffectValues(); //adds in effect values
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
