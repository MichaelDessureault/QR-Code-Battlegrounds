using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using System.Collections;

// Ability is a singleton, but doesn't extend the singleton class because it's not a MonoBehaviour
public class Ability
{
    public const int KMaxAbilities = 3;

    #region Singleton
    private static Ability _instance;

    public static Ability Instance
    {
        get { 

            if (_instance == null)
            {
                _instance = new Ability();
            }

            return _instance;
        }
    }
    #endregion

    public Skill healSkill;
    public Skill noSkill;
    public Skill oneHitSkill;

    public List<Skill> AssassinAbilitySet = new List<Skill>();
    public List<Skill> GuaridanAbilitySet = new List<Skill>();
    public List<Skill> WizardAbilitySet = new List<Skill>();
    public List<Skill> AllAbilitySet = new List<Skill>();
    
    public Ability()
    {
        _instance = this;
        CreateAbilityInformaiton();

        // heal and no skill setup
        healSkill = ScriptableObject.CreateInstance(typeof(Skill)) as Skill;
        healSkill.name = "Heal";

        noSkill = ScriptableObject.CreateInstance(typeof(Skill)) as Skill;
        noSkill.name = "-";
    }
    
    public List<Skill> GetAbilitySetForCombatType (StylesEnum.CombatTypes combatType, int characterLevel)
    {
        List<Skill> abilitySet = new List<Skill>();

        switch (combatType)
        {
            case StylesEnum.CombatTypes.assassin:
                PopulateAbilitySetForLevel(ref abilitySet, AssassinAbilitySet, characterLevel);
                break;
            case StylesEnum.CombatTypes.guardian:
                PopulateAbilitySetForLevel(ref abilitySet, GuaridanAbilitySet, characterLevel);
                break;
            case StylesEnum.CombatTypes.wizard:
                PopulateAbilitySetForLevel(ref abilitySet, WizardAbilitySet, characterLevel);
                break;
            default:
                return null;
        }

        if (abilitySet.Count == 0)
        {
            Debug.LogError("No skills found for " + combatType + " within level range 0 to " + characterLevel);
        }

        return abilitySet;
    }

    public Skill GetSkillsByUniqueId(StylesEnum.CombatTypes combatTypes, string id)
    {
        List<Skill> set = new List<Skill>();

        switch (combatTypes)
        {
            case StylesEnum.CombatTypes.assassin: set = AssassinAbilitySet; break;
            case StylesEnum.CombatTypes.guardian: set = GuaridanAbilitySet; break;
            case StylesEnum.CombatTypes.wizard: set = WizardAbilitySet; break;
            default: set = AllAbilitySet; break;
        }

        foreach (Skill s in set)
        {
            if (id.Equals(s.abilityInformation.uniqueId))
                return s;
        }

        return null;
    }


    private void PopulateAbilitySetForLevel (ref List<Skill> abilitySet, List<Skill> MainAbilitySet, int characterLevel)
    {
        foreach (Skill skill in MainAbilitySet)
        {
            if (skill.abilityInformation.level_required <= characterLevel)
            {
                if (abilitySet.Count == KMaxAbilities)
                {
                    abilitySet.RemoveAt(0); // removing the first one will be the lowest level ability... 
                }
                abilitySet.Add(skill);
            }
            else
            {
                break;
            }
        }
    }
    
    public Skill CheckForAbilityAfterLevelUp (StylesEnum.CombatTypes combatType, int newLevel) {
        Skill s = ScriptableObject.CreateInstance(typeof(Skill)) as Skill;
        Debug.Log("looking for level: " + newLevel);
        try {
            switch (combatType) {
                case StylesEnum.CombatTypes.assassin:
                    s = AssassinAbilitySet.Where(skill => skill.abilityInformation.level_required == newLevel).ToList().FirstOrDefault();
                    break;
                case StylesEnum.CombatTypes.guardian:
                    s = GuaridanAbilitySet.Where(skill => skill.abilityInformation.level_required == newLevel).ToList().FirstOrDefault();
                    break;
                case StylesEnum.CombatTypes.wizard:
                    s = WizardAbilitySet.Where(skill => skill.abilityInformation.level_required == newLevel).ToList().FirstOrDefault();
                    break;
                default: break;
            }
        } catch (System.Exception) {
            Debug.Log("Exception found with CheckForAbilityAfterLevelUp, Exception ignored");
        }
        return s;
    }

    // Gets all Skill scriptable objects that are found within Resources/Skills folder
    private void CreateAbilityInformaiton()
    {
        List<Object> skills = new List<Object>();
        skills.AddRange(Resources.LoadAll("Skills", typeof(Skill)));

        foreach (Object obj in skills)
        {
            Skill skill = (Skill)obj;
            switch(skill.abilityInformation.stylesEnum)
            {
                case StylesEnum.CombatTypes.assassin:
                    AssassinAbilitySet.Add(skill);
                    break;
                case StylesEnum.CombatTypes.guardian:
                    GuaridanAbilitySet.Add(skill);
                    break;
                case StylesEnum.CombatTypes.wizard:
                    WizardAbilitySet.Add(skill);
                    break;
                default:
                    {
                        AssassinAbilitySet.Add(skill);
                        GuaridanAbilitySet.Add(skill);
                        WizardAbilitySet.Add(skill);
                        AllAbilitySet.Add(skill);
                        break;
                    }
            } 
        }
        AssassinAbilitySet.Sort((s1, s2) => (s1).abilityInformation.level_required.CompareTo((s2).abilityInformation.level_required));
        GuaridanAbilitySet.Sort((s1, s2) => (s1).abilityInformation.level_required.CompareTo((s2).abilityInformation.level_required));
        WizardAbilitySet.Sort((s1, s2) => (s1).abilityInformation.level_required.CompareTo((s2).abilityInformation.level_required));
        AllAbilitySet.Sort((s1, s2) => (s1).abilityInformation.level_required.CompareTo((s2).abilityInformation.level_required));
    }

    private void PrintList (List<Skill> list)
    {
        foreach (Skill skill in list)
        {
            Debug.Log(skill.name + " requires level " + skill.abilityInformation.level_required + " to learn");
        }
    }
}