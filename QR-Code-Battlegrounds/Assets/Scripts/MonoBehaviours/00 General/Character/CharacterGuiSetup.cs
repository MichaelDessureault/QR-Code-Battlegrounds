using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

/// <summary>
/// Do note that CharaacterGuiSetup script has an Editor created for it, any variables added with this script have to be added in the Editor script as well
/// </summary>
public class CharacterGuiSetup : MonoBehaviour {
    #region assassin variables
    private Player assassin;
    public HealthBarWithCoroutine assassinHealthBar;
    public Text assassinName;
    public Text assassinLevel;
    public Text[] assassinAbilities = new Text[Ability.KMaxAbilities];
    public Text[] assassinAbilitiesPP = new Text[Ability.KMaxAbilities];
    #endregion


    #region guardian variables
    private Player guardian;
    public HealthBarWithCoroutine guardianHealthBar;
    public Text guardianName;
    public Text guardianLevel;
    public Text[] guardianAbilities = new Text[Ability.KMaxAbilities];
    public Text[] guardianAbilitiesPP = new Text[Ability.KMaxAbilities];
    #endregion


    #region wizard variables
    private Player wizard;
    public HealthBarWithCoroutine wizardHealthBar;
    public Text wizardName;
    public Text wizardLevel;
    public Text[] wizardAbilities = new Text[Ability.KMaxAbilities];
    public Text[] wizardAbilitiesPP = new Text[Ability.KMaxAbilities];
    #endregion

    private GameManager gmInstance;

    // Use this for initialization
    void Start () {

        gmInstance = GameManager.Instance;

        assassin = gmInstance.Assassin;
        guardian = gmInstance.Guardian;
        wizard = gmInstance.Wizard;

        AssassinSetup();
        GuardianSetup();
        WizardSetup();
	}
    
    public void UpdateAbilities (Player player)
    {
        Text[] abilities;
        Text[] abilitiesPP;

        if (player.Equals(assassin)) {
            abilities = assassinAbilities;
            abilitiesPP = assassinAbilitiesPP;
        } else if (player.Equals(guardian)) {
            abilities = guardianAbilities;
            abilitiesPP = guardianAbilitiesPP;
        }
        else if (player.Equals(wizard)) {
            abilities = wizardAbilities;
            abilitiesPP = wizardAbilitiesPP;
        } else {
            return;
        }

        if (abilities == null)
            return;

        List<Skill> skillSet = player.AbilitySet;

        for (int i = 0; i < skillSet.Count; i++) {
            if (abilities[i] != null) {
                abilities[i].text = skillSet[i].name;

                if (abilitiesPP[i] != null)
                    abilitiesPP[i].text = skillSet[i].abilityInformation.PPLeftOutOfMaxString();
            } else {
                break;
            }
        }
    }

    void AssassinSetup()
    {
        if (assassinHealthBar != null)
            assassinHealthBar.SetUp(assassin);

        if (assassinName != null)
            assassinName.text = assassin.name;

        if (assassinLevel != null)
            assassinLevel.text = "Level " + assassin.Stats_Component.Level;

        if (assassinAbilities != null)
            UpdateAbilities(assassin);
    }

    void GuardianSetup()
    {
        if (guardianHealthBar != null)
            guardianHealthBar.SetUp(guardian);

        if (guardianName != null)
            guardianName.text = guardian.name;

        if (guardianLevel != null)
            guardianLevel.text = "Level " + guardian.Stats_Component.Level;

        if (guardianAbilities != null)
            UpdateAbilities(guardian);
    }

    void WizardSetup()
    {
        if (wizardHealthBar != null)
            wizardHealthBar.SetUp(wizard);

        if (wizardName != null)
            wizardName.text = wizard.name;

        if (wizardLevel != null)
            wizardLevel.text = "Level " + wizard.Stats_Component.Level;

        if (wizardAbilities != null)
            UpdateAbilities(wizard);
    }
}
