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
    public bool isShop;
    public bool isBossEncounter;

    [Header("EncounterInformation")]
    public Image encounterImage;
    public Sprite randomEncounterImage;
    public Sprite shopEncounterImage;
    public Sprite spiralEncounterImage;
    public Sprite[] heroImages;
    public Sprite[] bossImages;

    SwapToCombat swaptoCombatScript;

    public Button confirmButton;

    public Canvas shopUI;
    

    void Start()
    {
        buttonInformation = GameObject.Find("ButtonInformation").GetComponentInChildren<TextMeshProUGUI>();
        controller = GameObject.FindObjectOfType<StageController>();

        swaptoCombatScript = GameObject.FindObjectOfType<SwapToCombat>();
        //confirmButton = swaptoCombatScript.GetComponent<Button>();
        
    }

    public void OpenShopUI()
    {
        shopUI.enabled = true;
    }
    
    public void ButtonClicked()
    {
        if(isRandomEncounter)
        {
            buttonInformation.text = "This is a random Encounter.";
            

            confirmButton.onClick.RemoveAllListeners();

            confirmButton.onClick.AddListener(
                delegate {
                swaptoCombatScript.StartRandomEncounter();
            }
            );
            
        }
        if (isSpecialEncounter)
        {
            buttonInformation.text = "This is a Special Encounter.";

            confirmButton.onClick.RemoveAllListeners();

            confirmButton.onClick.AddListener(
                delegate {
                    swaptoCombatScript.StartEliteEncounter();
                }
            );
        }
        if (isBossEncounter)
        {
            buttonInformation.text = "This is a boss Encounter.";

            confirmButton.onClick.RemoveAllListeners();

            confirmButton.onClick.AddListener(
                delegate {
                    swaptoCombatScript.StartBossEncounter();
                }
            );

        }
        if(isSpiral)
        {
            buttonInformation.text = "This is a Spiral.";
            
            
        }
        if(isShop)
        {
            buttonInformation.text = "This is a Shop.";

            confirmButton.onClick.RemoveAllListeners();

            confirmButton.onClick.AddListener(
                delegate {
                    OpenShopUI();
                }
            );
        }
    }
    
}
