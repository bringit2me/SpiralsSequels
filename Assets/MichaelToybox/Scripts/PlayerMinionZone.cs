using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerMinionZone : MonoBehaviour, IDropHandler
{
    [SerializeField] PlayerManager playerManager;
    protected CombatManager combatManager;



    void Start()
    {
        combatManager = GameObject.FindObjectOfType<CombatManager>();
        playerManager = combatManager.playerManager;
    }
    /// <summary>
    /// Resets the minionsInZone list, then updates the minionsInZone list by checking all child objects of type BaseMinion underneath the zone
    /// </summary>
    public void RefreshMinionsInZoneList()
    {
        combatManager.playerMinions.Clear();

        foreach (BaseMinion card in transform.GetComponentsInChildren<BaseMinion>())
        {
            combatManager.playerMinions.Add(card);
            card.zone = this;
        }
    }

    /// <summary>
    /// Enables all attacks of the minions in the minion zone
    /// </summary>
    public void EnableMinionAttacks()
    {
        foreach(BaseMinion card in combatManager.playerMinions)
        {
            card.canAttack = true;
        }
    }

    public virtual bool CheckZoneFull()
    {
        if (combatManager.playerMinions.Count < 10)
            return true;
        return false;
    }

    void IDropHandler.OnDrop(PointerEventData eventData)
    {
        Draggable card = eventData.pointerDrag.GetComponent<Draggable>();

        //if we dragged a card
        //the minion zone is not full (more than 10 cards)
        //if the card is a minion
        //if the card is not already played
        if(card != null && CheckZoneFull() && card.GetComponent<BaseMinion>() == true && card.GetComponent<BaseMinion>().isPlayed == false)
        {
            card.parentToReturnTo = this.transform;
            card.GetComponent<BaseMinion>().Played(playerManager);

            //Returns card to parentToReturnTo
            card.transform.SetParent(this.transform);

            //Turns on raycast blocking
            card.GetComponent<CanvasGroup>().blocksRaycasts = true;

            //updates minion zone list
            RefreshMinionsInZoneList();
        }
    }
}
