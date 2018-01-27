using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine; 
using System;


public class DatabaseConnector
{
    // Connection string
    private const string dbConnectionString = "";

    #region Singleton
    // Singleton setup
    private static DatabaseConnector _instance;

    public static DatabaseConnector Instance
    {
        get
        {
            if (_instance == null)
                _instance = new DatabaseConnector();
            return _instance;
        }
    }
    #endregion

    public DatabaseConnector () { }

    public List<GameObject> AreaCheck (GeoPoint playerGeo, float radius)
    {
        return RadarBinarySearch.Instance.Search(playerGeo, radius);
    }
}