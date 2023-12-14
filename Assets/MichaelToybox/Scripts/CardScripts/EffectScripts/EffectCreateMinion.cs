using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectCreateMinion : BaseEffect
{
    [Header("Minions to Create")]
    [SerializeField] List<MinionCreateEntry> minionsToCreate;

    public override void TriggerEffect()
    {
        base.TriggerEffect();

        BaseCard card = null;

        //Gets card reference
        if (minion != null)
            card = minion;
        else if (hero != null)
            card = hero;
        else if (spell != null)
            card = spell;


        PlayerMinionZone minionZone = combatManager.GetMinionZoneByTeam(card.team);

        foreach (MinionCreateEntry entry in minionsToCreate) //loops through all minions to be summoned
        {
            if (minionZone.CheckZoneFull() == false) //if the minion zone is full
                return; //stop code

            //creates card
            BaseMinion minion = Instantiate(entry.minion, minionZone.transform);
            //calls created on minion (sets references and toggle scripts. bypassed OnPlay effects)
            minion.Created(playerManager);

            //Minion Modification
            minion.ChangeAttack(entry.attackIncrease); //increases attack
            minion.ChangeHealth(entry.healthIncrease); //increases health
            if (entry.canAttackTurnCreated == true) //if we want to make the minion attack
                minion.canAttack = true; //sets minion can attack to true


            //Adds minion to zone
            minionZone.CreateMinionInZone(minion);
            //Refreshes minion list
            minionZone.RefreshMinionsInZoneList();
        }
    }

    public override int CalculateEffectValueAI()
    {

        int value = 0;

        BaseEnemyAI ai = combatManager.enemyAI; //gets AI reference for calculation
        PlayerMinionZone minionZone = combatManager.GetMinionZoneByTeam(Team.ENEMY);

        foreach (MinionCreateEntry entry in minionsToCreate) //loops through all minions to be summoned
        {
            if (minionZone.CheckZoneFull() == false) //if the minion zone is full
                return 0;

            value += entry.minion.CalculateValueAI(ai); //calculates value of summoned card
            value += entry.minion.manaCost; //adds back in the mana cost to the value (mana cost subtracted during value calculation)

            value += entry.minion.CalculateAttackChange(entry.attackIncrease);
            value += entry.minion.CalculateHealthChange(entry.healthIncrease);

            if (entry.canAttackTurnCreated == true)
                value += 1;

        }

        return value;
    }
}
