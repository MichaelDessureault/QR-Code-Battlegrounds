using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class Stats {

	[SerializeField] private int   level_private;
    [SerializeField] private int currentxp_private;
    [SerializeField] private int hp_private;
    [SerializeField] private int max_hp_private;
    [SerializeField] private float strength_private;
    [SerializeField] private float speed_private;
    [SerializeField] private float defense_private;
    [SerializeField] private StylesEnum.CombatTypes combattype_private;
    [SerializeField] private bool isDead;

	public int Level {
		get { return level_private;  }
		set { level_private = value; }
	}
    
	public int Current_Xp {
		get { return currentxp_private;  }
		set { currentxp_private = value; }
	}

	public int Hp {
		get { return hp_private;  }
		set { hp_private = value; }
	}

	public int MaxHp {
		get { return max_hp_private;  }
		set { max_hp_private = value; }
	}

	public float Strength {
		get { return strength_private;  }
		set { strength_private = value; }
	}

	public float Speed {
		get { return speed_private;  }
		set { speed_private = value; }
	}

	public float Defense {
		get { return defense_private;  }
		set { defense_private = value; }
	}
    
    public bool IsFullHealth {
        get { return (Hp == MaxHp); }
    }

    public StylesEnum.CombatTypes CombatType {
		get { return combattype_private; }
	}

    public bool IsDead {
        get { return isDead;  }
        set { isDead = value; }
    }

    public Stats () { }
    
    public Stats (StylesEnum.CombatTypes type)
    {
        NewStatsSetup(type);
    }

    public void NewStatsSetup(StylesEnum.CombatTypes type)
    {
        combattype_private = type;
        var hp = StylesEnum.CombatTypesBaseHp[type];
        float strength, speed, defense;
        
        switch (type)
        {
            case StylesEnum.CombatTypes.assassin:
                strength = StylesEnum.AssassinBaseStats["strength"];
                speed    = StylesEnum.AssassinBaseStats["speed"];
                defense  = StylesEnum.AssassinBaseStats["defense"];
                break;
            case StylesEnum.CombatTypes.guardian:
                strength = StylesEnum.GuardianBaseStats["strength"];
                speed    = StylesEnum.GuardianBaseStats["speed"];
                defense  = StylesEnum.GuardianBaseStats["defense"];
                break;
            default:
                strength = StylesEnum.WizardBaseStats["strength"];
                speed    = StylesEnum.WizardBaseStats["speed"];
                defense  = StylesEnum.WizardBaseStats["defense"];
                break;
        }

        hp_private = hp;
        max_hp_private = hp;
        currentxp_private = 0;
        level_private = 1;
        strength_private = strength;
        speed_private = speed;
        defense_private = defense;
        isDead = false;
    }

    public void RandomLevelWithRandomStats (int middleLevel)
    {
        // the max is exclusive so the max value is level + 1, clamp keeps it within the range of 1 to 100
        int newLevel = Mathf.Clamp(UnityEngine.Random.Range(middleLevel - 2, middleLevel + 2), 1, 100);
        RandomStatsForLevel(newLevel);
    }

    public void RandomStatsForLevel (int level) {
        Experience tempExperience = new Experience(this);

        for (int i = 1; i < level; i++) {
            tempExperience.LevelUp();
        }

        level_private = level;
        currentxp_private = Experience.LevelExp(level - 1);
    }

    public string ToJsonString()
    {
        return JsonUtility.ToJson(this);
    }

    public override string ToString()
    {
        return "Level: " + level_private + " CurrentXp: " + currentxp_private + " Hp: " + hp_private + " MaxHp: " + max_hp_private + " Strength: " + strength_private + " Speed: " + speed_private + " Defense: " + defense_private + " type: " + combattype_private + " dead: " + isDead;
    }
}
