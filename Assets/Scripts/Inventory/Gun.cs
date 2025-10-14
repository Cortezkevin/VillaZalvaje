using UnityEngine;
using UnityEngine.InputSystem;

public class Gun : MonoBehaviour
{
    [Header("Ajustes de Disparo")]
    public GameObject bulletPrefab; // Prefab de la bala
    public Transform firePoint;     // Punto desde donde sale la bala
    public float bulletSpeed = 10f; // Velocidad de la bala
    public float fireRate = 0.3f;   // Tiempo entre disparos
    private float nextFireTime = 0f;

    [Header("Sonido y Efectos (Opcional)")]
    public AudioSource shootSound;

    private void Update()
    {
        // Dispara si se presiona el click izquierdo y ha pasado el tiempo de enfriamiento
        if (Mouse.current.leftButton.wasPressedThisFrame && Time.time >= nextFireTime)
        {
            Shoot();
        }
    }

    private void Shoot()
    {
        // Crear la bala en el punto de disparo
        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);

        // Asignar velocidad a la bala
        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
        rb.linearVelocity = firePoint.right * bulletSpeed;

        // Reproducir sonido (si tiene)
        if (shootSound != null)
            shootSound.Play();

        // Registrar el siguiente tiempo de disparo
        nextFireTime = Time.time + fireRate;
    }
}
