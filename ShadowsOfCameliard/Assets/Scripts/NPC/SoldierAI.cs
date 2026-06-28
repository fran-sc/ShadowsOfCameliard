using System.Collections;
using UnityEngine;

// -----------------------------------------------------------------------------
// SoldierAI
//
// Responsabilidades:
// - Controla el comportamiento del soldado aliado (NPC estático de guardia).
// - Alterna aleatoriamente la orientación del sprite y dispara animaciones de ataque.
// - Reproduce efectos de sonido de ataque si el jugador está en rango.
//
// Atributos principales:
// - idleStepDuration: intervalo entre cambios en reposo.
// - attackStepDuration: duración de la animación de ataque.
// - IsPlayerInRange: comprueba si el jugador está a menos de 4 unidades.
//
// Notas:
// - El soldado no aplica daño real; la animación de ataque es decorativa.
// -----------------------------------------------------------------------------
public class SoldierAI : MonoBehaviour
{
    [SerializeField] float idleStepDuration = 0.5f;
    [SerializeField] float attackStepDuration = 0.5f;
    [SerializeField] AudioClip attackSoundEffect;

    Animator anim;

    bool IsPlayerInRange => PlayerController.Instance != null && 
        Vector2.Distance(transform.position, PlayerController.Instance.transform.position) < 4f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        anim = GetComponentInChildren<Animator>();

        StartCoroutine(Idle());
    }

    void OnDisable()
    {
        StopAllCoroutines();      
    }    

    // -----------------------------------------------------------------------------
    // Idle
    //
    // - Bucle infinito que aleatoriza la orientación del sprite periódicamente.
    // - Con probabilidad 0.5 dispara la animación de ataque y reproduce el sonido
    //   si el jugador está en rango.
    // -----------------------------------------------------------------------------
    IEnumerator Idle()
    {
        while (true)
        {
            // Orientación aleatoria del sprite del soldado
            if (Random.value < 0.5f)
            {
                // invertimos el sprite para que mire hacia el otro lado
                transform.localScale = new Vector3(
                    -transform.localScale.x, 
                    transform.localScale.y, 
                    transform.localScale.z);    
            }

            yield return new WaitForSeconds(idleStepDuration);

            // Disparo?
            if (Random.value < 0.5f)
            {
                // Activamos la animación de ataque
                anim.SetTrigger("attack");

                // Reproducimos el sonido de ataque si se ha configurado uno
                if (IsPlayerInRange && attackSoundEffect != null)
                {
                    AudioManager.Instance.PlayEffect(attackSoundEffect, 0.1f);
                }

                yield return new WaitForSeconds(attackStepDuration);
            }
        }
    }
}
