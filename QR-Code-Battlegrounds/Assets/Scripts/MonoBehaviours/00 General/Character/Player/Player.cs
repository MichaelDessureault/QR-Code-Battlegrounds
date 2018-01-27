using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Character {
    private Experience _experience_component;

    public Experience Experience_Component {
        get { return _experience_component; }
        set { _experience_component = value; }
    }

    public Sprite FrontSprite { get; set; }

    public Sprite BackSprite { get; set; }

    public Skill MoveSelected(int index) {
        selectedAbility = (index == -1) ? Ability.Instance.healSkill : abilitySet[index];
        return selectedAbility;
    }

    public IEnumerator GainExp(int enemylevel) {
        int xpchange;
        _experience_component.GainExp(enemylevel, out xpchange);

        // start coroutine for exp bar "animation"
        yield return StartCoroutine(battleSceneController.experienceBarController.StartGainingExp(xpchange));
    }

    public override IEnumerator TakeDamage(int damage) {
        int newHp = stats_component.Hp - damage;

        // Check if the new hp is below zero or zero if so the enemy has died
        // Reset the newHp value to 0 to make sure it doesn't go below
        // Update the IsDead variable
        if (newHp <= 0) {
            IsDead = true;
            newHp = 0;
        }

        stats_component.Hp = newHp;

        Enemy enemy = GameManager.Instance.SelectedEnemy;
        yield return StartCoroutine(battleGUIManager.TextMessageCoroutine(enemy.name + " is attacking " + name + " with " + enemy.SelectedAbility.name));
        StartCoroutine(battleGUIManager.FadeForHit(isPlayer: true));
        yield return StartCoroutine(healthBarController.StartPlayersHealthBarCoroutine(newHp: newHp, ishealing: false));
    }

    public bool IsHealable() {
        return (stats_component.Hp != stats_component.MaxHp);
    }

    public bool IsEtherable() {
        bool etherable = false;

        foreach (Skill s in abilitySet) {
            if (s.abilityInformation.current_pp != s.abilityInformation.max_pp) {
                etherable = true;
                break;
            }
        }

        return etherable;
    }

    public void EtherUsed() {
        foreach (Skill s in abilitySet) {
            s.abilityInformation.current_pp = s.abilityInformation.max_pp;
        }
    }
    
    public override IEnumerator HealWithCoroutine() {
        stats_component.Hp = stats_component.MaxHp;
        yield return StartCoroutine(battleGUIManager.TextMessageCoroutine(name + " is healing!"));
        yield return StartCoroutine(healthBarController.StartPlayersHealthBarCoroutine(newHp: stats_component.MaxHp, ishealing: true));
    }

    protected override IEnumerator DeadCoroutine() {
        // Display that the player has died
        yield return StartCoroutine(battleGUIManager.TextMessageCoroutine(name + " has died."));
        StartCoroutine(battleGUIManager.FadeForDeathAndMapLoadScene(isPlayer: true));
    }

    public void DecreasePPOnAbilitySelected() {
        if (selectedAbility.Equals(Ability.Instance.healSkill)) {
            StoreData.Instance.PotionUsed();
        } else {
            if (selectedAbility.abilityInformation.current_pp == 0)
                return;
            selectedAbility.abilityInformation.current_pp--;
        }
    }

    private Dictionary<int, Skill> newAbilitiesToLearn = new Dictionary<int, Skill>();

    public void CheckForNewAbility(int level) {
        Skill s = Ability.Instance.CheckForAbilityAfterLevelUp(CombatType, level);
        if (s != null) {
            newAbilitiesToLearn.Add(s.abilityInformation.level_required, s);
        }
    }

    private int learningAttempt = 0;
    public IEnumerator PromptChatWithNewAbility(int level) {
        Skill s = null;

        try {
            s = newAbilitiesToLearn[level];
        } catch (System.Exception) {
            yield break;
        }
        
        yield return StartCoroutine(WantToLearn(s));
        
        battleGUIManager.EnableYesNoWindow(false);
    }

    private IEnumerator WantToLearn(Skill s) {
        if (learningAttempt >= 2) {
            GiveUpOnAbility(s);
            yield break;
        }

        learningAttempt++;

        yield return StartCoroutine(battleGUIManager.TextMessageWaitForResponse
                                    (message: "Do you want to learn " + s.name + "?",
                                    iEnumeratorMethod: battleGUIManager.WaitForYesOrNoInput()));

        int yesno = battleGUIManager.yesNoValue;
        battleGUIManager.yesNoValue = -1;
        switch (yesno) {
            // no
            case 0:
                yield return (AreYouSureYouWantToForget(s));
                break;
            // yes
            case 1:
                yield return StartCoroutine(WhichAibilityToForget(s));
                break;
            // no answer
            default:
                yield return (StartCoroutine(GiveUpOnAbility(s)));
                break;
        }
    }

    private IEnumerator WhichAibilityToForget(Skill s) {
        battleGUIManager.removeAbility = true;
        yield return StartCoroutine(battleGUIManager.TextMessageWaitForResponse
                                   (message: "Select the ability you would like to forget", 
                                    iEnumeratorMethod: battleGUIManager.WaitForAbilitySelected()));

        int indexForAbilityToRemove = battleGUIManager.removeAbilityIndex;
        battleGUIManager.removeAbilityIndex = -1;
        battleGUIManager.removeAbility = false;

        // If the index is the KMabAbility view then the give up (back button) was selected
        if (indexForAbilityToRemove != -1 && indexForAbilityToRemove != Ability.KMaxAbilities) {
            Skill old = abilitySet[indexForAbilityToRemove];
            yield return StartCoroutine(battleGUIManager.TextMessageCoroutine(old.name + " is being replaced with " + s.name));
            abilitySet[indexForAbilityToRemove] = s;
            battleSceneController.PopulateAbilityWindow();
            yield return new WaitForSeconds(.3f);
        }

        // reset index for ability to remove for the next time around
        indexForAbilityToRemove = -1;
    }

    private IEnumerator AreYouSureYouWantToForget(Skill s) {
        yield return StartCoroutine(battleGUIManager.TextMessageWaitForResponse
                                    (message: "Are you sure you do not want to learn " + s.name + "?",
                                     iEnumeratorMethod: battleGUIManager.WaitForYesOrNoInput()));

        int yesno = battleGUIManager.yesNoValue;
        battleGUIManager.yesNoValue = -1;
        switch (yesno) {
            // no
            case 0:
                // restart 
                yield return StartCoroutine(WantToLearn(s));

                // the old want to learn will stop now
                break;
            // yes remove or no answer
            default:
                yield return (StartCoroutine(GiveUpOnAbility(s)));
                break;
        }
    }

    private IEnumerator GiveUpOnAbility (Skill s) {
        yield return StartCoroutine(battleGUIManager.TextMessageCoroutine("You have given up on learning " + s.name));
    }

    public string AbilitySetToJson() {
        string jsonString = string.Empty;

        foreach (Skill s in abilitySet) {
            jsonString += s.abilityInformation.ToJsonString();
        }

        return jsonString;
    }

}