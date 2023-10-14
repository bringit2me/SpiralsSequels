using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerMinionZone : MonoBehaviour, IDropHandler
{
    [SerializeField] PlayerManager playerManager;


    void Start()
    {
        playerManager = GameObject.FindObjectOfType<PlayerManager>();
    }

    public void EnableMinionAttack()
    {
        foreach(BaseMinion card in transform.GetComponentsInChildren<BaseMinion>() )
        {
            card.canAttack = true;
        }
    }

    void IDropHandler.OnDrop(PointerEventData eventData)
    {
        Draggable card = eventData.pointerDrag.GetComponent<Draggable>();

        if(card != null && card.GetComponent<BaseMinion>() == true && card.GetComponent<BaseMinion>().isPlayed == false)
        {
            card.parentToReturnTo = this.transform;
            card.GetComponent<BaseMinion>().Played(playerManager);

            //Returns card to parentToReturnTo
            card.transform.SetParent(this.transform);

            //Turns on raycast blocking
            card.GetComponent<CanvasGroup>().blocksRaycasts = true;
        }
    }
}
