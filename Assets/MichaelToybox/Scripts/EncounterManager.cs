using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EncounterManager : MonoBehaviour
{
    [Header("Floor Info")]
    public int seed = 0;
    public GameObject nextEncounter;
    [Header("Floor 1 Loaded Encounters")]
    [SerializeField] List<RandomEncounter> randomEncountersFloor1;
    [SerializeField] List<GameObject> eliteEncountersFloor1;
    [SerializeField] GameObject bossEncounterFloor1;
    [Header("Floor 1 Info")]
    [SerializeField] GameObject floor1;
    [SerializeField] RandomEncounter floor1Random;
    [SerializeField] List<GameObject> possibleFloor1Elites;
    [SerializeField] List<GameObject> possibleFloor1Bosses;
    [Header("Floor 2 Loaded Encounters")]
    [SerializeField] List<RandomEncounter> randomEncountersFloor2;
    [SerializeField] List<GameObject> eliteEncountersFloor2;
    [SerializeField] GameObject bossEncounterFloor2;
    [Header("Floor 2 Info")]
    [SerializeField] GameObject floor2;
    [SerializeField] RandomEncounter floor2Random;
    [SerializeField] List<GameObject> possibleFloor2Elites;
    [SerializeField] List<GameObject> possibleFloor2Bosses;
    [Header("Floor 3 Loaded Encounters")]
    [SerializeField] List<RandomEncounter> randomEncountersFloor3;
    [SerializeField] List<GameObject> eliteEncountersFloor3;
    [SerializeField] GameObject bossEncounterFloor3;
    [Header("Floor 3 Info")]
    [SerializeField] GameObject floor3;
    [SerializeField] RandomEncounter floor3Random;
    [SerializeField] List<GameObject> possibleFloor3Elites;
    [SerializeField] List<GameObject> possibleFloor3Bosses;

    int count = 0;

    private void Start()
    {
        count = 0;

        if (seed == 0)
            seed = Random.Range(10000000, 100000000);

        LoadEncounters(seed);
    }

    public GameObject GetRandomEncounter(int currentFloor)
    {
        GameObject encounter = null;

        if (currentFloor == 1) //we want an encounter from floor 1
        {
            encounter = randomEncountersFloor1[0].gameObject; //gets first encounter from floor 1
            randomEncountersFloor1.RemoveAt(0); //Removes encounter from list
        }
        else if (currentFloor == 2) //we want an encounter from floor 2
        {
            encounter = randomEncountersFloor2[0].gameObject; //gets first encounter from floor 2
            randomEncountersFloor2.RemoveAt(0); //Removes encounter from list
        }
        else if (currentFloor == 3) //we want an encounter from floor 3
        {
            encounter = randomEncountersFloor3[0].gameObject; //gets first encounter from floor 3
            randomEncountersFloor3.RemoveAt(0); //Removes encounter from list
        }

        encounter.transform.localScale = Vector3.one; //resets encounters scale
        return encounter; //returns encounter
    }

    public GameObject GetEliteEncounter(int currentFloor)
    {
        GameObject encounter = null;

        if (currentFloor == 1) //we want an encounter from floor 1
        {
            encounter = eliteEncountersFloor1[0].gameObject; //gets first elite from floor 1
            eliteEncountersFloor1.RemoveAt(0); //Removes encounter from list
        }
        else if (currentFloor == 2) //we want an encounter from floor 2
        {
            encounter = eliteEncountersFloor2[0].gameObject; //gets first elite from floor 2
            eliteEncountersFloor2.RemoveAt(0); //Removes encounter from list
        }
        else if (currentFloor == 3) //we want an encounter from floor 3
        {
            encounter = eliteEncountersFloor3[0].gameObject; //gets first elite from floor 3
            eliteEncountersFloor3.RemoveAt(0); //Removes encounter from list
        }

        encounter.transform.localScale = Vector3.one; //resets encounters scale
        return encounter; //returns encounter
    }

    public GameObject GetBossEncounter(int currentFloor)
    {
        GameObject encounter = null;

        if (currentFloor == 1) //we want an encounter from floor 1
        {
            encounter = bossEncounterFloor1;
        }
        else if (currentFloor == 2) //we want an encounter from floor 2
        {
            encounter = bossEncounterFloor2;
        }
        else if (currentFloor == 3) //we want an encounter from floor 3
        {
            encounter = bossEncounterFloor3;
        }

        encounter.transform.localScale = Vector3.one; //resets encounters scale
        return encounter; //returns encounter
    }



    /// <summary>
    /// Loads in encounters based on the seed passed in
    /// </summary>
    /// <param name="passedInSeed"></param>
    void LoadEncounters(int passedInSeed)
    {
        this.seed = passedInSeed;

        Random.InitState(seed);

        StartCoroutine(LoadEncountersFloor1());
    }

    IEnumerator LoadEncountersFloor1()
    {
        count = 0; //resets count

        //Shuffles lists for elites and bosses
        possibleFloor1Elites.Shuffle();
        possibleFloor1Bosses.Shuffle();

        yield return new WaitForEndOfFrame();

        for (int i = 0; i < 10; i++) //loops to get 10 random encounters
        {
            randomEncountersFloor1.Add(Instantiate(floor1Random, floor1.transform)); //creates encounter
        }

        yield return new WaitForEndOfFrame();

        for (int i = 0; i < 10; i++) //loops to get 10 random elites
        {
            eliteEncountersFloor1.Add(Instantiate(possibleFloor1Elites[count], floor1.transform)); //generates random elite

            count = Mathf.Clamp(count + 1, 0, possibleFloor1Elites.Count - 1); //increases count
        }
        yield return new WaitForEndOfFrame();

        bossEncounterFloor1 = Instantiate(possibleFloor1Bosses[0], floor1.transform); //gets boss encounter

        yield return new WaitForEndOfFrame();

        StartCoroutine(LoadEncountersFloor2());
    }

    IEnumerator LoadEncountersFloor2()
    {
        count = 0; //resets count

        //Shuffles lists for elites and bosses
        possibleFloor2Elites.Shuffle();
        possibleFloor2Bosses.Shuffle();

        yield return new WaitForEndOfFrame();

        for (int i = 0; i < 10; i++) //loops to get 10 random encounters
        {
            randomEncountersFloor2.Add(Instantiate(floor2Random, floor2.transform));
        }

        yield return new WaitForEndOfFrame();

        for (int i = 0; i < 10; i++) //loops to get 10 random elites
        {
            eliteEncountersFloor2.Add(Instantiate(possibleFloor2Elites[count], floor2.transform)); //generates random elite

            count = Mathf.Clamp(count + 1, 0, possibleFloor2Elites.Count - 1); //increases count
        }

        yield return new WaitForEndOfFrame();

        bossEncounterFloor2 = Instantiate(possibleFloor2Bosses[0], floor2.transform); //gets boss encounter

        yield return new WaitForEndOfFrame();

        StartCoroutine(LoadEncountersFloor3());
    }

    IEnumerator LoadEncountersFloor3()
    {
        count = 0; //resets count

        //Shuffles lists for elites and bosses
        possibleFloor3Elites.Shuffle();
        possibleFloor3Bosses.Shuffle();

        yield return new WaitForEndOfFrame();

        for (int i = 0; i < 10; i++) //loops to get 10 random encounters
        {
            randomEncountersFloor1.Add(Instantiate(floor3Random, floor3.transform));
        }

        yield return new WaitForEndOfFrame();

        for (int i = 0; i < 10; i++) //loops to get 10 random elites
        {
            eliteEncountersFloor3.Add(Instantiate(possibleFloor3Elites[count], floor3.transform)); //generates random elite

            count = Mathf.Clamp(count + 1, 0, possibleFloor3Elites.Count - 1); //increases count
        }

        yield return new WaitForEndOfFrame();

        bossEncounterFloor3 = Instantiate(possibleFloor3Bosses[0], floor3.transform); //gets boss encounter

        yield return new WaitForEndOfFrame();

        //LOADING DONE
        FinishedLoading();
    }

    /// <summary>
    /// Turns off all encounters
    /// </summary>
    public void FinishedLoading()
    {
        //Turns off random encounters
        foreach (RandomEncounter obj in randomEncountersFloor1)
        {
            obj.SetupEncounter();
            obj.gameObject.SetActive(false);
        }
        foreach (RandomEncounter obj in randomEncountersFloor2)
        {
            obj.SetupEncounter();
            obj.gameObject.SetActive(false);
        }
        foreach (RandomEncounter obj in randomEncountersFloor3)
        {
            obj.SetupEncounter();
            obj.gameObject.SetActive(false);
        }

        //Turns off elite encounters
        foreach (GameObject obj in eliteEncountersFloor1)
            obj.SetActive(false);
        foreach (GameObject obj in eliteEncountersFloor2)
            obj.SetActive(false);
        foreach (GameObject obj in eliteEncountersFloor3)
            obj.SetActive(false);

        //Turns off boss encounters
        bossEncounterFloor1.SetActive(false);
        bossEncounterFloor2.SetActive(false);
        bossEncounterFloor3.SetActive(false);

    }
}
