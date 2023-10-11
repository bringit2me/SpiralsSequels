using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class BaseSpell : BaseCard
{

    [Header("UI References")]
    [SerializeField] TMP_Text nameText;
    [SerializeField] TMP_Text descriptionText;
    [SerializeField] TMP_Text manaText;

    public virtual void Start()
    {
        SetupCardText();
    }

    public virtual void SetupCardText()
    {
        nameText.text = name;
        descriptionText.text = description;
        UpdateMana();
    }

    public virtual void UpdateMana()
    {
        manaText.text = "" + manaCost;
    }
}
