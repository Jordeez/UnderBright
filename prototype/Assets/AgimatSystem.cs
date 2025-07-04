/*
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AgimatSystem : MonoBehaviour
{
    [Header("References")]
    public PlayerStats playerStats;
    
    [Header("Health")]
    public float health = 100f;
    
    public class Agimat
    {
        public string name;
        public Sprite icon;
        public string effect;
        public string effectDescription;
        public float value;
    }


    [Header("Agimat Settings")]
    public List<Agimat> agimats = new List<Agimat>();

    public void AddAgimat(string name, Sprite icon, string effect, string effectDescription)
    {
        Agimat newAgimat = new Agimat
        {
            name = name,
            icon = icon,
            effect = effect,
            effectDescription = effectDescription
        };
        agimats.Add(newAgimat);
    }
    public bool hasAgimat(string name)
    {
        return agimats.Exists(agimat => agimat.name == name);
    }

    public Agimat getAgimat(string name)
    {
        return agimats.Find(agimats => agimats.name == name);
    }

    public void applyAgimatEffect(string name)
    {
        Agimat agimat = getAgimat(name);
        if (agimat != null)
        {
            // Apply the effect of the agimat
            Debug.Log($"Applying effect: {agimat.effect} - {agimat.effectDescription}");
            // Here you can implement the actual effect application logic
            if (agimat.effect == "Regeneration")
            {
                StartCoroutine(Regenerate(agimat.value));
            }
            else if (agimat.effect == "AttackBuff")
            {
                // Apply attack buff logic
                Debug.Log($"Increasing attack damage by {agimat.value}.");
                // Assuming 'attackDamage' is a variable in your player stats
                playerStats.attackDamage += agimat.value; 
            }
            else if (agimat.effect == "AttackSpeed")
            {
                // Apply attack speed logic
                Debug.Log($"Increasing attack speed by {agimat.value}.");
                playerStats.attackSpeed *= agimat.value;
            }
            else if (agimat.effect == "JumpBoost")
            {
                // Apply jump boost logic
                Debug.Log($"Increasing jump force by {agimat.value}.");
                playerStats.jumpForce *= agimat.value;
            }
            else if (agimat.effect == "Defense")
            {
                // Apply defense logic
                Debug.Log($"Decreasing damage taken by {agimat.value}.");
                playerStats.defense *= agimat.value;
            }
        }
    }

    public IEnumerator Regenerate(float regenValue)
    {
        while (true)
        {
            health += regenValue; // Assuming 'health' is a variable in your player stats
            Debug.Log($"Regenerating {regenValue} health.");
            yield return new WaitForSeconds(1f); // Adjust the regeneration interval as needed
        }
    }
    public void RemoveAgimat(string name)
    {
        Agimat agimat = getAgimat(name);
        if (agimat != null)
        {
            agimats.Remove(agimat);
            Debug.Log($"Removed Agimat: {name}");
        }
        else
        {
            Debug.LogWarning($"Agimat {name} not found.");
        }
    }
}
*/