using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SwordVisual : MonoBehaviour
{
    [SerializeField] private Sword sword;
    [SerializeField] private int _damageAmount = 2;
    private Animator animator;
    private const string ATTACK = "Attack";
    private CapsuleCollider2D _capsuleCollider2D;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        _capsuleCollider2D = GetComponent<CapsuleCollider2D>();
    }

    private void Start()
    {
        if (GameInput.Instance != null)
        {
            GameInput.Instance.OnPlayerAttack += OnPlayerAttack;
            Debug.Log("SwordVisual: Подписка на OnPlayerAttack успешно выполнена");
        }
        else
        {
            Debug.LogError("SwordVisual: GameInput.Instance не найден!");
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
                StartCoroutine(ShowAndHide());
                Debug.Log("SwordVisual: Анимация и логика атаки запущены");
            }
            else
            {
                Debug.Log("SwordVisual: Атака заблокирована (инвентарь открыт или клик по UI)");
            }
        }
    }

    private IEnumerator ShowAndHide()
    {
        gameObject.GetComponent<Renderer>().enabled = true;
        _capsuleCollider2D.enabled = true;
        yield return new WaitForSeconds(0.3f);
        gameObject.GetComponent<Renderer>().enabled = false;
        _capsuleCollider2D.enabled = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.transform.TryGetComponent(out EnemyEntity enemyEntity))
        {
            enemyEntity.TakeDamage(_damageAmount);
            Debug.Log("SwordVisual: Урон нанесён врагу");
        }
    }
}