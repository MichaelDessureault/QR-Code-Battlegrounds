using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageCalculation : MonoBehaviour {
    /*
	Damage Equation 

	Damage = ((((2 * level) / 5 + 2) * power * strength / defense ) / 5 + 2) * modifier
    
	modifier is made up of 
	type-effectiveness * critial * ramdom

	level is the attacking character's level
	strength is the strength of attacking the character
    power is the power of the ability used
	defense is the defense of the character being attacked
     
	type-effectiveness is based on the combat triangle; if it's the effective type vsing the enemy then it's 1.5, same type 1, in-effective type it's .5
	critial is a 1 and 10 chance to be 1.5 otherwise it's 1
	random is a random float between 0.85 and 1 (inclusive)

	*/
    private static AbilityInformation abilityUsedInformation;

	public static float CalculateDamage (Character attacking, Character taking, Skill abilityused)
    {
        // Ability accuracy, if the ability missed just return 0 damage
        int abilityAccuracy = abilityused.abilityInformation.accuracy;
        if (abilityAccuracy != 100)
        {
            if (Random.Range(0, 101) > abilityAccuracy)
                return 0;
        }

        var ASC = attacking.Stats_Component;
        var TSC = taking.Stats_Component;

        var attackingLevel    = ASC.Level;
        var attackingStrength = ASC.Strength;
        var takingDefense = TSC.Defense;
        var abilityPower = abilityused.abilityInformation.power;

        var modifier = Modifer(ASC.CombatType, TSC.CombatType);

        var damage = ((((2 * attackingLevel) / 5 + 2) * abilityPower * attackingStrength / takingDefense) / 5 + 30) * modifier;
        return damage;
    }

	private static float TypeTriangle (StylesEnum.CombatTypes attacking, StylesEnum.CombatTypes taking) {
		// if they are the same type just return 1
		// else check to see what the effectiveType is, and if the effective type is the same type as taking then return 1.5
		// else it's attacking the weakest in-effective type return .5f
		if (attacking == taking) {
			return 1;
		}

		var effectiveType = StylesEnum.CombatTriangle [attacking];
		if (effectiveType == taking) {
			return 1.5f;
		} else {
			return .5f;
		}
	}

	private static float Modifer (StylesEnum.CombatTypes attacking, StylesEnum.CombatTypes taking) {
		var t = TypeTriangle (attacking, taking);
		var c = (Random.Range (1, 10) == 1) ? 1.5f : 1f;
		var r = Random.Range (0.85f, 1f); 
		return (t * c * r);
	}
}
