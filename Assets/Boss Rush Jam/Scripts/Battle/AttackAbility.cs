using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Battle.Ability
{
    [CreateAssetMenu(fileName = "NewAbility", menuName ="Abilities/Ability")]
    public class AttackAbility : ScriptableObject
    {
        public string abilityName;
        public int maxDamage;
        public int minDamage;
        public Sprite abilityIcon;
        public float manaCost;
        public bool isUnlocked;
        public string animationTrigger;
        public GameObject particlePrefab;
    }
}

