using System;
using UnityEngine.InputSystem;
public class InputManager
{
    private PlayerControls playerControls;

    public float Movement => playerControls.Gameplay.Movement.ReadValue<float>();
    public bool IsJumpHeld => playerControls.Gameplay.Jump.ReadValue<float>() > 0;

    public event Action OnJump;
    public event Action OnAttack;
    public event Action OnAttack2;
    public event Action OnJumpUp;
    

    public InputManager()
    {
        playerControls = new PlayerControls();
        playerControls.Gameplay.Enable();

        playerControls.Gameplay.Jump.performed += OnJumpPerformed;
        playerControls.Gameplay.Attack.performed += OnAttackPerformed;
        playerControls.Gameplay.Attack2.performed += OnAttack2Performed;
        playerControls.Gameplay.Jump.canceled += OnJumpCanceled;
    }
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
    private void OnAttack2Performed(InputAction.CallbackContext obj)
    {
        OnAttack2?.Invoke();
    }

    public void DisablePlayerInput()
    {
        playerControls.Gameplay.Disable();
    }
}
