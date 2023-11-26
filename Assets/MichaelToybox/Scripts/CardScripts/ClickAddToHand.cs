using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ClickAddToHand : MonoBehaviour
{

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0) == false) //Player did not Left click
            return;

        GameObject target = null;

        PointerEventData pointerData = new PointerEventData(EventSystem.current) //gets pointer data
        {
            pointerId = -1,
        };

        pointerData.position = Input.mousePosition; //sets pointer data position
        List<RaycastResult> results = new List<RaycastResult>(); //list of raycast results
        EventSystem.current.RaycastAll(pointerData, results); //sets list of raycast results to everything under the mouse

        if (results.Count > 0)
        {
            //Card selected (adds to hand)
            SideboardDeckManager manager = GameObject.FindObjectOfType<SideboardDeckManager>();
            manager.CardSelected(this.GetComponent<BaseCard>());
        }
    }
}
