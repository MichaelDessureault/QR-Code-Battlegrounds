using Firebase;
using Firebase.Database;
using System.Collections.Generic;
using UnityEngine;

public class RadarPing : MonoBehaviour
{

    private const float LATDEGTOKM = 111f;
    private const float ONEKMTOLATDEG = 1 / LATDEGTOKM; // 0.009f

    public GameObject player;
    [SerializeField] private float geoPingRadiusMeter = 50f; // 50 meters

    public void Ping()
    {
        // check for enemies in the local area ~ 20 unities in all directions
        GeoPoint playerGeoPosition = GeoGameManager.Instance.GetMainMapMap().getPositionOnMap(new Vector2(player.transform.position.x, player.transform.position.z));
        float radius = (geoPingRadiusMeter / 1000) * ONEKMTOLATDEG; // ( 50 / 1000 ) * 0.009 = 0.00045f 

        List<GameObject> enemies = DatabaseConnector.Instance.AreaCheck(playerGeoPosition, radius);

        foreach (var enemy in enemies)
        {
            SpawnEnemy(enemy);
        }
    }

    void SpawnEnemy(GameObject enemy)
    {
        enemy.GetComponent<ObjectPosition>().SetPositionOnMap();
        enemy.GetComponent<MeshRenderer>().enabled = true;
    }
}
