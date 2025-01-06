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

    // Start is called before the first frame update
    void Start()
    {
        currentHealth = bossData.maxHealth;
        currentStage = bossData.GetCurrentStage(currentHealth);
        Debug.Log($"{bossData.bossName} initialized with {currentHealth} health.");
    }

    public void PerformAction()
    {
        BossAttacks selectedAttack = SelectedAttack();
        ExecuteAttack(selectedAttack);
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
        int damage = attack.GetDamage();
        Debug.Log($"{attack.attackName} deals {damage} damage!");

        if (attack.appliesDebuff)
        {
            Debug.Log($"{attack.attackName} applies debuff: {attack.debuffName} for {attack.debuffDuration} seconds.");
            // Apply debuff logic here
        }
    }
}
