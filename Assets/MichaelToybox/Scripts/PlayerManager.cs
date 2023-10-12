using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerManager : MonoBehaviour
{
    HandManager handManager;
    PlayerMinionZone minionZone;

    public int maxMana = 10;
    public int mana = 0;
    public int manaPerTurn = 2;
    public int manaPerTurnIncrease = 2;
    [SerializeField] float drawDelay = 0.25f;
    [Header("Neutral Deck")]
    public int drawCountNeutral = 4;
    public DeckManager neutralDeck;
    [Header("Hero Decks")]
    public int drawCountHero = 2;
    public DeckManager[] heroDecks;
    [Header("UI References")]
    public TMP_Text manaText;

    private void Awake()
    {
        handManager = this.GetComponent<HandManager>();
        minionZone = GameObject.FindObjectOfType<PlayerMinionZone>();
        UpdateManaText();
    }

    public void StartTurn()
    {
        //draws start of turn cards
        StartCoroutine(StartTurnDraw());
        minionZone.EnableMinionAttack(); //enables minions in the combat zone to attack
        mana = manaPerTurn; //sets mana to mana per turn
        UpdateManaText();

        //TESTING: sets hero can attack to true
        foreach(BaseHero hero in GameObject.FindObjectsOfType<BaseHero>())
        {
            if(hero.team == Team.PLAYER)
            {
                hero.canAttack = true;
            }
        }
    }

    public void EndTurn()
    {
        handManager.DiscardHand();
        manaPerTurn = Mathf.Clamp(manaPerTurn + manaPerTurnIncrease,0,maxMana);
    }

    IEnumerator StartTurnDraw()
    {
        foreach (DeckManager deck in heroDecks)
        {
            for (int i = 0; i < drawCountHero; i++)
            {
                deck.DrawCard(handManager);
                yield return new WaitForSeconds(drawDelay);
            }
        }

        for (int i = 0; i < drawCountNeutral; i++)
        {
            neutralDeck.DrawCard(handManager);
            yield return new WaitForSeconds(drawDelay);
        }
    }

    public void UpdateManaText()
    {
        manaText.text = "" + mana; //sets mana text
    }

    /// <summary>
    /// Checks if the player has enough mana to play the card
    /// </summary>
    /// <returns></returns>
    public bool CheckPlayable(BaseCard card)
    {
        if(mana >= card.manaCost) //if the player has enough mana to play the card
        {
            return true;
        }
        //not enough mana to play the card
        return false;
    }
}
