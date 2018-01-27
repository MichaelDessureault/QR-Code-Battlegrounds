using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StylesEnum {
	
	public enum CombatTypes {
		assassin,
		guardian,
		wizard,
        all
	}; 

    public static Dictionary<CombatTypes, int> CombatTypesBaseHp = new Dictionary<CombatTypes, int> () {
		{ CombatTypes.assassin, 100 }, { CombatTypes.guardian, 150 }, { CombatTypes.wizard, 100 }
	};
    
    public static Dictionary<CombatTypes, CombatTypes> CombatTriangle = new Dictionary<CombatTypes, CombatTypes> () {
		{ CombatTypes.assassin, CombatTypes.wizard }, { CombatTypes.guardian, CombatTypes.assassin }, { CombatTypes.wizard, CombatTypes.guardian }
	};

    public static Dictionary<string, int> AssassinBaseStats = new Dictionary<string, int>()
    {
        { "strength" , 75 }, { "speed", 125 }, { "defense", 100 }
    };

    public static Dictionary<string, int> GuardianBaseStats = new Dictionary<string, int>()
    {
        { "strength" , 75 }, { "speed", 75 }, { "defense", 125 }
    };

    public static Dictionary<string, int> WizardBaseStats = new Dictionary<string, int>()
    {
        { "strength" , 125 }, { "speed", 100 }, { "defense", 75 }
    };

    public static CombatTypes GetRandomType()
    {
        // will not resture all type...
        switch (Random.Range(0, 3))
        {
            case 0:  return CombatTypes.assassin;
            case 1:  return CombatTypes.guardian;
            default: return CombatTypes.wizard;
        }
    }
}
