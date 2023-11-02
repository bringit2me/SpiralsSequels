using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class StageButtonController : MonoBehaviour
{
    TextMeshProUGUI buttonInformation;
    StageController controller;

    [Header("EncounterType")]
    public bool isBasicEncounter;
    public bool isSpiral;
    public bool isBossEncounter;

    void Start()
    {
        buttonInformation = GameObject.Find("ButtonInformation").GetComponentInChildren<TextMeshProUGUI>();
        controller = GameObject.FindObjectOfType<StageController>();
    }

    
    public void ButtonClicked()
    {
        if(isBasicEncounter)
        {
            buttonInformation.text = "This is a basic Encounter.";
        }
        if(isBossEncounter)
        {
            buttonInformation.text = "This is a boss Encounter.";
        }
        if(isSpiral)
        {
            buttonInformation.text = "This is a Spiral.";
        }
    }
    
}
