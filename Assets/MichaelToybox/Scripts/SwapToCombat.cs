using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwapToCombat : MonoBehaviour
{
    public Canvas combatCanvas;
    public Canvas gameCanvas;
    public int currentStage = 1;
    public int currentFloor = 1;

    //References
    CombatManager combatManager;
    EncounterManager encounterManager;

    private void Awake()
    {
        combatManager = GameObject.FindObjectOfType<CombatManager>();
        encounterManager = GameObject.FindObjectOfType<EncounterManager>();
    }

    public void StartRandomEncounter()
    {
        GameObject encounter = encounterManager.GetRandomEncounter(currentStage, currentFloor); //gets random encounter
        encounter.SetActive(true); //enables encounter
        encounter.transform.SetParent(combatManager.enemyHolder.transform); //sets encounter parent (places it on the UI)

        combatManager.playerGoesFirst = true;

        SwapGameToCombat();
        currentFloor++;
    }

    public void StartEliteEncounter()
    {
        GameObject encounter = encounterManager.GetEliteEncounter(currentStage); //gets elite encounter
        encounter.SetActive(true); //enables encounter
        encounter.transform.SetParent(combatManager.enemyHolder.transform); //sets encounter parent (places it on the UI)

        combatManager.playerGoesFirst = false;

        SwapGameToCombat();
        currentFloor++;
    }

    public void StartBossEncounter()
    {
        GameObject encounter = encounterManager.GetBossEncounter(currentStage); //gets boss encounter
        encounter.SetActive(true); //enables encounter
        encounter.transform.SetParent(combatManager.enemyHolder.transform); //sets encounter parent (places it on the UI)

        combatManager.playerGoesFirst = false;

        SwapGameToCombat();
        currentFloor++;
    }


    /// <summary>
    /// Turns on combat canvas and turns off game canvas
    /// </summary>
    public void SwapGameToCombat()
    {
        combatCanvas.enabled = true;
        gameCanvas.enabled = false;

        combatManager.SetupCombatManager();

        //TESTING
        this.GetComponent<Canvas>().enabled = false;
    }

    /// <summary>
    /// Turns off combat canvas and turns on game canvas
    /// </summary>
    public void SwapCombatToGame()
    {
        combatCanvas.enabled = false;
        gameCanvas.enabled = true;

        Destroy(combatManager.enemyHolder.transform.GetChild(0).gameObject); //removes old encounter game object

        //TESTING
        if (GameObject.FindObjectOfType<CardDraftManager>() != null) //we have a card draft manager
            GameObject.FindObjectOfType<CardDraftManager>().SetupCardDraft();
        else if (GameObject.FindObjectOfType<HeroDraftManager>() != null)
            GameObject.FindObjectOfType<HeroDraftManager>().SetupButtons();
    }
}
