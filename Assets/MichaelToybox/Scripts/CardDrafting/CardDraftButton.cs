using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardDraftButton : MonoBehaviour
{
    BaseCard card;
    Button button;
    CardDraftManager cardDraftManager;

    private void Awake()
    {
        button = this.GetComponent<Button>();
        cardDraftManager = GameObject.FindObjectOfType<CardDraftManager>();
    }

    public void SetupButton(BaseCard c)
    {
        //sets card ref
        card = c;

        //Resets button click and card visual
        button.onClick.RemoveAllListeners();

        if(transform.GetChild(0) != null) //something underneath
            Destroy(transform.GetChild(0).gameObject); //destroys card underneath

        BaseCard temp = Instantiate(card, this.transform); //creates card for visual
        temp.transform.localScale = new Vector3(3, 3, 1); //sets card scale
        temp.GetComponent<Draggable>().enabled = false; //disables draggable
        temp.GetComponent<CanvasGroup>().enabled = false; //disables canvas group

        button.onClick.AddListener(delegate { ButtonClicked(); }); //sets onclick method
    }

    public void ButtonClicked()
    {
        cardDraftManager.CardChosen(card);
    }
}
