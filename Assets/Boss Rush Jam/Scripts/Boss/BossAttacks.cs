using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BossAttacks", menuName = "Boss/BossAttacks")]
public class BossAttacks : ScriptableObject
{
    [Header("General Attack Info")]
    public string attackName;
    public int minDamage;
    public int maxDamage;
    public float probability;
    public bool isSpecialAttack;
    public float attackCost;
    public AttackType attackType;

    [Header("Debuff Effects")]
    public bool appliesDebuff;
    public string debuffName;
    public float debuffDuration;
    public int debuffIntensity;

    [Header("Battle State Conditions")]
    public bool favourWhenPlayerHealthLow;
    public bool favourWhenPlayerBuffed;
    public bool favourWhenBossHealthLow;

    public int GetDamage()
    {
        return Random.Range(minDamage, maxDamage + 1);
    }

}
