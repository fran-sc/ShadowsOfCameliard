using UnityEngine;

// -----------------------------------------------------------------------------
// EnemyAttackEvent
//
// Responsabilidades:
// - Recibe eventos de animación de ataque y los delega al EnemyAI.
// - Reproduce el efecto de sonido de ataque en el momento correcto.
//
// Notas:
// - OnAttackAnimationEvent() es llamado desde un Animation Event del Animator.
// - La validación de rango y orientación la realiza EnemyAI.DealAttackDamage().
// -----------------------------------------------------------------------------
public class EnemyAttackEvent : MonoBehaviour
{
    [SerializeField] AudioManager.Effect attackSound; // Sonido de ataque del enemigo
    EnemyAI enemyAI;

    void Awake()
    {
        enemyAI = GetComponentInParent<EnemyAI>();
    }

    // -----------------------------------------------------------------------------
    // OnAttackAnimationEvent
    //
    // - Reproduce el sonido de ataque y delega el daño en EnemyAI.
    // - Llamado automáticamente por el Animation Event de la animación de ataque.
    // -----------------------------------------------------------------------------
    public void OnAttackAnimationEvent()
    {
        // Reproducimos el sonido de ataque del enemigo
        if (attackSound != AudioManager.Effect.None)
        {
            AudioManager.Instance.PlayEffect(attackSound);
        }

        // Llamamos al método DealAttackDamage del EnemyAI
        DealAttackDamage();
    }

    void DealAttackDamage()
    {
        if (enemyAI != null)
        {
            enemyAI.DealAttackDamage();
        }
    }
}
