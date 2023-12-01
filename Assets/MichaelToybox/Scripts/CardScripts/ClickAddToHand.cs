using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ClickAddToHand : MonoBehaviour
{

    public BaseCard card;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0) == false) //Player did not Left click
        {
            PointerEventData pointerData_ = new PointerEventData(EventSystem.current) //gets pointer data
            {
                pointerId = -1,
            };

            pointerData_.position = Input.mousePosition; //sets pointer data position
            List<RaycastResult> results_ = new List<RaycastResult>(); //list of raycast results
            EventSystem.current.RaycastAll(pointerData_, results_); //sets list of raycast results to everything under the mouse

            foreach (RaycastResult result in results_)
            {
                if (result.gameObject == this.gameObject)
                {
                    Debug.Log("Hovering: " + card.name);
                    return;
                }
            }
            return;
        }

        PointerEventData pointerData = new PointerEventData(EventSystem.current) //gets pointer data
        {
            pointerId = -1,
        };

        pointerData.position = Input.mousePosition; //sets pointer data position
        List<RaycastResult> results = new List<RaycastResult>(); //list of raycast results
        EventSystem.current.RaycastAll(pointerData, results); //sets list of raycast results to everything under the mouse

        foreach(RaycastResult result in results)
        {
            if(result.gameObject == this.gameObject)
            {
                //Card selected (adds to hand)
                SideboardDeckManager manager = GameObject.FindObjectOfType<SideboardDeckManager>();
                manager.CardSelected(card);
                return;
            }
        }
    }
}
