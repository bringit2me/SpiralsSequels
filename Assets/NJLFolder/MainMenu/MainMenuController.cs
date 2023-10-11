using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MainMenuController : MonoBehaviour
{
    private PlayerInformation playerInformation;

    public TextMeshProUGUI playerRunesText;
    public TextMeshProUGUI playerGoldText;



    void Start()
    {
        playerInformation = GameObject.FindObjectOfType<PlayerInformation>();
    }

    // Update is called once per frame
    void Update()
    {
        playerGoldText.text = playerInformation.playerGold + "";
        playerRunesText.text = playerInformation.playerRunes + "";
    }


}
