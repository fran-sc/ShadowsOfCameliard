using System.Collections;
using UnityEngine;

public enum EnemyState
{
    Idle,
    Roaming,
    Chasing,
    Attacking
}

// -----------------------------------------------------------------------------
// EnemyAI
//
// Responsabilidades:
// - Gestiona la máquina de estados del enemigo (Idle, Roaming, Chasing, Attacking).
// - Coordina el movimiento, el ataque y la detección del jugador.
// - Aplica daño y knockback al objetivo cuando el ataque conecta.
//
// Atributos principales:
// - idleChance: probabilidad de que el estado base sea Idle en lugar de Roaming.
// - attackRange: distancia máxima para atacar al jugador.
// - attackCooldown: tiempo entre ataques consecutivos.
//
// Notas:
// - Los estados Idle y Roaming se seleccionan aleatoriamente como estado base.
// - El estado Chasing se activa externamente desde EnemyDetector.
// - El ataque solo conecta si el enemigo está mirando al objetivo (IsFacingTarget).
// -----------------------------------------------------------------------------
public class EnemyAI : MonoBehaviour
{
    [Header("Idle/Roaming Settings")]
    [SerializeField] float idleChance = 0.5f;
    [SerializeField] float minStateDuration = 2f;
    [SerializeField] float maxStateDuration = 5f;
    [SerializeField] float roamingStepDuration = 2f;
    [SerializeField] float idleStepDuration = 0.5f;

    [Header("Attack Settings")]
    [SerializeField] float attackRange = 0.9f;
    [SerializeField] float attackCooldown = 1.2f;
    [SerializeField] int attackDamage = 1;
    [SerializeField] float pushForce = 2f;

    EnemyPathfinding enemyPathfinding;
    Animator animator;

    EnemyState state;
    public EnemyState State => state;

    GameObject currentTarget;

    bool canAttack = true;
    Coroutine attackCooldownCoroutine;

    float stateTimer;
    float stateDuration;

    float GetBaseStateDuration => Random.Range(minStateDuration, maxStateDuration);
    EnemyState GetRandomBaseState => Random.value < idleChance ? EnemyState.Idle : EnemyState.Roaming;

    void Awake()
    {
        enemyPathfinding = GetComponent<EnemyPathfinding>();
        animator = GetComponentInChildren<Animator>();
    }

    void OnDisable()
    {
        StopAllCoroutines();

        currentTarget = null;
        canAttack = true;

        if (enemyPathfinding != null)
        {
            enemyPathfinding.StopMovement();
        }
    }

    void Start()
    {
        ResetBaseState();
    }

    void Update()
    {
        if (!CanRunAI())
        {
            return;
        }

        stateTimer += Time.deltaTime;        
    }

    // -----------------------------------------------------------------------------
    // ResetBaseState
    //
    // - Cancela todas las corrutinas y transiciona al estado base aleatorio.
    // - Selecciona aleatoriamente entre Idle y Roaming según idleChance.
    // -----------------------------------------------------------------------------
    void ResetBaseState()
    {
        if (!CanRunAI())
        {
            return;
        }

        StopAllCoroutines();

        currentTarget = null;
        canAttack = true;

        state = GetRandomBaseState;
        stateDuration = GetBaseStateDuration;
        stateTimer = 0f;

        enemyPathfinding.StopMovement();

        switch (state)
        {
            case EnemyState.Idle:
                StartCoroutine(Idle());
                break;
                
            case EnemyState.Roaming:
                StartCoroutine(Roaming());
                break;
        }
    }

    IEnumerator Idle()
    {
        while (CanRunAI() && state == EnemyState.Idle && stateTimer < stateDuration)
        {
            if (Random.value < 0.5f)
            {
                // invertimos el sprite para que mire hacia el otro lado
                transform.localScale = new Vector3(
                    -transform.localScale.x, 
                    transform.localScale.y, 
                    transform.localScale.z);    
            }

            yield return new WaitForSeconds(idleStepDuration);
        }
    
        ResetBaseState();
    }

    IEnumerator Roaming()
    {
        while (CanRunAI() && state == EnemyState.Roaming && stateTimer < stateDuration)
        {
            Vector2 randomRoamingDirection = GetRoamingDirection();
            enemyPathfinding.MoveTo(randomRoamingDirection);

            yield return new WaitForSeconds(roamingStepDuration);
        }

        ResetBaseState();
    }

    IEnumerator Chasing()
    {
        while (CanRunAI() && state == EnemyState.Chasing && currentTarget != null)
        {
            float distanceToTarget = Vector2.Distance(transform.position, currentTarget.transform.position);

            if (distanceToTarget <= attackRange)
            {
                TryAttack();
            }
            else
            {
                Vector2 directionToTarget = (currentTarget.transform.position - transform.position).normalized;
                enemyPathfinding.MoveTo(directionToTarget);
            }

            yield return null;
        }

        if (state != EnemyState.Attacking)
        {
            ResetBaseState();
        }
    }

    // -----------------------------------------------------------------------------
    // TryAttack
    //
    // - Inicia el ataque si canAttack es verdadero y hay un objetivo válido.
    // - Orienta el sprite hacia el objetivo, dispara el trigger del Animator
    //   e inicia el cooldown del siguiente ataque.
    // -----------------------------------------------------------------------------
    void TryAttack()
    {
        if (!canAttack || currentTarget == null)
        {
            return;
        }

        state = EnemyState.Attacking;
        canAttack = false;

        enemyPathfinding.StopMovement();

        Vector2 directionToTarget = currentTarget.transform.position - transform.position;

        if (Mathf.Abs(directionToTarget.x) > 0.05f)
        {
            transform.localScale = new Vector3(directionToTarget.x > 0 ? 1 : -1, 1, 1);
        }

        if (animator != null)
        {
            animator.ResetTrigger("attack");
            animator.SetTrigger("attack");
        }

        if (attackCooldownCoroutine != null)
        {
            StopCoroutine(attackCooldownCoroutine);
        }

        attackCooldownCoroutine = StartCoroutine(AttackCooldown());
    }

    // -----------------------------------------------------------------------------
    // AttackCooldown
    //
    // - Espera attackCooldown segundos y decide si atacar de nuevo o reiniciar Chasing.
    // -----------------------------------------------------------------------------
    IEnumerator AttackCooldown()
    {
        yield return new WaitForSeconds(attackCooldown);

        canAttack = true;
        attackCooldownCoroutine = null;

        if (!CanRunAI())
        {
            yield break;
        }

        if (currentTarget == null)
        {
            ResetBaseState();
            yield break;
        }
        
        state = EnemyState.Chasing;

        float distanceToTarget = Vector2.Distance(transform.position, currentTarget.transform.position);

        if (distanceToTarget <= attackRange)
        {
            TryAttack();
        }
        else
        {
            StartCoroutine(Chasing());
        }
    }

    // -----------------------------------------------------------------------------
    // DealAttackDamage
    //
    // - Valida distancia y orientación antes de aplicar daño y knockback al objetivo.
    // - Llamado desde EnemyAttackEvent en el momento exacto de la animación.
    // -----------------------------------------------------------------------------
    public void DealAttackDamage()
    {
        if (currentTarget == null)
        {
            return;
        }

        float distanceToTarget = Vector2.Distance(transform.position, currentTarget.transform.position);
        
        if (distanceToTarget > attackRange)
        {
            return;
        }

        if (!IsFacingTarget())
        {
            return;
        }        

        // Daño al jugador
        ApplyDamageToTarget();
    }

    // -----------------------------------------------------------------------------
    // IsFacingTarget
    //
    // - Comprueba si la escala X del sprite es coherente con la posición del objetivo.
    // - Evita que el ataque conecte cuando el enemigo está de espaldas al jugador.
    // -----------------------------------------------------------------------------
    bool IsFacingTarget()
    {
        if (currentTarget == null)
        {
            return false;
        }

        float directionToTargetX = currentTarget.transform.position.x - transform.position.x;

        bool targetIsOnRight = directionToTargetX > 0f;
        bool targetIsOnLeft = directionToTargetX < 0f;

        bool enemyIsFacingRight = transform.localScale.x > 0f;
        bool enemyIsFacingLeft = transform.localScale.x < 0f;

        return (enemyIsFacingRight && targetIsOnRight) ||
            (enemyIsFacingLeft && targetIsOnLeft);
    }

    void ApplyDamageToTarget()
    {
        // Aplicamos daño al target (si recibe daño)
        DamageReceiver targetHealth = currentTarget.GetComponentInParent<DamageReceiver>();
        if (targetHealth != null)
        {
            targetHealth.TakeDamage(attackDamage);
        }

        // Aplicamos desplazamiento al enemigo (si se mueve)
        PushBack pushBack = currentTarget.GetComponent<PushBack>();
        if (pushBack != null)
        {
            pushBack.Push(
                (currentTarget.transform.position - transform.position).normalized, 
                pushForce);
        }
    }

    Vector2 GetRoamingDirection()
    {
        return new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized;
    }

    // -----------------------------------------------------------------------------
    // StartChasing
    //
    // - Cancela el estado actual, establece el objetivo y lanza la corrutina Chasing.
    // - Ignorado si el enemigo ya está persiguiendo al mismo objetivo.
    // -----------------------------------------------------------------------------
    public void StartChasing(GameObject target)
    {
        if (!CanRunAI() || target == null)
        {
            return;
        }

        if ((state == EnemyState.Chasing || state == EnemyState.Attacking) &&
            currentTarget == target)
        {
            return;
        }

        StopAllCoroutines();

        currentTarget = target;
        state = EnemyState.Chasing;

        enemyPathfinding.StopMovement();

        StartCoroutine(Chasing());
    }

    public void StopChasing()
    {
        if (!CanRunAI())
        {
            return;
        }

        if (attackCooldownCoroutine != null)
        {
            StopCoroutine(attackCooldownCoroutine);
            attackCooldownCoroutine = null;
        }

        currentTarget = null;
        canAttack = true;

        enemyPathfinding.StopMovement();
        
        ResetBaseState();
    }

    // Devuelve true solo si el componente está activo y EnemyPathfinding está disponible
    bool CanRunAI()
    {
        return isActiveAndEnabled
               && gameObject.activeInHierarchy
               && enemyPathfinding != null;
    }
}
