using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float acceleration = 15f; // suaviza el movimiento
    [SerializeField] private float deceleration = 15f; // freno natural

    private Rigidbody2D rb;
    private Vector2 moveInput;
    private Vector2 currentVelocity;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        // Leer entradas con el nuevo Input System
        moveInput = Vector2.zero;

        if (Keyboard.current.wKey.isPressed || Keyboard.current.upArrowKey.isPressed)
            moveInput.y = 1f;
        if (Keyboard.current.sKey.isPressed || Keyboard.current.downArrowKey.isPressed)
            moveInput.y = -1f;
        if (Keyboard.current.aKey.isPressed || Keyboard.current.leftArrowKey.isPressed)
            moveInput.x = -1f;
        if (Keyboard.current.dKey.isPressed || Keyboard.current.rightArrowKey.isPressed)
            moveInput.x = 1f;

        moveInput = moveInput.normalized; // evita moverse más rápido en diagonal
    }

    private void FixedUpdate()
    {
        MovePlayer();
    }

    private void MovePlayer()
    {
        // Movimiento suave con aceleración y desaceleración
        Vector2 targetVelocity = moveInput * moveSpeed;
        currentVelocity = rb.linearVelocity;

        if (moveInput.magnitude > 0.01f)
        {
            // Acelera hacia la dirección del input
            currentVelocity = Vector2.MoveTowards(currentVelocity, targetVelocity, acceleration * Time.fixedDeltaTime);
        }
        else
        {
            // Frena gradualmente cuando se suelta la tecla
            currentVelocity = Vector2.MoveTowards(currentVelocity, Vector2.zero, deceleration * Time.fixedDeltaTime);
        }

        rb.linearVelocity = currentVelocity;
    }
}
