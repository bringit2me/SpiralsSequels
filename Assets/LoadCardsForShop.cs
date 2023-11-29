using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadCardsForShop : MonoBehaviour
{
    public List<GameObject> cardsAvailableInShop;
    public int amountOfCardsInShop = 5;
    



    void Start()
    {
        LoadCardsInShop();
    }

    // Update is called once per frame
    void Update()
    {

    }

    void LoadCardsInShop()
    {
        for (int i = 0; i < amountOfCardsInShop; i++)
        {
            Instantiate(cardsAvailableInShop[i], this.gameObject.transform); 

        }
    }
}
