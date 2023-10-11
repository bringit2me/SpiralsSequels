using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseCard : MonoBehaviour
{
    public new string name;
    [TextArea(5, 15)]
    public string description;
    [Header("Stats")]
    public int manaCost;
}
