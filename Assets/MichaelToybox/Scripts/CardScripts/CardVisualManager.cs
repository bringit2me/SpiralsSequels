using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[RequireComponent(typeof(BaseCard))]
public class CardVisualManager : MonoBehaviour
{
    BaseCard card;
    BaseMinion minion;
    BaseHero hero;
    public List<CardStatChangeEntry> statChanges = new List<CardStatChangeEntry>();
    public List<CardEffectEntry> effectList = new List<CardEffectEntry>();
    [Header("UI References")]
    [SerializeField] TMP_Text nameText;
    [SerializeField] TMP_Text descriptionText;
    [SerializeField] TMP_Text manaText;
    [SerializeField] TMP_Text attackText;
    [SerializeField] TMP_Text healthText;
    [SerializeField] Color defaultTextColor = Color.black;
    [SerializeField] Color negativeTextColor = Color.red;
    [SerializeField] Color positiveTextColor = new Color(0, 1f, 0);

    private void Awake()
    {
        //Gets card references
        card = this.GetComponent<BaseCard>();
        minion = card.GetComponent<BaseMinion>();
        hero = card.GetComponent<BaseHero>();

        //Gets UI References (checks if they exist, then gets the TMP_Text reference)
        GetUIReferences();
    }

    public void GetUIReferences()
    {
        //Gets UI References (checks if they exist, then gets the TMP_Text reference)
        if (transform.Find("Name Text") != null)
            nameText = transform.Find("Name Text").GetComponent<TMP_Text>();
        if (transform.Find("Description Text") != null)
            descriptionText = transform.Find("Description Text").GetComponent<TMP_Text>();
        if (transform.Find("Mana Text") != null)
            manaText = transform.Find("Mana Text").GetComponent<TMP_Text>();
        if (transform.Find("Attack Text") != null)
            attackText = transform.Find("Attack Text").GetComponent<TMP_Text>();
        if (transform.Find("Health Text") != null)
            healthText = transform.Find("Health Text").GetComponent<TMP_Text>();
    }

    public void AddEffectEntry(CardEffectEntry effectEntry)
    {
        effectList.Add(effectEntry);
    }

    public void AddStatChangeEntry(int manaCost, CardEffectEntry effectEntry)
    {
        CardStatChangeEntry entry = new CardStatChangeEntry(manaCost, true);

        statChanges.Add(entry);
        
        if (effectEntry != null)
            effectList.Add(effectEntry);

    }

    public void AddStatChangeEntry(int attack, int health, CardEffectEntry effectEntry)
    {
        CardStatChangeEntry entry = new CardStatChangeEntry(attack, true, health, true);

        statChanges.Add(entry);

        if (effectEntry != null)
            effectList.Add(effectEntry);

    }

    public void AddStatChangeEntry(int manaCost, int attack, int health, CardEffectEntry effectEntry)
    {
        CardStatChangeEntry entry = new CardStatChangeEntry(manaCost, true, attack, true, health, true);

        statChanges.Add(entry);

        if (effectEntry != null)
            effectList.Add(effectEntry);

    }

    public void AddStatChangeEntry(int manaCost, bool updateCost, int attack, bool updateAttack, int health, bool updateHealth, CardEffectEntry effectEntry)
    {
        CardStatChangeEntry entry = new CardStatChangeEntry(manaCost, updateCost, attack, updateAttack, health, updateHealth);

        statChanges.Add(entry);

        if (effectEntry != null)
            effectList.Add(effectEntry);

    }

    public CardStatChangeEntry GetStatChangeEntry()
    {
        if (statChanges.Count > 0)
        {
            CardStatChangeEntry entry = statChanges[0];
            statChanges.RemoveAt(0);

            return entry;
        }

        return null;
    }

    /// <summary>
    /// Updates stats based on the next entry in the statChanges list
    /// </summary>
    public void UpdateStatChanges()
    {
        CardStatChangeEntry entry = GetStatChangeEntry();
        if (entry != null)
        {
            UpdateMana(entry.manaVal, entry.updateMana);
            UpdateAttack(entry.attackVal, entry.updateAttack);
            UpdateHealth(entry.healthVal, entry.updateHealth);
        }
    }

    public void UpdateName(string text)
    {
        if (nameText != null)
            nameText.text = text;
    }

    public void UpdateDescription(string text)
    {
        if (descriptionText != null)
            descriptionText.text = text;
    }

    public void UpdateMana(int value, bool updateValue)
    {
        if(manaText != null && updateValue == true) //if we have a mana cost text and we should update mana cost
        {
            manaText.text = "" + value; //sets text

            if (value > card.baseManaCost) //cost is higher
                manaText.color = negativeTextColor; //sets text to negative color
            else if (value < card.baseManaCost) //cost is lower
                manaText.color = positiveTextColor; //sets text to positive color
            else //cost the same
                manaText.color = defaultTextColor; //sets text to normal color
        }
    }

    public void UpdateAttack(int value, bool updateValue)
    {
        if (attackText != null && updateValue == true) //if we have an attack text and we should update attack
        {
            attackText.text = "" + value; //updates attack value

            if (minion != null) //have a minion reference
            {
                if (value > minion.baseAttack) //attack is higher
                    attackText.color = positiveTextColor; //sets text to positive color
                else if (value < minion.baseAttack) //attack is lower
                    attackText.color = negativeTextColor; //sets text to negative color
                else
                    attackText.color = defaultTextColor; //sets text to default color
            }
            else if (hero != null) //have a hero reference
            {
                if (value > hero.baseAttack) //attack is higher
                    attackText.color = positiveTextColor; //sets text to positive color
                else if (value < hero.baseAttack) //attack is lower
                    attackText.color = negativeTextColor; //sets text to negative color
                else
                    attackText.color = defaultTextColor; //sets text to default color
            }
        }
    }

    public void UpdateHealth(int value, bool updateValue)
    {
        if (healthText != null && updateValue == true) //if we have a health text and we should update health
        {
            healthText.text = "" + value; //updates health text

            if (minion != null) //have a minion reference
            {
                if (value > minion.maxHealth) //health is higher than max
                    healthText.color = positiveTextColor; //sets text to positive color
                else if (value < minion.maxHealth) //minion is damaged
                    healthText.color = negativeTextColor; //sets text to negative color
                else //health == max health
                {
                    if (value > minion.baseMaxHealth) //health > base max health
                        healthText.color = positiveTextColor;
                    else //health == base max health
                        healthText.color = defaultTextColor; //sets text to default color
                }
            }
            else if (hero != null) //have a hero reference
            {
                if (value > hero.maxHealth) //health is higher than max
                    healthText.color = positiveTextColor; //sets text to positive color
                else if (value < hero.maxHealth) //hero is damaged
                    healthText.color = negativeTextColor; //sets text to negative color
                else //health == max health
                {
                    if (value > hero.baseMaxHealth) //health > base max health
                        healthText.color = positiveTextColor;
                    else //health == base max health
                        healthText.color = defaultTextColor; //sets text to default color
                }
            }
        }
    }
}

[System.Serializable]
public class CardStatChangeEntry
{
    public bool updateMana = false;
    public int manaVal = 0;

    public bool updateHealth = false;
    public int healthVal = 0;

    public bool updateAttack = false;
    public int attackVal = 0;

    //Constructor that sets all values and if we should update them
    public CardStatChangeEntry( int mana, bool updateM, int attack, bool updateA, int health, bool updateH)
    {
        updateMana = updateM;
        manaVal = mana;

        updateAttack = updateA;
        attackVal = attack;

        updateHealth = updateH;
        healthVal = health;
    }
    //Simplified constructor for updating only mana costs
    public CardStatChangeEntry(int mana, bool updateM)
    {
        updateMana = updateM;
        manaVal = mana;

        //dont update attack and health
        updateAttack = false;
        updateHealth = false;
    }

    //Simplified constructor for updating only attack and health
    public CardStatChangeEntry(int attack, bool updateA, int health, bool updateH)
    {
        //dont update mana
        updateMana = false;

        updateAttack = updateA;
        attackVal = attack;

        updateHealth = updateH;
        healthVal = health;
    }
}

[System.Serializable]
public class CardEffectEntry
{
    public int count;
    public string name;
    public string description;
}
