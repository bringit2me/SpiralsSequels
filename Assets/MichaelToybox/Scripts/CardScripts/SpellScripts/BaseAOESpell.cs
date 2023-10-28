using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseAOESpell : BaseSpell
{
    [Header("Targeting Info")]
    public TargetingInfo targetTeam;
    public List<BaseCard> targets;

    public override void Cast()
    {
        base.Cast();
        targets = GetTargets();
    }

    /// <summary>
    /// Gets all cards to target based on the targetTeam
    /// </summary>
    /// <returns></returns>
    public virtual List<BaseCard> GetTargets()
    {
        List<BaseCard> targetList = new List<BaseCard>();

        //getting all cards in play
        if (targetTeam == TargetingInfo.ANY_OR_ALL)
        {
            targetList = combatManager.GetAllInPlay();
        }
        //This card is on the player team
        else if (team == Team.PLAYER)
        {
            if (targetTeam == TargetingInfo.SAME)
            {
                targetList = combatManager.GetAllInPlayPlayer();
            }
            else if (targetTeam == TargetingInfo.SAME_HERO)
            {
                foreach (BaseCard card in combatManager.playerHeroes)
                    targetList.Add(card);
            }
            else if (targetTeam == TargetingInfo.SAME_MINION)
            {
                foreach (BaseCard card in combatManager.playerMinions)
                    targetList.Add(card);
            }
            else if (targetTeam == TargetingInfo.OPPOSITE)
            {
                targetList = combatManager.GetAllInPlayEnemy();
            }
            else if (targetTeam == TargetingInfo.OPPOSITE_HERO)
            {
                foreach (BaseCard card in combatManager.enemyHeroes)
                    targetList.Add(card);
            }
            else if (targetTeam == TargetingInfo.OPPOSITE_MINION)
            {
                foreach (BaseCard card in combatManager.enemyMinions)
                    targetList.Add(card);
            }
        }
        //This card is on the enemy team
        else if (team == Team.ENEMY)
        {
            if (targetTeam == TargetingInfo.SAME)
            {
                targetList = combatManager.GetAllInPlayEnemy();
            }
            else if (targetTeam == TargetingInfo.SAME_HERO)
            {
                foreach (BaseCard card in combatManager.enemyHeroes)
                    targetList.Add(card);
            }
            else if (targetTeam == TargetingInfo.SAME_MINION)
            {
                foreach (BaseCard card in combatManager.enemyMinions)
                    targetList.Add(card);
            }
            else if (targetTeam == TargetingInfo.OPPOSITE)
            {
                targetList = combatManager.GetAllInPlayPlayer();
            }
            else if (targetTeam == TargetingInfo.OPPOSITE_HERO)
            {
                foreach (BaseCard card in combatManager.playerHeroes)
                    targetList.Add(card);
            }
            else if (targetTeam == TargetingInfo.OPPOSITE_MINION)
            {
                foreach (BaseCard card in combatManager.playerMinions)
                    targetList.Add(card);
            }
        }

        return targetList;
    }
}
