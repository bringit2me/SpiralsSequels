using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HeroDraftButton : MonoBehaviour
{
    [SerializeField] List<HeroDraftButton> draftButtons;
    [SerializeField] HeroDraftManager manager;
    [SerializeField] BaseHero hero;
    public Image selectionImage;

    Button button;
    TMP_Text childText;

    private void Start()
    {
        button = this.GetComponent<Button>();
        childText = this.GetComponentInChildren<TMP_Text>();

        if (selectionImage != null)
            selectionImage.enabled = false;
    }

    public void SetupButtonDraft(BaseHero h, HeroDraftManager manage)
    {
        manager = manage;
        hero = h;
        button.onClick.AddListener(delegate { OnClickDraft(); });
        childText.text = h.name + "\n" + h.attack + " Attack  |  " + h.maxHealth + " Health\n" + h.description;

        if (selectionImage != null)
            selectionImage.enabled = false;
    }

    public void SetupButtonPlayer(BaseHero h, HeroDraftManager manage)
    {
        manager = manage;
        hero = h;


        button.onClick.AddListener(delegate { OnClickPlayer(); });

        childText.text = h.name + "\n" + h.attack + " Attack  |  " + h.health + " / " + h.maxHealth + " Health\n" + h.description;

        if (selectionImage != null)
            selectionImage.enabled = false;
    }

    public void OnClickDraft()
    {
        foreach (HeroDraftButton button in draftButtons)
            button.selectionImage.enabled = false;
        selectionImage.enabled = true;

        manager.SetSelectedDraftHero(hero); //sets selected hero
    }

    public void OnClickPlayer()
    {
        foreach (HeroDraftButton button in draftButtons)
            button.selectionImage.enabled = false;
        selectionImage.enabled = true;

        manager.SetSelectedPlayerHero(hero); //sets selected hero
    }
}
