
using UnityEngine;

// -----------------------------------------------------------------------------
// EnemyPathfinding
//
// Responsabilidades:
// - Mueve al enemigo en la dirección indicada por EnemyAI.
// - Selecciona la velocidad según el estado (patrulla / persecución).
// - Gestiona el efecto de polvo al caminar.
//
// Atributos principales:
// - moveSpeed: velocidad de patrulla.
// - chaseSpeed: velocidad de persecución.
// - walkDustEffect: partículas de polvo al moverse.
//
// Notas:
// - El movimiento se bloquea si el enemigo está siendo empujado (PushBack.IsPushed).
// - La orientación del sprite se actualiza automáticamente según la dirección.
// -----------------------------------------------------------------------------
public class EnemyPathfinding : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] float moveSpeed;
    [SerializeField] float chaseSpeed;

    [Header("Walk Dust Settings")]
    [SerializeField] bool enableWalkDust = true;
    [SerializeField] ParticleSystem walkDustEffect;

    Rigidbody2D rb;
    PushBack pushBack;
    EnemyAI enemyAI;
    Animator anim;

    Vector2 moveDirection;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponentInChildren<Animator>();
        pushBack = GetComponent<PushBack>();
        enemyAI = GetComponent<EnemyAI>();
    }

    void Update()
    {
        SetAnimation();
    }

    void FixedUpdate()
    {
        Move();

        // Efecto de polvo al caminar
        if (enableWalkDust)
        {
            HandleWalkDustEffect();
        }        
    }

    // -----------------------------------------------------------------------------
    // Move
    //
    // - Aplica desplazamiento mediante MovePosition con la velocidad adecuada al estado.
    // - No mueve si el enemigo está Idle, Attacking o siendo empujado.
    // - Actualiza la orientación del sprite según la dirección X de movimiento.
    // -----------------------------------------------------------------------------
    void Move()
    {
        if (enemyAI.State == EnemyState.Idle || enemyAI.State == EnemyState.Attacking)
        {
            return;   
        }

        if (pushBack.IsPushed) 
        {
            return;
        }
        
        rb.MovePosition(rb.position + 
            moveDirection * (enemyAI.State == EnemyState.Chasing ? chaseSpeed : moveSpeed) * Time.fixedDeltaTime);

        if (Mathf.Abs(moveDirection.x) > 0.05f)
        {
            transform.localScale = new Vector3(moveDirection.x > 0 ? 1 : -1, 1, 1);
        }
    }

    void HandleWalkDustEffect()
    {
        if (moveDirection.magnitude > 0.1f && walkDustEffect.isStopped)
        {               
            walkDustEffect.Play();
        }
        else if (moveDirection.magnitude < 0.1f && walkDustEffect.isPlaying)
        {
            walkDustEffect.Stop();
        }
    }
    public void MoveTo(Vector2 targetDir)
    {
        moveDirection = targetDir;
    }

    void SetAnimation()
    {
        anim.SetFloat("movement", moveDirection.magnitude);
    }

    public void StopMovement()
    {
        moveDirection = Vector2.zero;
    }
}
