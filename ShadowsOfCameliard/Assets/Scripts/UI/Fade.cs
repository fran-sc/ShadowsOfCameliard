using System;
using System.Collections;
using UnityEngine;

// -----------------------------------------------------------------------------
// Fade
//
// Responsabilidades:
// - Desvanece el SpriteRenderer del objeto al entrar el jugador en su área.
// - Restaura la opacidad original al salir del área.
//
// Atributos principales:
// - fadeAlpha: valor de alpha objetivo al entrar en el área (0 = invisible).
// - fadeDelay: duración de la transición de opacidad.
//
// Notas:
// - Útil para transparentar obstáculos que tapan al jugador (rboles, techos...).
// - La transición es suave usando Mathf.Lerp frame a frame.
// -----------------------------------------------------------------------------
public class Fade : MonoBehaviour
{
    [SerializeField] float fadeDelay;
    
    [Range(0, 1)]
    [SerializeField] float fadeAlpha;

    SpriteRenderer sr;

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            StartCoroutine(FadeTransition(fadeAlpha));
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            StartCoroutine(FadeTransition(1));
        }
    }
    
    // -----------------------------------------------------------------------------
    // FadeTransition
    //
    // - Interpola el alpha del SpriteRenderer desde el valor actual hasta alpha
    //   en fadeDelay segundos, actualizando el color frame a frame.
    // -----------------------------------------------------------------------------
    IEnumerator FadeTransition(float alpha)
    {
        float fadeTime = 0;
        float currentAlpha = sr.color.a;

        while (fadeTime < fadeDelay)
        {
            fadeTime += Time.deltaTime;
            float newAlpha = Mathf.Lerp(currentAlpha, alpha, fadeTime / fadeDelay);
            Color color = new Color(sr.color.r, sr.color.g, sr.color.b, newAlpha);
            sr.color = color;
            yield return null;
        }
    }
}
