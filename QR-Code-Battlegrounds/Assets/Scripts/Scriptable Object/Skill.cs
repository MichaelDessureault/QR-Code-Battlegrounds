using System;
using System.Collections;
using UnityEngine;

[CreateAssetMenu]
public class Skill : ScriptableObject
{
    public bool GenerateUnqiueId = false;
    public new string name = String.Empty;
    public AbilityInformation abilityInformation;
    
    void OnValidate ()
    {
        if (GenerateUnqiueId)
        {
            GenerateUnqiueId = false;
            abilityInformation.uniqueId = GenerateId();
        }

        abilityInformation.Validate();
    }
    
    string GenerateId()
    {
        return Guid.NewGuid().ToString("N");
    }
}
