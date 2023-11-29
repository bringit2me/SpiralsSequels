using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class LoadShop : MonoBehaviour
{
    
    public int amountOfCardsInShop = 5;
    public GameObject cardUIButton;
    


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
            Instantiate(cardUIButton, this.gameObject.transform);

        }
    }
}
