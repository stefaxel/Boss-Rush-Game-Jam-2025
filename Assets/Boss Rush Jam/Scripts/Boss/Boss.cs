using Battle.BattleMana;
using Battle.CameraShake;
using Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Boss : MonoBehaviour
{
    [SerializeField] private BossData bossData;
    [SerializeField] private Transform particleSpawn;
    [SerializeField] private AudioClip hurtSound;

    private AudioSource audioSource;

    private int currentHealth;
    private BossStage currentStage;
    private int attackDamage;
    private BossAttacks selectedAttack;
    private Animator anim;
    private GameObject currentParticleEffect;

    public static event Action<Boss> OnBossSpawned;

    // Start is called before the first frame update
    void Start()
    {
        currentHealth = bossData.maxHealth;
        currentStage = bossData.GetCurrentStage(currentHealth);
        OnBossSpawned?.Invoke(this);
        anim = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
    }

    //Called in the BattleHandler to decide on an attack
    public void PerformAction(Mana bossMana)
    {
        selectedAttack = SelectedAttack(bossMana);
        if(selectedAttack != null )
        {
            ExecuteAttack(selectedAttack);
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
        if(hurtSound != null)
        {
            audioSource.PlayOneShot(hurtSound);
        }

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

    public float GetCurrentAttackDuration()
    {
        AnimatorStateInfo stateInfo = anim.GetCurrentAnimatorStateInfo(0);
        return stateInfo.length + currentParticleEffect.GetComponent<ParticleSystem>().main.duration;
    }

    public AttackType GetBossAttackType()
    {
        return selectedAttack.attackType;
    }

    private void TransitionToStage(BossStage newStage)
    {
        currentStage = newStage;
        anim.SetBool(newStage.nextStage, true);
        Debug.Log($"{bossData.bossName} transitions to stage: {currentStage.stageName}");
        // Trigger unique effects or animations for the new stage
    }

    //Selects Attack, may need to pass the mana avaliable so that it can make the attack
    private BossAttacks SelectedAttack(Mana bossMana)
    {
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
    }

    private void ExecuteAttack(BossAttacks attack)
    {
        anim.SetTrigger(attack.attackAnimation);

        if (attack.attackSound != null)
        {
            audioSource.PlayOneShot(attack.attackSound);
        }

        attackDamage = attack.GetDamage();

        Debug.Log($"{attack.attackName} deals {attackDamage} damage!");

        currentParticleEffect = attack.particlePrefab;
        Debug.Log($"Current particle effect = {currentParticleEffect}");
        float animationLength = anim.GetCurrentAnimatorStateInfo(0).length;

        if (attack.appliesDebuff)
        {
            Debug.Log($"{attack.attackName} applies debuff: {attack.debuffName} for {attack.debuffDuration} seconds.");
            // Apply debuff logic here
        }
    }

    private void SpawnParticleEffect()
    {
        Debug.Log("Enemy Spawning particles");
        if(currentParticleEffect != null)
        {
            Instantiate(currentParticleEffect, particleSpawn);
        }
    }

    private void ReturnToIdle()
    {
        anim.SetTrigger("idle");
    }
}
