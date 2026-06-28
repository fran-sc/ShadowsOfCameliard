using System.Collections;
using UnityEngine;

// -----------------------------------------------------------------------------
// ActiveWeapon
//
// Responsabilidades:
// - Gestiona el arma equipada actualmente por el jugador.
// - Instancia nuevas armas al recogerlas y configura sus parámetros.
// - Controla el disparo de proyectiles y el consumo de armas con recarga.
//
// Atributos principales:
// - currentWeapon: referencia al GameObject del arma activa.
// - attackSound: efecto de sonido del arma activa.
// - isFiring: flag para evitar disparos simultáneos.
//
// Notas:
// - Implementa IPlayerCollectObserver para reaccionar a items recogidos.
// - Las armas se instancian como hijos del jugador con el nombre del itemID.
// - FireProyectile espera a que termine la animación antes de disparar.
// -----------------------------------------------------------------------------
public class ActiveWeapon : MonoBehaviour, IPlayerCollectObserver
{
    GameObject currentWeapon;
    Animator anim;

    bool isFiring;
    AudioManager.Effect attackSound;

    void Awake()
    {
        anim = GetComponentInChildren<Animator>();
    }

    void Start()
    {
        // Nos registramos como observador del PlayerCollectSubject
        PlayerCollectSubject.Instance.AddObserver(this);
    }

    public GameObject GetCurrentWeapon()
    {
        return currentWeapon;
    }

    public bool hasWeapon(string weaponName)
    {
        return transform.Find(weaponName) != null;
    }

    // -----------------------------------------------------------------------------
    // SetActiveWeapon
    //
    // - Desactiva el arma anterior, activa la nueva por nombre y configura el
    //   Animator, el sonido de ataque y la compatibilidad con el escudo.
    // -----------------------------------------------------------------------------
    public void SetActiveWeapon(string weaponName)
    {
        // Si el jugador ya tiene un arma activa, la desactivamos antes de activar la nueva
        if (currentWeapon != null)
        {
            currentWeapon.SetActive(false);
        }       

        // Activamos el nuevo arma
        currentWeapon = transform.Find(weaponName)?.gameObject;

        if (currentWeapon == null)
        {
            Debug.LogError($"No se encontró el arma {weaponName} como hija del jugador.");
            return;
        }
        
        currentWeapon.SetActive(true);
        
        SpawnOnceWeaponSO weaponData = ResourcesManager.Instance.GetResource(currentWeapon.name) as SpawnOnceWeaponSO;
        if (weaponData == null)
        {
            Debug.LogError($"No se encontró SpawnOnceWeaponSO para el arma {weaponName}");
            return;
        }
        
        // Seteamos el parámetro del animator para reflejar el arma activa
        anim.SetFloat("item", weaponData.animatorItemID);

        // Seteamos el sonido de ataque del arma activa
        attackSound = weaponData.attackSound;

        // Actualizamos si esta arma permite usar escudo
        PlayerController.Instance.SetShieldCompatibility(
            weaponData.canUseShield,
            weaponData.blockMoveSpeedMultiplier
        );
    }

    // -----------------------------------------------------------------------------
    // FireWeapon
    //
    // - Reproduce el sonido de ataque y, si el arma es de disparo, inicia la
    //   corrutina FireProyectile.
    // - Ignorado si no hay arma activa o ya se está disparando.
    // -----------------------------------------------------------------------------
    public void FireWeapon()
    {
        if (currentWeapon == null)
        {
            return;
        }

        if (attackSound != AudioManager.Effect.None)
        {
            AudioManager.Instance.PlayEffect(attackSound);
        }

        SpawnOnceWeaponSO weaponData = ResourcesManager.Instance.GetResource(currentWeapon.name) as SpawnOnceWeaponSO;
        if (weaponData == null || !weaponData.isFiringWeapon)
        {
            return;
        }

        if (isFiring)
        {
            return; // Evitamos disparar de nuevo mientras ya estamos disparando
        }

        // Disparamos el proyectil
        StartCoroutine(FireProyectile(weaponData));
    }

    public void OnNotify(string itemID)
    {
        // Obtenemos el SpawnOnceWeaponSO asociado al objeto
        SpawnOnceSO data = ResourcesManager.Instance.GetResource(itemID) as SpawnOnceSO;
        if (data == null)
        {
            Debug.LogError($"No se encontró SpawnOnceWeaponSO para el item {itemID}");
            return;
        }

        // Si es un arma, la añadimos al inventario del jugador
        if (data.itemType == SpawnOnceSO.SpawnOnceType.Weapon)
        {
            SpawnOnceWeaponSO weaponData = data as SpawnOnceWeaponSO;

            // Si el jugador ya tiene el arma, no hacemos nada
            if (hasWeapon(weaponData.itemID))
            {
                Debug.Log($"El jugador ya tiene el arma {weaponData.itemID}. No se añade de nuevo.");
                return;
            }

            // Instanciamos el prefab del arma como hijo del jugador
            Transform playerTransform = PlayerController.Instance.transform;
            GameObject weapon = Instantiate(weaponData.weaponPrefab, playerTransform.position, Quaternion.identity, playerTransform);

            // Parametrizamos el arma 
            weapon.name = weaponData.itemID; // Asignamos el nombre del arma

            // Establecemos el daño del arma
            DamageSource damageSource = (weaponData.isFiringWeapon) ?
                ((SpawnOnceWeaponSO)weaponData).projectilePrefab.GetComponent<DamageSource>() :
                weapon.GetComponent<DamageSource>();

            if (damageSource != null)
            {
                damageSource.SetWeaponData(weaponData.damage, weaponData.pushForce);
            }
        }
    }

    // -----------------------------------------------------------------------------
    // FireProyectile
    //
    // - Espera la sincronización con la animación de ataque para instanciar el proyectil.
    // - Si el arma requiere recarga, la desactiva y notifica al InventoryManager.
    // -----------------------------------------------------------------------------
    IEnumerator FireProyectile(SpawnOnceWeaponSO weaponData)
    {
        isFiring = true;

        yield return new WaitForSeconds(0.1f); // Pequeña espera para sincronizar con la animación de disparo

        // Esperamos a que concluya la animación de disparo
        while (PlayerController.Instance.IsAttacking)
        {
            yield return null;
        }

        // Disparamos el arma instanciando el proyectil asociado
        GameObject projectile = Instantiate(weaponData.projectilePrefab, currentWeapon.transform.position, Quaternion.identity);
        projectile.transform.localScale = PlayerController.Instance.transform.localScale;

        // Desequipamos el arma si requiere recarga
        if (weaponData.requiresReload)
        {
            // Desactivamos el arma actual
            currentWeapon.SetActive(false); 
            currentWeapon = null;

            // Reseteamos el parámetro del animator
            anim.SetFloat("item", 0f);

            // Notificamos al InventoryManager para que desmarque el item seleccionado (si el arma se agota o requiere recarga)
            InventoryManager.Instance.UnselectCurrentItem();
        }

        isFiring = false;
    }
}
