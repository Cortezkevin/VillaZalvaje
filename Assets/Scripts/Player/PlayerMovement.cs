using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 5f;

    [Header("Audio Settings")] // NUEVO
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip footstepClip;
    [SerializeField] private float stepInterval = 0.4f; // Tiempo entre pasos

    private Rigidbody2D rb;
    private Animator animator;
    private SpriteRenderer spriteRenderer;

    private Vector2 movement;
    private float stepTimer; // NUEVO: Contador para el intervalo de pasos

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        // Si el AudioSource no está asignado, intentar obtenerlo del mismo objeto
        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
        }
    }

    private void Update()
    {
        ReadInput();
        UpdateAnimations();
        AdjustPlayerFacingDirection();

        // NUEVO: Manejar la reproducción de sonido de pasos
        HandleFootsteps();
    }

    private void FixedUpdate()
    {
        Move();
    }

    private void ReadInput()
    {
        movement = Vector2.zero;

        if (Keyboard.current.wKey.isPressed || Keyboard.current.upArrowKey.isPressed)
            movement.y = 1f;
        if (Keyboard.current.sKey.isPressed || Keyboard.current.downArrowKey.isPressed)
            movement.y = -1f;
        if (Keyboard.current.aKey.isPressed || Keyboard.current.leftArrowKey.isPressed)
            movement.x = -1f;
        if (Keyboard.current.dKey.isPressed || Keyboard.current.rightArrowKey.isPressed)
            movement.x = 1f;

        movement = movement.normalized; // evita moverse más rápido en diagonal
    }

    private void Move()
    {
        rb.MovePosition(rb.position + movement * (moveSpeed * Time.fixedDeltaTime));
    }

    private void UpdateAnimations()
    {
        animator.SetFloat("moveX", movement.x);
        animator.SetFloat("moveY", movement.y);

        bool isMoving = movement.sqrMagnitude > 0;
        animator.SetBool("isMoving", isMoving);
    }

    private void AdjustPlayerFacingDirection()
    {
        Vector3 mousePos = Mouse.current.position.ReadValue();
        Vector3 playerScreenPoint = Camera.main.WorldToScreenPoint(transform.position);

        spriteRenderer.flipX = mousePos.x < playerScreenPoint.x;
    }

    // --- NUEVO MÉTODO PARA EL SONIDO DE PASOS ---
    private void HandleFootsteps()
    {
        bool isMoving = movement.sqrMagnitude > 0;

        if (isMoving && audioSource != null && footstepClip != null)
        {
            stepTimer -= Time.deltaTime;

            if (stepTimer <= 0)
            {
                // Reproducir el sonido
                audioSource.PlayOneShot(footstepClip);

                // Reiniciar el contador. Lo ajustamos a la mitad si usamos animación de 8 frames
                // pero si no, stepInterval es suficiente.
                stepTimer = stepInterval;
            }
        }
        else
        {
            // Resetear el temporizador si no estamos moviéndonos
            stepTimer = 0;
        }
    }
}