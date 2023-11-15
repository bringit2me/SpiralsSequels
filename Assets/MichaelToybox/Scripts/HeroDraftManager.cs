using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HeroDraftManager : MonoBehaviour
{
    PlayerHeroManager playerHeroManager;
    public Button[] playerButtons;
    public Button[] draftButtons;
    public List<BaseHero> draftableHeroes;
    [Header("Selection")]
    [SerializeField] BaseHero selectedDraftHero;
    [SerializeField] BaseHero selectedPlayerHero;
    [Header("Testing Transition")]
    public Canvas gameCanvas;

    private void Start()
    {
        playerHeroManager = GameObject.FindObjectOfType<PlayerHeroManager>();
    }

    public void SetupButtons()
    {
        //resets reference
        selectedDraftHero = null;
        selectedPlayerHero = null;

        draftableHeroes.Shuffle();

        int count = 0;

        foreach (Button button in draftButtons)
        {
            button.GetComponent<HeroDraftButton>().SetupButtonDraft(draftableHeroes[count], this);
            count++;
        }

        count = 0;

        foreach (Button button in playerButtons)
        {
            button.GetComponent<HeroDraftButton>().SetupButtonPlayer(playerHeroManager.heroes[count],this);
            count++;
        }
    }

    public void SetSelectedDraftHero(BaseHero hero)
    {
        selectedDraftHero = hero;
        CheckHeroChange();
    }

    public void SetSelectedPlayerHero(BaseHero hero)
    {
        selectedPlayerHero = hero;
        CheckHeroChange();
    }

    /// <summary>
    /// Checks if we have the required information to change the hero
    /// </summary>
    void CheckHeroChange()
    {
        if(selectedDraftHero != null && selectedPlayerHero != null)
        {
            playerHeroManager.ChangeHero(selectedDraftHero, playerHeroManager.heroes.IndexOf(selectedPlayerHero));

            ExitDraftScreen();
        }
    }

    public void ExitDraftScreen()
    {
        //resets reference
        selectedDraftHero = null;
        selectedPlayerHero = null;

        //Sets up buttons
        SetupButtons();

        //TESTING: Swaps to game canvas
        gameCanvas.enabled = true;
        this.GetComponent<Canvas>().enabled = false;
    }
}
