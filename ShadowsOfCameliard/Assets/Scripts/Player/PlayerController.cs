using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

// -----------------------------------------------------------------------------
// PlayerController
//
// Responsabilidades:
// - Lee el input del jugador y gestiona movimiento, dash y combate.
// - Controla el estado de bloqueo con el escudo.
// - Expone propiedades de estado para que otras clases consulten el contexto.
//
// Atributos principales:
// - moveSpeed / dashSpeed: velocidades base de movimiento y dash.
// - dashTime / dashUseCooldown: duración y cooldown del dash.
// - trailEffect / walkDustEffect: efectos visuales de movimiento.
//
// Notas:
// - Las acciones de input se suscriben en Start() y se desuscriben en OnDestroy().
// - Bloquear, atacar y hacer dash son mutuamente excluyentes.
// -----------------------------------------------------------------------------
public class PlayerController : PersistentSingleton<PlayerController>
{
    [Header("Player Movement")]
    [SerializeField] float moveSpeed;

    [Header("Dash Settings")]
    [SerializeField] float dashSpeed;
    [SerializeField] float dashTime = .2f;
    [SerializeField] float dashUseCooldown = 1f;
    [SerializeField] GameObject trailEffect;

    [Header("Walk Dust Settings")]
    [SerializeField] bool enableWalkDust = true;
    [SerializeField] ParticleSystem walkDustEffect;

    Rigidbody2D rb;
    Animator anim;
    SpriteRenderer sr;
    PlayerControls controls;
    Vector2 movement;
    Vector2 lastMovementDirection = Vector2.down;

    float speed;
    float dashCooldownTimer;

    bool canDash = true;
    bool isDashing = false;
    Coroutine dashCoroutine;

    public Vector2 Movement => movement;

    public bool IsLookingRight { get; private set; } = true;
    public bool IsAttacking { get; private set; } = false;
    public bool IsDashing => isDashing;
    public bool CanDash => canDash;

    public bool CanUseShield { get; private set; }
    public bool IsBlocking { get; private set; }
    public float BlockMoveSpeedMultiplier { get; private set; } = 0.4f;

    // Fracción del cooldown consumida; 1 = disponible, 0 = en espera
    public float DashCooldownNormalized
    {
        get
        {
            if (dashUseCooldown <= 0f)
            {
                return 1f;
            }

            return 1f - Mathf.Clamp01(dashCooldownTimer / dashUseCooldown);
        }
    }

    ActiveWeapon activeWeapon;

    protected override void Awake()
    {
        base.Awake();

        if (Instance != this)
        {
            return;
        }
    }

    void Start()
    {
        if (Instance != this)
        {
            return;
        }

        controls = InputManager.Instance.Controls;

        controls.Player.Attack.performed += OnAttackPerformed;
        controls.Player.Dash.performed += OnDashPerformed;
        controls.Player.Inventory.performed += OnInventoryPerformed;

        // Nueva acción de bloqueo.
        controls.Player.Block.started += OnBlockStarted;
        controls.Player.Block.canceled += OnBlockCanceled;

        speed = moveSpeed;

        rb = GetComponent<Rigidbody2D>();
        anim = GetComponentInChildren<Animator>();
        sr = GetComponentInChildren<SpriteRenderer>();
        activeWeapon = GetComponent<ActiveWeapon>();

        if (trailEffect != null)
        {
            trailEffect.SetActive(false);
        }
    }

    protected override void OnDestroy()
    {
        if (Instance == this && controls != null)
        {
            controls.Player.Attack.performed -= OnAttackPerformed;
            controls.Player.Dash.performed -= OnDashPerformed;
            controls.Player.Inventory.performed -= OnInventoryPerformed;

            // Importante: desuscribirse también del bloqueo.
            controls.Player.Block.started -= OnBlockStarted;
            controls.Player.Block.canceled -= OnBlockCanceled;
        }

        base.OnDestroy();
    }

    void Update()
    {
        ReadInput();
        HandleDashCooldown();
    }

    void FixedUpdate()
    {
        Move();
    }

    void OnAttackPerformed(InputAction.CallbackContext context)
    {
        Attack();
    }

    void OnDashPerformed(InputAction.CallbackContext context)
    {
        Dash();
    }

    void OnInventoryPerformed(InputAction.CallbackContext context)
    {
        InventoryManager.Instance.ToggleInventory();
    }

    void OnBlockStarted(InputAction.CallbackContext context)
    {
        StartBlocking();
    }

    void OnBlockCanceled(InputAction.CallbackContext context)
    {
        StopBlocking();
    }

    void ReadInput()
    {
        if (controls == null)
        {
            return;
        }

        movement = controls.Player.Move.ReadValue<Vector2>();

        if (anim != null)
        {
            anim.SetFloat("movement", movement.magnitude);
        }

        if (movement.sqrMagnitude > 0.01f)
        {
            lastMovementDirection = movement.normalized;
        }

        if (movement.x != 0)
        {
            IsLookingRight = movement.x > 0;
        }
    }

    // -----------------------------------------------------------------------------
    // Move
    //
    // - Aplica el movimiento al Rigidbody2D con la velocidad actual.
    // - Reduce la velocidad si el jugador está bloqueando.
    // - Actualiza la escala del transform para orientar el sprite.
    // -----------------------------------------------------------------------------
    void Move()
    {
        float currentSpeed = speed;

        if (IsBlocking && !isDashing)
        {
            currentSpeed *= BlockMoveSpeedMultiplier;
        }

        rb.MovePosition(rb.position + movement.normalized * currentSpeed * Time.fixedDeltaTime);

        transform.localScale = new Vector3(IsLookingRight ? 1 : -1, 1, 1);

        if (enableWalkDust)
        {
            HandleWalkDustEffect();
        }
    }

    void HandleWalkDustEffect()
    {
        if (walkDustEffect == null)
        {
            return;
        }

        if (isDashing)
        {
            if (walkDustEffect.isPlaying)
            {
                walkDustEffect.Stop();
            }

            return;
        }

        if (movement.magnitude > 0.1f && walkDustEffect.isStopped)
        {
            walkDustEffect.Play();
        }
        else if (movement.magnitude < 0.1f && walkDustEffect.isPlaying)
        {
            walkDustEffect.Stop();
        }
    }

    void Attack()
    {
        if (IsAttacking)
        {
            return;
        }

        if (isDashing)
        {
            return;
        }

        // Mientras bloquea, no puede atacar.
        if (IsBlocking)
        {
            return;
        }

        if (anim != null)
        {
            anim.SetTrigger("attack");
        }

        if (activeWeapon != null)
        {
            activeWeapon.FireWeapon();
        }
    }

    // -----------------------------------------------------------------------------
    // Dash
    //
    // - Valida que el dash esté disponible y que haya dirección de movimiento.
    // - Cancela el bloqueo si estaba activo antes de iniciar el dash.
    // - Delega la ejecución en DashRoutine.
    // -----------------------------------------------------------------------------
    void Dash()
    {
        if (!canDash)
        {
            return;
        }

        if (isDashing)
        {
            return;
        }

        if (movement.sqrMagnitude <= 0.01f && lastMovementDirection.sqrMagnitude <= 0.01f)
        {
            return;
        }

        // Si empieza un dash, cancelamos el bloqueo.
        if (IsBlocking)
        {
            StopBlocking();
        }

        if (dashCoroutine != null)
        {
            StopCoroutine(dashCoroutine);
        }

        dashCoroutine = StartCoroutine(DashRoutine());
    }

    // -----------------------------------------------------------------------------
    // DashRoutine
    //
    // - Activa el modo dash: sube la velocidad, muestra el trail y espera dashTime.
    // - Al terminar restaura la velocidad normal y deshabilita el trail.
    // -----------------------------------------------------------------------------
    IEnumerator DashRoutine()
    {
        isDashing = true;
        canDash = false;
        dashCooldownTimer = dashUseCooldown;

        speed = dashSpeed;

        if (trailEffect != null)
        {
            trailEffect.SetActive(true);
        }

        yield return new WaitForSeconds(dashTime);

        speed = moveSpeed;
        isDashing = false;

        if (trailEffect != null)
        {
            trailEffect.SetActive(false);
        }

        dashCoroutine = null;
    }

    // -----------------------------------------------------------------------------
    // HandleDashCooldown
    //
    // - Decrementa el temporizador del cooldown y habilita el dash al llegar a cero.
    // -----------------------------------------------------------------------------
    void HandleDashCooldown()
    {
        if (canDash)
        {
            return;
        }

        dashCooldownTimer -= Time.deltaTime;

        if (dashCooldownTimer <= 0f)
        {
            dashCooldownTimer = 0f;
            canDash = true;
        }
    }

    public void SetAttackingFlag(bool attacking)
    {
        IsAttacking = attacking;

        // Si por animación o timing empieza un ataque, nos aseguramos de cancelar el bloqueo.
        if (IsAttacking && IsBlocking)
        {
            StopBlocking();
        }
    }

    // -----------------------------------------------------------------------------
    // ResetTriggerAnimations
    //
    // - Resetea el Animator con Rebind() para cancelar animaciones pendientes.
    // - Restaura el parámetro "item" y el estado de bloqueo tras el reset.
    // -----------------------------------------------------------------------------
    public void ResetTriggerAnimations()
    {
        if (anim == null)
        {
            return;
        }

        float currentItem = anim.GetFloat("item");
        anim.Rebind();
        anim.SetFloat("item", currentItem);

        // Tras un Rebind, restauramos el estado visual de bloqueo si procede.
        anim.SetBool("block", IsBlocking);
    }

    // -----------------------------------------------------------------------------
    // SetShieldCompatibility
    //
    // - Configura si el arma actual permite usar el escudo y el multiplicador de
    //   velocidad al bloquear. Si no puede usar escudo, cancela el bloqueo activo.
    // -----------------------------------------------------------------------------
    public void SetShieldCompatibility(bool canUseShield, float blockMoveSpeedMultiplier)
    {
        CanUseShield = canUseShield;
        BlockMoveSpeedMultiplier = blockMoveSpeedMultiplier;

        if (!CanUseShield)
        {
            StopBlocking();
        }
    }

    // -----------------------------------------------------------------------------
    // StartBlocking / StopBlocking
    //
    // - Activa o desactiva el estado de bloqueo y actualiza el parámetro del Animator.
    // - StartBlocking requiere que el arma soporte escudo y que no haya ataque ni dash.
    // -----------------------------------------------------------------------------
    public void StartBlocking()
    {
        if (!CanUseShield)
        {
            return;
        }

        if (IsAttacking)
        {
            return;
        }

        if (isDashing)
        {
            return;
        }

        IsBlocking = true;

        if (anim != null)
        {
            anim.SetBool("block", true);
        }
    }

    public void StopBlocking()
    {
        if (!IsBlocking)
        {
            return;
        }

        IsBlocking = false;

        if (anim != null)
        {
            anim.SetBool("block", false);
        }
    }
}