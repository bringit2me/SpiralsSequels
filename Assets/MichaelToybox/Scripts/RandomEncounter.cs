using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomEncounter : MonoBehaviour
{
    public List<BaseHero> possibleEncounters;
    [Header("Random Encounter Info")]
    public bool playerGoesFirst = false;
    public int minEnemyCount = 1;
    public int maxEnemyCount = 3;
    public int enemyCount = 0;
    [Header("Hero Stat Scaling Info")]
    public int healthBoostPerMissingEnemy = 10;
    public int healthBoostPerLevel = 2;
    public int attackIncreasePerXFloors = 3;
    [Header("Heroes")]
    public List<BaseHero> heroesToSpawn;
    public List<BaseHero> heroesSpawned;
    public List<GameObject> heroSpawnSpots;

    EnemyManager enemyManager;

    private void Awake()
    {
        enemyManager = this.GetComponent<EnemyManager>();
    }
    /// <summary>
    /// Sets up encounter
    /// </summary>
    public void SetupEncounter()
    {
        enemyCount = Random.Range(minEnemyCount, maxEnemyCount + 1);

        for (int i = 0; i < enemyCount; i++)
        {
            heroesToSpawn.Add(GetHeroToSpawn());
        }

        SpawnEncounter();
    }

    public void SpawnEncounter()
    {
        //creates enemy heroes and positions them
        CreateAndPositionHeroes();

        //Updates hero health
        UpdateHeroStats();

        //Sets hero references in enemy manager and how many cards from each deck the enemy should draw (also updates enemy AI)
        UpdateEnemyManager();
    }

    public void CreateAndPositionHeroes()
    {
        for (int i = 0, q = heroesToSpawn.Count; i < q; i++)
        {
            RectTransform spawnedHero = Instantiate(heroesToSpawn[i], heroSpawnSpots[i].transform).GetComponent<RectTransform>();

            heroesSpawned.Add(spawnedHero.GetComponent<BaseHero>());
        }
    }

    public void UpdateHeroStats()
    {
        foreach(BaseHero hero in heroesSpawned)
        {
            hero.maxHealth = hero.maxHealth + (maxEnemyCount - enemyCount) * healthBoostPerMissingEnemy;
            hero.health = hero.maxHealth;
            hero.SetupCardText();
        }
    }

    public void ScaleHeroStats(int currentFloor)
    {
        int scaler = currentFloor - 1;

        int healthScale = 0;
        int attackScale = 0;
        int manaScale = 0;

        healthScale = (healthBoostPerLevel * scaler) * (1 + maxEnemyCount - enemyCount);

        attackScale = Mathf.FloorToInt(currentFloor / (attackIncreasePerXFloors * 1.0f));


        if (currentFloor > 4)
            manaScale = 1;

        foreach (BaseHero hero in heroesSpawned)
        {
            hero.maxHealth = hero.maxHealth + healthScale; //scales max health and health
            hero.health = hero.maxHealth;
            hero.attack = hero.attack + attackScale; //scales attack
            hero.SetBaseStats();
            hero.SetupCardText();
        }

        enemyManager.manaPerTurn += manaScale; //scales mana
        enemyManager.UpdateManaText();

    }

    public void UpdateEnemyManager()
    {
        if (enemyCount == 1)
            enemyManager.drawCountHero = 9;
        else if (enemyCount == 2)
            enemyManager.drawCountHero = 4;
        else if (enemyCount == 3)
            enemyManager.drawCountHero = 3;

        enemyManager.GetChildHeroReferences();
        enemyManager.UpdateEnemyAI();
    }

    /// <summary>
    /// Gets a hero from the possibleEncounters list that is not already in the heroesToSpawn list
    /// </summary>
    /// <returns></returns>
    public BaseHero GetHeroToSpawn()
    {
        BaseHero heroToSpawn = null;
        while (heroToSpawn == null)
        {
            //Random number representing which hero to choose from the possible encounter
            int randomHero = Random.Range(0, possibleEncounters.Count);

            if (heroesToSpawn.Contains(possibleEncounters[randomHero]) == false)
            {
                heroToSpawn = possibleEncounters[randomHero];
            }
        }

        return heroToSpawn;
    }
}
