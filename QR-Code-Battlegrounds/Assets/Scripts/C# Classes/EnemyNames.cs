using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyNames
{
    private static string[] EnemyNamesArray = { "Araxxi", "Occulus", "Buzzy", "Swabbie", "Ash", "Ivara", "Boomer", "Titania", "Jimbo", "Harrow" };

    public static string GetRandomName()
    {
        int rnd = Random.Range(0, EnemyNamesArray.Length);
        return EnemyNamesArray[rnd];
    }
}