using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerStats : MonoBehaviour
{
    [Header("Player Health")]
    public int playerHealth = 10;
    [Header("Combat Stats")]
    public int attackPower = 5;
    public int playerDefense = 5;
    [Header("Magic Stats")]
    public int spellsNumber = 0;
}