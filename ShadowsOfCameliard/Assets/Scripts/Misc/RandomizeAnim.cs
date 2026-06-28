using UnityEngine;

// -----------------------------------------------------------------------------
// RandomizeAnim
//
// Responsabilidades:
// - Aleatoriza el tiempo de inicio de la animación actual para evitar sincronía visual.
//
// Notas:
// - Útil en grupos de objetos con la misma animación (antorchas, plantas, etc.)
//   para que no parpadeen o se muevan sincronizados.
// -----------------------------------------------------------------------------
public class RandomizeAnim : MonoBehaviour
{
    Animator anim;

    void Awake()
    {
        anim = GetComponent<Animator>();
    }

    void Start()
    {
        // Obtiene el AnimatorStateInfo de la capa 0 (primera capa)
        AnimatorStateInfo stateInfo = anim.GetCurrentAnimatorStateInfo(0);    

        // Reproduce la animación actual con un tiempo aleatorio
        anim.Play(stateInfo.fullPathHash, -1, Random.Range(0f, 1f));
    }
}
