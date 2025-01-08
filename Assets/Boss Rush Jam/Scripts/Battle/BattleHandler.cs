using Battle.BattleMana;
using Battle.Interact;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Battle.Handler
{
    public class BattleHandler : MonoBehaviour
    {
        [SerializeField] private float turnLength;

        [SerializeField] private int playerHealth;
        [SerializeField] private int enemyHealth;
        [SerializeField] private int attackDamage;

        private Boss boss;

        private int currentPlayerHealth;
        private int currentEnemyHealth;
        private bool isInTurn = false;

        private State state;
        private Mana playerMana;
        private Mana bossMana;

        public static event Action OnBattleEnd;

        private enum State
        {
            PlayerTurn,
            EnemyTurn,
            CriticalHitPlayer,
            PlayerWin,
            EnemyWin,
        }

        private void OnEnable()
        {
            BattleInteract.OnStartBattle += StartBattlePlayer;
            Boss.OnBossSpawned += SetBossReferece;
        }

        private void OnDisable()
        {
            BattleInteract.OnStartBattle -= StartBattlePlayer;
            Boss.OnBossSpawned -= SetBossReferece;
        }

        private void Start()
        {
            currentPlayerHealth = playerHealth;
            playerMana = new Mana();
        }

        private void SetBossReferece(Boss bossSpawned)
        {
            boss = bossSpawned;
            currentEnemyHealth = boss.ReturnBossHealth();

            bossMana = new Mana();
        }

        private void StartBattlePlayer()
        {
            Debug.Log("Battle started. Player's turn.");
            state = State.PlayerTurn;
            WaitForPlayerInput();
        }

        public void OnPlayerInput(InputAction.CallbackContext context)
        {
            if (state == State.PlayerTurn && context.performed && !isInTurn)
            {   
                //Debug.Log("Player input pressed");
                StopAllCoroutines();
                StartCoroutine(PlayerAttack());
                
            }
        }

        private void WaitForPlayerInput()
        {
            Debug.Log("Waiting for player input...");
            // Here you can display UI or hints indicating it's the player's turn.
        }

        private IEnumerator PlayerAttack()
        {
            isInTurn = true;
            //Debug.Log("Player attacks!");

            yield return new WaitForSeconds(turnLength);

            float baseCost = 20f;
            playerMana.ReturnCurrentMana(baseCost, AttackType.Basic);

            boss.TakeDamage(attackDamage);
            currentEnemyHealth = boss.ReturnBossHealth();
       
            if (currentEnemyHealth <= 0)
            {
                state = State.PlayerWin;
                Debug.Log("Player wins!");
                OnBattleEnd?.Invoke();
            }
            else
            {
                state = State.EnemyTurn;
                StartCoroutine(EnemyAttack());
            }

            isInTurn = false;
        }

        private IEnumerator EnemyAttack()
        {
            isInTurn = true;
            Debug.Log("Enemies turn. Implement damage function");
            yield return new WaitForSeconds(turnLength);

            boss.PerformAction();
            float baseCost = boss.GetAttackCost();
            AttackType bossAttackType = boss.GetBossAttackType();
            bossMana.ReturnCurrentMana(baseCost, bossAttackType);
            Debug.Log($"Boss used an attack costing {baseCost} mana.");
            currentPlayerHealth -= boss.ReturnAttackDamage();

            if (currentPlayerHealth <= 0)
            {
                state = State.EnemyWin;
                Debug.Log("Enemy wins!");
            }
            else
            {
                state = State.PlayerTurn;
                WaitForPlayerInput();
            }

            isInTurn = false;
        }
    }
}

