using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 15f;
    public int damage = 20;
    public float lifetime = 2f;

    private Vector2 direction;

    public void SetDirection(Vector2 dir)
    {
        direction = dir.normalized;
        Destroy(gameObject, lifetime);
    }

    void Update()
    {
        transform.Translate(direction * speed * Time.deltaTime, Space.World);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        EnemyStats enemy = other.GetComponent<EnemyStats>();
        if (enemy != null)
        {
            enemy.TakeDamage(damage);
            Debug.Log($"🔫 Impacto a {other.name} por {damage} de daño.");
            Destroy(gameObject);
        }
    }
}
