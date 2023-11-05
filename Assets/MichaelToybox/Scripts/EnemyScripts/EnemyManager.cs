using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : PlayerManager
{
    protected BaseEnemyAI enemyAI;

    public override void Awake()
    {
        //gets references in the scene
        combatManager = GameObject.FindObjectOfType<CombatManager>();
        //Gets references in the enemy encounter object
        handManager = this.GetComponent<HandManager>();
        minionZone = this.GetComponentInChildren<EnemyMinionZone>();
        enemyAI = this.GetComponent<BaseEnemyAI>();
    }

    /// <summary>
    /// Clears hero and heroDecks list. Then gets hero and deck references and adds them to the lists
    /// </summary>
    public virtual void GetChildHeroReferences()
    {
        heroes.Clear(); //clears hero list
        heroDecks.Clear(); //clears hero deck list

        foreach(BaseHero hero in this.GetComponentsInChildren<BaseHero>())
        {
            heroes.Add(hero); //adds hero to list
            if (hero.GetComponent<DeckManager>() == true) //if there is a deck attached to the hero
                heroDecks.Add(hero.GetComponent<DeckManager>()); //adds deck to list
            else
                Debug.LogWarning("MICHAEL WARNING: Hero added to EnemyManager hero list without a DeckManager attached");
        }
    }

    public override IEnumerator StartTurnDraw()
    {
        foreach (DeckManager deck in heroDecks) //loops through each hero deck
        {
            if (deck.hero != null && deck.hero.health <= 0) //if the decks hero is dead
                continue; //goes to next entry in the foreach loop

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

    public virtual void UpdateEnemyAI()
    {
        SetValueBoostFromHeroes();
    }

    public virtual void SetValueBoostFromHeroes()
    {
        enemyAI.aggroValueBoost = 0;
        enemyAI.midRangeValueBoost = 0;
        enemyAI.defenseValueBoost = 0;

        foreach(BaseHero hero in heroes)
        {
            enemyAI.aggroValueBoost += hero.aggroValueBoost;
            enemyAI.midRangeValueBoost += hero.midRangeValueBoost;
            enemyAI.defenseValueBoost += hero.defenseValueBoost;
        }
    }
}
