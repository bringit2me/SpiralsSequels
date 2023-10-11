using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class BaseMinion : MonoBehaviour
{
    [Header("Stats")]
    public int manaCost;
    public int attack;
    public int health;
    [Header("UI References")]
    [SerializeField] TMP_Text manaText;
    [SerializeField] TMP_Text attackText;
    [SerializeField] TMP_Text healthText;
}
