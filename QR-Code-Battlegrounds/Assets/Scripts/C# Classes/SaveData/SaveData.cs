using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;

public class SaveData : MonoBehaviour {
    public const string character1JsonFile = "/Character1.json";
    public const string character2JsonFile = "/Character2.json";
    public const string character3JsonFile = "/Character3.json";

    public const string character1AbilitySetJsonFile = "/Character1AbilitySet.json";
    public const string character2AbilitySetJsonFile = "/Character2AbilitySet.json";
    public const string character3AbilitySetJsonFile = "/Character3AbilitySet.json";
    public const string storeDataJsonFile = "/StoreData.json";
    public const string lastDatabaseUpdateFile = "/LastUpdateInfo.json";
    
    public static string[] characterSavePaths = new string[GameManager.numberOfCharacters]
        { character1JsonFile, character2JsonFile, character3JsonFile };

    public static string[] characterAbilitySetSavePaths = new string[GameManager.numberOfCharacters]
        { character1AbilitySetJsonFile, character2AbilitySetJsonFile, character3AbilitySetJsonFile };

    private static bool saving = false;

    // Saving
    public static void Save() { 
        if (!saving) {
            saving = true;
            SaveCharacterStats();
            SaveCharacterAbilitySets();
            SavePotionData();
            saving = false;
        }
    }

    private static void SaveCharacterStats() {
        Player[] characters = GameManager.Instance.GetCharactersArrayAsPlayer();
        if (characters != null) {
            for (int i = 0; i < GameManager.numberOfCharacters; i++) {
                Stats stats = characters[i].Stats_Component;

                if (stats != null) {
                    string jsonString = stats.ToJsonString();
                    File.WriteAllText(Application.persistentDataPath + characterSavePaths[i], jsonString);
                }
            }
        }
    }


    private static void SaveCharacterAbilitySets() {
        Player[] characters = GameManager.Instance.GetCharactersArrayAsPlayer();
        if (characters != null) {
            for (int i = 0; i < GameManager.numberOfCharacters; i++) {
                try {
                    Player player = characters[i];

                    string jsonString = player.AbilitySetToJson();
                    if (jsonString.Equals(string.Empty))
                        continue;

                    File.WriteAllText(Application.persistentDataPath + characterAbilitySetSavePaths[i], jsonString);
                } catch (Exception e) {
                    print(e.Message);
                }
            }
        }
    }


    private static void SavePotionData() {
        string jsonString = StoreData.Instance.ToJsonString();
        File.WriteAllText(Application.persistentDataPath + storeDataJsonFile, jsonString);
    }


    public static void SaveDatabaseUpdateTime(DateTime date)
    {
        File.WriteAllText(Application.persistentDataPath + lastDatabaseUpdateFile, date.Ticks.ToString());
    }

    
    public static DateTime? LoadDatabaseUpdateTime()
    {
        string path = Application.persistentDataPath + lastDatabaseUpdateFile;
        if (File.Exists(path))
        {
            string dateString = File.ReadAllText(path);
            long dateLong = long.Parse(dateString);
            return new DateTime(dateLong);
        }
        return null;
    }


    // Loading 
    public static List<Stats> LoadCharacterStats() {
        List<Stats> characterStatsArray = new List<Stats>();

        for (int i = 0; i < GameManager.numberOfCharacters; i++) {
            try {
                string jsonString = File.ReadAllText(Application.persistentDataPath + characterSavePaths[i]);
                Stats stats = JsonUtility.FromJson<Stats>(jsonString);
                characterStatsArray.Add(stats);
            } catch (Exception) {
                characterStatsArray.Add(null);
            }
        }

        return characterStatsArray;
    }


    public static List<List<Skill>> LoadCharacterAbilitySets() {
        List<List<Skill>> characterAbilitySetArray = new List<List<Skill>>();
        Ability abilityInstance = Ability.Instance;
        for (int i = 0; i < GameManager.numberOfCharacters; i++) {
            List<Skill> abilitySet = new List<Skill>();

            try {
                string jsonString = File.ReadAllText(Application.persistentDataPath + characterAbilitySetSavePaths[i]);
                string[] separator = new string[] { "}" };
                string[] jsonStrings = jsonString.Split(separator, StringSplitOptions.RemoveEmptyEntries);

                // Append the "}" back onto each string
                for (int j = 0; j < 3; j++) {
                    jsonStrings[j] += "}";
                }

                foreach (string js in jsonStrings) {
                    AbilityInformation ai = JsonUtility.FromJson<AbilityInformation>(js);
                    Skill s = abilityInstance.GetSkillsByUniqueId(ai.stylesEnum, ai.uniqueId);
                    abilitySet.Add(s);
                }

                characterAbilitySetArray.Add(abilitySet);
            } catch (Exception e) {
                characterAbilitySetArray.Add(abilitySet);
            }
        }
        return characterAbilitySetArray;
    }


    public static void LoadStoreData() {
        try {
            string jsonString = File.ReadAllText(Application.persistentDataPath + storeDataJsonFile);
            StoreData.Instance = JsonUtility.FromJson<StoreData>(jsonString);
        } catch (Exception e) {
            // not found... create it by calling "Instance"
            StoreData.Instance = new StoreData();
        }
    }

    
    public static void Deletefiles() {
        print("Deleting Files for the following paths");
        print(Application.persistentDataPath + storeDataJsonFile);
        
        File.Delete(Application.persistentDataPath + storeDataJsonFile);

        foreach (string s in characterSavePaths) {
            print(Application.persistentDataPath + s);
            File.Delete(Application.persistentDataPath + s);
        }

        foreach (string s in characterAbilitySetSavePaths) {
            print(Application.persistentDataPath + s);
            File.Delete(Application.persistentDataPath + s);
        }
    }
}