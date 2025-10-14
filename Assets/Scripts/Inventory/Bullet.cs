using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 15f;
    public int damage = 20;
    public float lifeTime = 2f;
    private Vector2 direction;

    public void SetDirection(Vector2 dir)
    {
        direction = dir.normalized;
        Destroy(gameObject, lifeTime); // destruir después de un tiempo
    }

    void Update()
    {
        transform.Translate(direction * speed * Time.deltaTime, Space.World);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        EnemyStats enemy = collision.GetComponent<EnemyStats>();
        if (enemy != null && enemy.IsAlive())
        {
            enemy.TakeDamage(damage);
            Debug.Log($"Bala impactó a {enemy.name} por {damage} de daño");
            Destroy(gameObject);
        }
    }
}
