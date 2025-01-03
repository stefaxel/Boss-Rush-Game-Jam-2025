using Battle;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Player
{
    public class PlayerController : MonoBehaviour
    {
        [Header("Player Movement")]
        [SerializeField] private float moveSpeed;

        private Rigidbody2D rb2d;
        private Vector2 movement;
        private bool canPlayerMove = true;

        public bool InteractPressed { get; private set; }


        // Start is called before the first frame update
        void Start()
        {
            rb2d = GetComponent<Rigidbody2D>();
        }

        private void OnEnable()
        {
            BattleInteract.OnStopMovement += StopMovement;
        }

        private void OnDisable()
        {
            BattleInteract.OnStopMovement -= StopMovement;
        }

        // Update is called once per frame
        void Update()
        {

        }

        private void FixedUpdate()
        {
            if(canPlayerMove)
                AddForcePlayer();
        }

        public void OnMove(InputAction.CallbackContext context)
        {
            movement = context.ReadValue<Vector2>();            
        }

        public void OnInteract(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                InteractPressed = true;
            }
            else if (context.canceled)
            {
                InteractPressed = false;
            }
        }

        private void AddForcePlayer()
        {
            rb2d.velocity = Vector2.ClampMagnitude(rb2d.velocity, moveSpeed);
            Vector2 move = (movement.y * transform.up).normalized + (movement.x * transform.right).normalized;
            rb2d.MovePosition(rb2d.position + move * moveSpeed * Time.deltaTime);
        }

        private void StopMovement()
        {
            canPlayerMove = false;
        }

        private void StartMovement()
        {
            canPlayerMove = true;
        }
    }
}

