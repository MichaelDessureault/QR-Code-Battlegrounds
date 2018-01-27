using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using System;

public class BattleSceneController : GameObjectSingleton<BattleSceneController> {
       
    // Private
    private Enemy enemy;
    private Player player;
    private Text[] abilityPPText;
    
    private int extraLevelsForBoss = 3;
    private string ppString = "PP     "; // 5 spaces
    private string choiceMessage = "Which will you choose?";

    // Properties
    public HealthBarController healthBarController { get; private set; }
    public ExperienceBarController experienceBarController { get; private set; }
    public BattleGUIManager battleGUIManager { get; private set; }
    public CombatController combatController { get; private set; }

    // Override the Awake within the gameobject singleton script
    new private void Awake() {
        if (healthBarController == null)
            healthBarController = FindObjectOfType<HealthBarController>();

        if (experienceBarController == null)
            experienceBarController = FindObjectOfType<ExperienceBarController>();

        if (battleGUIManager == null)
            battleGUIManager = FindObjectOfType<BattleGUIManager>();

        if (combatController == null)
            combatController = FindObjectOfType<CombatController>();
    }
    

    public void EnemyBegin() {
        enemy = GameManager.Instance.SelectedEnemy;

        if (enemy.IsBoss) {
            enemy.Stats_Component.RandomStatsForLevel(player.Stats_Component.Level + extraLevelsForBoss);
            battleGUIManager.HandleBoss();
        } else {
            enemy.Stats_Component.RandomLevelWithRandomStats(player.Stats_Component.Level);
        }

        // setup hpcontainers
        healthBarController.PopulateEnemiesHpContainer(enemy);
    }

    public void PlayerBegin() {
        player = GameManager.Instance.SelectedPlayer;

        battleGUIManager.PlayerSpriteSetup(player);

        healthBarController.PopulatePlayersHpContainer(player);

        experienceBarController.ExpSetup(player);

        PopulateAbilityWindow();

        // Prompt with next move choice
        StartCoroutine(battleGUIManager.TextMessageCoroutine(choiceMessage));

        // enable action window buttons
        battleGUIManager.UpdateActionWindowButtons(true);
    }
    
    public void PopulateAbilityWindow() {
        Text[] abilityButtonsText = battleGUIManager.AbilityButtonsText;
        abilityPPText = battleGUIManager.AbilityPPText;

        for (int i = 0; i < Ability.KMaxAbilities; i++) {
            var ability = player.GetAbilityAtIndex(i);
            abilityButtonsText[i].text = ability.name;

            int currentPP = ability.abilityInformation.current_pp;
            
            abilityPPText[i].text = ppString + currentPP + "/" + ability.abilityInformation.max_pp; // 5 spaces between PP and the current pp text

            Button parentsBtn = abilityButtonsText[i].transform.parent.gameObject.GetComponent<Button>();

            if (parentsBtn != null) {
                parentsBtn.interactable = (ability.Equals(Ability.Instance.noSkill) || currentPP <= 0) ? false : true;
            }
        }
    }

    public void UpdatePPForAbility(int index, Skill s) {
        if (index != -1) {
            var currentPP = s.abilityInformation.current_pp;
            abilityPPText[index].text = ppString + currentPP + "/" + s.abilityInformation.max_pp;
            if (currentPP <= 0) {
                Button btn = abilityPPText[index].transform.parent.gameObject.GetComponentInParent<Button>();
                if (btn != null) {
                    btn.interactable = false;
                }
            }
        }
    }

    public IEnumerator TurnHandler(int index) {
        Skill playerSelectedAbility = player.MoveSelected(index);
        Skill enemeySelectedAbility = enemy.EnemiesTurn();

        // begin attacks
        yield return StartCoroutine(combatController.Setup(player, playerSelectedAbility, enemy, enemeySelectedAbility));

        player.DecreasePPOnAbilitySelected();
        UpdatePPForAbility(index, playerSelectedAbility);
        // validate that the enemy has not died
        if (enemy.IsDead) {
            enemy.Dead();
            yield break;
        }

        // validate that the player has no died
        if (player.IsDead) {
            player.Dead();
            yield break;
        }

        // neather have died prompt with next move
        StartCoroutine(battleGUIManager.TextMessageCoroutine(choiceMessage));
        // enable action window buttons
        battleGUIManager.UpdateActionWindowButtons(true);
    }

    public void Run() {
        // call destroy enemy instead of destroy because it checks to see if the root branch has other enemies
        // if otheres ar enot found it removes the full branch with the gameobject as well
        enemy.Destroy();
    }
}