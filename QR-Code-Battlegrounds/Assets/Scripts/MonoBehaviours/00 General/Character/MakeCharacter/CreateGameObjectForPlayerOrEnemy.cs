using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.EventSystems;
using System.Linq;

public class CreateGameObjectForPlayerOrEnemy : MonoBehaviour {
    public static GameObject NewCharacter(string name, StylesEnum.CombatTypes type, Stats stats = null, List<Skill> skillset = null) {
        var obj = CreateCubeObject(name);

        if (stats == null) {
            stats = new Stats();
            stats.NewStatsSetup(type);
        }

        // Add the script components
        var player = obj.AddComponent<Player>();
        player.name = name;
        player.Stats_Component = stats;
        player.CombatType = type;

        if (skillset == null || skillset.Count == 0)
            player.AbilitySetSetup();
        else
            player.AbilitySet = skillset;

        Sprites spritesInstance = Sprites.Instance;
        player.FrontSprite = spritesInstance.GetCharacterFrontSpriteForCombatType(type);
        player.BackSprite = spritesInstance.GetCharacterBackSpriteForCombatType(type);
        player.Experience_Component = new Experience(stats);

        return obj;
    }

    public static GameObject CreateEnemy(string name, StylesEnum.CombatTypes type, GeoPoint geoPoint, Sprite sprite, bool isBoss) {
        // creates a general cube gameobject
        GameObject obj = CreateBushObj(name);

        // Add the extra enemy related components
        var enemy = obj.AddComponent<Enemy>();
        enemy.name = name;
        enemy.Stats_Component = new Stats(type);
        enemy.AbilitySetSetup();
        enemy.FrontSprite = sprite;
        enemy.CombatType = type;
        enemy.IsBoss = isBoss;

        // Setup event trigger for enemy
        obj.AddComponent<EventTrigger>();
        obj.AddComponent<EnemyEventTrigger>();
        obj.AddComponent<ObjectPosition>().SetLatLon(geoPoint);

        return obj;
    }


    public static GameObject CreateEnemyFromQRCode(QRCode code, StylesEnum.CombatTypes type, Sprite sprite, bool isBoss)
    {
        // creates a general cube gameobject
        GameObject obj = CreateBushObj(code.username);

        // Add the extra enemy related components
        var enemy = obj.AddComponent<Enemy>();
        enemy.Stats_Component = new Stats(type);
        enemy.AbilitySetSetup();
        enemy.CombatType = type;
        enemy.IsBoss = isBoss;
        enemy.name = code.username;
        enemy.code = code;
        if (!String.IsNullOrEmpty(code.imageURL) && Uri.IsWellFormedUriString(code.imageURL, UriKind.Absolute))
        {
            enemy.GetEnemySpriteFromURL(code.imageURL);
        }
        else
        {
            enemy.FrontSprite = sprite;
        }
        // Setup event trigger for enemy
        obj.AddComponent<EventTrigger>();
        obj.AddComponent<EnemyEventTrigger>();
        obj.AddComponent<ObjectPosition>().SetLatLon(code.GeoPoint);

        return obj;
    }


    private static GameObject CreateCubeObject(string name) {
        // Add the required componenets
        GameObject obj = GameObject.CreatePrimitive(PrimitiveType.Cube);
        obj.name = name;
        obj.GetComponent<MeshRenderer>().enabled = false;
        return obj;
    }

    private static GameObject CreateBushObj(string name) {
        // Add the required componenets
        GameObject obj = Instantiate(GameManager.Instance.bushPrefab);
        obj.name = name;
        obj.GetComponent<MeshRenderer>().enabled = false;
        return obj;
    }

}
