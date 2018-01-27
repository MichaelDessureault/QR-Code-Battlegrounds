using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

[Serializable]
public class StoreData
{
    [SerializeField] private int potionAmount = 10;
    [SerializeField] private int etherAmount = 10;
    [SerializeField] private int coins = 1000;

    #region singleton
    private static StoreData _instance; 

    public static StoreData Instance
    {
        get { return _instance; } 
        set { if (_instance == null) { _instance = value; } }
    }
    #endregion

    public StoreData() {
    }

    public int PotionAmount
    {
        get { return potionAmount; }
        set { potionAmount = value; }
    }
    public int EtherAmount
    {
        get { return etherAmount; }
        set { etherAmount = value; }
    }
    public int CoinAmount
    {
        get { return coins;  }
        set { coins = value; }
    }
    public void PotionUsed()
    {
        potionAmount--;
    }

    public void EtherUsed ()
    {
        etherAmount--;
    }

    public string ToJsonString()
    {
        return JsonUtility.ToJson(this);
    }
}