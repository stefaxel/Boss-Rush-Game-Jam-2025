using Player;
using System;
using System.Collections;
using System.Collections.Generic;
using Transitions.FadeBlack;
using UnityEditor;
using UnityEngine;

namespace Battle.Interact
{
    public class BattleInteract : MonoBehaviour
    {
        [Header("Player Settings")]
        [SerializeField] private LayerMask playerLayer;
        [SerializeField] private Transform playerLane;

        [Header("Boss Settings")]
        [SerializeField] private Transform enemyLane;
        [SerializeField] private GameObject enemyPrefab;

        [SerializeField] private GameObject healthAndManaBar;
        [SerializeField] private GameObject interactKey;

        public static event Action OnInteract;
        public static event Action OnStopMovement;
        public static event Action OnStartBattle;
        public static event Action OnEnableMovement;
        private bool hasInteracted = false;
        private bool isPlayerInRange = false;

        private Vector2 storedPlayerPosition;
        private GameObject enemyGO;

        private RaycastHit2D hit;
        private PlayerController playerController;

        private void OnEnable()
        {
            Fade.OnSetupBattle += SetupArena;
            Fade.OnReturnToWorld += ReturnToWorld;
        }

        private void OnDisable()
        {
            Fade.OnSetupBattle -= SetupArena;
            Fade.OnReturnToWorld -= ReturnToWorld;
        }

        // Update is called once per frame
        void Update()
        {
            DetectPlayerInteraction();
        }

        private void DetectPlayerInteraction()
        {
            hit = Physics2D.BoxCast(transform.position, new Vector2(3.5f, 3.5f), 0f, Vector2.right, playerLayer);
            isPlayerInRange = hit.collider != null && hit.collider.gameObject.GetComponent<PlayerController>() != null;

            if(isPlayerInRange && !hasInteracted)
            {
                interactKey.SetActive(true);
                //Debug.Log("Player has been detected");
                playerController = hit.collider.gameObject.GetComponent<PlayerController>();

                if(playerController != null && playerController.InteractPressed)
                {
                    //Debug.Log("Player has interacted and the fade animation will be played");
                    OnInteract?.Invoke();
                    hasInteracted = true;
                }
            }
            else
            {
                interactKey.SetActive(false);
            }
        }

        private void SetupArena()
        {
            //Debug.Log("Handle moving character to arena");
            interactKey.SetActive(false);

            enemyGO = Instantiate(enemyPrefab, enemyLane.position, transform.rotation);
            storedPlayerPosition = playerController.transform.position;

            playerController.transform.position = playerLane.transform.position;
            OnStopMovement?.Invoke();
            OnStartBattle?.Invoke();
            healthAndManaBar.SetActive(true);
        }

        private void ReturnToWorld()
        {
            //Debug.Log("Moving player back to world");

            Destroy(enemyGO);
            healthAndManaBar.SetActive(false);

            playerController.transform.position = storedPlayerPosition;
            OnEnableMovement?.Invoke();
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(transform.position, new Vector2(3.5f, 3.5f));
        }
    }
}

