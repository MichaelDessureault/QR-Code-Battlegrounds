using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

public class QRCode
{
    
    private string qrCodeID;
    private GeoPoint geoPoint;

    public string imageURL;
    public string userID;
    public string username;
    public float lat_d;
    public float lon_d;

    public QRCode()
    {
        GenerateQRCode();
    }

    public string QRCodeID
    {
        get { return qrCodeID; }
        set { qrCodeID = value; }
    }

    public string ImageURL
    {
        get { return imageURL; }
        set { imageURL = value; }
    }

    public string UserID
    {
        get { return userID; }
        set { userID = value; }
    }

    public float Latitude
    {
        get { return lat_d; }
        set { lat_d = value; }
    }

    public float Longitude
    {
        get { return lon_d; }
        set { lon_d = value; }
    }


    public GeoPoint GeoPoint
    {
        get
        {
            if (geoPoint == null)
                geoPoint = new GeoPoint(lat_d, lon_d);
            return geoPoint;
        }
    }

    public void GenerateQRCode()
    {
        qrCodeID = System.Guid.NewGuid().ToString().Replace('-', '0');
    }

    public static bool VALIDATE_QR_CODE_ID(string id)
    {
        return Regex.IsMatch(id, @"^[a-zA-Z0-9]+$");
    }
}
