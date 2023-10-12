using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;


public class HeroCombatTarget : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    BaseHero hero;
    //these variables are for when/if we add a targeting arrow
    Vector3 startPos;
    Vector3 endPos;

    void Awake()
    {
        hero = this.GetComponent<BaseHero>();
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

        if (hit.gameObject != null && hero.canAttack == true)
        {
            if (hit.gameObject.GetComponent<BaseMinion>() == true)
            {
                //gets target reference
                BaseMinion target = hit.gameObject.GetComponent<BaseMinion>();

                if (target.team != hero.team) //checks if the targeted minion is not on the same team
                {
                    //damages target
                    hero.AttackMinion(target);
                }
            }
            else if (hit.gameObject.GetComponent<BaseHero>() == true)
            {
                //gets target reference
                BaseHero target = hit.gameObject.GetComponent<BaseHero>();

                if (target.team != hero.team) //checks if the targeted hero is not on the same team
                {
                    //damages target hero
                    hero.AttackHero(hit.gameObject.GetComponent<BaseHero>());
                }
            }
        }
    }
}
