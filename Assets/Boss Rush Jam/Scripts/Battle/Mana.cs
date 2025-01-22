using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Battle.BattleMana
{
    public class Mana
    {
        private float currentMana = 100f;
        private float maxMana = 100f;
        private float manaRegeneration = 10f;
        //private float manaCost;
        private float manaOvercharged = 120f;
        private float penaltyThreshold = 180f;

        //Checks to see if mana has reached a level for critical attacks
        public ManaCheckResults CheckManaResults()
        {
            ManaCheckResults results = new ManaCheckResults();

            if(currentMana > maxMana)
            {
                results.isOvercharged = true;

                if (IsPenaltyApplied(currentMana))
                {
                    results.isPenaltyApplied = true;
                }
                else
                {
                    results.isSafe = true;
                }
            }
            else
            {
                results.isOvercharged = false;
                results.isSafe = true;
            }

            return results;
        }

        public bool CanPerformAttack(float cost) //, AttackType attackType
        {
            return currentMana >= cost;
        }

        //Regenerates Mana for player and boss
        public float ManaRegeneration(bool hasUsedFocus, bool hasSkippedTurn, bool isItemUsed, float itemManaRegen, bool hasTakenDmage)
        {
            float regen = manaRegeneration;

            if (hasUsedFocus)
            {
                regen =+ 20f;
            }
            if(isItemUsed)
            {
                regen =+ itemManaRegen;
            }
            if (hasTakenDmage)
            {
                regen =+ 5f;
            }
            if (hasSkippedTurn)
            {
                return currentMana = manaOvercharged;
            }

            currentMana = Mathf.Min(currentMana + regen, penaltyThreshold);
            //Debug.Log("The mana regenerated is: " + currentMana);
            return currentMana;
        }

        //Returns the current mana once an attack is performed
        public float AttackManaReduction(float baseCost) //
        {
            return currentMana -= baseCost;
        }

        public float ReturnCurrentManaValue()
        {
            return currentMana;
        }

        private bool IsPenaltyApplied(float currentMana)
        {
            float ratio = currentMana / penaltyThreshold;
            float probability = Mathf.Log(ratio + 1) / Mathf.Log(2);
            float randomValue = Random.Range(0, 1f);
            
            return randomValue < probability;
        }
    }

    public struct ManaCheckResults
    {
        public bool isOvercharged;
        public bool isPenaltyApplied;
        public bool isSafe;
    }
}

