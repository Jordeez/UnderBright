/*
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemBuffs : MonoBehaviour
{
    public enum BuffType
    {
        Regeneration,
        AttackBuff,
        AttackSpeed,
        JumpBoost,
        Defense
    }

    [System.Serializable]
    public class ActiveBuff
    {
        public BuffType type;
        public float duration;
        public float endTime;
        public float value;
        public GameObject visualEffect;
    }

    [Header("Buff Settings")]
    [SerializeField] private float regenerationRate = 1f;
    [SerializeField] private float attackBuffMultiplier = 1.5f;
    [SerializeField] private float attackSpeedMultiplier = 1.3f;
    [SerializeField] private float jumpBoostMultiplier = 1.4f;
    [SerializeField] private float defenseReduction = 0.7f;

    [Header("Visual Effects")]
    [SerializeField] private GameObject regenerationEffectPrefab;
    [SerializeField] private GameObject attackBuffEffectPrefab;
    [SerializeField] private GameObject attackSpeedEffectPrefab;
    [SerializeField] private GameObject jumpBoostEffectPrefab;
    [SerializeField] private GameObject defenseEffectPrefab;

    private PlayerStats playerStats;
    private List<ActiveBuff> activeBuffs = new List<ActiveBuff>();
    private float originalAttack;
    private float originalAttackSpeed;
    private float originalJumpForce;
    private float originalDefense;

    void Start()
    {
        playerStats = GetComponent<PlayerStats>();
        if (playerStats == null)
        {
            Debug.LogError("PlayerStats component not found!");
            return;
        }

        // Cache original values
        originalAttack = playerStats.attackDamage;
        originalAttackSpeed = playerStats.attackSpeed;
        originalJumpForce = playerStats.jumpForce;
        originalDefense = playerStats.defense;
    }

    void Update()
    {
        UpdateBuffs();
        HandleRegeneration();
    }

    public void ApplyBuff(BuffType type, float duration, float value = 0f)
    {
        // Check if this buff type is already active
        ActiveBuff existingBuff = activeBuffs.Find(b => b.type == type);
        
        if (existingBuff != null)
        {
            // Refresh duration if buff already exists
            existingBuff.endTime = Time.time + duration;
            return;
        }

        // Create new buff
        ActiveBuff newBuff = new ActiveBuff
        {
            type = type,
            duration = duration,
            endTime = Time.time + duration,
            value = value
        };

        // Apply buff effects
        switch (type)
        {
            case BuffType.Regeneration:
                newBuff.visualEffect = Instantiate(regenerationEffectPrefab, transform);
                break;
                
            case BuffType.AttackBuff:
                playerStats.attackDamage *= attackBuffMultiplier;
                newBuff.visualEffect = Instantiate(attackBuffEffectPrefab, transform);
                break;
                
            case BuffType.AttackSpeed:
                playerStats.attackSpeed *= attackSpeedMultiplier;
                newBuff.visualEffect = Instantiate(attackSpeedEffectPrefab, transform);
                break;
                
            case BuffType.JumpBoost:
                playerStats.jumpForce *= jumpBoostMultiplier;
                newBuff.visualEffect = Instantiate(jumpBoostEffectPrefab, transform);
                break;
                
            case BuffType.Defense:
                playerStats.defense *= defenseReduction;
                newBuff.visualEffect = Instantiate(defenseEffectPrefab, transform);
                break;
        }

        activeBuffs.Add(newBuff);
    }

    private void UpdateBuffs()
    {
        for (int i = activeBuffs.Count - 1; i >= 0; i--)
        {
            if (Time.time >= activeBuffs[i].endTime)
            {
                RemoveBuff(activeBuffs[i]);
                activeBuffs.RemoveAt(i);
            }
        }
    }

    private void RemoveBuff(ActiveBuff buff)
    {
        // Remove visual effect
        if (buff.visualEffect != null)
        {
            Destroy(buff.visualEffect);
        }

        // Revert stat changes
        switch (buff.type)
        {
            case BuffType.AttackBuff:
                playerStats.attackDamage = originalAttack;
                break;
                
            case BuffType.AttackSpeed:
                playerStats.attackSpeed = originalAttackSpeed;
                break;
                
            case BuffType.JumpBoost:
                playerStats.jumpForce = originalJumpForce;
                break;
                
            case BuffType.Defense:
                playerStats.defense = originalDefense;
                break;
        }
    }

    private void HandleRegeneration()
    {
        bool isRegenerating = activeBuffs.Exists(b => b.type == BuffType.Regeneration);
        
        if (isRegenerating && playerStats.currentHealth < playerStats.maxHealth)
        {
            playerStats.currentHealth += regenerationRate * Time.deltaTime;
            playerStats.currentHealth = Mathf.Min(playerStats.currentHealth, playerStats.maxHealth);
        }
    }

    public bool HasBuff(BuffType type)
    {
        return activeBuffs.Exists(b => b.type == type);
    }

    public float GetRemainingDuration(BuffType type)
    {
        ActiveBuff buff = activeBuffs.Find(b => b.type == type);
        return buff != null ? buff.endTime - Time.time : 0f;
    }
}
*/