using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardDraftManager : MonoBehaviour
{
    public Canvas thisCanvas;
    public Canvas swapToCanvas;
    CardDraftButton[] draftButtons;
    //References
    Preloader preloader;
    SideboardDeckManager sideboardDeck;

    private void Start()
    {
        //Finds script references
        preloader = GameObject.FindObjectOfType<Preloader>();
        sideboardDeck = GameObject.FindObjectOfType<SideboardDeckManager>();
        //Gets all draft buttons
        draftButtons = GameObject.FindObjectsOfType<CardDraftButton>();
    }

    /// <summary>
    /// Sets up all buttons
    /// </summary>
    public void SetupCardDraft()
    {
        foreach(CardDraftButton button in draftButtons)
        {
            button.SetupButton(preloader.GetCard());
        }
    }
    public void CardChosen(BaseCard card)
    {
        sideboardDeck.AddCardToDeck(card); //adds card to deck
        //swaps canvas
        thisCanvas.enabled = false;
        swapToCanvas.enabled = true;

        //TESTING
        GameObject.FindObjectOfType<HeroDraftManager>().SetupButtons();
    }

    public void Skip()
    {
        thisCanvas.enabled = false;
        swapToCanvas.enabled = true;

        //TESTING
        GameObject.FindObjectOfType<HeroDraftManager>().SetupButtons();
    }
}
