using System.Collections;
using UnityEngine;

// -----------------------------------------------------------------------------
// InkRevealController
//
// Responsabilidades:
// - Controla la revelación progresiva de la tinta en el material de la página.
// - Interpola el parámetro _RevealAmount del shader desde 1 (oculto) hasta 0.
//
// Atributos principales:
// - revealMaterial: material del shader que implementa el efecto de revelación.
// - revealDuration: duración en segundos de la animación.
//
// Notas:
// - El parámetro del shader se obtiene con Shader.PropertyToID para optimización.
// - Si se llama a Reveal() antes de terminar, cancela la revelación anterior.
// -----------------------------------------------------------------------------
public class InkRevealController : MonoBehaviour
{
    [SerializeField] private Material revealMaterial;
    [SerializeField] private float revealDuration = 2f;
    private static readonly int RevealAmountID = Shader.PropertyToID("_RevealAmount");

    void Start()
    {
        // Initialize the reveal amount to 1 (fully hidden)
        revealMaterial.SetFloat(RevealAmountID, 1f);
    }

    public void Reveal()
    {
        StopAllCoroutines();
        StartCoroutine(RevealRoutine());
    }

    // -----------------------------------------------------------------------------
    // RevealRoutine
    //
    // - Reinicia _RevealAmount a 1 y lo interpola hasta 0 en revealDuration segundos.
    // - El valor del parámetro del shader se actualiza cada frame.
    // -----------------------------------------------------------------------------
    private IEnumerator RevealRoutine()
    {
        float elapsed = 0f;

        revealMaterial.SetFloat(RevealAmountID, 1f);

        while (elapsed < revealDuration)
        {
            elapsed += Time.deltaTime;

            float t = Mathf.Clamp01(elapsed / revealDuration);
            revealMaterial.SetFloat(RevealAmountID, 1 - t);

            yield return null;
        }

        revealMaterial.SetFloat(RevealAmountID, 0f);
    }
}
