using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Preloader : MonoBehaviour
{
    public int seed;
    EncounterManager encounterManager;
    [Header("Preloaded Cards & Heroes")]
    public List<BaseCard> preloadedCards;
    public List<BaseHero> preloadedHeroes;
    [Header("Sideboard Card Lists")]
    public List<BaseCard> commonCards;
    public List<BaseCard> rareCards;
    public List<BaseCard> epicCards;
    [Header("Hero Lists")]
    public List<BaseHero> commonHeroes;
    public List<BaseHero> rareHeroes;
    public List<BaseHero> epicHeroes;

    private void Start()
    {
        encounterManager = GameObject.FindObjectOfType<EncounterManager>();
        if (seed == 0)
            seed = Random.Range(10000000, 100000000);

        Random.InitState(seed); //sets Random Number Generator seed

        SetupRarityLists();
    }

    /// <summary>
    /// Sets up all rarity lists (gets all cards and heroes inside the resources folder)
    /// </summary>
    void SetupRarityLists()
    {
        StartCoroutine(PreloadCards());
    }


    /// <summary>
    /// Preloads cards and adds them to the respective rarity list
    /// First selects a rarity, then selects a random card from the rarity list
    /// </summary>
    /// <returns></returns>
    IEnumerator PreloadCards()
    {
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

}

public enum Rarity
{
    NONE,
    COMMON,
    RARE,
    EPIC
}
