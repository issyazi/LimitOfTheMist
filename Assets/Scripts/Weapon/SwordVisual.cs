using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordVisual : MonoBehaviour
{
    [SerializeField] private Sword sword;
    [SerializeField] private int _damageAmount = 2;
    private Animator animator;
    private const string ATTACK = "Attack";
    private CapsuleCollider2D _capsuleCollider2D;

    
    private void Awake() {
        animator = GetComponent<Animator>();
        _capsuleCollider2D = GetComponent<CapsuleCollider2D>();
    }

    private void Update() {
        if (Input.GetMouseButtonDown(0)){
            StartCoroutine(ShowAndHide());
            animator.SetTrigger(ATTACK);
        }
        Vector2 InputVector = GameInput.Instance.GetMovementVector();
        InputVector = InputVector.normalized;

        if (InputVector != Vector2.zero && Time.timeScale!=0) {
            animator.SetFloat("moveX",InputVector.x);
            animator.SetFloat("moveY",InputVector.y);
        }
    }
    IEnumerator ShowAndHide() {
        GameObject.Find("SwordVisual").GetComponent<Renderer>().enabled = true;
        GameObject.Find("SwordVisual").GetComponent<CapsuleCollider2D>().enabled = true;
        yield return new WaitForSeconds(0.3f);
        GameObject.Find("SwordVisual").GetComponent<Renderer>().enabled = false;
        GameObject.Find("SwordVisual").GetComponent<CapsuleCollider2D>().enabled = false;
    }
    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.transform.TryGetComponent(out EnemyEntity enemyEntity)) {
            enemyEntity.TakeDamage(_damageAmount);
        }
    }    
    
    
   }
