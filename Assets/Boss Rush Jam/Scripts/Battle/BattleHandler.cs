using Battle.Ability;
using Battle.BattleMana;
using Battle.Interact;
using Menu.RadialMenuSelect;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Random = UnityEngine.Random;

namespace Battle.Handler
{
    public class BattleHandler : MonoBehaviour
    {
        [SerializeField] private float turnLength;

        [SerializeField] private int playerHealth;
        [SerializeField] private int enemyHealth;
        [SerializeField] private int attackDamage;

        [SerializeField] private GameObject radialAttackMenu;

        private Boss boss;

        private int currentPlayerHealth;
        private int currentEnemyHealth;
        private bool isInTurn = false;

        private State state;
        private Mana playerMana;
        private Mana bossMana;

        public static event Action OnBattleEnd;
        public static event Action OnUIMap;

        private float manaAmountPlayer;
        private float manaAmountBoss;

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
            ButtonSelect.OnAttack += ReceivePlayerAttack;
        }

        private void OnDisable()
        {
            BattleInteract.OnStartBattle -= StartBattlePlayer;
            Boss.OnBossSpawned -= SetBossReferece;
            ButtonSelect.OnAttack -= ReceivePlayerAttack;
        }

        private void Start()
        {
            currentPlayerHealth = playerHealth;
            playerMana = new Mana();
            manaAmountPlayer = playerMana.ReturnCurrentManaValue();
        }

        private void SetBossReferece(Boss bossSpawned)
        {
            boss = bossSpawned;
            currentEnemyHealth = boss.ReturnBossHealth();

            bossMana = new Mana();
            manaAmountBoss = bossMana.ReturnCurrentManaValue();
        }

        private void StartBattlePlayer()
        {
            //Debug.Log("Battle started. Player's turn.");
            state = State.PlayerTurn;
            WaitForPlayerInput();
        }

        //public void OnPlayerInput(InputAction.CallbackContext context)
        //{
        //    if (state == State.PlayerTurn && context.performed && !isInTurn)
        //    {   
        //        //Debug.Log("Player input pressed");
        //        StopAllCoroutines();
        //        StartCoroutine(PlayerAttack());
        //        radialAttackMenu.SetActive(false);
                
        //    }
        //}

        private void ReceivePlayerAttack(AttackAbility attack)
        {
            radialAttackMenu.SetActive(false);
            StopAllCoroutines();
            StartCoroutine(PlayerAttack(attack));
        }

        public void OnShift(InputAction.CallbackContext context)
        {
            if(context.performed && !isInTurn)
            {
                radialAttackMenu.SetActive(true);
                //OnUIMap?.Invoke();
            }
        }

        private void WaitForPlayerInput()
        {
            Debug.Log("Waiting for player input...");
            // Here you can display UI or hints indicating it's the player's turn.
        }

        private IEnumerator PlayerAttack(AttackAbility attack)
        {
            isInTurn = true;
            //Debug.Log("Player attacks!");

            yield return new WaitForSeconds(turnLength);

            //float baseCost = 20f;

            //Check if the attack can be performed
            if (playerMana.CanPerformAttack(attack.manaCost))
            {
                float testMana = playerMana.AttackManaReduction(attack.manaCost);
                //Debug.Log("Player performed attack, player's current mana is: " + testMana);

                int damageDealt = Random.Range(attack.minDamage, attack.maxDamage + 1);

                boss.TakeDamage(damageDealt);
                currentEnemyHealth = boss.ReturnBossHealth();
                playerMana.ManaRegeneration(false, false, false, 0, false);
                bossMana.ManaRegeneration(false, false, false, 0, true);

                //Debug.Log($"After the player's attack, the boss's mana is: {bossMana.ReturnCurrentManaValue()}");

                //Debug.Log($"The players current mana is: {playerMana.ReturnCurrentManaValue()}");

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
            }
            else
            {
                Debug.Log("Player doesn't have the mana to perform attack");
            }
            

            isInTurn = false;
        }

        private IEnumerator EnemyAttack()
        {
            isInTurn = true;
            //Debug.Log("Enemies turn. Implement damage function");
            yield return new WaitForSeconds(turnLength);

            boss.PerformAction(bossMana);

            if(boss.GetAttackCost() > 0)
            {
                float testMana = bossMana.AttackManaReduction(boss.GetAttackCost());

                Debug.Log("Boss performed attack, boss's current mana is: " + testMana);


                playerMana.ManaRegeneration(false, false, false, 0, true);
                bossMana.ManaRegeneration(false, false, false, 0, false);

                currentPlayerHealth -= boss.ReturnAttackDamage();

                Debug.Log($"After the boss's attack, the player's mana is: {playerMana.ReturnCurrentManaValue()}");

                Debug.Log($"The enemies current mana is: {bossMana.ReturnCurrentManaValue()}");
            }
            else
            {
                Debug.Log("Boss skipped its turn");
            }

            //float baseCost = boss.GetAttackCost();
            //AttackType bossAttackType = boss.GetBossAttackType();
            //bossMana.ReturnCurrentMana(baseCost);
            //Debug.Log($"Boss used an attack costing {baseCost} mana.");
            //currentPlayerHealth -= boss.ReturnAttackDamage();
            //playerMana.ManaRegeneration(false, false, false, 0, true);
            //bossMana.ManaRegeneration(false, false, false, 0, false);

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

