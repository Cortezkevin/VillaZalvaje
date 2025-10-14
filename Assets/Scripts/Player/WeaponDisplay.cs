using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class WeaponDisplay : MonoBehaviour
{
    [Header("Weapon Holder")]
    public SpriteRenderer weaponRenderer;
    public Transform weaponHolder;

    [Header("Weapon Sprites")]
    public Sprite knifeSprite;
    public Sprite gunSprite;
    public Sprite grenadeSprite;
    public Sprite shotgunSprite;
    public Sprite cokeSprite;

    [Header("Rotation Settings")]
    public bool enableRotation = true;
    public float rotationOffset = 0f;

    [Header("Rotation Limits")]
    [Range(0f, 90f)]
    public float maxUpwardAngle = 45f;
    [Range(0f, 90f)]
    public float maxDownwardAngle = 45f;

    [Header("Animation Settings - Knife")]
    public float knifeSlashAngle = 90f;
    public float knifeSlashSpeed = 15f;

    [Header("Knife Combat")] // ⚔️ NUEVO
    public float knifeRange = 2f;
    public int knifeDamage = 25;
    public float knifeAngle = 45f;

    [Header("Gun Settings")]
    public GameObject bulletPrefab;
    public Transform firePoint; 
    public float fireRate = 0.3f;
    private float lastFireTime;

    [Header("Animation Settings - Gun")]
    public float gunRecoilDistance = 0.2f;
    public float gunRecoilSpeed = 20f;

    [Header("Animation Settings - Throw")]
    public float throwDistance = 1f;
    public float throwSpeed = 8f;

    private Vector3 originalLocalPosition;
    private bool isAnimating = false;
    private float currentMouseAngle = 0f;
    private bool isFacingLeft = false;

    void Start()
    {
        if (weaponHolder != null)
        {
            originalLocalPosition = weaponHolder.localPosition;
        }
    }

    void Update()
    {
        UpdateWeaponDisplay();

        if (enableRotation && weaponHolder != null)
        {
            RotateWeaponTowardsMouse();
        }

        if (Mouse.current.leftButton.wasPressedThisFrame && !isAnimating)
        {
            UseWeapon();
        }
    }

    private void UpdateWeaponDisplay()
    {
        if (weaponRenderer == null)
        {
            Debug.LogWarning("Weapon Renderer no está asignado!");
            return;
        }

        ItemData selectedItem = InventoryManager.Instance?.GetSelectedItem();

        if (selectedItem == null)
        {
            weaponRenderer.sprite = null;
            weaponRenderer.enabled = false;
            return;
        }

        weaponRenderer.enabled = true;

        switch (selectedItem.itemName)
        {
            case "Knife":
                weaponRenderer.sprite = knifeSprite;
                break;
            case "Gun":
                weaponRenderer.sprite = gunSprite;
                break;
            case "Grenade":
                weaponRenderer.sprite = grenadeSprite;
                break;
            case "Shotgun":
                weaponRenderer.sprite = shotgunSprite;
                break;
            case "Coke":
                weaponRenderer.sprite = cokeSprite;
                break;
            default:
                weaponRenderer.sprite = selectedItem.itemIcon;
                break;
        }
    }

    private void RotateWeaponTowardsMouse()
    {
        Vector3 mousePos = Mouse.current.position.ReadValue();
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(mousePos);
        mouseWorldPos.z = 0f;

        Vector2 direction = (mouseWorldPos - weaponHolder.position).normalized;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        bool isPointingLeft = (angle > 90f || angle < -90f);
        isFacingLeft = isPointingLeft;

        if (isPointingLeft)
        {
            float normalizedAngle = angle;
            if (angle < 0)
            {
                normalizedAngle = 180f + (180f + angle);
            }
            angle = Mathf.Clamp(normalizedAngle, 180f - maxDownwardAngle, 180f + maxUpwardAngle);
        }
        else
        {
            angle = Mathf.Clamp(angle, -maxDownwardAngle, maxUpwardAngle);
        }

        currentMouseAngle = angle;

        if (!isAnimating)
        {
            weaponHolder.rotation = Quaternion.Euler(0f, 0f, angle + rotationOffset);
        }

        if (weaponRenderer != null)
        {
            weaponRenderer.flipY = isPointingLeft;
        }
    }

    private void UseWeapon()
    {
        ItemData selectedItem = InventoryManager.Instance?.GetSelectedItem();

        if (selectedItem == null)
        {
            Debug.Log("No hay arma equipada");
            return;
        }

        Debug.Log("Usando: " + selectedItem.itemName);

        switch (selectedItem.itemName)
        {
            case "Knife":
                StartCoroutine(KnifeSlashAnimation());
                break;
            case "Gun":
                if (Time.time - lastFireTime >= fireRate)
                {
                    StartCoroutine(GunRecoilAnimation());
                    FireBullet();
                    lastFireTime = Time.time;
                }
                break;
            case "Shotgun":
                StartCoroutine(GunRecoilAnimation());
                break;
            case "Grenade":
                StartCoroutine(ThrowAnimation());
                break;
            case "Coke":
                StartCoroutine(DrinkAnimation());
                break;
            default:
                Debug.Log("Este item no tiene animación");
                break;
        }
    }

    private void FireBullet()
    {
        if (bulletPrefab == null || firePoint == null)
        {
            Debug.LogWarning("BulletPrefab o FirePoint no asignado en el inspector!");
            return;
        }

        // --- Calcular posición del mouse en world (robusto y siempre correcto) ---
        Vector3 mousePos = Mouse.current.position.ReadValue();
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(mousePos);
        mouseWorldPos.z = 0f;

        // Dirección desde el firePoint hacia el cursor
        Vector2 direction = (mouseWorldPos - firePoint.position).normalized;

        // Instanciar la bala en el firePoint
        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);

        // Ajustar rotación de la bala para que apunte en la dirección correcta
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        bullet.transform.rotation = Quaternion.Euler(0f, 0f, angle);

        // Enviar dirección al script de la bala (si existe)
        Bullet bulletScript = bullet.GetComponent<Bullet>();
        if (bulletScript != null)
        {
            bulletScript.SetDirection(direction);
        }

        Debug.Log("Disparo ejecutado hacia: " + direction);
    }



    // ANIMACIÓN: Cortar con cuchillo (ACTUALIZADA CON DAÑO)
    private IEnumerator KnifeSlashAnimation()
    {
        isAnimating = true;

        float startAngle = currentMouseAngle + rotationOffset;
        float slashDirection = isFacingLeft ? 1f : -1f;
        float slashAngle = startAngle + (knifeSlashAngle * slashDirection);

        float elapsedTime = 0f;
        float duration = 1f / knifeSlashSpeed;

        // Fase 1: Bajar el cuchillo (cortar)
        while (elapsedTime < duration)
        {
            float currentAngle = Mathf.Lerp(startAngle, slashAngle, elapsedTime / duration);
            weaponHolder.rotation = Quaternion.Euler(0f, 0f, currentAngle);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        weaponHolder.rotation = Quaternion.Euler(0f, 0f, slashAngle);

        // ⚔️ NUEVO: Detectar y hacer daño a enemigos
        DetectAndDamageEnemies();

        yield return new WaitForSeconds(0.05f);

        // Fase 2: Subir el cuchillo (volver a posición original)
        elapsedTime = 0f;
        while (elapsedTime < duration)
        {
            float currentAngle = Mathf.Lerp(slashAngle, startAngle, elapsedTime / duration);
            weaponHolder.rotation = Quaternion.Euler(0f, 0f, currentAngle);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        weaponHolder.rotation = Quaternion.Euler(0f, 0f, startAngle);
        isAnimating = false;
    }

    // ⚔️ NUEVO: Método para detectar y dañar enemigos
    private void DetectAndDamageEnemies()
    {
        // Detectar todos los colliders en rango
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, knifeRange);

        foreach (Collider2D hit in hits)
        {
            // Verificar si tiene EnemyStats
            EnemyStats enemyStats = hit.GetComponent<EnemyStats>();

            if (enemyStats != null && enemyStats.IsAlive())
            {
                // Verificar si el enemigo está en la dirección del ataque
                Vector2 directionToEnemy = (hit.transform.position - transform.position).normalized;
                Vector2 attackDirection = weaponHolder.right.normalized;

                float angle = Vector2.Angle(attackDirection, directionToEnemy);

                // Si el enemigo está en el ángulo de ataque
                if (angle < knifeAngle)
                {
                    enemyStats.TakeDamage(knifeDamage);
                    Debug.Log("¡Golpeaste a " + hit.gameObject.name + " por " + knifeDamage + " de daño!");
                }
            }
        }
    }

    // ANIMACIÓN: Retroceso de arma de fuego
    private IEnumerator GunRecoilAnimation()
    {
        isAnimating = true;

        Vector3 startPos = weaponHolder.localPosition;
        Vector3 recoilDirection = -weaponHolder.right;
        Vector3 recoilPos = startPos + recoilDirection * gunRecoilDistance;

        float elapsedTime = 0f;
        float duration = 1f / gunRecoilSpeed;

        while (elapsedTime < duration)
        {
            weaponHolder.localPosition = Vector3.Lerp(startPos, recoilPos, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        weaponHolder.localPosition = recoilPos;
        yield return new WaitForSeconds(0.05f);

        elapsedTime = 0f;
        while (elapsedTime < duration * 1.5f)
        {
            weaponHolder.localPosition = Vector3.Lerp(recoilPos, startPos, elapsedTime / (duration * 1.5f));
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        weaponHolder.localPosition = startPos;
        isAnimating = false;
    }

    // ANIMACIÓN: Lanzar granada
    private IEnumerator ThrowAnimation()
    {
        isAnimating = true;

        Vector3 startPos = weaponHolder.localPosition;
        Vector3 backPos = startPos + Vector3.left * 0.3f + Vector3.down * 0.2f;

        float elapsedTime = 0f;
        float duration = 1f / throwSpeed;

        while (elapsedTime < duration)
        {
            weaponHolder.localPosition = Vector3.Lerp(startPos, backPos, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        Vector3 throwPos = startPos + weaponHolder.right * throwDistance;
        elapsedTime = 0f;

        while (elapsedTime < duration * 0.5f)
        {
            weaponHolder.localPosition = Vector3.Lerp(backPos, throwPos, elapsedTime / (duration * 0.5f));
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        weaponHolder.localPosition = startPos;
        isAnimating = false;
    }

    // ANIMACIÓN: Beber Coke
    private IEnumerator DrinkAnimation()
    {
        isAnimating = true;

        Vector3 startPos = weaponHolder.localPosition;
        Vector3 drinkPos = startPos + Vector3.up * 0.3f;

        float elapsedTime = 0f;
        float duration = 0.3f;

        while (elapsedTime < duration)
        {
            weaponHolder.localPosition = Vector3.Lerp(startPos, drinkPos, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        yield return new WaitForSeconds(0.5f);

        elapsedTime = 0f;
        while (elapsedTime < duration)
        {
            weaponHolder.localPosition = Vector3.Lerp(drinkPos, startPos, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        weaponHolder.localPosition = startPos;
        isAnimating = false;
    }

    public void FlipWeapon(bool facingLeft)
    {
        if (weaponRenderer != null)
        {
            weaponRenderer.flipX = facingLeft;
        }
    }

    // ⚔️ NUEVO: Visualizar rango de ataque en el Editor
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, knifeRange);
    }
}