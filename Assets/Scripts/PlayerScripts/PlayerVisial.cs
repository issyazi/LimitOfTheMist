using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerVisial : MonoBehaviour
{
    private Animator animator;
    private const string IS_MOVING = "Moving";
    private const string ATTACK = "Attack";
    private bool isMove;

    private void Awake() {
        animator = GetComponent<Animator>();
    }
  
    private void Update() {
        isMove = PlayerController.Instance.Moving();
        animator.SetBool(IS_MOVING, isMove);
        if (Input.GetMouseButtonDown(0)){
            animator.SetTrigger(ATTACK);
            
        }

        Vector2 InputVector = GameInput.Instance.GetMovementVector();
        InputVector = InputVector.normalized;

        if (InputVector != Vector2.zero && Time.timeScale!=0) {
            animator.SetFloat("moveX",InputVector.x);
            animator.SetFloat("moveY",InputVector.y);
        }
        
   }
}

