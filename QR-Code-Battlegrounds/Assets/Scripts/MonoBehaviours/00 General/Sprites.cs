using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Do note that Sprites script has an Editor created for it, any variables added with this script have to be added in the Editor script as well
/// </summary>
public class Sprites : MonoBehaviour {

    #region singleton

    private static Sprites _instance;

    public static Sprites Instance
    {
        get {
            if (_instance == null)
            {
                _instance = FindObjectOfType<Sprites>();

                if (_instance == null)
                    Debug.LogError("No object found with Sprites script attachted to it");
            }
            return _instance;
        }
    }

    #endregion

    // variables
    public Sprite[] enemySprites;
    public Sprite[] characterFrontSprites;
    public Sprite[] characterBackSprites;
    public Material[] battleSceneBackgroundMaterials;

    public int BackgroundLength { get { return battleSceneBackgroundMaterials.Length; } }

    // Start (safety check)
    private void Start() {

        if (enemySprites.Length == 0 || characterFrontSprites.Length != GameManager.numberOfCharacters || characterBackSprites.Length == GameManager.numberOfCharacters) {
            Debug.LogWarning("Some sprites are missing in the Sprites script attachted to " + gameObject.name + " game object.  \n " +
                "Characters must have a front and back image populated");
        }

        _instance = this;
    }

    // Getters
    public Sprite GetRandomEnemySprite ()
    {
        return enemySprites[Random.Range(0, enemySprites.Length)];
    }

    public Sprite GetCharacterFrontSpriteForCombatType(StylesEnum.CombatTypes combatType)
    {
        if (combatType == StylesEnum.CombatTypes.all)
            return characterFrontSprites[0]; // default to the first Sprite in the array

        return characterFrontSprites[(int) combatType];
    }

    public Sprite GetCharacterBackSpriteForCombatType(StylesEnum.CombatTypes combatType)
    {
        if (combatType == StylesEnum.CombatTypes.all)
            return characterBackSprites[0]; // default to the first Sprite in the array

        return characterBackSprites[(int)combatType];
    }
}
