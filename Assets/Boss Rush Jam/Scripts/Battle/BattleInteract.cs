using Player;
using System;
using System.Collections;
using System.Collections.Generic;
using Transitions;
using UnityEngine;

namespace Battle
{
    public class BattleInteract : MonoBehaviour
    {
        [SerializeField] private LayerMask playerLayer;
        [SerializeField] private Transform playerLane;

        public static event Action OnInteract;
        public static event Action OnStopMovement;
        private bool hasInteracted = false;
        private bool isPlayerInRange = false;

        private RaycastHit2D hit;
        private PlayerController playerController;

        private void OnEnable()
        {
            Fade.OnSetupBattle += MoveCharacterToArena;
        }

        private void OnDisable()
        {
            Fade.OnSetupBattle -= MoveCharacterToArena;
        }

        // Update is called once per frame
        void Update()
        {
            DetectPlayerInteraction();
        }

        private void DetectPlayerInteraction()
        {
            hit = Physics2D.BoxCast(transform.position, new Vector2(1f, 1f), 0f, Vector2.right, playerLayer);
            isPlayerInRange = hit.collider != null && hit.collider.gameObject.GetComponent<PlayerController>() != null;

            if(isPlayerInRange && !hasInteracted)
            {
                Debug.Log("Player has been detected");
                playerController = hit.collider.gameObject.GetComponent<PlayerController>();

                if(playerController != null && playerController.InteractPressed)
                {
                    Debug.Log("Player has interacted and the fade animation will be played");
                    OnInteract?.Invoke();
                    hasInteracted = true;
                }
            }
        }

        private void MoveCharacterToArena()
        {
            Debug.Log("Handle moving character to arena");

            playerController.transform.position = playerLane.transform.position;
            OnStopMovement?.Invoke();
        }
    }
}

