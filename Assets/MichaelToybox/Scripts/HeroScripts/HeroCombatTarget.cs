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
    ArrowRenderer arrowRenderer;

    void Awake()
    {
        hero = this.GetComponent<BaseHero>();
        arrowRenderer = GameObject.FindObjectOfType<ArrowRenderer>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        startPos = this.transform.position;
    }

    void IDragHandler.OnDrag(PointerEventData eventData)
    {
        endPos = eventData.position;
        if (hero.canAttack == true)
        {
            //displays arrow from this object to mouse
            arrowRenderer.ExecuteArrowRender(this.gameObject);
        }
    }

    void IEndDragHandler.OnEndDrag(PointerEventData eventData)
    {
        RaycastResult hit = eventData.pointerCurrentRaycast;

        if (hit.gameObject != null && hero.canAttack == true && hero.isDead == false)
        {
            AttackTarget(hit.gameObject);
        }
        //removes arrow
        arrowRenderer.ResetArrowRenderer();
    }

    public void AttackTarget(GameObject attackTarget)
    {
        if (attackTarget.GetComponent<BaseMinion>() == true && attackTarget.GetComponent<BaseMinion>().targetable == true) //if we hit a minion and minion is targetable
        {
            //gets target reference
            BaseMinion target = attackTarget.gameObject.GetComponent<BaseMinion>();

            if (target.team != hero.team) //checks if the targeted minion is not on the same team
            {
                //damages target
                hero.AttackMinion(target);
            }
        }
        else if (attackTarget.GetComponent<BaseHero>() == true && attackTarget.GetComponent<BaseHero>().isDead == false && attackTarget.GetComponent<BaseHero>().targetable == true) //if we hit a hero and hero is not dead and hero is targetable
        {
            //gets target reference
            BaseHero target = attackTarget.gameObject.GetComponent<BaseHero>();

            if (target.team != hero.team) //checks if the targeted hero is not on the same team
            {
                //damages target hero
                hero.AttackHero(attackTarget.gameObject.GetComponent<BaseHero>());
            }
        }
    }
}
