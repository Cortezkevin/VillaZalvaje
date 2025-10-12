using System.Runtime.CompilerServices;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    [SerializeField]
    private float speed;

    [SerializeField]
    private float rotationSpeed;

    [Header("Combat Settings")]
    [SerializeField]
    private int damageAmount = 10; // Daño que hace al jugador

    [SerializeField]
    private float damageInterval = 1f; // Cada cuánto puede hacer daño (en segundos)

    private float lastDamageTime = 0f;

    private Rigidbody2D rigidbody;
    private PlayerAwernessController playerAwernessController;
    private Vector2 targetDireccion;

    private void Awake()
    {
        rigidbody = GetComponent<Rigidbody2D>();
        playerAwernessController = GetComponent<PlayerAwernessController>();
    }

    private void FixedUpdate()
    {
        UpdateTargetDirection();
        SetVelocity();
    }

    private void UpdateTargetDirection()
    {
        if (playerAwernessController.AwareOfPlayer)
        {
            targetDireccion = playerAwernessController.DirectionToPlayer;
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
            rigidbody.linearVelocity = Vector2.Lerp(rigidbody.linearVelocity, Vector2.zero, 0.1f);
        }
        else
        {
            Vector2 desiredVelocity = targetDireccion.normalized * speed;
            rigidbody.linearVelocity = Vector2.Lerp(rigidbody.linearVelocity, desiredVelocity, 0.1f);
        }
    }

    // Detectar colisión con el jugador
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            DamagePlayer();
        }
    }

    // Mantener daño mientras está tocando al jugador
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