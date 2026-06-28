using UnityEngine;

// -----------------------------------------------------------------------------
// DamageReceiver
//
// Responsabilidades:
// - Clase base abstracta para cualquier entidad que pueda recibir daño o curarse.
// - Gestiona la salud actual y sus límites máximos.
// - Delega en métodos virtuales el comportamiento específico de daño y muerte.
//
// Atributos principales:
// - maxHealth: salud máxima configurable desde el Inspector.
// - deathSound: efecto de sonido reproducido al morir.
// - currentHealth / isDead: estado actual de salud.
//
// Notas:
// - Die() es abstracto; cada subclase define su propia secuencia de muerte.
// - OnDamaged() y OnHealed() son puntos de extensión para efectos visuales y UI.
// - currentHealth se mantiene dentro de [0, maxHealth] en todo momento.
// -----------------------------------------------------------------------------
public abstract class DamageReceiver : MonoBehaviour
{
    [Header("Health Settings")]
    [SerializeField] protected int maxHealth = 1;
    [SerializeField] private AudioManager.Effect deathSound;

    protected int currentHealth;
    protected bool isDead;

    public int MaxHealth => maxHealth;
    public int CurrentHealth => currentHealth;
    public bool IsDead => isDead;

    protected virtual void Start()
    {
        currentHealth = maxHealth;
    }

    // -----------------------------------------------------------------------------
    // TakeDamage
    //
    // - Aplica el daño, limita la salud a [0, maxHealth] y notifica OnDamaged.
    // - Si la salud llega a 0 marca isDead, reproduce el sonido de muerte y llama Die().
    //
    // Notas:
    // - Ignorado si el receptor ya está muerto o el daño es <= 0.
    // -----------------------------------------------------------------------------
    public virtual void TakeDamage(int damage)
    {
        if (isDead) return;
        if (damage <= 0) return;

        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        OnDamaged(damage);

        if (currentHealth <= 0)
        {
            isDead = true;

            if (deathSound != AudioManager.Effect.None)
            {
                AudioManager.Instance.PlayEffect(deathSound);
            }
            
            Die();
        }
    }

    // -----------------------------------------------------------------------------
    // Heal
    //
    // - Suma amount a la salud actual, limita a [0, maxHealth] y notifica OnHealed.
    //
    // Notas:
    // - Ignorado si el receptor está muerto o amount es <= 0.
    // -----------------------------------------------------------------------------
    public virtual void Heal(int amount)
    {
        if (isDead) return;
        if (amount <= 0) return;

        currentHealth += amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        OnHealed(amount);
    }

    public virtual void HealToFull()
    {
        if (isDead) return;

        currentHealth = maxHealth;
        OnHealed(maxHealth);
    }

    protected virtual void OnDamaged(int damage)
    {
    }

    protected virtual void OnHealed(int amount)
    {
    }

    protected abstract void Die();
}