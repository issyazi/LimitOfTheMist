using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public static PlayerController Instance { get; private set; }
    [SerializeField] private float moveSpeed = 5f;
    
    private Rigidbody2D rb;
    private float minMoveSpeed = 0.1f;
    private bool isMoving = false;
    private Vector2 InputVector;
    private AudioSource runAudioSource; // Добавляем AudioSource для звука бега

    private void Awake()
    {
        Instance = this;
        rb = GetComponent<Rigidbody2D>();
        runAudioSource = GetComponent<AudioSource>(); // Получаем AudioSource
        if (runAudioSource == null)
        {
            Debug.LogError("AudioSource component not found on Player!");
        }
    }

    private void Start()
    {
        GameInput.Instance.OnPlayerAttack += Player_OnPlayerAttack;
    }

    private void Player_OnPlayerAttack(object sender, System.EventArgs e)
    {
        ActiveWeapon.Instance.GetActiveWeapon().Attack();
    }

    private void Update()
    {
        InputVector = GameInput.Instance.GetMovementVector();

        // Управляем звуком бега
        if (isMoving && !runAudioSource.isPlaying)
        {
            runAudioSource.Play();
        }
        else if (!isMoving && runAudioSource.isPlaying)
        {
            runAudioSource.Stop();
        }
    }

    private void FixedUpdate()
    {
        HandleMovement();
    }

    private void HandleMovement()
    {
        rb.MovePosition(rb.position + InputVector * (moveSpeed * Time.fixedDeltaTime));

        if (Mathf.Abs(InputVector.x) > minMoveSpeed || Mathf.Abs(InputVector.y) > minMoveSpeed)
        {
            isMoving = true;
        }
        else
        {
            isMoving = false;
        }
    }

    public bool Moving()
    {
        return isMoving;
    }
}