using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

// -----------------------------------------------------------------------------
// PlayerHealth
//
// Responsabilidades:
// - Gestiona la vida actual del jugador.
// - Aplica daño y efectos visuales de impacto.
// - Controla la regeneración cuando el jugador está inactivo.
// - Notifica a la UI cuando cambia la salud.
//
// Atributos principales:
// - enableIdleRegeneration: habilita la regeneración por inactividad.
// - idleHealInterval: tiempo en inactividad para la curación.
// - idleMovementThreshold: umbral de movimiento para considerar inactividad.
// - idleHealLimit: límite de corazones recuperables por inactividad.
//
// Notas:
// - La muerte del jugador se resuelve reiniciando la escena actual.
// - Se utiliza un efecto de desvanecimiento de pantalla al morir.
// - La regeneración por inactividad solo ocurre si el jugador está quieto y 
// por debajo del límite de curación.
// -----------------------------------------------------------------------------
public class PlayerHealth : DamageReceiver
{
    [Header("Hit Effect")]
    [SerializeField] private Color hitColor = Color.red;
    [SerializeField] private float hitTime = 0.1f;

    [Header("Idle Regeneration")]
    [SerializeField] private bool enableIdleRegeneration = true;
    [SerializeField] private int idleHealLimit = 3;
    [SerializeField] private float idleHealInterval = 3f;
    [SerializeField] private float idleMovementThreshold = 0.05f;
    [SerializeField] private AudioManager.Effect healerSound;

    SpriteRenderer spriteRenderer;
    Material material;
    Coroutine hitEffectCoroutine;

    Rigidbody2D rb;
    Animator anim;

    float idleTimer;

    // Compara la magnitud del movimiento actual con el umbral para determinar inactividad
    bool IsPlayerIdle => 
        PlayerController.Instance.Movement.magnitude < idleMovementThreshold;

    protected override void Start()
    {
        base.Start();

        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        anim = GetComponentInChildren<Animator>();

        if (spriteRenderer != null)
        {
            // Obtiene la instancia del material, no el asset compartido
            material = spriteRenderer.material; 
        }

        rb = GetComponent<Rigidbody2D>();

        // Sincroniza la UI con el estado inicial de salud
        UpdateHealthUI(); 
    }

    private void Update()
    {
        // Chequeo de regeneración por inactividad
        HandleIdleRegeneration();
    }

    private void OnEnable()
    {
        // Suscripción al evento de carga de escena para actualizar la UI
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;

        if (material != null)
        {
            // Restaura el color por si se desactiva durante un hit
            material.color = Color.white; 
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        UpdateHealthUI();
    }

    // -----------------------------------------------------------------------------    
    // TakeDamage
    // 
    // - Aplica daño al jugador.
    // - Considera si está bloqueando con el escudo.
    // -----------------------------------------------------------------------------
    public override void TakeDamage(int damage)
    {
        // Evitamos el daño mientras el jugador está bloqueando con el escudo
        if (PlayerController.Instance != null && PlayerController.Instance.IsBlocking)
        {
            AudioManager.Instance.PlayEffect(AudioManager.Effect.ShieldBlock);
            return;
        }

        base.TakeDamage(damage);

        // Cualquier daño recibido reinicia el temporizador de regeneración
        idleTimer = 0f; 
    }

    // -----------------------------------------------------------------------------    
    // OnDamaged
    // 
    // - Actualiza la UI y reproduce el efecto visual de impacto.
    // 
    // Notas:
    // - Se llama automáticamente desde TakeDamage() después de aplicar el daño.
    // -----------------------------------------------------------------------------
    protected override void OnDamaged(int damage)
    {
        // Refleja la salud reducida en los corazones de la UI
        UpdateHealthUI(); 

        // Parpadeo visual de impacto sobre el sprite
        PlayHitEffect(); 
    }

    // -----------------------------------------------------------------------------    
    // OnHealed
    // 
    // - Aplica curación al jugador.
    //
    // Notas:
    // - Se llama automáticamente desde Heal() después de aplicar la curación.
    // -----------------------------------------------------------------------------
    protected override void OnHealed(int amount)
    {
        // Refleja la salud recuperada en los corazones de la UI
        UpdateHealthUI(); 

        if (healerSound != AudioManager.Effect.None) // Solo reproduce sonido si hay uno asignado
        {
            AudioManager.Instance.PlayEffect(healerSound);
        }
    }

    // -----------------------------------------------------------------------------
    // Die
    //
    // - Inicia la secuencia de muerte del jugador.
    //
    // Notas:
    // - Delega la lógica en la corrutina DeathCoroutine.
    // - Se llama automáticamente desde TakeDamage() cuando la salud llega a cero.
    // -----------------------------------------------------------------------------
    protected override void Die()
    {
        StartCoroutine(DeathCoroutine());
    }

    // -----------------------------------------------------------------------------
    // DeathCoroutine
    //
    // - Desactiva los controles y la física del jugador.
    // - Reproduce la animación de muerte.
    // - Aplica un fundido a negro y reinicia la escena activa.
    //
    // Notas:
    // - Restaura el estado del jugador antes de cargar la escena para que la
    //   instancia persistente quede en buen estado tras el reinicio.
    // -----------------------------------------------------------------------------
    IEnumerator DeathCoroutine()
    {
        // Desactivamos los controles del jugador
        InputManager.Instance.Controls.Player.Disable();

        // Desactivamos el rigibody del jugador
        if (rb != null)
        {
            rb.simulated = false;
        }

        // Reproducimos la animacion de muerte del jugador
        if (anim != null)
        {
            anim.SetTrigger("death");
        }

        // Esperamos un tiempo antes de reiniciar la escena
        yield return new WaitForSeconds(2f);

        // Hacemos un fade out de la pantalla antes de reiniciar la escena
        UIFade.Instance.FadeToBlack();
        yield return new WaitForSeconds(UIFade.Instance.FadeDuration);

        // Reiniciamos el estado del jugador
        isDead = false;
        currentHealth = maxHealth;
        UpdateHealthUI();

        if (rb != null)
        {
            rb.simulated = true;
        }

        // Esperamos un frame para asegurarnos de que la escena se haya reiniciado
        yield return null;

        // Reiniciamos la escena actual
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);        
    }

    // -----------------------------------------------------------------------------
    // HandleIdleRegeneration
    //
    // - Acumula tiempo de inactividad y aplica curación cuando se supera el intervalo.
    // - Reinicia el temporizador si el jugador se mueve.
    //
    // Notas:
    // - Solo actúa si enableIdleRegeneration está habilitado, el jugador no ha
    //   muerto y la salud actual está por debajo de idleHealLimit.
    // -----------------------------------------------------------------------------
    private void HandleIdleRegeneration()
    {
        // Condiciones para la regeneración por inactividad
        if (!enableIdleRegeneration) return;         
        if (isDead) return;                            
        if (currentHealth >= idleHealLimit) return;    

        if (IsPlayerIdle)
        {
            // Acumula el tiempo que lleva el jugador quieto
            idleTimer += Time.deltaTime; 

            if (idleTimer >= idleHealInterval)
            {
                // Cura un punto de vida al cumplirse el intervalo
                Heal(1);        

                // Reinicia el ciclo de curación
                idleTimer = 0f; 
            }
        }
        else
        {
            // El movimiento interrumpe el progreso de curación
            idleTimer = 0f; 
        }
    }
    

    // -----------------------------------------------------------------------------
    // UpdateHealthUI
    //
    // - Notifica al UIManager del valor actual de salud para refrescar los corazones.
    // -----------------------------------------------------------------------------
    private void UpdateHealthUI()
    {
        if (UIManager.Instance != null)
        {
            UIManager.Instance.UpdateHealth(currentHealth, maxHealth);
        }
    }

    // -----------------------------------------------------------------------------
    // PlayHitEffect
    //
    // - Lanza la corrutina de efecto de impacto, cancelando la anterior si estaba
    //   en curso.
    // -----------------------------------------------------------------------------
    private void PlayHitEffect()
    {
        if (material == null) return;

        if (hitEffectCoroutine != null)
        {
            // Cancela el efecto anterior para evitar solapamientos
            StopCoroutine(hitEffectCoroutine); 
        }

        hitEffectCoroutine = StartCoroutine(HitEffectRoutine());
    }

    // -----------------------------------------------------------------------------
    // HitEffectRoutine
    //
    // - Cambia el color del material al color de impacto durante hitTime segundos
    //   y lo restaura al color original al finalizar.
    // -----------------------------------------------------------------------------
    private IEnumerator HitEffectRoutine()
    {
        material.color = hitColor;

        yield return new WaitForSeconds(hitTime);

        material.color = Color.white;

        // Libera la referencia al finalizar el efecto
        hitEffectCoroutine = null; 
    }
}