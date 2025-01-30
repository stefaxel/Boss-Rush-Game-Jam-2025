using Battle.Handler;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Battle.UI
{
    public class PlayerHealthAndMana : MonoBehaviour
    {
        [SerializeField] private Image healthFill;
        [SerializeField] private Image manaFill;

        private int currentPlayerHealth;
        private int startPlayerHealth;
        private float currentPlayerMana;
        private float startPlayerMana;

        private void OnEnable()
        {
            BattleHandler.OnStartHealthMana += SetStartHealthAndMana;
            BattleHandler.OnCurrentHealthMana += UpdateHealthAndManaBar;
        }

        private void OnDisable()
        {
            BattleHandler.OnStartHealthMana -= SetStartHealthAndMana;
            BattleHandler.OnCurrentHealthMana -= UpdateHealthAndManaBar;
        }

        private void SetStartHealthAndMana(int health, float mana)
        {
            Debug.Log("Recieved event for getting the health and mana of player at start of battle");
            startPlayerHealth = health;
            startPlayerMana = mana;
        }

        private void UpdateHealthAndManaBar(int health, float mana)
        {
            Debug.Log("Adjusting the fill of health and mana bars");
            currentPlayerHealth = health;
            currentPlayerMana = mana;

            Debug.Log($"current player health = {currentPlayerHealth} current player mana = {currentPlayerMana}");

            float healthFillAmount = (float)currentPlayerHealth / (float)startPlayerHealth;
            float manaFillAmount = currentPlayerMana / startPlayerMana;

            Debug.Log($"heath fill amount: {healthFillAmount} mana fill amount: {manaFillAmount}");

            healthFill.fillAmount = healthFillAmount;
            manaFill.fillAmount = manaFillAmount;
        }
    }
}

