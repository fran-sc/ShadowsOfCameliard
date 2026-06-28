using System.Collections;
using UnityEngine;
using UnityEngine.UI;

// -----------------------------------------------------------------------------
// CodexFade
//
// Responsabilidades:
// - Gestiona efectos de fade in/out específicos de la escena del códice.
// - Usa duraciones independientes para el fundido a negro y la recuperación.
//
// Notas:
// - La corrutina activa se cancela antes de iniciar un nuevo fundido.
// - Versión ligera de UIFade, sin guardia de fadeImage nula ni Debug.Log.
// -----------------------------------------------------------------------------
public class CodexFade : MonoBehaviour
{
    [Header("Fade Settings")]
    [SerializeField] float fadeInDuration = 3f;
    [SerializeField] float fadeOutDuration = 3f;
    [SerializeField] Image fadeImage;
        
    IEnumerator fadeCoroutine;

    public void FadeToBlack()
    {
        if (fadeCoroutine != null)
        {
            StopCoroutine(fadeCoroutine);
        }

        Color color = fadeImage.color;
        color.a = 0f;
        fadeImage.color = color;

        fadeCoroutine = Fade(1f, fadeOutDuration);
        StartCoroutine(fadeCoroutine);
    }

    public void FadeFromBlack()
    {
        if (fadeCoroutine != null)
        {
            StopCoroutine(fadeCoroutine);
        }

        Color color = fadeImage.color;
        color.a = 1f;
        fadeImage.color = color;

        fadeCoroutine = Fade(0f, fadeInDuration);

        StartCoroutine(fadeCoroutine);
    }

    // -----------------------------------------------------------------------------
    // Fade
    //
    // - Interpola el alpha de fadeImage desde el valor actual hasta targetAlpha
    //   a lo largo de fadeDuration segundos, frame a frame.
    // -----------------------------------------------------------------------------
    private IEnumerator Fade(float targetAlpha, float fadeDuration)
    {
        Color color = fadeImage.color;
        float startAlpha = color.a;
        float time = 0;

        while (time < fadeDuration)
        {
            time += Time.deltaTime;
            color.a = Mathf.Lerp(startAlpha, targetAlpha, time / fadeDuration);
            fadeImage.color = color;
            yield return null;
        }
    }   
}
