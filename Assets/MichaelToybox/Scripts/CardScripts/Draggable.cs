using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Draggable : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    Vector3 startPos;
    public Transform parentToReturnTo;
    PlayerManager playerManager;
    BaseCard cardRef;
    bool isMinion = false;

    void Start()
    {
        cardRef = this.GetComponent<BaseCard>();
        playerManager = cardRef.playerManager;
        if (this.GetComponent<BaseMinion>() == true)
            isMinion = true;

    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if(eventData.pointerDrag.GetComponent<BaseCard>() == true)
        {
            //Checks if card is not playable
            if(playerManager.CheckPlayable(eventData.pointerDrag.GetComponent<BaseCard>()) == false)
            {
                //stops card drag
                eventData.pointerDrag = null;
            }
        }
        //Gets start position and parent
        parentToReturnTo = this.transform.parent;
        startPos = this.transform.position;

        //Turns off raycast blocking
        GetComponent<CanvasGroup>().blocksRaycasts = false;
    }

    void IDragHandler.OnDrag(PointerEventData eventData)
    {
        this.transform.position = eventData.position;

        if (eventData.position.y >= -440 + 175)
        {
            this.transform.SetParent(transform.root);
        }
        else
        {
            this.transform.SetParent(parentToReturnTo);
        }
    }

    void IEndDragHandler.OnEndDrag(PointerEventData eventData)
    {
        if (eventData.pointerDrag.GetComponent<BaseSpell>() == true) //if we stopped dragging a spell
        {
            if(eventData.position.y >= -440 + 175) //if the mouse is above the hand space
            {
                this.transform.SetParent(transform.root);
                eventData.pointerDrag.GetComponent<BaseSpell>().Played(playerManager);
                return;
            }
        }

        //Returns card to parentToReturnTo
        this.transform.SetParent(parentToReturnTo);

        //Turns on raycast blocking
        GetComponent<CanvasGroup>().blocksRaycasts = true;
    }
}
