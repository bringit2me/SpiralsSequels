using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class MapControllerScript : MonoBehaviour
{
    public GameObject baseMapButton;
    public Transform[] pointLocation;
    int currPoint = 0;


    void Start()
    {
        
        for (int i = 0; i < pointLocation.Length; i++)
        {
            Instantiate(baseMapButton, pointLocation[currPoint]);
            currPoint++;
            Debug.Log(currPoint);
        }

    }

    
    void Update()
    {
        
    }
}
