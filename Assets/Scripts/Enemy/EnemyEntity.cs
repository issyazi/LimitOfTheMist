using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(PolygonCollider2D))]
[RequireComponent(typeof(CapsuleCollider2D))]
[RequireComponent(typeof(EnemyAI))]
public class EnemyEntity : MonoBehaviour
{
    public event EventHandler OnTakeHit;
    public event EventHandler OnDeath;

    [SerializeField] private int _maxHealth;
    [SerializeField] private EnemyDifficulty difficulty; // Сложность врага
    [SerializeField] private GameObject echoPrefab; // Префаб Echo (ID 4)

    public enum EnemyDifficulty { Weak, Medium, Hard } // Типы сложности врага

    private PolygonCollider2D _polygonCollider2D;
    private CapsuleCollider2D _capsuleCollider2D;
    private EnemyAI _enemyAI;
    private int _currentHealth;

    private void Awake()
    {
        _polygonCollider2D = GetComponent<PolygonCollider2D>();
        _capsuleCollider2D = GetComponent<CapsuleCollider2D>();
        _enemyAI = GetComponent<EnemyAI>();
    }

    private void Start()
    {
        _currentHealth = _maxHealth;
    }

    public void TakeDamage(int damage)
    {
        if (_currentHealth > 0)
        {
            _currentHealth -= damage;
            OnTakeHit?.Invoke(this, EventArgs.Empty);
        }
        DetectDeath();
    }

    private void DetectDeath()
    {
        if (_currentHealth <= 0)
        {
            _capsuleCollider2D.enabled = false;
            _polygonCollider2D.enabled = false;
            _enemyAI.SetDeathState();
            OnDeath?.Invoke(this, EventArgs.Empty);
            DropEcho(); // Вызываем метод выпадения Echo
            StartCoroutine(TimeToDie());
        }
    }

    private void DropEcho()
    {
        if (echoPrefab == null)
        {
            Debug.LogError("Echo prefab not assigned on " + gameObject.name);
            return;
        }

        // Определяем количество Echo в зависимости от сложности
        int echoCount = 0;
        switch (difficulty)
        {
            case EnemyDifficulty.Weak:
                echoCount = UnityEngine.Random.Range(0, 6); // Слабые: 0-5
                break;
            case EnemyDifficulty.Medium:
                echoCount = UnityEngine.Random.Range(5, 16); // Средние: 5-15
                break;
            case EnemyDifficulty.Hard:
                echoCount = UnityEngine.Random.Range(15, 51); // Сложные: 15-50
                break;
        }

        if (echoCount > 0)
        {
            // Создаём Echo на месте смерти врага
            GameObject echo = Instantiate(echoPrefab, transform.position, Quaternion.identity);
            EchoPickup echoPickup = echo.GetComponent<EchoPickup>();
            if (echoPickup != null)
            {
                echoPickup.count = echoCount;
            }
            else
            {
                Debug.LogError("EchoPickup component not found on echo prefab!");
            }
        }
    }

    public void PolygonColliderTurnOff()
    {
        _polygonCollider2D.enabled = false;
    }

    public void PolygonColliderTurnOn()
    {
        _polygonCollider2D.enabled = true;
    }

    IEnumerator TimeToDie()
    {
        yield return new WaitForSeconds(5f);
        Destroy(gameObject);
    }
}