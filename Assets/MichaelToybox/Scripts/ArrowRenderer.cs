using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ArrowRenderer : MonoBehaviour
{
    LineRenderer lineRenderer;

    private void Start()
    {
        lineRenderer = this.GetComponent<LineRenderer>();
    }

    public void ExecuteArrowRender(GameObject origin)
    {
        lineRenderer.positionCount = 2;
        lineRenderer.SetPosition(0, origin.transform.position + Vector3.back); //sets start pos

        GameObject target;

        PointerEventData pointerData = new PointerEventData(EventSystem.current) //gets pointer data
        {
            pointerId = -1,
        };

        pointerData.position = Input.mousePosition; //sets pointer data position
        List<RaycastResult> results = new List<RaycastResult>(); //list of raycast results
        EventSystem.current.RaycastAll(pointerData, results); //sets list of raycast results to everything under the mouse

        if (results.Count > 0 && results[0].gameObject.GetComponent<BaseCard>() == true)
        {
            target = results[0].gameObject; //sets our target to the first thing under the mouse

            lineRenderer.SetPosition(1, target.transform.position + Vector3.back);
        }
        else
        {
            lineRenderer.SetPosition(1, Input.mousePosition + Vector3.back);
        }
    }

    public void ResetArrowRenderer()
    {
        lineRenderer.positionCount = 0;
    }
}
