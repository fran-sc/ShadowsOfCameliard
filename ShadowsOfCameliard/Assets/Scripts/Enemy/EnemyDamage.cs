using System.Collections;
using UnityEngine;

// -----------------------------------------------------------------------------
// EnemyDamage
//
// Responsabilidades:
// - Gestiona la salud, el efecto visual de impacto y la muerte del enemigo.
// - Desactiva física y colisionador al morir y lanza la animación de muerte.
// - Instancia un VFX de muerte y destruye el GameObject tras el retraso configurado.
//
// Atributos principales:
// - hitColor / hitTime: color y duración del destello al recibir daño.
// - deathDuration: tiempo antes de destruir el objeto al morir.
// - deathVFX: efecto de partículas instanciado al morir.
//
// Notas:
// - El material se obtiene por instancia para no afectar a otros enemigos.
// - Die() y OnDamaged() son los puntos de extensión de DamageReceiver.
// -----------------------------------------------------------------------------
public class EnemyDamage : DamageReceiver
{
    [Header("Hit Settings")]
    [SerializeField] private Color hitColor = Color.red;
    [SerializeField] private float hitTime = 0.1f;

    [Header("Death Settings")]
    [SerializeField] private float deathDuration = 0.5f;
    [SerializeField] private GameObject deathVFX;

    private SpriteRenderer spriteRenderer;
    private Material material;
    private Animator animator;
    private Collider2D enemyCollider;
    private Rigidbody2D rb;
    private Coroutine hitEffectCoroutine;

    protected override void Start()
    {
        base.Start();

        spriteRenderer = GetComponentInChildren<SpriteRenderer>();

        if (spriteRenderer != null)
        {
            material = spriteRenderer.material;
        }

        animator = GetComponentInChildren<Animator>();
        enemyCollider = GetComponent<Collider2D>();
        rb = GetComponent<Rigidbody2D>();
    }

    protected override void OnDamaged(int damage)
    {
        PlayHitEffect();
    }

    // -----------------------------------------------------------------------------
    // Die
    //
    // - Cancela todas las corrutinas, deshabilita collider y Rigidbody.
    // - Dispara la animación de muerte e invoca DestroyWithFX tras deathDuration.
    // -----------------------------------------------------------------------------
    protected override void Die()
    {
        StopAllCoroutines();

        if (material != null)
        {
            material.color = Color.white;
        }

        if (enemyCollider != null)
        {
            enemyCollider.enabled = false;
        }

        if (rb != null)
        {
            rb.simulated = false;
        }

        if (animator != null)
        {
            animator.SetTrigger("death");
        }

        Invoke(nameof(DestroyWithFX), deathDuration);
    }

    private void PlayHitEffect()
    {
        if (material == null) return;

        if (hitEffectCoroutine != null)
        {
            StopCoroutine(hitEffectCoroutine);
        }

        hitEffectCoroutine = StartCoroutine(HitEffectRoutine());
    }

    private IEnumerator HitEffectRoutine()
    {
        material.color = hitColor;

        yield return new WaitForSeconds(hitTime);

        material.color = Color.white;
        hitEffectCoroutine = null;
    }

    // -----------------------------------------------------------------------------
    // DestroyWithFX
    //
    // - Instancia el VFX de muerte en la posición actual y destruye el GameObject.
    // -----------------------------------------------------------------------------
    private void DestroyWithFX()
    {
        if (deathVFX != null)
        {
            Instantiate(deathVFX, transform.position, Quaternion.identity);
        }

        Destroy(gameObject);
    }
}