using System;
using UnityEngine;

[Serializable]
public class AbilityInformation {
    [SerializeField] public string uniqueId;
    [SerializeField] public StylesEnum.CombatTypes stylesEnum;
    public int power;
    public int accuracy;
    public int max_pp;
    public int current_pp;
    public int level_required;

    // methods 
    public string PPLeftOutOfMaxString() {
        return (current_pp + "/" + max_pp);
    }

    public string PPLeftOutOfMaxStringWithPP() {
        return ("PP  " + current_pp + "/" + max_pp);
    }

    public void Validate() {
        //power = Mathf.Clamp(power, 1, 200);
        accuracy = Mathf.Clamp(accuracy, 1, 100);
        max_pp = Mathf.Clamp(max_pp, 1, 50);
        current_pp = Mathf.Clamp(current_pp, 0, max_pp);
        level_required = Mathf.Clamp(level_required, 0, 100);
    }


    // This is a forced json string due to not wanting the other variables
    // If the other variables are set to [NonSerialized] they will not display in the inspector
    public string ToJsonString() {
        return "{\"uniqueId\":\"" + uniqueId + "\",\"stylesEnum\":" + (int)stylesEnum + "}";
        //return JsonUtility.ToJson(this);
    }
}
