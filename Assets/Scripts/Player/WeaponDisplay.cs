using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class WeaponDisplay : MonoBehaviour
{
    public static WeaponDisplay Instance;

    [Header("Weapon Holder")]
    public SpriteRenderer weaponRenderer;
    public Transform weaponHolder;

    public ItemData selectedItem;

    private AmmoDisplay ammoDisplay;

    private PlayerStats playerStats;

    [Header("Weapon Sprites")]
    public Sprite knifeSprite;
    public Sprite gunSprite;
    public Sprite grenadeSprite;
    public Sprite shotgunSprite;
    public Sprite cokeSprite;
    public Sprite firstkitSprite;
    public Sprite medicineSprite;

    // --- NUEVO: Configuración de Audio ---
    [Header("Audio Settings")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip gunShotClip;
    [SerializeField] private AudioClip knifeSlashClip;
    // ------------------------------------

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

    [Header("Sincronización de Munición (Lectura UI)")]
    public int displayCurrentAmmo; // 🟢 NUEVO: Valor actual sincronizado para AmmoDisplay
    public int displayMaxAmmo;     // 🟢 NUEVO: Valor máximo sincronizado para AmmoDisplay

    [Header("Gun Settings")]
    public GameObject bulletPrefab;
    public Transform firePoint;
    public float fireRate = 0.3f;
    private float lastFireTime;
    public int gunMaxAmmo = 7;       // ⬅️ Renombrado
    public int gunCurrentAmmo;     // ⬅️ Renombrado
    public float reloadTime = 1.5f;
    private bool isReloading = false;

    [Header("Shotgun Settings")]
    public int maxShotgunAmmo = 4;        // Máximo de disparos (cartuchos) por cargador
    public int currentShotgunAmmo;
    public int pelletsPerShot = 4;        // Número de "perdigones" por cartucho
    public float shotgunFireRate = 1.2f;  // Cadencia más lenta
    public float shotgunPelletDamage = 8; // Menos daño individual por perdigón
    public float shotgunSpreadAngle = 15f;// Ángulo de dispersión para los perdigones
    private float lastShotgunFireTime;

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
            originalLocalPosition = weaponHolder.localPosition;

        gunCurrentAmmo = gunMaxAmmo;
        currentShotgunAmmo = maxShotgunAmmo;
        ammoDisplay = FindAnyObjectByType<AmmoDisplay>();
        ammoDisplay?.UpdateAmmoUI(); // Muestra balas al inicio

        // NUEVO: Obtener AudioSource si no está asignado
        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
        }

        // AÑADIDO: Obtener la instancia de PlayerStats
        playerStats = PlayerStats.Instance;
        if (playerStats == null)
        {
            Debug.LogError("PlayerStats no encontrado. Asegúrate de que existe una instancia en la escena.");
        }
    }

    void Awake()
    {
        // 🟢 NUEVO: Implementación del Singleton
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
            return;
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

        if (Keyboard.current.rKey.wasPressedThisFrame && !isReloading)
        {
            StartCoroutine(ReloadGun());
        }
        if (Keyboard.current.eKey.wasPressedThisFrame && !isAnimating && selectedItem != null)
        {
            UseHealingItem();
        }

    }

    private void UseHealingItem()
    {
        if (playerStats == null || InventoryManager.Instance == null) return;

        ItemData item = InventoryManager.Instance.GetSelectedItem();

        // 1. Verificar si hay un item seleccionado.
        if (item == null) return;

        // 2. Obtener el índice del slot seleccionado.
        int selectedSlotIndex = InventoryManager.Instance.GetSelectedSlot();
        if (selectedSlotIndex == -1) return; // Por seguridad

        int healAmount = 0;

        switch (item.itemName)
        {
            case "Firstkit": // Curación de 70 puntos
                healAmount = 70;
                break;
            case "Medicine": // Curación de 30 puntos
                healAmount = 30;
                break;
            default:
                // Si el item seleccionado no es de curación, no hace nada
                Debug.Log($"El item {item.itemName} no es un item de curación.");
                return;
        }

        // 3. Verificar si el jugador necesita curarse
        if (playerStats.GetCurrentHealth() < playerStats.GetMaxHealth())
        {
            // Intentar curar y pasar el índice del slot para que PlayerStats lo consuma
            playerStats.Heal(healAmount, selectedSlotIndex);
        }
        else
        {
            Debug.Log("La vida está al máximo. No se necesita curación.");
        }
    }

    public Sprite GetItemSprite(ItemData itemData)
    {
        if (itemData == null) return null;

        switch (itemData.itemName)
        {
            case "Knife":
                return knifeSprite;
            case "Gun":
                return gunSprite;
            case "Grenade":
                return grenadeSprite;
            case "Shotgun":
                return shotgunSprite;
            case "Coke":
                return cokeSprite;
            case "Firstkit":
                return firstkitSprite;
            case "Medicine":
                return medicineSprite;
            default:
                // Si el nombre no coincide con ninguna variable serializada, usa el ícono del ItemData.
                return itemData.itemIcon;
        }
    }

    private void UpdateWeaponDisplay()
    {
        if (weaponRenderer == null)
        {
            Debug.LogWarning("Weapon Renderer no está asignado!");
            return;
        }

        selectedItem = InventoryManager.Instance?.GetSelectedItem();

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
                // 🟢 SINCRONIZACIÓN CLAVE: Pistola
                displayCurrentAmmo = gunCurrentAmmo;
                displayMaxAmmo = gunMaxAmmo;
                break;

            case "Shotgun":
                weaponRenderer.sprite = shotgunSprite;
                // 🟢 SINCRONIZACIÓN CLAVE: Escopeta
                displayCurrentAmmo = currentShotgunAmmo;
                displayMaxAmmo = maxShotgunAmmo;
                break;
            case "Coke":
                weaponRenderer.sprite = cokeSprite;
                break;
            case "Firstkit":
                weaponRenderer.sprite = firstkitSprite;
                break;
            case "Medicine":
                weaponRenderer.sprite = medicineSprite;
                break;
            default:
                weaponRenderer.sprite = selectedItem.itemIcon;
                displayCurrentAmmo = 0;
                displayMaxAmmo = 0;
                break;
        }
        ammoDisplay?.UpdateAmmoUI();
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
                if (isReloading)
                    return;

                if (gunCurrentAmmo <= 0) // ⬅️ Usar gunCurrentAmmo
                {
                    Debug.Log("¡Sin balas! Recarga con 'R'");
                    return;
                }

                if (Time.time - lastFireTime >= fireRate)
                {
                    StartCoroutine(GunRecoilAnimation());
                    FireBullet();

                    gunCurrentAmmo--; // ⬅️ Decrementar gunCurrentAmmo
                    displayCurrentAmmo = gunCurrentAmmo; // 🟢 Sincronizar inmediatamente

                    ammoDisplay?.UpdateAmmoUI();
                    lastFireTime = Time.time;
                }
                break;
            case "Shotgun":
                if (isReloading)
                    return;

                if (currentShotgunAmmo <= 0) // Usamos la munición de escopeta
                {
                    Debug.Log("¡Escopeta sin cartuchos! Recarga con 'R'");
                    return;
                }

                if (Time.time - lastShotgunFireTime >= shotgunFireRate)
                {
                    StartCoroutine(GunRecoilAnimation());
                    FireShotgun();

                    currentShotgunAmmo--;

                    displayCurrentAmmo = currentShotgunAmmo;

                    ammoDisplay?.UpdateAmmoUI();
                    Debug.Log("Cartuchos restantes: " + currentShotgunAmmo);
                    lastShotgunFireTime = Time.time;
                }
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

    private void FireShotgun()
    {
        if (bulletPrefab == null || firePoint == null)
        {
            Debug.LogWarning("BulletPrefab o FirePoint no asignado para Shotgun!");
            return;
        }

        // Reproducir sonido
        if (audioSource != null && gunShotClip != null)
        {
            audioSource.PlayOneShot(gunShotClip);
        }

        // Obtener la posición del mouse
        Vector3 mousePos = Mouse.current.position.ReadValue();
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(mousePos);
        mouseWorldPos.z = 0f;

        // 🟢 VARIABLES LOCALES DEFINIDAS CORRECTAMENTE AQUÍ
        Vector2 centerDirection = (mouseWorldPos - firePoint.position).normalized;
        float centerAngle = Mathf.Atan2(centerDirection.y, centerDirection.x) * Mathf.Rad2Deg;

        // Bucle para disparar múltiples perdigones
        for (int i = 0; i < pelletsPerShot; i++)
        {
            float randomSpread = Random.Range(-shotgunSpreadAngle / 2f, shotgunSpreadAngle / 2f);
            float finalAngle = centerAngle + randomSpread;

            Vector2 pelletDirection = new Vector2(
                Mathf.Cos(finalAngle * Mathf.Deg2Rad),
                Mathf.Sin(finalAngle * Mathf.Deg2Rad)
            ).normalized;

            GameObject pellet = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);

            // ... (Ajustar rotación visual del perdigón)

            // Enviar la dirección y el daño al script de la bala
            Bullet bulletScript = pellet.GetComponent<Bullet>();
            if (bulletScript != null)
            {
                // bulletScript.SetDamage(shotgunPelletDamage); // Usar si tienes SetDamage
                bulletScript.SetDirection(pelletDirection);
            }
        }
        Debug.Log($"Shotgun disparó {pelletsPerShot} perdigones.");
    }

    private void FireBullet()
    {
        if (bulletPrefab == null || firePoint == null)
        {
            Debug.LogWarning("BulletPrefab o FirePoint no asignado en el inspector!");
            return;
        }

        // --- NUEVO: Reproducir sonido de disparo ---
        if (audioSource != null && gunShotClip != null)
        {
            audioSource.PlayOneShot(gunShotClip);
        }
        else
        {
            Debug.LogWarning("AudioSource o GunShotClip no asignado para disparo.");
        }
        // ------------------------------------------

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
        // [CÓDIGO DE BALA EXISTENTE]
        Bullet bulletScript = bullet.GetComponent<Bullet>();
        if (bulletScript != null)
        {
            bulletScript.SetDirection(direction);
        }

        Debug.Log("Disparo ejecutado hacia: " + direction);
    }


    private IEnumerator ReloadGun()
    {
        if (selectedItem == null)
        {
            Debug.Log("Nada equipado para recargar.");
            yield break;
        }

        bool isShotgun = (selectedItem.itemName == "Shotgun");

        // ❌ ERROR CORREGIDO: Usamos variables locales para la comprobación y luego actualizamos las variables reales.

        int currentWeaponAmmo = isShotgun ? currentShotgunAmmo : gunCurrentAmmo; // ⬅️ Usar gunCurrentAmmo
        int maxWeaponAmmo = isShotgun ? maxShotgunAmmo : gunMaxAmmo;           // ⬅️ Usar gunMaxAmmo

        if (currentWeaponAmmo == maxWeaponAmmo)
        {
            Debug.Log("Cargador lleno");
            yield break;
        }

        isReloading = true;
        Debug.Log($"Recargando {(isShotgun ? "Escopeta" : "Pistola")}...");

        yield return new WaitForSeconds(reloadTime);

        // 🟢 CLAVE: Actualizamos las variables correctas
        if (isShotgun)
        {
            currentShotgunAmmo = maxShotgunAmmo;
            displayCurrentAmmo = maxShotgunAmmo;
            displayMaxAmmo = maxShotgunAmmo;
        }
        else // Pistola
        {
            gunCurrentAmmo = gunMaxAmmo;
            displayCurrentAmmo = gunMaxAmmo;
            displayMaxAmmo = gunMaxAmmo;
        }

        ammoDisplay?.UpdateAmmoUI();
        isReloading = false;
        Debug.Log("Recarga completa");
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, knifeRange);
    }

    // ANIMACIÓN: Cortar con cuchillo (ACTUALIZADA CON DAÑO)
    private IEnumerator KnifeSlashAnimation()
    {
        isAnimating = true;

        // --- NUEVO: Reproducir sonido de cuchillo al inicio del slash ---
        if (audioSource != null && knifeSlashClip != null)
        {
            audioSource.PlayOneShot(knifeSlashClip);
        }
        // -----------------------------------------------------------------

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

}