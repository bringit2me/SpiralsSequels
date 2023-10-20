using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;

public class PlayerManager : MonoBehaviour
{
    [Header("Player Info")]
    public Team team;
    public int maxMana = 10;
    public int mana = 0;
    public int manaPerTurn = 2;
    public int manaPerTurnIncrease = 2;
    [SerializeField] float drawDelay = 0.25f;
    [Header("Neutral Deck Info")]
    public int drawCountNeutral = 4;
    public DeckManager neutralDeck;
    [Header("Hero Info")]
    public int drawCountHero = 2;
    public List<BaseHero> heroes;
    public List<DeckManager> heroDecks;
    [Header("Minion Zone Info")]
    public PlayerMinionZone minionZone;
    [Header("UI References")]
    public TMP_Text manaText;

    //References
    [HideInInspector] public HandManager handManager;
    protected CombatManager combatManager;

    public virtual void Awake()
    {
        combatManager = GameObject.FindObjectOfType<CombatManager>();
        handManager = this.GetComponent<HandManager>();
        minionZone = combatManager.playerMinionZone;
    }

    public virtual void StartTurn()
    {
        //draws start of turn cards
        StartCoroutine(StartTurnDraw());
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

    public virtual void EndTurn()
    {
        handManager.DiscardHand();
        manaPerTurn = Mathf.Clamp(manaPerTurn + manaPerTurnIncrease,0,maxMana);
    }

    public virtual IEnumerator StartTurnDraw()
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

    /// <summary>
    /// Updates mana text
    /// </summary>
    public virtual void UpdateManaText()
    {
        manaText.text = "" + mana; //sets mana text
    }

    /// <summary>
    /// Checks if the player has enough mana to play the card
    /// </summary>
    /// <returns></returns>
    public virtual bool CheckPlayable(BaseCard card)
    {
        if(mana >= card.manaCost) //if the player has enough mana to play the card
        {
            return true;
        }
        //not enough mana to play the card
        return false;
    }

    public virtual GameObject GetClickTarget(TargetingInfo targetInfo, Team teamCheckAgainst)
    {
        if (Input.GetKeyDown(KeyCode.Mouse0) == false) //Player did not Left click
            return null;

        GameObject target;

        PointerEventData pointerData = new PointerEventData(EventSystem.current)
        {
            pointerId = -1,
        };
        pointerData.position = Input.mousePosition;
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pointerData, results);

        target = results[0].gameObject;
        if (target != null)
            Debug.Log("target: " + target.name);

        //if the target did not pass the target check (example, target ios hero when trying to target a minion)
        if (target != null && CheckTargetingInfo(targetInfo, target, teamCheckAgainst) == false)
        {
            return null;
        }

        return target;
    }

    /// <summary>
    /// Checks if the target meets the targeting info conditions.
    /// Checks if the target's team matches the targetInfo and if the target's type matches the targwtInfo
    /// </summary>
    /// <returns></returns>
    public virtual bool CheckTargetingInfo(TargetingInfo targetInfo, GameObject target, Team teamCheckAgainst)
    {
        bool targetTeamComplies = false;
        bool targetIsMinion = false;
        Team targetTeam = Team.NONE;

        //gets the team of the target
        if (target.GetComponent<BaseMinion>())
        {
            targetTeam = target.GetComponent<BaseMinion>().team;
            targetIsMinion = true;
        }
        if (target.GetComponent<BaseHero>())
        {
            targetTeam = target.GetComponent<BaseHero>().team;
            targetIsMinion = false;
        }
        

        //if the target has no team. return none
        if (targetTeam == Team.NONE)
            return false;

        switch ((int)targetInfo)
        {
            case 0:
                targetTeamComplies = targetTeam != Team.NONE; //Any team
                break;
            case 1:
                targetTeamComplies = targetTeam == teamCheckAgainst; //Same team
                break;
            case 2:
                targetTeamComplies = targetTeam != teamCheckAgainst; //Opposite team
                break;
            case 3:
                targetTeamComplies = targetTeam != Team.NONE && targetIsMinion == true; //Any team AND Minion
                break;
            case 4:
                targetTeamComplies = targetTeam == teamCheckAgainst && targetIsMinion == true; //Same team AND Minion
                break;
            case 5:
                targetTeamComplies = targetTeam != teamCheckAgainst && targetIsMinion == true; //Opposite team AND Minion
                break;
            case 6:
                targetTeamComplies = targetTeam != Team.NONE && targetIsMinion == false; //Any team AND Hero
                break;
            case 7:
                targetTeamComplies = targetTeam == teamCheckAgainst && targetIsMinion == false; //Same team AND Hero
                break;
            case 8:
                targetTeamComplies = targetTeam != teamCheckAgainst && targetIsMinion == false; //Opposite team AND Hero
                break;
        }

        return targetTeamComplies;
    }
}
