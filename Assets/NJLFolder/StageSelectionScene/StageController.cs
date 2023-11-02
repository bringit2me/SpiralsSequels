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
    




    private void Awake()
    {
        RandomizeStages();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void RandomizeStages()
    {
        
        bool bossButtonSpawned = false;
        for (int i = 0; i < buttons.Length; i++)
        {
            int randNum = Random.Range(1, 3); //3 encounters(basic, boss, and spiral)
            if (currStage == stagesToBoss && !bossButtonSpawned)
            {
                randNum = 3;
                bossButtonSpawned = true;
            }
            
            
            

            if(randNum == 1)
            {
                Debug.Log("This is a basic Encounter");
                

                TextMeshProUGUI buttonText = buttons[i].GetComponentInChildren<TextMeshProUGUI>();
                buttonText.text = "Basic Encounter";

                StageButtonController currButton = buttons[i].GetComponentInChildren<StageButtonController>();
                currButton.isBasicEncounter = true;
            }
            if(randNum == 2)
            {
                Debug.Log("This is a spiral Encounter");
                

                TextMeshProUGUI buttonText = buttons[i].GetComponentInChildren<TextMeshProUGUI>();
                buttonText.text = "Spiral";

                StageButtonController currButton = buttons[i].GetComponentInChildren<StageButtonController>();
                currButton.isSpiral = true;
            }
            if(randNum == 3)
            {
                Debug.Log("This is a boss Encounter");
                

                TextMeshProUGUI buttonText = buttons[i].GetComponentInChildren<TextMeshProUGUI>();
                buttonText.text = "Boss Encounter";

                StageButtonController currButton = buttons[i].GetComponentInChildren<StageButtonController>();
                currButton.isBossEncounter = true;
            }
        }
    }
}
