using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HeroDraftButton : MonoBehaviour
{
    [SerializeField] List<HeroDraftButton> draftButtons;
    [SerializeField] HeroDraftManager manager;
    [SerializeField] BaseHero hero;
    public Image selectionImage;

    private void Start()
    {
        if (selectionImage != null)
            selectionImage.enabled = false;
    }

    public void SetupButtonDraft(BaseHero h, HeroDraftManager manage)
    {
        manager = manage;
        hero = h;
        this.GetComponent<Button>().onClick.AddListener(delegate { OnClickDraft(); });
        this.GetComponentInChildren<TMPro.TMP_Text>().text = h.name + "\n" + h.description;

        if (selectionImage != null)
            selectionImage.enabled = false;
    }

    public void SetupButtonPlayer(BaseHero h, HeroDraftManager manage)
    {
        manager = manage;
        hero = h;
        this.GetComponent<Button>().onClick.AddListener(delegate { OnClickPlayer(); });
        this.GetComponentInChildren<TMPro.TMP_Text>().text = h.name + "\n" + h.description;

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
