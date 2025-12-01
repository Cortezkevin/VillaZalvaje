using UnityEngine;

public class Grenade : MonoBehaviour
{
    [Header("Explosion Config")]
    public float delay = 3f;             // Tiempo hasta la explosión
    public float radius = 3f;            // Radio del daño
    public int damage = 100;             // Daño total
    public LayerMask whatIsDestructible; // Capas que la explosión afectará (Enemigos, Cajas, etc.)

    [Header("Visuals/Effects")]
    public GameObject explosionEffectPrefab; // Efecto visual de la explosión (opcional)

    private float countdown;
    private bool exploded = false;

    void Start()
    {
        countdown = delay;
    }

    void Update()
    {
        countdown -= Time.deltaTime;
        if (countdown <= 0f && !exploded)
        {
            Explode();
            exploded = true;
        }
    }

    void Explode()
    {
        Debug.Log("¡BOOM! Granada explotó.");

        // Opcional: Instanciar efecto de explosión
        if (explosionEffectPrefab != null)
        {
            Instantiate(explosionEffectPrefab, transform.position, Quaternion.identity);
        }

        // 1. Detectar objetos dentro del radio
        Collider2D[] objectsToDamage = Physics2D.OverlapCircleAll(transform.position, radius, whatIsDestructible);

        foreach (Collider2D obj in objectsToDamage)
        {
            // 2. Aplicar lógica de daño a enemigos
            EnemyStats enemyStats = obj.GetComponent<EnemyStats>();
            if (enemyStats != null)
            {
                // Podrías aplicar daño total o daño que disminuye con la distancia (opcional avanzado)
                enemyStats.TakeDamage(damage);
                Debug.Log($"Granada hirió a {obj.name} por {damage} de daño.");
            }

            // Opcional: Aplicar fuerza de explosión (si tienes Rigidbody2D en los objetos)
            Rigidbody2D rb = obj.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                Vector2 direction = obj.transform.position - transform.position;
                rb.AddForce(direction.normalized * 5f, ForceMode2D.Impulse); // Fuerza de ejemplo
            }
        }

        // 3. Eliminar la granada de la escena
        Destroy(gameObject);
    }

    // Visualizar el radio de explosión en el editor
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}