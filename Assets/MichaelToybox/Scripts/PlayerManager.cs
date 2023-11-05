using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;

//This script manages the player during combat
public class PlayerManager : MonoBehaviour
{
    [Header("Player Info")]
    public Team team;
    public int maxMana = 10;
    public int mana = 0;
    public int manaPerTurn = 2;
    public int manaPerTurnIncrease = 2;
    [SerializeField] protected float drawDelay = 0.25f;
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
        //gets references in the scene
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

    /// <summary>
    /// Ending the turn discards the players hand and increments the players mana per turn by manaPerTurnIncrease (capped at maxMana)
    /// </summary>
    public virtual void EndTurn()
    {
        handManager.DiscardHand(); //discards hand
        manaPerTurn = Mathf.Clamp(manaPerTurn + manaPerTurnIncrease,0,maxMana); //updates mana
    }

    /// <summary>
    /// Handles drawing the cards at the start of the turn. with a small delay between draws
    /// </summary>
    /// <returns></returns>
    public virtual IEnumerator StartTurnDraw()
    {
        foreach (DeckManager deck in heroDecks) //loops through each hero deck
        {
            //TODO: check if hero is dead, we do not want to draw cards if the hero is dead

            for (int i = 0; i < drawCountHero; i++) //loops for each time we want to draw from the hero deck
            {
                deck.DrawCard(handManager); //draws from the heroes deck
                yield return new WaitForSeconds(drawDelay);
            }
        }

        for (int i = 0; i < drawCountNeutral; i++) //loops for each time we want to draw from the neutral deck
        {
            neutralDeck.DrawCard(handManager); //draws from the neurtral deck
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

    /// <summary>
    /// Gets the target we clicked on.
    /// Checks that out target is what we want to click on based on the targetInfo
    /// </summary>
    /// <param name="targetInfo"></param>
    /// <param name="teamCheckAgainst"></param>
    /// <returns></returns>
    public virtual BaseCard GetClickTarget(TargetingInfo targetInfo, Team teamCheckAgainst)
    {
        if (Input.GetKeyDown(KeyCode.Mouse0) == false) //Player did not Left click
            return null;

        GameObject target = null;

        PointerEventData pointerData = new PointerEventData(EventSystem.current) //gets pointer data
        {
            pointerId = -1,
        };

        pointerData.position = Input.mousePosition; //sets pointer data position
        List<RaycastResult> results = new List<RaycastResult>(); //list of raycast results
        EventSystem.current.RaycastAll(pointerData, results); //sets list of raycast results to everything under the mouse

        if (results.Count > 0)
            target = results[0].gameObject; //sets our target to the first thing under the mouse
        else
            return null;

        //if the target did not pass the target check (example, target ios hero when trying to target a minion)
        if (target != null && CheckTargetingInfo(targetInfo, target, teamCheckAgainst) == false)
        {
            return null;
        }

        return target.GetComponent<BaseCard>();
    }

    /// <summary>
    /// Checks if the target meets the targeting info conditions.
    /// Checks if the target's team matches the targetInfo and if the target's type matches the targwtInfo
    /// </summary>
    /// <returns></returns>
    public virtual bool CheckTargetingInfo(TargetingInfo targetInfo, GameObject target, Team teamCheckAgainst)
    {
        //targetTeamComplies is changed based on what team relationship we are trying to check
        bool targetTeamComplies = false;
        //bool representing if our target is a minion
        bool targetIsMinion = false;
        Team targetTeam = Team.NONE;

        //gets the team of the target
        if (target.GetComponent<BaseMinion>()) //minion
        {
            targetTeam = target.GetComponent<BaseMinion>().team;
            targetIsMinion = true;
        }
        if (target.GetComponent<BaseHero>() && target.GetComponent<BaseHero>().isDead == false) //hero and hero is alive
        {
            targetTeam = target.GetComponent<BaseHero>().team;
            targetIsMinion = false;
            
        }
        

        //if the target has no team. return false
        if (targetTeam == Team.NONE)
            return false;

        //switch statement. based on enum targetInfo
        //the main logic for team checking
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

        //returns targetTeamComplies
        //targetTeamComplies is changed based on what team relationship we are trying to check (above switch statement)
        return targetTeamComplies;
    }
}
