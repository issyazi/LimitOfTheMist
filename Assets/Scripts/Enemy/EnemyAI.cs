using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using EternumUtils;
using UnityEngine.InputSystem.XR.Haptics;
using Unity.VisualScripting;
using System;
using UnityEngine.EventSystems;

public class EnemyAI : MonoBehaviour
{
    [SerializeField] private State _startingState;
    [SerializeField] private float _roamingDistanceMax = 7f;
    [SerializeField] private float _roamingDistanceMin = 3f;
    [SerializeField] private float _roamingTimerMax = 2f;
    [SerializeField] private bool _isChasingEnemy = false;
    [SerializeField] private bool _isAttackingEnemy = false;
    [SerializeField] private float _attackingDistance = 2f;
    [SerializeField] private float _chasingDistance = 4f;
    [SerializeField] private float _chasingSpeedMultiplier = 2f;
    [SerializeField] private float _attackRate = 2f;
    private float _nextAttackTime = 0f;
    private NavMeshAgent _navMeshAgent;
    private State _currentState;
    private float _roamingTimer;
    private Vector3 _roamPosition;
    private Vector3 _startingPosition;
    private float _roamingSpeed;
    private float _chasingSpeed;
    private float _nextCheckDirectionTime = 0f;
    private float _checkDirectionDuration = 0.1f;
    private Vector3 _lastPosition;
    public event EventHandler OnEnemyAttack;



    private enum State {
        Idle,
        Roaming,
        Chasing,
        Attacking,
        Death
    }

    public void SetDeathState() {
        _navMeshAgent.ResetPath();
        _currentState = State.Death;
    }

    private void Awake() {
        _navMeshAgent = GetComponent<NavMeshAgent>();
        _navMeshAgent.updateRotation = false;
        _navMeshAgent.updateUpAxis = false;
        _currentState = _startingState;
        _roamingSpeed = _navMeshAgent.speed;
        _chasingSpeed = _navMeshAgent.speed * _chasingSpeedMultiplier;
    }
    private void Update() {
        StateHandler ();
        MovementDirectHandler();
    }
    private void StateHandler () {
        switch (_currentState) {
            case State.Roaming:
                _roamingTimer -= Time.deltaTime;
                if (_roamingTimer < 0) {
                    Roaming();
                    _roamingTimer = _roamingTimerMax;
                }
                CheckCurrentState();
                break;
            case State.Chasing:
                ChasingTarget();
                CheckCurrentState();
                break;
            case State.Attacking:
                AttackingTarget();
                CheckCurrentState();
                break;
            case State.Death:
                break;
            default:
            case State.Idle:
                break;

        }
    }
    private void AttackingTarget() {
        if (Time.time > _nextAttackTime) {
            OnEnemyAttack?.Invoke(this, EventArgs.Empty);
            _nextAttackTime = Time.time + _attackRate;
        }
    }
    private void MovementDirectHandler(){
        if(Time.time > _nextCheckDirectionTime) {
            if (IsRunning) {
                ChangeFacingDir(_lastPosition, transform.position);
            } else if (_currentState == State.Attacking) {
                ChangeFacingDir(transform.position, PlayerController.Instance.transform.position);
            }
            _lastPosition = transform.position;
            _nextCheckDirectionTime = Time.time + _checkDirectionDuration;
        }
    }
    private void ChasingTarget() {
        _navMeshAgent.SetDestination(PlayerController.Instance.transform.position);
    }
    private State CheckCurrentState() {
        float distanceToPlayer = Vector3.Distance(transform.position, PlayerController.Instance.transform.position);
        State newState = State.Roaming;

        if (_isChasingEnemy) {
            if (distanceToPlayer <= _chasingDistance) {
                newState = State.Chasing;
            }
        }
        if (_isAttackingEnemy) {
            if (distanceToPlayer <= _attackingDistance){
                newState = State.Attacking;
            }
            
        }
        if (newState != _currentState) {
            if (newState == State.Chasing) {
                _navMeshAgent.ResetPath();
                _navMeshAgent.speed = _chasingSpeed;

            } else if (newState == State.Roaming) {
                _navMeshAgent.speed = _roamingSpeed;
                _roamingTimer = 0f;

            } else if (newState == State.Attacking) {
                _navMeshAgent.ResetPath();
            }
            _currentState = newState;
        }

        return _startingState;
    }
    public bool IsRunning {
        get {
            if(_navMeshAgent.velocity == Vector3.zero) {
                return false;
            } else {
                return true;
            }
        }
    }
    public float GetRoamingAnimationSpeed() {
        return _navMeshAgent.speed/ _roamingSpeed;
    }
    private void Roaming() {
        _startingPosition = transform.position;
        _roamPosition = GetRoamingPosition();
        _navMeshAgent.SetDestination(_roamPosition);
    }
    private Vector3 GetRoamingPosition() {
        return _startingPosition + Utils.GetRandomDir() * UnityEngine.Random.Range(_roamingDistanceMin, _roamingDistanceMax);
    }
    private void ChangeFacingDir(Vector3 sourcePosition, Vector3 targerPos){
        if(sourcePosition.x > targerPos.x) {
            transform.rotation = Quaternion.Euler(0,0,0);
        } 
        else {
            transform.rotation = Quaternion.Euler(0,180,0);
        }
    }


}
