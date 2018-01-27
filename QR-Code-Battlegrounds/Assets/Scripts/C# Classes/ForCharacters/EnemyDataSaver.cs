using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class EnemyDataSaver {

    public string name;
    public StylesEnum.CombatTypes type;
    public Sprite sprite;
    public bool isBoss = false;

    [SerializeField] private float lat_d;
    [SerializeField] private float lon_d;
    private GeoPoint geoPoint; 
    
    public GeoPoint GeoPoint
    {
        get
        {
            if (geoPoint == null)
                geoPoint = new GeoPoint(lat_d, lon_d);
            return geoPoint;
        }
    }
    
    public EnemyDataSaver(string _name, StylesEnum.CombatTypes _type, float lat, float lon, Sprite _sprite, bool _isBoss)
    {
        name = _name;
        type = _type;
        lat_d = lat;
        lon_d = lon;
        sprite = _sprite;
        isBoss = _isBoss;
    }
}