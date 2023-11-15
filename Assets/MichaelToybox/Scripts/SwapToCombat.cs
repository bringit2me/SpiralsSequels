using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwapToCombat : MonoBehaviour
{
    public Canvas combatCanvas;
    public Canvas gameCanvas;

    //References
    CombatManager combatManager;
    [SerializeField] RandomEncounter randomEncounter;

    private void Awake()
    {
        combatManager = GameObject.FindObjectOfType<CombatManager>();
    }

    /// <summary>
    /// Turns on combat canvas and turns off game canvas
    /// </summary>
    public void SwapGameToCombat()
    {
        combatCanvas.enabled = true;
        gameCanvas.enabled = false;

        //TEMP: Creates random encounter
        Instantiate(randomEncounter, combatManager.enemyHolder.transform);

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
        GameObject.FindObjectOfType<HeroDraftManager>().SetupButtons();
    }
}
