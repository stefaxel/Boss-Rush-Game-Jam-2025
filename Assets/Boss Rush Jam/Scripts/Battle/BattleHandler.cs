using Battle.Ability;
using Battle.BattleMana;
using Battle.CameraShake;
using Battle.Interact;
using Cinemachine;
using Menu.RadialMenuSelect;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
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
        [SerializeField] private AudioClip hurtSound;

        [SerializeField] private GameObject radialAttackMenu;
        [SerializeField] private GameObject shiftKeyHint;
        [SerializeField] private GameObject qteHintGO;
        [SerializeField] private TMP_Text qteHintText;

        private Boss boss;
        private AudioSource audioSource;

        private int currentPlayerHealth;
        private int currentEnemyHealth;
        private bool isInTurn = false;
        private bool hasBattleStarted = false;

        private State state;
        private Mana playerMana;
        private Mana bossMana;

        public static event Action OnBattleEnd;
        public static event Action OnUIMap;
        public static event Action<int, float> OnStartHealthMana;
        public static event Action<int, float> OnCurrentHealthMana;

        private InputAction blockAction;
        private InputControl currentQTE;
        private bool isQTEActive = false;
        private bool isQTESuccess = false;
        private float qteTimeLimit = 5f;
        private Coroutine qteCoroutine;

        private float manaAmountPlayer;
        private float manaAmountBoss;

        private CinemachineImpulseSource impulseSource;

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
            Boss.OnBossSpawned += SetBossReference;
            ButtonSelect.OnAttack += ReceivePlayerAttack;
        }

        private void OnDisable()
        {
            BattleInteract.OnStartBattle -= StartBattlePlayer;
            Boss.OnBossSpawned -= SetBossReference;
            ButtonSelect.OnAttack -= ReceivePlayerAttack;
        }

        private void Start()
        {
            currentPlayerHealth = playerHealth;
            playerMana = new Mana();
            manaAmountPlayer = playerMana.ReturnCurrentManaValue();
            impulseSource = GetComponent<CinemachineImpulseSource>();
            audioSource = GetComponent<AudioSource>();

            PlayerInput playerInput = FindObjectOfType<PlayerInput>();
            blockAction = playerInput.actions["Block"];

            //blockAction.performed += context => CheckQTE(context);
        }

        private void SetBossReference(Boss bossSpawned)
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
                shiftKeyHint.SetActive(false);
                //OnUIMap?.Invoke();
            }
        }

        private void WaitForPlayerInput()
        {
            shiftKeyHint.SetActive(true);
        }

        private IEnumerator PlayerAttack(AttackAbility attack)
        {
            isInTurn = true;
            //Debug.Log("Player attacks!");

            if(!hasBattleStarted)
            {
                OnStartHealthMana?.Invoke(playerHealth, manaAmountPlayer);
                hasBattleStarted = true;
            }

            float animationDuration = AnimationController.instance.GetAnimationLength(attack.animationTrigger);
            float particleDuration = attack.particlePrefab != null ? attack.particlePrefab.GetComponent<ParticleSystem>().main.duration : 0;

            float attackDuration = animationDuration + particleDuration;

            yield return new WaitForSeconds(attackDuration);


            //Check if the attack can be performed
            if (playerMana.CanPerformAttack(attack.manaCost))
            {
                manaAmountPlayer = playerMana.AttackManaReduction(attack.manaCost);

                int damageDealt = Random.Range(attack.minDamage, attack.maxDamage + 1);

                boss.TakeDamage(damageDealt);
                currentEnemyHealth = boss.ReturnBossHealth();
                CameraShakeManager.instance.CameraShake(impulseSource);

                playerMana.ManaRegeneration(false, false, false, 0, false);
                bossMana.ManaRegeneration(false, false, false, 0, true);

                manaAmountPlayer = playerMana.ReturnCurrentManaValue();
                Debug.Log($"Current mana amount player: {manaAmountPlayer}");

                OnCurrentHealthMana?.Invoke(currentPlayerHealth,manaAmountPlayer);

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

            boss.PerformAction(bossMana);

            float animationDuration = boss.GetCurrentAttackDuration();

            StartQTE();

            yield return new WaitForSeconds(animationDuration);

            if (boss.GetAttackCost() > 0)
            {
                float testMana = bossMana.AttackManaReduction(boss.GetAttackCost());

                Debug.Log("Boss performed attack, boss's current mana is: " + testMana);

                playerMana.ManaRegeneration(false, false, false, 0, true);
                bossMana.ManaRegeneration(false, false, false, 0, false);

                int baseDamage = boss.ReturnAttackDamage();
                float damageMultiplier = isQTESuccess ? 0.5f : 1.5f;
                int finalDamage = Mathf.RoundToInt(baseDamage * (int)damageMultiplier);

                currentPlayerHealth -= finalDamage; //-= boss.ReturnAttackDamage();
                if(hurtSound != null)
                {
                    Debug.Log("Playing attack for enemy");
                    audioSource.PlayOneShot(hurtSound);
                }
                
                OnCurrentHealthMana?.Invoke(currentPlayerHealth, playerMana.ReturnCurrentManaValue());

                Debug.Log($"After the boss's attack, the player's mana is: {playerMana.ReturnCurrentManaValue()}");

                Debug.Log($"The enemies current mana is: {bossMana.ReturnCurrentManaValue()}");
            }
            else
            {
                Debug.Log("Boss skipped its turn");
            }

            CameraShakeManager.instance.CameraShake(impulseSource);

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

            //isInTurn = false;
        } 

        private void StartQTE()
        {
            if (isQTEActive) return;

            isQTEActive = true;

            List<InputControl> controls = blockAction.controls.ToList();
            if (controls.Count == 0) return;

            currentQTE  = controls[Random.Range(0, controls.Count)];

            Debug.Log($"Press {currentQTE.displayName} to block!");

            qteHintGO.SetActive(true);
            qteHintText.text = $"Press {currentQTE.displayName}";

            qteCoroutine = StartCoroutine(QTETimeout());
        }

        public void CheckQTE(InputAction.CallbackContext context)
        {
            if (!isQTEActive) return;

            if(context.control == currentQTE)
            {
                isQTEActive = false;
                StopCoroutine(QTETimeout());
                qteHintGO.SetActive(false);
                BlockSuccess();
            }
            else
            {
                isQTEActive = false;
                qteHintGO.SetActive(false);
                BlockFail();
            }
        }

        private IEnumerator QTETimeout()
        {
            yield return new WaitForSeconds(qteTimeLimit);

            if (isQTEActive)
            {
                isQTEActive = false;
                qteHintGO.SetActive(false);
                BlockFail();
            }
        }

        private void BlockSuccess()
        {
            Debug.Log("Player successfully blocked the attack");
            isQTESuccess = true;
            isInTurn = false;
        }

        private void BlockFail()
        {
            Debug.Log("Block failed! Taking extra damage!");
            isQTESuccess = false;
            isInTurn = false;
        }
    }
}

