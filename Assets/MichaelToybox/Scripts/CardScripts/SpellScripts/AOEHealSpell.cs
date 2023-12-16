using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AOEHealSpell : BaseAOESpell
{
    [Header("Heal Value")]
    [SerializeField] int heal;

    public override void Cast()
    {
        base.Cast();

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
            //Gets minion reference. if card is not a minion it will be null
            BaseMinion minion = card.GetComponent<BaseMinion>();
            //Gets hero reference. if card is not a hero it will be null
            BaseHero hero = card.GetComponent<BaseHero>();

            if (minion == true)
            {
                //Add stat change entry ot card. Also sets card effect entry (extra description to show when hovering card)
                minion.visualManager.AddStatChangeEntry(0, false, 0, false, minion.health + minion.CalculateHeal(healValue), true, null);
                playAnimCopy.cardVisualsToUpdate.Add(minion); //adds card to updater (updates card visuals after animation)

                //heals minion
                minion.Heal(healValue);
            }
            else if (hero == true && hero.isDead == false)
            {
                //Add stat change entry ot card. Also sets card effect entry (extra description to show when hovering card)
                hero.visualManager.AddStatChangeEntry(0, false, 0, false, hero.health + hero.CalculateHeal(healValue), true, null);
                playAnimCopy.cardVisualsToUpdate.Add(hero); //adds card to updater (updates card visuals after animation)

                //heals hero
                hero.Heal(healValue);
            }
        }

        if (targets.Count > 0)
        {
            playAnimCopy.target = targets[0].gameObject; //sets target
            anim.PlayAnimation(playAnimCopy); //plays animation
        }

        EndCast();
    }

    public override int CalculateValueAI(BaseEnemyAI ai)
    {
        int value = 0;
        List<BaseCard> targets = combatManager.GetTargets(team, targetTeam); //gets all targets of the spell

        bool effectsFriendlyHero = false;
        bool effectsFriendlyMinions = false;
        bool effectsPlayerHero = false;
        bool effectsPlayerMinions = false;

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
            BaseMinion minion = card.GetComponent<BaseMinion>();
            BaseHero hero = card.GetComponent<BaseHero>();

            if (card.team == ai.team) //if the target is on the same team as the AI
            {
                if (minion == true)
                {
                    value += minion.CalculateHeal(healValue);
                    if(minion.CalculateHeal(healValue) > 0)
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

        value += valueBoostAI; //adds in value boost
        value += CalculateEffectValues(); //adds in effect values
        value -= manaCost; //subtracts mana cost

        //checks if AI is agressive and an enemy hero is effected
        if (ai.playstyle == EnemyPlaystyle.AGGRESSIVE && effectsPlayerHero == true)
            value = value - 2;
        //checks if AI is mid range and a friendly minion is effected
        if (ai.playstyle == EnemyPlaystyle.MID_RANGE && effectsFriendlyMinions == true)
            value = (int)(value * ValueToPercent(ai.midRangeValue));
        //checks if AI is defensive and a friendly hero is effected
        if (ai.playstyle == EnemyPlaystyle.DEFENSIVE && effectsFriendlyHero == true)
            value = (int)(value * ValueToPercent(ai.defenseValue));

        return value;
    }
}
