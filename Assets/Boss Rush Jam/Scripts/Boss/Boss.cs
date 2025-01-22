using Battle.BattleMana;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Boss : MonoBehaviour
{
    [SerializeField] private BossData bossData;
    private int currentHealth;
    private BossStage currentStage;
    private int attackDamage;
    private BossAttacks selectedAttack;

    public static event Action<Boss> OnBossSpawned;

    // Start is called before the first frame update
    void Start()
    {
        currentHealth = bossData.maxHealth;
        currentStage = bossData.GetCurrentStage(currentHealth);
        OnBossSpawned?.Invoke(this);
        //Debug.Log($"{bossData.bossName} initialized with {currentHealth} health.");
    }

    //Called in the BattleHandler to decide on an attack
    public void PerformAction(Mana bossMana)
    {
        selectedAttack = SelectedAttack(bossMana);
        if(selectedAttack != null )
        {
            ExecuteAttack(selectedAttack);
            //Debug.Log($"Attack cost for {selectedAttack.attackName} of attack type {selectedAttack.attackType}: {selectedAttack.attackCost}");
        }
        else
        {
            Debug.Log("Boss has no valid attacks and skips its turn.");
            bossMana.ManaRegeneration(false, true, false, 0, false);
        }
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;

        BossStage newStage = bossData.GetCurrentStage(currentHealth);
        if (newStage != currentStage)
        {
            Debug.Log("Calling transistion to new stage");
            TransitionToStage(newStage);
        }

        if (currentHealth <= 0)
        {
            Debug.Log($"{bossData.bossName} defeated!");
        }
    }

    public int ReturnBossHealth()
    {
        //Debug.Log($"{bossData.name} current health: {currentHealth}");
        return currentHealth;
    }

    public int ReturnAttackDamage()
    {
        return attackDamage;
    }

    public float GetAttackCost()
    {
        return selectedAttack.attackCost;
    }

    public AttackType GetBossAttackType()
    {
        return selectedAttack.attackType;
    }

    private void TransitionToStage(BossStage newStage)
    {
        currentStage = newStage;
        Debug.Log($"{bossData.bossName} transitions to stage: {currentStage.stageName}");
        // Trigger unique effects or animations for the new stage
    }

    //Selects Attack, may need to pass the mana avaliable so that it can make the attack
    private BossAttacks SelectedAttack(Mana bossMana)
    {
        //BossStage.StageAction[] stageSpecificAttacks = currentStage.stageActions; //Get the stage-specific actions
        //BossAttacks[] allAttacks = bossData.defaultAttacks; //Get all possible attacks
        //List<BossAttacks> validAttacks = new List<BossAttacks>();

        //foreach (BossAttacks attack in allAttacks)
        //{
        //    if (bossMana.CanPerformAttack(attack.attackCost))
        //    {
        //        validAttacks.Add(attack);
        //    }
        //}

        //Use the stage-specific attacks
        BossAttacks[] stageAttacks = currentStage.stageAttacks;
        List<BossAttacks> validAttacks = new List<BossAttacks>();

        foreach (BossAttacks attack in stageAttacks)
        {
            if (bossMana.CanPerformAttack(attack.attackCost))
            {
                validAttacks.Add(attack);
            }
        }

        if(validAttacks.Count > 0)
        {
            return validAttacks[Random.Range(0,validAttacks.Count)];
        }
        if (validAttacks.Count == 0 && bossData.defaultAttacks.Length > 0)
        {
            return bossData.defaultAttacks[Random.Range(0, bossData.defaultAttacks.Length)];
        }
        else
        {
            return null;
        }

        //Combine or filter based on logic
        //BossAttacks attack = FliterAndChooseAttack(stageSpecificAttacks, allAttacks);
        //return attack;
    }

    private BossAttacks FliterAndChooseAttack(BossStage.StageAction[] stageActions, BossAttacks[] allAttacks)
    {
        // Example: Favor attacks based on probability and conditions
        // Replace this with your selection logic
        return allAttacks[Random.Range(0, allAttacks.Length)];
    }

    private void ExecuteAttack(BossAttacks attack)
    {
        attackDamage = attack.GetDamage();
        Debug.Log($"{attack.attackName} deals {attackDamage} damage!");

        if (attack.appliesDebuff)
        {
            Debug.Log($"{attack.attackName} applies debuff: {attack.debuffName} for {attack.debuffDuration} seconds.");
            // Apply debuff logic here
        }
    }
}
