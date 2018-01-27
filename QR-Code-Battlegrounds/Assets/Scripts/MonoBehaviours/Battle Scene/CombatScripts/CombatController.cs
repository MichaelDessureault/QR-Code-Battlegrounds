using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatController : MonoBehaviour
{
    //public float waitTime = .2f;

    private WaitForSeconds wait = new WaitForSeconds(.2f);

    private Player player;
    private Enemy enemy;

    private Skill playerAbility;
    private Skill enemyAbility;

    private Ability abilityInstance;

    public IEnumerator Setup (Player player, Skill playerAbility, Enemy enemy, Skill enemyAbility)
    {
        this.player = player;
        this.playerAbility = playerAbility;

        this.enemy = enemy;
        this.enemyAbility = enemyAbility;

        abilityInstance = Ability.Instance;
        yield return StartCoroutine (AttackingConditions());
    }

    // Will sort out who attacks first player or endemy based on speed
    public IEnumerator AttackingConditions()
    {
        bool isPlayerHealing = (playerAbility.Equals(abilityInstance.healSkill));
        bool isEnemyHealing  = (enemyAbility.Equals(abilityInstance.healSkill));

        bool playerdone = false;
        bool enemydone  = false;
        
        if (isPlayerHealing)
        {
            yield return StartCoroutine(player.HealWithCoroutine());
            yield return wait;
            playerdone = true;
        }

        if (isEnemyHealing)
        {
            yield return StartCoroutine(enemy.HealWithCoroutine());
            yield return wait;
            enemydone = true;
        }
        
        if (playerdone && enemydone)
            yield break;

        if (IsPlayerFaster())
        {
            if (!playerdone)
            {
                yield return StartCoroutine(PlayerAttacks());
                // validate that enemy has not died
                if (enemy.IsDead)
                    yield break;

                yield return wait;
            }

            if (!enemydone)
            {
                yield return StartCoroutine(EnemyAttacks());
            }
        }
        else
        {
            if (!enemydone)
            {
                yield return StartCoroutine(EnemyAttacks());

                // validate that player has not died
                if (player.IsDead)
                    yield break;

                yield return wait;
            }

            if (!playerdone)
            {
                yield return StartCoroutine(PlayerAttacks());
            }
        }
    }

    private bool IsPlayerFaster()
    {
        var cSpeed = player.Stats_Component.Speed;
        var eSpeed = enemy.Stats_Component.Speed;
        if (cSpeed == eSpeed)
            return (Random.Range(0, 2) == 0);
        return (cSpeed > eSpeed);
    }

    private IEnumerator PlayerAttacks ()
    {
        int playerDamageToDeal = PlayerDamage();

        if (playerDamageToDeal == 0)
        {
            // player missed
            yield return StartCoroutine(BattleSceneController.Instance.battleGUIManager.TextMessageCoroutine(player.name + " has missed..."));
        }
        else
        {
            yield return StartCoroutine(enemy.TakeDamage(playerDamageToDeal));
            GameManager.Instance.SaveDataOnDatabase();
        }
    }

    private IEnumerator EnemyAttacks()
    {
        int enemyDamageToDeal = EnemyDamage();
        if (enemyDamageToDeal == 0)
        {
            // player missed
            yield return StartCoroutine(BattleSceneController.Instance.battleGUIManager.TextMessageCoroutine(enemy.name + " has missed..."));
        }
        else
        {
            yield return StartCoroutine(player.TakeDamage(enemyDamageToDeal));
            GameManager.Instance.SaveDataOnDatabase();
        }
    }
    
    // returns the amount of damage the player will deal
    private int PlayerDamage()
    {
        return Mathf.RoundToInt(DamageCalculation.CalculateDamage(attacking: player, taking: enemy, abilityused: playerAbility));
    }

    // returns the amount of damage the enemy will deal
    private int EnemyDamage()
    {
        return Mathf.RoundToInt(DamageCalculation.CalculateDamage(attacking: enemy, taking: player, abilityused: enemyAbility));
    }
} 
