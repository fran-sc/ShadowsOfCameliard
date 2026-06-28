using UnityEngine;

// -----------------------------------------------------------------------------
// WeaponCollider
//
// Responsabilidades:
// - Sincroniza el estado del collider del arma con el flag IsAttacking del jugador.
// - Garantiza que el arma solo puede dañar durante la ventana activa del ataque.
//
// Notas:
// - El collider se habilita/deshabilita cada frame en Update().
// -----------------------------------------------------------------------------
public class WeaponCollider : MonoBehaviour
{
    [SerializeField] Collider2D weaponCollider;
    PlayerController playerController;

    void Start()
    {
        playerController = PlayerController.Instance;
    }

    void Update()
    {
        UpdateWeaponColliderState();
    }

    void UpdateWeaponColliderState()
    {
        weaponCollider.enabled = playerController.IsAttacking;
    }
}

