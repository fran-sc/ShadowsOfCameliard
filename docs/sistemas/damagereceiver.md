# DamageReceiver

`DamageReceiver` es la clase base abstracta para objetos que pueden recibir daño y morir.

## Codigo base

```csharp
using UnityEngine;

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
```

## Responsabilidades

- Guardar vida maxima y vida actual.
- Ignorar dano invalido o recibido tras la muerte.
- Ejecutar hooks `OnDamaged` y `OnHealed`.
- Reproducir sonido de muerte si esta configurado.
- Forzar a las clases hijas a implementar `Die()`.

## Clases que la especializan

- `PlayerHealth`: muerte del jugador, regeneracion y actualizacion de UI.
- `EnemyDamage`: feedback de golpe, muerte y destruccion con VFX.

[< volver](README.md)