using System.Runtime.CompilerServices;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    [SerializeField]
    private float speed;

    [SerializeField]
    private float rotationSpeed;

    // EnemyMovement.cs

    [Header("Steering & Obstacle Avoidance")]
    [SerializeField] private LayerMask obstacleLayer; // Capa de los objetos a esquivar (ej: Paredes)
    [SerializeField] private float raycastDistance = 1.0f; // Distancia para chequear obstáculos delante
    [SerializeField] private float avoidForce = 5f; // Fuerza de desvío aplicada al vector de movimiento
    private Vector2 avoidanceDirection = Vector2.zero;

    [Header("Combat Settings")]
    [SerializeField]
    private int damageAmount = 10; // Daño que hace al jugador

    [SerializeField]
    private float damageInterval = 1f; // Cada cuánto puede hacer daño (en segundos)

    private float lastDamageTime = 0f;

    // --- Audio de Pasos ---
    [Header("Audio Settings")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip footstepClip;
    [SerializeField] private float stepInterval = 0.8f; // Intervalo de tiempo entre sonidos de pasos
    private float stepTimer = 0f;
    // ----------------------

    private Rigidbody2D rigidbody;
    private PlayerAwernessController playerAwernessController;
    private Vector2 targetDireccion;

    private void Awake()
    {
        rigidbody = GetComponent<Rigidbody2D>();
        playerAwernessController = GetComponent<PlayerAwernessController>();

        // Obtener AudioSource si no está asignado
        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
        }
    }

    private void FixedUpdate()
    {
        UpdateTargetDirection();
        SetVelocity();
        // RotateTowardsTarget(); 
    }

    private void Update()
    {
        // Llamamos al manejo de audio en Update para usar Time.deltaTime
        HandleFootsteps();
    }

    private void UpdateTargetDirection()
    {
        if (playerAwernessController.AwareOfPlayer)
        {
            // 1. Dirección base: Hacia el jugador
            Vector2 directionToPlayer = playerAwernessController.DirectionToPlayer;

            // 2. Ejecutar Raycast para detectar obstáculos
            RaycastHit2D hit = Physics2D.Raycast(transform.position, directionToPlayer, raycastDistance, obstacleLayer);

            // 3. Determinar el vector de evasión
            if (hit.collider != null)
            {
                // Hay un obstáculo delante.
                Debug.DrawRay(transform.position, directionToPlayer * raycastDistance, Color.red);

                // Calculamos una dirección perpendicular (para rodear el objeto)
                // Se puede hacer de forma simple (rotando 90 grados) o buscando un lado libre.

                // Método simple (rotación 90 grados):
                Vector2 perpendicular = new Vector2(-directionToPlayer.y, directionToPlayer.x);

                // Intentaremos determinar si girar a la izquierda o derecha es mejor.
                // Para simplificar, elegiremos una dirección fija o aleatoria para empezar a girar.
                // Aquí se usaría lógica avanzada, pero para empezar: girar 90 grados.
                avoidanceDirection = perpendicular.normalized;

                // Si quieres que pruebe a qué lado es mejor girar, puedes usar:
                /*
                float angleToObstacle = Vector2.SignedAngle(directionToPlayer, hit.normal);
                avoidanceDirection = angleToObstacle > 0 ? perpendicular : -perpendicular;
                */
            }
            else
            {
                Debug.DrawRay(transform.position, directionToPlayer * raycastDistance, Color.green);
                avoidanceDirection = Vector2.zero;
            }

            // 4. Mezclar las direcciones (Steering)
            if (avoidanceDirection != Vector2.zero)
            {
                // Mezclamos la dirección al jugador con el vector de evasión
                targetDireccion = (directionToPlayer + avoidanceDirection * avoidForce).normalized;
            }
            else
            {
                // Si no hay obstáculo, se mueve directamente hacia el jugador
                targetDireccion = directionToPlayer;
            }
        }
        else
        {
            targetDireccion = Vector2.zero;
        }
    }

    private void RotateTowardsTarget()
    {
        if (targetDireccion == Vector2.zero)
        {
            return;
        }

        Quaternion targetRotation = Quaternion.LookRotation(transform.forward, targetDireccion);
        Quaternion rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        rigidbody.SetRotation(rotation);
    }

    private void SetVelocity()
    {
        if (targetDireccion == Vector2.zero)
        {
            // Frena suavemente
            rigidbody.linearVelocity = Vector2.Lerp(rigidbody.linearVelocity, Vector2.zero, 0.1f);
        }
        else
        {
            // Acelera hacia el objetivo
            Vector2 desiredVelocity = targetDireccion.normalized * speed;
            rigidbody.linearVelocity = Vector2.Lerp(rigidbody.linearVelocity, desiredVelocity, 0.1f);
        }
    }

    private void HandleFootsteps()
    {
        // El enemigo se mueve si tiene un objetivo (targetDireccion no es cero)
        bool isMoving = targetDireccion.sqrMagnitude > 0;

        if (isMoving && audioSource != null && footstepClip != null)
        {
            stepTimer -= Time.deltaTime;

            if (stepTimer <= 0)
            {
                // Reproducir sonido de paso
                audioSource.PlayOneShot(footstepClip);
                stepTimer = stepInterval;
            }
        }
        else
        {
            // Resetear el temporizador si no estamos moviéndonos
            stepTimer = 0;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            DamagePlayer();
        }
    }

    // Mantener daño mientras esté tocando al jugador
    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if (Time.time >= lastDamageTime + damageInterval)
            {
                DamagePlayer();
            }
        }
    }

    private void DamagePlayer()
    {
        if (PlayerStats.Instance != null)
        {
            PlayerStats.Instance.TakeDamage(damageAmount);
            lastDamageTime = Time.time; 
            Debug.Log("Zombie hizo " + damageAmount + " de daño al jugador!");
        }
    }
}