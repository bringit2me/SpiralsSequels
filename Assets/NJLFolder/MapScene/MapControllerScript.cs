using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class MapControllerScript : MonoBehaviour
{
    [Header("References")]
    public GameObject baseMapButton;
    int currPoint = 0;

    [Header("Control Panel")]
    public int rows;
    public int spacing = 100;
    public int colls = 3;


    void Start()
    {
        for (int y = 0; y < colls; y++)
        {
            for (int i = 0; i < rows; i++)
            {
                GameObject tempButton = Instantiate(baseMapButton, this.gameObject.transform);

                Vector3 currentButtonPosition = tempButton.transform.position;
                currentButtonPosition = new Vector3(currentButtonPosition.x + (spacing * i), currentButtonPosition.y + (spacing * (y - 1)), currentButtonPosition.z);
                tempButton.transform.position = currentButtonPosition;

                tempButton.GetComponent<MapButton>().level = colls;
            }
        }
        

    }

    
    void Update()
    {
        
    }
}
