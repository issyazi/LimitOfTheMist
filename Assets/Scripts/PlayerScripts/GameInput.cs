using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class GameInput : MonoBehaviour
{
    public static GameInput Instance { get; private set; }
    [SerializeField] private Inventory inventory;
    private PlayerControls playerControls;
    public event EventHandler OnPlayerAttack;
    private bool isCombatEnabled = true;
    private bool wasUIClickedThisFrame = false;

    public Inventory Inventory => inventory; // Публичное свойство для доступа к inventory

    private void Awake()
    {
        Instance = this;
        playerControls = new PlayerControls();
        playerControls.Enable();

        playerControls.Combat.Attack.started += PlayerAttack_started;

        if (inventory == null)
        {
            Debug.LogError("GameInput: Inventory не привязан в Inspector!");
        }
    }

    private void Update()
    {
        wasUIClickedThisFrame = false;
    }

    private void PlayerAttack_started(InputAction.CallbackContext obj)
    {
        StartCoroutine(HandleAttack());
    }

    private IEnumerator HandleAttack()
    {
        yield return null;

        if (inventory == null)
        {
            Debug.LogError("Inventory не привязан в GameInput!");
            yield break;
        }
        if (EventSystem.current == null)
        {
            Debug.LogError("EventSystem отсутствует в сцене!");
            yield break;
        }

        bool isOverUI = EventSystem.current.IsPointerOverGameObject();
        Debug.Log($"Попытка атаки. Инвентарь открыт: {inventory.IsInventoryOpen}, Над UI: {isOverUI}, Текущий выбранный объект: {EventSystem.current.currentSelectedGameObject?.name}, Combat включён: {isCombatEnabled}, UI клик в этом кадре: {wasUIClickedThisFrame}");

        if (inventory.IsInventoryOpen || isOverUI || !isCombatEnabled || wasUIClickedThisFrame)
        {
            Debug.Log("Атака заблокирована: инвентарь открыт, клик по UI, Combat отключён или UI клик в этом кадре");
            yield break;
        }

        OnPlayerAttack?.Invoke(this, EventArgs.Empty);
    }

    public void SetCombatEnabled(bool enabled)
    {
        Debug.Log($"SetCombatEnabled: {enabled}");
        isCombatEnabled = enabled;
        if (enabled)
        {
            playerControls.Combat.Enable();
        }
        else
        {
            playerControls.Combat.Disable();
        }
    }

    public void MarkUIClicked()
    {
        wasUIClickedThisFrame = true;
    }

    public Vector2 GetMovementVector()
    {
        Vector2 inputVector = playerControls.Movement.Move.ReadValue<Vector2>();
        return inputVector;
    }

    private void OnDestroy()
    {
        if (playerControls != null)
        {
            playerControls.Combat.Attack.started -= PlayerAttack_started;
            playerControls.Dispose();
        }
    }

    // Статический метод для проверки активности диалога
    public static bool IsDialogueActive()
    {
        NPCKaira[] kairas = FindObjectsOfType<NPCKaira>();
        foreach (var kaira in kairas)
        {
            if (kaira.IsTalking) return true; // Используем публичное свойство
        }
        return false;
    }
}