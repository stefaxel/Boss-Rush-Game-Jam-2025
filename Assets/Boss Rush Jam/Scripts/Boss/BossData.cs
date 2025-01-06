using System.Collections;
using System.Collections.Generic;
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
    public BossAttacks[] attacks;

    public BossStage GetCurrentStage(int currentHealth)
    {
        foreach(BossStage stage in stages)
        {
            if (currentHealth <= stage.healthThreshold)
                return stage;
        }
        return stages[0];
    }
}
