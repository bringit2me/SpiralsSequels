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

    void IDropHandler.OnDrop(PointerEventData eventData)
    {
        Draggable card = eventData.pointerDrag.GetComponent<Draggable>();

        if(card.GetComponent<BaseMinion>() == true)
        {
            card.parentToReturnTo = this.transform;
            card.GetComponent<BaseMinion>().Played(playerManager);
        }
    }
}
