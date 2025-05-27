using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerVisual : MonoBehaviour
{
    private Animator animator;
    private const string IS_MOVING = "Moving";
    private const string ATTACK = "Attack";
    private bool isMove;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    private void Start()
    {
        if (GameInput.Instance != null)
        {
            GameInput.Instance.OnPlayerAttack += OnPlayerAttack;
            Debug.Log("PlayerVisual: Подписка на OnPlayerAttack успешно выполнена");
        }
        else
        {
            Debug.LogError("PlayerVisual: GameInput.Instance не найден!");
        }
    }

    private void OnDestroy()
    {
        if (GameInput.Instance != null)
        {
            GameInput.Instance.OnPlayerAttack -= OnPlayerAttack;
        }
    }

    private void Update()
    {
        isMove = PlayerController.Instance.Moving();
        animator.SetBool(IS_MOVING, isMove);

        Vector2 inputVector = GameInput.Instance.GetMovementVector();
        inputVector = inputVector.normalized;

        if (inputVector != Vector2.zero && Time.timeScale != 0)
        {
            animator.SetFloat("moveX", inputVector.x);
            animator.SetFloat("moveY", inputVector.y);
        }
    }

    private void OnPlayerAttack(object sender, System.EventArgs e)
    {
        if (GameInput.Instance != null && GameInput.Instance.Inventory != null) // Используем Inventory
        {
            bool isOverUI = EventSystem.current != null && EventSystem.current.IsPointerOverGameObject();
            if (!GameInput.Instance.Inventory.IsInventoryOpen && !isOverUI)
            {
                animator.SetTrigger(ATTACK);
                Debug.Log("PlayerVisual: Анимация атаки запущена");
            }
            else
            {
                Debug.Log("PlayerVisual: Атака заблокирована (инвентарь открыт или клик по UI)");
            }
        }
    }
}