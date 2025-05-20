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
    private PolygonCollider2D _polygonCollider2D;
    private CapsuleCollider2D _capsuleCollider2D;
    private EnemyAI _enemyAI;
    private void Awake() {
        _polygonCollider2D = GetComponent<PolygonCollider2D>();
        _capsuleCollider2D = GetComponent<CapsuleCollider2D>();
        _enemyAI = GetComponent<EnemyAI>();

    }

    private int _currentHealth;
    private void Start() {
        _currentHealth = _maxHealth;

    }
    public void TakeDamage (int damage) {
        if (_currentHealth > 0) {
            _currentHealth -= damage;
            OnTakeHit?.Invoke(this, EventArgs.Empty);
        }
        DetectDeath();
    }
    private void DetectDeath() {
        if (_currentHealth <= 0) {  
            _capsuleCollider2D.enabled = false;
            _polygonCollider2D.enabled = false;
            _enemyAI.SetDeathState();
            OnDeath?.Invoke(this, EventArgs.Empty);
            StartCoroutine(TimeToDie());
        }
    }
    public void PolygonColliderTurnOf() {
        _polygonCollider2D.enabled = false;
    }
    public void PolygonColliderTurnOn() {
        _polygonCollider2D.enabled = true;
    }
    IEnumerator TimeToDie() {
        yield return new WaitForSeconds(5f);
        Destroy(gameObject);
    }
}
