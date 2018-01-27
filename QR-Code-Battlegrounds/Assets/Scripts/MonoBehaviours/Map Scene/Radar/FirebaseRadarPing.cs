using System.Collections.Generic;
using System.Linq;
using System.Text;
using System;
using System.Collections;
using System.Collections.Generic;
using Firebase;
using Firebase.Database;
using Firebase.Unity.Editor;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;


public class FirebaseRadarPing : MonoBehaviour
{

    public int resolution_distance;

    private FirebaseDatabaseConnector database;

    private const float KMS_IN_LATITUDE = 111f;

    private const float KM_TO_LATITUDE = 1 / KMS_IN_LATITUDE; // 0.009f

    private const float METER_TO_LATITUDE = KM_TO_LATITUDE / 1000;

    public GameObject player;
    
    //Output Text
    public Text message;

    private void Start()
    {
        database = FindObjectOfType<FirebaseDatabaseConnector>();
    }


    //Radar Ping from Firebase Database
    public void FirebasePing()
    {
        LoadFromDatabaseInDistance();
    }


    public void DisplayEnemies(List<QRCode> codes)
    {
        List<GameObject> enemies = MakeFoundEnemiesIntoGameObjects(codes);
        foreach (var enemy in enemies)
        {
            SpawnEnemy(enemy);
        }
    }
 

    private void LoadFromDatabase()
    {
        if (database != null)
        {
            FirebaseDatabaseConnector.QRCodeListCallback callback = LoadFromDatabase;
            database.GetQRCodes(callback);
        }
    }


    private void LoadFromDatabase(List<QRCode> codes, string reply)
    {
        message.text = reply;
        if (codes != null)
        {
            DisplayEnemies(codes);
        }
    }


    private void LoadFromDatabaseInDistance()
    {
        if (database != null)
        {
            FirebaseDatabaseConnector.QRCodeListCallback callback = LoadFromDatabaseInDistance;
            GeoPoint player_loc = GetPlayerLocation();
            float resolution = GetResolutionDistanceInDegrees();
            float start = player_loc.lat_d - resolution;
            float end = player_loc.lat_d + resolution;
            database.GetQRCodesWithinLatitudes(callback, start, end);
        }
    }


    private void LoadFromDatabaseInDistance(List<QRCode> codes, string reply)
    {
        message.text = reply;
        if (codes != null)
        {
            foreach(var code in codes)
            {
                float distance = GoogleStaticMap.haversine_dist(code.GeoPoint, GetPlayerLocation());
                if(distance<=resolution_distance)
                {
                    Debug.Log("Enenmy found at location " + code.GeoPoint.ToString());
                }
            }
            DisplayEnemies(codes);
        }
    }


    private void SpawnEnemy(GameObject enemy)
    {
        enemy.GetComponent<ObjectPosition>().SetPositionOnMap();
        enemy.GetComponent<MeshRenderer>().enabled = true;
    }


    private bool WithinRanage(GeoPoint searchElement, GeoPoint listElement, float range)
    {
        var d = searchElement.Distance(listElement.ToVector2());
        return (d <= range);
    }


    //Resolution distance in degrees
    private float GetResolutionDistanceInDegrees()
    {
        return resolution_distance * METER_TO_LATITUDE;
    }


    private List<GameObject> MakeFoundEnemiesIntoGameObjects(List<QRCode> list)
    {
        // Find the enemy holder to store the enemy in
        Destroy(GameObject.Find("Enemy Holder"));
        GameObject enemyHolder = new GameObject("Enemy Holder");

        List<GameObject> objList = new List<GameObject>();

        foreach (QRCode code in list)
        {
            var obj = CreateGameObjectForPlayerOrEnemy.CreateEnemyFromQRCode(code, StylesEnum.GetRandomType(), Sprites.Instance.GetRandomEnemySprite(), false);
            obj.GetComponent<Transform>().parent = enemyHolder.GetComponent<Transform>();
            objList.Add(obj);
        }

        return objList;
    }

    
    private GeoPoint GetPlayerLocation()
    {
        return GeoGameManager.Instance.GetMainMapMap().getPositionOnMap(new Vector2(player.transform.position.x, player.transform.position.z));
    }

}