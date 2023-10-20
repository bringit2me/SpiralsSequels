using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MinionCombatTarget : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    BaseMinion card;
    //these variables are for when/if we add a targeting arrow
    Vector3 startPos;
    Vector3 endPos;
    CombatManager combatManager;
    void Awake()
    {
        card = this.GetComponent<BaseMinion>();
        combatManager = GameObject.FindObjectOfType<CombatManager>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        startPos = this.transform.position;
        Debug.Log("Minion Begin Drag");
    }

    void IDragHandler.OnDrag(PointerEventData eventData)
    {
        endPos = eventData.position;
    }

    void IEndDragHandler.OnEndDrag(PointerEventData eventData)
    {
        RaycastResult hit = eventData.pointerCurrentRaycast;

        if (hit.gameObject != null && card.canAttack == true)
        {
            if (hit.gameObject.GetComponent<BaseMinion>() == true)
            {
                //gets target reference
                BaseMinion target = hit.gameObject.GetComponent<BaseMinion>();

                if (target.team != card.team) //checks if the targeted minion is not on the same team
                {
                    //damages target
                    card.AttackMinion(target);
                    //retaliation damage
                    card.TakeDamage(target.attack);
                }
            }
            else if (hit.gameObject.GetComponent<BaseHero>() == true)
            {
                //gets target reference
                BaseHero target = hit.gameObject.GetComponent<BaseHero>();

                if (target.team != card.team) //checks if the targeted hero is not on the same team
                {
                    //damages target hero
                    card.AttackHero(hit.gameObject.GetComponent<BaseHero>());
                }
            }
        }
    }
}
