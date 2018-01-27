using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class RadarBinarySearch
{
    // Variables
    private List<EnemyDataSaver> list_from_the_database = new List<EnemyDataSaver>();  // array of values
    
    #region Singleton
    // Singleton setup
    private static RadarBinarySearch _instance;

    public static RadarBinarySearch Instance
    {
        get {
            if (_instance == null) 
                _instance = new RadarBinarySearch();
            return _instance;
        }
    }
    #endregion

    public RadarBinarySearch() { }
    
    public List<GameObject> Search(GeoPoint searchCoordinate, float radius)
    {
        int position = -1; // location of search key in array
 
        list_from_the_database = new List<EnemyDataSaver>(); // allocate space for array

        for (int i = 0; i < 100; i++)
        {
            list_from_the_database.Add(ForceGeoPoint(searchCoordinate, radius));
        }

        // Sorts the list using the GeoPoint from the EnemyDataSaver script
        list_from_the_database.Sort((c1, c2) => (c1).GeoPoint.CoordinateSqrMagnitude().CompareTo((c2).GeoPoint.CoordinateSqrMagnitude()));
        
        // use binary search to try to find integer
        position = BinarySearchWithRadius(searchCoordinate);

        // return value of -1 indicates integer was not found
        if (position != -1)
        {
            // get the values around +10 to -10 this middle index 
            List<EnemyDataSaver> foundEnemies = FindPointsAroundLocation(position, 1f);

            if (foundEnemies.Count != 0)
            {
                return MakeFoundEnemiesIntoGameObjects(foundEnemies);
            }
        }
        return null;
    }
    
    public int BinarySearchWithRadius(GeoPoint searchElement)
    {
        int low = 0; // 0 is always going to be the first element
        int high = list_from_the_database.Count - 1; // Find highest element
        int middle = (low + high + 1) / 2; // Find middle element
        int location = -1; // Return value -1 if not found

        do // Search for element
        {
            // if an element is found within the range of the players coordinates it returns that position
            // a check will be done after to check around the found position for more enemies
            if (WithinRanage(searchElement, list_from_the_database[middle].GeoPoint, 1f))
                return middle;

            // if element is found at middle
            if (searchElement.IsEqual(list_from_the_database[middle].GeoPoint))
                location = middle; // location is current middle
            // middle element is too high
            else if (searchElement.CoordinateMagnitude() < list_from_the_database[middle].GeoPoint.CoordinateMagnitude())
                high = middle - 1; // eliminate lower half
            else // middle element is too low
                low = middle + 1; // eleminate lower half

            middle = (low + high + 1) / 2; // recalculate the middle  
        } while ((low <= high) && (location == -1));

        return location; // return location of search key
    }
    
    private List<EnemyDataSaver> FindPointsAroundLocation(int indexlocation, float range)
    {
        EnemyDataSaver temp = list_from_the_database[indexlocation];

        List<EnemyDataSaver> foundEnemies = new List<EnemyDataSaver>() { temp };

        bool leftcontinue = true;
        bool rightcontinue = true;
        var max = list_from_the_database.Count;

        for (int i = 1; i < 3; i++)
        {
            var left = indexlocation - i;
            var right = indexlocation + i;

            // left of starting index
            if (left >= 0 && leftcontinue && WithinRanage(list_from_the_database[left].GeoPoint, temp.GeoPoint, range))
                foundEnemies.Add(list_from_the_database[left]);
            else
                leftcontinue = false;

            // right of starting index
            if (right < max && rightcontinue && WithinRanage(list_from_the_database[right].GeoPoint, temp.GeoPoint, range))
                foundEnemies.Add(list_from_the_database[right]);
            else
                rightcontinue = false;

            if (!leftcontinue && !rightcontinue)
                break;
        }

        return foundEnemies;
    }

    private bool WithinRanage(GeoPoint searchElement, GeoPoint listElement, float range)
    {
        var d = searchElement.Distance(listElement.ToVector2());

        return (d <= range);
    }
    
    private List<GameObject> MakeFoundEnemiesIntoGameObjects (List<EnemyDataSaver> list)
    {
        // Find the enemy holder to store the enemy in
        GameObject enemyHolder = GameObject.Find("Enemy Holder");
        if (enemyHolder == null)
            enemyHolder = new GameObject("Enemy Holder");


        List<GameObject> objList = new List<GameObject>();

        foreach (EnemyDataSaver eds in list)
        {
            var obj = CreateGameObjectForPlayerOrEnemy.CreateEnemy(eds.name, eds.type, eds.GeoPoint, eds.sprite, eds.isBoss);
            obj.GetComponent<Transform>().parent = enemyHolder.GetComponent<Transform>();
            objList.Add(obj);
        }

        return objList;
    }

    // temp until database implementation 
    private EnemyDataSaver ForceGeoPoint(GeoPoint playersgeo, float radius)
    {
        var lat = playersgeo.lat_d;
        var lon = playersgeo.lon_d;
        
        var temp  = Random.Range(-(radius * 2), (radius * 2));
        var temp2 = Random.Range(-(radius * 2), (radius * 2));

        return new EnemyDataSaver(EnemyNames.GetRandomName(), StylesEnum.GetRandomType(), lat + temp, lon + temp2, 
                                    Sprites.Instance.GetRandomEnemySprite(), (Random.Range(1, 4) == 1));
    }
}