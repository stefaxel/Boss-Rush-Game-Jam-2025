using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "BossData", menuName = "Boss/BossData")]
public class BossData : ScriptableObject
{
    public string bossName;
    public int maxHealth;
    public float criticalHitChance;

    [Header("Stages")]
    public BossStage[] stages;

    [Header("Attacks")]
    public BossAttacks[] defaultAttacks;

    public BossStage GetCurrentStage(int currentHealth)
    {
        BossStage[] sortedStage = stages.OrderBy(stage => stage.healthThreshold).ToArray();

        foreach(BossStage stage in sortedStage)
        {
            if (currentHealth <= stage.healthThreshold)
            {
                Debug.Log($"Current health: {currentHealth}. Returning stage: {stage.stageName} (Threshold: {stage.healthThreshold})");
                return stage;
            }
        }
        Debug.Log($"Current health: {currentHealth}. Returning default stage: {stages[0].stageName}");
        return sortedStage[0];
    }
}
