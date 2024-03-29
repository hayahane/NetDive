using NetDive.UI;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Interactions;

namespace NetDive.Player
{
    [RequireComponent(typeof(PlayerInput))]
    public class PlayerInputController : MonoBehaviour
    {
        [field: SerializeField] public PlayerCharacterController PC { get; set; }
        [field: SerializeField] public TpCameraController TpCam { get; set; }
        [field: SerializeField] public Camera Camera { get; set; }
        [field: SerializeField] public PlayerInput PlayerInput { get; set; }
        [field: SerializeField] public NetFormController NetFormController { get; set; }
        [field: SerializeField] public NetFormUIManager NetFormUIManager { get; set; }

        private Vector3 _moveInput;

        private void Start()
        {
            PlayerInput = GetComponent<PlayerInput>();
        }

        private void OnEnable()
        {
            Cursor.lockState = CursorLockMode.Locked;

            NetFormController.SourceChanged += TpCam.ChangeFocus;
            NetFormController.SourceChanged += NetFormUIManager.ChangeInputPrompt;

            PlayerInput.actions["ReleaseCursor"].started += UnlockCursor;

            PlayerInput.actions["Move"].performed += OnMove;
            PlayerInput.actions["Move"].canceled += OnMove;
            PlayerInput.actions["Run"].started += OnRun;
            PlayerInput.actions["Look"].performed += OnLook;
            PlayerInput.actions["Look"].canceled += OnLook;
            PlayerInput.actions["Jump"].started += OnJump;
            PlayerInput.actions["Jump"].canceled += OnJump;

            PlayerInput.actions["Attack"].started += LockCursor;
            PlayerInput.actions["Attack"].started += OnAttack;

            PlayerInput.actions["Interact"].started += OnDisconnect;
            PlayerInput.actions["Interact"].canceled += OnDisconnect;
            PlayerInput.actions["Interact"].performed += OnInteract;

            PlayerInput.actions["QuiteInteract"].performed += OnQuiteInteract;
        }

        private void OnDisable()
        {
            PlayerInput.actions["ReleaseCursor"].started -= UnlockCursor;

            PlayerInput.actions["Move"].performed -= OnMove;
            PlayerInput.actions["Move"].canceled -= OnMove;
            PlayerInput.actions["Run"].started -= OnRun;
            PlayerInput.actions["Look"].performed -= OnLook;
            PlayerInput.actions["Look"].canceled -= OnLook;
            PlayerInput.actions["Jump"].started -= OnJump;
            PlayerInput.actions["Jump"].canceled -= OnJump;

            PlayerInput.actions["Attack"].started -= LockCursor;
            PlayerInput.actions["Attack"].started -= OnAttack;

            PlayerInput.actions["Interact"].started -= OnDisconnect;
            PlayerInput.actions["Interact"].canceled -= OnDisconnect;
            PlayerInput.actions["Interact"].performed -= OnInteract;

            PlayerInput.actions["QuiteInteract"].performed -= OnQuiteInteract;
        }

        private void Update()
        {
            PC.MoveDirection = Quaternion.Euler(0, Camera.transform.eulerAngles.y, 0) * _moveInput;
        }

        private static void UnlockCursor(InputAction.CallbackContext context)
        {
            Cursor.lockState = CursorLockMode.None;
        }

        private static void LockCursor(InputAction.CallbackContext context)
        {
            Cursor.lockState = CursorLockMode.Locked;
        }

        private void OnRun(InputAction.CallbackContext context)
        {
            PC.IsRunning = true;
        }

        private void OnMove(InputAction.CallbackContext context)
        {
            if (context.canceled)
            {
                _moveInput = Vector3.zero;
                return;
            }

            var input = context.ReadValue<Vector2>();
            _moveInput = new Vector3(input.x, 0, input.y);
        }

        private void OnLook(InputAction.CallbackContext context)
        {
            if (context.canceled)
            {
                TpCam.LookInput = Vector2.zero;
                return;
            }

            TpCam.LookInput = context.ReadValue<Vector2>();
        }

        private void OnAttack(InputAction.CallbackContext context)
        {
            if (Cursor.lockState == CursorLockMode.None) return;

            PC.IsAttacking = true;
        }

        private void OnJump(InputAction.CallbackContext context)
        {
            if (context.started)
            {
                PC.BeginJump = true;
                PC.IsJumping = true;
                return;
            }

            if (context.canceled) PC.IsJumping = false;
        }

        private void OnDisconnect(InputAction.CallbackContext context)
        {
            if (NetFormController.SourceInHand == null) return;

            if (context is { canceled: true, interaction: not HoldInteraction })
            {
                NetFormUIManager.StopDisconnectCurrent();
                return;
            }

            NetFormUIManager.DisconnectCurrent();
        }

        private void OnInteract(InputAction.CallbackContext context)
        {
            switch (context.interaction)
            {
                case PressInteraction:
                    if (NetFormController.SourceInHand == null)
                    {
                        NetFormController.SelectSource();
                        if (NetFormController.SourceInHand != null)
                            PC.enabled = false;
                    }
                    else
                    {
                        NetFormController.ConnectInstance();
                    }

                    break;
                case HoldInteraction:
                    NetFormController.DisconnectInstance();
                    break;
            }
        }

        private void OnQuiteInteract(InputAction.CallbackContext context)
        {
            NetFormController.DeselectSource();
            PC.enabled = true;
        }
    }
}