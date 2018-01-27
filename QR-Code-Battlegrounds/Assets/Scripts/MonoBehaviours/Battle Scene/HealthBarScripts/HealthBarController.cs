using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthBarController : MonoBehaviour {

    public HealthBarWithCoroutine playerHealthBar;
    public HealthBarWithCoroutine enemiesHealthBar;
    public BattleGUIManager battleGUIManager;
    
    public void PopulatePlayersHpContainer (Player player) {
        if (player.Stats_Component == null)
            print("players stats component is null");
        else
            playerHealthBar.SetUp(player);
    }

    public void PopulateEnemiesHpContainer (Enemy enemy) {
        if (enemy.Stats_Component == null)
            print("enemy stats component is null");
        else 
            enemiesHealthBar.SetUp(enemy);
    }
    
    public IEnumerator StartPlayersHealthBarCoroutine(int newHp, bool ishealing)
    {
        if (ishealing)
            yield return StartCoroutine(playerHealthBar.BeginHealing(newHp));
        else
            yield return StartCoroutine(playerHealthBar.BeginTakingDamage(newHp));
    }

    public IEnumerator StartEnemyHealthBarCoroutine(int newHp, bool ishealing)
    {
        if (ishealing)
            yield return StartCoroutine(enemiesHealthBar.BeginHealing(newHp));
        else
            yield return StartCoroutine(enemiesHealthBar.BeginTakingDamage(newHp));
    }
}