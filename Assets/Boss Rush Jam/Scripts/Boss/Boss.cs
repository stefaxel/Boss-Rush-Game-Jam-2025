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
        Debug.Log($"{bossData.bossName} initialized with {currentHealth} health.");
    }

    public void PerformAction()
    {
        selectedAttack = SelectedAttack();
        ExecuteAttack(selectedAttack);

        Debug.Log($"Attack cost for {selectedAttack.attackName} of attack type {selectedAttack.attackType}: {selectedAttack.attackCost}");
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;

        BossStage newStage = bossData.GetCurrentStage(currentHealth);
        if (newStage != currentStage)
        {
            TransitionToStage(newStage);
        }

        if (currentHealth <= 0)
        {
            Debug.Log($"{bossData.bossName} defeated!");
        }
    }

    public int ReturnBossHealth()
    {
        Debug.Log($"{bossData.name} current health: {currentHealth}");
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

    private BossAttacks SelectedAttack()
    {
        BossStage.StageAction[] stageSpecificAttacks = currentStage.stageActions; //Get the stage-specific actions
        BossAttacks[] allAttacks = bossData.attacks; //Get all possible attacks

        //Combine or filter based on logic
        BossAttacks attack = FliterAndChooseAttack(stageSpecificAttacks, allAttacks);
        return attack;
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
