using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateMinionSpell : BaseSpell
{
    [Header("Minions to Create")]
    [SerializeField] List<MinionCreateEntry> minionsToCreate;

    public override void Cast()
    {
        base.Cast();

        PlayerMinionZone minionZone = combatManager.GetMinionZoneByTeam(team);

        foreach(MinionCreateEntry entry in minionsToCreate) //loops through all minions to be summoned
        {
            if (minionZone.CheckZoneFull() == false) //if the minion zone is full
                break; //stop code
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

        EndCast();
    }

    public override int CalculateValueAI(BaseEnemyAI ai)
    {
        int value = 0;

        PlayerMinionZone minionZone = combatManager.GetMinionZoneByTeam(team);

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

        value += valueBoostAI; //adds in value boost
        value += CalculateEffectValues(); //adds in effect values
        value -= manaCost; //subtracts mana cost

        return value;
    }
}

[System.Serializable]
public class MinionCreateEntry
{
    public BaseMinion minion;
    [Header("Minion Modification")]
    public int attackIncrease = 0;
    public int healthIncrease = 0;
    public bool canAttackTurnCreated = false;

}
