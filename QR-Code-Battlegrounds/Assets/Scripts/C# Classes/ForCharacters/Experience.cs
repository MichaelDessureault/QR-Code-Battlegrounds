using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Experience {

    private Stats characterStats;
    private const int MAX_LEVEL = 100;

    // Use this for initialization
    public Experience(Stats stats) {
        characterStats = stats;
    }

    public void GainExp(int enemeyKilledLevel, out int _xpchange) {
        _xpchange = 25 + ((enemeyKilledLevel - characterStats.Level) * 10);

        UpdateXp(_xpchange);
    }


    private void UpdateXp(int xpchange) {
        int currentLevel = characterStats.Level;

        if (currentLevel != MAX_LEVEL) {

            var tempCurrentXp = characterStats.Current_Xp;

            // levelup check
            var currentLevelExp = LevelExp(currentLevel);
            for (int i = 0; i <= xpchange; i++) {
                if (i + tempCurrentXp == currentLevelExp) {
                    currentLevel++;
                    currentLevelExp = LevelExp(currentLevel);
                    LevelUp();
                    GameManager.Instance.SelectedPlayer.CheckForNewAbility(currentLevel);
                }
            }

            var newXp = tempCurrentXp + xpchange;
            characterStats.Current_Xp = (currentLevel == MAX_LEVEL) ? LevelExp(MAX_LEVEL) : newXp;
            characterStats.Level = currentLevel;

        }
    }

    // character leveled up
    public void LevelUp() {
        var newHp = characterStats.MaxHp + HpChange();
        characterStats.Hp = newHp;
        characterStats.MaxHp = newHp;
        characterStats.Strength = StatUpgrade(characterStats.Strength);
        characterStats.Speed = StatUpgrade(characterStats.Speed);
        characterStats.Defense = StatUpgrade(characterStats.Defense);
    }

    private int HpChange() {
        var temp = Random.Range(10, 20);
        return StylesEnum.CombatTypesBaseHp[characterStats.CombatType] / 7 + temp;
    }

    private float StatUpgrade(float previous_level_stat) {
        return previous_level_stat * 1.1f;
    }


    // level equation x^3
    public static int LevelExp(int level) {
        return Mathf.RoundToInt(Mathf.Pow(level, 3));
    }
}
