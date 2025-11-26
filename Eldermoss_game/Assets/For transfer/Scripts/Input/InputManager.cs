using System;
using UnityEngine.InputSystem;

public class InputManager
{
    private PlayerControls playerControls;

    // Properties for å lese input
    public float Movement => playerControls.Gameplay.Movement.ReadValue<float>();
    public bool IsJumpHeld => playerControls.Gameplay.Jump.ReadValue<float>() > 0;

    // Events for å signalisere handlinger
    public event Action OnJump;
    public event Action OnAttack;
    public event Action OnJumpUp;

    public InputManager()
    {
        playerControls = new PlayerControls();
        playerControls.Gameplay.Enable(); // Aktiveres ved oppstart

        // Kobler input actions til private event-handlere (må være i samme klasse!)
        playerControls.Gameplay.Jump.performed += OnJumpPerformed;
        playerControls.Gameplay.Attack.performed += OnAttackPerformed;
        playerControls.Gameplay.Jump.canceled += OnJumpCanceled;
    }

    // --- PRIVATE HANDLERE (Kalt av Unity Input System) ---

    private void OnJumpPerformed(InputAction.CallbackContext context)
    {
        OnJump?.Invoke();
    }

    private void OnJumpCanceled(InputAction.CallbackContext context)
    {
        OnJumpUp?.Invoke();
    }

    private void OnAttackPerformed(InputAction.CallbackContext obj)
    {
        OnAttack?.Invoke();
    }

    // --- OFFENTLIGE KONTROLL-FUNKSJONER (Kalt av PauseMenu/Health) ---

    public void DisablePlayerInput()
    {
        // Deaktiverer Action Map når spillet pauser eller dør
        playerControls.Gameplay.Disable();
    }

    public void EnablePlayerInput()
    {
        // Aktiverer Action Map når spillet fortsetter
        playerControls.Gameplay.Enable();
    }
}