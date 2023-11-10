using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class StageButtonController : MonoBehaviour
{
    TextMeshProUGUI buttonInformation;
    StageController controller;
    

    [Header("EncounterType")]
    public bool isRandomEncounter;
    public bool isSpecialEncounter;
    public bool isSpiral;
    public bool isBossEncounter;

    [Header("EncounterInformation")]
    public Image encounterImage;
    public Sprite randomEncounterImage;
    public Sprite shopEncounterImage;
    public Sprite spiralEncounterImage;
    public Sprite[] heroImages;
    public Sprite[] bossImages;
    

    void Start()
    {
        buttonInformation = GameObject.Find("ButtonInformation").GetComponentInChildren<TextMeshProUGUI>();
        controller = GameObject.FindObjectOfType<StageController>();
        
        
    }

    
    public void ButtonClicked()
    {
        if(isRandomEncounter)
        {
            buttonInformation.text = "This is a random Encounter.";
            
        }
        if (isSpecialEncounter)
        {
            buttonInformation.text = "This is a Special Encounter.";
            
        }
        if (isBossEncounter)
        {
            buttonInformation.text = "This is a boss Encounter.";
            
        }
        if(isSpiral)
        {
            buttonInformation.text = "This is a Spiral.";
            
            
        }
    }
    
}
