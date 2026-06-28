using UnityEngine;

// -----------------------------------------------------------------------------
// PlayerAnimatorEvents
//
// Responsabilidades:
// - Recibe eventos de animación del Animator y los delega al PlayerController.
// - Controla el flag IsAttacking durante la ventana activa del ataque.
//
// Notas:
// - OnAttackStart() y OnAttackEnd() son llamados desde Animation Events del Animator.
// -----------------------------------------------------------------------------
public class PlayerAnimatorEvents : MonoBehaviour
{
    PlayerController playerController;

    void Start()
    {
        playerController = GetComponentInParent<PlayerController>();
    }

    // Activa el flag de ataque al comenzar la ventana de daño de la animación
    void OnAttackStart()
    {
        playerController.SetAttackingFlag(true);
    }

    // Desactiva el flag de ataque al terminar la ventana de daño de la animación
    void OnAttackEnd()
    {
        playerController.SetAttackingFlag(false);
    }
}
