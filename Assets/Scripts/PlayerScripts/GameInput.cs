using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameInput : MonoBehaviour
{
    public static GameInput Instance {get; private set;}
    private PlayerControls playerControls;
    public event EventHandler OnPlayerAttack;
    private void Awake() {
        Instance = this;

        playerControls = new PlayerControls();
        playerControls.Enable();

        playerControls.Combat.Attack.started+= PlayerAttack_started;
    }
    private void PlayerAttack_started(InputAction.CallbackContext obj) {
        OnPlayerAttack?.Invoke(this, EventArgs.Empty); 
    }

    public Vector2 GetMovementVector() {
        Vector2 inputVector = playerControls.Movement.Move.ReadValue<Vector2>();
        return inputVector;
    }
}
