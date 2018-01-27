using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;


public class FakeDatabaseController : DatabaseConnector
{
    ////private static Dictionary<GameObject, GeoPoint> enemiesDictionary = new Dictionary<GameObject, GeoPoint>();
    //private static List<GeoPoint> enemies_geo_list = new List<GeoPoint>();
    ////private bool startEnemies = true;


    //// Database stuff
    //public List<GeoPoint> GetEnemiesFromDatabase()
    //{
    //    return enemies_geo_list;
    //}

    //int enemycounter = 0;

    //private string NewName()
    //{
    //    enemycounter++;
    //    return "enemy" + enemycounter;
    //}

    //private void GenerateEnemies(int amount)
    //{
    //    if (amount > 0)
    //    {
    //        for (int i = 0; i < amount; i++)
    //        {
    //            GenerateEnemy();
    //        }
    //    }
    //}

    //// random type
    //private void GenerateEnemy()
    //{
    //    StylesEnum.CombatTypes type;

    //    switch (Random.Range(0, 3))
    //    {
    //        case 0: type = StylesEnum.CombatTypes.assassin; break;
    //        case 1: type = StylesEnum.CombatTypes.guardian; break;
    //        default: type = StylesEnum.CombatTypes.wizard; break;
    //    }

    //    var enemy = CreateACharacterOrEnemyGameObj.CreateEnemy(name: "enemy1", type: type, lat: 0.1f, lon: 0.1f);
    //    //addEnemyToDatabaseList(enemy);
    //}

    //// passed type
    //private void GenerateEnemy(StylesEnum.CombatTypes type)
    //{
    //    var enemy = CreateACharacterOrEnemyGameObj.CreateEnemy(name: "enemy1", type: type, lat: 0.1f, lon: 0.1f);
    //    //addEnemyToDatabaseList(enemy);
    //}

}