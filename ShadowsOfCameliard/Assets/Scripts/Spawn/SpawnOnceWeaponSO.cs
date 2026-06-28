using UnityEngine;

// -----------------------------------------------------------------------------
// SpawnOnceWeaponSO
//
// Responsabilidades:
// - Extiende SpawnOnceSO con todas las propiedades específicas de un arma.
// - Define daño, visuals, configuración de disparo y compatibilidad con escudo.
//
// Atributos principales:
// - weaponPrefab: prefab instanciado como hijo del jugador al recoger el arma.
// - animatorItemID: valor del parámetro "item" del Animator al equipar el arma.
// - isFiringWeapon / requiresReload: control del comportamiento de disparo.
// - canUseShield: si es verdadero permite bloquear con esta arma equipada.
//
// Notas:
// - OnValidate() fuerza itemType = Weapon para evitar errores de configuración.
// -----------------------------------------------------------------------------
[CreateAssetMenu(fileName = "SpawnOnceWeaponSO", menuName = "Scriptable Objects/SpawnOnceWeaponSO")]
public class SpawnOnceWeaponSO : SpawnOnceSO
{
    [Header("Weapon Properties")]
    public int damage; // Daño provocado por el arma
    public float pushForce; // Fuerza de empuje al impactar con un enemigo

    [Header("Weapon Visuals")]
    public Sprite weaponSprite; // Sprite del arma para mostrar en el inventario o UI
    
    [Header("Player Weapon")]
    public string weaponName; // Nombre del arma para identificarla en el inventario del jugador
    public GameObject weaponPrefab; // Prefab del arma para instanciar en el juego
    public float animatorItemID; // ID del item para setear el parámetro del animator al equipar el arma
    public AudioManager.Effect attackSound; // Sonido de ataque del arma

    [Header("Firing Properties")]
    public bool isFiringWeapon; // Indica si el arma es un arma de disparo
    public bool requiresReload; // Indica si el arma requiere recarga
    public GameObject projectilePrefab; // Prefab del proyectil que se disparará

    [Header("Shield / Block Properties")]
    public bool canUseShield; // Indica si el arma puede ser usada junto con el escudo
    public float blockMoveSpeedMultiplier = 0.4f; // Penalización de velocidad al bloquear

    private void OnValidate()
    {
        itemType = SpawnOnceType.Weapon; 
    }
}
