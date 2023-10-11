using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInformation : MonoBehaviour
{
    [Header("PLAYER INFORMATION")]
    public int playerRunes;
    public int playerGold;
    void Start()
    {
        DontDestroyOnLoad(this);
    }

    private void Update()
    {
        AddGoldToPlayer();
        AddRunesToPlayer();
    }

    void AddGoldToPlayer()
    {
        if(Input.GetKeyDown(KeyCode.P))
        {
            playerGold += 10;
        }
    }

    void AddRunesToPlayer()
    {
        if (Input.GetKeyDown(KeyCode.O))
        {
            playerRunes += 10;
        }
    }
}
