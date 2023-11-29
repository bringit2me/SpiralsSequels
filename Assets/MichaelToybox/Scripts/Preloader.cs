using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Preloader : MonoBehaviour
{
    public int seed;
    EncounterManager encounterManager;
    public int amountToPreload = 150;
    [Header("Rarity")]
    public int commonChance = 70;
    public int rareChance = 20;
    public int epicChance = 10;
    [Header("Preloaded Cards & Heroes")]
    public List<BaseCard> preloadedCards;
    public List<BaseHero> preloadedHeroes;

    [Header("Sideboard Card Lists")]
    public List<BaseCard> allCards;
    public List<BaseCard> commonCards;
    public List<BaseCard> rareCards;
    public List<BaseCard> epicCards;
    [Header("Hero Lists")]
    public List<BaseHero> allHeroes;
    public List<BaseHero> commonHeroes;
    public List<BaseHero> rareHeroes;
    public List<BaseHero> epicHeroes;

    private void Start()
    {
        encounterManager = GameObject.FindObjectOfType<EncounterManager>();
        if (seed == 0)
            seed = Random.Range(10000000, 100000000);

        Random.InitState(seed); //sets Random Number Generator seed

        StartRarityListSetup();
    }

    /// <summary>
    /// Calls SetupAllCardsHeroesLists() (which calls to setup common card rarity list, then the next one, ect)
    /// When done loading the rarity lists, it then calls to preload the cards, which then calls to preload the heroes, then calls to preload encounters
    /// </summary>
    void StartRarityListSetup()
    {
        StartCoroutine(SetupAllCardsHeroesLists());
    }

    /// <summary>
    /// sets up allCards and allHeroes lists
    /// </summary>
    /// <returns></returns>
    IEnumerator SetupAllCardsHeroesLists()
    {
        //Loops through all minions
        foreach(BaseMinion minion in Resources.LoadAll<BaseMinion>(""))
        {
            allCards.Add(minion);
        }
        //Loops through all minions
        foreach (BaseSpell spell in Resources.LoadAll<BaseSpell>(""))
        {
            allCards.Add(spell);
        }
        foreach(BaseHero hero in Resources.LoadAll<BaseHero>(""))
        {
            allHeroes.Add(hero);
        }
        yield return new WaitForEndOfFrame();
        StartCoroutine(SetupCardRarityLists());
    }

    IEnumerator SetupCardRarityLists()
    {
        foreach(BaseCard card in allCards)
        {
            if (card.rarity == Rarity.NONE)
                continue;
            else if (card.rarity == Rarity.COMMON)
                commonCards.Add(card);
            else if (card.rarity == Rarity.RARE)
                rareCards.Add(card);
            else if (card.rarity == Rarity.EPIC)
                epicCards.Add(card);
        }

        yield return new WaitForEndOfFrame();
        StartCoroutine(SetupHeroRarityLists());
    }

    IEnumerator SetupHeroRarityLists()
    {
        foreach (BaseHero hero in allHeroes)
        {
            if (hero.rarity == Rarity.NONE)
                continue;
            else if (hero.rarity == Rarity.COMMON)
                commonHeroes.Add(hero);
            else if (hero.rarity == Rarity.RARE)
                rareHeroes.Add(hero);
            else if (hero.rarity == Rarity.EPIC)
                epicHeroes.Add(hero);
        }

        yield return new WaitForEndOfFrame();
        StartCoroutine(PreloadCards());
    }


    /// <summary>
    /// Preloads cards and adds them to the respective rarity list
    /// First selects a rarity, then selects a random card from the rarity list
    /// </summary>
    /// <returns></returns>
    IEnumerator PreloadCards()
    {
        int randNum = 0;
        BaseCard lastCard1 = null;
        BaseCard lastCard2 = null;
        for (int i = 0; i < amountToPreload; i++)
        {
            randNum = Random.Range(0, 101);

            if (randNum < commonChance)
            {
                preloadedCards.Add(GetRandomCommonCard(lastCard1, lastCard2));
            }
            else if (randNum < commonChance + rareChance)
            {
                preloadedCards.Add(GetRandomRareCard(lastCard1, lastCard2));
            }
            else if (randNum < commonChance + rareChance + epicChance)
            {
                preloadedCards.Add(GetRandomEpicCard(lastCard1, lastCard2));
            }

            if (preloadedCards.Count > 0)
                lastCard1 = preloadedCards[preloadedCards.Count - 1];
            if (preloadedCards.Count > 1)
                lastCard2 = preloadedCards[preloadedCards.Count - 2];

        }

        yield return new WaitForEndOfFrame();
        StartCoroutine(PreloadHeroes());
    }

    /// <summary>
    /// Preloads heroes and adds them to the respective rarity list
    /// First selects a rarity, then selects a random hero from the rarity list
    /// </summary>
    /// <returns></returns>
    IEnumerator PreloadHeroes()
    {
        int randNum = 0;
        BaseHero lastHero1 = null;
        BaseHero lastHero2 = null;
        for (int i = 0; i < amountToPreload; i++)
        {
            randNum = Random.Range(0, 101);

            if (randNum < commonChance)
            {
                preloadedHeroes.Add(GetRandomCommonHero(lastHero1, lastHero2));
            }
            else if (randNum < commonChance + rareChance)
            {
                preloadedHeroes.Add(GetRandomRareHero(lastHero1, lastHero2));
            }
            else if (randNum < commonChance + rareChance + epicChance)
            {
                preloadedHeroes.Add(GetRandomEpicHero(lastHero1, lastHero2));
            }

            if (preloadedHeroes.Count > 0)
                lastHero1 = preloadedHeroes[preloadedHeroes.Count - 1];
            if (preloadedHeroes.Count > 1)
                lastHero2 = preloadedHeroes[preloadedHeroes.Count - 2];
        }

        yield return new WaitForEndOfFrame();
        LoadEncounters();
    }

    /// <summary>
    /// Tells encounterManager to load the encounters
    /// </summary>
    /// <returns></returns>
    public void LoadEncounters()
    {
        encounterManager.LoadEncounters();
    }

    BaseCard GetRandomCommonCard(BaseCard card1, BaseCard card2)
    {
        bool cardChosen = false;
        BaseCard cardToChoose = null;

        while(cardChosen == false) //while we do not have a card
        {
            cardToChoose = commonCards[Random.Range(0, commonCards.Count)]; //chooses a random card

            if (cardToChoose != card1 && cardToChoose != card2) //if the card is not the same as the last 2 card selected
                cardChosen = true;
        }

        return cardToChoose;
    }

    BaseCard GetRandomRareCard(BaseCard card1, BaseCard card2)
    {
        bool cardChosen = false;
        BaseCard cardToChoose = null;

        while (cardChosen == false) //while we do not have a card
        {
            cardToChoose = rareCards[Random.Range(0, rareCards.Count)]; //chooses a random card

            if (cardToChoose != card1 && cardToChoose != card2) //if the card is not the same as the last 2 card selected
                cardChosen = true;
        }

        return cardToChoose;
    }

    BaseCard GetRandomEpicCard(BaseCard card1, BaseCard card2)
    {
        bool cardChosen = false;
        BaseCard cardToChoose = null;

        while (cardChosen == false) //while we do not have a card
        {
            cardToChoose = epicCards[Random.Range(0, epicCards.Count)]; //chooses a random card

            if (cardToChoose != card1 && cardToChoose != card2) //if the card is not the same as the last 2 card selected
                cardChosen = true;
        }

        return cardToChoose;
    }

    BaseHero GetRandomCommonHero(BaseHero hero1, BaseHero hero2)
    {
        bool cardChosen = false;
        BaseHero heroToChoose = null;

        while (cardChosen == false) //while we do not have a card
        {
            heroToChoose = commonHeroes[Random.Range(0, commonHeroes.Count)]; //chooses a random card

            if (heroToChoose != hero1 && heroToChoose != hero2) //if the card is not the same as the last 2 card selected
                cardChosen = true;
        }

        return heroToChoose;
    }

    BaseHero GetRandomRareHero(BaseHero hero1, BaseHero hero2)
    {
        bool cardChosen = false;
        BaseHero heroToChoose = null;

        while (cardChosen == false) //while we do not have a card
        {
            heroToChoose = rareHeroes[Random.Range(0, rareHeroes.Count)]; //chooses a random card

            if (heroToChoose != hero1 && heroToChoose != hero2) //if the card is not the same as the last 2 card selected
                cardChosen = true;
        }

        return heroToChoose;
    }

    BaseHero GetRandomEpicHero(BaseHero hero1, BaseHero hero2)
    {
        bool cardChosen = false;
        BaseHero heroToChoose = null;

        while (cardChosen == false) //while we do not have a card
        {
            heroToChoose = epicHeroes[Random.Range(0, epicHeroes.Count)]; //chooses a random card

            if (heroToChoose != hero1 && heroToChoose != hero2) //if the card is not the same as the last 2 card selected
                cardChosen = true;
        }

        return heroToChoose;
    }

}

public enum Rarity
{
    NONE,
    COMMON,
    RARE,
    EPIC
}
