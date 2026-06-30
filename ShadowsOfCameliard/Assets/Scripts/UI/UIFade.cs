using System.Collections;
using UnityEngine;
using UnityEngine.UI;


// -----------------------------------------------------------------------------
// UIFade
//
// Responsabilidades:
// - Gestiona los efectos de fundido (fade in/out) de pantalla completa.
// - Expone métodos para fundir la pantalla a negro o recuperarla desde negro.
//
// Atributos principales:
// - fadeImage: imagen UI de pantalla completa sobre la que se interpola el alpha.
// - fadeDuration: duración total del efecto de fundido en segundos.
//
// Notas:
// - Solo puede haber un fundido activo a la vez; iniciar uno nuevo cancela el anterior.
// - La propiedad FadeDuration es de solo lectura y se expone para que otras clases
//   puedan esperar la duración exacta del efecto.
// -----------------------------------------------------------------------------
public class UIFade : PersistentSingleton<UIFade>
{
    [SerializeField] Image fadeImage;
    [SerializeField] float fadeDuration;

    public float FadeDuration => fadeDuration;

    private IEnumerator fadeCoroutine;

    // -----------------------------------------------------------------------------
    // FadeToBlack
    //
    // - Inicia un fundido desde transparente hasta negro completo.
    //
    // Notas:
    // - Cancela cualquier fundido previo antes de iniciar el nuevo.
    // - Fija el alpha inicial a 0 para garantizar que el fundido parte de transparente.
    // -----------------------------------------------------------------------------
    public void FadeToBlack()
    {
        if (fadeCoroutine != null)
        {
            StopCoroutine(fadeCoroutine); // Cancela el fundido en curso para evitar solapamientos
        }

        if (fadeImage == null)
        {
            Debug.LogError("UIFade.FadeToBlack(): fadeImage is not assigned.");
            return;
        }

        Color color = fadeImage.color;
        color.a = 0f;               // Asegura que la imagen arranca totalmente transparente
        fadeImage.color = color;

        fadeCoroutine = Fade(1f);   // Objetivo: alpha 1 (negro opaco)
        StartCoroutine(fadeCoroutine);
    }

    // -----------------------------------------------------------------------------
    // FadeFromBlack
    //
    // - Inicia un fundido desde negro completo hasta transparente.
    //
    // Notas:
    // - Cancela cualquier fundido previo antes de iniciar el nuevo.
    // - Fija el alpha inicial a 1 para garantizar que el fundido parte de opaco.
    // -----------------------------------------------------------------------------
    public void FadeFromBlack()
    {
        if (fadeCoroutine != null)
        {
            StopCoroutine(fadeCoroutine); // Cancela el fundido en curso para evitar solapamientos
        }

        if (fadeImage == null)
        {
            Debug.LogError("UIFade.FadeFromBlack(): fadeImage is not assigned.");
            return;
        }

        Color color = fadeImage.color;
        color.a = 1f;               // Asegura que la imagen arranca totalmente opaca
        fadeImage.color = color;

        fadeCoroutine = Fade(0f);   // Objetivo: alpha 0 (transparente)

        StartCoroutine(fadeCoroutine);
    }

    // -----------------------------------------------------------------------------
    // Fade
    //
    // - Interpola el alpha de fadeImage desde su valor actual hasta targetAlpha
    //   a lo largo de fadeDuration segundos, frame a frame.
    //
    // Notas:
    // - Al finalizar, asigna el alpha exacto para evitar imprecisiones de punto flotante.
    // -----------------------------------------------------------------------------
    private IEnumerator Fade(float targetAlpha)
    {
        if (fadeImage == null)
        {
            Debug.LogError("UIFade.Fade(): fadeImage is not assigned.");
            yield break;
        }

        Color color = fadeImage.color;
        float startAlpha = color.a;
        float time = 0;

        while (time < fadeDuration)
        {
            time += Time.deltaTime;
            // Interpolación lineal del alpha según el tiempo transcurrido
            color.a = Mathf.Lerp(startAlpha, targetAlpha, time / fadeDuration); 
            fadeImage.color = color;
            yield return null;
        }

        // Fija el valor exacto final para evitar imprecisiones de punto flotante
        color.a = targetAlpha; 
        fadeImage.color = color;

        Debug.Log($"UIFade.Fade(): Fade completed. Final alpha: {targetAlpha}");

        yield return null;
    }
}
