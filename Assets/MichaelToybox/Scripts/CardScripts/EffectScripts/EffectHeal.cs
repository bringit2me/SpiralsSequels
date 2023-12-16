using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectHeal : BaseEffect
{
    [Header("Heal Amount")]
    public int heal;
    [Space]
    public bool targetHero;

    public override void TriggerEffect()
    {
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

        if (minion != null && targetHero == false) //we have a minion reference
        {
            //Add stat change entry ot card. Also sets card effect entry (extra description to show when hovering card)
            minion.visualManager.AddStatChangeEntry(0, false, 0, false, minion.health + minion.CalculateHeal(healValue), true, null);
            triggerAnimCopy.cardVisualsToUpdate.Add(minion); //adds card to updater (updates card visuals after animation)
            //Restores minions health
            minion.Heal(healValue);
        }
        else if (hero != null) //we have a hero reference
        {
            //Add stat change entry ot card. Also sets card effect entry (extra description to show when hovering card)
            hero.visualManager.AddStatChangeEntry(0, false, 0, false, hero.health + hero.CalculateHeal(healValue), true, null);
            triggerAnimCopy.cardVisualsToUpdate.Add(hero); //adds card to updater (updates card visuals after animation)
            ///Restores heroes health
            hero.Heal(healValue);
        }

        base.TriggerEffect();
    }

    public override int CalculateEffectValueAI()
    {
        int value = 0;

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

        if (minion != null && targetHero == false) //we have a minion reference
        {
            //Restores minions health
            value += minion.CalculateHeal(healValue);
        }
        else if (hero != null) //we have a hero reference
        {
            ///Restores heroes health
            value += hero.CalculateHeal(healValue);
        }

        return value;
    }
}
