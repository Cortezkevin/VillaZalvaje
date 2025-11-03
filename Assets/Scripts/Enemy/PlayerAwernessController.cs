using UnityEngine;

public class PlayerAwernessController : MonoBehaviour
{
    public bool AwareOfPlayer { get; private set; }
    public Vector2 DirectionToPlayer { get; private set; }

    [SerializeField]
    private float playerAwarenessDistance;

    private Transform player;
    private bool wasAwareLastFrame = false;

    private void Awake()
    {
        player = FindFirstObjectByType<PlayerMovement>().transform;
    }

    void Update()
    {
        Vector2 enemyToPlayerVector = player.position - transform.position;
        DirectionToPlayer = enemyToPlayerVector.normalized;

        if (enemyToPlayerVector.magnitude <= playerAwarenessDistance)
        {
            AwareOfPlayer = true;

            // Si acaba de detectar al jugador
            if (!wasAwareLastFrame)
            {
                OnPlayerDetected();
            }
        }
        else
        {
            AwareOfPlayer = false;

            // Si acaba de perder al jugador
            if (wasAwareLastFrame)
            {
                OnPlayerLost();
            }
        }

        wasAwareLastFrame = AwareOfPlayer;
    }

    private void OnPlayerDetected()
    {
        if (SoundtrackController.Instance != null)
        {
            SoundtrackController.Instance.RegisterEnemyAwareness();
        }
    }

    private void OnPlayerLost()
    {
        if (SoundtrackController.Instance != null)
        {
            SoundtrackController.Instance.UnregisterEnemyAwareness();
        }
    }

    // Limpieza al destruir el enemigo
    private void OnDestroy()
    {
        if (AwareOfPlayer && SoundtrackController.Instance != null)
        {
            SoundtrackController.Instance.UnregisterEnemyAwareness();
        }
    }
}