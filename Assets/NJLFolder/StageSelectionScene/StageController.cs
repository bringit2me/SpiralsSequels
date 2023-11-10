using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StageController : MonoBehaviour
{

    //Goes up after stage completion
    [Header("Stage Information")]
    public int stagesToBoss = 4;
    public int currStage = 0;
    

    [Header("UI References")]
    public GameObject[] buttons;
    TextMeshProUGUI toNextBossText;
    




    private void Awake()
    {
        RandomizeStages();

        toNextBossText = GameObject.Find("ToNextBossText").GetComponent<TextMeshProUGUI>();

        toNextBossText.text = "Stage number: " + currStage;
    }

    
    void Update()
    {
        TestingControls();
    }

    void TestingControls() //REMOVE ME WHEN NO LONGER NEEDED
    {
        if(Input.GetKeyDown(KeyCode.N))
        {
            Debug.Log("Randomizing Stages...");
            RandomizeStages();
        }
        if(Input.GetKeyDown(KeyCode.M))
        {
            Debug.Log("Increasing Current stage number...");
            currStage++;
            toNextBossText.text = "Stage number: " + currStage;
        }
    }

    void RandomizeStages() //Randomizes the stages the player has to choose from at the start of the scene
    {
        
        bool bossButtonSpawned = false;
        for (int i = 0; i < buttons.Length; i++) //For loop runs code within it for each button in our list of buttons
        {
            int randNum = Random.Range(1, 4); //Generates a random number. 4 encounters(randomEncounter,specialEncounter, boss, and spiral)
            if (currStage == stagesToBoss && !bossButtonSpawned) //If the current stage is equal to the stages to boss int we garuntee a boss stage
            {
                randNum = 4;
                bossButtonSpawned = true;
            }
            
            
            
            //Each number corresponds to one of our stages, we then change the buttons text and set its identity for the StageButtonController script.
            if(randNum == 1)
            {
                Debug.Log("This is a random Encounter");
                

                TextMeshProUGUI buttonText = buttons[i].GetComponentInChildren<TextMeshProUGUI>();
                buttonText.text = "Random Encounter";

                StageButtonController currButton = buttons[i].GetComponentInChildren<StageButtonController>();
                currButton.isRandomEncounter = true;
                currButton.encounterImage.sprite = currButton.randomEncounterImage;
            }
            if(randNum == 2)
            {
                Debug.Log("This is a spiral Encounter");
                

                TextMeshProUGUI buttonText = buttons[i].GetComponentInChildren<TextMeshProUGUI>();
                buttonText.text = "Spiral";

                StageButtonController currButton = buttons[i].GetComponentInChildren<StageButtonController>();
                currButton.isSpiral = true;
                currButton.encounterImage.sprite = currButton.spiralEncounterImage;
            }
            if (randNum == 3)
            {
                Debug.Log("This is a Special Encounter");


                TextMeshProUGUI buttonText = buttons[i].GetComponentInChildren<TextMeshProUGUI>();
                buttonText.text = "Special Encounter";

                StageButtonController currButton = buttons[i].GetComponentInChildren<StageButtonController>();
                currButton.isSpecialEncounter = true;
                //Icon needs to be changed
            }
            if (randNum == 4)
            {
                Debug.Log("This is a boss Encounter");
                

                TextMeshProUGUI buttonText = buttons[i].GetComponentInChildren<TextMeshProUGUI>();
                buttonText.text = "Boss Encounter";

                StageButtonController currButton = buttons[i].GetComponentInChildren<StageButtonController>();
                currButton.isBossEncounter = true;
                //Icon needs to be changed
            }
        }
    }
}
