using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

public class PlayerAbilityManager : MonoBehaviour
{
    private HashSet<string> abilities = new HashSet<string>();

    public void UnlockAbility(string abilityName)
    {
        if (!abilities.Contains(abilityName))
        {
            abilities.Add(abilityName);
            Debug.Log($"Unlocked Ability: {abilityName}");
        }
    }

    public bool HasAbility(string abilityName)
    {
        return abilities.Contains(abilityName);
    }
}