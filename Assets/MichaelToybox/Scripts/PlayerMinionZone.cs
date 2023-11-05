using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PlayerMinionZone : MonoBehaviour, IDropHandler
{
    [SerializeField] protected PlayerManager playerManager;
    protected CombatManager combatManager;
    public GridLayoutGroup layout;
    [Header("Zone Info")]
    [SerializeField] BaseCard dummyCard;
    [SerializeField] protected int maxMinions = 10;



    public virtual void Awake()
    {
        //gets references
        combatManager = GameObject.FindObjectOfType<CombatManager>();
        playerManager = combatManager.playerManager;
        layout = this.GetComponent<GridLayoutGroup>();
    }
    /// <summary>
    /// Resets the minionsInZone list, then updates the minionsInZone list by checking all child objects of type BaseMinion underneath the zone
    /// </summary>
    public virtual void RefreshMinionsInZoneList()
    {
        combatManager.playerMinions.Clear(); //clears minion list

        foreach (BaseMinion card in transform.GetComponentsInChildren<BaseMinion>()) //loops through all child objects of type BaseMinion
        {
            combatManager.playerMinions.Add(card);
            card.zone = this;
        }
    }

    /// <summary>
    /// Enables all attacks of the minions in the minion zone
    /// </summary>
    public virtual void EnableMinionAttacks()
    {
        foreach(BaseMinion card in combatManager.playerMinions)
        {
            card.canAttack = true;
        }
    }

    /// <summary>
    /// Checks if the minion zone is full (true = not full)
    /// If playMinions.count less than maxMinions returns true
    /// </summary>
    /// <returns></returns>
    public virtual bool CheckZoneFull()
    {
        if (combatManager.playerMinions.Count < maxMinions)
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
        //if the card is on the same team as the player
        if(card != null && CheckZoneFull() && card.GetComponent<BaseMinion>() == true && card.GetComponent<BaseMinion>().isPlayed == false && card.GetComponent<BaseMinion>().team == playerManager.team)
        {
            PlayMinionToZone(card);
        }
    }

    public virtual void PlayMinionToZone(Draggable card)
    {
        card.parentToReturnTo = this.transform;
        card.GetComponent<BaseMinion>().Played(playerManager);

        //Returns card to parentToReturnTo
        card.transform.SetParent(this.transform);

        //Turns on raycast blocking
        card.GetComponent<CanvasGroup>().blocksRaycasts = true;

        //updates minion zone list
        RefreshMinionsInZoneList();
        //tells combat manager to update all cards in play
        combatManager.UpdateAllCardsInPlay();
    }
    public virtual void PlayMinionToZone(BaseCard card)
    {
        if (card.GetComponent<BaseMinion>() == false)
        {
            Debug.LogWarning("MICHAEL WARNING: card tried to be played to minion zone, but it is not of type BaseMinion");
            return;
        }

        card.GetComponent<Draggable>().parentToReturnTo = this.transform;
        card.GetComponent<BaseMinion>().Played(playerManager);

        //Returns card to parentToReturnTo
        card.transform.SetParent(this.transform);

        //Turns on raycast blocking
        card.GetComponent<CanvasGroup>().blocksRaycasts = true;

        //updates minion zone list
        RefreshMinionsInZoneList();
        //tells combat manager to update all cards in play
        combatManager.UpdateAllCardsInPlay();
    }

    public virtual Vector3 GetNextCardPosition()
    {
        GameObject target = Instantiate(dummyCard, this.transform).gameObject;
        Canvas.ForceUpdateCanvases();
        target.transform.SetParent(target.transform.root);
        Destroy(target, 2f);
        return target.transform.position;
    }

    public virtual void DummyReparent()
    {

    }
}
