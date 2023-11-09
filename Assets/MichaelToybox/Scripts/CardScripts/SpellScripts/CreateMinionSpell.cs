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

            Debug.Log("Created Minion");
        }

        EndCast();
    }

    public override int CalculateValueAI(BaseEnemyAI ai)
    {
        int value = 0;

        foreach (MinionCreateEntry entry in minionsToCreate) //loops through all minions to be summoned
        {
            value += entry.minion.CalculateValueAI(ai);

            value += entry.minion.CalculateAttackChange(entry.attackIncrease);
            value += entry.minion.CalculateHealthChange(entry.healthIncrease);

            if (entry.canAttackTurnCreated == true)
                value += 1;

        }

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
