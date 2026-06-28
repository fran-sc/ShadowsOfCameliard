using System.Collections;
using UnityEngine;

public enum CharacterState
{
    Idle,
    Talking,
    Walking,
    Running
}

// -----------------------------------------------------------------------------
// CharacterAI
//
// Responsabilidades:
// - Controla el comportamiento básico de un NPC (personaje no jugable).
// - Muestra diálogo y spawnea items al interactuar con el jugador.
// - Opcionalmente puede curar al jugador al contacto.
//
// Atributos principales:
// - talkLines: frases de diálogo aleatorias al acercarse.
// - talkLineWhenSpawn: frase especial al ofrecer un item.
// - isHealer / healAmount: si el NPC cura, cantidad de vida que restaura.
//
// Notas:
// - La orientación del sprite se aleatoriza periódicamente en el estado Idle.
// - ResourceSpawner gestiona el spawn del item ofrecido por el NPC.
// -----------------------------------------------------------------------------
public class CharacterAI : MonoBehaviour
{
    [SerializeField] float idleStepDuration = 0.5f;

    [Header("Talk Lines")]
    [TextArea(3, 10)][SerializeField] string talkLineWhenSpawn;

    [TextArea(3, 10)][SerializeField] string[] talkLines;

    [Header("Healer")]
    [SerializeField] private bool isHealer = false;
    [SerializeField] private int healAmount = 5;

    CharacterState state;
    Animator anim;
    ResourceSpawner resourceSpawner;

    void Start()
    {
        anim = GetComponentInChildren<Animator>();
        resourceSpawner = GetComponent<ResourceSpawner>();

        SetState(CharacterState.Idle);
    }

    void OnDisable()
    {
        StopAllCoroutines();      
    }

    void SetState(CharacterState state)
    {
        this.state = state;

        switch (this.state)
        {
            case CharacterState.Idle:
                StartCoroutine(Idle());
                break;
        }
    }

    IEnumerator Idle()
    {
        while (CanRunAI() && state == CharacterState.Idle)
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
    }

    bool CanRunAI()
    {
        return isActiveAndEnabled
               && gameObject.activeInHierarchy;
    }

    // -----------------------------------------------------------------------------
    // OnTriggerEnter2D
    //
    // - Al entrar el jugador en el trigger: spawnea el item si procede y muestra
    //   el diálogo asociado. Si el NPC es sanador aplica curación al jugador.
    // - Si no hay item que spawnear, muestra una línea de diálogo aleatoria.
    // -----------------------------------------------------------------------------
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            // Si hay algo para spawnear...
            if (resourceSpawner != null && resourceSpawner.CheckResourceToSpawn())
            {   
                // Mostramos el mensaje de que se orfece un item al jugador
                MessageManager.Instance.ShowMessage(talkLineWhenSpawn);

                // Spawneamos el recurso
                resourceSpawner.SpawnResource();

                // Si es un sanador, curamos al jugador
                if (isHealer)
                {
                    PlayerHealth playerHealth = collision.GetComponentInParent<PlayerHealth>();
                    if (playerHealth != null)
                    {
                        playerHealth.Heal(healAmount);
                    }
                }                

                return;
            }

            // Diálogo
            if (talkLines.Length > 0)
            {
                // Extraemos de forma aleatoria una línea de diálogo
                string lineToTalk = talkLines[Random.Range(0, talkLines.Length)];
                
                // Mostramos el diálogo en la UI
                MessageManager.Instance.ShowMessage(lineToTalk);
            }
        }
    }
}
